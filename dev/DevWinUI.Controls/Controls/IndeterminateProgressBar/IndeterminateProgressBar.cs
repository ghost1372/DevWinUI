namespace DevWinUI;

[TemplatePart(Name = PART_RootGrid, Type = typeof(Grid))]
public partial class IndeterminateProgressBar : Control
{
    private Dictionary<int, KeyFrameDetails> _keyFrameMap = null;
    private Dictionary<int, KeyFrameDetails> _opKeyFrameMap = null;
    private Storyboard storyBoard;
    private bool isStoryboardRunning;
    private long _visibilityPropertyToken;
    private const string PART_RootGrid = "PART_RootGrid";
    private Grid rootGrid;

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (IndeterminateProgressBar)d;
        if (ctl != null)
        {
            if ((bool)e.NewValue)
            {
                ctl.UpdateKeyFrames();
                ctl.StartAnimation();
            }
            else
            {
                ctl.StopAnimation();
            }
        }
    }
    private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDelay = (Duration)e.OldValue;
        var newDelay = pBar.Delay;
        pBar.OnDelayChanged(oldDelay, newDelay);
    }
    private static void OnDotWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDotWidth = (double)e.OldValue;
        var newDotWidth = pBar.DotWidth;
        pBar.OnDotWidthChanged(oldDotWidth, newDotWidth);
    }

    private static void OnDotHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDotHeight = (double)e.OldValue;
        var newDotHeight = pBar.DotHeight;
        pBar.OnDotHeightChanged(oldDotHeight, newDotHeight);
    }

    private static void OnDotRadiusXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDotRadiusX = (double)e.OldValue;
        var newDotRadiusX = pBar.DotRadiusX;
        pBar.OnDotRadiusXChanged(oldDotRadiusX, newDotRadiusX);
    }

    private static void OnDotRadiusYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDotRadiusY = (double)e.OldValue;
        var newDotRadiusY = pBar.DotRadiusY;
        pBar.OnDotRadiusYChanged(oldDotRadiusY, newDotRadiusY);
    }

    private static void OnDurationAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDurationA = (Duration)e.OldValue;
        var newDurationA = pBar.DurationA;
        pBar.OnDurationAChanged(oldDurationA, newDurationA);
    }

    private static void OnDurationBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDurationB = (Duration)e.OldValue;
        var newDurationB = pBar.DurationB;
        pBar.OnDurationBChanged(oldDurationB, newDurationB);
    }

    private static void OnDurationCChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldDurationC = (Duration)e.OldValue;
        var newDurationC = pBar.DurationC;
        pBar.OnDurationCChanged(oldDurationC, newDurationC);
    }
    private static void OnKeyFrameAChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldKeyFrameA = (double)e.OldValue;
        var newKeyFrameA = pBar.KeyFrameA;
        pBar.OnKeyFrameAChanged(oldKeyFrameA, newKeyFrameA);
    }

    private static void OnKeyFrameBChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldKeyFrameB = (double)e.OldValue;
        var newKeyFrameB = pBar.KeyFrameB;
        pBar.OnKeyFrameBChanged(oldKeyFrameB, newKeyFrameB);
    }
    private static void OnOscillateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldOscillate = (bool)e.OldValue;
        var newOscillate = pBar.Oscillate;
        pBar.OnOscillateChanged(oldOscillate, newOscillate);
    }
    private static void OnReverseDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldReverseDuration = (Duration)e.OldValue;
        var newReverseDuration = pBar.ReverseDuration;
        pBar.OnReverseDurationChanged(oldReverseDuration, newReverseDuration);
    }
    private static void OnTotalDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var pBar = (IndeterminateProgressBar)d;
        var oldTotalDuration = (Duration)e.OldValue;
        var newTotalDuration = pBar.TotalDuration;
        pBar.OnTotalDurationChanged(oldTotalDuration, newTotalDuration);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        rootGrid = GetTemplateChild(PART_RootGrid) as Grid;
        _keyFrameMap = new Dictionary<int, KeyFrameDetails>();
        _opKeyFrameMap = new Dictionary<int, KeyFrameDetails>();

        GetKeyFramesFromStoryboard();

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        UnregisterPropertyChangedCallback(VisibilityProperty, _visibilityPropertyToken);
        _visibilityPropertyToken = RegisterPropertyChangedCallback(VisibilityProperty, OnVisibilityPropertyChanged);

        UpdateKeyFrames();
        StartAnimation();
    }

    private void OnVisibilityPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (Visibility == Visibility.Visible)
        {
            UpdateKeyFrames();
            StartAnimation();
        }
        else
        {
            StopAnimation();
        }
    }
    
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        RestartStoryboardAnimation();
    }

    protected void OnDelayChanged(Duration oldDelay, Duration newDelay)
    {
        var isActive = isStoryboardRunning;
        if (isActive)
            StopAnimation();

        UpdateTimelineDelay(newDelay);

        if (isActive)
            StartAnimation();
    }

    protected void OnDotWidthChanged(double oldDotWidth, double newDotWidth)
    {
        if (isStoryboardRunning)
            RestartStoryboardAnimation();
    }
    
    protected void OnDotHeightChanged(double oldDotHeight, double newDotHeight)
    {
        if (isStoryboardRunning)
            RestartStoryboardAnimation();
    }

    protected void OnDotRadiusXChanged(double oldDotRadiusX, double newDotRadiusX)
    {
        if (isStoryboardRunning)
            RestartStoryboardAnimation();
    }

    protected void OnDotRadiusYChanged(double oldDotRadiusY, double newDotRadiusY)
    {
        if (isStoryboardRunning)
            RestartStoryboardAnimation();
    }

    protected void OnDurationAChanged(Duration oldDurationA, Duration newDurationA)
    {
        var isActive = isStoryboardRunning;
        if (isActive)
            StopAnimation();

        UpdateKeyTimes(1, newDurationA);

        if (isActive)
            StartAnimation();
    }
    
    protected void OnDurationBChanged(Duration oldDurationB, Duration newDurationB)
    {
        var isActive = isStoryboardRunning;
        if (isActive)
            StopAnimation();

        UpdateKeyTimes(2, newDurationB);

        if (isActive)
            StartAnimation();
    }

    protected void OnDurationCChanged(Duration oldDurationC, Duration newDurationC)
    {
        var isActive = isStoryboardRunning;
        if (isActive)
            StopAnimation();

        UpdateKeyTimes(3, newDurationC);

        if (isActive)
            StartAnimation();
    }

    protected void OnKeyFrameAChanged(double oldKeyFrameA, double newKeyFrameA)
    {
        RestartStoryboardAnimation();
    }

    protected void OnKeyFrameBChanged(double oldKeyFrameB, double newKeyFrameB)
    {
        RestartStoryboardAnimation();
    }

    protected void OnOscillateChanged(bool oldOscillate, bool newOscillate)
    {
        if (storyBoard == null)
            return;

        StopAnimation();
        storyBoard.AutoReverse = newOscillate;
        storyBoard.Duration = newOscillate ? ReverseDuration : TotalDuration;
        StartAnimation();
    }

    protected void OnReverseDurationChanged(Duration oldReverseDuration, Duration newReverseDuration)
    {
        if ((storyBoard == null) || (!Oscillate))
            return;

        storyBoard.Duration = newReverseDuration;
        RestartStoryboardAnimation();
    }
    
    protected void OnTotalDurationChanged(Duration oldTotalDuration, Duration newTotalDuration)
    {
        if ((storyBoard == null) || (Oscillate))
            return;

        storyBoard.Duration = newTotalDuration;
        RestartStoryboardAnimation();
    }

    private void StartAnimation()
    {
        if ((storyBoard == null) || (isStoryboardRunning))
            return;

        storyBoard.Begin();
        isStoryboardRunning = true;
    }

    private void StopAnimation()
    {
        if ((storyBoard == null) || (!isStoryboardRunning))
            return;

        // Move the timeline to the end and stop the animation
        storyBoard.SeekAlignedToLastTick(TimeSpan.FromSeconds(0));
        storyBoard.Stop();
        isStoryboardRunning = false;
    }

    private void RestartStoryboardAnimation()
    {
        StopAnimation();
        UpdateKeyFrames();
        StartAnimation();
    }

    private void GetKeyFramesFromStoryboard()
    {
        if (rootGrid == null)
        {
            return;
        }
        foreach (var item in rootGrid.Resources)
        {
            if (item.Key.Equals("RootStoryboard"))
            {
                storyBoard = item.Value as Storyboard;
            }
        }

        if (storyBoard == null)
            return;

        foreach (var timeline in storyBoard.Children)
        {
            var dakeys = timeline as DoubleAnimationUsingKeyFrames;
            if (dakeys == null)
                continue;

            var targetName = Storyboard.GetTargetName(dakeys);
            ProcessDoubleAnimationWithKeys(dakeys, !targetName.StartsWith("Trans"));
        }
    }

    private void ProcessDoubleAnimationWithKeys(DoubleAnimationUsingKeyFrames dakeys, bool isOpacityAnim = false)
    {
        // Get all the keyframes in the instance.
        for (var i = 0; i < dakeys.KeyFrames.Count; i++)
        {
            var frame = dakeys.KeyFrames[i];

            Dictionary<int, KeyFrameDetails> targetMap = null;

            targetMap = isOpacityAnim ? _opKeyFrameMap : _keyFrameMap;

            if (!targetMap.ContainsKey(i))
            {
                targetMap[i] = new KeyFrameDetails() { KeyFrames = new List<DoubleKeyFrame>() };
            }

            // Update the keyframe time and add it to the map
            targetMap[i].KeyFrameTime = frame.KeyTime;
            targetMap[i].KeyFrames.Add(frame);
        }
    }

    private void UpdateKeyFrames()
    {
        // Get the current width of the UserControl1
        var width = this.ActualWidth;
        // Update the values only if the current width is greater than Zero and is visible
        if ((!(width > 0.0)) || (Visibility != Visibility.Visible))
            return;
        var point0 = -10;
        var pointA = width * KeyFrameA;
        var pointB = width * KeyFrameB;
        var pointC = width + 10;
        // Update the keyframes stored in the map
        UpdateKeyFrame(0, point0);
        UpdateKeyFrame(1, pointA);
        UpdateKeyFrame(2, pointB);
        UpdateKeyFrame(3, pointC);
    }

    private void UpdateKeyFrame(int key, double newValue)
    {
        if (!_keyFrameMap.ContainsKey(key))
            return;

        foreach (var frame in _keyFrameMap[key].KeyFrames)
        {
            if (frame is LinearDoubleKeyFrame)
            {
                frame.SetValue(LinearDoubleKeyFrame.ValueProperty, newValue);
            }
            else if (frame is EasingDoubleKeyFrame)
            {
                frame.SetValue(EasingDoubleKeyFrame.ValueProperty, newValue);
            }
        }
    }

    private void UpdateKeyTimes(int key, Duration newDuration)
    {
        switch (key)
        {
            case 1:
                UpdateKeyTime(1, newDuration);
                UpdateKeyTime(2, newDuration + DurationB);
                UpdateKeyTime(3, newDuration + DurationB + DurationC);
                break;

            case 2:
                UpdateKeyTime(2, DurationA + newDuration);
                UpdateKeyTime(3, DurationA + newDuration + DurationC);
                break;

            case 3:
                UpdateKeyTime(3, DurationA + DurationB + newDuration);
                break;
        }

        // Update the opacity animation duration based on the complete duration
        // of the animation
        UpdateOpacityKeyTime(1, DurationA + DurationB + DurationC);
    }

    private void UpdateKeyTime(int key, Duration newDuration)
    {
        if (!_keyFrameMap.ContainsKey(key))
            return;

        var newKeyTime = KeyTime.FromTimeSpan(newDuration.TimeSpan);
        _keyFrameMap[key].KeyFrameTime = newKeyTime;

        foreach (var frame in _keyFrameMap[key].KeyFrames)
        {
            if (frame is LinearDoubleKeyFrame)
            {
                frame.SetValue(LinearDoubleKeyFrame.KeyTimeProperty, newKeyTime);
            }
            else if (frame is EasingDoubleKeyFrame)
            {
                frame.SetValue(EasingDoubleKeyFrame.KeyTimeProperty, newKeyTime);
            }
        }
    }

    private void UpdateOpacityKeyTime(int key, Duration newDuration)
    {
        if (!_opKeyFrameMap.ContainsKey(key))
            return;

        var newKeyTime = KeyTime.FromTimeSpan(newDuration.TimeSpan);
        _opKeyFrameMap[key].KeyFrameTime = newKeyTime;

        foreach (DiscreteDoubleKeyFrame frame in _opKeyFrameMap[key].KeyFrames.OfType<DiscreteDoubleKeyFrame>())
        {
            frame.SetValue(DiscreteDoubleKeyFrame.KeyTimeProperty, newKeyTime);
        }
    }

    private void UpdateTimelineDelay(Duration newDelay)
    {
        var nextDelay = new Duration(TimeSpan.FromSeconds(0));

        if (storyBoard == null)
            return;

        for (var i = 0; i < storyBoard.Children.Count; i++)
        {
            // The first five animations are for translation
            // The next five animations are for opacity
            if (i == 5)
                nextDelay = newDelay;
            else
                nextDelay += newDelay;


            var timeline = storyBoard.Children[i] as DoubleAnimationUsingKeyFrames;
            timeline?.SetValue(DoubleAnimationUsingKeyFrames.BeginTimeProperty, nextDelay.TimeSpan);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
