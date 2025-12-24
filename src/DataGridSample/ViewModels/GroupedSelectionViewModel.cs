using System.Collections.ObjectModel;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using DataGridSample.Models;
using DataGridSample.Mvvm;

namespace DataGridSample.ViewModels;

public class GroupedSelectionViewModel : ObservableObject
{
    public GroupedSelectionViewModel()
    {
        Items = new ObservableCollection<Country>(Countries.All);
        GroupedView = new DataGridCollectionView(Items);
        GroupedView.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(Country.Region)));

        SelectionModel = new SelectionModel<Country>
        {
            SingleSelect = false,
            Source = GroupedView
        };

        _useVimNavigation = false;
        UpdateGestureOverrides();
    }

    public ObservableCollection<Country> Items { get; }

    public DataGridCollectionView GroupedView { get; }

    public SelectionModel<Country> SelectionModel { get; }

    private bool _useVimNavigation;

    public bool UseVimNavigation
    {
        get => _useVimNavigation;
        set
        {
            if (SetProperty(ref _useVimNavigation, value))
            {
                UpdateGestureOverrides();
            }
        }
    }

    private DataGridKeyboardGestures? _keyboardGestureOverrides;

    public DataGridKeyboardGestures? KeyboardGestureOverrides
    {
        get => _keyboardGestureOverrides;
        set => SetProperty(ref _keyboardGestureOverrides, value);
    }

    private void UpdateGestureOverrides()
    {
        if (_useVimNavigation)
        {
            KeyboardGestureOverrides = new DataGridKeyboardGestures
            {
                MoveDown = new KeyGesture(Key.J),
                MoveUp = new KeyGesture(Key.K)
            };
        }
        else
        {
            KeyboardGestureOverrides = null;
        }
    }
}
