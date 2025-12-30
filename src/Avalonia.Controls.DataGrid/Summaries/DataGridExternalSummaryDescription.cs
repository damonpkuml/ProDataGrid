using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace Avalonia.Controls
{
    /// <summary>
    /// 使用外部汇总描述
    /// </summary>
#if !DATAGRID_INTERNAL
public
#else
    internal
#endif
        class DataGridExternalSummaryDescription : DataGridSummaryDescription
    {
        public override DataGridAggregateType AggregateType => DataGridAggregateType.Custom;

        /// <summary>
        /// 
        /// </summary>
        public string ExternalKey { get; set; }

        public object? DefaultValue { get; set; } = 0;

        public override object? Calculate(IEnumerable items, DataGridColumn column)
        {
            var dic = DataGridExternalSummaryHelper.GetExternalSummary(column.OwningGrid);
            if (dic != null && dic.TryGetValue(ExternalKey, out var value))
            {
                return value;
            }
            return DefaultValue;
        }
    }
}
