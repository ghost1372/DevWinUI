namespace DevWinUI;

internal partial class DisposableCollector : IDisposable
{
    private bool disposedValue;

    private List<IDisposable> objects = new List<IDisposable>();

    public void Add(IDisposable item)
    {
        objects.Add(item);
    }

    public void Dispose()
    {
        if (!disposedValue)
        {
            var objects = Interlocked.Exchange(ref this.objects, null!);

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                objects[i].Dispose();
            }

            disposedValue = true;
        }
    }

}

internal static partial class DisposableCollectorExtensions
{
    public static T TraceDisposable<T>(this T obj, DisposableCollector disposableCollector) where T : IDisposable
    {
        disposableCollector.Add(obj);
        return obj;
    }
}
