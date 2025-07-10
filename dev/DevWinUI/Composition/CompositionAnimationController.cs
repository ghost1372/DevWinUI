using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;
public partial class CompositionAnimationController
{
    private readonly Visual _visual;
    private readonly string _propertyName;
    private readonly KeyFrameAnimation _animation;
    private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(16);
    private readonly DispatcherTimer _sliderAnimator = new();
    private AnimationController _controller;
    private double _delta;
    private DateTimeOffset _lastTime;
    private bool _isPaused = true;

    public Slider? Slider { get; set; }
    public SymbolIcon? PlayIcon { get; set; }

    public CompositionAnimationController(Visual visual, string propertyName, KeyFrameAnimation animation)
    {
        _visual = visual;
        _propertyName = propertyName;
        _animation = animation;

        _sliderAnimator.Interval = _interval;
        _sliderAnimator.Tick += OnSliderTick;

        if (animation.Duration.TotalMilliseconds > 0 && Slider != null)
        {
            _delta = (Slider.Maximum / animation.Duration.TotalMilliseconds) * _interval.TotalMilliseconds;
        }
    }

    public void AttachSlider(Slider slider)
    {
        Slider = slider;
        Slider.ValueChanged += OnSliderChanged;
        Slider.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPressedThumb), true);
        Slider.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(OnReleasedThumb), true);
        if (_animation.Duration.TotalMilliseconds > 0)
        {
            _delta = (slider.Maximum / _animation.Duration.TotalMilliseconds) * _interval.TotalMilliseconds;
        }
    }

    public void EnsureController()
    {
        if (_controller == null)
        {
            _visual.StartAnimation(_propertyName, _animation);
            _controller = _visual.TryGetAnimationController(_propertyName);
            _controller?.Pause();
        }
    }

    public void PlayPause()
    {
        EnsureController();
        if (_isPaused)
        {
            _controller.Resume();
            StartSlider();
            if (PlayIcon != null) PlayIcon.Symbol = Symbol.Pause;
        }
        else
        {
            _controller.Pause();
            StopSlider();
            if (PlayIcon != null) PlayIcon.Symbol = Symbol.Play;
        }
        _isPaused = !_isPaused;
    }

    public void Stop()
    {
        EnsureController();
        _isPaused = false;
        PlayPause();
        _controller.PlaybackRate = 1;
        _controller.Progress = 0;
        if (Slider != null) Slider.Value = 0;
    }

    public void SpeedUp()
    {
        EnsureController();
        if (Math.Abs(_controller.PlaybackRate) < AnimationController.MaxPlaybackRate && !_isPaused)
            _controller.PlaybackRate *= 2;
    }

    public void SlowDown()
    {
        EnsureController();
        if (Math.Abs(_controller.PlaybackRate) > AnimationController.MinPlaybackRate && !_isPaused)
            _controller.PlaybackRate /= 2;
    }

    public void Reverse()
    {
        EnsureController();
        if (!_isPaused)
            _controller.PlaybackRate *= -1;
    }

    private void OnSliderChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        EnsureController();
        _controller.Progress = (float)(Slider?.Value ?? 0) * 0.01f;
    }

    private void OnSliderTick(object sender, object e)
    {
        DateTimeOffset now = DateTimeOffset.Now;
        double ticks = (now - _lastTime).TotalMilliseconds / _interval.TotalMilliseconds;
        _lastTime = now;

        if (Slider != null && _controller != null)
        {
            Slider.Value += _delta * ticks * _controller.PlaybackRate;

            if (_controller.PlaybackRate > 0 && Slider.Value >= 100)
                Slider.Value = 0;
            else if (_controller.PlaybackRate < 0 && Slider.Value <= 0)
                Slider.Value = 100;
        }
    }

    private void StartSlider()
    {
        _lastTime = DateTimeOffset.Now;
        _sliderAnimator.Start();
    }

    private void StopSlider()
    {
        _sliderAnimator.Stop();
    }

    private void OnPressedThumb(object sender, PointerRoutedEventArgs e)
    {
        EnsureController();
        StopSlider();
        _controller.Pause();
    }

    private void OnReleasedThumb(object sender, PointerRoutedEventArgs e)
    {
        EnsureController();
        if (!_isPaused)
        {
            StartSlider();
            _controller.Resume();
        }
    }
}

