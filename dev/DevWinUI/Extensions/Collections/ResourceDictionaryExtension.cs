using System.Collections.ObjectModel;

namespace DevWinUI;

public static partial class ResourceDictionaryExtension
{
    public static void AddIfNotNull(this Collection<ResourceDictionary> mergedDictionaries, ResourceDictionary item)
    {
        if (item != null)
        {
            mergedDictionaries.Add(item);
        }
    }

    public static void RemoveIfNotNull(this Collection<ResourceDictionary> mergedDictionaries, ResourceDictionary item)
    {
        if (item != null)
        {
            mergedDictionaries.Remove(item);
        }
    }

    public static void InsertOrReplace(this Collection<ResourceDictionary> mergedDictionaries, int index, ResourceDictionary item)
    {
        if (mergedDictionaries.Count > index)
        {
            mergedDictionaries[index] = item;
        }
        else
        {
            mergedDictionaries.Insert(index, item);
        }
    }

    public static void RemoveAll<T>(this Collection<ResourceDictionary> mergedDictionaries) where T : ResourceDictionary
    {
        for (int i = mergedDictionaries.Count - 1; i >= 0; i--)
        {
            if (mergedDictionaries[i] is T)
            {
                mergedDictionaries.RemoveAt(i);
            }
        }
    }

    public static void InsertIfNotExists(this Collection<ResourceDictionary> mergedDictionaries, int index, ResourceDictionary item)
    {
        if (!mergedDictionaries.Contains(item))
        {
            mergedDictionaries.Insert(index, item);
        }
    }

    public static void AddIfNotExists(this Collection<ResourceDictionary> resourceDictionaries, ResourceDictionary item)
    {
        if (!resourceDictionaries.Any(d => d.Source == item.Source))
        {
            resourceDictionaries.Add(item);
        }
    }

    public static void Swap(this Collection<ResourceDictionary> mergedDictionaries, int index1, int index2)
    {
        if (index1 == index2)
        {
            return;
        }

        var smallIndex = Math.Min(index1, index2);
        var largeIndex = Math.Max(index1, index2);
        var tmp = mergedDictionaries[smallIndex];
        mergedDictionaries.RemoveAt(smallIndex);
        mergedDictionaries.Insert(largeIndex, tmp);
    }

    public static void AddIfNotNull(this IList<ResourceDictionary> mergedDictionaries, ResourceDictionary item)
    {
        if (item != null)
        {
            mergedDictionaries.Add(item);
        }
    }

    public static void RemoveIfNotNull(this IList<ResourceDictionary> mergedDictionaries, ResourceDictionary item)
    {
        if (item != null)
        {
            mergedDictionaries.Remove(item);
        }
    }

    public static void InsertOrReplace(this IList<ResourceDictionary> mergedDictionaries, int index, ResourceDictionary item)
    {
        if (mergedDictionaries.Count > index)
        {
            mergedDictionaries[index] = item;
        }
        else
        {
            mergedDictionaries.Insert(index, item);
        }
    }

    public static void RemoveAll<T>(this IList<ResourceDictionary> mergedDictionaries) where T : ResourceDictionary
    {
        for (int i = mergedDictionaries.Count - 1; i >= 0; i--)
        {
            if (mergedDictionaries[i] is T)
            {
                mergedDictionaries.RemoveAt(i);
            }
        }
    }

    public static void InsertIfNotExists(this IList<ResourceDictionary> mergedDictionaries, int index, ResourceDictionary item)
    {
        if (!mergedDictionaries.Contains(item))
        {
            mergedDictionaries.Insert(index, item);
        }
    }

    public static void Swap(this IList<ResourceDictionary> mergedDictionaries, int index1, int index2)
    {
        if (index1 == index2)
        {
            return;
        }

        var smallIndex = Math.Min(index1, index2);
        var largeIndex = Math.Max(index1, index2);
        var tmp = mergedDictionaries[smallIndex];
        mergedDictionaries.RemoveAt(smallIndex);
        mergedDictionaries.Insert(largeIndex, tmp);
    }

    public static void AddIfNotExists(this IList<ResourceDictionary> resourceDictionaries, ResourceDictionary item)
    {
        if (!resourceDictionaries.Any(d => d.Source == item.Source))
        {
            resourceDictionaries.Add(item);
        }
    }

    public static void CopyFrom(this ResourceDictionary destination, ResourceDictionary source)
    {
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
                foreach (var mergedResource in source.MergedDictionaries)
                {
                    var themeDictionary = new ResourceDictionary();
                    themeDictionary.CopyFrom(mergedResource);
                    destination.MergedDictionaries.Add(themeDictionary);
                }
            }

            // Clone all contents
            foreach (var item in source)
            {
                destination[item.Key] = item.Value;
            }
        }
    }
}
