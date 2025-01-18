using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Items))]
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(StepBarItem))]
[TemplatePart(Name = ElementProgressBar, Type = typeof(ProgressBar))]
public partial class StepBar : ItemsControl
{
    private const string ElementProgressBar = "PART_ProgressBar";
    private const string ElementProgressBarBorder = "PART_ProgressBarBorder";
    private const string ElementRootGridVertical = "PART_RootGridVertical";

    private ProgressBar progressBar;
    private Border progressBarBorder;
    private Grid rootGridVertical;
    private int ItemsCount => Items.Count;
    private ControlTemplate? HorizontalTemplate { get; set; }
    private ControlTemplate? VerticalTemplate { get; set; }
    private Style? HorizontalItemContainerStyle { get; set; }
    private Style? VerticalItemContainerStyle { get; set; }
    private ItemsPanelTemplate HorizontalItemsPanelTemplate { get; set; }
    private ItemsPanelTemplate VerticalItemsPanelTemplate { get; set; }
    public event EventHandler<int> StepChanged;
    private int _oriStepIndex = -1;
    private void UpdateTemplate()
    {
        Template = Orientation switch
        {
            Orientation.Horizontal => HorizontalTemplate,
            Orientation.Vertical => VerticalTemplate,
            _ => Template
        };

        ItemContainerStyle = Orientation switch
        {
            Orientation.Horizontal => HorizontalItemContainerStyle,
            Orientation.Vertical => VerticalItemContainerStyle,
            _ => ItemContainerStyle
        };
    }
    public StepBar()
    {
        this.DefaultStyleKey = typeof(StepBar);

        if (Application.Current.Resources["StepBarHorizontalControlTemplate"] is ControlTemplate horizontalTemplate)
            this.HorizontalTemplate = horizontalTemplate;

        if (Application.Current.Resources["StepBarVerticalControlTemplate"] is ControlTemplate verticalTemplate)
            this.VerticalTemplate = verticalTemplate;

        if (Application.Current.Resources["StepBarItemHorizontalStyle"] is Style horizontalItemContainerStyle)
            this.HorizontalItemContainerStyle = horizontalItemContainerStyle;

        if (Application.Current.Resources["StepBarItemVerticalStyle"] is Style verticalItemContainerStyle)
            this.VerticalItemContainerStyle = verticalItemContainerStyle;
        
        if (Application.Current.Resources["StepBarHorizontalItemsPanelTemplate"] is ItemsPanelTemplate horizontalItemsPanelTemplate)
            this.HorizontalItemsPanelTemplate = horizontalItemsPanelTemplate;

        if (Application.Current.Resources["StepBarVerticalItemsPanelTemplate"] is ItemsPanelTemplate verticalItemsPanelTemplate)
            this.VerticalItemsPanelTemplate = verticalItemsPanelTemplate;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        progressBar = GetTemplateChild(ElementProgressBar) as ProgressBar;
        progressBarBorder = GetTemplateChild(ElementProgressBarBorder) as Border;
        rootGridVertical = GetTemplateChild(ElementRootGridVertical) as Grid;

        SetProgressBarMaximumValue();
        UpdateItemsPanel();
        UpdateHeaderDisplayMode();
        UpdateProgressBarVisualStates();
        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateItems();
    }
    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        UpdateItems();
    }

    private void UpdateItemsPanel()
    {
        UpdateTemplate();

        if (Orientation == Orientation.Horizontal)
        {
            ItemsPanel = HorizontalItemsPanelTemplate;
        }
        else
        {
            ItemsPanel = VerticalItemsPanelTemplate;
        }
    }

    private void UpdateHeaderDisplayMode()
    {
        if (Orientation == Orientation.Horizontal)
        {
            if (progressBar == null)
            {
                return;
            }
            switch (HeaderDisplayMode)
            {
                case StepBarHeaderDisplayMode.Top:
                    progressBar.VerticalAlignment = VerticalAlignment.Bottom;
                    progressBar.HorizontalAlignment = HorizontalAlignment.Center;
                    progressBar.Margin = new Thickness(0, 0, 0, 20);
                    break;
                case StepBarHeaderDisplayMode.Left:
                case StepBarHeaderDisplayMode.Right:
                case StepBarHeaderDisplayMode.Bottom:
                    progressBar.VerticalAlignment = VerticalAlignment.Top;
                    progressBar.HorizontalAlignment = HorizontalAlignment.Center;
                    progressBar.Margin = new Thickness(0, 20, 0, 0);
                    break;
            }
        }
        else
        {
            if (rootGridVertical == null)
            {
                return;
            }
            switch (HeaderDisplayMode)
            {
                case StepBarHeaderDisplayMode.Top:
                case StepBarHeaderDisplayMode.Bottom:
                case StepBarHeaderDisplayMode.Right:
                    rootGridVertical.FlowDirection = FlowDirection.LeftToRight;
                    break;
                case StepBarHeaderDisplayMode.Left:
                    rootGridVertical.FlowDirection = FlowDirection.RightToLeft;
                    break;
            }
        }
        UpdateItems();
    }

    private void SetProgressBarMaximumValue()
    {
        if (progressBar != null)
        {
            progressBar.Maximum = ItemsCount - 1;
        }
    }

    private void UpdateProgressBarSize()
    {
        int colOrRowCount = ItemsCount;

        if (Orientation == Orientation.Horizontal)
        {
            if (progressBar == null || colOrRowCount <= 0)
            {
                return;
            }
            progressBar.Width = (colOrRowCount - 1) * (ActualWidth / colOrRowCount);
        }
        else
        {
            if (progressBarBorder == null || colOrRowCount <= 0)
            {
                return;
            }
            progressBarBorder.Height = (colOrRowCount - 1) * (ActualHeight / colOrRowCount);
        }
    }

    private void UpdateItems()
    {
        int count = ItemsCount;
        if (count <= 0)
        {
            return;
        }

        SetProgressBarMaximumValue();
        SetProgressBarValueWithAnimation(StepIndex);

        for (int i = 0; i < count; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is StepBarItem stepBarItem)
            {
                stepBarItem.Index = i + 1;
                stepBarItem.Orientation = this.Orientation;
                stepBarItem.HeaderDisplayMode = this.HeaderDisplayMode;
                stepBarItem.ItemTemplate = ItemTemplate;
                stepBarItem.ShowStepIndex = ShowStepIndex;
            }
        }

        if (_oriStepIndex > 0)
        {
            StepIndex = _oriStepIndex;
            _oriStepIndex = -1;
        }
        else
        {
            OnStepIndexChanged(StepIndex);
        }

        UpdateProgressBarSize();
    }

    private void OnStepIndexChanged(int stepIndex)
    {
        if (progressBar == null)
        {
            return;
        }

        if (stepIndex < 0)
        {
            SetProgressBarValueWithAnimation(0);
            return;
        }

        for (int i = 0; i < stepIndex; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is StepBarItem stepItemFinished)
            {
                UpdateProgressBarVisualStates();
                stepItemFinished.ProgressState = StepProgressState.Complete;
                stepItemFinished.Status = Status;
            }
        }

        for (int i = stepIndex + 1; i < ItemsCount; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is StepBarItem stepItemFinished)
            {
                UpdateProgressBarVisualStates();
                stepItemFinished.ProgressState = StepProgressState.Waiting;
                stepItemFinished.Status = Status;
            }
        }

        if (ItemContainerGenerator.ContainerFromIndex(stepIndex) is StepBarItem stepItemSelected)
        {
            UpdateProgressBarVisualStates();
            stepItemSelected.ProgressState = StepProgressState.UnderWay;
            stepItemSelected.Status = Status;
        }
        SetProgressBarValueWithAnimation(StepIndex);
        StepChanged?.Invoke(this, StepIndex);
    }

    private void SetProgressBarValueWithAnimation(double toValue, int duration = 200)
    {
        if (progressBar == null)
        {
            return;
        }
        var horizontalAnimation = new DoubleAnimation
        {
            From = progressBar.Value,
            To = toValue,
            Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
            EnableDependentAnimation = true
        };

        Storyboard.SetTarget(horizontalAnimation, progressBar);
        Storyboard.SetTargetProperty(horizontalAnimation, nameof(ProgressBar.Value));

        var horizontalStoryboard = new Storyboard();
        horizontalStoryboard.Children.Add(horizontalAnimation);
        horizontalStoryboard.Begin();
    }
    
    private void UpdateProgressBarVisualStates()
    {
        VisualStateManager.GoToState(this, Status.ToString(), true);
    }

    public void Next() => StepIndex++;
    public void Prev() => StepIndex--;
}
