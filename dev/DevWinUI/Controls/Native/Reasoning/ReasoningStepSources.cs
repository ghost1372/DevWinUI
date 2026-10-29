namespace DevWinUI;

public sealed partial class ReasoningStepSources : Control
{
    public IReadOnlyList<ReasoningSource> Sources
    {
        get { return (IReadOnlyList<ReasoningSource>)GetValue(SourcesProperty); }
        set { SetValue(SourcesProperty, value); }
    }

    public static readonly DependencyProperty SourcesProperty =
        DependencyProperty.Register(nameof(Sources), typeof(IReadOnlyList<ReasoningSource>), typeof(ReasoningStepSources), new PropertyMetadata(Array.Empty<ReasoningSource>()));

    public ReasoningStepSources()
    {
        DefaultStyleKey = typeof(ReasoningStepSources);
    }
}
