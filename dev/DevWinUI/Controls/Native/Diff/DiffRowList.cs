using System.Collections;

namespace DevWinUI;

internal readonly record struct RowRef(int Index, int Hidden);

internal sealed partial class DiffRowList : IList
{
    private readonly RowRef[] _refs;
    private readonly Func<int, bool> _isChange;
    private readonly Func<int, RowRef, object> _create;

    public DiffRowList(RowRef[] refs, Func<int, bool> isChange, Func<int, RowRef, object> create)
    {
        _refs = refs;
        _isChange = isChange;
        _create = create;
    }

    public int Count => _refs.Length;

    public bool IsReadOnly => true;

    public bool IsFixedSize => true;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    public object? this[int index]
    {
        get { return _create(index, _refs[index]); }
        set { throw new NotSupportedException(); }
    }

    public bool IsChange(int index)
    {
        RowRef row = _refs[index];
        return row.Hidden == 0 && _isChange(row.Index);
    }

    public int Add(object? value) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(object? value) => false;

    public int IndexOf(object? value) => -1;

    public void Insert(int index, object? value) => throw new NotSupportedException();

    public void Remove(object? value) => throw new NotSupportedException();

    public void RemoveAt(int index) => throw new NotSupportedException();

    public void CopyTo(Array array, int index)
    {
        for (int i = 0; i < _refs.Length; i++)
        {
            array.SetValue(this[i], index + i);
        }
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < _refs.Length; i++)
        {
            yield return this[i];
        }
    }
}
