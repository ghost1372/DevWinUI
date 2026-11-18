namespace DevWinUI;

public partial class GotoTop : Button
{
    private Action _gotoTopAction;

    private FrameworkElement _targetElement;
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
        if (_targetElement != null)
        {
            _targetElement.Loaded -= TargetElement_Loaded;
            _targetElement = null;
        }

        if (_scrollViewer != null)
        {
            _scrollViewer.ViewChanged -= ScrollViewer_ViewChanged;
            _scrollViewer = null;
        }

        if (_scrollView != null)
        {
            _scrollView.ViewChanged -= ScrollView_ViewChanged;
            _scrollView = null;
        }

        _gotoTopAction = null;

        // Resolve the scrolling element from any FrameworkElement.
        if (obj is FrameworkElement element && obj is not ScrollViewer && obj is not ScrollView)
        {
            _targetElement = element;
            _targetElement.Loaded += TargetElement_Loaded;

            if (element is Control control)
            {
                control.ApplyTemplate();
            }

            obj = element.FindDescendant<DependencyObject>(static child => child is ScrollViewer or ScrollView);
        }

        if (obj is ScrollViewer viewer)
        {
            _scrollViewer = viewer;
            _scrollViewer.ViewChanged += ScrollViewer_ViewChanged;

            _gotoTopAction = () => viewer.ChangeView(null, 0, null, !Animated);
        }
        else if (obj is ScrollView view)
        {
            _scrollView = view;
            _scrollView.ViewChanged += ScrollView_ViewChanged;

            _gotoTopAction = () => view.ScrollTo(0, 0,
                new ScrollingScrollOptions(Animated
                    ? ScrollingAnimationMode.Auto
                    : ScrollingAnimationMode.Disabled));
        }
    }

    private void TargetElement_Loaded(object sender, RoutedEventArgs e)
    {
        if (ReferenceEquals(sender, _targetElement))
        {
            CreateGotoAction(Target);
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
