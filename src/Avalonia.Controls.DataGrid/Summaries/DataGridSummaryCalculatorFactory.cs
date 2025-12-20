// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace Avalonia.Controls
{
    /// <summary>
    /// Factory for creating summary calculators based on aggregate type.
    /// </summary>
    internal class DataGridSummaryCalculatorFactory
    {
        private readonly Dictionary<DataGridAggregateType, IDataGridSummaryCalculator> _calculators;

        public DataGridSummaryCalculatorFactory()
        {
            _calculators = new Dictionary<DataGridAggregateType, IDataGridSummaryCalculator>
            {
                { DataGridAggregateType.Sum, new SumCalculator() },
                { DataGridAggregateType.Average, new AverageCalculator() },
                { DataGridAggregateType.Count, new CountCalculator() },
                { DataGridAggregateType.CountDistinct, new CountDistinctCalculator() },
                { DataGridAggregateType.Min, new MinCalculator() },
                { DataGridAggregateType.Max, new MaxCalculator() },
                { DataGridAggregateType.First, new FirstCalculator() },
                { DataGridAggregateType.Last, new LastCalculator() }
            };
        }

        /// <summary>
        /// Gets the calculator for the specified aggregate type.
        /// </summary>
        /// <param name="aggregateType">The aggregate type.</param>
        /// <returns>The calculator, or null if none found.</returns>
        public IDataGridSummaryCalculator? GetCalculator(DataGridAggregateType aggregateType)
        {
            return _calculators.TryGetValue(aggregateType, out var calculator) ? calculator : null;
        }

        /// <summary>
        /// Registers a custom calculator for an aggregate type.
        /// </summary>
        /// <param name="aggregateType">The aggregate type.</param>
        /// <param name="calculator">The calculator to register.</param>
        public void RegisterCalculator(DataGridAggregateType aggregateType, IDataGridSummaryCalculator calculator)
        {
            _calculators[aggregateType] = calculator;
        }
    }
}
