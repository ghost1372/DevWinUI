using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_ThirdPickerHost), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_SecondPickerHost), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_SecondColumnDivider), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_FirstColumnDivider), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_ThirdTextBlockColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_SecondTextBlockColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_FirstTextBlockColumn), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_ThirdTextBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_SecondTextBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_FirstTextBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_HeaderContentPresenter), Type = typeof(ContentPresenter))]
public partial class LoopingList : Control
{
    private const string PART_FlyoutButton = "PART_FlyoutButton";
    private const string PART_ThirdPickerHost = "PART_ThirdPickerHost";
    private const string PART_SecondPickerHost = "PART_SecondPickerHost";
    private const string PART_SecondColumnDivider = "PART_SecondColumnDivider";
    private const string PART_FirstColumnDivider = "PART_FirstColumnDivider";
    private const string PART_ThirdTextBlockColumn = "PART_ThirdTextBlockColumn";
    private const string PART_SecondTextBlockColumn = "PART_SecondTextBlockColumn";
    private const string PART_FirstTextBlockColumn = "PART_FirstTextBlockColumn";
    private const string PART_ThirdTextBlock = "PART_ThirdTextBlock";
    private const string PART_SecondTextBlock = "PART_SecondTextBlock";
    private const string PART_FirstTextBlock = "PART_FirstTextBlock";
    private const string PART_HeaderContentPresenter = "PART_HeaderContentPresenter";

    private LoopingListFlyout flyout;

    private ColumnDefinition gridFirstColumn;
    private ColumnDefinition gridSecondColumn;
    private ColumnDefinition gridThirdColumn;

    private TextBlock primaryTextBlock;
    private TextBlock secondaryTextBlock;
    private TextBlock tertiaryTextBlock;

    private Rectangle firstColumnDivider;
    private Rectangle secondColumnDivider;

    private Border secondPickerHost;
    private Border thirdPickerHost;

    private ContentPresenter headerContentPresenter;

    private LoopingSelector primarySelector = null;
    private LoopingSelector secondarySelector = null;
    private LoopingSelector tertiarySelector = null;

    public event EventHandler<LoopingListEventArgs> Accepted;
    public event EventHandler<RoutedEventArgs> Dismissed;

    private bool HasValue;
    public LoopingList()
    {
        DefaultStyleKey = typeof(LoopingList);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        primaryTextBlock = GetTemplateChild(PART_FirstTextBlock) as TextBlock;
        secondaryTextBlock = GetTemplateChild(PART_SecondTextBlock) as TextBlock;
        tertiaryTextBlock = GetTemplateChild(PART_ThirdTextBlock) as TextBlock;
        gridFirstColumn = GetTemplateChild(PART_FirstTextBlockColumn) as ColumnDefinition;
        gridSecondColumn = GetTemplateChild(PART_SecondTextBlockColumn) as ColumnDefinition;
        gridThirdColumn = GetTemplateChild(PART_ThirdTextBlockColumn) as ColumnDefinition;
        firstColumnDivider = GetTemplateChild(PART_FirstColumnDivider) as Rectangle;
        secondColumnDivider = GetTemplateChild(PART_SecondColumnDivider) as Rectangle;
        secondPickerHost = GetTemplateChild(PART_SecondPickerHost) as Border;
        thirdPickerHost = GetTemplateChild(PART_ThirdPickerHost) as Border;
        headerContentPresenter = GetTemplateChild(PART_HeaderContentPresenter) as ContentPresenter;

        var btnFlyout = GetTemplateChild(PART_FlyoutButton) as Button;
        
        var selectors = CreateLoopingSelectors();

        flyout = new LoopingListFlyout(selectors.primary, selectors.secondary, selectors.tertiary);

        flyout.Accepted -= OnAcceptButton;
        flyout.Accepted += OnAcceptButton;

        flyout.Dismissed -= OnDismissButton;
        flyout.Dismissed += OnDismissButton;

        flyout.Opened -= OnFlyoutOpened;
        flyout.Opened += OnFlyoutOpened;

        UpdateColumnsVisibility();
        UpdatePlaceholderText();
        UpdateSelectedItem();
        UpdateShouldLoop();
        UpdateHeader();

        btnFlyout.Click -= OnFlyoutButtonClicked;
        btnFlyout.Click += OnFlyoutButtonClicked;
    }

    private void UpdateHeader()
    {
        if (headerContentPresenter != null)
        {
            headerContentPresenter.Visibility = Header != null ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public void ClearSelection()
    {
        if (primaryTextBlock != null)
        {
            primaryTextBlock.Text = string.Empty;
            PrimarySelectedIndex = -1;
            PrimarySelectedItem = null;
        }

        if (secondaryTextBlock != null)
        {
            secondaryTextBlock.Text = string.Empty;
            SecondarySelectedIndex = -1;
            SecondarySelectedItem = null;
        }

        if (tertiaryTextBlock != null)
        {
            tertiaryTextBlock.Text = string.Empty;
            TertiarySelectedIndex = -1;
            TertiarySelectedItem = null;
        }

        HasValue = false;
        UpdatePlaceholderText();
    }
    private void UpdateShouldLoop()
    {
        if (primarySelector != null)
        {
            primarySelector.ShouldLoop = ShouldLoop;
        }

        if (secondarySelector != null)
        {
            secondarySelector.ShouldLoop = ShouldLoop;
        }

        if (tertiarySelector != null)
        {
            tertiarySelector.ShouldLoop = ShouldLoop;
        }
    }
    private void OnFlyoutButtonClicked(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        flyout.ShowAt(button);
    }

    private void OnFlyoutOpened(object? sender, object e)
    {
        UpdateFlyoutColumnsVisibility();
    }
    
    private (LoopingSelector primary, LoopingSelector secondary, LoopingSelector tertiary) CreateLoopingSelectors()
    {
        if (PrimaryItems != null && PrimaryItems.Count > 0 && primarySelector == null)
        {
            primarySelector = new LoopingSelector
            {
                ItemHeight = 40,
                Padding = new Thickness(0, 3, 0, 6),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Items = PrimaryItems.Select(x => new LoopingSelectorItem { PrimaryText = x } as object).ToList()
            };

            void PrimaySelectorChanged(object sender, SelectionChangedEventArgs args)
            {
                PrimarySelectedIndex = primarySelector.SelectedIndex;
                PrimarySelectedItem = primarySelector.SelectedItem?.ToString();
            }
            primarySelector.SelectionChanged -= PrimaySelectorChanged;
            primarySelector.SelectionChanged += PrimaySelectorChanged;

            primarySelector.SelectedIndex = PrimarySelectedIndex;
            primarySelector.SelectedItem = PrimarySelectedItem;
        }

        if (SecondaryItems != null && SecondaryItems.Count > 0 && secondarySelector == null)
        {
            secondarySelector = new LoopingSelector
            {
                ItemHeight = 40,
                Padding = new Thickness(0, 3, 0, 6),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Items = SecondaryItems.Select(x => new LoopingSelectorItem { PrimaryText = x } as object).ToList()
            };

            void SecondarySelectorChanged(object sender, SelectionChangedEventArgs args)
            {
                SecondarySelectedIndex = secondarySelector.SelectedIndex;
                SecondarySelectedItem = secondarySelector.SelectedItem?.ToString();
            }
            secondarySelector.SelectionChanged -= SecondarySelectorChanged;
            secondarySelector.SelectionChanged += SecondarySelectorChanged;

            secondarySelector.SelectedIndex = SecondarySelectedIndex;
            secondarySelector.SelectedItem = SecondarySelectedItem;
        }

        if (TertiaryItems != null && TertiaryItems.Count > 0 && tertiarySelector == null)
        {
            tertiarySelector = new LoopingSelector
            {
                ItemHeight = 40,
                Padding = new Thickness(0, 3, 0, 6),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Items = TertiaryItems.Select(x => new LoopingSelectorItem { PrimaryText = x } as object).ToList()
            };

            void TertiarySelectorChanged(object sender, SelectionChangedEventArgs args)
            {
                TertiarySelectedIndex = tertiarySelector.SelectedIndex;
                TertiarySelectedItem = tertiarySelector.SelectedItem?.ToString();
            }
            tertiarySelector.SelectionChanged -= TertiarySelectorChanged;
            tertiarySelector.SelectionChanged += TertiarySelectorChanged;

            tertiarySelector.SelectedIndex = TertiarySelectedIndex;
            tertiarySelector.SelectedItem = TertiarySelectedItem;
        }

        return (primary: primarySelector, secondary: secondarySelector, tertiary: tertiarySelector);
    }
    private void UpdateColumnsVisibility()
    {
        if (gridSecondColumn == null || gridThirdColumn == null || firstColumnDivider == null || secondColumnDivider == null || secondPickerHost == null || thirdPickerHost == null)
            return;

        if (SecondaryItems == null || SecondaryItems?.Count == 0)
        {
            gridSecondColumn.Width = new GridLength(1, GridUnitType.Auto);
            gridThirdColumn.Width = new GridLength(1, GridUnitType.Auto);
            firstColumnDivider.Visibility = Visibility.Collapsed;
            secondPickerHost.Visibility = Visibility.Collapsed;
            secondColumnDivider.Visibility = Visibility.Collapsed;
            thirdPickerHost.Visibility = Visibility.Collapsed;

        }
        else if ((SecondaryItems != null && SecondaryItems.Count > 0) && (TertiaryItems == null || TertiaryItems?.Count == 0))
        {
            gridSecondColumn.Width = new GridLength(1, GridUnitType.Star);
            gridThirdColumn.Width = new GridLength(1, GridUnitType.Auto);
            firstColumnDivider.Visibility = Visibility.Visible;
            secondPickerHost.Visibility = Visibility.Visible;
            secondColumnDivider.Visibility = Visibility.Collapsed;
            thirdPickerHost.Visibility = Visibility.Collapsed;
        }
        else if ((SecondaryItems != null && SecondaryItems.Count > 0) && (TertiaryItems != null && TertiaryItems.Count > 0))
        {
            gridSecondColumn.Width = new GridLength(1, GridUnitType.Star);
            gridThirdColumn.Width = new GridLength(1, GridUnitType.Star);
            firstColumnDivider.Visibility = Visibility.Visible;
            secondPickerHost.Visibility = Visibility.Visible;
            secondColumnDivider.Visibility = Visibility.Visible;
            thirdPickerHost.Visibility = Visibility.Visible;
        }
    }
    private void UpdateFlyoutColumnsVisibility()
    {
        if (flyout.Presenter == null)
            return;

        if (flyout.Presenter.CanHandleColumnVisibility())
        {
            if (SecondaryItems == null || SecondaryItems?.Count == 0)
            {
                flyout.Presenter.ShowSecondaryColumnsOnly();
            }
            else if ((SecondaryItems != null && SecondaryItems.Count > 0) && (TertiaryItems == null || TertiaryItems?.Count == 0))
            {
                flyout.Presenter.ShowSecondaryAndTertiaryColumns();
            }
            else if ((SecondaryItems != null && SecondaryItems.Count > 0) && (TertiaryItems != null && TertiaryItems.Count > 0))
            {
                flyout.Presenter.ShowAllColumns();
            }
        }
    }
    private void UpdateSelectedItem()
    {
        if (primaryTextBlock == null || secondaryTextBlock == null || tertiaryTextBlock == null)
            return;

        if (PrimaryItems != null && PrimaryItems.Count > 0)
        {
            if (PrimarySelectedIndex != -1)
            {
                primaryTextBlock.Text = PrimaryItems[PrimarySelectedIndex];
            }
            else if (!string.IsNullOrEmpty(PrimarySelectedItem) && primarySelector != null)
            {
                var item = PrimaryItems.Where(x => x.ToLower().Equals(PrimarySelectedItem.ToLower())).FirstOrDefault();
                primaryTextBlock.Text = item;

                primarySelector.SelectedIndex = PrimaryItems.IndexOf(item);
            }
            else
            {
                primaryTextBlock.Text = PrimaryPlaceholderText;
                VisualStateManager.GoToState(this, "HasNoValue", true);
            }
        }

        if (SecondaryItems != null && SecondaryItems.Count > 0)
        {
            if (SecondarySelectedIndex != -1)
            {
                secondaryTextBlock.Text = SecondaryItems[SecondarySelectedIndex];
            }
            else if (!string.IsNullOrEmpty(SecondarySelectedItem) && secondarySelector != null)
            {
                var item = SecondaryItems.Where(x => x.ToLower().Equals(SecondarySelectedItem.ToLower())).FirstOrDefault();
                secondaryTextBlock.Text = item;
                secondarySelector.SelectedIndex = SecondaryItems.IndexOf(item);
            }
            else
            {
                secondaryTextBlock.Text = SecondaryPlaceholderText;
                VisualStateManager.GoToState(this, "HasNoValue", true);
            }
        }

        if (TertiaryItems != null && TertiaryItems.Count > 0)
        {
            if (TertiarySelectedIndex != -1)
            {
                tertiaryTextBlock.Text = TertiaryItems[TertiarySelectedIndex];
            }
            else if (!string.IsNullOrEmpty(TertiarySelectedItem) && tertiarySelector != null)
            {
                var item = TertiaryItems.Where(x => x.ToLower().Equals(TertiarySelectedItem.ToLower())).FirstOrDefault();
                tertiaryTextBlock.Text = item;
                tertiarySelector.SelectedIndex = TertiaryItems.IndexOf(item);
            }
            else
            {
                tertiaryTextBlock.Text = TertiaryPlaceholderText;
                VisualStateManager.GoToState(this, "HasNoValue", true);
            }
        }
    }

    private void OnDismissButton(object sender, RoutedEventArgs e)
    {
        Dismissed?.Invoke(this, e);
        flyout.Hide();
    }

    private void OnAcceptButton(object sender, LoopingListEventArgs e)
    {
        Accepted?.Invoke(this, e);

        if (e.PrimaryInfo.LoopingSelectorHasValue)
        {
            primaryTextBlock.Text = e.PrimaryInfo.SelectedItem;
        }

        if (e.SecondaryInfo.LoopingSelectorHasValue)
        {
            secondaryTextBlock.Text = e.SecondaryInfo.SelectedItem;
        }

        if (e.TertiaryInfo.LoopingSelectorHasValue)
        {
            tertiaryTextBlock.Text = e.TertiaryInfo.SelectedItem;
        }

        HasValue = e.PrimaryInfo.SelectedItemHasValue || 
            e.SecondaryInfo.SelectedItemHasValue || 
            e.TertiaryInfo.SelectedItemHasValue;

        VisualStateManager.GoToState(this, HasValue ? "HasValue" : "HasNoValue", true);

        flyout.Hide();
    }

    private void UpdatePlaceholderText()
    {
        if (primaryTextBlock != null && secondaryTextBlock != null && tertiaryTextBlock != null && !HasValue)
        {
            primaryTextBlock.Text = PrimaryPlaceholderText;
            secondaryTextBlock.Text = SecondaryPlaceholderText;
            tertiaryTextBlock.Text = TertiaryPlaceholderText;

            VisualStateManager.GoToState(this, "HasNoValue", true);
        }
    }
}
