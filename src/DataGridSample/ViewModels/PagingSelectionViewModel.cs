using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls.Selection;
using DataGridSample.Models;
using DataGridSample.Mvvm;

namespace DataGridSample.ViewModels;

public class PagingSelectionViewModel : ObservableObject
{
    private bool _syncingSelection;
    private int _pageSize = 10;

    public PagingSelectionViewModel()
    {
        Items = new ObservableCollection<Country>(Countries.All);
        ItemsView = new DataGridCollectionView(Items)
        {
            PageSize = _pageSize
        };

        SelectionModel = new SelectionModel<Country>
        {
            SingleSelect = false,
            Source = ItemsView
        };

        SelectedItems = new ObservableCollection<object>();

        SelectionModel.SelectionChanged += OnSelectionChanged;
        ItemsView.PageChanged += (_, __) => RaisePageProperties();

        NextPageCommand = new RelayCommand(_ => ItemsView.MoveToNextPage(), _ => CanMoveToNext());
        PreviousPageCommand = new RelayCommand(_ => ItemsView.MoveToPreviousPage(), _ => CanMoveToPrevious());
        FirstPageCommand = new RelayCommand(_ => ItemsView.MoveToFirstPage(), _ => CanMoveToPrevious());
        LastPageCommand = new RelayCommand(_ => ItemsView.MoveToLastPage(), _ => CanMoveToNext());
        ClearSelectionCommand = new RelayCommand(_ => SelectionModel.Clear());
        SelectAcrossPagesCommand = new RelayCommand(_ => SelectAcrossPages());
    }

    public ObservableCollection<Country> Items { get; }

    public DataGridCollectionView ItemsView { get; }

    public SelectionModel<Country> SelectionModel { get; }

    public ObservableCollection<object> SelectedItems { get; }

    public RelayCommand NextPageCommand { get; }

    public RelayCommand PreviousPageCommand { get; }

    public RelayCommand FirstPageCommand { get; }

    public RelayCommand LastPageCommand { get; }

    public RelayCommand ClearSelectionCommand { get; }

    public RelayCommand SelectAcrossPagesCommand { get; }

    public int PageIndex => ItemsView.PageIndex;

    public int PageCount => ItemsView.PageSize > 0
        ? Math.Max(1, (int)Math.Ceiling((double)ItemsView.ItemCount / ItemsView.PageSize))
        : 1;

    public string PageStatus => $"Page {PageIndex + 1} / {PageCount}";

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value < 1)
            {
                value = 1;
            }

            if (SetProperty(ref _pageSize, value))
            {
                ItemsView.PageSize = _pageSize;
                RaisePageProperties();
            }
        }
    }

    public int SelectedCount => SelectionModel.SelectedItems.Count;

    public string SelectionSummary => $"{SelectedCount} selected (across all pages)";

    private void OnSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs e) =>
        RefreshSelectedItemsFromModel();

    private void SelectAcrossPages()
    {
        _syncingSelection = true;
        try
        {
            using (SelectionModel.BatchUpdate())
            {
                SelectionModel.Clear();

                var indexes = Enumerable.Range(0, Items.Count)
                    .Where(i => i % 7 == 0 || i == Items.Count - 1)
                    .ToList();

                foreach (var index in indexes)
                {
                    SelectionModel.Select(index);
                }
            }
        }
        finally
        {
            _syncingSelection = false;
        }

        RefreshSelectedItemsFromModel();
    }

    private bool CanMoveToNext() =>
        ItemsView.PageSize > 0 && PageIndex < PageCount - 1;

    private bool CanMoveToPrevious() =>
        ItemsView.PageSize > 0 && PageIndex > 0;

    private void RaisePageProperties()
    {
        OnPropertyChanged(nameof(PageIndex));
        OnPropertyChanged(nameof(PageCount));
        OnPropertyChanged(nameof(PageStatus));
        OnPropertyChanged(nameof(PageSize));

        NextPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
        FirstPageCommand.RaiseCanExecuteChanged();
        LastPageCommand.RaiseCanExecuteChanged();
    }

    private void RefreshSelectedItemsFromModel()
    {
        if (_syncingSelection)
        {
            return;
        }

        _syncingSelection = true;
        try
        {
            SelectedItems.Clear();
            foreach (var item in SelectionModel.SelectedItems)
            {
                SelectedItems.Add(item!);
            }
        }
        finally
        {
            _syncingSelection = false;
        }

        OnPropertyChanged(nameof(SelectedCount));
        OnPropertyChanged(nameof(SelectionSummary));
    }
}
