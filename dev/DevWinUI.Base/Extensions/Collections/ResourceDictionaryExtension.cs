namespace DevWinUI;

public static partial class ResourceDictionaryExtension
{
    extension(Collection<ResourceDictionary> dictionaries)
    {
        public void AddIfNotNull(ResourceDictionary? item)
        {
            if (item is not null)
            {
                dictionaries.Add(item);
            }
        }

        public void RemoveIfNotNull(ResourceDictionary? item)
        {
            if (item is not null)
            {
                dictionaries.Remove(item);
            }
        }

        public void InsertOrReplace(int index, ResourceDictionary item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (dictionaries.Count > index)
                dictionaries[index] = item;
            else
                dictionaries.Insert(index, item);
        }

        public void RemoveAll<T>() where T : ResourceDictionary
        {
            for (int i = dictionaries.Count - 1; i >= 0; i--)
            {
                if (dictionaries[i] is T)
                    dictionaries.RemoveAt(i);
            }
        }

        public void InsertIfNotExists(int index, ResourceDictionary item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!dictionaries.Contains(item))
                dictionaries.Insert(index, item);
        }

        public void AddIfNotExists(ResourceDictionary item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!dictionaries.Any(d => d.Source == item.Source))
                dictionaries.Add(item);
        }

        public void Swap(int index1, int index2)
        {
            if (index1 == index2) return;

            var smallIndex = Math.Min(index1, index2);
            var largeIndex = Math.Max(index1, index2);
            var tmp = dictionaries[smallIndex];
            dictionaries.RemoveAt(smallIndex);
            dictionaries.Insert(largeIndex, tmp);
        }
    }

    extension(IList<ResourceDictionary> dictionaries)
    {
        public void AddIfNotNull(ResourceDictionary? item)
        {
            if (item is not null)
            {
                dictionaries.Add(item);
            }
        }

        public void RemoveIfNotNull(ResourceDictionary? item)
        {
            if (item is not null)
            {
                dictionaries.Remove(item);
            }
        }

        public void InsertOrReplace(int index, ResourceDictionary item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (dictionaries.Count > index)
                dictionaries[index] = item;
            else
                dictionaries.Insert(index, item);
        }

        public void RemoveAll<T>() where T : ResourceDictionary
        {
            for (int i = dictionaries.Count - 1; i >= 0; i--)
            {
                if (dictionaries[i] is T)
                    dictionaries.RemoveAt(i);
            }
        }

        public void InsertIfNotExists(int index, ResourceDictionary item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!dictionaries.Contains(item))
                dictionaries.Insert(index, item);
        }

        public void Swap(int index1, int index2)
        {
            if (index1 == index2) return;

            var smallIndex = Math.Min(index1, index2);
            var largeIndex = Math.Max(index1, index2);
            var tmp = dictionaries[smallIndex];
            dictionaries.RemoveAt(smallIndex);
            dictionaries.Insert(largeIndex, tmp);
        }

        public void AddIfNotExists(ResourceDictionary item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (!dictionaries.Any(d => d.Source == item.Source))
                dictionaries.Add(item);
        }
    }

    extension(ResourceDictionary destination)
    {
        public void CopyFrom(ResourceDictionary source)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);

            if (source.Source != null)
            {
                destination.Source = source.Source;
            }
            else
            {
                // Clone theme dictionaries
                if (source.ThemeDictionaries != null)
                {
                    foreach (var theme in source.ThemeDictionaries)
                    {
                        if (theme.Value is ResourceDictionary themedResource)
                        {
                            var themeDictionary = new ResourceDictionary();
                            themeDictionary.CopyFrom(themedResource);
                            destination.ThemeDictionaries[theme.Key] = themeDictionary;
                        }
                        else
                        {
                            destination.ThemeDictionaries[theme.Key] = theme.Value;
                        }
                    }
                }

                // Clone merged dictionaries
                if (source.MergedDictionaries != null)
                {
                    foreach (var merged in source.MergedDictionaries)
                    {
                        var mergedCopy = new ResourceDictionary();
                        mergedCopy.CopyFrom(merged);
                        destination.MergedDictionaries.Add(mergedCopy);
                    }
                }

                // Clone all key-value contents
                foreach (var item in source)
                {
                    destination[item.Key] = item.Value;
                }
            }
        }
    }
}
