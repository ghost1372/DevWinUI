namespace DevWinUI;

public static partial class ObservableCollectionExtensions
{
    extension<T>(ObservableCollection<T> collection)
    {
        public ObservableCollection<T> Clone()
        {
            ArgumentNullException.ThrowIfNull(collection);

            return new ObservableCollection<T>(collection);
        }

        public void AddIfNotExists(T value)
        {
            ArgumentNullException.ThrowIfNull(collection);

            if (!collection.Contains(value))
            {
                collection.Add(value);
            }
        }

        public void UpdateValue(T value, T newValue)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(newValue);

            var index = collection.IndexOf(value);
            if (index < 0)
            {
                throw new ArgumentException("The value was not found in the collection.", nameof(value));
            }

            collection[index] = newValue;
        }

        public void DeleteIfExists(T value)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(value);

            if (collection.Contains(value))
            {
                collection.Remove(value);
            }
        }

        public bool AreValuesNull()
        {
            ArgumentNullException.ThrowIfNull(collection);

            return collection.All(static x => x is null);
        }
    }
}
