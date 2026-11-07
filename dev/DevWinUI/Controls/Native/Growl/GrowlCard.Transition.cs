namespace DevWinUI;

public sealed partial class GrowlCard
{
    private const double TransitionOffset = 8;

    private static readonly TimeSpan ColorDuration = TimeSpan.FromMilliseconds(350);
    private static readonly TimeSpan OutDuration = TimeSpan.FromMilliseconds(160);
    private static readonly TimeSpan InDelay = TimeSpan.FromMilliseconds(90);
    private static readonly TimeSpan InDuration = TimeSpan.FromMilliseconds(260);
    private static readonly TimeSpan SizeDuration = TimeSpan.FromMilliseconds(300);

    private static void AnimateDouble(Storyboard storyboard, DependencyObject target, string property, double from, double to, TimeSpan duration, TimeSpan? delay = null, bool dependent = false)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new Duration(duration),
            BeginTime = delay ?? TimeSpan.Zero,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            EnableDependentAnimation = dependent
        };

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, property);
        storyboard.Children.Add(animation);
    }

    private bool CanAnimate()
    {
        return IsLoaded
            && Visibility == Visibility.Visible
            && _root is not null
            && _surface is not null
            && _snapshot is not null
            && _contentHost is { ActualWidth: > 0, ActualHeight: > 0 };
    }

    private void RequestApply()
    {
        if (_applyScheduled)
        {
            return;
        }

        _applyScheduled = true;

        if (!DispatcherQueue.TryEnqueue(RunApply))
        {
            Commit();
            _applyScheduled = false;
        }
    }

    private async void RunApply()
    {
        try
        {
            await ApplyAsync();
        }
        catch (Exception)
        {
        }
        finally
        {
            _applyScheduled = false;
        }
    }

    private async Task ApplyAsync()
    {
        FinishTransition();

        if (Severity == _appliedType || !CanAnimate())
        {
            Commit();
            _applyScheduled = false;
            return;
        }

        double width = _contentHost!.ActualWidth;
        double height = _contentHost.ActualHeight;
        double fromHeight = _root!.ActualHeight;
        Color fromColor = _indicatorBrush.Color;
        var bitmap = new RenderTargetBitmap();

        try
        {
            await bitmap.RenderAsync(_contentHost);
        }
        catch
        {
            Commit();
            _applyScheduled = false;
            return;
        }

        Commit();
        _applyScheduled = false;

        if (CanAnimate())
        {
            PlayTransition(bitmap, width, height, fromHeight, fromColor);
        }
    }

    private void Commit()
    {
        if (_titleBlock is not null)
        {
            _titleBlock.Text = Title;
        }

        if (_subTitleBlock is not null)
        {
            _subTitleBlock.Text = SubTitle;
        }

        if (_iconPresenter is not null)
        {
            _iconPresenter.Content = Icon;
        }

        if (_contentPresenter is not null)
        {
            _contentPresenter.Content = Content;
        }

        if (_actionPresenter is not null)
        {
            _actionPresenter.Content = ActionContent;
        }

        _appliedType = Severity;
        _indicatorBrush.Color = ResolveIndicatorColor(_appliedType);
        UpdateCountdownBrush();

        UpdateVisualStates();
        UpdateProgressText();
        UpdateSentTime();

        if (IsAutoCloseEnabled != _autoCloseActive)
        {
            StartAutoClose();
        }
    }

    private void PlayTransition(RenderTargetBitmap bitmap, double width, double height, double fromHeight, Color fromColor)
    {
        var root = _root!;
        var surface = _surface!;
        var snapshot = _snapshot!;
        var host = _contentHost!;

        UpdateLayout();

        double toHeight = root.ActualHeight;
        Color toColor = _indicatorBrush.Color;

        var snapshotShift = new CompositeTransform();
        var hostShift = new CompositeTransform { TranslateY = TransitionOffset };

        snapshot.Source = bitmap;
        snapshot.Width = width;
        snapshot.Height = height;
        snapshot.RenderTransform = snapshotShift;
        snapshot.Opacity = 1;
        snapshot.Visibility = Visibility.Visible;

        host.RenderTransform = hostShift;
        host.Opacity = 0;

        _indicatorBrush.Color = fromColor;

        var storyboard = new Storyboard();

        AnimateDouble(storyboard, snapshot, "Opacity", 1, 0, OutDuration);
        AnimateDouble(storyboard, snapshotShift, "TranslateY", 0, -TransitionOffset, OutDuration);
        AnimateDouble(storyboard, host, "Opacity", 0, 1, InDuration, InDelay);
        AnimateDouble(storyboard, hostShift, "TranslateY", TransitionOffset, 0, InDuration, InDelay);

        var colorAnimation = new ColorAnimation
        {
            From = fromColor,
            To = toColor,
            Duration = new Duration(ColorDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
            EnableDependentAnimation = true
        };
        Storyboard.SetTarget(colorAnimation, _indicatorBrush);
        Storyboard.SetTargetProperty(colorAnimation, "Color");
        storyboard.Children.Add(colorAnimation);

        if (Math.Abs(fromHeight - toHeight) > 1)
        {
            surface.Clip = new RectangleGeometry { Rect = new Rect(0, 0, surface.ActualWidth, Math.Max(0, fromHeight - 2)) };
            surface.SizeChanged += OnSurfaceSizeChanged;
            AnimateDouble(storyboard, root, "Height", fromHeight, toHeight, SizeDuration, null, true);
        }

        storyboard.Completed += (_, _) =>
        {
            if (ReferenceEquals(_transition, storyboard))
            {
                FinishTransition();
            }
        };

        _transition = storyboard;
        storyboard.Begin();
    }

    private void FinishTransition()
    {
        if (_transition is null)
        {
            return;
        }

        var storyboard = _transition;
        _transition = null;
        storyboard.Stop();

        if (_snapshot is not null)
        {
            _snapshot.Visibility = Visibility.Collapsed;
            _snapshot.Opacity = 0;
            _snapshot.RenderTransform = null;
            _snapshot.Source = null;
        }

        if (_contentHost is not null)
        {
            _contentHost.Opacity = 1;
            _contentHost.RenderTransform = null;
        }

        if (_surface is not null)
        {
            _surface.SizeChanged -= OnSurfaceSizeChanged;
            _surface.Clip = null;
        }

        _root?.ClearValue(HeightProperty);
        _indicatorBrush.Color = ResolveIndicatorColor(_appliedType);
    }

    private void OnSurfaceSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_surface?.Clip is RectangleGeometry clip)
        {
            clip.Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
        }
    }

}
