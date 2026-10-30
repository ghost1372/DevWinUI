using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = PartHost, Type = typeof(WordWrapPanel))]
public sealed partial class ScrambledText : Control
{
    private const string PartHost = "PART_Host";

    private const double WaveSpeed = 900;
    private const double LiftDistance = 16;
    private const double MagnifyGain = 0.9;
    private const double BlurSpread = 3;
    private const double BlurOpacity = 0.3;
    private const double BlurDimming = 0.55;
    private const double IntroDelay = 0.25;
    private const double PulseDuration = 0.15;
    private const double RiseTime = 0.06;
    private const double BaseSwapInterval = 0.05;
    private const double MaxFrameTime = 0.1;
    private const double IntensityEpsilon = 0.001;

    private static readonly (double X, double Y)[] BlurDirections = { (1, 0), (-1, 0), (0, 1), (0, -1) };

    private readonly List<ScrambledTextLetter> _letters = new();

    private string[] _scrambleGlyphs = Array.Empty<string>();
    private WordWrapPanel? _host;
    private TimeSpan _lastFrame;
    private double _now;
    private bool _hasLastFrame;
    private bool _rendering;
    private bool _introPlayed;
    private bool _waitingIntro;

    private double Now => _now;

    public ScrambledText()
    {
        DefaultStyleKey = typeof(ScrambledText);

        UpdateScrambleGlyphs();

        PointerMoved += OnPointerMoved;
        PointerExited += OnPointerLeft;
        PointerCanceled += OnPointerLeft;
        PointerPressed += OnPointerPressed;
        EffectiveViewportChanged += OnViewportChanged;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _host = GetTemplateChild(PartHost) as WordWrapPanel;
        if (_host is not null) BuildLetters();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new ScrambledTextAutomationPeer(this);

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        StopRendering();
        ResetLetters();
    }

    private void BuildLetters()
    {
        if (_host is null) return;

        StopRendering();
        _host.Children.Clear();
        _letters.Clear();
        _waitingIntro = Intro && !_introPlayed;

        int word = 0;
        foreach (char ch in Text ?? string.Empty)
        {
            if (ch == '\r') continue;

            if (ch == '\n')
            {
                var lineBreak = new Grid { IsHitTestVisible = false };
                WordWrapPanel.SetKind(lineBreak, ScrambledTextLetterKind.Break);
                WordWrapPanel.SetWord(lineBreak, word++);
                _host.Children.Add(lineBreak);
                continue;
            }

            var letter = MakeLetter(ch);
            WordWrapPanel.SetWord(letter.Cell, word);
            WordWrapPanel.SetKind(letter.Cell, letter.IsSpace ? ScrambledTextLetterKind.Space : ScrambledTextLetterKind.Normal);
            if (letter.IsSpace) word++;
            if (_waitingIntro) letter.Glyph.Opacity = 0;

            _letters.Add(letter);
            _host.Children.Add(letter.Cell);
        }
    }

    private static ScrambledTextLetter MakeLetter(char ch)
    {
        string text = ch.ToString();
        bool isSpace = char.IsWhiteSpace(ch);
        var transform = new CompositeTransform();

        var sizer = new TextBlock
        {
            Text = isSpace ? "\u00A0" : text,
            Opacity = 0,
            IsHitTestVisible = false,
        };

        var glyph = new TextBlock
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsHitTestVisible = false,
        };

        var cell = new Grid
        {
            RenderTransform = transform,
            RenderTransformOrigin = new Point(0.5, 0.5),
            IsHitTestVisible = false,
        };
        cell.Children.Add(sizer);
        cell.Children.Add(glyph);

        return new ScrambledTextLetter
        {
            Cell = cell,
            Glyph = glyph,
            Transform = transform,
            Real = text,
            IsSpace = isSpace,
        };
    }

    private void UpdateScrambleGlyphs()
    {
        string source = string.IsNullOrEmpty(ScrambleChars) ? DefaultScrambleChars : ScrambleChars;

        _scrambleGlyphs = new string[source.Length];
        for (int i = 0; i < source.Length; i++) _scrambleGlyphs[i] = source[i].ToString();
    }

    private void RefreshScrambleColor()
    {
        foreach (var letter in _letters)
        {
            if (letter.ShowingScramble) SetScrambled(letter, true);
        }
    }

    private void OnViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
    {
        if (!_waitingIntro || ActualWidth <= 0) return;

        var viewport = args.EffectiveViewport;
        bool visible = viewport.Width > 0 && viewport.Height > 0
                       && viewport.Right > 0 && viewport.Bottom > 0
                       && viewport.Left < ActualWidth && viewport.Top < ActualHeight;

        if (visible) StartIntro();
    }

    private void StartIntro()
    {
        _introPlayed = true;
        _waitingIntro = false;

        double now = Now;
        int count = _letters.Count;

        for (int i = 0; i < count; i++)
        {
            var letter = _letters[i];
            letter.Glyph.Opacity = 1;
            if (letter.IsSpace) continue;

            letter.ScrambleUntil = now + IntroDelay + (count > 1 ? (double)i / (count - 1) : 0) * Duration;
            SetScrambled(letter, true);
            letter.Glyph.Text = RandomGlyph();
            letter.NextSwap = now + SwapInterval();
        }

        StartRendering();
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_host is null || _waitingIntro) return;

        var point = e.GetCurrentPoint(_host).Position;
        double radius = Math.Max(1, Radius);
        double now = Now;
        bool scramble = Effect == ScrambledTextEffect.Scramble;

        foreach (var letter in _letters)
        {
            if (letter.IsSpace) continue;

            double distance = Distance(letter, point);
            double t = distance < radius ? 1 - distance / radius : 0;
            double falloff = t * t * (3 - 2 * t);

            if (scramble)
            {
                if (falloff > 0.02) letter.ScrambleUntil = Math.Max(letter.ScrambleUntil, now + Duration * falloff);
            }
            else
            {
                letter.PointerTarget = falloff;
            }
        }

        StartRendering();
    }

    private void OnPointerLeft(object sender, PointerRoutedEventArgs e)
    {
        foreach (var letter in _letters) letter.PointerTarget = 0;
        StartRendering();
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!ClickBurst || _host is null || _waitingIntro) return;

        var point = e.GetCurrentPoint(_host).Position;
        double now = Now;

        foreach (var letter in _letters)
        {
            if (!letter.IsSpace) letter.PendingAt = now + Distance(letter, point) / WaveSpeed;
        }

        StartRendering();
    }

    private static double Distance(ScrambledTextLetter letter, Point point)
    {
        var slot = LayoutInformation.GetLayoutSlot(letter.Cell);
        double dx = slot.X + slot.Width / 2 - point.X;
        double dy = slot.Y + slot.Height / 2 - point.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private void StartRendering()
    {
        if (_rendering || _letters.Count == 0) return;

        _rendering = true;
        _hasLastFrame = false;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopRendering()
    {
        if (!_rendering) return;

        _rendering = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        TimeSpan frameTime = e.As<RenderingEventArgs>().RenderingTime;
        double dt = _hasLastFrame ? Math.Clamp((frameTime - _lastFrame).TotalSeconds, 0, MaxFrameTime) : 0;
        _lastFrame = frameTime;
        _hasLastFrame = true;
        _now += dt;

        double now = _now;
        double riseK = 1 - Math.Exp(-dt / RiseTime);
        double fallK = 1 - Math.Exp(-dt / Math.Max(0.01, Trail / 3));
        double interval = SwapInterval();
        bool scramble = Effect == ScrambledTextEffect.Scramble;
        bool busy = false;

        foreach (var letter in _letters)
        {
            if (!double.IsPositiveInfinity(letter.PendingAt))
            {
                if (now >= letter.PendingAt)
                {
                    letter.PendingAt = double.PositiveInfinity;
                    if (scramble) letter.ScrambleUntil = Math.Max(letter.ScrambleUntil, now + Duration * 0.5);
                    else letter.PulseUntil = now + PulseDuration;
                }
                else
                {
                    busy = true;
                }
            }

            if (!letter.IsSpace && now < letter.ScrambleUntil)
            {
                busy = true;
                if (!letter.ShowingScramble) SetScrambled(letter, true);

                if (now >= letter.NextSwap)
                {
                    letter.Glyph.Text = RandomGlyph();
                    letter.NextSwap = now + interval;
                }
            }
            else if (letter.ShowingScramble)
            {
                letter.Glyph.Text = letter.Real;
                SetScrambled(letter, false);
            }

            bool pulsing = now < letter.PulseUntil;
            if (pulsing) busy = true;

            double target = scramble ? 0 : Math.Max(letter.PointerTarget, pulsing ? 1 : 0);
            double diff = target - letter.Intensity;

            if (Math.Abs(diff) > IntensityEpsilon)
            {
                letter.Intensity += diff * (diff > 0 ? riseK : fallK);
                busy = true;
                ApplyEffect(letter);
            }
            else if (letter.Intensity != target)
            {
                letter.Intensity = target;
                ApplyEffect(letter);
            }
        }

        if (!busy) StopRendering();
    }

    private double SwapInterval() => BaseSwapInterval / Math.Max(0.05, Speed);

    private string RandomGlyph() => _scrambleGlyphs[Random.Shared.Next(_scrambleGlyphs.Length)];

    private void SetScrambled(ScrambledTextLetter letter, bool on)
    {
        letter.ShowingScramble = on;

        if (on && ScrambleColor is not null) letter.Glyph.Foreground = ScrambleColor;
        else letter.Glyph.ClearValue(TextBlock.ForegroundProperty);
    }

    private void ApplyEffect(ScrambledTextLetter letter)
    {
        double t = letter.Intensity;

        switch (Effect)
        {
            case ScrambledTextEffect.Lift:
                letter.Transform.TranslateY = -t * Strength * LiftDistance;
                break;

            case ScrambledTextEffect.Magnify:
                letter.Transform.ScaleX = letter.Transform.ScaleY = 1 + MagnifyGain * t * Strength;
                Canvas.SetZIndex(letter.Cell, (int)(t * 100));
                break;

            case ScrambledTextEffect.Blur:
                ApplyBlur(letter, t);
                break;
        }
    }

    private void ApplyBlur(ScrambledTextLetter letter, double t)
    {
        if (t < 0.002)
        {
            letter.Glyph.Opacity = 1;
            HideGhosts(letter);
            return;
        }

        EnsureGhosts(letter);

        double strength = Math.Min(1, Strength * t);
        double offset = BlurSpread * Strength * t;

        letter.Glyph.Opacity = 1 - BlurDimming * strength;

        for (int i = 0; i < BlurDirections.Length; i++)
        {
            var ghost = letter.Ghosts![i];
            ghost.Visibility = Visibility.Visible;
            ghost.Text = letter.Glyph.Text;
            ghost.Opacity = BlurOpacity * strength;
            letter.GhostTransforms![i].X = BlurDirections[i].X * offset;
            letter.GhostTransforms[i].Y = BlurDirections[i].Y * offset;
        }
    }

    private static void EnsureGhosts(ScrambledTextLetter letter)
    {
        if (letter.Ghosts is not null) return;

        letter.Ghosts = new TextBlock[BlurDirections.Length];
        letter.GhostTransforms = new TranslateTransform[BlurDirections.Length];

        for (int i = 0; i < BlurDirections.Length; i++)
        {
            letter.GhostTransforms[i] = new TranslateTransform();
            letter.Ghosts[i] = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
                RenderTransform = letter.GhostTransforms[i],
                Visibility = Visibility.Collapsed,
            };
            letter.Cell.Children.Insert(1 + i, letter.Ghosts[i]);
        }
    }

    private static void HideGhosts(ScrambledTextLetter letter)
    {
        if (letter.Ghosts is null) return;
        foreach (var ghost in letter.Ghosts) ghost.Visibility = Visibility.Collapsed;
    }

    private void ResetLetters()
    {
        foreach (var letter in _letters)
        {
            letter.Intensity = 0;
            letter.PointerTarget = 0;
            letter.PulseUntil = 0;
            letter.ScrambleUntil = 0;
            letter.PendingAt = double.PositiveInfinity;

            letter.Transform.TranslateY = 0;
            letter.Transform.ScaleX = letter.Transform.ScaleY = 1;
            Canvas.SetZIndex(letter.Cell, 0);

            letter.Glyph.Text = letter.Real;
            SetScrambled(letter, false);
            if (!_waitingIntro) letter.Glyph.Opacity = 1;
            HideGhosts(letter);
        }
    }
}
