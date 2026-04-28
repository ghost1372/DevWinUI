namespace DevWinUI;

public sealed partial class ShortcutWithTextLabel2 : Control
{
    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ShortcutWithTextLabel2), new PropertyMetadata(default(string)));

    public List<object> Keys
    {
        get { return (List<object>)GetValue(KeysProperty); }
        set { SetValue(KeysProperty, value); }
    }

    public static readonly DependencyProperty KeysProperty = DependencyProperty.Register(nameof(Keys), typeof(List<object>), typeof(ShortcutWithTextLabel2), new PropertyMetadata(default(string)));

    public Placement LabelPlacement
    {
        get { return (Placement)GetValue(LabelPlacementProperty); }
        set { SetValue(LabelPlacementProperty, value); }
    }

    public static readonly DependencyProperty LabelPlacementProperty = DependencyProperty.Register(nameof(LabelPlacement), typeof(Placement), typeof(ShortcutWithTextLabel2), new PropertyMetadata(defaultValue: Placement.After, OnIsLabelPlacementChanged));

    public Style KeyVisualStyle
    {
        get { return (Style)GetValue(KeyVisualStyleProperty); }
        set { SetValue(KeyVisualStyleProperty, value); }
    }

    public static readonly DependencyProperty KeyVisualStyleProperty = DependencyProperty.Register(nameof(KeyVisualStyle), typeof(Style), typeof(ShortcutWithTextLabel2), new PropertyMetadata(default(Style)));

    public ShortcutWithTextLabel2()
    {
        DefaultStyleKey = typeof(ShortcutWithTextLabel2);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    private static void OnIsLabelPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs newValue)
    {
        if (d is ShortcutWithTextLabel2 labelControl)
        {
            if (labelControl.LabelPlacement == Placement.Before)
            {
                VisualStateManager.GoToState(labelControl, "LabelBefore", true);
            }
            else
            {
                VisualStateManager.GoToState(labelControl, "LabelAfter", true);
            }
        }
    }

    public enum Placement
    {
        Before,
        After,
    }
}
