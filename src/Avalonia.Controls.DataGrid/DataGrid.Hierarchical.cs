// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace Avalonia.Controls
{
    /// <summary>
    /// Hierarchical helpers for DataGrid.
    /// </summary>
    partial class DataGrid
    {
        internal bool TryToggleHierarchicalAtSlot(int slot, bool toggleSubtree = false)
        {
            if (!_hierarchicalRowsEnabled || _hierarchicalAdapter == null)
            {
                return false;
            }

            if (TryHandleGroupSlotAsNode(slot, GroupSlotAction.Toggle, toggleSubtree))
            {
                return true;
            }

            if (!TryGetHierarchicalIndexFromSlot(slot, out var hierarchicalIndex))
            {
                return false;
            }

            if (toggleSubtree)
            {
                var node = _hierarchicalAdapter.NodeAt(hierarchicalIndex);
                RunHierarchicalAction(() =>
                {
                    if (node.IsExpanded)
                    {
                        _hierarchicalAdapter.CollapseAll(node);
                    }
                    else
                    {
                        _hierarchicalAdapter.ExpandAll(node);
                    }
                });
                return true;
            }

            _hierarchicalAdapter.Toggle(hierarchicalIndex);
            return true;
        }

        private bool TryGetHierarchicalIndexFromSlot(int slot, out int hierarchicalIndex)
        {
            hierarchicalIndex = -1;

            if (_hierarchicalAdapter == null || slot < 0)
            {
                return false;
            }

            if (RowGroupHeadersTable != null && RowGroupHeadersTable.Contains(slot))
            {
                return false;
            }

            var rowIndex = RowIndexFromSlot(slot);
            if (rowIndex < 0 || rowIndex >= _hierarchicalAdapter.Count)
            {
                return false;
            }

            hierarchicalIndex = rowIndex;
            return true;
        }

        private bool TryHandleGroupSlotAsNode(int slot, GroupSlotAction action, bool subtree = false)
        {
            if (_hierarchicalModel?.Options.TreatGroupsAsNodes != true)
            {
                return false;
            }

            if (RowGroupHeadersTable == null || !RowGroupHeadersTable.Contains(slot))
            {
                return false;
            }

            var rowGroupInfo = RowGroupHeadersTable.GetValueAt(slot);
            if (rowGroupInfo?.CollectionViewGroup == null)
            {
                return false;
            }

            switch (action)
            {
                case GroupSlotAction.Collapse:
                    if (!rowGroupInfo.IsVisible)
                    {
                        return false;
                    }

                    CollapseRowGroup(rowGroupInfo.CollectionViewGroup, collapseAllSubgroups: subtree);
                    break;
                case GroupSlotAction.Expand:
                    if (rowGroupInfo.IsVisible)
                    {
                        return false;
                    }

                    ExpandRowGroup(rowGroupInfo.CollectionViewGroup, expandAllSubgroups: subtree);
                    break;
                default:
                    if (rowGroupInfo.IsVisible)
                    {
                        CollapseRowGroup(rowGroupInfo.CollectionViewGroup, collapseAllSubgroups: subtree);
                    }
                    else
                    {
                        ExpandRowGroup(rowGroupInfo.CollectionViewGroup, expandAllSubgroups: subtree);
                    }
                    break;
            }

            return true;
        }

        private enum GroupSlotAction
        {
            Toggle,
            Expand,
            Collapse
        }
    }
}
