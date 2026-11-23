namespace DevWinUI;

public sealed partial class GrowlHost
{
    private const double VisibleEpsilon = 0.5;
    private const double MinCardWidth = 240;
    private const double MinSidePadding = 8;

    private double? _widthLimit;
    private double _visibleLeft;
    private double _visibleWidth = double.PositiveInfinity;
    private double _cutLeft;
    private double _cutRight;
    private bool _visibleHooked;
    private Grid? _templateRoot;

    internal double? WidthLimit
    {
        get => _widthLimit;
        set
        {
            if (_widthLimit == value)
            {
                return;
            }

            _widthLimit = value;
            UpdateVisibleBounds();
            RefreshWidthLimits();
        }
    }

    private double GetMaxCardWidth()
    {
        if (_widthLimit is { } limit)
        {
            return Math.Max(0, limit);
        }

        double width = double.IsInfinity(_visibleWidth) ? ActualWidth : _visibleWidth;
        double available = width - (Padding.Left - _cutLeft) - (Padding.Right - _cutRight);

        return available > 0 ? available : double.PositiveInfinity;
    }

    private void ApplyMaxWidth(GrowlCard card)
    {
        double max = GetMaxCardWidth();

        if (card.MaxWidth != max)
        {
            card.MaxWidth = max;
        }
    }

    private void EnsureVisibleHook()
    {
        if (_visibleHooked)
        {
            return;
        }

        _visibleHooked = true;
        LayoutUpdated += (_, _) => UpdateVisibleBounds();
    }

    private void UpdateVisibleBounds()
    {
        _templateRoot ??= VisualTreeHelper.GetChildrenCount(this) > 0 ? VisualTreeHelper.GetChild(this, 0) as Grid : null;

        if (_templateRoot is null)
        {
            return;
        }

        if (_widthLimit is not null || !IsLoaded || ActualWidth <= 0)
        {
            ResetVisibleBounds();
            return;
        }

        double left = 0;
        double right = ActualWidth;
        var parent = VisualTreeHelper.GetParent(this);

        while (parent is not null)
        {
            if (parent is FrameworkElement element && element.ActualWidth > 0 && element.Visibility == Visibility.Visible)
            {
                try
                {
                    double x = element.TransformToVisual(this).TransformPoint(new Point(0, 0)).X;

                    left = Math.Max(left, x);
                    right = Math.Min(right, x + element.ActualWidth);
                }
                catch
                {
                }
            }

            parent = VisualTreeHelper.GetParent(parent);
        }

        double width = right - left;

        if (width <= 0)
        {
            ResetVisibleBounds();
            return;
        }

        double padding = Padding.Left + Padding.Right;
        double cutLeft = 0;
        double cutRight = 0;

        if (padding > 0)
        {
            double allowed = Math.Min(padding, Math.Max(2 * MinSidePadding, width - MinCardWidth));
            double cut = padding - allowed;

            cutLeft = cut * Padding.Left / padding;
            cutRight = cut * Padding.Right / padding;
        }

        bool unchanged = left <= VisibleEpsilon && width >= ActualWidth - VisibleEpsilon && cutLeft <= VisibleEpsilon && cutRight <= VisibleEpsilon;

        if (unchanged)
        {
            ResetVisibleBounds();
            return;
        }

        if (Math.Abs(width - _visibleWidth) < VisibleEpsilon
            && Math.Abs(left - _visibleLeft) < VisibleEpsilon
            && Math.Abs(cutLeft - _cutLeft) < VisibleEpsilon
            && Math.Abs(cutRight - _cutRight) < VisibleEpsilon)
        {
            return;
        }

        _visibleLeft = left;
        _visibleWidth = width;
        _cutLeft = cutLeft;
        _cutRight = cutRight;

        _templateRoot.HorizontalAlignment = HorizontalAlignment.Left;
        _templateRoot.Margin = new Thickness(left - cutLeft, 0, 0, 0);
        _templateRoot.Width = width + cutLeft + cutRight;

        RefreshWidthLimits();
    }

    private void ResetVisibleBounds()
    {
        if (double.IsInfinity(_visibleWidth))
        {
            return;
        }

        _visibleLeft = 0;
        _visibleWidth = double.PositiveInfinity;
        _cutLeft = 0;
        _cutRight = 0;

        if (_templateRoot is not null)
        {
            _templateRoot.ClearValue(FrameworkElement.WidthProperty);
            _templateRoot.ClearValue(FrameworkElement.MarginProperty);
            _templateRoot.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
        }

        RefreshWidthLimits();
    }

    private void RefreshWidthLimits()
    {
        foreach (var position in Enum.GetValues<GrowlPosition>())
        {
            ApplyLayout(position);
        }
    }

    private void HookPanel(Panel panel, GrowlPosition position)
    {
        _templateRoot = null;
        EnsureVisibleHook();

        panel.Background ??= new SolidColorBrush(Colors.Transparent);

        panel.PointerEntered += (_, _) => SetExpanded(position, true);
        panel.PointerExited += (_, e) =>
        {
            var point = e.GetCurrentPoint(panel).Position;

            if (point.X <= 0 || point.Y <= 0 || point.X >= panel.ActualWidth || point.Y >= panel.ActualHeight)
            {
                SetExpanded(position, false);
            }
        };
    }

    private void SetExpanded(GrowlPosition position, bool expanded)
    {
        if (LayoutMode != GrowlHostLayoutMode.Stacked || (expanded && !StackExpandOnHover))
        {
            return;
        }

        bool changed = expanded ? _expanded.Add(position) : _expanded.Remove(position);

        if (changed)
        {
            ApplyLayout(position);
        }
    }

    private static bool IsTop(GrowlPosition position)
    {
        return position is GrowlPosition.TopLeft or GrowlPosition.TopCenter or GrowlPosition.TopRight;
    }

    private static HorizontalAlignment GetAlignment(GrowlPosition position)
    {
        return position switch
        {
            GrowlPosition.TopLeft or GrowlPosition.BottomLeft => HorizontalAlignment.Left,
            GrowlPosition.TopCenter or GrowlPosition.BottomCenter => HorizontalAlignment.Center,
            _ => HorizontalAlignment.Right
        };
    }

    private bool InsertAtFront(GrowlPosition position)
    {
        return LayoutMode == GrowlHostLayoutMode.Stacked ? IsTop(position) : NewestOnTop;
    }

    private void Pump(GrowlPosition position)
    {
        if (!_panels.TryGetValue(position, out var panel))
        {
            return;
        }

        bool frontFirst = InsertAtFront(position);

        foreach (var handle in _handles.Where(h => h.Position == position && !h.IsShown && !h.IsClosed).ToList())
        {
            if (MaxVisiblePerPosition > 0 && panel.Children.Count >= MaxVisiblePerPosition)
            {
                break;
            }

            if (frontFirst)
            {
                panel.Children.Insert(0, handle.Card);
            }
            else
            {
                panel.Children.Add(handle.Card);
            }

            handle.IsShown = true;
            PlayShow(handle);
            GrowlSoundPlayer.Play(handle.Sound, handle.Card.Severity, handle.SoundUri);
        }

        ApplyLayout(position);
    }

    private void PlayShow(GrowlHandle handle)
    {
        var card = handle.Card;

        if (handle.ShowAnimation == GrowlShowAnimation.Snap)
        {
            ElementCompositionPreview.GetElementVisual(card).Opacity = 0;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            card.Loaded -= OnLoaded;
            GrowlAnimator.Show(card, handle.ShowAnimation, handle.Position, AnimationDuration);
        }

        card.Loaded += OnLoaded;
    }

    private void Reorder(GrowlPosition position)
    {
        if (!_panels.TryGetValue(position, out var panel))
        {
            return;
        }

        var cards = _handles
            .Where(h => h.Position == position && h.IsShown)
            .Select(h => h.Card)
            .ToList();

        if (InsertAtFront(position))
        {
            cards.Reverse();
        }

        panel.Children.Clear();

        foreach (var card in cards)
        {
            panel.Children.Add(card);
        }
    }

    private void ApplyLayout(GrowlPosition position)
    {
        if (!_panels.TryGetValue(position, out var panel))
        {
            return;
        }

        int count = panel.Children.Count;

        if (count == 0)
        {
            _expanded.Remove(position);
        }

        bool stacked = LayoutMode == GrowlHostLayoutMode.Stacked;
        bool collapsed = stacked && !_expanded.Contains(position);
        bool top = IsTop(position);
        double peek = Math.Max(0, StackPeekHeight);

        if (panel is StackPanel stackPanel)
        {
            stackPanel.Spacing = collapsed ? 0 : Spacing;
        }

        for (int i = 0; i < count; i++)
        {
            if (panel.Children[i] is not GrowlCard card)
            {
                continue;
            }

            ApplyMaxWidth(card);

            int depth = stacked ? (top ? i : count - 1 - i) : 0;

            card.IsCountdownSuspended = stacked && (collapsed ? depth > 0 : card.PauseAutoCloseOnPointerOver);

            if (card.RenderTransform is not ScaleTransform scale)
            {
                scale = new ScaleTransform();
                card.RenderTransform = scale;
            }

            card.RenderTransformOrigin = new Point(0.5, top ? 1 : 0);
            Canvas.SetZIndex(card, stacked ? count - depth : 0);

            if (!collapsed || depth == 0)
            {
                scale.ScaleX = 1;
                scale.ScaleY = 1;
                card.Margin = new Thickness(0);
                card.Opacity = 1;
                card.IsHitTestVisible = true;
                continue;
            }

            double factor = Math.Max(0.8, 1 - 0.05 * depth);
            double hidden = Math.Max(0, card.ActualHeight - (depth < StackVisibleDepth ? peek : 0));

            scale.ScaleX = factor;
            scale.ScaleY = factor;
            card.Margin = top ? new Thickness(0, -hidden, 0, 0) : new Thickness(0, 0, 0, -hidden);
            card.Opacity = depth < StackVisibleDepth ? 1 : 0;
            card.IsHitTestVisible = depth < StackVisibleDepth;
        }

        LayoutChanged?.Invoke(this);
    }

    private void RemoveWithShift(Panel panel, GrowlCard card)
    {
        if (LayoutMode != GrowlHostLayoutMode.List)
        {
            panel.Children.Remove(card);
            return;
        }

        var before = panel.Children
            .OfType<GrowlCard>()
            .Where(c => c != card)
            .ToDictionary(c => c, c => OffsetIn(this, c));

        panel.Children.Remove(card);
        UpdateLayout();

        foreach (var (other, old) in before)
        {
            var delta = old - OffsetIn(this, other);

            if (delta.LengthSquared() > 0.25f)
            {
                GrowlAnimator.Shift(other, delta, AnimationDuration);
            }
        }
    }

    private static Vector2 OffsetIn(UIElement root, UIElement element)
    {
        var point = element.TransformToVisual(root).TransformPoint(new Point(0, 0));
        return new Vector2((float)point.X, (float)point.Y);
    }

}
