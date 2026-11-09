namespace DevWinUI;

public abstract partial class RadarElement : DependencyObject
{
    internal event Action? Changed;

    protected static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RadarElement)d).Changed?.Invoke();
    }
}
