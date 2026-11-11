using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;

public sealed partial class Diagram
{
    private static double NodeHeight(DiagramNodeView v) =>
        v.Card != null && v.Card.ActualHeight > 0 ? v.Card.ActualHeight : v.LastHeight;

    private static double EstimateHeight(DiagramNode n, double width)
    {
        double h = 10 + 12 + 2 + 20;
        if (!string.IsNullOrEmpty(n.Caption))
        {
            double perLine = Math.Max(1, (width - 28) / 6.2);
            h += 6 + 16 * Math.Ceiling(n.Caption.Length / perLine);
        }

        h += n.Conditions.Count * (6 + 32);
        return h;
    }

    private void MeasureNode(DiagramNodeView v)
    {
        if (v.FixedHeight > 0)
        {
            v.LastHeight = v.FixedHeight;
            return;
        }

        var node = v.Node;
        if (node.CacheEpoch == _epoch && node.CachedHeight > 0)
        {
            v.LastHeight = node.CachedHeight;
            return;
        }

        if (v.Card != null) return;

        BuildCard(v);
        v.Card!.Measure(new Size(v.Width, double.PositiveInfinity));
        double h = v.Card.DesiredSize.Height;
        ResetCard(v);

        if (h > 0)
        {
            v.LastHeight = h;
            node.CachedHeight = h;
            node.CacheEpoch = _epoch;
        }
    }

    private void ResetCard(DiagramNodeView v)
    {
        v.Card = null;
        v.Face = null;
        v.Toggler = null;
        v.Muted = null;
        v.Pins = null;
        v.Hover = false;
    }

    private void BuildCard(DiagramNodeView v)
    {
        var node = v.Node;
        var dir = EffectiveDirection;
        v.Muted = new List<TextBlock>();

        FrameworkElement body;
        if (node.HasPins)
        {
            body = BuildPinBody(v);
        }
        else if (NodeTemplate != null || NodeTemplateSelector != null)
        {
            var holder = new ContentControl
            {
                Content = node,
                Padding = new Thickness(12, 10, 12, 12),
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Top,
            };
            holder.ContentTemplate = NodeTemplateSelector?.SelectTemplate(node, holder) ?? NodeTemplate;
            body = holder;
        }
        else
        {
            body = BuildDefaultBody(v);
        }

        var accent = new Border
        {
            Width = node.HasPins ? 0 : 4,
            Background = v.HueBrush,
            CornerRadius = new CornerRadius(10, 0, 0, 10),
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Grid.SetColumn(body, 1);
        grid.Children.Add(accent);
        grid.Children.Add(body);

        v.Face = new Border
        {
            CornerRadius = new CornerRadius(10),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(1),
            Child = grid,
        };

        FrameworkElement content = v.Face;
        if (Collapsible && v.Children.Count > 0 && !node.HasPins)
        {
            v.Toggler = BuildToggler(v, dir);
            v.Face.Margin = dir switch
            {
                DiagramDirection.BottomToTop => new Thickness(0, 12, 0, 0),
                DiagramDirection.LeftToRight => new Thickness(0, 0, 12, 0),
                DiagramDirection.RightToLeft => new Thickness(12, 0, 0, 0),
                _ => new Thickness(0, 0, 0, 12),
            };

            var host = new Grid();
            host.Children.Add(v.Face);
            host.Children.Add(v.Toggler);
            content = host;
        }

        v.Card = new ContentControl
        {
            Content = content,
            Width = v.Width,
            Padding = new Thickness(0),
            IsTabStop = true,
            UseSystemFocusVisuals = true,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Top,
        };

        if (v.FixedHeight > 0) v.Card.Height = v.FixedHeight;

        var name = node.DisplayText;
        if (!string.IsNullOrEmpty(name)) AutomationProperties.SetName(v.Card, name);
        if (!string.IsNullOrEmpty(node.Caption)) AutomationProperties.SetHelpText(v.Card, node.Caption);

        WireNode(v);
        UpdateFace(v);
    }

    private FrameworkElement BuildDefaultBody(DiagramNodeView v)
    {
        var node = v.Node;
        var body = new StackPanel { Spacing = 6, Padding = new Thickness(12, 10, 12, 12) };

        body.Children.Add(new TextBlock
        {
            Text = node.DisplayText,
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            TextTrimming = TextTrimming.CharacterEllipsis,
        });

        if (!string.IsNullOrEmpty(node.Caption))
        {
            var caption = new TextBlock
            {
                Text = node.Caption,
                FontSize = 12,
                Foreground = NodeCaptionForeground,
                TextWrapping = TextWrapping.Wrap,
            };
            v.Muted!.Add(caption);
            body.Children.Add(caption);
        }

        foreach (var cond in node.Conditions)
            body.Children.Add(BuildConditionRow(v, cond));

        return body;
    }

    private Button BuildToggler(DiagramNodeView v, DiagramDirection dir)
    {
        var button = new Button
        {
            Width = 24,
            Height = 24,
            MinWidth = 0,
            MinHeight = 0,
            Padding = new Thickness(0),
            CornerRadius = new CornerRadius(12),
            BorderThickness = new Thickness(1),
            Background = TogglerBackground,
            BorderBrush = TogglerBorderBrush,
            Foreground = TogglerForeground,
            UseSystemFocusVisuals = true,
            HorizontalAlignment = dir switch
            {
                DiagramDirection.LeftToRight => HorizontalAlignment.Right,
                DiagramDirection.RightToLeft => HorizontalAlignment.Left,
                _ => HorizontalAlignment.Center,
            },
            VerticalAlignment = dir switch
            {
                DiagramDirection.TopToBottom => VerticalAlignment.Bottom,
                DiagramDirection.BottomToTop => VerticalAlignment.Top,
                _ => VerticalAlignment.Center,
            },
        };

        button.Click += (_, _) => ToggleExpanded(v.Node);
        v.Toggler = button;
        UpdateTogglerContent(v, dir);
        return button;
    }

    private void UpdateTogglerContent(DiagramNodeView v, DiagramDirection dir)
    {
        if (v.Toggler == null) return;

        bool expanded = v.Node.Expanded;
        string glyph = dir switch
        {
            DiagramDirection.BottomToTop => "\uE70D",
            DiagramDirection.LeftToRight => "\uE76B",
            DiagramDirection.RightToLeft => "\uE76C",
            _ => "\uE70E",
        };

        v.Toggler.Content = expanded
            ? new FontIcon { Glyph = glyph, FontSize = 10 }
            : new TextBlock { Text = v.Children.Count.ToString(), FontSize = 11 };
        AutomationProperties.SetName(v.Toggler, expanded ? "Collapse" : "Expand");
    }

    private UIElement BuildConditionRow(DiagramNodeView v, DiagramCondition cond)
    {
        var row = new Grid { ColumnSpacing = 8 };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(36) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var label = new TextBlock
        {
            Text = cond.Label ?? "",
            FontSize = 12,
            Foreground = NodeCaptionForeground,
            VerticalAlignment = VerticalAlignment.Center,
        };
        v.Muted!.Add(label);

        var combo = new ComboBox
        {
            ItemsSource = cond.Options,
            SelectedItem = cond.Value,
            PlaceholderText = cond.Placeholder ?? "Select…",
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        AutomationProperties.SetName(combo, $"{cond.Label} condition");

        combo.SelectionChanged += (_, _) =>
        {
            cond.Value = combo.SelectedItem as string;
            ConditionChanged?.Invoke(this, new DiagramConditionEventArgs(v.Node, cond));
        };
        combo.DropDownOpened += (_, _) => v.ComboOpen = true;
        combo.DropDownClosed += (_, _) => v.ComboOpen = false;

        Grid.SetColumn(combo, 1);
        row.Children.Add(label);
        row.Children.Add(combo);
        return row;
    }

    private void WireNode(DiagramNodeView v)
    {
        var card = v.Card!;

        card.SizeChanged += (_, _) =>
        {
            double h = card.ActualHeight;
            if (h > 0 && Math.Abs(h - v.LastHeight) > 0.5)
            {
                v.LastHeight = h;
                v.Node.CachedHeight = h;
                v.Node.CacheEpoch = _epoch;
                _boundsDirty = true;
                UpdateMiniRect(v);
            }

            UpdateEdges();
            UpdateMinimap();
        };

        card.PointerEntered += (_, _) => { v.Hover = true; UpdateHot(v); };
        card.PointerExited += (_, _) => { v.Hover = false; UpdateHot(v); };
        card.GotFocus += (_, _) =>
        {
            v.Focus = true;
            UpdateHot(v);
            if (card.FocusState == FocusState.Keyboard) EnsureVisible(v);
        };
        card.LostFocus += (_, _) => { v.Focus = false; UpdateHot(v); };

        card.PointerPressed += (_, e) =>
        {
            if (_nodes == null || v.ComboOpen) return;
            if (IsInsideInteractive(e.OriginalSource as DependencyObject, card)) return;
            if (!e.GetCurrentPoint(card).Properties.IsLeftButtonPressed) return;

            var p = e.GetCurrentPoint(_nodes).Position;
            _pressed = v;
            _dragMoved = false;
            _pressScreen = e.GetCurrentPoint(this).Position;
            _grab = new Point(p.X - v.Node.Left, p.Y - v.Node.Top);
            v.Drag = AllowDrag;
            card.CapturePointer(e.Pointer);
            v.Z = ++_z;
            Canvas.SetZIndex(card, v.Z);
            card.Focus(FocusState.Pointer);
            UpdateHot(v);
            e.Handled = true;
        };

        card.PointerMoved += (_, e) =>
        {
            if (_pressed != v || _nodes == null) return;

            var screen = e.GetCurrentPoint(this).Position;
            if (!_dragMoved && (Math.Abs(screen.X - _pressScreen.X) > 4 || Math.Abs(screen.Y - _pressScreen.Y) > 4))
                _dragMoved = true;

            if (AllowDrag && _dragMoved)
            {
                var p = e.GetCurrentPoint(_nodes).Position;
                MoveTo(v, p.X - _grab.X, p.Y - _grab.Y, true);
            }

            e.Handled = true;
        };

        card.PointerReleased += (_, e) => EndPress(v, e.Pointer, true);
        card.PointerCaptureLost += (_, _) => EndPress(v, null, false);
        card.KeyDown += (_, e) => OnCardKeyDown(v, e);
    }

    private void EndPress(DiagramNodeView v, Pointer? pointer, bool released)
    {
        if (_pressed != v) return;

        _pressed = null;
        v.Drag = false;
        if (pointer != null) v.Card?.ReleasePointerCapture(pointer);
        UpdateHot(v);

        if (_dragMoved)
        {
            if (AllowDrag) NodeMoved?.Invoke(this, new DiagramNodeEventArgs(v.Node));
        }
        else if (released && SelectionMode != DiagramSelectionMode.None && v.Node.Selectable)
        {
            ToggleSelection(v.Node);
        }

        _dragMoved = false;
    }

    private static bool IsDown(VirtualKey key) =>
        Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(key)
            .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

    private void OnCardKeyDown(DiagramNodeView v, KeyRoutedEventArgs e)
    {
        if (IsInsideInteractive(e.OriginalSource as DependencyObject, v.Card!)) return;

        switch (e.Key)
        {
            case VirtualKey.Enter:
            case VirtualKey.Space:
                if (SelectionMode != DiagramSelectionMode.None && v.Node.Selectable)
                {
                    e.Handled = true;
                    ToggleSelection(v.Node);
                }

                return;

            case VirtualKey.Add:
            case (VirtualKey)187:
                if (Collapsible && v.Children.Count > 0 && !v.Node.Expanded)
                {
                    e.Handled = true;
                    ToggleExpanded(v.Node);
                }

                return;

            case VirtualKey.Subtract:
            case (VirtualKey)189:
                if (Collapsible && v.Children.Count > 0 && v.Node.Expanded)
                {
                    e.Handled = true;
                    ToggleExpanded(v.Node);
                }

                return;

            case VirtualKey.PageDown:
            case VirtualKey.PageUp:
                int i = _all.IndexOf(v) + (e.Key == VirtualKey.PageDown ? 1 : -1);
                if (i >= 0 && i < _all.Count)
                {
                    e.Handled = true;
                    FocusNode(_all[i].Node);
                }

                return;
        }

        double ux = 0, uy = 0;
        switch (e.Key)
        {
            case VirtualKey.Left: ux = -1; break;
            case VirtualKey.Right: ux = 1; break;
            case VirtualKey.Up: uy = -1; break;
            case VirtualKey.Down: uy = 1; break;
            default: return;
        }

        e.Handled = true;

        if (AllowDrag && !IsDown(VirtualKey.Control))
        {
            double step = IsDown(VirtualKey.Shift) ? 32 : 8;
            MoveTo(v, v.Node.Left + ux * step, v.Node.Top + uy * step, false);
            EnsureVisible(v);
            NodeMoved?.Invoke(this, new DiagramNodeEventArgs(v.Node));
        }
        else
        {
            FocusNeighbor(v, ux, uy);
        }
    }

    private void FocusNeighbor(DiagramNodeView v, double ux, double uy)
    {
        double cx = v.Node.Left + v.Width / 2, cy = v.Node.Top + NodeHeight(v) / 2;
        DiagramNodeView? best = null;
        double bestScore = double.MaxValue;

        foreach (var o in _all)
        {
            if (o == v) continue;

            double ox = o.Node.Left + o.Width / 2 - cx, oy = o.Node.Top + NodeHeight(o) / 2 - cy;
            double along = ox * ux + oy * uy;
            if (along <= 1) continue;

            double score = along + 2 * Math.Abs(ox * uy - oy * ux);
            if (score < bestScore)
            {
                bestScore = score;
                best = o;
            }
        }

        if (best != null) FocusNode(best.Node);
    }

    private static bool IsInsideInteractive(DependencyObject? d, DependencyObject stop)
    {
        while (d != null && d != stop)
        {
            if (d is ComboBox or Button or FrameworkElement { Tag: DiagramPin }) return true;
            d = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(d);
        }

        return false;
    }

    private void UpdateHot(DiagramNodeView v)
    {
        UpdateFace(v);
        ApplyEdgeBrushes();
    }

    private void UpdateFace(DiagramNodeView v)
    {
        if (v.Face == null || v.Card == null) return;

        bool sel = v.Selected;
        v.Face.BorderThickness = new Thickness(sel ? 2 : 1);
        v.Face.Padding = new Thickness(sel ? 0 : 1);
        v.Face.Background = sel ? SelectedNodeBackground ?? NodeBackground : NodeBackground;
        v.Face.BorderBrush = sel ? SelectedNodeBorderBrush ?? v.HueBrush : v.Hot ? v.HueBrush : NodeBorderBrush;
        v.Card.Opacity = v.Match ? 1 : 0.35;
    }

    private void ApplyNodeBrushes()
    {
        foreach (var v in _all)
        {
            if (v.Card == null) continue;

            UpdateFace(v);

            if (v.Muted != null)
                foreach (var t in v.Muted) t.Foreground = NodeCaptionForeground;

            if (v.Toggler != null)
            {
                v.Toggler.Background = TogglerBackground;
                v.Toggler.BorderBrush = TogglerBorderBrush;
                v.Toggler.Foreground = TogglerForeground;
            }
        }

        ApplyLabelBrushes();
    }

    private void MoveTo(DiagramNodeView v, double left, double top, bool snap)
    {
        if (snap && SnapToGrid && SnapSize > 0)
        {
            left = Math.Round(left / SnapSize) * SnapSize;
            top = Math.Round(top / SnapSize) * SnapSize;
        }

        var n = v.Node;
        n.Left = left;
        n.Top = top;
        n.OffsetX = left - v.BaseLeft;
        n.OffsetY = top - v.BaseTop;

        if (Layout == DiagramLayout.Free)
        {
            n.SetCanvas(left, top);
            n.OffsetX = 0;
            n.OffsetY = 0;
            v.BaseLeft = left;
            v.BaseTop = top;
        }

        if (v.Card != null)
        {
            Canvas.SetLeft(v.Card, left);
            Canvas.SetTop(v.Card, top);
        }

        _boundsDirty = true;
        UpdateMiniRect(v);
        Cull();
        UpdateEdges();
        UpdateMinimap();
    }
}
