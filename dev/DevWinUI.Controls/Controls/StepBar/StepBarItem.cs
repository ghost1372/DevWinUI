using System.Diagnostics.CodeAnalysis;

namespace DevWinUI;
public partial class StepBarItem : ContentControl
{
    private const string ContentPresenterElement = "PART_Content";
    private const string BorderElement = "PART_Border";
    private const string PanelElement = "PART_Panel";
    private ContentPresenter contentPresenter;
    private Border border;
    private StackPanel panel;
    internal bool ShowStepIndex
    {
        get { return (bool)GetValue(ShowStepIndexProperty); }
        set { SetValue(ShowStepIndexProperty, value); }
    }

    internal static readonly DependencyProperty ShowStepIndexProperty =
        DependencyProperty.Register(nameof(ShowStepIndex), typeof(bool), typeof(StepBarItem), new PropertyMetadata(true));

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

    internal IconSource Icon
    {
        get { return (IconSource)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    internal static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconSource), typeof(StepBarItem), new PropertyMetadata(null));

    public IconSource UnderWayIcon
    {
        get { return (IconSource)GetValue(UnderWayIconProperty); }
        set { SetValue(UnderWayIconProperty, value); }
    }

    public static readonly DependencyProperty UnderWayIconProperty =
        DependencyProperty.Register(nameof(UnderWayIcon), typeof(IconSource), typeof(StepBarItem), new PropertyMetadata(null, OnIconChanged));

    public IconSource WaitingIcon
    {
        get { return (IconSource)GetValue(WaitingIconProperty); }
        set { SetValue(WaitingIconProperty, value); }
    }

    public static readonly DependencyProperty WaitingIconProperty =
        DependencyProperty.Register(nameof(WaitingIcon), typeof(IconSource), typeof(StepBarItem), new PropertyMetadata(null, OnIconChanged));

    public IconSource CompleteIcon
    {
        get { return (IconSource)GetValue(CompleteIconProperty); }
        set { SetValue(CompleteIconProperty, value); }
    }

    public static readonly DependencyProperty CompleteIconProperty =
        DependencyProperty.Register(nameof(CompleteIcon), typeof(IconSource), typeof(StepBarItem), new PropertyMetadata(null, OnIconChanged));

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBarItem)d;
        if (ctl != null)
        {
            ctl.UpdateVisualStates();
        }
    }
    internal bool ShowStepIcon
    {
        get { return (bool)GetValue(ShowStepIconProperty); }
        set { SetValue(ShowStepIconProperty, value); }
    }

    internal static readonly DependencyProperty ShowStepIconProperty =
        DependencyProperty.Register(nameof(ShowStepIcon), typeof(bool), typeof(StepBarItem), new PropertyMetadata(false));

    public StepBarDisplayMode DisplayMode
    {
        get { return (StepBarDisplayMode)GetValue(DisplayModeProperty); }
        set { SetValue(DisplayModeProperty, value); }
    }

    public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register(nameof(DisplayMode), typeof(StepBarDisplayMode), typeof(StepBarItem), new PropertyMetadata(StepBarDisplayMode.Index, OnDisplayModeChanged));

    private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StepBarItem)d;
        if (ctl != null)
        {
            switch ((StepBarDisplayMode)e.NewValue)
            {
                case StepBarDisplayMode.Index:
                    ctl.ShowStepIcon = false;
                    break;
                case StepBarDisplayMode.Icon:
                    ctl.ShowStepIcon = true;
                    break;
            }
        }
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(StepBarItem))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ContentPresenter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(StackPanel))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Border))]
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

    public void UpdateVisualStates()
    {
        string visualState = "Waiting";
        switch (ProgressState)
        {
            case StepProgressState.Complete:
                visualState = Status.ToString();
                Icon = CompleteIcon;
                break;
            case StepProgressState.Waiting:
                visualState = "Waiting";
                Icon = WaitingIcon;
                break;
            case StepProgressState.UnderWay:
                visualState = Status.ToString();
                Icon = UnderWayIcon;
                break;
        }

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
}
