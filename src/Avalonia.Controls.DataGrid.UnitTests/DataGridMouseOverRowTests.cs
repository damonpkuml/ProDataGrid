// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Xunit;

namespace Avalonia.Controls.DataGridTests;

public class DataGridMouseOverRowTests
{
    [AvaloniaFact]
    public void MouseOverRowIndex_Does_Not_Throw_When_Displayed_Element_Is_Not_Row()
    {
        var grid = new DataGrid();

        grid.DisplayData.LoadScrollingSlot(0, new DataGridRowGroupHeader(), updateSlotInformation: true);

        var exception = Record.Exception(() => grid.MouseOverRowIndex = 0);

        Assert.Null(exception);
    }
}
