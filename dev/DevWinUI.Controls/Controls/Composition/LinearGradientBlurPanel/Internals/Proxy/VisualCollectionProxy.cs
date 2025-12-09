namespace DevWinUI;

internal partial class VisualCollectionProxy : IVisualCollection
{
    private readonly VisualCollection collection;

    public VisualCollectionProxy(VisualCollection collection)
    {
        this.collection = collection;
    }

    public object RawObject => collection;

    public void InsertAtTop(IVisual visual)
    {
        collection.InsertAtTop((Visual)visual.RawObject);
    }

    public void RemoveAll()
    {
        collection.RemoveAll();
    }
}
