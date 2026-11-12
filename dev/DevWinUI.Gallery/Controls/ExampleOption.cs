using System.Collections;
using System.Collections.Specialized;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;

namespace DevWinUIGallery;

public abstract partial class ExampleOption : UserControl
{
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(ExampleOption), new PropertyMetadata(null, OnHeaderChanged));

    private readonly TextBlock _header = new() { FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, Visibility = Visibility.Collapsed };
    private readonly StackPanel _root = new() { Spacing = 6 };
    private bool _captured;

    protected ExampleOption()
    {
        _root.Children.Add(_header);
        Content = _root;
        HorizontalAlignment = HorizontalAlignment.Stretch;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        Loaded += (_, _) =>
        {
            if (_captured)
            {
                return;
            }

            _captured = true;
            CaptureDefault();
        };
    }

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public void Reset()
    {
        if (_captured)
        {
            ResetValue();
        }
    }

    protected void SetEditor(FrameworkElement editor)
    {
        _root.Children.Add(editor);
    }

    protected abstract void CaptureDefault();

    protected abstract void ResetValue();

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var option = (ExampleOption)d;
        var text = e.NewValue as string;
        option._header.Text = text ?? string.Empty;
        option._header.Visibility = string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
    }
}

public sealed partial class OptionPanel : StackPanel
{
    public OptionPanel()
    {
        Spacing = 16;
    }
}

public sealed partial class BrushItem : DependencyObject
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(BrushItem), new PropertyMetadata(null));

    public static readonly DependencyProperty BrushProperty =
        DependencyProperty.Register(nameof(Brush), typeof(Brush), typeof(BrushItem), new PropertyMetadata(null));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Brush Brush
    {
        get => (Brush)GetValue(BrushProperty);
        set => SetValue(BrushProperty, value);
    }
}

public sealed partial class CollectionOption : ExampleOption
{
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(object), typeof(CollectionOption), new PropertyMetadata(null, (d, _) => ((CollectionOption)d).Bind()));

    private readonly ListView _list = new() { MaxHeight = 140 };
    private readonly TextBox _input = new() { PlaceholderText = "New item" };
    private List<object> _default = new();
    private INotifyCollectionChanged _observed;
    private bool _suppress;

    public CollectionOption()
    {
        var add = new Button { Content = "Add" };
        var remove = new Button { Content = "Remove" };
        var clear = new Button { Content = "Clear" };

        add.Click += (_, _) => Add();
        remove.Click += (_, _) => Remove();
        clear.Click += (_, _) =>
        {
            Items?.Clear();
            Refresh();
        };

        var inputRow = new Grid { ColumnSpacing = 8 };
        inputRow.ColumnDefinitions.Add(new ColumnDefinition());
        inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(add, 1);
        inputRow.Children.Add(_input);
        inputRow.Children.Add(add);

        var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { remove, clear } };
        SetEditor(new StackPanel { Spacing = 8, Children = { inputRow, _list, actions } });
    }

    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public event TypedEventHandler<CollectionOption, IList> ValueChanged;

    public Func<int, object> ItemFactory { get; set; }

    private IList Items => Source as IList;

    protected override void CaptureDefault()
    {
        _default = Items?.Cast<object>().ToList() ?? new List<object>();
    }

    protected override void ResetValue()
    {
        if (Items is not { } items)
        {
            return;
        }

        _suppress = true;
        try
        {
            items.Clear();
            foreach (var item in _default)
            {
                items.Add(item);
            }
        }
        finally
        {
            _suppress = false;
        }

        Refresh();
        if (items is INotifyCollectionChanged)
        {
            RaiseValueChanged();
        }
    }

    private void Bind()
    {
        if (_observed is not null)
        {
            _observed.CollectionChanged -= OnCollectionChanged;
        }

        _observed = Items as INotifyCollectionChanged;
        if (_observed is not null)
        {
            _observed.CollectionChanged += OnCollectionChanged;
        }

        _list.ItemsSource = Items;
        RaiseValueChanged();
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_suppress)
        {
            RaiseValueChanged();
        }
    }

    private void RaiseValueChanged() => ValueChanged?.Invoke(this, Items);

    private void Refresh()
    {
        if (Items is null or INotifyCollectionChanged)
        {
            return;
        }

        _list.ItemsSource = null;
        _list.ItemsSource = Items;
        RaiseValueChanged();
    }

    private void Add()
    {
        if (Items is not { } items)
        {
            return;
        }

        var text = _input.Text;
        items.Add(string.IsNullOrWhiteSpace(text) ? ItemFactory?.Invoke(items.Count) ?? $"Item {items.Count + 1}" : text);
        _input.Text = string.Empty;
        Refresh();
    }

    private void Remove()
    {
        if (Items is not { Count: > 0 } items)
        {
            return;
        }

        var index = _list.SelectedIndex >= 0 ? _list.SelectedIndex : items.Count - 1;
        items.RemoveAt(index);
        Refresh();
    }
}

public sealed partial class ColorOption : ExampleOption
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(Color), typeof(ColorOption), new PropertyMetadata(Microsoft.UI.Colors.Transparent, OnValueChanged));

    public static readonly DependencyProperty BrushProperty =
        DependencyProperty.Register(nameof(Brush), typeof(Brush), typeof(ColorOption), new PropertyMetadata(null, OnBrushChanged));

    public static readonly DependencyProperty IsAlphaEnabledProperty =
        DependencyProperty.Register(nameof(IsAlphaEnabled), typeof(bool), typeof(ColorOption), new PropertyMetadata(true, OnChanged));

    private readonly ColorPicker _picker = new();
    private readonly SolidColorBrush _swatchBrush = new();
    private readonly TextBlock _hex = new() { VerticalAlignment = VerticalAlignment.Center };
    private bool _sync;
    private bool _fromEditor;
    private Color _default;

    public ColorOption()
    {
        var swatch = new Border { Width = 20, Height = 20, CornerRadius = new CornerRadius(4), Background = _swatchBrush };
        var content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, Children = { swatch, _hex } };

        _picker.ColorChanged += (_, e) =>
        {
            if (_sync)
            {
                return;
            }

            _fromEditor = true;
            try
            {
                Value = e.NewColor;
            }
            finally
            {
                _fromEditor = false;
            }
        };

        SetEditor(new DropDownButton
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Content = content,
            Flyout = new Flyout { Content = _picker }
        });
    }

    public event TypedEventHandler<ColorOption, Color> ValueChanged;

    public Color Value
    {
        get => (Color)GetValue(ValueProperty);
        set
        {
            // Break x:Bind TwoWay feedback loops: ignore writes that don't change anything.
            if (Equals(GetValue(ValueProperty), value))
            {
                return;
            }

            SetValue(ValueProperty, value);
        }
    }

    /// <summary>
    /// Brush view of <see cref="Value"/>. Always a SolidColorBrush when written by this option.
    /// Setting a SolidColorBrush updates Value; non-solid brushes (gradients etc.) are ignored.
    /// </summary>
    public Brush Brush
    {
        get => (Brush)GetValue(BrushProperty);
        set
        {
            // Same color => nothing to do (breaks x:Bind TwoWay loops).
            if (value is SolidColorBrush next && GetValue(BrushProperty) is SolidColorBrush current && current.Color == next.Color)
            {
                return;
            }

            SetValue(BrushProperty, value);
        }
    }

    public bool IsAlphaEnabled
    {
        get => (bool)GetValue(IsAlphaEnabledProperty);
        set => SetValue(IsAlphaEnabledProperty, value);
    }

    protected override void CaptureDefault() => _default = Value;

    protected override void ResetValue() => Value = _default;

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ColorOption)d).Sync();
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var option = (ColorOption)d;
        option.Sync();
        option.ValueChanged?.Invoke(option, (Color)e.NewValue);
    }

    private static void OnBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var option = (ColorOption)d;
        if (e.NewValue is SolidColorBrush solid && solid.Color != option.Value)
        {
            option.Value = solid.Color; // triggers Sync(), which sees Brush already matches
        }
    }

    private void Sync()
    {
        var color = Value;
        _sync = true;
        try
        {
            if (_picker.IsAlphaEnabled != IsAlphaEnabled)
            {
                _picker.IsAlphaEnabled = IsAlphaEnabled;
            }

            // Never write back into the picker while it is raising ColorChanged.
            if (!_fromEditor && _picker.Color != color)
            {
                _picker.Color = color;
            }

            _swatchBrush.Color = color;
            _hex.Text = IsAlphaEnabled ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}" : $"#{color.R:X2}{color.G:X2}{color.B:X2}";

            // Keep Brush in step with Value.
            if (GetValue(BrushProperty) is not SolidColorBrush current || current.Color != color)
            {
                Brush = new SolidColorBrush(color);
            }
        }
        finally
        {
            _sync = false;
        }
    }
}

[ContentProperty(Name = nameof(Items))]
public sealed partial class BrushOption : ExampleOption
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(Brush), typeof(BrushOption), new PropertyMetadata(null, OnValueChanged));

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(DependencyObjectCollection), typeof(BrushOption), new PropertyMetadata(null));

    private readonly ComboBox _editor = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
    private bool _sync;
    private bool _fromEditor;
    private Brush _default;

    public BrushOption()
    {
        Items = new DependencyObjectCollection();
        _editor.SelectionChanged += (_, _) =>
        {
            if (_sync || _editor.SelectedItem is not ComboBoxItem { Tag: Brush brush })
            {
                return;
            }

            _fromEditor = true;
            try
            {
                Value = brush;
            }
            finally
            {
                _fromEditor = false;
            }
        };
        SetEditor(_editor);
    }

    public event TypedEventHandler<BrushOption, Brush> ValueChanged;

    public Brush Value
    {
        get => (Brush)GetValue(ValueProperty);
        set
        {
            if (Same((Brush)GetValue(ValueProperty), value))
            {
                return;
            }

            SetValue(ValueProperty, value);
        }
    }

    public DependencyObjectCollection Items
    {
        get => (DependencyObjectCollection)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    protected override void CaptureDefault()
    {
        Build();
        _default = Value;
        Sync();
    }

    protected override void ResetValue() => Value = _default;

    private static (string Title, Brush Brush)[] Palette() => new (string, Brush)[]
    {
        ("Red", new SolidColorBrush(Microsoft.UI.Colors.IndianRed)),
        ("Green", new SolidColorBrush(Microsoft.UI.Colors.SeaGreen)),
        ("Blue", new SolidColorBrush(Microsoft.UI.Colors.SteelBlue)),
        ("Orange", new SolidColorBrush(Microsoft.UI.Colors.DarkOrange)),
        ("Purple", new SolidColorBrush(Microsoft.UI.Colors.MediumPurple)),
        ("Gray", new SolidColorBrush(Microsoft.UI.Colors.Gray))
    };

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var option = (BrushOption)d;
        option.Sync();
        option.ValueChanged?.Invoke(option, e.NewValue as Brush);
    }

    private static bool Same(Brush a, Brush b)
    {
        return ReferenceEquals(a, b) || (a is SolidColorBrush x && b is SolidColorBrush y && x.Color == y.Color);
    }

    private static ComboBoxItem CreateItem(string title, Brush brush)
    {
        // Each item gets its own swatch brush; the Tag keeps the real brush.
        var swatch = brush is SolidColorBrush solid ? new SolidColorBrush(solid.Color) : brush;
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        panel.Children.Add(new Border { Width = 16, Height = 16, CornerRadius = new CornerRadius(8), Background = swatch });
        panel.Children.Add(new TextBlock { Text = title, VerticalAlignment = VerticalAlignment.Center });
        return new ComboBoxItem { Content = panel, Tag = brush };
    }

    private void Build()
    {
        var entries = Items.OfType<BrushItem>().Select(x => (x.Title, x.Brush)).ToArray();
        if (entries.Length == 0)
        {
            entries = Palette();
        }

        _sync = true;
        try
        {
            _editor.Items.Clear();
            foreach (var (title, brush) in entries)
            {
                _editor.Items.Add(CreateItem(title, brush));
            }
        }
        finally
        {
            _sync = false;
        }
    }

    private void Sync()
    {
        // The ComboBox already shows the user's pick; don't touch it from inside SelectionChanged.
        if (_fromEditor)
        {
            return;
        }

        var match = _editor.Items.OfType<ComboBoxItem>().FirstOrDefault(x => Same(x.Tag as Brush, Value));
        if (ReferenceEquals(_editor.SelectedItem, match))
        {
            return;
        }

        _sync = true;
        try
        {
            _editor.SelectedItem = match;
        }
        finally
        {
            _sync = false;
        }
    }
}

public sealed partial class ChoiceOption : ExampleOption
{
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(object), typeof(ChoiceOption), new PropertyMetadata(null, OnValueChanged));

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(ChoiceOption), new PropertyMetadata(null, OnChanged));

    private readonly ComboBox _editor = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
    private Type _enumType;
    private bool _sync;
    private bool _fromEditor;
    private object _default;

    public ChoiceOption()
    {
        _editor.SelectionChanged += (_, _) =>
        {
            if (_sync || _editor.SelectedItem is not { } item)
            {
                return;
            }

            object newValue = item;
            if (_enumType != null)
            {
                if (item is not string name || !Enum.TryParse(_enumType, name, out var parsed))
                {
                    return;
                }

                newValue = parsed;
            }

            _fromEditor = true;
            try
            {
                Value = newValue;
            }
            finally
            {
                _fromEditor = false;
            }
        };
        SetEditor(_editor);
    }

    public event TypedEventHandler<ChoiceOption, object> ValueChanged;

    public object Value
    {
        get => GetValue(ValueProperty);
        set
        {
            // Boxed enums are new objects every time, so compare by value.
            if (Equals(GetValue(ValueProperty), value))
            {
                return;
            }

            SetValue(ValueProperty, value);
        }
    }

    public object ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    protected override void CaptureDefault() => _default = Value;

    protected override void ResetValue() => Value = _default;

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ChoiceOption)d).Sync();
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var option = (ChoiceOption)d;
        option.Sync();
        option.ValueChanged?.Invoke(option, e.NewValue);
    }

    private void Sync()
    {
        // Change came from the ComboBox itself: its selection is already correct,
        // and touching it inside SelectionChanged is what throws.
        if (_fromEditor)
        {
            return;
        }

        _sync = true;
        try
        {
            if (ItemsSource != null)
            {
                _enumType = null;
                if (!ReferenceEquals(_editor.ItemsSource, ItemsSource))
                {
                    _editor.ItemsSource = ItemsSource;
                }

                if (!Equals(_editor.SelectedItem, Value))
                {
                    _editor.SelectedItem = Value;
                }
            }
            else if (Value is Enum value)
            {
                var type = value.GetType();
                if (_enumType != type)
                {
                    _enumType = type;
                    _editor.ItemsSource = Enum.GetNames(type);
                }

                var name = value.ToString();
                if (!Equals(_editor.SelectedItem, name))
                {
                    _editor.SelectedItem = name;
                }
            }
        }
        finally
        {
            _sync = false;
        }
    }
}
