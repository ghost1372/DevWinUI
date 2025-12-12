using System.Collections;

namespace DevWinUI;

public interface ICycleCollection : IList
{
    int ItemsCount { get; }

    int ConvertToItemIndex(int index);

    int ConvertFromItemIndex(int index);
    object GetItem(int index);

    bool IsCycleItem(int index);
    bool IsCycleItem(object item);

    bool IsHeader(int index);

    bool IsFooter(int index);

    bool IsHeader(object item);

    bool IsFooter(object item);

    int CrossLength { get; }
}

public interface ICycleCollectionProvider<T>
{
    T GetItem(int index);
    bool IsCycleItem(T item);
    bool IsHeader(T item);

    bool IsFooter(T item);
}
