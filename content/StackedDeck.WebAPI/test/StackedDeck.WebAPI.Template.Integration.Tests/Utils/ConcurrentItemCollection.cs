using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace StackedDeck.WebAPI.Template.Integration.Tests.Utils;

/// <summary>
/// A thread-safe <see cref="ICollection{T}"/>, which serializes every access through a lock.
/// </summary>
/// <remarks>
/// The OpenTelemetry in-memory exporters only accept an <see cref="ICollection{T}"/> &amp; append to it from
/// whichever thread ends a span or collects metrics. Spans end concurrently on request threads while the tests
/// read the collection, which a plain <see cref="List{T}"/> doesn't tolerate.
/// </remarks>
/// <typeparam name="T">The type of the items.</typeparam>
public sealed class ConcurrentItemCollection<T> : ICollection<T>
{
    private readonly Lock syncRoot = new();
    private readonly List<T> items = [];

    /// <inheritdoc />
    public int Count
    {
        get
        {
            lock (syncRoot)
            {
                return items.Count;
            }
        }
    }

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(T item)
    {
        lock (syncRoot)
        {
            items.Add(item);
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (syncRoot)
        {
            items.Clear();
        }
    }

    /// <inheritdoc />
    public bool Contains(T item)
    {
        lock (syncRoot)
        {
            return items.Contains(item);
        }
    }

    /// <inheritdoc />
    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (syncRoot)
        {
            items.CopyTo(array, arrayIndex);
        }
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        lock (syncRoot)
        {
            return items.Remove(item);
        }
    }

    /// <summary>
    /// Copies the items into a point-in-time snapshot, which is safe to enumerate while items keep being added.
    /// </summary>
    /// <returns>The items, in the order they were added.</returns>
    public IReadOnlyList<T> Snapshot()
    {
        lock (syncRoot)
        {
            return [.. items];
        }
    }

    /// <inheritdoc />
    /// <remarks>Enumerates a <see cref="Snapshot"/>, so that concurrent additions never invalidate it.</remarks>
    public IEnumerator<T> GetEnumerator() => Snapshot().GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
