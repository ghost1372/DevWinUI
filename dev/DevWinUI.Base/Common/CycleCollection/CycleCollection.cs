using System.Collections;
using System.Collections.Specialized;

namespace DevWinUI;

public partial class CycleCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged, ICycleCollection, ICycleCollection<T>
{
    private readonly ObservableCollection<T> collection;
    private ObservableCollection<T> headers;
    private ObservableCollection<T> footers;
    private int crossLength;

    public int CrossLength => crossLength;
    public CycleCollection(ObservableCollection<T> collection, int crossLength = 2)
    {
        this.collection = collection;
        this.crossLength = crossLength;

        headers = new ObservableCollection<T>();
        footers = new ObservableCollection<T>();

        UpdateCycle();
        UpdateCollection();
        collection.CollectionChanged += ItemsCollectionChanged;
        headers.CollectionChanged += InnerHeader_CollectionChanged;
        footers.CollectionChanged += InnerFooter_CollectionChanged;

    }

    private void InnerHeader_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex));
                break;
            case NotifyCollectionChangedAction.Move:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, e.OldStartingIndex, e.NewStartingIndex));
                break;
            case NotifyCollectionChangedAction.Remove:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex));
                break;
            case NotifyCollectionChangedAction.Replace:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, e.NewStartingIndex));
                break;
            case NotifyCollectionChangedAction.Reset:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                break;
        }
    }

    private void InnerFooter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex + headers.Count + ItemsCount));
                break;
            case NotifyCollectionChangedAction.Move:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, e.OldStartingIndex + headers.Count + collection.Count, e.NewStartingIndex + headers.Count + collection.Count));
                break;
            case NotifyCollectionChangedAction.Remove:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, e.OldStartingIndex + headers.Count + ItemsCount));
                break;
            case NotifyCollectionChangedAction.Replace:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, e.NewStartingIndex + headers.Count + collection.Count));
                break;
            case NotifyCollectionChangedAction.Reset:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                break;
        }
    }

    #region ICycleCollectionProvider
    public int ItemsCount => collection.Count;

    object ICycleCollection.GetItem(int index)
    {
        return collection[index];
    }

    public T GetItem(int index)
    {
        return collection[index];
    }

    public int ConvertToItemIndex(int index)
    {
        if (index < headers.Count)
        {
            return index + ItemsCount - headers.Count;
        }
        else if (index > headers.Count + ItemsCount)
        {
            return index - headers.Count - ItemsCount;
        }
        return index - headers.Count;
    }

    public int ConvertFromItemIndex(int index)
    {
        return index + headers.Count;
    }

    public bool IsCycleItem(int index)
    {
        return IsHeader(index) || IsFooter(index);
    }

    public bool IsCycleItem(T item)
    {
        return IsHeader(item) || IsFooter(item);
    }


    bool ICycleCollection.IsCycleItem(object item)
    {
        return ((ICycleCollection)this).IsHeader(item) || ((ICycleCollection)this).IsFooter(item);

    }

    public bool IsHeader(int index)
    {
        return index < headers.Count;
    }

    public bool IsFooter(int index)
    {
        return index > headers.Count + collection.Count;
    }

    public bool IsHeader(T item)
    {
        return headers.Contains(item);
    }

    public bool IsFooter(T item)
    {
        return footers.Contains(item);
    }

    bool ICycleCollection.IsHeader(object item)
    {
        return IsHeader((T)item);
    }

    bool ICycleCollection.IsFooter(object item)
    {
        return IsFooter((T)item);
    }


    #endregion ICycleCollectionProvider

    #region ICollection

    public int Count => collection.Count + headers.Count + footers.Count;

    public bool IsReadOnly => false;

    public T this[int index]
    {
        get
        {
            if (index < headers.Count)
            {
                return headers[index];
            }
            else if (index < headers.Count + collection.Count)
            {
                return collection[index - headers.Count];
            }
            else
            {
                return footers[index - headers.Count - collection.Count];
            }
        }
        set
        {
            collection[ConvertToItemIndex(index)] = value;
        }
    }

    public void Add(T item)
    {
        collection.Add(item);
    }

    public void Clear()
    {
        collection.Clear();
    }

    public bool Contains(T item)
    {
        return collection.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (arrayIndex < headers.Count)
        {
            headers.CopyTo(array, arrayIndex);
            return;
        }
        for (int i = 0; i < headers.Count; i++)
        {
            array[i] = headers[i];
            arrayIndex--;
        }
        for (int i = 0; i < collection.Count && arrayIndex >= 0; i++, arrayIndex--)
        {
            array[i + headers.Count] = collection[i];
            arrayIndex--;
        }
        for (int i = 0; i < footers.Count && arrayIndex >= 0; i++, arrayIndex--)
        {
            array[i + headers.Count + collection.Count] = footers[i];
            arrayIndex--;
        }
    }

    public bool Remove(T item)
    {
        return collection.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new CycleCollectionEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return ConvertFromItemIndex(collection.IndexOf(item));
    }

    public void Insert(int index, T item)
    {
        collection.Insert(ConvertToItemIndex(index), item);
    }

    public void RemoveAt(int index)
    {
        collection.RemoveAt(ConvertToItemIndex(index));
    }

    #endregion ICollection

    public void Move(int oldIndex, int newIndex)
    {
        collection.Move(ConvertToItemIndex(oldIndex), ConvertToItemIndex(newIndex));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, oldIndex, newIndex));
    }

    #region INotifyChanged

    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
        OnPropertyChanged("Count");
        OnPropertyChanged("ItemsCount");
    }

    protected void OnPropertyChanged([CallerMemberName] string propName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    #endregion INotifyChanged


    private void UpdateCollection()
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

    }

    private void UpdateCycle()
    {
        headers.Clear();
        footers.Clear();

        if (collection.Count == 0) return;
        if (collection.Count == 1)
        {
            for (int i = 0; i < 2; i++)
            {
                headers.Add(collection[0]);
                footers.Add(collection[0]);
            }
        }
        else if (collection.Count < crossLength)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                headers.Add(collection[i]);
                footers.Add(collection[i]);
            }
        }
        else
        {
            for (int i = 0; i < crossLength; i++)
            {
                var reverseIndex = collection.Count - crossLength + i;
                headers.Add(collection[reverseIndex]);
                footers.Add(collection[i]);
            }
        }
    }


    private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, ConvertFromItemIndex(e.NewStartingIndex)));
                break;
            case NotifyCollectionChangedAction.Move:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.OldItems, ConvertFromItemIndex(e.OldStartingIndex), ConvertFromItemIndex(e.NewStartingIndex)));
                break;
            case NotifyCollectionChangedAction.Remove:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, ConvertFromItemIndex(e.OldStartingIndex)));
                break;
            case NotifyCollectionChangedAction.Replace:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, ConvertFromItemIndex(e.NewStartingIndex)));
                break;
            case NotifyCollectionChangedAction.Reset:
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                break;
        }
        UpdateCycle();
    }

    #region IList
    bool IList.IsFixedSize => true;

    bool IList.IsReadOnly => false;

    int ICollection.Count => this.Count;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => null;

    object IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value;
    }


    int IList.Add(object value)
    {
        this.Add((T)value);
        return ConvertFromItemIndex(ItemsCount - 1);
    }

    void IList.Clear()
    {
        this.Clear();
    }

    bool IList.Contains(object value)
    {
        return this.Contains((T)value);
    }

    int IList.IndexOf(object value)
    {
        return this.IndexOf((T)value);
    }

    void IList.Insert(int index, object value)
    {
        this.Insert(index, (T)value);
    }

    void IList.Remove(object value)
    {
        this.Remove((T)value);
    }

    void IList.RemoveAt(int index)
    {
        this.RemoveAt(index);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        var tmp = new T[index];
        this.CopyTo(tmp, index);
        for (int i = 0; i < index; i++)
        {
            array.SetValue(tmp[i], i);
        }
    }

    #endregion IList

    partial class CycleCollectionEnumerator : IEnumerator<T>
    {
        private CycleCollection<T> parent;
        private int position;

        internal CycleCollectionEnumerator(CycleCollection<T> parent)
        {
            this.parent = parent;
            this.position = -1;
        }

        public T Current
        {
            get
            {

                if (position == -1 ||
                    position == parent.Count)
                {
                    throw new InvalidOperationException();
                }
                return parent[position];
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public bool MoveNext()
        {

            if (position != parent.Count)
            {
                position++;
            }
            return position < parent.Count;
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
