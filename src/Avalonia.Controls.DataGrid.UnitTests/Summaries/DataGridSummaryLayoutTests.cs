// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using Xunit;

namespace Avalonia.Controls.DataGridTests.Summaries;

public class DataGridSummaryLayoutTests
{
    [AvaloniaFact]
    public void SummaryRow_Cells_Include_RowGroupSpacerColumn_When_Grouped()
    {
        var items = new ObservableCollection<Item>
        {
            new() { Name = "A", Group = "G1", Value = 1 },
            new() { Name = "B", Group = "G1", Value = 2 },
            new() { Name = "C", Group = "G2", Value = 3 }
        };

        var view = new DataGridCollectionView(items);
        view.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(Item.Group)));

        var window = new Window
        {
            Width = 400,
            Height = 300
        };
        window.SetThemeStyles();

        var grid = new DataGrid
        {
            AutoGenerateColumns = false,
            ItemsSource = view,
            ShowGroupSummary = true,
            GroupSummaryPosition = DataGridGroupSummaryPosition.Header
        };

        var valueColumn = new DataGridTextColumn
        {
            Header = "Value",
            Binding = new Binding(nameof(Item.Value))
        };
        valueColumn.Summaries.Add(new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Count
        });

        grid.ColumnsInternal.Add(valueColumn);
        grid.ColumnsInternal.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding(nameof(Item.Name))
        });

        window.Content = grid;
        window.Show();
        grid.UpdateLayout();

        try
        {
            Assert.True(grid.ColumnsInternal.RowGroupSpacerColumn.IsRepresented);

            var header = grid.GetVisualDescendants()
                .OfType<DataGridRowGroupHeader>()
                .FirstOrDefault();

            Assert.NotNull(header);
            Assert.NotNull(header!.SummaryRow);
            Assert.Equal(grid.ColumnsItemsInternal.Count, header.SummaryRow.Cells.Count);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void TotalSummaryRow_Invalidates_On_ColumnWidth_Change()
    {
        var items = new ObservableCollection<Item>
        {
            new() { Name = "A", Value = 1 },
            new() { Name = "B", Value = 2 }
        };

        var window = new Window
        {
            Width = 400,
            Height = 300
        };
        window.SetThemeStyles();

        var grid = new DataGrid
        {
            AutoGenerateColumns = false,
            ItemsSource = items,
            ShowTotalSummary = true
        };

        var valueColumn = new DataGridTextColumn
        {
            Header = "Value",
            Binding = new Binding(nameof(Item.Value)),
            Width = new DataGridLength(80)
        };
        valueColumn.Summaries.Add(new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Count
        });

        grid.ColumnsInternal.Add(valueColumn);
        grid.ColumnsInternal.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding(nameof(Item.Name)),
            Width = new DataGridLength(120)
        });

        window.Content = grid;
        window.Show();
        grid.UpdateLayout();

        try
        {
            var summaryRow = grid.TotalSummaryRow;
            Assert.NotNull(summaryRow);
            Assert.NotNull(summaryRow!.CellsPresenter);
            Assert.True(summaryRow.CellsPresenter.IsMeasureValid);

            valueColumn.Width = new DataGridLength(140);

            Assert.False(summaryRow.CellsPresenter.IsMeasureValid);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void SummaryRow_Measure_Does_Not_Return_NaN_When_RowHeaderWidth_Auto()
    {
        var items = new ObservableCollection<Item>
        {
            new() { Name = "A", Value = 1 },
            new() { Name = "B", Value = 2 }
        };

        var window = new Window
        {
            Width = 400,
            Height = 300
        };
        window.SetThemeStyles();

        var grid = new DataGrid
        {
            AutoGenerateColumns = false,
            ItemsSource = items,
            ShowTotalSummary = true,
            HeadersVisibility = DataGridHeadersVisibility.All
        };

        var valueColumn = new DataGridTextColumn
        {
            Header = "Value",
            Binding = new Binding(nameof(Item.Value)),
            Width = new DataGridLength(120)
        };
        valueColumn.Summaries.Add(new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Count
        });

        grid.ColumnsInternal.Add(valueColumn);

        window.Content = grid;
        window.Show();
        grid.UpdateLayout();

        try
        {
            var summaryRow = grid.TotalSummaryRow;
            Assert.NotNull(summaryRow);
            Assert.NotNull(summaryRow!.CellsPresenter);

            var width = summaryRow.CellsPresenter.DesiredSize.Width;
            Assert.False(double.IsNaN(width));
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class Item
    {
        public string Name { get; set; } = string.Empty;

        public string Group { get; set; } = string.Empty;

        public int Value { get; set; }
    }
}
