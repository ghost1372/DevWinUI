namespace DevWinUI;

public sealed partial class GrowlHost
{
    private void HookPanel(Panel panel, GrowlPosition position)
    {
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
