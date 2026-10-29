using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using System.Collections.Specialized;
using Windows.System;

namespace DevWinUI;

[ContentProperty(Name = nameof(Items))]
[TemplatePart(Name = RowPartName, Type = typeof(FlexRowPanel))]
public sealed partial class ExpandingCards : Control
{
    private const string RowPartName = "PART_Row";

    private readonly List<ExpandingCardsCard> _cards = new();
    private FlexRowPanel? _row;
    private int _active;
    private int _committed;
    private bool _ticking;
    private double _last = double.NaN;

    public ExpandingCards()
    {
        DefaultStyleKey = typeof(ExpandingCards);
        Items = new ObservableCollection<ExpandingCardItem>();

        KeyDown += OnKeyDown;
        GotFocus += (_, _) => UpdateFocusRing();
        LostFocus += (_, _) => UpdateFocusRing();
        PointerExited += (_, _) => SetActive(_committed);
        Unloaded += (_, _) => StopTicking();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        StopTicking();
        _row = GetTemplateChild(RowPartName) as FlexRowPanel;
        Rebuild(resetActive: true);
    }

    private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (ExpandingCards)d;
        if (e.OldValue is ObservableCollection<ExpandingCardItem> oldItems)
        {
            oldItems.CollectionChanged -= self.OnCollectionChanged;
        }
        if (e.NewValue is ObservableCollection<ExpandingCardItem> newItems)
        {
            newItems.CollectionChanged += self.OnCollectionChanged;
        }
        self.Rebuild(resetActive: true);
    }

    private static void OnDefaultActiveIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ExpandingCards)d).Rebuild(resetActive: true);

    private static void OnAppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ExpandingCards)d).ApplyAppearance();

    private static void OnFlexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((ExpandingCards)d).SnapToTargets();

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        Rebuild(resetActive: false);

    private void Rebuild(bool resetActive)
    {
        if (_row is null)
        {
            return;
        }

        StopTicking();
        _row.Children.Clear();
        _cards.Clear();

        var items = Items;
        if (items is null || items.Count == 0)
        {
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            int index = i;

            var frame = new ExpandingCardFrame
            {
                Title = item.Title,
                ImageSource = CreateImage(item.Source),
            };
            ApplyAppearance(frame);
            AutomationProperties.SetName(frame, string.IsNullOrWhiteSpace(item.Alt) ? item.Title : $"{item.Title}. {item.Alt}");

            frame.PointerEntered += (_, e) =>
            {
                if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                {
                    SetActive(index);
                }
            };
            frame.Tapped += (_, _) =>
            {
                _committed = index;
                SetActive(index);
                Focus(FocusState.Pointer);
            };

            _row.Children.Add(frame);
            _cards.Add(new ExpandingCardsCard { Frame = frame });
        }

        int start = resetActive ? DefaultActiveIndex : _committed;
        _committed = _active = Math.Clamp(start, 0, _cards.Count - 1);
        SnapToTargets();
        UpdateFocusRing();
    }

    private void ApplyAppearance()
    {
        foreach (var card in _cards)
        {
            ApplyAppearance(card.Frame);
        }
    }

    private void ApplyAppearance(ExpandingCardFrame frame)
    {
        frame.CornerRadius = CardCornerRadius;
        SetOrClear(frame, ForegroundProperty, TitleForeground);
        SetOrClear(frame, ExpandingCardFrame.ScrimBrushProperty, ScrimBrush);
        SetOrClear(frame, ExpandingCardFrame.FocusBrushProperty, FocusBrush);
    }

    private static void SetOrClear(DependencyObject target, DependencyProperty property, object? value)
    {
        if (value is null)
        {
            target.ClearValue(property);
        }
        else
        {
            target.SetValue(property, value);
        }
    }

    private static ImageSource? CreateImage(string? source)
    {
        if (string.IsNullOrWhiteSpace(source) || !Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out var uri))
        {
            return null;
        }
        return new BitmapImage(uri);
    }

    private double Target(int i) => i == _active ? ExpandedFlex : CollapsedFlex;

    private void SnapToTargets()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].Current = Target(i);
        }
        ApplyAll();
    }

    private void SetActive(int index)
    {
        if (_cards.Count == 0)
        {
            return;
        }

        index = Math.Clamp(index, 0, _cards.Count - 1);
        if (index == _active)
        {
            UpdateFocusRing();
            return;
        }

        _active = index;
        UpdateFocusRing();

        if (ReduceMotion)
        {
            SnapToTargets();
        }
        else
        {
            StartTicking();
        }
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (_cards.Count == 0)
        {
            return;
        }

        bool rtl = FlowDirection == FlowDirection.RightToLeft;
        int next = _active;

        switch (e.Key)
        {
            case VirtualKey.Right:
            case VirtualKey.Down:
                next += rtl && e.Key == VirtualKey.Right ? -1 : 1;
                break;
            case VirtualKey.Left:
            case VirtualKey.Up:
                next += rtl && e.Key == VirtualKey.Left ? 1 : -1;
                break;
            case VirtualKey.Home:
                next = 0;
                break;
            case VirtualKey.End:
                next = _cards.Count - 1;
                break;
            default:
                return;
        }

        next = Math.Clamp(next, 0, _cards.Count - 1);
        _committed = next;
        SetActive(next);
        e.Handled = true;
    }

    private void UpdateFocusRing()
    {
        bool show = FocusState == FocusState.Keyboard;
        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].Frame.ShowFocusRing = show && i == _active;
        }
    }

    private void StartTicking()
    {
        if (_ticking)
        {
            return;
        }

        _ticking = true;
        _last = double.NaN;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopTicking()
    {
        if (!_ticking)
        {
            return;
        }

        _ticking = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        double now = e is RenderingEventArgs args
            ? args.RenderingTime.TotalSeconds
            : (double.IsNaN(_last) ? 0 : _last) + 1.0 / 60;
        double dt = double.IsNaN(_last) ? 1.0 / 60 : Math.Clamp(now - _last, 0.001, 0.05);
        _last = now;

        double a = 1 - Math.Exp(-12 * dt);
        bool moving = false;

        for (int i = 0; i < _cards.Count; i++)
        {
            var c = _cards[i];
            double t = Target(i);
            c.Current += (t - c.Current) * a;
            if (Math.Abs(t - c.Current) < 0.002)
            {
                c.Current = t;
            }
            else
            {
                moving = true;
            }
        }

        ApplyAll();
        if (!moving)
        {
            StopTicking();
        }
    }

    private void ApplyAll()
    {
        double range = ExpandedFlex - CollapsedFlex;
        foreach (var c in _cards)
        {
            FlexRowPanel.SetFlex(c.Frame, c.Current);
            c.Frame.Progress = range <= 0 ? 0 : Math.Clamp((c.Current - CollapsedFlex) / range, 0, 1);
        }
    }
}
