// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Collections;
using Avalonia.Controls;
using DataGridSample.Mvvm;

namespace DataGridSample.ViewModels
{
    /// <summary>
    /// ViewModel for the filtering with summaries sample page.
    /// </summary>
    public class SummariesFilteringViewModel : ObservableObject
    {
        private readonly SampleOrder[] _allOrders;
        private readonly Func<object?, bool> _filterPredicate;
        private string _filterText = string.Empty;
        private string _selectedRegion = "All";

        public SummariesFilteringViewModel()
        {
            _allOrders = SampleOrder.GenerateOrders(200);
            View = new DataGridCollectionView(_allOrders);
            _filterPredicate = FilterOrders;
            View.Filter = _filterPredicate;

            Regions = new[] { "All", "North", "South", "East", "West" };
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
        }

        public DataGridCollectionView View { get; }

        public string[] Regions { get; }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                {
                    RefreshFilter();
                }
            }
        }

        public string SelectedRegion
        {
            get => _selectedRegion;
            set
            {
                if (SetProperty(ref _selectedRegion, value))
                {
                    RefreshFilter();
                }
            }
        }

        public ICommand ClearFiltersCommand { get; }

        private bool FilterOrders(object? item)
        {
            if (item is not SampleOrder order)
                return false;

            // Region filter
            if (SelectedRegion != "All" && order.Region != SelectedRegion)
                return false;

            // Text filter
            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                var text = FilterText.ToLowerInvariant();
                if (!order.Customer.ToLowerInvariant().Contains(text) &&
                    !order.Product.ToLowerInvariant().Contains(text) &&
                    !order.OrderId.ToString().Contains(text))
                {
                    return false;
                }
            }

            return true;
        }

        private void RefreshFilter()
        {
            if (!Equals(View.Filter, _filterPredicate))
            {
                View.Filter = _filterPredicate;
                return;
            }

            View.Refresh();
        }

        private void ClearFilters()
        {
            FilterText = string.Empty;
            SelectedRegion = "All";
        }
    }
}
