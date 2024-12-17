using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = HeaderContentPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = DescriptionPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_Root, Type = typeof(Grid))]
[TemplatePart(Name = PART_ConfirmButton, Type = typeof(Button))]
[TemplatePart(Name = PART_CalendarWithClockView, Type = typeof(CalendarWithClock))]
public partial class DateTimePicker : DateTimeBase
{
    private const string HeaderContentPresenter = "HeaderContentPresenter";
    private const string DescriptionPresenter = "DescriptionPresenter";
    private const string PART_Root = "PART_Root";
    private const string PART_ConfirmButton = "PART_ConfirmButton";
    private const string PART_CalendarWithClockView = "PART_CalendarWithClockView";
    private ContentPresenter headerContentPresenter;
    private ContentPresenter descriptionContentPresenter;
    private Grid rootGrid;
    private Button confirmButton;
    private CalendarWithClock calendarWithClock;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        headerContentPresenter = GetTemplateChild(nameof(HeaderContentPresenter)) as ContentPresenter;
        descriptionContentPresenter = GetTemplateChild(nameof(DescriptionPresenter)) as ContentPresenter;

        UpdateHeaderVisibility();
        UpdateDescriptionVisibility();

        rootGrid = GetTemplateChild(PART_Root) as Grid;
        confirmButton = GetTemplateChild(PART_ConfirmButton) as Button;
        calendarWithClock = GetTemplateChild(PART_CalendarWithClockView) as CalendarWithClock;

        if (rootGrid != null)
        {
            rootGrid.PointerEntered -= OnPointerEntered;
            rootGrid.PointerEntered += OnPointerEntered;
            rootGrid.PointerExited -= OnPointerExited;
            rootGrid.PointerExited += OnPointerExited;
            rootGrid.PointerReleased -= OnPointerReleased;
            rootGrid.PointerReleased += OnPointerReleased;
            rootGrid.PointerPressed -= OnPointerPressed;
            rootGrid.PointerPressed += OnPointerPressed;
        }

        if (confirmButton != null)
        {
            confirmButton.Click -= OnConfirmButton;
            confirmButton.Click += OnConfirmButton;
        }

        if (calendarWithClock != null)
        {
            calendarWithClock.SelectedTime = SelectedTime;
            calendarWithClock.SelectedDate = SelectedDate;
        }
    }

    private void OnConfirmButton(object sender, RoutedEventArgs e)
    {
        FlyoutBase.GetAttachedFlyout(rootGrid).Hide();
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "PointerOver", true);
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Pressed", true);
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Normal", true);
        FlyoutBase flyout = FlyoutBase.GetAttachedFlyout(rootGrid);

        if (flyout != null)
        {
            flyout.Closed -= Flyout_Closed;
            flyout.Closed += Flyout_Closed;
            FlyoutBase.ShowAttachedFlyout(rootGrid);
        }
    }

    private void Flyout_Closed(object sender, object e)
    {
        UpdatePlaceholder();
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, "Normal", true);
    }

    private void UpdateSelectedTime()
    {
        calendarWithClock.SelectedTime = SelectedTime;
        UpdatePlaceholder();
    }

    private void UpdateSelectedDate()
    {
        calendarWithClock.SelectedDate = SelectedDate;
        UpdatePlaceholder();
    }

    private void UpdatePlaceholder()
    {
        PlaceholderText = $"{calendarWithClock.SelectedDateTime}";
    }

    private void UpdateHeaderVisibility()
    {
        if (headerContentPresenter != null)
        {
            headerContentPresenter.Visibility = Header == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }

    private void UpdateDescriptionVisibility()
    {
        if (descriptionContentPresenter != null)
        {
            descriptionContentPresenter.Visibility = Description == null
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
