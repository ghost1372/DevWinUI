namespace DevWinUI;

[TemplatePart(Name = nameof(PART_TextPanel), Type = typeof(StackPanel))]
[TemplatePart(Name = nameof(PART_ImageContentHolder), Type = typeof(ContentPresenter))]
public partial class OverviewPageHeader : Control
{
    private const string PART_TextPanel = "PART_TextPanel";
    private const string PART_ImageContentHolder = "PART_ImageContentHolder";

    private ContentPresenter imageContentHolder;
    private StackPanel textPanel;

    public object ImageContent
    {
        get => (object)GetValue(ImageContentProperty);
        set => SetValue(ImageContentProperty, value);
    }

    public static readonly DependencyProperty ImageContentProperty =
        DependencyProperty.Register(nameof(ImageContent), typeof(object), typeof(OverviewPageHeader), new PropertyMetadata(defaultValue: null));

    public object ActionContent
    {
        get => (object)GetValue(ActionContentProperty);
        set => SetValue(ActionContentProperty, value);
    }

    public static readonly DependencyProperty ActionContentProperty =
        DependencyProperty.Register(nameof(ActionContent), typeof(object), typeof(OverviewPageHeader), new PropertyMetadata(defaultValue: null));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(OverviewPageHeader), new PropertyMetadata(defaultValue: null));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(OverviewPageHeader), new PropertyMetadata(defaultValue: null));

    public OverviewPageHeader()
    {
        DefaultStyleKey = typeof(OverviewPageHeader);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        textPanel = GetTemplateChild(PART_TextPanel) as StackPanel;
        imageContentHolder = GetTemplateChild(PART_ImageContentHolder) as ContentPresenter;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Calculate if the text + image collide
        if ((textPanel.ActualWidth + imageContentHolder.ActualWidth) >= e.NewSize.Width)
        {
            VisualStateManager.GoToState(this, "NarrowLayout", true);
        }
        else
        {
            VisualStateManager.GoToState(this, "WideLayout", true);
        }
    }
}
