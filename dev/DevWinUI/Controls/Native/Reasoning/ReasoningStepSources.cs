namespace DevWinUI;

public sealed partial class ReasoningStepSources : Control
{
    private static readonly ReasoningTemplate DefaultItemTemplateResources = new();

    private static readonly DataTemplate DefaultItemTemplate =
        (DataTemplate)DefaultItemTemplateResources["DefaultReasoningStepSourcesItemTemplate"];

    public IReadOnlyList<ReasoningSource> Sources
    {
        get { return (IReadOnlyList<ReasoningSource>)GetValue(SourcesProperty); }
        set { SetValue(SourcesProperty, value); }
    }

    public static readonly DependencyProperty SourcesProperty =
        DependencyProperty.Register(nameof(Sources), typeof(IReadOnlyList<ReasoningSource>), typeof(ReasoningStepSources), new PropertyMetadata(Array.Empty<ReasoningSource>()));

    public DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(ReasoningStepSources), new PropertyMetadata(DefaultItemTemplate));

    public ReasoningStepSources()
    {
        DefaultStyleKey = typeof(ReasoningStepSources);
    }
}
