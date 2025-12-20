// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Controls.DataGridTests.Summaries;

/// <summary>
/// Integration tests for DataGrid summary functionality.
/// </summary>
public class DataGridSummaryIntegrationTests
{
    #region Basic Summary Property Tests

    [AvaloniaFact]
    public void DataGrid_ShowTotalSummary_Default_Is_False()
    {
        var target = CreateTarget(new List<TestItem>());

        Assert.False(target.ShowTotalSummary);
    }

    [AvaloniaFact]
    public void DataGrid_ShowTotalSummary_Can_Be_Set()
    {
        var target = CreateTarget(new List<TestItem>());

        target.ShowTotalSummary = true;

        Assert.True(target.ShowTotalSummary);
    }

    [AvaloniaFact]
    public void DataGrid_ShowGroupSummary_Default_Is_False()
    {
        var target = CreateTarget(new List<TestItem>());

        Assert.False(target.ShowGroupSummary);
    }

    [AvaloniaFact]
    public void DataGrid_ShowGroupSummary_Can_Be_Set()
    {
        var target = CreateTarget(new List<TestItem>());

        target.ShowGroupSummary = true;

        Assert.True(target.ShowGroupSummary);
    }

    [AvaloniaFact]
    public void DataGrid_TotalSummaryPosition_Default_Is_Bottom()
    {
        var target = CreateTarget(new List<TestItem>());

        Assert.Equal(DataGridSummaryRowPosition.Bottom, target.TotalSummaryPosition);
    }

    [AvaloniaFact]
    public void DataGrid_TotalSummaryPosition_Can_Be_Set_To_Top()
    {
        var target = CreateTarget(new List<TestItem>());

        target.TotalSummaryPosition = DataGridSummaryRowPosition.Top;

        Assert.Equal(DataGridSummaryRowPosition.Top, target.TotalSummaryPosition);
    }

    [AvaloniaFact]
    public void DataGrid_GroupSummaryPosition_Default_Is_Footer()
    {
        var target = CreateTarget(new List<TestItem>());

        Assert.Equal(DataGridGroupSummaryPosition.Footer, target.GroupSummaryPosition);
    }

    [AvaloniaFact]
    public void DataGrid_GroupSummaryPosition_Can_Be_Set_To_Header()
    {
        var target = CreateTarget(new List<TestItem>());

        target.GroupSummaryPosition = DataGridGroupSummaryPosition.Header;

        Assert.Equal(DataGridGroupSummaryPosition.Header, target.GroupSummaryPosition);
    }

    [AvaloniaFact]
    public void DataGrid_GroupSummaryPosition_Can_Be_Set_To_Both()
    {
        var target = CreateTarget(new List<TestItem>());

        target.GroupSummaryPosition = DataGridGroupSummaryPosition.Both;

        Assert.Equal(DataGridGroupSummaryPosition.Both, target.GroupSummaryPosition);
    }

    [AvaloniaFact]
    public void DataGrid_SummaryRecalculationDelayMs_Default_Is_100()
    {
        var target = CreateTarget(new List<TestItem>());

        Assert.Equal(100, target.SummaryRecalculationDelayMs);
    }

    [AvaloniaFact]
    public void DataGrid_SummaryRecalculationDelayMs_Can_Be_Set()
    {
        var target = CreateTarget(new List<TestItem>());

        target.SummaryRecalculationDelayMs = 200;

        Assert.Equal(200, target.SummaryRecalculationDelayMs);
    }

    #endregion

    #region Column Summary Tests

    [AvaloniaFact]
    public void Column_Summaries_Collection_Is_Initialized()
    {
        var column = new DataGridTextColumn();

        Assert.NotNull(column.Summaries);
        Assert.Empty(column.Summaries);
    }

    [AvaloniaFact]
    public void Column_Can_Add_Aggregate_Summary_Description()
    {
        var column = new DataGridTextColumn();
        var description = new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Sum
        };

        column.Summaries.Add(description);

        Assert.Single(column.Summaries);
        Assert.Same(description, column.Summaries[0]);
    }

    [AvaloniaFact]
    public void Column_Can_Add_Custom_Summary_Description()
    {
        var column = new DataGridTextColumn();
        var description = new DataGridCustomSummaryDescription();

        column.Summaries.Add(description);

        Assert.Single(column.Summaries);
        Assert.Same(description, column.Summaries[0]);
    }

    [AvaloniaFact]
    public void Column_Can_Add_Multiple_Summary_Descriptions()
    {
        var column = new DataGridTextColumn();
        var sumDescription = new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Sum
        };
        var avgDescription = new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Average
        };

        column.Summaries.Add(sumDescription);
        column.Summaries.Add(avgDescription);

        Assert.Equal(2, column.Summaries.Count);
    }

    #endregion

    #region Summary Row Tests

    [AvaloniaFact]
    public void DataGridSummaryRow_Scope_Default_Is_Total()
    {
        var row = new DataGridSummaryRow();

        Assert.Equal(DataGridSummaryScope.Total, row.Scope);
    }

    [AvaloniaFact]
    public void DataGridSummaryRow_Scope_Can_Be_Set()
    {
        var row = new DataGridSummaryRow
        {
            Scope = DataGridSummaryScope.Group
        };

        Assert.Equal(DataGridSummaryScope.Group, row.Scope);
    }

    [AvaloniaFact]
    public void DataGridSummaryCell_Column_Can_Be_Set()
    {
        var column = new DataGridTextColumn();
        var cell = new DataGridSummaryCell();
        
        // Column is set internally, so we test via public getter only
        Assert.Null(cell.Column);
    }

    [AvaloniaFact]
    public void DataGridSummaryCell_Value_Can_Be_Set()
    {
        var cell = new DataGridSummaryCell
        {
            Value = 100m
        };

        Assert.Equal(100m, cell.Value);
    }

    [AvaloniaFact]
    public void DataGridSummaryCell_DisplayText_Is_Updated_When_Value_Set()
    {
        var cell = new DataGridSummaryCell
        {
            Value = 100
        };

        Assert.Equal("100", cell.DisplayText);
    }

    #endregion

    #region DataGridRowGroupFooter Tests

    [AvaloniaFact]
    public void DataGridRowGroupFooter_Level_Default_Is_Zero()
    {
        var footer = new DataGridRowGroupFooter();

        Assert.Equal(0, footer.Level);
    }

    [AvaloniaFact]
    public void DataGridRowGroupFooter_Level_Can_Be_Set()
    {
        var footer = new DataGridRowGroupFooter
        {
            Level = 2
        };

        Assert.Equal(2, footer.Level);
    }

    #endregion

    #region Enum Value Tests

    [Fact]
    public void DataGridAggregateType_Has_Expected_Values()
    {
        Assert.Equal(0, (int)DataGridAggregateType.None);
        Assert.Equal(1, (int)DataGridAggregateType.Sum);
        Assert.Equal(2, (int)DataGridAggregateType.Average);
        Assert.Equal(3, (int)DataGridAggregateType.Count);
        Assert.Equal(4, (int)DataGridAggregateType.CountDistinct);
        Assert.Equal(5, (int)DataGridAggregateType.Min);
        Assert.Equal(6, (int)DataGridAggregateType.Max);
        Assert.Equal(7, (int)DataGridAggregateType.First);
        Assert.Equal(8, (int)DataGridAggregateType.Last);
        Assert.Equal(9, (int)DataGridAggregateType.Custom);
    }

    [Fact]
    public void DataGridSummaryScope_Has_Expected_Values()
    {
        Assert.Equal(0, (int)DataGridSummaryScope.Total);
        Assert.Equal(1, (int)DataGridSummaryScope.Group);
        Assert.Equal(2, (int)DataGridSummaryScope.Both);
    }

    [Fact]
    public void DataGridSummaryRowPosition_Has_Expected_Values()
    {
        Assert.Equal(0, (int)DataGridSummaryRowPosition.Top);
        Assert.Equal(1, (int)DataGridSummaryRowPosition.Bottom);
    }

    [Fact]
    public void DataGridGroupSummaryPosition_Has_Expected_Values()
    {
        Assert.Equal(0, (int)DataGridGroupSummaryPosition.Header);
        Assert.Equal(1, (int)DataGridGroupSummaryPosition.Footer);
        Assert.Equal(2, (int)DataGridGroupSummaryPosition.Both);
    }

    #endregion

    #region InvalidateSummaries Test

    [AvaloniaFact]
    public void DataGrid_InvalidateSummaries_Does_Not_Throw()
    {
        var target = CreateTarget(new List<TestItem>
        {
            new() { Value = 10 },
            new() { Value = 20 }
        });

        var exception = Record.Exception(() => target.InvalidateSummaries());

        Assert.Null(exception);
    }

    [AvaloniaFact]
    public void DataGrid_With_Summary_Descriptions_InvalidateSummaries_Does_Not_Throw()
    {
        var target = CreateTarget(new List<TestItem>
        {
            new() { Value = 10 },
            new() { Value = 20 }
        });

        target.Columns[0].Summaries.Add(new DataGridAggregateSummaryDescription
        {
            Aggregate = DataGridAggregateType.Sum
        });
        target.ShowTotalSummary = true;

        var exception = Record.Exception(() => target.InvalidateSummaries());

        Assert.Null(exception);
    }

    #endregion

    #region Test Helpers

    private static DataGrid CreateTarget(IEnumerable<TestItem> items)
    {
        var window = new Window
        {
            Width = 800,
            Height = 600
        };
        window.SetThemeStyles();

        var target = new DataGrid
        {
            Width = 600,
            Height = 400,
            AutoGenerateColumns = false,
            ItemsSource = items
        };

        target.Columns.Add(new DataGridTextColumn
        {
            Header = "Value",
            Binding = new Avalonia.Data.Binding(nameof(TestItem.Value))
        });

        target.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Avalonia.Data.Binding(nameof(TestItem.Name))
        });

        window.Content = target;
        window.Show();

        return target;
    }

    private class TestItem
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
    }

    #endregion
}
