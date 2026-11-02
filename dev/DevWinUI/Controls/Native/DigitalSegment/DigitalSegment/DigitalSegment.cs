namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Repeater), Type = typeof(ItemsRepeater))]
public partial class DigitalSegment : Control
{
    private const string PART_Repeater = "PART_Repeater";
    private ItemsRepeater? repeater;
    private DispatcherTimer? scrollTimer;
    private string scrollBuffer = string.Empty;
    private string? scrollBufferText;
    private int scrollBufferSymbols = -1;
    private int scrollIndex;

    private ObservableCollection<SegmentChar> Digits { get; } = [];

    public DigitalSegment()
    {
        DefaultStyleKey = typeof(DigitalSegment);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (IsScrolling)
            StartScrolling();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // A running DispatcherTimer keeps ticking (and keeps the control alive) after it leaves the tree.
        StopScrolling();
    }

    private void UpdateText()
    {
        if (repeater == null || Model == null)
            return;

        string? text = Text;
        int count = SymbolCount > 0 ? SymbolCount : (text?.Length ?? 0);
        Type modelType = Model.GetType();

        for (int i = 0; i < count; i++)
        {
            string charToShow = !string.IsNullOrEmpty(text) && i < text.Length
                ? DigitalSegmentHelper.CharToString(text[i])
                : string.Empty;

            SegmentChar digit;
            if (i < Digits.Count && Digits[i].GetType() == modelType)
            {
                digit = Digits[i];
            }
            else
            {
                digit = Model.Clone();

                if (i < Digits.Count)
                    Digits[i] = digit;
                else
                    Digits.Add(digit);
            }

            digit.SynchronizeAppearanceFrom(this);
            digit.Character = charToShow;
        }

        while (Digits.Count > count)
            Digits.RemoveAt(Digits.Count - 1);

        // Only rebuild the scroll buffer when its inputs changed.
        if (scrollBufferSymbols != Digits.Count || !string.Equals(scrollBufferText, text, StringComparison.Ordinal))
        {
            scrollBufferText = text;
            scrollBufferSymbols = Digits.Count;
            scrollBuffer = string.IsNullOrEmpty(text) ? string.Empty : text.PadRight(text.Length + Digits.Count);
            scrollIndex = 0;
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        repeater = GetTemplateChild(PART_Repeater) as ItemsRepeater;
        if (repeater != null)
            repeater.ItemsSource = Digits;

        UpdateSpacing();
        UpdateText();
    }

    private static void OnScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DigitalSegment ctl)
        {
            // Not loaded yet: OnLoaded starts the timer.
            if ((bool)e.NewValue && ctl.IsLoaded)
                ctl.StartScrolling();
            else
                ctl.StopScrolling();
        }
    }

    private void StartScrolling()
    {
        if (scrollTimer == null)
        {
            scrollTimer = new DispatcherTimer();
            scrollTimer.Tick += ScrollStep;
        }

        scrollTimer.Interval = ScrollSpeed;
        scrollTimer.Start();
    }

    private void StopScrolling()
    {
        scrollTimer?.Stop();
    }

    private void UpdateScrollSpeed()
    {
        if (scrollTimer != null)
            scrollTimer.Interval = ScrollSpeed;
    }

    private void UpdateSpacing()
    {
        // The spacing lives on the layout of the repeater and can't be a TemplateBinding.
        if (repeater?.Layout is StackLayout layout)
            layout.Spacing = Spacing;
    }

    private void ScrollStep(object? sender, object e)
    {
        string buffer = scrollBuffer;
        int visibleCount = Digits.Count;

        if (buffer.Length == 0 || visibleCount == 0)
            return;

        bool rightToLeft = ScrollDirection == DigitalSegmentScrollDirection.RightToLeft;

        for (int i = 0; i < visibleCount; i++)
        {
            int charIndex = rightToLeft
                ? (scrollIndex + i) % buffer.Length
                : (scrollIndex - (visibleCount - 1 - i) + buffer.Length) % buffer.Length;

            Digits[i].Character = DigitalSegmentHelper.CharToString(buffer[charIndex]);
        }

        if (rightToLeft)
        {
            scrollIndex++;
            if (scrollIndex >= buffer.Length)
                scrollIndex = 0;
        }
        else
        {
            scrollIndex--;
            if (scrollIndex < 0)
                scrollIndex = buffer.Length - 1;
        }
    }
}
