using System.Collections;

namespace DevWinUI;

internal sealed partial class ReadOnlyList<T> : IReadOnlyList<T>
{
    private T[] _items = [];
    private int _count;
    private bool _frozen;

    public ReadOnlyList()
    {
    }

    public ReadOnlyList(int capacity)
    {
        if (capacity > 0)
        {
            _items = new T[capacity];
        }
    }

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _items[index];
        }
    }

    public int Count => _count;

    public void Add(T item)
    {
        if (_frozen)
            throw new InvalidOperationException("The collection is frozen");

        if (_count == _items.Length)
        {
            Array.Resize(ref _items, _items.Length == 0 ? 2 : _items.Length * 2);
        }

        _items[_count++] = item;
    }

    public void Freeze() => _frozen = true;

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

