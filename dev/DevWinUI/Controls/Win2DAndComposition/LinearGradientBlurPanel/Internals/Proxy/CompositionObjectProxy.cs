namespace DevWinUI;

internal partial class CompositionObjectProxy : ICompositionObject
{
    private bool disposedValue;

    private CompositionObject compositionObject;
    private CompositionPropertySetProxy? properties;

    public CompositionObjectProxy(CompositionObject compositionObject)
    {
        this.compositionObject = compositionObject;
    }

    protected bool IsDisposed => disposedValue;

    public ICompositionPropertySet Properties => disposedValue ? null! : (properties ??= new CompositionPropertySetProxy(compositionObject.Properties));

    public object RawObject => compositionObject;

    public void StartAnimation(string propertyName, ICompositionAnimation animation)
    {
        compositionObject.StartAnimation(propertyName, (CompositionAnimation)animation.RawObject);
    }

    public void StopAnimation(string propertyName)
    {
        compositionObject.StopAnimation(propertyName);
    }

    protected virtual void DisposeCore() { }

    public void Dispose()
    {
        if (!disposedValue)
        {
            disposedValue = true;

            DisposeCore();

            properties = null!;

            compositionObject?.Dispose();
            compositionObject = null!;
        }
    }
}
