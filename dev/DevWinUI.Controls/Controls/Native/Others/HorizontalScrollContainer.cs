namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ScrollBackBtn), Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_ScrollForwardBtn), Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_Scroller), Type = typeof(ScrollViewer))]
public partial class HorizontalScrollContainer : Control
{
    private const string PART_Scroller = "PART_Scroller";
    private const string PART_ScrollBackBtn = "PART_ScrollBackBtn";
    private const string PART_ScrollForwardBtn = "PART_ScrollForwardBtn";

    private ScrollViewer scroller;
    private Button ScrollBackBtn;
    private Button ScrollForwardBtn;

    public string ScrollBackButtonToolTipText
    {
        get { return (string)GetValue(ScrollBackButtonToolTipTextProperty); }
        set { SetValue(ScrollBackButtonToolTipTextProperty, value); }
    }

    public static readonly DependencyProperty ScrollBackButtonToolTipTextProperty =
        DependencyProperty.Register(nameof(ScrollBackButtonToolTipText), typeof(string), typeof(HorizontalScrollContainer), new PropertyMetadata(null));

    public string ScrollForwardButtonToolTipText
    {
        get { return (string)GetValue(ScrollForwardButtonToolTipTextProperty); }
        set { SetValue(ScrollForwardButtonToolTipTextProperty, value); }
    }

    public static readonly DependencyProperty ScrollForwardButtonToolTipTextProperty =
        DependencyProperty.Register(nameof(ScrollForwardButtonToolTipText), typeof(string), typeof(HorizontalScrollContainer), new PropertyMetadata(null));

    public object Source
    {
        get => (object)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(object), typeof(HorizontalScrollContainer), new PropertyMetadata(null));

    public HorizontalScrollContainer()
    {
        DefaultStyleKey = typeof(HorizontalScrollContainer);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        scroller = GetTemplateChild(PART_Scroller) as ScrollViewer;
        ScrollBackBtn = GetTemplateChild(PART_ScrollBackBtn) as Button;
        ScrollForwardBtn = GetTemplateChild(PART_ScrollForwardBtn) as Button;

        scroller.SizeChanged -= OnScrollerSizeChanged;
        scroller.SizeChanged += OnScrollerSizeChanged;

        scroller.ViewChanging -= OnScrollerViewChanging;
        scroller.ViewChanging += OnScrollerViewChanging;

        ScrollBackBtn.Click -= OnBackBtnClick;
        ScrollBackBtn.Click += OnBackBtnClick;

        ScrollForwardBtn.Click -= OnForwardBtnClick;
        ScrollForwardBtn.Click += OnForwardBtnClick;
    }

    private void OnForwardBtnClick(object sender, RoutedEventArgs e)
    {
        scroller.ChangeView(scroller.HorizontalOffset + scroller.ViewportWidth, null, null);

        // Manually focus to ScrollBackBtn since this button disappears after scrolling to the end.
        ScrollBackBtn.Focus(FocusState.Programmatic);
    }

    private void OnBackBtnClick(object sender, RoutedEventArgs e)
    {
        scroller.ChangeView(scroller.HorizontalOffset - scroller.ViewportWidth, null, null);

        // Manually focus to ScrollForwardBtn since this button disappears after scrolling to the end.
        ScrollForwardBtn.Focus(FocusState.Programmatic);
    }

    private void OnScrollerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
    {
        if (ScrollBackBtn == null || ScrollForwardBtn == null)
            return;

        if (e.FinalView.HorizontalOffset < 1)
        {
            ScrollBackBtn.Visibility = Visibility.Collapsed;
        }
        else if (e.FinalView.HorizontalOffset > 1)
        {
            ScrollBackBtn.Visibility = Visibility.Visible;
        }

        if (e.FinalView.HorizontalOffset > scroller.ScrollableWidth - 1)
        {
            ScrollForwardBtn.Visibility = Visibility.Collapsed;
        }
        else if (e.FinalView.HorizontalOffset < scroller.ScrollableWidth - 1)
        {
            ScrollForwardBtn.Visibility = Visibility.Visible;
        }
    }

    private void OnScrollerSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateScrollButtonsVisibility();
    }

    private void UpdateScrollButtonsVisibility()
    {
        if (scroller == null || ScrollForwardBtn == null)
            return;

        if (scroller.ScrollableWidth > 0)
        {
            ScrollForwardBtn.Visibility = Visibility.Visible;
        }
        else
        {
            ScrollForwardBtn.Visibility = Visibility.Collapsed;
        }
    }
}
