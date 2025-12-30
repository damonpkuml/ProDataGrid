using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
#if !DATAGRID_INTERNAL
public
#else
    internal
#endif
        class DataGridExternalSummaryHelper
    {
        public static readonly StyledProperty<Dictionary<string, object>> ExternalSummaryProperty =
            AvaloniaProperty.RegisterAttached<DataGridExternalSummaryHelper, DataGrid, Dictionary<string, object>>(
                "ExternalSummary");

        public static void SetExternalSummary(DataGrid element, Dictionary<string, object> value)
        {
            element.SetValue(ExternalSummaryProperty, value);
        }

        public static Dictionary<string, object> GetExternalSummary(DataGrid element)
        {
            return element.GetValue(ExternalSummaryProperty);
        }

        static DataGridExternalSummaryHelper()
        {
            ExternalSummaryProperty.Changed.AddClassHandler<DataGrid>((x, e) =>
            {
                x.InvalidateSummaries();
            });
        }
    }
}
