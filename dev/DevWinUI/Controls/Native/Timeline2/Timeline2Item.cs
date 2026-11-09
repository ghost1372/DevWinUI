using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;

public sealed partial class Timeline2Item : ContentControl
{
    private const double ConnectorThickness = 2;
    private const double ConnectorInset = 6;

    private static readonly GridLength StarLength = new(1, GridUnitType.Star);
    private static readonly GridLength AutoLength = GridLength.Auto;
    private static readonly GridLength ZeroLength = new(0);

    [ThreadStatic]
    private static InputCursor? s_handCursor;

    private Grid _root = null!;
    private Grid _separator = null!;
    private Border _marker = null!;
    private FontIcon _icon = null!;
    private ContentPresenter _markerPresenter = null!;
    private Border _connector = null!;
    private ContentPresenter _opposite = null!;
    private StackPanel _body = null!;
    private ContentPresenter _date = null!;
    private ContentPresenter _title = null!;
    private ContentPresenter _description = null!;
    private ContentPresenter _content = null!;
    private ColumnDefinition _col0 = null!;
    private ColumnDefinition _col1 = null!;
    private ColumnDefinition _col2 = null!;
    private ColumnDefinition _sepCol1 = null!;
    private RowDefinition _row0 = null!;
    private RowDefinition _row1 = null!;
    private RowDefinition _row2 = null!;
    private RowDefinition _sepRow1 = null!;
    private FrameworkElement? _leading;
    private FrameworkElement? _trailing;
    private ImageBrush? _avatarBrush;
    private bool _ready;
    private bool _isCentered;
    private bool _clickable;

    public Timeline2Item()
    {
        DefaultStyleKey = typeof(Timeline2Item);
    }

    internal Timeline2? Owner { get; set; }

    internal int Index { get; set; }

    internal bool IsLast { get; set; }

    internal bool IsCentered => _isCentered;

    internal bool IsClickable => _clickable;

    internal bool IsOwnItem { get; set; }

    internal string? EntryTitle => (Content as Timeline2Entry)?.Title;

    internal double LeadingExtent => _leading?.DesiredSize.Height ?? 0;

    internal double TrailingExtent => _trailing?.DesiredSize.Height ?? 0;

    internal double BarExtent => _ready ? _separator.DesiredSize.Height : 0;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild("PART_Root") is not Grid root
            || GetTemplateChild("PART_Separator") is not Grid separator
            || GetTemplateChild("PART_Marker") is not Border marker
            || GetTemplateChild("PART_Icon") is not FontIcon icon
            || GetTemplateChild("PART_MarkerPresenter") is not ContentPresenter markerPresenter
            || GetTemplateChild("PART_Connector") is not Border connector
            || GetTemplateChild("PART_Opposite") is not ContentPresenter opposite
            || GetTemplateChild("PART_Body") is not StackPanel body
            || GetTemplateChild("PART_Date") is not ContentPresenter date
            || GetTemplateChild("PART_Title") is not ContentPresenter title
            || GetTemplateChild("PART_Description") is not ContentPresenter description
            || GetTemplateChild("PART_Content") is not ContentPresenter content)
        {
            _ready = false;
            return;
        }

        _root = root;
        _separator = separator;
        _marker = marker;
        _icon = icon;
        _markerPresenter = markerPresenter;
        _connector = connector;
        _opposite = opposite;
        _body = body;
        _date = date;
        _title = title;
        _description = description;
        _content = content;

        _col0 = root.ColumnDefinitions[0];
        _col1 = root.ColumnDefinitions[1];
        _col2 = root.ColumnDefinitions[2];
        _row0 = root.RowDefinitions[0];
        _row1 = root.RowDefinitions[1];
        _row2 = root.RowDefinitions[2];
        _sepCol1 = separator.ColumnDefinitions[1];
        _sepRow1 = separator.RowDefinitions[1];

        _ready = true;
        Update();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        Update();
    }

    protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
    {
        base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);
        Update();
    }

    protected override void OnTapped(TappedRoutedEventArgs e)
    {
        base.OnTapped(e);

        Invoke();
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);

        if (_clickable && (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Space))
        {
            Invoke();
            e.Handled = true;
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new Timeline2ItemAutomationPeer(this);

    internal void Invoke()
    {
        if (_clickable)
        {
            Owner?.NotifyItemClick(this);
        }
    }

    internal void Update()
    {
        if (!_ready || Owner is not { } owner)
        {
            return;
        }

        AutomationProperties.SetPositionInSet(this, Index + 1);
        AutomationProperties.SetSizeOfSet(this, owner.Items.Count);

        var entry = Content as Timeline2Entry;
        bool customTemplate = IsOwnItem
            ? ContentTemplate is not null || ContentTemplateSelector is not null
            : owner.ItemTemplate is not null || owner.ItemTemplateSelector is not null;
        bool entryMode = entry is not null && !customTemplate;

        var state = owner.GetState(Index);
        bool filled = state != Timeline2ItemState.Pending;
        bool trackColored = state == Timeline2ItemState.Completed
            || (owner.Reverse && state == Timeline2ItemState.Active);

        var color = entry?.Color ?? owner.Color;
        var accent = owner.GetColorBrush(color);
        var onAccent = owner.GetOnColorBrush(color);

        var avatar = entry?.ResolveAvatar();
        bool hasVisual = owner.MarkerTemplate is not null || avatar is not null || !string.IsNullOrEmpty(entry?.Icon);
        double markerSize = GetMarkerSize(owner, hasVisual);

        ApplyLayout(owner, markerSize);
        ApplyMarker(owner, entry, avatar, markerSize, filled, accent, onAccent);
        ApplyConnector(owner, trackColored, accent);
        ApplyContent(owner, entry, entryMode);
        ApplyInteraction(owner);
    }

    private static double GetMarkerSize(Timeline2 owner, bool hasVisual)
    {
        if (!double.IsNaN(owner.MarkerSize))
        {
            return owner.MarkerSize;
        }

        double size = Timeline2.IndicatorSizes[(int)owner.Size];
        return hasVisual ? size : Math.Min(size, Timeline2.DotSize);
    }

    private void ApplyLayout(Timeline2 owner, double markerSize)
    {
        bool vertical = owner.Orientation == Orientation.Vertical;
        bool alternate = owner.Align == Timeline2Align.Alternate;
        bool hasOpposite = owner.HasOpposite;
        bool barFirst = owner.Align switch
        {
            Timeline2Align.Start => true,
            Timeline2Align.End => false,
            _ => Index % 2 == 0,
        };

        double top = (markerSize / 2) - 10;
        double gap = double.IsNaN(owner.ItemSpacing) ? (markerSize / 2) + 10 : owner.ItemSpacing;
        double spacing = owner.BarSpacing;

        FrameworkElement first = barFirst ? _opposite : _body;
        FrameworkElement last = barFirst ? _body : _opposite;
        _leading = first;
        _trailing = last;
        _isCentered = !vertical && alternate;

        GridLength opposite = vertical
            ? (hasOpposite || alternate ? StarLength : ZeroLength)
            : (hasOpposite ? AutoLength : ZeroLength);

        if (vertical)
        {
            _col0.Width = barFirst ? opposite : StarLength;
            _col1.Width = AutoLength;
            _col2.Width = barFirst ? StarLength : opposite;
            _row0.Height = StarLength;
            _row1.Height = ZeroLength;
            _row2.Height = ZeroLength;

            first.Margin = new Thickness(0, top, spacing, gap);
            first.HorizontalAlignment = HorizontalAlignment.Right;
            first.VerticalAlignment = VerticalAlignment.Top;

            last.Margin = new Thickness(spacing, top, 0, gap);
            last.HorizontalAlignment = HorizontalAlignment.Stretch;
            last.VerticalAlignment = VerticalAlignment.Top;
        }
        else
        {
            _col0.Width = StarLength;
            _col1.Width = ZeroLength;
            _col2.Width = ZeroLength;

            if (alternate)
            {
                _row0.Height = StarLength;
                _row1.Height = AutoLength;
                _row2.Height = StarLength;
            }
            else
            {
                _row0.Height = barFirst ? opposite : AutoLength;
                _row1.Height = AutoLength;
                _row2.Height = barFirst ? AutoLength : opposite;
            }

            first.Margin = new Thickness(0, 0, gap, spacing);
            first.HorizontalAlignment = HorizontalAlignment.Stretch;
            first.VerticalAlignment = VerticalAlignment.Bottom;

            last.Margin = new Thickness(0, spacing, gap, 0);
            last.HorizontalAlignment = HorizontalAlignment.Stretch;
            last.VerticalAlignment = VerticalAlignment.Top;
        }

        Place(first, 0, vertical);
        Place(_separator, 1, vertical);
        Place(last, 2, vertical);

        _separator.HorizontalAlignment = vertical ? HorizontalAlignment.Center : HorizontalAlignment.Stretch;
        _separator.VerticalAlignment = vertical ? VerticalAlignment.Stretch : VerticalAlignment.Center;
        _sepCol1.Width = vertical ? ZeroLength : StarLength;
        _sepRow1.Height = vertical ? StarLength : ZeroLength;

        var markerHorizontal = vertical ? HorizontalAlignment.Center : HorizontalAlignment.Left;
        var markerVertical = vertical ? VerticalAlignment.Top : VerticalAlignment.Center;
        _marker.HorizontalAlignment = markerHorizontal;
        _marker.VerticalAlignment = markerVertical;
        _markerPresenter.HorizontalAlignment = markerHorizontal;
        _markerPresenter.VerticalAlignment = markerVertical;

        _connector.Width = vertical ? ConnectorThickness : double.NaN;
        _connector.Height = vertical ? double.NaN : ConnectorThickness;
        _connector.HorizontalAlignment = vertical ? HorizontalAlignment.Center : HorizontalAlignment.Stretch;
        _connector.VerticalAlignment = vertical ? VerticalAlignment.Stretch : VerticalAlignment.Center;
        _connector.Margin = vertical
            ? new Thickness(0, ConnectorInset, 0, ConnectorInset)
            : new Thickness(ConnectorInset, 0, ConnectorInset, 0);
        Grid.SetRow(_connector, vertical ? 1 : 0);
        Grid.SetColumn(_connector, vertical ? 0 : 1);
    }

    private static void Place(FrameworkElement element, int slot, bool vertical)
    {
        Grid.SetRow(element, vertical ? 0 : slot);
        Grid.SetColumn(element, vertical ? slot : 0);
    }

    private void ApplyMarker(Timeline2 owner, Timeline2Entry? entry, ImageSource? avatar, double size, bool filled, Brush? accent, Brush? onAccent)
    {
        if (owner.MarkerTemplate is { } template)
        {
            _marker.Visibility = Visibility.Collapsed;
            SetSlot(_markerPresenter, Content, template);
            return;
        }

        SetSlot(_markerPresenter, null, null);
        _marker.Visibility = Visibility.Visible;

        bool outline = owner.Kind == Timeline2Kind.Outline;
        var shape = entry?.Shape ?? owner.MarkerShape;

        _marker.Width = size;
        _marker.Height = size;
        _marker.CornerRadius = shape switch
        {
            Timeline2MarkerShape.Circle => new CornerRadius(size / 2),
            Timeline2MarkerShape.RoundedSquare => new CornerRadius(size / 4),
            _ => new CornerRadius(0),
        };

        if (outline)
        {
            _marker.BorderThickness = new Thickness(2);
            _marker.BorderBrush = filled ? accent : owner.ElevatedBrush;
            _marker.Background = avatar is not null ? GetAvatarBrush(avatar) : owner.SurfaceBrush;
        }
        else
        {
            _marker.BorderThickness = new Thickness(0);
            _marker.BorderBrush = null;
            _marker.Background = avatar is not null ? GetAvatarBrush(avatar) : (filled ? accent : owner.ElevatedBrush);
        }

        string? glyph = avatar is null ? entry?.Icon : null;
        if (string.IsNullOrEmpty(glyph))
        {
            _icon.Visibility = Visibility.Collapsed;
            return;
        }

        var iconBrush = filled ? (outline ? accent : onAccent) : owner.DimmedForeground;
        _icon.Glyph = glyph;
        _icon.FontSize = Math.Max(8, size / 2);
        if (iconBrush is not null)
        {
            _icon.Foreground = iconBrush;
        }

        _icon.Visibility = Visibility.Visible;
    }

    private void ApplyConnector(Timeline2 owner, bool colored, Brush? accent)
    {
        _connector.Background = colored ? accent : owner.ElevatedBrush;
        _connector.Visibility = IsLast ? Visibility.Collapsed : Visibility.Visible;
    }

    private void ApplyContent(Timeline2 owner, Timeline2Entry? entry, bool entryMode)
    {
        SetForeground(_opposite, owner.DimmedForeground);
        SetForeground(_date, owner.DimmedForeground);
        SetForeground(_title, owner.HighlightedForeground);
        SetForeground(_description, owner.MutedForeground);

        object? oppositeContent = null;
        DataTemplate? oppositeTemplate = null;

        if (owner.OppositeTemplate is { } customOpposite)
        {
            oppositeContent = Content;
            oppositeTemplate = customOpposite;
        }
        else if (entry is not null && owner.DatePlacement == Timeline2DatePlacement.Opposite)
        {
            if (owner.DateTemplate is { } dateOpposite)
            {
                oppositeContent = entry;
                oppositeTemplate = dateOpposite;
            }
            else if (!string.IsNullOrEmpty(entry.Date))
            {
                oppositeContent = entry.Date;
            }
        }

        SetSlot(_opposite, oppositeContent, oppositeTemplate);

        if (entryMode && entry is not null)
        {
            bool dateInline = owner.DatePlacement == Timeline2DatePlacement.Content;

            SetSlot(_date, dateInline ? EntrySlot(entry, entry.Date, owner.DateTemplate) : null, dateInline ? owner.DateTemplate : null);
            SetSlot(_title, EntrySlot(entry, entry.Title, owner.TitleTemplate), owner.TitleTemplate);
            SetSlot(_description, EntrySlot(entry, entry.Description, owner.DescriptionTemplate), owner.DescriptionTemplate);
            SetSlot(_content, null, null);
            return;
        }

        SetSlot(_date, null, null);
        SetSlot(_title, null, null);
        SetSlot(_description, null, null);

        if (!ReferenceEquals(_content.ContentTemplateSelector, ContentTemplateSelector))
        {
            _content.ContentTemplateSelector = ContentTemplateSelector;
        }

        SetSlot(_content, Content, ContentTemplate);
    }

    private void ApplyInteraction(Timeline2 owner)
    {
        bool clickable = owner.IsItemClickEnabled;
        IsTabStop = clickable;

        if (clickable == _clickable)
        {
            return;
        }

        _clickable = clickable;
        s_handCursor ??= InputSystemCursor.Create(InputSystemCursorShape.Hand);
        ProtectedCursor = clickable ? s_handCursor : null;
    }

    private static object? EntrySlot(Timeline2Entry entry, string? text, DataTemplate? template)
    {
        if (template is not null)
        {
            return entry;
        }

        return string.IsNullOrEmpty(text) ? null : text;
    }

    private static void SetSlot(ContentPresenter presenter, object? content, DataTemplate? template)
    {
        if (!ReferenceEquals(presenter.ContentTemplate, template))
        {
            presenter.Content = null;
            presenter.ContentTemplate = template;
        }

        presenter.Content = content;
        presenter.Visibility = content is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private static void SetForeground(ContentPresenter presenter, Brush? brush)
    {
        if (brush is not null)
        {
            presenter.Foreground = brush;
        }
    }

    private ImageBrush GetAvatarBrush(ImageSource source)
    {
        _avatarBrush ??= new ImageBrush { Stretch = Stretch.UniformToFill };

        if (!ReferenceEquals(_avatarBrush.ImageSource, source))
        {
            _avatarBrush.ImageSource = source;
        }

        return _avatarBrush;
    }
}
