using Microsoft.UI.Xaml.Input;

namespace DevWinUI;
public partial class AccordionPanel
{
    private bool _isBusy = false;
    private int _lastDeltaSign = 0;

    private AccordionItem _current = null;

    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (!_isBusy)
        {
            var point = e.GetCurrentPoint(this);
            int sign = Math.Sign(point.Properties.MouseWheelDelta);

            var control = GetEligibleControl(point.Properties.MouseWheelDelta);
            if (sign < 0 && this.SelectedIndex != this.Children.Count - 1)
            {
                this.AnimateNext(control);
            }
            if (sign > 0 && this.SelectedIndex > 0)
            {
                this.AnimatePrev(control);
            }
        }
    }

    private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        _current = null;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if ((Math.Sign(e.Delta.Translation.Y) < 0 && this.SelectedIndex == this.Children.Count - 1))
        {
            e.Complete();
            return;
        }

        _current = _current ?? GetEligibleControl(e.Delta.Translation.Y);

        double deltaY = e.Delta.Translation.Y;
        double translateY = _current.GetTranslateY();
        _lastDeltaSign = Math.Sign(deltaY);

        double y = Math.Max(GetTopBound(), translateY + deltaY);
        y = Math.Min(0, y);
        _current.TranslateY(y);
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (_current != null)
        {
            if (_lastDeltaSign < 0)
            {
                this.AnimateNext(_current);
            }
            else
            {
                this.AnimatePrev(_current);
            }
        }
    }

    private async void AnimateNext(AccordionItem control)
    {
        _isBusy = true;
        await control.AnimateYAsync(GetTopBound());
        _isBusy = false;

        if (!control.IsUp)
        {
            control.IsUp = true;
            this.SelectedIndex++;
        }
    }

    private async void AnimatePrev(AccordionItem control)
    {
        _isBusy = true;
        await control.AnimateYAsync(0);
        _isBusy = false;

        if (control.IsUp)
        {
            control.IsUp = false;
            this.SelectedIndex--;
        }
    }

    private AccordionItem GetEligibleControl(double deltaY)
    {
        int index = 0;
        if (this.SelectedIndex > 0)
        {
            if (Math.Sign(deltaY) < 0)
            {
                index = this.SelectedIndex;
            }
            else
            {
                index = this.SelectedIndex - 1;
            }
        }
        return base.Children[base.Children.Count - index - 1] as AccordionItem;
    }
}
