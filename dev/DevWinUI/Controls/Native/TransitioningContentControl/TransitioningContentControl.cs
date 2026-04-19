namespace DevWinUI;
public partial class TransitioningContentControl : ContentControl
{
    private FrameworkElement _contentPresenter;
    private Storyboard _storyboardBuildIn;
    private long _visibilityToken;

    public TransitioningContentControl()
    {
        Loaded += TransitioningContentControl_Loaded;
        Unloaded += TransitioningContentControl_Unloaded;
    }

    public static readonly DependencyProperty TransitionModeProperty = DependencyProperty.Register(
        nameof(TransitionMode), typeof(TransitionMode), typeof(TransitioningContentControl),
        new PropertyMetadata(TransitionMode.Right2Left, OnTransitionModeChanged));

    private static void OnTransitionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TransitioningContentControl)d;
        ctl.OnTransitionModeChanged((TransitionMode)e.NewValue);
    }

    private void OnTransitionModeChanged(TransitionMode newValue)
    {
        _storyboardBuildIn = TryGetResource<Storyboard>($"{newValue}Transition");
        StartTransition();
    }

    public TransitionMode TransitionMode
    {
        get => (TransitionMode)GetValue(TransitionModeProperty);
        set => SetValue(TransitionModeProperty, value);
    }

    public static readonly DependencyProperty TransitionStoryboardProperty = DependencyProperty.Register(
        nameof(TransitionStoryboard), typeof(Storyboard), typeof(TransitioningContentControl), new PropertyMetadata(default(Storyboard)));

    public Storyboard TransitionStoryboard
    {
        get => (Storyboard)GetValue(TransitionStoryboardProperty);
        set => SetValue(TransitionStoryboardProperty, value);
    }

    private void TransitioningContentControl_Loaded(object sender, RoutedEventArgs e)
    {
        _visibilityToken = RegisterPropertyChangedCallback(VisibilityProperty, OnVisibilityChanged);
    }

    private void TransitioningContentControl_Unloaded(object sender, RoutedEventArgs e)
    {
        UnregisterPropertyChangedCallback(VisibilityProperty, _visibilityToken);
    }

    private void OnVisibilityChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (dp == VisibilityProperty)
        {
            StartTransition();
        }
    }

    private void StartTransition()
    {
        if (_contentPresenter == null || Visibility != Visibility.Visible)
            return;

        var storyboard = TransitionStoryboard ?? _storyboardBuildIn;

        if (storyboard != null)
        {
            try
            {
                storyboard.Stop(); // Stop any active animation before starting a new one
                Storyboard.SetTarget(storyboard, _contentPresenter);
                storyboard.Begin();
            }
            catch (Exception)
            {
            }
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _contentPresenter = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
        if (_contentPresenter != null)
        {
            _contentPresenter.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            _contentPresenter.RenderTransform = new TransformGroup
            {
                Children =
                {
                    new ScaleTransform(),
                    new SkewTransform(),
                    new RotateTransform(),
                    new TranslateTransform()
                }
            };
        }

        StartTransition();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);

        if (newContent != null)
        {
            StartTransition();
        }
    }

    private TResource TryGetResource<TResource>(string key) where TResource : class
    {
        if (Application.Current.Resources.TryGetValue(key, out var resource) && resource is TResource typedResource)
        {
            return typedResource;
        }
        return null;
    }
}
