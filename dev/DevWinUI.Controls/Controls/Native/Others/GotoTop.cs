namespace DevWinUI;
public partial class GotoTop : Button
{
    private Action _gotoTopAction;

    private ScrollViewer _scrollViewer;
    private ScrollView _scrollView;

    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
        nameof(Target), typeof(DependencyObject), typeof(GotoTop), new PropertyMetadata(null, OnTargetChanged));

    public DependencyObject Target
    {
        get => (DependencyObject)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public static readonly DependencyProperty AnimatedProperty = DependencyProperty.Register(
        nameof(Animated), typeof(bool), typeof(GotoTop), new PropertyMetadata(true));

    public bool Animated
    {
        get => (bool)GetValue(AnimatedProperty);
        set => SetValue(AnimatedProperty, value);
    }

    public static readonly DependencyProperty HidingHeightProperty = DependencyProperty.Register(
        nameof(HidingHeight), typeof(double), typeof(GotoTop), new PropertyMetadata(0.0));

    public double HidingHeight
    {
        get => (double)GetValue(HidingHeightProperty);
        set => SetValue(HidingHeightProperty, value);
    }

    public static readonly DependencyProperty AutoHidingProperty = DependencyProperty.Register(
        nameof(AutoHiding), typeof(bool), typeof(GotoTop), new PropertyMetadata(true));

    public bool AutoHiding
    {
        get => (bool)GetValue(AutoHidingProperty);
        set => SetValue(AutoHidingProperty, value);
    }

    public GotoTop()
    {
        this.DefaultStyleKey = typeof(GotoTop);

        Loaded += (s, e) => CreateGotoAction(Target);
        Click -= GotoTop_Click;
        Click += GotoTop_Click;
    }

    private void GotoTop_Click(object sender, RoutedEventArgs e)
    {
        _gotoTopAction?.Invoke();
    }

    private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is GotoTop gotoTop)
        {
            gotoTop.CreateGotoAction(e.NewValue as DependencyObject);
        }
    }

    public virtual void CreateGotoAction(DependencyObject obj)
    {
        if (_scrollViewer != null)
        {
            _scrollViewer.ViewChanged -= ScrollViewer_ViewChanged;
        }
        if (_scrollView != null)
        {
            _scrollView.ViewChanged -= ScrollView_ViewChanged;
        }

        if (obj is ScrollViewer viewer && viewer != null)
        {
            _scrollViewer = viewer;
            _scrollViewer.ViewChanged += ScrollViewer_ViewChanged;

            if (Animated)
            {
                _gotoTopAction = () => _scrollViewer.ChangeView(null, 0, null, false);
            }
            else
            {
                _gotoTopAction = () => _scrollViewer.ChangeView(null, 0, null, true);
            }
        }
        else if (obj is ScrollView view && view != null)
        {
            _scrollView = view;
            _scrollView.ViewChanged += ScrollView_ViewChanged;

            if (Animated)
            {
                _gotoTopAction = () => _scrollView?.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Auto));
            }
            else
            {
                _gotoTopAction = () => _scrollView?.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
            }
        }
    }

    private void ScrollView_ViewChanged(ScrollView sender, object args)
    {
        if (AutoHiding)
        {
            Visibility = sender.VerticalOffset > HidingHeight ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (AutoHiding)
        {
            Visibility = _scrollViewer.VerticalOffset > HidingHeight ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
