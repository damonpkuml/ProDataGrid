// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Collections;
using DataGridSample.ViewModels;

namespace DataGridSample.ViewModels
{
    /// <summary>
    /// ViewModel for the grouped summaries sample page.
    /// </summary>
    public class SummariesGroupedViewModel
    {
        public SummariesGroupedViewModel()
        {
            var orders = SampleOrder.GenerateOrders(100);
            GroupedView = new DataGridCollectionView(orders);
            GroupedView.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(SampleOrder.Region)));
        }

        public DataGridCollectionView GroupedView { get; }
    }
}
