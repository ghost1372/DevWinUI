namespace DevWinUI;

public static partial class DictionaryExtension
{
    extension<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        public Dictionary<TKey, TValue> Clone()
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            return new Dictionary<TKey, TValue>(dictionary);
        }

        public void AddIfNotExists(TKey key, TValue value)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }

        public void Update(TKey key, TValue value)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            dictionary[key] = value;
        }

        public void Update(KeyValuePair<TKey, TValue> pair)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(pair.Key);

            dictionary[pair.Key] = pair.Value;
        }

        public void DeleteIfExistsKey(TKey key)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentNullException.ThrowIfNull(key);

            dictionary.Remove(key);
        }

        public void DeleteIfExistsValue(TValue value)
        {
            ArgumentNullException.ThrowIfNull(dictionary);

            var pair = dictionary.FirstOrDefault(kv => Equals(kv.Value, value));
            if (!Equals(pair, default(KeyValuePair<TKey, TValue>)))
            {
                dictionary.Remove(pair.Key);
            }
        }

        public bool AreValuesNull()
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            return dictionary.All(static kv => kv.Value is null);
        }

        public bool AreKeysNull()
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            return dictionary.All(static kv => kv.Key is null);
        }

        public TKey GetKeyFromValue(TValue value)
        {
            ArgumentNullException.ThrowIfNull(dictionary);

            var pair = dictionary.FirstOrDefault(kv => Equals(kv.Value, value));
            if (Equals(pair, default(KeyValuePair<TKey, TValue>)))
                throw new KeyNotFoundException("Value not found in dictionary.");

            return pair.Key;
        }
    }
}
