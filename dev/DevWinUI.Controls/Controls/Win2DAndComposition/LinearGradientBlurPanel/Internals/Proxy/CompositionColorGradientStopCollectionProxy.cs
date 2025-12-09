namespace DevWinUI;

internal partial class CompositionColorGradientStopCollectionProxy : ICompositionColorGradientStopCollection
{
    private readonly CompositionColorGradientStopCollection collection;

    public CompositionColorGradientStopCollectionProxy(CompositionColorGradientStopCollection collection)
    {
        this.collection = collection;
    }

    public ICompositionColorGradientStop this[int index] => new CompositionColorGradientStopProxy(collection[index]);

    public object RawObject => collection;

    public void Add(ICompositionColorGradientStop stop)
    {
        collection.Add((CompositionColorGradientStop)stop.RawObject);
    }

    public void Clear()
    {
        collection.Clear();
    }

    public void Remove(ICompositionColorGradientStop stop)
    {
        collection.Remove((CompositionColorGradientStop?)stop?.RawObject);
    }
}
