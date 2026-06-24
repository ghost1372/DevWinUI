using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = HeaderContentPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = DescriptionPresenter, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_Root, Type = typeof(Grid))]
[TemplatePart(Name = PART_ConfirmButton, Type = typeof(Button))]
[TemplatePart(Name = PART_ClockView, Type = typeof(Clock))]
public partial class ClockPicker : Control
{
    private const string HeaderContentPresenter = "HeaderContentPresenter";
    private const string DescriptionPresenter = "DescriptionPresenter";
    private const string PART_Root = "PART_Root";
    private const string PART_ConfirmButton = "PART_ConfirmButton";
    private const string PART_ClockView = "PART_ClockView";

    public event EventHandler<RoutedEventArgs> ConfirmClick;
    public event EventHandler<TimeSpan> SelectedTimeChanged;

    private ContentPresenter headerContentPresenter;
    private ContentPresenter descriptionContentPresenter;
    private Grid rootGrid;
    private Button confirmButton;
    internal Clock clock;
    private bool isUpdating;

    public ClockPicker()
    {
        this.DefaultStyleKey = typeof(ClockPicker);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        headerContentPresenter = GetTemplateChild(nameof(HeaderContentPresenter)) as ContentPresenter;
        descriptionContentPresenter = GetTemplateChild(nameof(DescriptionPresenter)) as ContentPresenter;

        UpdateHeaderVisibility();
        UpdateDescriptionVisibility();

        rootGrid = GetTemplateChild(PART_Root) as Grid;
        confirmButton = GetTemplateChild(PART_ConfirmButton) as Button;
        clock = GetTemplateChild(PART_ClockView) as Clock;

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

        if (clock != null)
        {
            clock.SelectedTimeChanged -= Clock_SelectedTimeChanged;
            clock.SelectedTimeChanged += Clock_SelectedTimeChanged;

            clock.TimeFormat = TimeFormat;
            UpdateClockTime();
        }

        UpdateTemplate();
    }

    private void Clock_SelectedTimeChanged(object sender, DateTime e)
    {
        if (!isUpdating && clock != null)
        {
            try
            {
                isUpdating = true;
                var timeSpan = new TimeSpan(e.Hour, e.Minute, e.Second);
                SelectedTime = timeSpan;
                SelectedTimeOnly = TimeOnly.FromTimeSpan(timeSpan);
                SelectedTimeChanged?.Invoke(this, timeSpan);
                UpdatePlaceholder();
            }
            finally
            {
                isUpdating = false;
            }
        }
    }

    private void OnConfirmButton(object sender, RoutedEventArgs e)
    {
        ConfirmClick?.Invoke(this, e);
        if (rootGrid != null)
        {
            FlyoutBase.GetAttachedFlyout(rootGrid).Hide();
        }
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

    private void OnSelectedTimeChanged()
    {
        if (isUpdating)
            return;

        try
        {
            isUpdating = true;
            if (SelectedTime.HasValue)
            {
                SelectedTimeOnly = TimeOnly.FromTimeSpan(SelectedTime.Value);
            }
            UpdateClockTime();
            UpdatePlaceholder();
        }
        finally
        {
            isUpdating = false;
        }
    }

    private void OnSelectedTimeOnlyChanged()
    {
        if (isUpdating)
            return;

        try
        {
            isUpdating = true;
            if (SelectedTimeOnly.HasValue)
            {
                SelectedTime = SelectedTimeOnly.Value.ToTimeSpan();
            }
            UpdateClockTime();
            UpdatePlaceholder();
        }
        finally
        {
            isUpdating = false;
        }
    }

    private void UpdateClockTime()
    {
        if (clock != null && SelectedTime.HasValue)
        {
            var now = DateTime.Now;
            clock.SelectedTime = new DateTime(
                now.Year, now.Month, now.Day,
                SelectedTime.Value.Hours, SelectedTime.Value.Minutes, SelectedTime.Value.Seconds);
        }
    }

    private void UpdatePlaceholder()
    {
        if (SelectedTime.HasValue)
        {
            PlaceholderText = SelectedTime.Value.ToString(TimeFormat);
        }
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

    private void UpdateTemplate()
    {
        if (clock != null)
        {
            if (ShowConfirmButton)
            {
                FlyoutBorderThickness = new Thickness(0);
                FlyoutCornerRadius = new CornerRadius(0);
                clock.ClockCornerRadius = new CornerRadius(4, 4, 0, 0);
            }
            else
            {
                FlyoutBorderThickness = new Thickness(1);
                FlyoutCornerRadius = new CornerRadius(4);
                clock.ClockCornerRadius = new CornerRadius(4);
            }
        }
    }

    public Clock GetClock()
    {
        return clock;
    }
}
