namespace DevWinUI;

public static partial class CollectionExtension
{
    extension<T>(ICollection<T> collection)
    {
        public void AddRange(IEnumerable<T> newItems)
        {
            ArgumentNullException.ThrowIfNull(collection);
            ArgumentNullException.ThrowIfNull(newItems);

            if (collection is List<T> list)
            {
                list.AddRange(newItems);
            }
            else
            {
                foreach (var item in newItems)
                {
                    collection.Add(item);
                }
            }
        }
    }
}
