namespace DevWinUI;

public sealed partial class ScrollSplitCard
{
    public Uri? Image
    {
        get { return (Uri?)GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }

    public static readonly DependencyProperty ImageProperty =
        DependencyProperty.Register(nameof(Image), typeof(Uri), typeof(ScrollSplitCard), new PropertyMetadata(null, (d, _) => ((ScrollSplitCard)d).Rebuild()));

    public string ImageAlt
    {
        get { return (string)GetValue(ImageAltProperty); }
        set { SetValue(ImageAltProperty, value); }
    }

    public static readonly DependencyProperty ImageAltProperty =
        DependencyProperty.Register(nameof(ImageAlt), typeof(string), typeof(ScrollSplitCard), new PropertyMetadata("", (d, _) => ((ScrollSplitCard)d).UpdateAutomation()));

    public IReadOnlyList<ScrollSplitCardItem> Cards
    {
        get { return (IReadOnlyList<ScrollSplitCardItem>)GetValue(CardsProperty); }
        set { SetValue(CardsProperty, value); }
    }

    public static readonly DependencyProperty CardsProperty =
        DependencyProperty.Register(nameof(Cards), typeof(IReadOnlyList<ScrollSplitCardItem>), typeof(ScrollSplitCard), new PropertyMetadata(Array.Empty<ScrollSplitCardItem>(), (d, _) => ((ScrollSplitCard)d).Rebuild()));

    public ScrollSplitCardTone Tone
    {
        get { return (ScrollSplitCardTone)GetValue(ToneProperty); }
        set { SetValue(ToneProperty, value); }
    }

    public static readonly DependencyProperty ToneProperty =
        DependencyProperty.Register(nameof(Tone), typeof(ScrollSplitCardTone), typeof(ScrollSplitCard), new PropertyMetadata(ScrollSplitCardTone.Card, (d, _) => ((ScrollSplitCard)d).Retone()));

    public ScrollSplitCardSize Size
    {
        get { return (ScrollSplitCardSize)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(ScrollSplitCardSize), typeof(ScrollSplitCard), new PropertyMetadata(ScrollSplitCardSize.Medium, (d, _) => ((ScrollSplitCard)d).Rebuild()));

    public string Hint
    {
        get { return (string)GetValue(HintProperty); }
        set { SetValue(HintProperty, value); }
    }

    public static readonly DependencyProperty HintProperty =
        DependencyProperty.Register(nameof(Hint), typeof(string), typeof(ScrollSplitCard), new PropertyMetadata("Scroll", (d, _) => ((ScrollSplitCard)d).UpdateTexts()));

    public string EndLabel
    {
        get { return (string)GetValue(EndLabelProperty); }
        set { SetValue(EndLabelProperty, value); }
    }

    public static readonly DependencyProperty EndLabelProperty =
        DependencyProperty.Register(nameof(EndLabel), typeof(string), typeof(ScrollSplitCard), new PropertyMetadata("", (d, _) => ((ScrollSplitCard)d).UpdateTexts()));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(ScrollSplitCard), new PropertyMetadata("Scroll split card", (d, _) => ((ScrollSplitCard)d).UpdateAutomation()));

    public bool IsAnimationEnabled
    {
        get { return (bool)GetValue(IsAnimationEnabledProperty); }
        set { SetValue(IsAnimationEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAnimationEnabledProperty =
        DependencyProperty.Register(nameof(IsAnimationEnabled), typeof(bool), typeof(ScrollSplitCard), new PropertyMetadata(true, (d, _) => ((ScrollSplitCard)d).ApplyMotionMode()));
}
