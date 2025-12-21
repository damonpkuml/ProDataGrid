using System.Globalization;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CoverageReporter;

public sealed class CoverageReportTask : Microsoft.Build.Utilities.Task
{
    [Required]
    public ITaskItem[] InputFiles { get; set; } = Array.Empty<ITaskItem>();

    public int Limit { get; set; }

    public string? SearchDirectory { get; set; }

    public bool LogAsWarning { get; set; }

    public bool LogToConsole { get; set; }

    public int FileColumnWidth { get; set; } = 64;

    public override bool Execute()
    {
        var success = true;
        var inputs = InputFiles.Where(item => !string.IsNullOrWhiteSpace(item.ItemSpec)).ToList();

        var searchDirectory = SearchDirectory;
        if (inputs.Count == 0 && !string.IsNullOrWhiteSpace(searchDirectory))
        {
            inputs = FindNewestCoverageFile(searchDirectory!)
                .Select(path => new TaskItem(path))
                .Cast<ITaskItem>()
                .ToList();
        }

        if (inputs.Count == 0)
        {
            LogMessage("Coverage report: no coverage files found.");
            return true;
        }

        foreach (var input in inputs)
        {
            var path = input.ItemSpec;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Log.LogWarning($"Coverage file not found: {path}");
                success = false;
                continue;
            }

            try
            {
                var rows = BuildRows(path);
                WriteTable(rows, Limit);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex, showStackTrace: false);
                success = false;
            }
        }

        return success && !Log.HasLoggedErrors;
    }

    private static List<CoverageRow> BuildRows(string coveragePath)
    {
        var doc = XDocument.Load(coveragePath);
        var fileStats = new Dictionary<string, CoverageStats>(StringComparer.OrdinalIgnoreCase);

        foreach (var classNode in doc.Descendants("class"))
        {
            var filename = classNode.Attribute("filename")?.Value;
            if (string.IsNullOrWhiteSpace(filename))
            {
                continue;
            }

            var key = filename!;
            if (!fileStats.TryGetValue(key, out var stats))
            {
                stats = new CoverageStats();
                fileStats[key] = stats;
            }

            foreach (var lineNode in classNode.Descendants("line"))
            {
                stats.LinesValid++;
                var hits = ParseInt(lineNode.Attribute("hits")?.Value);
                if (hits > 0)
                {
                    stats.LinesCovered++;
                }

                var conditionCoverage = lineNode.Attribute("condition-coverage")?.Value;
                if (!string.IsNullOrWhiteSpace(conditionCoverage))
                {
                    var condition = conditionCoverage!;
                    if (TryParseConditionCoverage(condition, out var branchCovered, out var branchTotal))
                    {
                        stats.BranchesCovered += branchCovered;
                        stats.BranchesValid += branchTotal;
                    }
                }
            }
        }

        return fileStats
            .Select(kvp => new CoverageRow(kvp.Key, kvp.Value))
            .OrderBy(row => row.LineRate)
            .ThenBy(row => row.File, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void WriteTable(List<CoverageRow> rows, int limit)
    {
        if (limit > 0)
        {
            rows = rows.Take(limit).ToList();
        }

        if (rows.Count == 0)
        {
            LogMessage("Coverage report: no files found.");
            return;
        }

        var tableLines = BuildTableLines(rows, includePaddingLines: false, fileColumnWidth: FileColumnWidth);
        if (LogToConsole)
        {
            Console.WriteLine("Coverage report (per file):");
            foreach (var line in tableLines)
            {
                Console.WriteLine(line);
            }
            return;
        }

        LogMessage("Coverage report (per file):");
        if (LogAsWarning)
        {
            foreach (var line in tableLines)
            {
                Log.LogWarning(line);
            }
            return;
        }

        foreach (var line in tableLines)
        {
            LogMessage(line);
        }
    }

    private void LogMessage(string message)
    {
        Log.LogMessage(MessageImportance.High, message);
    }

    private static IEnumerable<string> FindNewestCoverageFile(string searchDirectory)
    {
        if (string.IsNullOrWhiteSpace(searchDirectory) || !Directory.Exists(searchDirectory))
        {
            return Array.Empty<string>();
        }

        var files = Directory.GetFiles(searchDirectory, "coverage.cobertura.xml", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            return Array.Empty<string>();
        }

        var newest = files
            .Select(path => new FileInfo(path))
            .OrderByDescending(info => info.LastWriteTimeUtc)
            .First();

        return new[] { newest.FullName };
    }

    private static int ParseInt(string? value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    private static bool TryParseConditionCoverage(string value, out int covered, out int total)
    {
        covered = 0;
        total = 0;

        var openParen = value.IndexOf('(');
        var closeParen = value.IndexOf(')', openParen + 1);
        if (openParen < 0 || closeParen < 0)
        {
            return false;
        }

        var fraction = value.Substring(openParen + 1, closeParen - openParen - 1);
        var slash = fraction.IndexOf('/');
        if (slash < 0)
        {
            return false;
        }

        var coveredPart = fraction.Substring(0, slash);
        var totalPart = fraction.Substring(slash + 1);

        covered = ParseInt(coveredPart);
        total = ParseInt(totalPart);
        return total > 0;
    }

    private static string PadLeft(string value, int width) => value.PadLeft(width);
    private static string PadRight(string value, int width) => value.PadRight(width);

    private static List<string> BuildTableLines(List<CoverageRow> rows, bool includePaddingLines, int fileColumnWidth)
    {
        var linePctWidth = Math.Max("Line%".Length, rows.Max(r => r.LineRateText.Length));
        var linesWidth = Math.Max("Lines".Length, rows.Max(r => r.LineCountText.Length));
        var branchPctWidth = Math.Max("Branch%".Length, rows.Max(r => r.BranchRateText.Length));
        var branchesWidth = Math.Max("Branches".Length, rows.Max(r => r.BranchCountText.Length));
        var fileWidthLimit = fileColumnWidth;
        var displayRows = rows.Select(row => new
        {
            Row = row,
            File = TruncatePath(row.File, fileWidthLimit)
        }).ToList();
        var fileWidth = Math.Max("File".Length, displayRows.Max(r => r.File.Length));

        var lines = new List<string>();
        if (includePaddingLines)
        {
            lines.Add(string.Empty);
        }
        lines.Add($"| {PadRight("Line%", linePctWidth)} | {PadRight("Lines", linesWidth)} | {PadRight("Branch%", branchPctWidth)} | {PadRight("Branches", branchesWidth)} | {PadRight("File", fileWidth)} |");
        lines.Add($"|-{new string('-', linePctWidth)}-|-{new string('-', linesWidth)}-|-{new string('-', branchPctWidth)}-|-{new string('-', branchesWidth)}-|-{new string('-', fileWidth)}-|");

        lines.AddRange(displayRows.Select(entry =>
            $"| {PadLeft(entry.Row.LineRateText, linePctWidth)} | {PadLeft(entry.Row.LineCountText, linesWidth)} | {PadLeft(entry.Row.BranchRateText, branchPctWidth)} | {PadLeft(entry.Row.BranchCountText, branchesWidth)} | {PadRight(entry.File, fileWidth)} |"));

        if (includePaddingLines)
        {
            lines.Add(string.Empty);
        }
        return lines;
    }

    private static string TruncatePath(string path, int maxWidth)
    {
        if (maxWidth <= 0 || path.Length <= maxWidth)
        {
            return path;
        }

        if (maxWidth <= 3)
        {
            return path.Substring(path.Length - maxWidth);
        }

        return "..." + path.Substring(path.Length - (maxWidth - 3));
    }

    private sealed class CoverageStats
    {
        public int LinesCovered { get; set; }
        public int LinesValid { get; set; }
        public int BranchesCovered { get; set; }
        public int BranchesValid { get; set; }
    }

    private sealed class CoverageRow
    {
        public CoverageRow(string file, CoverageStats stats)
        {
            File = file;
            LinesCovered = stats.LinesCovered;
            LinesValid = stats.LinesValid;
            BranchesCovered = stats.BranchesCovered;
            BranchesValid = stats.BranchesValid;
        }

        public string File { get; }
        public int LinesCovered { get; }
        public int LinesValid { get; }
        public int BranchesCovered { get; }
        public int BranchesValid { get; }

        public double LineRate => LinesValid == 0 ? 1.0 : (double)LinesCovered / LinesValid;
        public double BranchRate => BranchesValid == 0 ? 1.0 : (double)BranchesCovered / BranchesValid;

        public string LineRateText => FormatPercent(LineRate);
        public string BranchRateText => BranchesValid == 0 ? "-" : FormatPercent(BranchRate);
        public string LineCountText => $"{LinesCovered}/{LinesValid}";
        public string BranchCountText => BranchesValid == 0 ? "-" : $"{BranchesCovered}/{BranchesValid}";

        private static string FormatPercent(double rate)
        {
            var value = Math.Round(rate * 100, 2).ToString("0.00", CultureInfo.InvariantCulture);
            return $"{value}%";
        }
    }
}
