namespace DevWinUI;

public static partial class ListExtension
{
    extension<T>(IList<T> list)
    {
        public List<T> Clone()
        {
            ArgumentNullException.ThrowIfNull(list);
            return list.ToList();
        }

        public void AddIfNotExists(T value)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(value);

            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        public void UpdateValue(T value, T newValue)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(newValue);

            var index = list.IndexOf(value);
            if (index < 0)
            {
                throw new ArgumentException("The value was not found in the list.", nameof(value));
            }

            list[index] = newValue;
        }

        public void DeleteIfExists(T value)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(value);

            if (list.Contains(value))
            {
                list.Remove(value);
            }
        }

        public bool AreValuesNull()
        {
            ArgumentNullException.ThrowIfNull(list);
            return list.All(static x => x is null);
        }
    }
}
