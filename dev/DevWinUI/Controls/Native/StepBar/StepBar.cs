using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Items))]
[StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(StepBarItem))]
[TemplatePart(Name = ElementProgressBar, Type = typeof(ProgressBar))]
[TemplatePart(Name = ElementProgressBarBorder, Type = typeof(Border))]
[TemplatePart(Name = ElementRootGridVertical, Type = typeof(Grid))]
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
    public event EventHandler<StepBarItemClickEventArgs>? ItemClick;

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ItemsPanelTemplate))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ControlTemplate))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Style))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(StepBar))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(StepBarItem))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ProgressBar))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Border))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Grid))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LayoutTransformer))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UniformGrid))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ContentPresenter))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(StackPanel))]
    public StepBar()
    {
        DefaultStyleKey = typeof(StepBar);

        HorizontalTemplate = GetResource<ControlTemplate>("StepBarHorizontalControlTemplate");
        VerticalTemplate = GetResource<ControlTemplate>("StepBarVerticalControlTemplate");
        HorizontalItemContainerStyle = GetResource<Style>("StepBarItemHorizontalStyle");
        VerticalItemContainerStyle = GetResource<Style>("StepBarItemVerticalStyle");
        HorizontalItemsPanelTemplate = GetResource<ItemsPanelTemplate>("StepBarHorizontalItemsPanelTemplate");
        VerticalItemsPanelTemplate = GetResource<ItemsPanelTemplate>("StepBarVerticalItemsPanelTemplate");

        SizeChanged += OnSizeChanged;
    }

    private static T? GetResource<T>(string key) where T : class
    {
        return Application.Current.Resources.TryGetValue(key, out var value) ? value as T : null;
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
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is StepBarItem stepBarItem)
        {
            int index = ItemContainerGenerator.IndexFromContainer(stepBarItem);
            if (index < 0)
            {
                index = Items.IndexOf(item);
            }

            ConfigureItem(stepBarItem, index);
            ApplyProgressState(stepBarItem, index, ItemsCount, StepIndex);
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateProgressBarSize();
    }

    protected override void OnItemsChanged(object e)
    {
        base.OnItemsChanged(e);
        UpdateItems();
    }

    private void UpdateTemplate()
    {
        bool isHorizontal = Orientation == Orientation.Horizontal;

        Template = (isHorizontal ? HorizontalTemplate : VerticalTemplate) ?? Template;
        ItemContainerStyle = (isHorizontal ? HorizontalItemContainerStyle : VerticalItemContainerStyle) ?? ItemContainerStyle;
    }

    private void UpdateItemsPanel()
    {
        UpdateTemplate();

        ItemsPanel = (Orientation == Orientation.Horizontal ? HorizontalItemsPanelTemplate : VerticalItemsPanelTemplate) ?? ItemsPanel;
    }

    private void UpdateHeaderDisplayMode()
    {
        if (Orientation == Orientation.Horizontal)
        {
            if (progressBar == null)
            {
                return;
            }

            progressBar.HorizontalAlignment = HorizontalAlignment.Center;

            if (HeaderDisplayMode == StepBarHeaderDisplayMode.Top)
            {
                progressBar.VerticalAlignment = VerticalAlignment.Bottom;
                progressBar.Margin = new Thickness(0, 0, 0, 20);
            }
            else
            {
                progressBar.VerticalAlignment = VerticalAlignment.Top;
                progressBar.Margin = new Thickness(0, 20, 0, 0);
            }
        }
        else
        {
            if (rootGridVertical == null)
            {
                return;
            }

            rootGridVertical.FlowDirection = HeaderDisplayMode == StepBarHeaderDisplayMode.Left
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;
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
        int count = ItemsCount;
        if (count <= 0)
        {
            return;
        }

        if (Orientation == Orientation.Horizontal)
        {
            if (progressBar != null)
            {
                progressBar.Width = (count - 1) * (ActualWidth / count);
            }
        }
        else if (progressBarBorder != null)
        {
            progressBarBorder.Height = (count - 1) * (ActualHeight / count);
        }
    }

    private void ConfigureItem(StepBarItem stepBarItem, int index)
    {
        stepBarItem.Index = index + 1;
        stepBarItem.Orientation = Orientation;
        stepBarItem.HeaderDisplayMode = HeaderDisplayMode;
        stepBarItem.ItemTemplate = ItemTemplate;
        stepBarItem.ShowStepIndex = ShowStepIndex;
        stepBarItem.DisplayMode = DisplayMode;

        if (stepBarItem.WaitingIcon is null && WaitingIcon is not null)
            stepBarItem.WaitingIcon = WaitingIcon;

        if (stepBarItem.UnderWayIcon is null && UnderWayIcon is not null)
            stepBarItem.UnderWayIcon = UnderWayIcon;

        if (stepBarItem.CompleteIcon is null && CompleteIcon is not null)
            stepBarItem.CompleteIcon = CompleteIcon;

        stepBarItem.Tapped -= OnItemTapped;
        stepBarItem.Tapped += OnItemTapped;
    }

    private void ApplyProgressState(StepBarItem item, int index, int count, int stepIndex)
    {
        item.ProgressState = index < stepIndex
            ? StepProgressState.Complete
            : index > stepIndex
                ? StepProgressState.Waiting
                : index == count - 1 ? StepProgressState.Complete : StepProgressState.UnderWay;
        item.Status = Status;
    }

    private void UpdateItems()
    {
        int count = ItemsCount;
        if (count <= 0)
        {
            return;
        }

        SetProgressBarMaximumValue();

        for (int i = 0; i < count; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is StepBarItem stepBarItem)
            {
                ConfigureItem(stepBarItem, i);
            }
        }

        int stepIndex = Math.Clamp(StepIndex, 0, count - 1);
        if (stepIndex != StepIndex)
        {
            StepIndex = stepIndex;
        }
        else
        {
            UpdateStepVisuals(stepIndex);
        }

        UpdateProgressBarSize();
    }

    private void OnStepIndexChanged(int stepIndex)
    {
        int count = ItemsCount;
        if (count > 0)
        {
            int clamped = Math.Clamp(stepIndex, 0, count - 1);
            if (clamped != stepIndex)
            {
                StepIndex = clamped;
                return;
            }
        }

        if (UpdateStepVisuals(stepIndex))
        {
            StepChanged?.Invoke(this, stepIndex);
        }
    }

    private bool UpdateStepVisuals(int stepIndex)
    {
        if (progressBar == null)
        {
            return false;
        }

        if (stepIndex < 0)
        {
            SetProgressBarValueWithAnimation(0);
            return false;
        }

        UpdateProgressBarVisualStates();

        int count = ItemsCount;
        for (int i = 0; i < count; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is StepBarItem item)
            {
                ApplyProgressState(item, i, count, stepIndex);
            }
        }

        SetProgressBarValueWithAnimation(stepIndex);
        return true;
    }

    private void SetProgressBarValueWithAnimation(double toValue, int duration = 200)
    {
        if (progressBar == null)
        {
            return;
        }

        var animation = new DoubleAnimation
        {
            From = progressBar.Value,
            To = toValue,
            Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
            EnableDependentAnimation = true
        };

        Storyboard.SetTarget(animation, progressBar);
        Storyboard.SetTargetProperty(animation, nameof(ProgressBar.Value));

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }

    private void UpdateProgressBarVisualStates()
    {
        VisualStateManager.GoToState(this, Status.ToString(), true);
    }

    private void OnItemTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is StepBarItem stepBarItem)
        {
            int index = ItemContainerGenerator.IndexFromContainer(stepBarItem);

            ItemClick?.Invoke(this, new StepBarItemClickEventArgs(index, stepBarItem));

            if (AutoSelectOnItemClick)
            {
                StepIndex = index;
            }
        }
    }

    public void Next() => StepIndex++;

    public void Prev() => StepIndex--;
}
