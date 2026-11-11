using Microsoft.UI.Xaml.Documents;

namespace DevWinUI;

public sealed partial class Diagram
{
    private const double HeaderH = 44, RowH = 36, PinPadY = 6, PinCenterInset = 17;
    private const double PinTop = 2 + HeaderH + PinPadY;

    private static double PinNodeHeight(DiagramNode n) =>
        4 + HeaderH + 2 * PinPadY + Math.Max(n.Inputs.Count, n.Outputs.Count) * RowH;

    private static Point PinCenter(DiagramNodeView v, DiagramPin pin)
    {
        var list = pin.IsOutput ? v.Node.Outputs : v.Node.Inputs;
        int i = Math.Max(0, list.IndexOf(pin));
        double y = v.Node.Top + PinTop + i * RowH + RowH / 2;
        double x = pin.IsOutput ? v.Node.Left + v.Width - PinCenterInset : v.Node.Left + PinCenterInset;
        return new Point(x, y);
    }

    private static Point PinEdgeAnchor(DiagramNodeView v, DiagramPin pin)
    {
        var c = PinCenter(v, pin);
        return new Point(pin.IsOutput ? v.Node.Left + v.Width : v.Node.Left, c.Y);
    }

    private SolidColorBrush PinBrush(DiagramNodeView v, DiagramPin pin)
    {
        if (pin.IsExec) return TypeBrush("exec");
        var t = ResolvedType(v, pin);
        return TypeBrush(t.IsVar ? string.Empty : t.Name);
    }

    private static string? Shorten(string? id) => string.IsNullOrEmpty(id) ? null : id.Length > 8 ? id[..8] : id;

    private FrameworkElement BuildPinBody(DiagramNodeView v)
    {
        var node = v.Node;
        v.Pins = new();

        var root = new StackPanel();
        root.Children.Add(BuildHeader(v));

        int rows = Math.Max(node.Inputs.Count, node.Outputs.Count);
        var grid = new Grid { Margin = new Thickness(0, PinPadY, 0, PinPadY) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        for (int i = 0; i < rows; i++) grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(RowH) });

        for (int i = 0; i < node.Inputs.Count; i++)
        {
            var cell = BuildPinCell(v, node.Inputs[i], false);
            Grid.SetRow(cell, i);
            grid.Children.Add(cell);
        }

        for (int i = 0; i < node.Outputs.Count; i++)
        {
            var cell = BuildPinCell(v, node.Outputs[i], true);
            Grid.SetRow(cell, i);
            Grid.SetColumn(cell, 1);
            grid.Children.Add(cell);
        }

        root.Children.Add(grid);
        return root;
    }

    private FrameworkElement BuildHeader(DiagramNodeView v)
    {
        var node = v.Node;
        var c = node.Hue;
        var fill = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
        fill.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0x50, c.R, c.G, c.B), Offset = 0 });
        fill.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0x0C, c.R, c.G, c.B), Offset = 1 });

        var grid = new Grid { Padding = new Thickness(12, 0, 12, 0), ColumnSpacing = 8 };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        if (!string.IsNullOrEmpty(node.Glyph))
        {
            grid.Children.Add(new FontIcon
            {
                Glyph = node.Glyph,
                FontSize = 14,
                Foreground = v.HueBrush,
                VerticalAlignment = VerticalAlignment.Center,
            });
        }

        var text = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Spacing = 1 };
        text.Children.Add(new TextBlock
        {
            Text = node.DisplayText,
            FontSize = 13,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            TextTrimming = TextTrimming.CharacterEllipsis,
        });

        var sub = node.Subtitle ?? (ShowNodeIds ? Shorten(node.Id) : null);
        if (!string.IsNullOrEmpty(sub))
        {
            var s = new TextBlock
            {
                Text = sub,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                Foreground = NodeCaptionForeground,
            };
            v.Muted!.Add(s);
            text.Children.Add(s);
        }

        Grid.SetColumn(text, 1);
        grid.Children.Add(text);

        return new Border
        {
            Height = HeaderH,
            Background = fill,
            BorderBrush = NodeBorderBrush,
            BorderThickness = new Thickness(0, 0, 0, 1),
            CornerRadius = new CornerRadius(9, 9, 0, 0),
            Child = grid,
        };
    }

    private FrameworkElement BuildPinCell(DiagramNodeView v, DiagramPin pin, bool output)
    {
        var pv = BuildHandle(v, pin);
        v.Pins![pin] = pv;

        var align = output ? TextAlignment.Right : TextAlignment.Left;
        var text = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        text.Children.Add(new TextBlock
        {
            Text = pin.DisplayLabel,
            FontSize = 12,
            TextAlignment = align,
            TextTrimming = TextTrimming.CharacterEllipsis,
        });

        if (!pin.IsExec && ShowPinTypes)
        {
            var tb = BuildTypeText(ResolvedType(v, pin));
            tb.TextAlignment = align;
            text.Children.Add(tb);
        }

        var cell = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 4,
            Margin = new Thickness(6, 0, 6, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = output ? HorizontalAlignment.Right : HorizontalAlignment.Left,
        };

        if (output)
        {
            cell.Children.Add(text);
            cell.Children.Add(pv.Handle);
        }
        else
        {
            cell.Children.Add(pv.Handle);
            cell.Children.Add(text);
        }

        return cell;
    }

    private TextBlock BuildTypeText(DiagramTypeRef t)
    {
        var tb = new TextBlock
        {
            FontFamily = new FontFamily("Consolas"),
            FontSize = 10,
            TextWrapping = TextWrapping.NoWrap,
            IsHitTestVisible = false,
        };
        AppendType(tb.Inlines, t);
        return tb;
    }

    private void AppendType(InlineCollection inlines, DiagramTypeRef t)
    {
        inlines.Add(new Run
        {
            Text = t.IsVar ? t.ToString() : t.Name,
            Foreground = t.IsVar ? NodeCaptionForeground : TypeBrush(t.Name),
        });

        if (t.Args.Count == 0) return;

        inlines.Add(new Run { Text = "[", Foreground = NodeCaptionForeground });
        for (int i = 0; i < t.Args.Count; i++)
        {
            if (i > 0) inlines.Add(new Run { Text = ", ", Foreground = NodeCaptionForeground });
            AppendType(inlines, t.Args[i]);
        }

        inlines.Add(new Run { Text = "]", Foreground = NodeCaptionForeground });
    }

    private DiagramPinVisual BuildHandle(DiagramNodeView v, DiagramPin pin)
    {
        var t = ResolvedType(v, pin);
        string name = pin.IsExec ? "exec" : t.IsVar ? string.Empty : t.Name;
        var def = LookupType(name);
        var color = TypeColor(name);
        var brush = TypeBrush(name);
        var shape = def?.Shape ?? PinShape;
        var glyph = def?.Glyph ?? (pin.IsExec ? "\uE768" : null);

        FrameworkElement inner;
        if (glyph != null)
        {
            inner = new FontIcon
            {
                Glyph = glyph,
                FontSize = 12,
                Foreground = brush,
                Opacity = pin.Connected || pin.IsExec ? 1 : 0.75,
            };
        }
        else
        {
            var dot = new Border
            {
                Width = 12,
                Height = 12,
                BorderThickness = new Thickness(2),
                BorderBrush = brush,
                Background = pin.Connected ? brush : new SolidColorBrush(Color.FromArgb(0x30, color.R, color.G, color.B)),
                CornerRadius = new CornerRadius(shape == DiagramPinShape.Circle ? 6 : shape == DiagramPinShape.Diamond ? 2 : 3),
            };

            if (shape == DiagramPinShape.Diamond)
            {
                dot.Width = dot.Height = 10;
                dot.RenderTransformOrigin = new Point(0.5, 0.5);
                dot.RenderTransform = new RotateTransform { Angle = 45 };
            }

            inner = dot;
        }

        inner.HorizontalAlignment = HorizontalAlignment.Center;
        inner.VerticalAlignment = VerticalAlignment.Center;

        var scale = new ScaleTransform();
        var handle = new Border
        {
            Width = 18,
            Height = 18,
            Tag = pin,
            Background = new SolidColorBrush(Colors.Transparent),
            Child = inner,
            VerticalAlignment = VerticalAlignment.Center,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = scale,
        };

        if (!pin.IsExec) ToolTipService.SetToolTip(handle, $"{pin.DisplayLabel}: {t}");

        handle.PointerPressed += (_, e) => Pin_PointerPressed(v, pin, handle, e);
        handle.PointerMoved += (_, e) => Pin_PointerMoved(e);
        handle.PointerReleased += (_, e) => Pin_PointerReleased(e);
        handle.PointerCaptureLost += (_, _) => EndLink(null, false);

        return new DiagramPinVisual { Pin = pin, Handle = handle, Scale = scale };
    }
}
