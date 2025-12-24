using Avalonia.Controls;
using Avalonia.Input;
using DataGridSample.ViewModels;

namespace DataGridSample.Pages
{
    public partial class KeyboardGestureHandlingPage : UserControl
    {
        public KeyboardGestureHandlingPage()
        {
            InitializeComponent();
        }

        private void OnGridKeyDown(object? sender, KeyEventArgs e)
        {
            if (DataContext is not KeyboardGestureHandlingViewModel viewModel)
            {
                return;
            }

            var modifiers = e.KeyModifiers == KeyModifiers.None ? string.Empty : $" ({e.KeyModifiers})";
            var handled = e.Handled ? " handled" : string.Empty;
            viewModel.LastKey = $"{e.Key}{modifiers}{handled}";
        }
    }
}
