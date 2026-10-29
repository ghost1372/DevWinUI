namespace DevWinUI;

public sealed partial class CollapsibleGrid : Grid
{
    static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(220);

    readonly RectangleGeometry _clip = new();
    Storyboard? _storyboard;
    bool _animating;

    public CollapsibleGrid()
    {
        Visibility = Visibility.Collapsed;
        Height = 0;
        SizeChanged += OnSizeChanged;
    }

    public bool IsOpen { get; private set; }

    public void Set(bool open, bool animate)
    {
        if (open == IsOpen) return;

        IsOpen = open;
        _storyboard?.Stop();
        _storyboard = null;
        _animating = false;
        Clip = null;
        animate &= IsLoaded && ReasoningFx.Motion;

        if (open)
        {
            Visibility = Visibility.Visible;
            Height = double.NaN;
            if (!animate) return;

            UpdateLayout();
            double target = ActualHeight;
            if (target <= 0) return;

            Run(0, target, () => Height = double.NaN);
        }
        else
        {
            if (!animate || ActualHeight <= 0)
            {
                Collapse();
                return;
            }

            Run(ActualHeight, 0, Collapse);
        }
    }

    void Collapse()
    {
        Height = 0;
        Visibility = Visibility.Collapsed;
    }

    void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_animating) _clip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
    }

    void Run(double from, double to, Action done)
    {
        Height = to;

        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new Duration(AnimationDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop,
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, this);
        Storyboard.SetTargetProperty(animation, "Height");

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Completed += (_, _) =>
        {
            if (!ReferenceEquals(_storyboard, storyboard)) return;
            _storyboard = null;
            _animating = false;
            Clip = null;
            done();
        };

        _storyboard = storyboard;
        _animating = true;
        _clip.Rect = new Rect(0, 0, ActualWidth, from);
        Clip = _clip;
        storyboard.Begin();
    }
}
