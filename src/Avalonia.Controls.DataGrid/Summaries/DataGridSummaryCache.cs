// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Collections;
using System;
using System.Collections.Generic;

namespace Avalonia.Controls
{
    /// <summary>
    /// Cache for storing calculated summary values.
    /// </summary>
    internal class DataGridSummaryCache
    {
        private readonly Dictionary<CacheKey, object?> _cache = new();

        /// <summary>
        /// Gets a cached value.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="description">The summary description.</param>
        /// <param name="scope">The scope (Total or Group).</param>
        /// <param name="group">The group (for group scope).</param>
        /// <returns>The cached value, or null if not found.</returns>
        public object? Get(DataGridColumn column, DataGridSummaryDescription description, DataGridSummaryScope scope, DataGridCollectionViewGroup? group)
        {
            var key = new CacheKey(column, description, scope, group);
            return _cache.TryGetValue(key, out var value) ? value : null;
        }

        /// <summary>
        /// Sets a cached value.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="description">The summary description.</param>
        /// <param name="scope">The scope (Total or Group).</param>
        /// <param name="group">The group (for group scope).</param>
        /// <param name="value">The value to cache.</param>
        public void Set(DataGridColumn column, DataGridSummaryDescription description, DataGridSummaryScope scope, DataGridCollectionViewGroup? group, object? value)
        {
            var key = new CacheKey(column, description, scope, group);
            _cache[key] = value;
        }

        /// <summary>
        /// Checks if a value is cached.
        /// </summary>
        public bool Contains(DataGridColumn column, DataGridSummaryDescription description, DataGridSummaryScope scope, DataGridCollectionViewGroup? group)
        {
            var key = new CacheKey(column, description, scope, group);
            return _cache.ContainsKey(key);
        }

        /// <summary>
        /// Tries to get a cached value.
        /// </summary>
        public bool TryGet(DataGridColumn column, DataGridSummaryDescription description, DataGridSummaryScope scope, DataGridCollectionViewGroup? group, out object? value)
        {
            var key = new CacheKey(column, description, scope, group);
            return _cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Invalidates all cached values.
        /// </summary>
        public void InvalidateAll()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Invalidates cached values for a specific column.
        /// </summary>
        public void InvalidateColumn(DataGridColumn column)
        {
            var keysToRemove = new List<CacheKey>();
            foreach (var key in _cache.Keys)
            {
                if (key.Column == column)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Invalidates cached values for a specific group.
        /// </summary>
        public void InvalidateGroup(DataGridCollectionViewGroup? group)
        {
            var keysToRemove = new List<CacheKey>();
            foreach (var key in _cache.Keys)
            {
                if (key.Group == group)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Invalidates all total scope values.
        /// </summary>
        public void InvalidateTotals()
        {
            var keysToRemove = new List<CacheKey>();
            foreach (var key in _cache.Keys)
            {
                if (key.Scope == DataGridSummaryScope.Total)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// Invalidates all group scope values.
        /// </summary>
        public void InvalidateGroups()
        {
            var keysToRemove = new List<CacheKey>();
            foreach (var key in _cache.Keys)
            {
                if (key.Scope == DataGridSummaryScope.Group)
                {
                    keysToRemove.Add(key);
                }
            }
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            public readonly DataGridColumn Column;
            public readonly DataGridSummaryDescription Description;
            public readonly DataGridSummaryScope Scope;
            public readonly DataGridCollectionViewGroup? Group;

            public CacheKey(DataGridColumn column, DataGridSummaryDescription description, DataGridSummaryScope scope, DataGridCollectionViewGroup? group)
            {
                Column = column;
                Description = description;
                Scope = scope;
                Group = group;
            }

            public bool Equals(CacheKey other)
            {
                return Column == other.Column &&
                       Description == other.Description &&
                       Scope == other.Scope &&
                       Group == other.Group;
            }

            public override bool Equals(object? obj)
            {
                return obj is CacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 17;
                    hash = hash * 31 + (Column?.GetHashCode() ?? 0);
                    hash = hash * 31 + (Description?.GetHashCode() ?? 0);
                    hash = hash * 31 + Scope.GetHashCode();
                    hash = hash * 31 + (Group?.GetHashCode() ?? 0);
                    return hash;
                }
            }
        }
    }
}
