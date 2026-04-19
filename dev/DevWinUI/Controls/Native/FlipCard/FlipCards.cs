namespace DevWinUI;

public partial class FlipCards : StackPanel
{
    private const char space = ' ';
    private const string time = "HH mm ss";
    private const string date = "dd MM yyyy";
    private const string date_time = "HH mm ss  dd MM yyyy";
    private const string invalid_source = "Invalid argument";

    private int _count;

    public string Value
    {
        get { return (string)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(FlipCards), new PropertyMetadata(null, OnValueChanged));

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FlipCards)d;
        if (ctl != null)
        {
            ctl.AddLayout();
        }
    }
    public FlipCardsSourceType Source
    {
        get { return (FlipCardsSourceType)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(FlipCardsSourceType), typeof(FlipCards), new PropertyMetadata(FlipCardsSourceType.Time));


    public FlipCards()
    {
        Orientation = Orientation.Horizontal;
        var timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };
        timer.Tick += (object s, object args) =>
        {
            if (Source != FlipCardsSourceType.Value)
            {
                var format = Source switch
                {
                    FlipCardsSourceType.Time => time,
                    FlipCardsSourceType.Date => date,
                    FlipCardsSourceType.TimeDate => date_time,
                    _ => throw new ArgumentException(invalid_source)
                };
                Value = DateTime.Now.ToString(format);
            }
        };
        timer.Start();
    }
    private void SetElement(string name, char glyph)
    {
        var element = Children.Cast<FrameworkElement>()
        .FirstOrDefault(f => (string)f.Tag == name);
        if (element is FlipBlock flipCard)
        {
            flipCard.Value = glyph.ToString();
        }
    }

    private void AddElement(string name)
    {
        FrameworkElement element = name == null
            ? new Canvas
            {
                Width = 5
            }
            : new FlipBlock()
            {
                Tag = name
            };
        Children.Add(element);
    }

    private void AddLayout()
    {
        if (Value == null)
            return;

        var array = Value.ToCharArray();
        var length = array.Length;
        var list = Enumerable.Range(0, length);
        if (_count != length)
        {
            Children.Clear();
            foreach (int item in list)
            {
                AddElement((array[item] == space)
                ? null : item.ToString());
            }
            _count = length;
        }
        foreach (int item in list)
        {
            SetElement(item.ToString(), array[item]);
        }
    }
}
