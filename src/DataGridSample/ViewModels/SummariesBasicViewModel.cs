// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.ObjectModel;
using DataGridSample.Models;

namespace DataGridSample.ViewModels
{
    /// <summary>
    /// ViewModel for the basic summaries sample page.
    /// </summary>
    public class SummariesBasicViewModel
    {
        public SummariesBasicViewModel()
        {
            Orders = new ObservableCollection<SampleOrder>(SampleOrder.GenerateOrders(50));
        }

        public ObservableCollection<SampleOrder> Orders { get; }
    }

    /// <summary>
    /// Sample order model for summaries demo.
    /// </summary>
    public class SampleOrder
    {
        public int OrderId { get; set; }
        public string Customer { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
        public string Region { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public static SampleOrder[] GenerateOrders(int count)
        {
            var customers = new[] { "Acme Corp", "Globex Inc", "Initech", "Umbrella Corp", "Wayne Enterprises", "Stark Industries" };
            var products = new[] { "Widget A", "Gadget B", "Device C", "Component D", "Module E" };
            var regions = new[] { "North", "South", "East", "West" };
            var categories = new[] { "Electronics", "Hardware", "Software", "Services" };

            var orders = new SampleOrder[count];
            var random = new System.Random(42);

            for (int i = 0; i < count; i++)
            {
                orders[i] = new SampleOrder
                {
                    OrderId = 1000 + i,
                    Customer = customers[random.Next(customers.Length)],
                    Product = products[random.Next(products.Length)],
                    Quantity = random.Next(1, 100),
                    UnitPrice = Math.Round((decimal)(random.NextDouble() * 100 + 10), 2),
                    Region = regions[random.Next(regions.Length)],
                    Category = categories[random.Next(categories.Length)]
                };
            }

            return orders;
        }
    }
}
