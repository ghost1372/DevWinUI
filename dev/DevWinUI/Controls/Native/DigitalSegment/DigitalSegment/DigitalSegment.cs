namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Repeater), Type = typeof(ItemsRepeater))]
public partial class DigitalSegment : Control
{
    private const string PART_Repeater = "PART_Repeater";
    private ItemsRepeater? repeater;
    private DispatcherTimer? scrollTimer;
    private string scrollBuffer = string.Empty;
    private int scrollIndex;

    private ObservableCollection<SegmentChar> Digits { get; } = [];

    public DigitalSegment()
    {
        DefaultStyleKey = typeof(DigitalSegment);
    }

    private void UpdateText()
    {
        if (repeater == null || Model == null)
            return;

        int count = SymbolCount > 0 ? SymbolCount : (Text?.Length ?? 0);

        for (int i = 0; i < count; i++)
        {
            string charToShow = string.Empty;

            if (!string.IsNullOrEmpty(Text) && i < Text.Length)
                charToShow = Text[i].ToString();

            SegmentChar digit;
            if (i < Digits.Count && Digits[i].GetType() == Model.GetType())
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

        string updatedScrollBuffer = string.IsNullOrEmpty(Text) ? string.Empty : Text + new string(' ', Digits.Count);
        if (!string.Equals(scrollBuffer, updatedScrollBuffer, StringComparison.Ordinal))
        {
            scrollBuffer = updatedScrollBuffer;
            scrollIndex = 0;
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        repeater = GetTemplateChild(PART_Repeater) as ItemsRepeater;
        if (repeater != null)
            repeater.ItemsSource = Digits;

        UpdateText();
    }

    private static void OnScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DigitalSegment ctl)
        {
            if ((bool)e.NewValue)
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
            scrollTimer.Interval = ScrollSpeed;
            scrollTimer.Tick += ScrollStep;
        }
        scrollTimer.Start();
    }

    private void StopScrolling()
    {
        scrollTimer?.Stop();
    }

    private string GetScrollingBuffer()
    {
        return scrollBuffer;
    }

    private void ScrollStep(object sender, object e)
    {
        if (string.IsNullOrEmpty(Text) || Digits.Count == 0)
            return;

        string buffer = GetScrollingBuffer();
        int visibleCount = Digits.Count;

        for (int i = 0; i < visibleCount; i++)
        {
            int charIndex;

            if (ScrollDirection == DigitalSegmentScrollDirection.RightToLeft)
            {
                charIndex = (scrollIndex + i) % buffer.Length;
            }
            else
            {
                charIndex = (scrollIndex - (visibleCount - 1 - i) + buffer.Length) % buffer.Length;
            }

            Digits[i].Character = buffer[charIndex].ToString();
        }

        if (ScrollDirection == DigitalSegmentScrollDirection.RightToLeft)
        {
            scrollIndex++;
            if (scrollIndex >= buffer.Length) scrollIndex = 0;
        }
        else
        {
            scrollIndex--;
            if (scrollIndex < 0) scrollIndex = buffer.Length - 1;
        }
    }
}
