
using System.ComponentModel;
using System.Reflection;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = TemplateBorderName, Type = typeof(Border))]
[TemplatePart(Name = TemplateRootName, Type = typeof(Canvas))]
public partial class LoadingIndicator : Control
{
    internal const string TemplateBorderName = "PART_Border";
    internal const string TemplateRootName = "PART_Root";
    private const string EpicStyleName = "LoadingIndicatorEpicStyle";

    private readonly List<Path> _paths = new();
    private readonly List<Canvas> _layers = new();
    private readonly List<Rectangle> _rects = new();
    private readonly List<Ellipse> _ellipses = new();

    protected Border PART_Border;

    private Canvas _root;
    private Storyboard _storyboard;
    private bool _loaded;
    private bool _built;

    public LoadingIndicator()
    {
        DefaultStyleKey = typeof(LoadingIndicator);
        AutomationProperties.SetName(this, "Loading");

        RegisterPropertyChangedCallback(ForegroundProperty, (_, _) => Rebuild());

        Loaded += (_, _) =>
        {
            _loaded = true;
            Rebuild();
            StartAnimation();
        };
        Unloaded += (_, _) =>
        {
            _loaded = false;
            _built = false;
            _storyboard?.Stop();
        };
    }

    private bool IsEpicMode => GetLoadingIndicatorModeDescription(Mode) == EpicStyleName;

    private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (LoadingIndicator)d;
        ctl.SetLoadingIndicatorMode((LoadingIndicatorMode)e.NewValue);

        if (ctl._root != null && ctl.IsEpicMode)
        {
            ctl.Rebuild();
            ctl.StartAnimation();
        }
    }

    private void SetLoadingIndicatorMode(LoadingIndicatorMode loadingIndicatorMode)
    {
        var styleName = GetLoadingIndicatorModeDescription(loadingIndicatorMode);
        if (string.IsNullOrWhiteSpace(styleName))
        {
            return;
        }

        var style = Application.Current.Resources[styleName] as Style;
        if (style is not null && Style != style)
        {
            Style = style;
        }
    }

    private static void OnSpeedRatioChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var li = (LoadingIndicator)o;

        if (li._root != null)
        {
            if (li._storyboard != null)
            {
                li._storyboard.SpeedRatio = (double)e.NewValue;
            }

            return;
        }

        if (li.PART_Border == null || li.IsActive == false)
        {
            return;
        }

        SetStoryBoardSpeedRatio(li.PART_Border, (double)e.NewValue);
    }

    private static void OnIsActiveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        var li = (LoadingIndicator)o;

        if (li.PART_Border == null)
        {
            li.ApplyTemplate();
        }

        if (li.PART_Border == null)
        {
            return;
        }

        if ((bool)e.NewValue == false)
        {
            VisualStateManager.GoToState(li, IndicatorVisualStateNames.InactiveState.Name, false);
            li.PART_Border.SetValue(VisibilityProperty, Visibility.Collapsed);
            li._storyboard?.Stop();
        }
        else
        {
            VisualStateManager.GoToState(li, IndicatorVisualStateNames.ActiveState.Name, false);
            li.PART_Border.SetValue(VisibilityProperty, Visibility.Visible);

            if (li._root != null)
            {
                li.StartAnimation();
            }
            else
            {
                SetStoryBoardSpeedRatio(li.PART_Border, li.SpeedRatio);
            }
        }
    }

    private static void SetStoryBoardSpeedRatio(FrameworkElement element, double speedRatio)
    {
        var activeStates = element.GetActiveVisualStates();
        foreach (var activeState in activeStates)
        {
            activeState.Storyboard.SpeedRatio = speedRatio;
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        SetLoadingIndicatorMode(Mode);

        _storyboard?.Stop();
        PART_Border = (Border)GetTemplateChild(TemplateBorderName);
        _root = GetTemplateChild(TemplateRootName) as Canvas;

        if (PART_Border == null)
        {
            return;
        }

        VisualStateManager.GoToState(this,
            IsActive
                ? IndicatorVisualStateNames.ActiveState.Name
                : IndicatorVisualStateNames.InactiveState.Name, false);

        if (_root != null)
        {
            Rebuild();
            StartAnimation();
        }
        else
        {
            SetStoryBoardSpeedRatio(PART_Border, SpeedRatio);
        }

        PART_Border.SetValue(VisibilityProperty, IsActive ? Visibility.Visible : Visibility.Collapsed);
    }

    public string GetLoadingIndicatorModeDescription(LoadingIndicatorMode value)
    {
        return
            value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault()
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description;
    }

    private static (double Size, double Duration) Defaults(LoadingIndicatorMode type) => type switch
    {
        LoadingIndicatorMode.Atom => (60, 1000),
        LoadingIndicatorMode.Flower => (70, 2500),
        LoadingIndicatorMode.Pixel => (70, 2000),
        LoadingIndicatorMode.HollowDots => (0, 1000),
        LoadingIndicatorMode.IntersectingCircles => (70, 1200),
        LoadingIndicatorMode.Orbit => (55, 1200),
        LoadingIndicatorMode.Radar => (60, 2000),
        LoadingIndicatorMode.ScalingSquares => (65, 1250),
        LoadingIndicatorMode.HalfCircle => (60, 1000),
        LoadingIndicatorMode.TrinityRings => (66, 1500),
        LoadingIndicatorMode.FulfillingSquare => (50, 4000),
        LoadingIndicatorMode.CirclesToRhombuses => (0, 1200),
        LoadingIndicatorMode.Semipolar => (65, 2000),
        LoadingIndicatorMode.SelfBuildingSquare => (40, 6000),
        LoadingIndicatorMode.SwappingSquares => (65, 1000),
        LoadingIndicatorMode.FulfillingBouncingCircle => (60, 4000),
        LoadingIndicatorMode.Fingerprint => (64, 1500),
        LoadingIndicatorMode.Spring => (60, 3000),
        LoadingIndicatorMode.LoopingRhombuses => (0, 2500),
        LoadingIndicatorMode.BreedingRhombus => (65, 2000),
        _ => (60, 1000),
    };

    private double S => double.IsNaN(IndicatorSize) ? Defaults(Mode).Size : IndicatorSize;

    private double Duration => Math.Max(1, double.IsNaN(AnimationDuration) ? Defaults(Mode).Duration : AnimationDuration);

    private double D => ItemSize;

    private int N => Math.Max(1, ItemCount);

    private Windows.Foundation.Size Extent() => Mode switch
    {
        LoadingIndicatorMode.HollowDots => new Windows.Foundation.Size(N * D * 1.5, D * 3),
        LoadingIndicatorMode.CirclesToRhombuses => new Windows.Foundation.Size(N * D + (N - 1) * D * 1.125, D),
        LoadingIndicatorMode.LoopingRhombuses => new Windows.Foundation.Size(D * 4, D * 1.5),
        _ => new Windows.Foundation.Size(S, S),
    };

    private static void OnRebuild(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((LoadingIndicator)d).Rebuild();

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((LoadingIndicator)d).StartAnimation();

    private static void OnPhaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var s = (LoadingIndicator)d;
        if (s._loaded && s._built)
        {
            s.Update((double)e.NewValue);
        }
    }

    private void Rebuild()
    {
        if (!_loaded || _root is null)
        {
            return;
        }

        _built = false;
        _root.Children.Clear();
        _paths.Clear();
        _layers.Clear();
        _rects.Clear();
        _ellipses.Clear();

        var extent = Extent();
        _root.Width = extent.Width;
        _root.Height = extent.Height;

        Build();
        _built = true;
        Update(Phase);
    }

    private void StartAnimation()
    {
        _storyboard?.Stop();
        if (!_loaded || !IsActive || _root is null)
        {
            return;
        }

        var animation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(Duration)),
            RepeatBehavior = RepeatBehavior.Forever,
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, this);
        Storyboard.SetTargetProperty(animation, nameof(Phase));

        _storyboard = new Storyboard { SpeedRatio = SpeedRatio };
        _storyboard.Children.Add(animation);
        _storyboard.Begin();
    }

    private void Build()
    {
        switch (Mode)
        {
            case LoadingIndicatorMode.Atom: BuildAtom(); break;
            case LoadingIndicatorMode.Flower: BuildFlower(); break;
            case LoadingIndicatorMode.Pixel: BuildPixel(); break;
            case LoadingIndicatorMode.HollowDots: BuildHollowDots(); break;
            case LoadingIndicatorMode.IntersectingCircles: BuildIntersectingCircles(); break;
            case LoadingIndicatorMode.Orbit: BuildOrbit(); break;
            case LoadingIndicatorMode.Radar: BuildRadar(); break;
            case LoadingIndicatorMode.ScalingSquares: BuildScalingSquares(); break;
            case LoadingIndicatorMode.HalfCircle: BuildHalfCircle(); break;
            case LoadingIndicatorMode.TrinityRings: BuildTrinityRings(); break;
            case LoadingIndicatorMode.FulfillingSquare: BuildFulfillingSquare(); break;
            case LoadingIndicatorMode.CirclesToRhombuses: BuildCirclesToRhombuses(); break;
            case LoadingIndicatorMode.Semipolar: BuildSemipolar(); break;
            case LoadingIndicatorMode.SelfBuildingSquare: BuildSelfBuildingSquare(); break;
            case LoadingIndicatorMode.SwappingSquares: BuildSwappingSquares(); break;
            case LoadingIndicatorMode.FulfillingBouncingCircle: BuildFulfillingBouncingCircle(); break;
            case LoadingIndicatorMode.Fingerprint: BuildFingerprint(); break;
            case LoadingIndicatorMode.Spring: BuildSpring(); break;
            case LoadingIndicatorMode.LoopingRhombuses: BuildLoopingRhombuses(); break;
            case LoadingIndicatorMode.BreedingRhombus: BuildBreedingRhombus(); break;
        }
    }

    private void Update(double t)
    {
        switch (Mode)
        {
            case LoadingIndicatorMode.Atom: UpdateAtom(t); break;
            case LoadingIndicatorMode.Flower: UpdateFlower(t); break;
            case LoadingIndicatorMode.Pixel: UpdatePixel(t); break;
            case LoadingIndicatorMode.HollowDots: UpdateHollowDots(t); break;
            case LoadingIndicatorMode.IntersectingCircles: UpdateIntersectingCircles(t); break;
            case LoadingIndicatorMode.Orbit: UpdateOrbit(t); break;
            case LoadingIndicatorMode.Radar: UpdateRadar(t); break;
            case LoadingIndicatorMode.ScalingSquares: UpdateScalingSquares(t); break;
            case LoadingIndicatorMode.HalfCircle: UpdateHalfCircle(t); break;
            case LoadingIndicatorMode.TrinityRings: UpdateTrinityRings(t); break;
            case LoadingIndicatorMode.FulfillingSquare: UpdateFulfillingSquare(t); break;
            case LoadingIndicatorMode.CirclesToRhombuses: UpdateCirclesToRhombuses(t); break;
            case LoadingIndicatorMode.Semipolar: UpdateSemipolar(t); break;
            case LoadingIndicatorMode.SelfBuildingSquare: UpdateSelfBuildingSquare(t); break;
            case LoadingIndicatorMode.SwappingSquares: UpdateSwappingSquares(t); break;
            case LoadingIndicatorMode.FulfillingBouncingCircle: UpdateFulfillingBouncingCircle(t); break;
            case LoadingIndicatorMode.Fingerprint: UpdateFingerprint(t); break;
            case LoadingIndicatorMode.Spring: UpdateSpring(t); break;
            case LoadingIndicatorMode.LoopingRhombuses: UpdateLoopingRhombuses(t); break;
            case LoadingIndicatorMode.BreedingRhombus: UpdateBreedingRhombus(t); break;
        }
    }

    // ---- Atom ----

    private void BuildAtom()
    {
        for (int i = 0; i < 3; i++)
        {
            AddPath(_root, S / 25);
        }

        AddEllipse(_root, S * 0.4, S * 0.4, S * 0.2, S * 0.2);
    }

    private void UpdateAtom(double t)
    {
        for (int i = 0; i < 3; i++)
        {
            SetProjectedArc(_paths[i], S / 2, S / 2, S / 2 - S / 50, 225, 180, 360 * t, 0, 66, 120 * i);
        }
    }

    // ---- Flower ----

    private const int Petals = 8;

    private void BuildFlower()
    {
        var ring = AddLayer(_root, S, S);
        double d = S * 0.14;
        for (int i = 0; i < Petals; i++)
        {
            AddEllipse(ring, S / 2 - d / 2, S / 2 - d / 2, d, d);
        }

        AddEllipse(_root, S / 2 - S * 0.1, S / 2 - S * 0.1, S * 0.2, S * 0.2);
    }

    private void UpdateFlower(double t)
    {
        double r = Keys(t, true, (0, 0.14), (0.5, 0.34), (1, 0.14)) * S;
        double scale = Keys(t, true, (0, 1), (0.5, 0.7), (1, 1));
        Xf(_layers[0]).Rotation = Keys(t, true, (0, 0), (1, 180));
        for (int i = 0; i < Petals; i++)
        {
            double a = Rad(i * 360.0 / Petals);
            var x = Xf(_ellipses[i]);
            x.TranslateX = r * Math.Sin(a);
            x.TranslateY = -r * Math.Cos(a);
            x.ScaleX = x.ScaleY = scale;
        }

        var c = Xf(_ellipses[Petals]);
        c.ScaleX = c.ScaleY = Keys(t, true, (0, 1), (0.5, 0.55), (1, 1));
    }

    // ---- Pixel ----

    private void BuildPixel()
    {
        double p = S / 7;
        var group = AddLayer(_root, S, S);
        for (int i = 0; i < 9; i++)
        {
            AddRect(group, S / 2 - p / 2, S / 2 - p / 2, p, p);
        }
    }

    private void UpdatePixel(double t)
    {
        double p = S / 7;
        double d = Keys(t, true, (0, 1), (0.5, 3), (1, 1)) * p;
        Xf(_layers[0]).Rotation = Keys(t, true, (0, 0), (1, 90));
        for (int i = 0; i < 9; i++)
        {
            var x = Xf(_rects[i]);
            x.TranslateX = (i % 3 - 1) * d;
            x.TranslateY = (i / 3 - 1) * d;
        }
    }

    // ---- Hollow dots ----

    private void BuildHollowDots()
    {
        for (int i = 0; i < N; i++)
        {
            AddEllipse(_root, i * D * 1.5 + D * 0.25, D * 2, D, D, D / 5);
        }
    }

    private void UpdateHollowDots(double t)
    {
        for (int i = 0; i < _ellipses.Count; i++)
        {
            double tau = Frac(t - i * 0.15);
            Xf(_ellipses[i]).TranslateY = Keys(tau, true, (0, 0), (0.5, -D), (1, 0));
        }
    }

    // ---- Intersecting circles ----

    private void BuildIntersectingCircles()
    {
        AddLayer(_root, S, S);
        double d = S / 2, th = S / 20;
        for (int i = 0; i < 7; i++)
        {
            double cx = S / 2, cy = S / 2;
            if (i > 0)
            {
                double a = Rad((i - 1) * 60.0);
                cx += S / 4 * Math.Sin(a);
                cy -= S / 4 * Math.Cos(a);
            }

            var group = AddLayer(_layers[0], d, d, cx - d / 2, cy - d / 2);
            AddArc(group, d / 2, d / 2, d / 2 - th / 2, 225, 90, th);
            AddArc(group, d / 2, d / 2, d / 2 - th / 2, 45, 90, th);
        }
    }

    private void UpdateIntersectingCircles(double t) => Xf(_layers[0]).Rotation = 360 * t;

    // ---- Orbit ----

    private static readonly (double Rx, double Ry, double Start)[] OrbitRings =
    {
        (35, -45, 135),
        (50, 10, 45),
        (35, 55, -45),
    };

    private void BuildOrbit()
    {
        for (int i = 0; i < 3; i++)
        {
            AddPath(_root, S / 18);
        }
    }

    private void UpdateOrbit(double t)
    {
        double r = S / 2 - S / 36;
        for (int i = 0; i < 3; i++)
        {
            var ring = OrbitRings[i];
            SetProjectedArc(_paths[i], S / 2, S / 2, r, ring.Start, 90, 360 * t, ring.Ry, ring.Rx, 0);
        }
    }

    // ---- Radar ----

    private void BuildRadar()
    {
        double th = S / 15;
        for (int i = 0; i < 4; i++)
        {
            var layer = AddLayer(_root, S, S);
            AddArc(layer, S / 2, S / 2, S / 2 * (1 - 0.2 * i) - th / 2, 225, 180, th);
        }
    }

    private void UpdateRadar(double t)
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            double tau = Frac(t - i * 0.06);
            Xf(_layers[i]).Rotation = 360 * tau * (i % 2 == 0 ? 1 : -1);
        }
    }

    // ---- Scaling squares ----

    private void BuildScalingSquares()
    {
        var group = AddLayer(_root, S, S);
        double q = S * 0.3, far = S * 0.7;
        foreach (var (x, y) in new[] { (0.0, 0.0), (far, 0.0), (0.0, far), (far, far) })
        {
            AddRect(group, x, y, q, q);
        }
    }

    private void UpdateScalingSquares(double t)
    {
        Xf(_layers[0]).Rotation = Keys(t, true, (0, 0), (1, 180));
        double scale = Keys(t, true, (0, 1), (0.5, 0.55), (1, 1));
        foreach (var sq in _rects)
        {
            var x = Xf(sq);
            x.ScaleX = x.ScaleY = scale;
        }
    }

    // ---- Half circle ----

    private void BuildHalfCircle()
    {
        double th = S / 10, r = S / 2 - th / 2;
        AddArc(AddLayer(_root, S, S), S / 2, S / 2, r, -45, 90, th);
        AddArc(AddLayer(_root, S, S), S / 2, S / 2, r, 135, 90, th);
    }

    private void UpdateHalfCircle(double t)
    {
        Xf(_layers[0]).Rotation = 360 * t;
        Xf(_layers[1]).Rotation = -360 * t;
    }

    // ---- Trinity rings ----

    private void BuildTrinityRings()
    {
        for (int i = 0; i < 3; i++)
        {
            AddPath(_root, S / 30);
        }
    }

    private void UpdateTrinityRings(double t)
    {
        double c = S / 2, th = S / 30;
        double r1 = S / 2 - th, r2 = S * 0.65 / 2, r3 = S * 0.3 / 2;
        SetProjectedArc(_paths[0], c, c, r1, 0, 360, 0, 360 * t, 0, 20 + 80 * t);
        SetProjectedArc(_paths[1], c, c, r2, 0, 360, 0, 0, 360 * t, 100 - 100 * t);
        SetProjectedArc(_paths[2], c, c, r3, 0, 360, 0, 0, -360 * (1 - t), 100 - 460 * t);
    }

    // ---- Fulfilling square ----

    private void BuildFulfillingSquare()
    {
        double b = S * 0.04;
        var layer = AddLayer(_root, S, S);
        AddRect(layer, 0, 0, S, S, b);
        AddRect(layer, b, S - b, S - 2 * b, 0);
    }

    private void UpdateFulfillingSquare(double t)
    {
        double b = S * 0.04, full = S - 2 * b;
        double h = Keys(t, true, (0, 0), (0.1, 0), (0.4, 1), (0.6, 1), (0.9, 0), (1, 0)) * full;
        _rects[1].Height = h;
        Canvas.SetTop(_rects[1], S - b - h);
        Xf(_layers[0]).Rotation = Keys(t, true, (0, 0), (0.1, 0), (0.5, 180), (0.9, 360), (1, 360));
    }

    // ---- Circles to rhombuses ----

    private void BuildCirclesToRhombuses()
    {
        double gap = D * 1.125;
        for (int i = 0; i < N; i++)
        {
            AddRect(_root, i * (D + gap), 0, D, D, D / 5, D / 2);
        }
    }

    private void UpdateCirclesToRhombuses(double t)
    {
        for (int i = 0; i < _rects.Count; i++)
        {
            double tau = Frac(t - i * 0.12);
            double morph = Keys(tau, true, (0, 0), (0.5, 1), (1, 0));
            var r = _rects[i];
            r.RadiusX = r.RadiusY = D / 2 * (1 - morph);
            var x = Xf(r);
            x.Rotation = 90 * tau;
            x.ScaleX = x.ScaleY = Lerp(1, 0.75, morph);
        }
    }

    // ---- Semipolar ----

    private void BuildSemipolar()
    {
        double th = S / 20;
        for (int i = 0; i < 5; i++)
        {
            var layer = AddLayer(_root, S, S);
            AddArc(layer, S / 2, S / 2, S / 2 * (1 - 0.2 * i) - th / 2, 225, 180, th);
        }
    }

    private void UpdateSemipolar(double t)
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            double tau = Frac(t - i * 0.05);
            var x = Xf(_layers[i]);
            x.Rotation = Keys(tau, true, (0, 0), (0.5, 360), (1, 0));
            x.ScaleX = x.ScaleY = Keys(tau, true, (0, 1), (0.5, 0.7), (1, 1));
        }
    }

    // ---- Self-building square ----

    private void BuildSelfBuildingSquare()
    {
        double q = S / 4, gap = S / 12;
        for (int i = 0; i < 9; i++)
        {
            AddRect(_root, i % 3 * (q + gap), i / 3 * (q + gap), q, q);
        }
    }

    private void UpdateSelfBuildingSquare(double t)
    {
        for (int i = 0; i < _rects.Count; i++)
        {
            double tau = Frac(t - i * 0.04);
            _rects[i].Opacity = Keys(tau, false, (0, 0), (0.05, 1), (0.51, 1), (0.56, 0), (1, 0));
            Xf(_rects[i]).TranslateY = Keys(tau, false, (0, -S / 4), (0.05, 0), (1, 0));
        }
    }

    // ---- Swapping squares ----

    private void BuildSwappingSquares()
    {
        double q = S * 0.4, far = S * 0.6, th = S / 16;
        foreach (var (x, y) in new[] { (0.0, 0.0), (far, 0.0), (0.0, far), (far, far) })
        {
            AddRect(_root, x, y, q, q, th);
        }
    }

    private void UpdateSwappingSquares(double t)
    {
        for (int i = 0; i < _rects.Count; i++)
        {
            double tau = Frac(t - (i == 0 || i == 3 ? 0.5 : 0));
            var x = Xf(_rects[i]);
            x.Rotation = Keys(tau, true, (0, 0), (0.5, 90), (1, 180));
            x.ScaleX = x.ScaleY = Keys(tau, true, (0, 1), (0.5, 0.5), (1, 1));
        }
    }

    // ---- Fulfilling bouncing circle ----

    private void BuildFulfillingBouncingCircle()
    {
        double th = S / 16, d = S - 4 * th;
        AddEllipse(_root, 0, 0, S, S, th);
        AddEllipse(_root, (S - d) / 2, (S - d) / 2, d, d);
    }

    private void UpdateFulfillingBouncingCircle(double t)
    {
        double s = t < 0.5 ? EaseOutBounce(t / 0.5) : Keys(t, true, (0.5, 1), (0.85, 1), (1, 0));
        var x = Xf(_ellipses[1]);
        x.ScaleX = x.ScaleY = Math.Max(0, s);
    }

    // ---- Fingerprint ----

    private const int FingerprintRings = 9;

    private void BuildFingerprint()
    {
        double th = S / 32;
        for (int n = 1; n <= FingerprintRings; n++)
        {
            var layer = AddLayer(_root, S, S);
            AddArc(layer, S / 2, S / 2, S / 2 * n / FingerprintRings - th / 2, -45, 90, th);
        }
    }

    private void UpdateFingerprint(double t)
    {
        for (int i = 0; i < _layers.Count; i++)
        {
            double tau = Frac(t - (i + 1) * 50.0 / Duration);
            Xf(_layers[i]).Rotation = 360 * CubicBezier(0.68, -0.75, 0.265, 1.75, tau);
        }
    }

    // ---- Spring ----

    private void BuildSpring()
    {
        for (int k = 0; k < 2; k++)
        {
            var clip = new Canvas
            {
                Width = S,
                Height = S / 2,
                Clip = new RectangleGeometry { Rect = new Rect(0, 0, S, S / 2) },
            };
            Canvas.SetLeft(clip, 0);
            Canvas.SetTop(clip, k == 0 ? 0 : S / 2);
            _root.Children.Add(clip);

            var flip = AddLayer(clip, S, S, 0, k == 0 ? 0 : -S / 2);
            if (k == 1)
            {
                Xf(flip).ScaleY = -1;
            }

            var spin = AddLayer(flip, S, S);
            AddPath(spin, S / 7);
        }
    }

    private void UpdateSpring(double t)
    {
        double bw = Keys(t, true, (0, S / 7), (0.25, S / 23.33), (0.5, S / 7), (0.75, S / 23.33), (1, S / 7));
        double rot = Keys(t, true, (0, -200), (0.5, 115), (1, -200));
        for (int k = 0; k < 2; k++)
        {
            _paths[k].StrokeThickness = bw;
            _paths[k].Data = ArcGeometry(S / 2, S / 2, S / 2 - bw / 2, -45, 180);
            Xf(_layers[2 * k + 1]).Rotation = rot;
        }
    }

    // ---- Looping rhombuses ----

    private void BuildLoopingRhombuses()
    {
        for (int i = 0; i < 3; i++)
        {
            AddRect(_root, 0, (D * 1.5 - D) / 2, D, D);
        }
    }

    private void UpdateLoopingRhombuses(double t)
    {
        for (int i = 0; i < _rects.Count; i++)
        {
            double tau = Frac(t + i / 3.0);
            var x = Xf(_rects[i]);
            x.TranslateX = (3.0 - 4.66 * tau) * D;
            x.Rotation = 45;
            x.ScaleX = x.ScaleY = Keys(tau, false, (0, 0), (0.5, 1), (1, 0));
        }
    }

    // ---- Breeding rhombus ----

    private const int SmallRhombi = 8;

    private void BuildBreedingRhombus()
    {
        double s = S / 7.5, b = S / 3.25;
        for (int i = 0; i < SmallRhombi; i++)
        {
            double a = Rad(i * 360.0 / SmallRhombi);
            double cx = S / 2 + S * 0.4 * Math.Sin(a), cy = S / 2 - S * 0.4 * Math.Cos(a);
            AddRect(_root, cx - s / 2, cy - s / 2, s, s);
        }

        AddRect(_root, S / 2 - b / 2, S / 2 - b / 2, b, b);
    }

    private void UpdateBreedingRhombus(double t)
    {
        for (int i = 0; i < SmallRhombi; i++)
        {
            double tau = Frac(t - 100.0 * (i + 1) / Duration);
            var x = Xf(_rects[i]);
            x.Rotation = 45;
            x.ScaleX = x.ScaleY = Keys(tau, true, (0, 0), (0.5, 1), (1, 0));
        }

        var big = Xf(_rects[SmallRhombi]);
        big.Rotation = 45;
        big.ScaleX = big.ScaleY = Keys(Frac(t - 0.25), true, (0, 0), (0.5, 1), (1, 0));
    }

    // ---- Math ----

    private static double Rad(double deg) => deg * Math.PI / 180;

    private static double Frac(double x) => x - Math.Floor(x);

    private static double Lerp(double a, double b, double t) => a + (b - a) * t;

    private static double Smooth(double t)
        => t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;

    private static double Keys(double t, bool smooth, params (double At, double Value)[] k)
    {
        t = Math.Clamp(t, 0, 1);
        if (t <= k[0].At)
        {
            return k[0].Value;
        }

        for (int i = 1; i < k.Length; i++)
        {
            if (t <= k[i].At)
            {
                double u = (t - k[i - 1].At) / (k[i].At - k[i - 1].At);
                return Lerp(k[i - 1].Value, k[i].Value, smooth ? Smooth(u) : u);
            }
        }

        return k[^1].Value;
    }

    private static double CubicBezier(double x1, double y1, double x2, double y2, double x)
    {
        static double B(double p1, double p2, double s)
        {
            double u = 1 - s;
            return 3 * u * u * s * p1 + 3 * u * s * s * p2 + s * s * s;
        }

        static double Db(double p1, double p2, double s)
        {
            double u = 1 - s;
            return 3 * u * u * p1 + 6 * u * s * (p2 - p1) + 3 * s * s * (1 - p2);
        }

        x = Math.Clamp(x, 0, 1);
        double t = x;
        for (int i = 0; i < 10; i++)
        {
            double err = B(x1, x2, t) - x;
            double d = Db(x1, x2, t);
            if (Math.Abs(err) < 1e-5 || Math.Abs(d) < 1e-6)
            {
                break;
            }

            t -= err / d;
        }

        return B(y1, y2, Math.Clamp(t, 0, 1));
    }

    private static double EaseOutBounce(double x)
    {
        const double n1 = 7.5625, d1 = 2.75;
        if (x < 1 / d1) return n1 * x * x;
        if (x < 2 / d1) { x -= 1.5 / d1; return n1 * x * x + 0.75; }
        if (x < 2.5 / d1) { x -= 2.25 / d1; return n1 * x * x + 0.9375; }
        x -= 2.625 / d1;
        return n1 * x * x + 0.984375;
    }

    // ---- Visual helpers ----

    private Brush Paint() => Foreground;

    private static CompositeTransform Xf(UIElement e) => (CompositeTransform)e.RenderTransform;

    private Canvas AddLayer(Canvas parent, double w, double h, double x = 0, double y = 0)
    {
        var c = new Canvas
        {
            Width = w,
            Height = h,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = new CompositeTransform(),
        };
        Canvas.SetLeft(c, x);
        Canvas.SetTop(c, y);
        parent.Children.Add(c);
        _layers.Add(c);
        return c;
    }

    private Rectangle AddRect(Canvas parent, double x, double y, double w, double h, double stroke = 0, double radius = 0)
    {
        var r = new Rectangle
        {
            RadiusX = radius,
            RadiusY = radius,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = new CompositeTransform(),
        };
        if (stroke > 0)
        {
            r.Stroke = Paint();
            r.StrokeThickness = stroke;
            r.Width = Math.Max(0, w - stroke);
            r.Height = Math.Max(0, h - stroke);
            Canvas.SetLeft(r, x + stroke / 2);
            Canvas.SetTop(r, y + stroke / 2);
        }
        else
        {
            r.Fill = Paint();
            r.Width = w;
            r.Height = h;
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y);
        }

        parent.Children.Add(r);
        _rects.Add(r);
        return r;
    }

    private Ellipse AddEllipse(Canvas parent, double x, double y, double w, double h, double stroke = 0)
    {
        var e = new Ellipse
        {
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = new CompositeTransform(),
        };
        if (stroke > 0)
        {
            e.Stroke = Paint();
            e.StrokeThickness = stroke;
            e.Width = Math.Max(0, w - stroke);
            e.Height = Math.Max(0, h - stroke);
            Canvas.SetLeft(e, x + stroke / 2);
            Canvas.SetTop(e, y + stroke / 2);
        }
        else
        {
            e.Fill = Paint();
            e.Width = w;
            e.Height = h;
            Canvas.SetLeft(e, x);
            Canvas.SetTop(e, y);
        }

        parent.Children.Add(e);
        _ellipses.Add(e);
        return e;
    }

    private Path AddPath(Canvas parent, double thickness)
    {
        var p = new Path { Stroke = Paint(), StrokeThickness = thickness };
        parent.Children.Add(p);
        _paths.Add(p);
        return p;
    }

    private Path AddArc(Canvas parent, double cx, double cy, double r, double startDeg, double sweepDeg, double thickness)
    {
        var p = AddPath(parent, thickness);
        p.Data = ArcGeometry(cx, cy, r, startDeg, sweepDeg);
        return p;
    }

    private static Point Pt(double cx, double cy, double r, double deg)
    {
        double a = Rad(deg);
        return new Point(cx + r * Math.Sin(a), cy - r * Math.Cos(a));
    }

    private static PathGeometry ArcGeometry(double cx, double cy, double r, double startDeg, double sweepDeg)
    {
        r = Math.Max(0.01, r);
        var size = new Windows.Foundation.Size(r, r);
        var fig = new PathFigure { StartPoint = Pt(cx, cy, r, startDeg), IsClosed = false };
        if (sweepDeg >= 359.9)
        {
            fig.Segments.Add(new ArcSegment { Point = Pt(cx, cy, r, startDeg + 180), Size = size, SweepDirection = SweepDirection.Clockwise });
            fig.Segments.Add(new ArcSegment { Point = Pt(cx, cy, r, startDeg + 360), Size = size, SweepDirection = SweepDirection.Clockwise });
        }
        else
        {
            fig.Segments.Add(new ArcSegment
            {
                Point = Pt(cx, cy, r, startDeg + sweepDeg),
                Size = size,
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = sweepDeg > 180,
            });
        }

        var g = new PathGeometry();
        g.Figures.Add(fig);
        return g;
    }

    private static Point Project(double x, double y, double spinDeg, double ryDeg, double rxDeg, double outerDeg)
    {
        double z = 0;
        double c = Math.Cos(Rad(spinDeg)), s = Math.Sin(Rad(spinDeg));
        (x, y) = (x * c - y * s, x * s + y * c);
        c = Math.Cos(Rad(ryDeg)); s = Math.Sin(Rad(ryDeg));
        (x, z) = (x * c + z * s, -x * s + z * c);
        c = Math.Cos(Rad(rxDeg)); s = Math.Sin(Rad(rxDeg));
        (y, z) = (y * c - z * s, y * s + z * c);
        c = Math.Cos(Rad(outerDeg)); s = Math.Sin(Rad(outerDeg));
        (x, y) = (x * c - y * s, x * s + y * c);
        return new Point(x, y);
    }

    private static void SetProjectedArc(
        Path p, double cx, double cy, double r, double startDeg, double sweepDeg,
        double spinDeg, double ryDeg, double rxDeg, double outerDeg)
    {
        int n = Math.Max(2, (int)(sweepDeg / 8));
        var fig = new PathFigure { IsClosed = sweepDeg >= 359.9 };
        var seg = new PolyLineSegment();
        for (int i = 0; i <= n; i++)
        {
            double a = Rad(startDeg + sweepDeg * i / n);
            var q = Project(r * Math.Sin(a), -r * Math.Cos(a), spinDeg, ryDeg, rxDeg, outerDeg);
            var pt = new Point(cx + q.X, cy + q.Y);
            if (i == 0)
            {
                fig.StartPoint = pt;
            }
            else
            {
                seg.Points.Add(pt);
            }
        }

        fig.Segments.Add(seg);
        var g = new PathGeometry();
        g.Figures.Add(fig);
        p.Data = g;
    }
}
