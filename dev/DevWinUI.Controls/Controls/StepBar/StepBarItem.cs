namespace DevWinUI;
public partial class StepBarItem : ContentControl
{
    private const string ContentPresenterElement = "PART_Content";
    private const string BorderElement = "PART_Border";
    private const string PanelElement = "PART_Panel";
    private ContentPresenter contentPresenter;
    private Border border;
    private StackPanel panel;
    internal DataTemplate ItemTemplate
    {
        get { return (DataTemplate)GetValue(ItemTemplateProperty); }
        set { SetValue(ItemTemplateProperty, value); }
    }

    internal static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(StepBarItem), new PropertyMetadata(default(DataTemplate)));

    public int Index
    {
        get => (int)GetValue(IndexProperty);
        internal set => SetValue(IndexProperty, value);
    }
    public static readonly DependencyProperty IndexProperty =
        DependencyProperty.Register(nameof(Index), typeof(int), typeof(StepBarItem), new PropertyMetadata(-1));

    public StepProgressState ProgressState
    {
        get => (StepProgressState)GetValue(ProgressStateProperty);
        internal set => SetValue(ProgressStateProperty, value);
    }
    public static readonly DependencyProperty ProgressStateProperty =
        DependencyProperty.Register(nameof(ProgressState), typeof(StepProgressState), typeof(StepBarItem), new PropertyMetadata(StepProgressState.Waiting, OnStatusChanged));

    public StepStatus Status
    {
        get => (StepStatus)GetValue(StatusProperty);
        internal set => SetValue(StatusProperty, value);
    }
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(StepStatus), typeof(StepBarItem), new PropertyMetadata(StepStatus.Info, OnStatusChanged));

    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBarItem)d;
        if (ctl != null)
        {
            ctl.UpdateVisualStates();
        }
    }

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        internal set => SetValue(OrientationProperty, value);
    }
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(StepBarItem), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBarItem)d;
        if (ctl != null)
        {
            ctl.UpdateHeaderDisplayMode();
        }
    }

    public StepBarHeaderDisplayMode HeaderDisplayMode
    {
        get { return (StepBarHeaderDisplayMode)GetValue(HeaderDisplayModeProperty); }
        internal set { SetValue(HeaderDisplayModeProperty, value); }
    }

    public static readonly DependencyProperty HeaderDisplayModeProperty =
        DependencyProperty.Register(nameof(HeaderDisplayMode), typeof(StepBarHeaderDisplayMode), typeof(StepBarItem), new PropertyMetadata(StepBarHeaderDisplayMode.Bottom, OnHeaderDisplayModeChanged));

    private static void OnHeaderDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBarItem)d;
        if (ctl != null)
        {
            ctl.UpdateHeaderDisplayMode();
        }
    }
    public void UpdateVisualStates()
    {
        string visualState = ProgressState switch
        {
            StepProgressState.Waiting => "Waiting",
            StepProgressState.UnderWay => Status.ToString(),
            StepProgressState.Complete => Status.ToString(),
            _ => "Waiting"
        };

        VisualStateManager.GoToState(this, visualState, true);

    }

    private void UpdateHeaderDisplayMode()
    {
        if (contentPresenter == null || panel == null)
        {
            return;
        }
        if (Orientation == Orientation.Horizontal)
        {
            switch (HeaderDisplayMode)
            {
                case StepBarHeaderDisplayMode.Top:
                    contentPresenter.Margin = new Thickness(0, 0, 0, 5);
                    panel.Children.Clear();
                    panel.Children.Add(contentPresenter);
                    panel.Children.Add(border);
                    break;
                case StepBarHeaderDisplayMode.Left:
                case StepBarHeaderDisplayMode.Right:
                case StepBarHeaderDisplayMode.Bottom:
                    contentPresenter.Margin = new Thickness(0, 5, 0, 0);
                    panel.Children.Clear();
                    panel.Children.Add(border);
                    panel.Children.Add(contentPresenter);
                    break;
            }
        }
    }
    public StepBarItem()
    {
        this.DefaultStyleKey = typeof(StepBarItem);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        contentPresenter = GetTemplateChild(ContentPresenterElement) as ContentPresenter;
        border = GetTemplateChild(BorderElement) as Border;
        panel = GetTemplateChild(PanelElement) as StackPanel;
        UpdateHeaderDisplayMode();
        UpdateVisualStates();
    }
}
