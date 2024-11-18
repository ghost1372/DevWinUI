using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.System;

namespace DevWinUI;

public partial class PagerControl : Control
{
    private int lastSelectedPageIndex = -1;
    private int lastNumberOfPagesCount = 0;


    private IList<object> comboBoxEntries;
    private IList<object> numberPanelElements;

    private FrameworkElement? rootGrid;
    private Button? firstPageButton;
    private Button? previousPageButton;
    private Button? nextPageButton;
    private Button? lastPageButton;
    private ComboBox? comboBox;
    private NumberBox? numberBox;
    private ItemsRepeater? numberPanelRepeater;
    private FrameworkElement? selectedPageIndicator;

    public PagerControl()
    {
        comboBoxEntries = new ObservableCollection<object>();
        numberPanelElements = new ObservableCollection<object>();

        var templateSettings = new PagerControlTemplateSettings();
        templateSettings.Pages = comboBoxEntries;
        templateSettings.NumberPanelItems = numberPanelElements;
        TemplateSettings = templateSettings;

        DefaultStyleKey = typeof(PagerControl);

        Loaded += PagerControl_Loaded;
    }

    ~PagerControl()
    {
        try
        {
            if (rootGrid != null)
            {
                rootGrid.KeyDown -= RootGrid_KeyDown;
                rootGrid = null;
            }
            if (firstPageButton != null)
            {
                firstPageButton.Click -= FirstPageButton_Click;
                firstPageButton = null;
            }
            if (previousPageButton != null)
            {
                previousPageButton.Click -= PreviousPageButton_Click;
                previousPageButton = null;
            }
            if (nextPageButton != null)
            {
                nextPageButton.Click -= NextPageButton_Click;
                nextPageButton = null;
            }
            if (lastPageButton != null)
            {
                lastPageButton.Click -= LastPageButton_Click;
                lastPageButton = null;
            }
            if (comboBox != null)
            {
                comboBox.SelectionChanged -= ComboBox_SelectionChanged;
                comboBox = null;
            }
            if (numberBox != null)
            {
                numberBox.ValueChanged -= NumberBox_ValueChanged;
                numberBox = null;
            }

            numberPanelRepeater = null;
            selectedPageIndicator = null;
        }
        catch (Exception)
        {
        }
    }

    protected override void OnApplyTemplate()
    {
        if (string.IsNullOrEmpty(PrefixText))
        {
            PrefixText = "Page";
        }
        if (string.IsNullOrEmpty(SuffixText))
        {
            SuffixText = "of";
        }

        if (GetTemplateChild(c_rootGridName) is FrameworkElement rootGridTemplate)
        {
            rootGrid = rootGridTemplate;
            rootGrid.KeyDown += RootGrid_KeyDown;
        }
        if (GetTemplateChild(c_firstPageButtonName) is Button firstPageButtonTemplate)
        {
            firstPageButton = firstPageButtonTemplate;
            firstPageButton.Click += FirstPageButton_Click;
            AutomationProperties.SetName(firstPageButtonTemplate, "First page");
        }
        if (GetTemplateChild(c_previousPageButtonName) is Button previousPageButtonTemplate)
        {
            previousPageButton = previousPageButtonTemplate;
            previousPageButton.Click += PreviousPageButton_Click;
            AutomationProperties.SetName(previousPageButtonTemplate, "Previous page");

        }
        if (GetTemplateChild(c_nextPageButtonName) is Button nextPageButtonTemplate)
        {
            nextPageButton = nextPageButtonTemplate;
            nextPageButton.Click += NextPageButton_Click;
            AutomationProperties.SetName(nextPageButtonTemplate, "Next page");
        }
        if (GetTemplateChild(c_lastPageButtonName) is Button lastPageButtonTemplate)
        {
            lastPageButton = lastPageButtonTemplate;
            lastPageButton.Click += LastPageButton_Click;
            AutomationProperties.SetName(lastPageButtonTemplate, "Last page");
        }
        if (GetTemplateChild(c_comboBoxName) is ComboBox comboBoxTemplate)
        {
            comboBox = comboBoxTemplate;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            FillComboBoxCollectionToSize(NumberOfPages);
            comboBox.SelectedIndex = SelectedPageIndex - 1;
            AutomationProperties.SetName(comboBox, "Page");
        }
        if (GetTemplateChild(c_numberBoxName) is NumberBox numberBoxTemplate)
        {
            numberBox = numberBoxTemplate;
            numberBox.ValueChanged += NumberBox_ValueChanged;
            numberBox.Value = SelectedPageIndex + 1;
            AutomationProperties.SetName(numberBox, "Page");
        }

        numberPanelRepeater = GetTemplateChild(c_numberPanelRepeaterName) as ItemsRepeater;
        selectedPageIndicator = GetTemplateChild(c_numberPanelIndicatorName) as FrameworkElement;

        OnDisplayModeChanged();
        UpdateOnEdgeButtonVisualStates();
        OnNumberOfPagesChanged(0);

        OnButtonVisibilityChanged(
            FirstButtonVisibility,
            c_firstPageButtonVisibleVisualState,
            c_firstPageButtonNotVisibleVisualState,
            c_firstPageButtonHiddenVisualState,
            0);

        OnButtonVisibilityChanged(
            PreviousButtonVisibility,
            c_previousPageButtonVisibleVisualState,
            c_previousPageButtonNotVisibleVisualState,
            c_previousPageButtonHiddenVisualState,
            0);

        OnButtonVisibilityChanged(
            NextButtonVisibility,
            c_nextPageButtonVisibleVisualState,
            c_nextPageButtonNotVisibleVisualState,
            c_nextPageButtonHiddenVisualState,
            NumberOfPages - 1);

        OnButtonVisibilityChanged(
            LastButtonVisibility,
            c_lastPageButtonVisibleVisualState,
            c_lastPageButtonNotVisibleVisualState,
            c_lastPageButtonHiddenVisualState,
            NumberOfPages - 1);

        OnSelectedPageIndexChange(-1);
    }


    private void OnPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        var property = args.Property;
        if (Template != null)
        {
            if (property == FirstButtonVisibilityProperty)
            {
                OnButtonVisibilityChanged(
                    FirstButtonVisibility,
                    c_firstPageButtonVisibleVisualState,
                    c_firstPageButtonNotVisibleVisualState,
                    c_firstPageButtonHiddenVisualState,
                    0);
            }
            else if (property == PreviousButtonVisibilityProperty)
            {

                OnButtonVisibilityChanged(
                    PreviousButtonVisibility,
                    c_previousPageButtonVisibleVisualState,
                    c_previousPageButtonNotVisibleVisualState,
                    c_previousPageButtonHiddenVisualState,
                    0);
            }
            else if (property == NextButtonVisibilityProperty)
            {

                OnButtonVisibilityChanged(
                    NextButtonVisibility,
                    c_nextPageButtonVisibleVisualState,
                    c_nextPageButtonNotVisibleVisualState,
                    c_nextPageButtonHiddenVisualState,
                    NumberOfPages - 1);
            }
            else if (property == LastButtonVisibilityProperty)
            {

                OnButtonVisibilityChanged(
                    LastButtonVisibility,
                    c_lastPageButtonVisibleVisualState,
                    c_lastPageButtonNotVisibleVisualState,
                    c_lastPageButtonHiddenVisualState,
                    NumberOfPages - 1);
            }

            else if (property == DisplayModeProperty)
            {
                OnDisplayModeChanged();
                // Why are we calling this you might ask.
                // The reason is that that method only updates what it currently needs to update.
                // So when we switch to ComboBox from NumberPanel, the NumberPanel element list might be out of date.
                UpdateTemplateSettingElementLists();
            }
            else if (property == NumberOfPagesProperty)
            {
                OnNumberOfPagesChanged((int)args.OldValue);
            }
            else if (property == SelectedPageIndexProperty)
            {
                OnSelectedPageIndexChange((int)args.OldValue);
            }
            else if (property == ButtonPanelAlwaysShowFirstLastPageIndexProperty)
            {
                UpdateNumberPanel(NumberOfPages);
            }
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new PagerControlAutomationPeer(this);
    }

    /* Property changed handlers */
    private void OnDisplayModeChanged()
    {
        // Cache for performance reasons
        var displayMode = DisplayMode;

        if (displayMode == PagerControlDisplayMode.ButtonPanel)
        {
            VisualStateManager.GoToState(this, c_numberPanelVisibleVisualState, false);
        }
        else if (displayMode == PagerControlDisplayMode.ComboBox)
        {
            VisualStateManager.GoToState(this, c_comboBoxVisibleVisualState, false);
        }
        else if (displayMode == PagerControlDisplayMode.NumberBox)
        {
            VisualStateManager.GoToState(this, c_numberBoxVisibleVisualState, false);
        }
        else
        {
            UpdateDisplayModeAutoState();
        }
    }

    private void UpdateDisplayModeAutoState()
    {
        var numberOfPages = NumberOfPages;
        if (numberOfPages > -1)
        {
            VisualStateManager.GoToState(this, numberOfPages < c_AutoDisplayModeNumberOfPagesThreshold ?
                c_comboBoxVisibleVisualState : c_numberBoxVisibleVisualState, false);
        }
        else
        {
            VisualStateManager.GoToState(this, c_numberBoxVisibleVisualState, false);
        }
    }

    private void OnNumberOfPagesChanged(int oldValue)
    {
        lastNumberOfPagesCount = oldValue;
        var numberOfPages = NumberOfPages;
        if (numberOfPages < SelectedPageIndex && numberOfPages > -1)
        {
            SelectedPageIndex = numberOfPages - 1;
        }
        UpdateTemplateSettingElementLists();
        if (DisplayMode == PagerControlDisplayMode.Auto)
        {
            UpdateDisplayModeAutoState();
        }
        if (numberOfPages > -1)
        {
            VisualStateManager.GoToState(this, c_finiteItemsModeState, false);
            if (numberBox != null)
            {
                numberBox.Maximum = numberOfPages;
            }
        }
        else
        {
            VisualStateManager.GoToState(this, c_infiniteItemsModeState, false);
            if (numberBox != null)
            {
                numberBox.Maximum = double.PositiveInfinity;
            }
        }
        UpdateOnEdgeButtonVisualStates();
    }

    private void OnSelectedPageIndexChange(int oldValue)
    {
        // If we don't have any pages, there is nothing we should do.
        // Ensure that SelectedPageIndex will end up in the valid range of values
        // Special case is NumberOfPages being 0, in that case, don't verify upperbound restrictions
        if (SelectedPageIndex > NumberOfPages - 1 && NumberOfPages > 0)
        {
            SelectedPageIndex = NumberOfPages - 1;
        }
        else if (SelectedPageIndex < 0)
        {
            SelectedPageIndex = 0;
        }
        // Now handle the value changes
        lastSelectedPageIndex = oldValue;

        if (comboBox != null)

        {
            if (SelectedPageIndex < comboBoxEntries.Count)
            {
                comboBox.SelectedIndex = SelectedPageIndex;
            }
        }
        if (numberBox != null)

        {
            numberBox.Value = SelectedPageIndex + 1;
        }

        UpdateOnEdgeButtonVisualStates();
        UpdateTemplateSettingElementLists();

        if (DisplayMode == PagerControlDisplayMode.ButtonPanel)
        {
            // NumberPanel needs to update based on multiple parameters.
            // SelectedPageIndex is one of them, so let's do that now!
            UpdateNumberPanel(NumberOfPages);
        }

        // Fire value property change for UIA
        if (FrameworkElementAutomationPeer.FromElement(this) is PagerControlAutomationPeer peer)
        {
            peer.RaiseSelectionChanged(lastSelectedPageIndex, SelectedPageIndex);
        }
        RaiseSelectedIndexChanged();
    }

    private void RaiseSelectedIndexChanged()
    {
        var args = new PagerControlSelectedIndexChangedEventArgs(SelectedPageIndex, lastSelectedPageIndex);
        SelectedIndexChanged?.Invoke(this, args);
    }

    private void OnButtonVisibilityChanged(PagerControlButtonVisibility visibility, string visibleStateName, string collapsedStateName, string hiddenStateName, int hiddenOnEdgePageCriteria)
    {
        if (visibility == PagerControlButtonVisibility.Visible)
        {
            VisualStateManager.GoToState(this, visibleStateName, false);
        }
        else if (visibility == PagerControlButtonVisibility.Hidden)
        {
            VisualStateManager.GoToState(this, collapsedStateName, false);
        }
        else
        {
            if (SelectedPageIndex != hiddenOnEdgePageCriteria)
            {
                VisualStateManager.GoToState(this, visibleStateName, false);
            }
            else
            {
                VisualStateManager.GoToState(this, hiddenStateName, false);
            }
        }
    }

    private void UpdateTemplateSettingElementLists()
    {
        // Cache values for performance :)
        var displayMode = DisplayMode;
        var numberOfPages = NumberOfPages;

        if (displayMode == PagerControlDisplayMode.ComboBox ||
            displayMode == PagerControlDisplayMode.Auto)
        {
            if (numberOfPages > -1)
            {
                FillComboBoxCollectionToSize(numberOfPages);
            }
            else
            {
                if (comboBoxEntries.Count < c_infiniteModeComboBoxItemsIncrement)
                {
                    FillComboBoxCollectionToSize(c_infiniteModeComboBoxItemsIncrement);
                }
            }
        }
        else if (displayMode == PagerControlDisplayMode.ButtonPanel)
        {
            UpdateNumberPanel(numberOfPages);
        }
    }

    private void FillComboBoxCollectionToSize(int numberOfPages)
    {
        var currentComboBoxItemsCount = comboBoxEntries.Count;
        if (currentComboBoxItemsCount <= numberOfPages)
        {
            // We are increasing the number of pages, so add the missing numbers.
            for (int i = currentComboBoxItemsCount; i < numberOfPages; i++)
            {
                comboBoxEntries.Add(i + 1);
            }
        }
        else
        {
            // We are decreasing the number of pages, so remove numbers starting at the end.
            for (int i = currentComboBoxItemsCount; i > numberOfPages; i--)
            {
                comboBoxEntries.RemoveAt(comboBoxEntries.Count - 1);
            }
        }
    }

    private void UpdateOnEdgeButtonVisualStates()
    {
        // Cache those values as we need them often and accessing a DP is (relatively)expensive

        var selectedPageIndex = SelectedPageIndex;
        var numberOfPages = NumberOfPages;

        // Handle disabled/enabled status of buttons
        if (selectedPageIndex == 0)
        {
            VisualStateManager.GoToState(this, c_firstPageButtonDisabledVisualState, false);
            VisualStateManager.GoToState(this, c_previousPageButtonDisabledVisualState, false);
            VisualStateManager.GoToState(this, c_nextPageButtonEnabledVisualState, false);
            VisualStateManager.GoToState(this, c_lastPageButtonEnabledVisualState, false);
        }
        else if (selectedPageIndex >= numberOfPages - 1)
        {
            VisualStateManager.GoToState(this, c_firstPageButtonEnabledVisualState, false);
            VisualStateManager.GoToState(this, c_previousPageButtonEnabledVisualState, false);
            if (numberOfPages > -1)
            {
                VisualStateManager.GoToState(this, c_nextPageButtonDisabledVisualState, false);
            }
            else
            {
                VisualStateManager.GoToState(this, c_nextPageButtonEnabledVisualState, false);
            }
            VisualStateManager.GoToState(this, c_lastPageButtonDisabledVisualState, false);
        }
        else
        {
            VisualStateManager.GoToState(this, c_firstPageButtonEnabledVisualState, false);
            VisualStateManager.GoToState(this, c_previousPageButtonEnabledVisualState, false);
            VisualStateManager.GoToState(this, c_nextPageButtonEnabledVisualState, false);
            VisualStateManager.GoToState(this, c_lastPageButtonEnabledVisualState, false);
        }

        // Handle HiddenOnEdge states
        if (FirstButtonVisibility == PagerControlButtonVisibility.HiddenOnEdge)
        {
            if (selectedPageIndex != 0)
            {
                VisualStateManager.GoToState(this, c_firstPageButtonVisibleVisualState, false);
            }
            else
            {
                VisualStateManager.GoToState(this, c_firstPageButtonHiddenVisualState, false);
            }
        }
        if (PreviousButtonVisibility == PagerControlButtonVisibility.HiddenOnEdge)
        {
            if (selectedPageIndex != 0)
            {
                VisualStateManager.GoToState(this, c_previousPageButtonVisibleVisualState, false);
            }
            else
            {
                VisualStateManager.GoToState(this, c_previousPageButtonHiddenVisualState, false);
            }
        }
        if (NextButtonVisibility == PagerControlButtonVisibility.HiddenOnEdge)
        {
            if (selectedPageIndex != numberOfPages - 1)
            {
                VisualStateManager.GoToState(this, c_nextPageButtonVisibleVisualState, false);
            }
            else
            {
                VisualStateManager.GoToState(this, c_nextPageButtonHiddenVisualState, false);
            }
        }
        if (LastButtonVisibility == PagerControlButtonVisibility.HiddenOnEdge)
        {
            if (selectedPageIndex != numberOfPages - 1)
            {
                VisualStateManager.GoToState(this, c_lastPageButtonVisibleVisualState, false);
            }
            else
            {
                VisualStateManager.GoToState(this, c_lastPageButtonHiddenVisualState, false);
            }
        }
    }

    private void UpdateNumberPanel(int numberOfPages)
    {
        if (numberOfPages < 0)
        {
            UpdateNumberOfPanelCollectionInfiniteItems();
        }
        else if (numberOfPages < 8)
        {
            UpdateNumberPanelCollectionAllItems(numberOfPages);
        }
        else
        {
            var selectedIndex = SelectedPageIndex;
            // Idea: Choose correct "template" based on SelectedPageIndex (x) and NumberOfPages (n)
            // 1 2 3 4 5 6 ... n <-- Items
            if (selectedIndex < 4)
            {
                // First four items selected, create following pattern:
                // 1 2 3 4 5... n
                UpdateNumberPanelCollectionStartWithEllipsis(numberOfPages, selectedIndex);
            }
            else if (selectedIndex >= numberOfPages - 4)
            {
                // Last four items selected, create following pattern:
                //1 [...] n-4 n-3 n-2 n-1 n
                UpdateNumberPanelCollectionEndWithEllipsis(numberOfPages, selectedIndex);
            }
            else
            {
                // Neither start or end, so lets do this:
                // 1 [...] x-2 x-1 x x+1 x+2 [...] n
                // where x > 4 and x < n - 4
                UpdateNumberPanelCollectionCenterWithEllipsis(numberOfPages, selectedIndex);
            }
        }
    }


    private void UpdateNumberOfPanelCollectionInfiniteItems()
    {
        var selectedIndex = SelectedPageIndex;

        numberPanelElements.Clear();
        if (selectedIndex < 3)
        {
            AppendButtonToNumberPanelList(1, 0);
            AppendButtonToNumberPanelList(2, 0);
            AppendButtonToNumberPanelList(3, 0);
            AppendButtonToNumberPanelList(4, 0);
            AppendButtonToNumberPanelList(5, 0);
            MoveIdentifierToElement(selectedIndex);
        }
        else
        {
            AppendButtonToNumberPanelList(1, 0);
            AppendEllipsisIconToNumberPanelList();
            AppendButtonToNumberPanelList(selectedIndex, 0);
            AppendButtonToNumberPanelList(selectedIndex + 1, 0);
            AppendButtonToNumberPanelList(selectedIndex + 2, 0);
            MoveIdentifierToElement(3);
        }
    }

    private void UpdateNumberPanelCollectionAllItems(int numberOfPages)
    {
        // Check that the NumberOfPages did not change, so we can skip recreating collection
        if (lastNumberOfPagesCount != numberOfPages)
        {
            numberPanelElements.Clear();
            for (int i = 0; i < numberOfPages && i < 7; i++)
            {
                AppendButtonToNumberPanelList(i + 1, numberOfPages);
            }
        }
        MoveIdentifierToElement(SelectedPageIndex);
    }

    private void UpdateNumberPanelCollectionStartWithEllipsis(int numberOfPages, int selectedIndex)
    {
        if (lastNumberOfPagesCount != numberOfPages)
        {
            // Do a recalculation as the number of pages changed.
            numberPanelElements.Clear();
            AppendButtonToNumberPanelList(1, numberOfPages);
            AppendButtonToNumberPanelList(2, numberOfPages);
            AppendButtonToNumberPanelList(3, numberOfPages);
            AppendButtonToNumberPanelList(4, numberOfPages);
            AppendButtonToNumberPanelList(5, numberOfPages);
            if (ButtonPanelAlwaysShowFirstLastPageIndex)
            {
                AppendEllipsisIconToNumberPanelList();
                AppendButtonToNumberPanelList(numberOfPages, numberOfPages);
            }
        }
        // SelectedIndex is definitely the correct index here as we are counting from start
        MoveIdentifierToElement(selectedIndex);
    }

    private void UpdateNumberPanelCollectionEndWithEllipsis(int numberOfPages, int selectedIndex)
    {
        if (lastNumberOfPagesCount != numberOfPages)
        {
            // Do a recalculation as the number of pages changed.
            numberPanelElements.Clear();
            if (ButtonPanelAlwaysShowFirstLastPageIndex)
            {
                AppendButtonToNumberPanelList(1, numberOfPages);
                AppendEllipsisIconToNumberPanelList();
            }
            AppendButtonToNumberPanelList(numberOfPages - 4, numberOfPages);
            AppendButtonToNumberPanelList(numberOfPages - 3, numberOfPages);
            AppendButtonToNumberPanelList(numberOfPages - 2, numberOfPages);
            AppendButtonToNumberPanelList(numberOfPages - 1, numberOfPages);
            AppendButtonToNumberPanelList(numberOfPages, numberOfPages);
        }
        // We can only be either the last, the second from last or third from last
        // => SelectedIndex = NumberOfPages - y with y in {1,2,3}
        if (ButtonPanelAlwaysShowFirstLastPageIndex)
        {
            // Last item sits at index 4.
            // SelectedPageIndex for the last page is NumberOfPages - 1.
            // => SelectedItem = SelectedIndex - NumberOfPages + 7;
            MoveIdentifierToElement(selectedIndex - numberOfPages + 7);
        }
        else
        {
            // Last item sits at index 6.
            // SelectedPageIndex for the last page is NumberOfPages - 1.
            // => SelectedItem = SelectedIndex - NumberOfPages + 5;
            MoveIdentifierToElement(selectedIndex - numberOfPages + 5);
        }
    }

    private void UpdateNumberPanelCollectionCenterWithEllipsis(int numberOfPages, int selectedIndex)
    {
        var showFirstLastPageIndex = ButtonPanelAlwaysShowFirstLastPageIndex;
        if (lastNumberOfPagesCount != numberOfPages)
        {
            // Do a recalculation as the number of pages changed.
            numberPanelElements.Clear();
            if (showFirstLastPageIndex)
            {
                AppendButtonToNumberPanelList(1, numberOfPages);
                AppendEllipsisIconToNumberPanelList();
            }
            AppendButtonToNumberPanelList(selectedIndex, numberOfPages);
            AppendButtonToNumberPanelList(selectedIndex + 1, numberOfPages);
            AppendButtonToNumberPanelList(selectedIndex + 2, numberOfPages);
            if (showFirstLastPageIndex)
            {
                AppendEllipsisIconToNumberPanelList();
                AppendButtonToNumberPanelList(numberOfPages, numberOfPages);
            }
        }
        // "selectedIndex + 1" is our representation for SelectedIndex.
        if (showFirstLastPageIndex)
        {
            // SelectedIndex + 1 is the fifth element.
            // Collections are base zero, so the index to underline is 3.
            MoveIdentifierToElement(3);
        }
        else
        {
            // SelectedIndex + 1 is the third element.
            // Collections are base zero, so the index to underline is 1.
            MoveIdentifierToElement(1);
        }
    }

    private void MoveIdentifierToElement(int index)
    {
        if (selectedPageIndicator != null && numberPanelRepeater != null)
        {
            numberPanelRepeater.UpdateLayout();
            if (numberPanelRepeater.TryGetElement(index) is FrameworkElement element)
            {
                var boundingRect = element.TransformToVisual(numberPanelRepeater).TransformBounds(new Rect(0, 0, (float)numberPanelRepeater.ActualWidth, (float)numberPanelRepeater.ActualHeight));
                Thickness newMargin = new Thickness()
                {
                    Left = boundingRect.X,
                    Top = 0,
                    Right = 0,
                    Bottom = 0,
                };
                selectedPageIndicator.Margin = newMargin;
                selectedPageIndicator.Width = element.ActualWidth;
            }
        }
    }

    private void AppendButtonToNumberPanelList(int pageNumber, int numberOfPages)
    {

        var button = new Button()
        {
            Content = pageNumber
        };
        button.Click += (object sender, RoutedEventArgs args) =>
        {
            var unboxedValue = button.Content as int?;
            if (unboxedValue != null)
            {
                SelectedPageIndex = unboxedValue.Value - 1;
            }
        };

        // Set the default style of buttons
        if (ResourceLookup(c_numberPanelButtonStyleName) is Style style)
        {
            button.Style = style;
        }
        AutomationProperties.SetName(button, "Page" + pageNumber);
        AutomationProperties.SetPositionInSet(button, pageNumber);
        AutomationProperties.SetSizeOfSet(button, numberOfPages);
        numberPanelElements.Add(button);
    }

    void AppendEllipsisIconToNumberPanelList()
    {
        var ellipsisIcon = new SymbolIcon()
        {
            Symbol = Symbol.More
        };
        numberPanelElements.Add(ellipsisIcon);
    }

    private void RootGrid_KeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (args.Key == VirtualKey.Left || args.Key == VirtualKey.GamepadDPadLeft)
        {
            FocusManager.TryMoveFocus(FocusNavigationDirection.Left);
        }
        else if (args.Key == VirtualKey.Right || args.Key == VirtualKey.GamepadDPadRight)
        {
            FocusManager.TryMoveFocus(FocusNavigationDirection.Right);
        }
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (comboBox != null)
        {
            SelectedPageIndex = comboBox.SelectedIndex;
        }
    }

    private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        SelectedPageIndex = (int)(args.NewValue - 1);
    }

    private void FirstPageButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedPageIndex = 0;
        if (FirstButtonCommand != null)
        {
            FirstButtonCommand.Execute(null);
        }
    }

    private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
    {
        // In this method, SelectedPageIndex is always greater than 1.
        SelectedPageIndex = SelectedPageIndex - 1;
        if (PreviousButtonCommand != null)
        {
            PreviousButtonCommand.Execute(null);
        }
    }

    private void NextPageButton_Click(object sender, RoutedEventArgs e)
    {
        // In this method, SelectedPageIndex is always less than maximum.
        SelectedPageIndex = SelectedPageIndex + 1;
        if (NextButtonCommand != null)
        {
            NextButtonCommand.Execute(null);
        }
    }

    private void LastPageButton_Click(object sender, RoutedEventArgs e)
    {
        SelectedPageIndex = NumberOfPages - 1;
        if (LastButtonCommand != null)
        {
            LastButtonCommand.Execute(null);
        }
    }

    private void PagerControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Needed so we can update the UI and selection indicator once we have actually rendered
        OnSelectedPageIndexChange(-1);
    }

    public object? ResourceLookup(string name)
    {
        if (Application.Current.Resources.ContainsKey(name))
        {
            return Application.Current.Resources[name];
        }
        return null;
    }
}
