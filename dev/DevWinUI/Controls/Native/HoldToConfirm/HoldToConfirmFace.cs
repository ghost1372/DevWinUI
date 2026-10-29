using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

internal sealed partial class HoldToConfirmFace
{
    private const double SlotWidth = 24;

    private Storyboard? _sb;

    private HoldToConfirmFace(Border root, Grid iconSlot, FontIcon glyph, Path tick, Grid labelHost, TextBlock labelA, TextBlock labelB)
    {
        Root = root;
        IconSlot = iconSlot;
        Glyph = glyph;
        Tick = tick;
        LabelHost = labelHost;
        LabelA = labelA;
        LabelB = labelB;
    }

    public Border Root { get; }
    public Grid IconSlot { get; }
    public FontIcon Glyph { get; }
    public Path Tick { get; }
    public Grid LabelHost { get; }
    public TextBlock LabelA { get; }
    public TextBlock LabelB { get; }

    public static HoldToConfirmFace? Create(Func<string, DependencyObject?> get, string prefix)
    {
        if (get(prefix + "Face") is not Border root) return null;
        if (get(prefix + "IconSlot") is not Grid iconSlot) return null;
        if (get(prefix + "Glyph") is not FontIcon glyph) return null;
        if (get(prefix + "Tick") is not Path tick) return null;
        if (get(prefix + "LabelHost") is not Grid labelHost) return null;
        if (get(prefix + "LabelA") is not TextBlock labelA) return null;
        if (get(prefix + "LabelB") is not TextBlock labelB) return null;

        HoldToConfirmFx.ClipTo(iconSlot, 2);
        HoldToConfirmFx.ClipTo(labelHost, 8);
        return new HoldToConfirmFace(root, iconSlot, glyph, tick, labelHost, labelA, labelB);
    }

    public void Snap(bool done, bool hasGlyph)
    {
        _sb?.Stop();
        _sb = null;

        var inc = done ? LabelB : LabelA;
        var outl = done ? LabelA : LabelB;
        LabelHost.Width = double.NaN;
        inc.Visibility = Visibility.Visible;
        outl.Visibility = Visibility.Collapsed;
        HoldToConfirmFx.Tween(inc, 1, 1, 0, 0, 0);

        IconSlot.Width = done || hasGlyph ? SlotWidth : 0;
        Glyph.Visibility = hasGlyph && !done ? Visibility.Visible : Visibility.Collapsed;
        HoldToConfirmFx.Tween(Glyph, 1, 1, 0, 0, 0);
        Tick.Visibility = done ? Visibility.Visible : Visibility.Collapsed;
        Tick.StrokeDashOffset = done ? 0 : 8;
        HoldToConfirmFx.Tween(Tick, 1, 1, 0, 0, 0);
    }

    public void Morph(bool done, bool hasGlyph)
    {
        _sb?.Stop();
        _sb = null;

        var inc = done ? LabelB : LabelA;
        var outl = done ? LabelA : LabelB;

        double fromW = LabelHost.ActualWidth;
        double slotFrom = IconSlot.ActualWidth;
        double slotTo = done || hasGlyph ? SlotWidth : 0;

        inc.Visibility = Visibility.Visible;
        inc.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
        double toW = inc.DesiredSize.Width;

        LabelHost.Width = toW;
        IconSlot.Width = slotTo;

        HoldToConfirmFx.Tween(outl, 1, 0, 0, -6, 160);
        HoldToConfirmFx.Tween(inc, 0, 1, 6, 0, 220, 60);

        var sb = new Storyboard();
        sb.Children.Add(HoldToConfirmFx.Dependent(LabelHost, "Width", fromW, toW, 260));
        if (Math.Abs(slotFrom - slotTo) > 0.5) sb.Children.Add(HoldToConfirmFx.Dependent(IconSlot, "Width", slotFrom, slotTo, 260));

        if (done)
        {
            if (hasGlyph) HoldToConfirmFx.Tween(Glyph, 1, 0, 0, 0, 140);
            Tick.Visibility = Visibility.Visible;
            Tick.StrokeDashOffset = 0;
            HoldToConfirmFx.Tween(Tick, 0, 1, 0, 0, 80, 80);
            sb.Children.Add(HoldToConfirmFx.Dependent(Tick, "StrokeDashOffset", 8, 0, 300, 100));
        }
        else
        {
            HoldToConfirmFx.Tween(Tick, 1, 0, 0, 0, 140);
            if (hasGlyph)
            {
                Glyph.Visibility = Visibility.Visible;
                HoldToConfirmFx.Tween(Glyph, 0, 1, 0, 0, 160, 80);
            }
        }

        sb.Completed += (_, _) =>
        {
            if (!ReferenceEquals(_sb, sb)) return;
            _sb = null;
            LabelHost.Width = double.NaN;
            outl.Visibility = Visibility.Collapsed;
            Glyph.Visibility = hasGlyph && !done ? Visibility.Visible : Visibility.Collapsed;
            Tick.Visibility = done ? Visibility.Visible : Visibility.Collapsed;
            Tick.StrokeDashOffset = done ? 0 : 8;
        };
        _sb = sb;
        sb.Begin();
    }
}
