namespace DevWinUI;

public partial class BruteForceItem : Control
{
    private bool _isRenderingSubscribed;
    private bool _updatingDisplay;
    private long _lastRenderTick;
    private string _targetChar = string.Empty;

    public BruteForceItem()
    {
        DefaultStyleKey = typeof(BruteForceItem);
        Loaded += OnLoaded;
        Unloaded += UnOnLoaded;
        if (Characters == null)
        {
            Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{};:,.<>?/\\|~".ToCharArray();
        }
        ApplyState(State);
    }

    public char[] Characters
    {
        get { return (char[])GetValue(CharactersProperty); }
        set { SetValue(CharactersProperty, value); }
    }

    public static readonly DependencyProperty CharactersProperty =
        DependencyProperty.Register(nameof(Characters), typeof(char[]), typeof(BruteForceItem), new PropertyMetadata(null));


    public TimeSpan RenderDelay
    {
        get { return (TimeSpan)GetValue(RenderDelayProperty); }
        set { SetValue(RenderDelayProperty, value); }
    }

    public static readonly DependencyProperty RenderDelayProperty =
        DependencyProperty.Register(nameof(RenderDelay), typeof(TimeSpan), typeof(BruteForceItem), new PropertyMetadata(TimeSpan.FromMilliseconds(50)));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(BruteForceItem), new PropertyMetadata(string.Empty, OnTextChanged));

    public BruteForceState State
    {
        get => (BruteForceState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(BruteForceState), typeof(BruteForceItem), new PropertyMetadata(BruteForceState.Pending, OnStateChanged));

    public void SetState(BruteForceState state)
    {
        if (State == state)
        {
            ApplyState(state);
            return;
        }

        State = state;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ApplyVisualState(State);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (State == BruteForceState.Pending)
        {
            StartRenderingUpdates();
        }
    }

    private void UnOnLoaded(object sender, RoutedEventArgs e)
    {
        StopRenderingUpdates();
    }

    private void StartRenderingUpdates()
    {
        if (_isRenderingSubscribed)
        {
            return;
        }

        CompositionTarget.Rendering += OnCompositionTargetRendering;
        _isRenderingSubscribed = true;
    }

    private void StopRenderingUpdates()
    {
        if (!_isRenderingSubscribed)
        {
            return;
        }

        CompositionTarget.Rendering -= OnCompositionTargetRendering;
        _isRenderingSubscribed = false;
    }

    private void OnCompositionTargetRendering(object sender, object e)
    {
        var now = Environment.TickCount64;
        if (now - _lastRenderTick < RenderDelay.TotalMilliseconds)
        {
            return;
        }

        _lastRenderTick = now;
        SetDisplayChar(GetRandomChar());
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var item = (BruteForceItem)d;
        if (item._updatingDisplay)
        {
            return;
        }

        item._targetChar = (string)e.NewValue ?? string.Empty;

        if (item.State == BruteForceState.Success)
        {
            item.SetDisplayChar(item.GetResolvedChar());
        }
    }

    private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((BruteForceItem)d).ApplyState((BruteForceState)e.NewValue);
    }

    private void ApplyState(BruteForceState state)
    {
        ApplyVisualState(state);

        switch (state)
        {
            case BruteForceState.Success:
                StopRenderingUpdates();
                SetDisplayChar(GetResolvedChar());
                break;

            case BruteForceState.Warning:
            case BruteForceState.Error:
                StopRenderingUpdates();
                SetDisplayChar(GetWrongRandomChar());
                break;

            default:
                StartRenderingUpdates();
                break;
        }
    }

    private void SetDisplayChar(string value)
    {
        _updatingDisplay = true;
        Text = value;
        _updatingDisplay = false;
    }

    private string GetResolvedChar()
    {
        return string.IsNullOrEmpty(_targetChar) ? GetRandomChar() : _targetChar[0].ToString();
    }

    private string GetWrongRandomChar()
    {
        if (string.IsNullOrEmpty(_targetChar))
        {
            return GetRandomChar();
        }

        char correct = _targetChar[0];
        char result;
        do
        {
            result = Characters[Random.Shared.Next(Characters.Length)];
        }
        while (result == correct);

        return result.ToString();
    }

    private void ApplyVisualState(BruteForceState state)
    {
        VisualStateManager.GoToState(this, state.ToString(), true);
    }

    private string GetRandomChar()
    {
        return Characters[Random.Shared.Next(Characters.Length)].ToString();
    }
}
