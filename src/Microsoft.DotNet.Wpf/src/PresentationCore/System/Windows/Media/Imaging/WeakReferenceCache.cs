// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Threading;

namespace System.Windows.Media.Imaging;

/// <summary>
/// Provides keyed-caching for different objects stored as weak references.
/// </summary>
/// <remarks>The cache operations are thread-safe.</remarks>
internal sealed class WeakReferenceCache<K, V> where K : notnull
                                               where V : class
{
    /// <summary>
    /// Max number of elements stored in the cache.
    /// </summary>
    private const int MaxCacheSize = 300;

    private readonly Dictionary<K, WeakReference<V>> _cacheStore = new();
    private readonly Lock _cacheLock = new();

    /// <summary>
    /// Adds an object to cache
    /// </summary>
    public void AddToCache(K key, WeakReference<V> value)
    {
        lock (_cacheLock)
        {
            // if entry is already there, exit
            if (_cacheStore.ContainsKey(key))
            {
                return;
            }

            // if the table has reached the max size, try to see if we can reduce its size
            if (_cacheStore.Count == MaxCacheSize)
            {
                foreach (KeyValuePair<K, WeakReference<V>> item in _cacheStore)
                {
                    // if the value is a WeakReference that has been GC'd, remove it
                    if (!item.Value.TryGetTarget(out _))
                    {
                        _cacheStore.Remove(item.Key);
                    }
                }
            }

            // if table is still maxed out, exit
            if (_cacheStore.Count == MaxCacheSize)
            {
                return;
            }

            // add it
            _cacheStore.Add(key, value);
        }
    }

    /// <summary>
    /// Removes an object from cache
    /// </summary>
    public void RemoveFromCache(K key)
    {
        lock (_cacheLock)
        {
            // if entry is there, remove it
            _cacheStore.Remove(key);
        }
    }

    /// <summary>
    /// Attempts to retrieve an object from cache
    /// </summary>
    public bool TryGetValue(K key, out WeakReference<V>? value)
    {
        lock (_cacheLock)
        {
            return _cacheStore.TryGetValue(key, out value);
        }
    }
}
