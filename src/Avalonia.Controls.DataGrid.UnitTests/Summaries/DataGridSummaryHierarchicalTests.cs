// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Xunit;

namespace Avalonia.Controls.DataGridTests.Summaries;

public class DataGridSummaryHierarchicalTests
{
    [AvaloniaFact]
    public void TotalSummary_Updates_When_Hierarchical_Nodes_Collapse()
    {
        var rootItem = new Item("root");
        rootItem.Children.Add(new Item("child-a"));
        rootItem.Children.Add(new Item("child-b"));

        var model = new HierarchicalModel(new HierarchicalOptions
        {
            ChildrenSelector = o => ((Item)o).Children
        });
        model.SetRoot(rootItem);
        model.Expand(model.Root!);

        var window = new Window
        {
            Width = 400,
            Height = 300
        };
        window.SetThemeStyles();

        var grid = new DataGrid
        {
            HierarchicalModel = model,
            HierarchicalRowsEnabled = true,
            AutoGenerateColumns = false,
            ItemsSource = model.Flattened,
            ShowTotalSummary = true,
            SummaryRecalculationDelayMs = 0
        };

        var column = new DataGridHierarchicalColumn
        {
            Header = "Name",
            Binding = new Binding("Item.Name")
        };
        column.Summaries.Add(new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Count
        });

        grid.ColumnsInternal.Add(column);

        window.Content = grid;
        window.Show();
        grid.UpdateLayout();
        grid.RecalculateSummaries();

        try
        {
            var summaryRow = grid.TotalSummaryRow;
            Assert.NotNull(summaryRow);

            var cell = summaryRow!.Cells.First(c => ReferenceEquals(c.Column, column));
            Assert.Equal(3, Assert.IsType<int>(cell.Value));

            model.Collapse(model.Root!);
            Dispatcher.UIThread.RunJobs();
            grid.UpdateLayout();

            Assert.Equal(1, Assert.IsType<int>(cell.Value));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void TotalSummary_Uses_Item_Path_For_Hierarchical_Nodes()
    {
        var rootItem = new Item("root", size: 10);
        rootItem.Children.Add(new Item("child-a", size: 5));
        rootItem.Children.Add(new Item("child-b", size: 15));

        var model = new HierarchicalModel(new HierarchicalOptions
        {
            ChildrenSelector = o => ((Item)o).Children
        });
        model.SetRoot(rootItem);
        model.Expand(model.Root!);

        var window = new Window
        {
            Width = 400,
            Height = 300
        };
        window.SetThemeStyles();

        var grid = new DataGrid
        {
            HierarchicalModel = model,
            HierarchicalRowsEnabled = true,
            AutoGenerateColumns = false,
            ItemsSource = model.Flattened,
            ShowTotalSummary = true,
            SummaryRecalculationDelayMs = 0
        };

        var column = new DataGridHierarchicalColumn
        {
            Header = "Size",
            Binding = new Binding("Item.Size")
        };
        column.Summaries.Add(new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Sum
        });

        grid.ColumnsInternal.Add(column);

        window.Content = grid;
        window.Show();
        grid.UpdateLayout();
        grid.RecalculateSummaries();

        try
        {
            var summaryRow = grid.TotalSummaryRow;
            Assert.NotNull(summaryRow);

            var cell = summaryRow!.Cells.First(c => ReferenceEquals(c.Column, column));
            Assert.Equal(30m, Assert.IsType<decimal>(cell.Value));
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class Item
    {
        public Item(string name, int size = 0)
        {
            Name = name;
            Size = size;
            Children = new ObservableCollection<Item>();
        }

        public string Name { get; }

        public int Size { get; }

        public ObservableCollection<Item> Children { get; }
    }
}
