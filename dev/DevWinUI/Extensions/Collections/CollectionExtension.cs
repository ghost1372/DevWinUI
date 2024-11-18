using System.Collections.ObjectModel;

namespace DevWinUI;

public static partial class CollectionExtension
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (collection is List<T> list)
        {
            list.AddRange(newItems);
        }
        else
        {
            foreach (var local in newItems)
            {
                collection.Add(local);
            }
        }
    }

    /// <summary>
    /// Clones the observable collection.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <returns>The new collection.</returns>
    public static ObservableCollection<T> Clone<T>(this ObservableCollection<T> collection)
    {
        var collectionToReturn = new ObservableCollection<T>();

        foreach (var val in collection)
        {
            collectionToReturn.Add(val);
        }

        return collectionToReturn;
    }

    /// <summary>
    /// Adds a value if it doesn't exist yet.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="value">The value.</param>
    public static void AddIfNotExists<T>(this ObservableCollection<T> collection, T value)
    {
        CheckObservableCollectionIsNull(collection);

        if (!collection.Contains(value))
        {
            collection.Add(value);
        }
    }

    /// <summary>
    /// Updates a value.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="value">The value.</param>
    /// <param name="newValue">The new value.</param>
    public static void UpdateValue<T>(this ObservableCollection<T> collection, T value, T newValue)
    {
        CheckObservableCollectionAndValueIsNull(collection, value);
        CheckValueIsNull(newValue);
        var index = collection.IndexOf(value);
        collection[index] = newValue;
    }

    /// <summary>
    /// Deletes a value if it exists.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="value">The value.</param>
    public static void DeleteIfExists<T>(this ObservableCollection<T> collection, T value)
    {
        CheckObservableCollectionAndValueIsNull(collection, value);

        if (collection.Contains(value))
        {
            collection.Remove(value);
        }
    }

    /// <summary>
    /// Checks whether all values are <c>null</c> or not.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <returns><c>true</c> if all values are <c>null</c>.</returns>
    public static bool AreValuesNull<T>(this ObservableCollection<T> collection)
    {
        CheckObservableCollectionIsNull(collection);
        return collection.All(x => x == null);
    }

    /// <summary>
    /// Checks whether the collection and the values are <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="value">The value.</param>
    private static void CheckObservableCollectionAndValueIsNull<T>(this ObservableCollection<T> collection, T value)
    {
        CheckObservableCollectionIsNull(collection);
        CheckValueIsNull(value);
    }

    /// <summary>
    /// Checks whether a value is <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="value">The value.</param>
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static void CheckValueIsNull<T>(T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }
    }

    /// <summary>
    /// Checks whether the collection is <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="collection">The collection.</param>
    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private static void CheckObservableCollectionIsNull<T>(this ObservableCollection<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }
    }
}
