using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;
public partial class ClockRadioButton : RadioButton
{
    private Ellipse _focusRing;

    public int Num { get; set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _focusRing = GetTemplateChild("FocusRing") as Ellipse;
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        if (_focusRing != null)
        {
            //Only show the ring for keyboard focus, never for pointer/programmatic focus.
            _focusRing.Visibility = FocusState == FocusState.Keyboard ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        if (_focusRing != null)
        {
            _focusRing.Visibility = Visibility.Collapsed;
        }
    }
}
