namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Panel), Type = typeof(StackPanel))]
public partial class DigitalSegment : Control
{
    private const string PART_Panel = "PART_Panel";
    private StackPanel panel;
    private DispatcherTimer scrollTimer;
    private int scrollIndex = 0;

    public DigitalSegment()
    {
        DefaultStyleKey = typeof(DigitalSegment);
    }

    private void UpdateText()
    {
        if(panel == null || Model == null)
            return;

        panel.Children.Clear();

        int count = SymbolCount > 0 ? SymbolCount : (Text?.Length ?? 0);

        for (int i = 0; i < count; i++)
        {
            string charToShow = string.Empty;

            if (!string.IsNullOrEmpty(Text) && i < Text.Length)
                charToShow = Text[i].ToString();

            var digit = Model.Clone();
            digit.Angle = this.Angle;
            digit.ColonBackground = this.ColonBackground;
            digit.ColonForeground = this.ColonForeground;
            digit.SegmentForeground = this.SegmentForeground;
            digit.SegmentBackground = this.SegmentBackground;
            digit.Stroke = this.Stroke;
            digit.StrokeThickness = this.StrokeThickness;
            digit.IsColonBlink = this.IsColonBlink;
            digit.MatrixDotGap = this.MatrixDotGap;
            digit.IsMatrixSquare = this.IsMatrixSquare;
            digit.MatrixDotSize = this.MatrixDotSize;
            digit.Character = charToShow;

            panel.Children.Add(digit);
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        panel = GetTemplateChild(PART_Panel) as StackPanel;

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
        int visibleCount = panel?.Children.Count ?? 0;
        return Text + new string(' ', visibleCount);
    }

    private void ScrollStep(object sender, object e)
    {
        if (string.IsNullOrEmpty(Text) || panel == null || panel.Children.Count == 0)
            return;

        string buffer = GetScrollingBuffer();
        int visibleCount = panel.Children.Count;

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

            var digit = (SegmentChar)panel.Children[i];
            digit.Character = buffer[charIndex].ToString();
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
