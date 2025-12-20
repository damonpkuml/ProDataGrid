// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;

namespace DataGridSample.ViewModels
{
    /// <summary>
    /// ViewModel for the custom summaries sample page.
    /// </summary>
    public class SummariesCustomViewModel
    {
        public SummariesCustomViewModel()
        {
            Orders = new ObservableCollection<SampleOrder>(SampleOrder.GenerateOrders(50));
            
            // Create custom calculators
            StandardDeviationCalculator = new StandardDeviationCalculator();
            WeightedAverageCalculator = new WeightedAverageCalculator();
            PercentageOfTotalCalculator = new PercentageOfTotalCalculator();
        }

        public ObservableCollection<SampleOrder> Orders { get; }
        
        public IDataGridSummaryCalculator StandardDeviationCalculator { get; }
        public IDataGridSummaryCalculator WeightedAverageCalculator { get; }
        public IDataGridSummaryCalculator PercentageOfTotalCalculator { get; }
    }

    /// <summary>
    /// Custom calculator for standard deviation.
    /// </summary>
    public class StandardDeviationCalculator : IDataGridSummaryCalculator
    {
        public string Name => "Standard Deviation";
        
        public bool SupportsIncremental => false;

        public object? Calculate(IEnumerable items, DataGridColumn column, string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            var values = new System.Collections.Generic.List<decimal>();
            
            foreach (var item in items)
            {
                var value = GetPropertyValue(item, propertyName);
                if (value is IConvertible convertible)
                {
                    try
                    {
                        values.Add(Convert.ToDecimal(convertible));
                    }
                    catch
                    {
                        // Skip non-numeric values
                    }
                }
            }

            if (values.Count < 2)
                return null;

            var mean = values.Average();
            var sumOfSquares = values.Sum(v => (v - mean) * (v - mean));
            var stdDev = Math.Sqrt((double)(sumOfSquares / (values.Count - 1)));
            
            return Math.Round((decimal)stdDev, 2);
        }

        public IDataGridSummaryState? CreateState() => null;

        private static object? GetPropertyValue(object item, string propertyName)
        {
            var property = item.GetType().GetProperty(propertyName);
            return property?.GetValue(item);
        }
    }

    /// <summary>
    /// Custom calculator for weighted average (Quantity * UnitPrice / Sum(Quantity)).
    /// </summary>
    public class WeightedAverageCalculator : IDataGridSummaryCalculator
    {
        public string Name => "Weighted Average";
        
        public bool SupportsIncremental => false;

        public object? Calculate(IEnumerable items, DataGridColumn column, string? propertyName)
        {
            decimal totalWeight = 0;
            decimal weightedSum = 0;
            
            foreach (var item in items)
            {
                if (item is SampleOrder order)
                {
                    totalWeight += order.Quantity;
                    weightedSum += order.Quantity * order.UnitPrice;
                }
            }

            if (totalWeight == 0)
                return null;

            return Math.Round(weightedSum / totalWeight, 2);
        }

        public IDataGridSummaryState? CreateState() => null;
    }

    /// <summary>
    /// Custom calculator showing percentage of filtered items vs total.
    /// </summary>
    public class PercentageOfTotalCalculator : IDataGridSummaryCalculator
    {
        public string Name => "Percentage of Total";
        
        public bool SupportsIncremental => false;

        public object? Calculate(IEnumerable items, DataGridColumn column, string? propertyName)
        {
            int count = 0;
            foreach (var _ in items)
            {
                count++;
            }
            
            // For demo purposes, assume total is 50 items (as created in the ViewModel)
            const int totalItems = 50;
            var percentage = (count * 100.0) / totalItems;
            
            return $"{percentage:F1}%";
        }

        public IDataGridSummaryState? CreateState() => null;
    }
}
