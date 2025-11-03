using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_DismissButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_AcceptButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(PART_FlyoutSecondPickerSpacing), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_FlyoutFirstPickerSpacing), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_FlyoutThirdColumnHost), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_FlyoutSecondColumnHost), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_FlyoutFirstColumnHost), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_FlyoutThirdPickerHost), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_FlyoutSecondPickerHost), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_FlyoutFirstPickerHost), Type = typeof(Border))]
internal partial class LoopingListFlyoutPresenter : FlyoutPresenter
{
    private const string PART_FlyoutFirstPickerHost = "PART_FlyoutFirstPickerHost";
    private const string PART_FlyoutSecondPickerHost = "PART_FlyoutSecondPickerHost";
    private const string PART_FlyoutThirdPickerHost = "PART_FlyoutThirdPickerHost";
    private const string PART_FlyoutFirstColumnHost = "PART_FlyoutFirstColumnHost";
    private const string PART_FlyoutSecondColumnHost = "PART_FlyoutSecondColumnHost";
    private const string PART_FlyoutThirdColumnHost = "PART_FlyoutThirdColumnHost";
    private const string PART_FlyoutFirstPickerSpacing = "PART_FlyoutFirstPickerSpacing";
    private const string PART_FlyoutSecondPickerSpacing = "PART_FlyoutSecondPickerSpacing";
    private const string PART_AcceptButton = "PART_AcceptButton";
    private const string PART_DismissButton = "PART_DismissButton";

    public event EventHandler<LoopingListEventArgs> Accepted;
    public event EventHandler<RoutedEventArgs> Dismissed;

    private ColumnDefinition gridFirstColumn;
    private ColumnDefinition gridSecondColumn;
    private ColumnDefinition gridThirdColumn;

    private Border primaryBorderHost;
    private Border secondaryBorderHost;
    private Border tertiaryBorderHost;

    private Button acceptButton;
    private Button dismissButton;

    private LoopingSelector primarySelector;
    private LoopingSelector secondarySelector;
    private LoopingSelector tertiarySelector;

    private Rectangle firstPickerSpacing;
    private Rectangle secondPickerSpacing;

    public LoopingListFlyoutPresenter(LoopingSelector primarySelector, LoopingSelector secondarySelector, LoopingSelector tertiarySelector)
    {
        this.primarySelector = primarySelector;
        this.secondarySelector = secondarySelector;
        this.tertiarySelector = tertiarySelector;
    }
    protected override void OnApplyTemplate()
    {
        primaryBorderHost = GetTemplateChild(PART_FlyoutFirstPickerHost) as Border;
        secondaryBorderHost = GetTemplateChild(PART_FlyoutSecondPickerHost) as Border;
        tertiaryBorderHost = GetTemplateChild(PART_FlyoutThirdPickerHost) as Border;

        gridFirstColumn = GetTemplateChild(PART_FlyoutFirstColumnHost) as ColumnDefinition;
        gridSecondColumn = GetTemplateChild(PART_FlyoutSecondColumnHost) as ColumnDefinition;
        gridThirdColumn = GetTemplateChild(PART_FlyoutThirdColumnHost) as ColumnDefinition;

        firstPickerSpacing = GetTemplateChild(PART_FlyoutFirstPickerSpacing) as Rectangle;
        secondPickerSpacing = GetTemplateChild(PART_FlyoutSecondPickerSpacing) as Rectangle;

        acceptButton = GetTemplateChild(PART_AcceptButton) as Button;
        dismissButton = GetTemplateChild(PART_DismissButton) as Button;

        acceptButton.Click -= OnAcceptButton;
        acceptButton.Click += OnAcceptButton;

        dismissButton.Click -= OnDismissButton;
        dismissButton.Click += OnDismissButton;

        primaryBorderHost.Child = primarySelector;
        secondaryBorderHost.Child = secondarySelector;
        tertiaryBorderHost.Child = tertiarySelector;
    }
    public bool CanHandleColumnVisibility()
    {
        if (gridSecondColumn == null || gridThirdColumn == null || secondaryBorderHost == null || tertiaryBorderHost == null || firstPickerSpacing == null || secondPickerSpacing == null)
            return false;

        return true;
    }
    public void ShowSecondaryColumnsOnly()
    {
        gridSecondColumn.Width = new GridLength(1, GridUnitType.Auto);
        gridThirdColumn.Width = new GridLength(1, GridUnitType.Auto);

        firstPickerSpacing.Visibility = Visibility.Collapsed;
        secondaryBorderHost.Visibility = Visibility.Collapsed;
        secondPickerSpacing.Visibility = Visibility.Collapsed;
        tertiaryBorderHost.Visibility = Visibility.Collapsed;
    }
    public void ShowSecondaryAndTertiaryColumns()
    {
        gridSecondColumn.Width = new GridLength(1, GridUnitType.Star);
        gridThirdColumn.Width = new GridLength(1, GridUnitType.Auto);
        firstPickerSpacing.Visibility = Visibility.Visible;
        secondaryBorderHost.Visibility = Visibility.Visible;
        secondPickerSpacing.Visibility = Visibility.Collapsed;
        tertiaryBorderHost.Visibility = Visibility.Collapsed;
    }
    public void ShowAllColumns()
    {
        gridSecondColumn.Width = new GridLength(1, GridUnitType.Star);
        gridThirdColumn.Width = new GridLength(1, GridUnitType.Star);
        firstPickerSpacing.Visibility = Visibility.Visible;
        secondaryBorderHost.Visibility = Visibility.Visible;
        secondPickerSpacing.Visibility = Visibility.Visible;
        tertiaryBorderHost.Visibility = Visibility.Visible;
    }

    private void OnDismissButton(object sender, RoutedEventArgs e)
    {
        Dismissed?.Invoke(this, null);
    }

    private void OnAcceptButton(object sender, RoutedEventArgs e)
    {
        var primaryInfo = new LoopingListInfo()
        {
            LoopingSelectorHasValue = primarySelector != null,
            SelectedItem = primarySelector?.SelectedItem?.ToString(),
        };
        var secondaryInfo = new LoopingListInfo()
        {
            LoopingSelectorHasValue = secondarySelector != null,
            SelectedItem = secondarySelector?.SelectedItem?.ToString(),
        };
        var tertiaryInfo = new LoopingListInfo()
        {
            LoopingSelectorHasValue = tertiarySelector != null,
            SelectedItem = tertiarySelector?.SelectedItem?.ToString(),
        };

        var args = new LoopingListEventArgs(primaryInfo, secondaryInfo, tertiaryInfo);
        Accepted?.Invoke(this, args);
    }
}
