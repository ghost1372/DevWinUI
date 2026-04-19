// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace DevWinUI;

public partial class PagerControl
{
    private const string c_numberBoxVisibleVisualState = "NumberBoxVisible";
    private const string c_comboBoxVisibleVisualState = "ComboBoxVisible";
    private const string c_numberPanelVisibleVisualState = "NumberPanelVisible";

    private const string c_firstPageButtonVisibleVisualState = "FirstPageButtonVisible";
    private const string c_firstPageButtonNotVisibleVisualState = "FirstPageButtonCollapsed";
    private const string c_firstPageButtonHiddenVisualState = "FirstPageButtonHidden";
    private const string c_firstPageButtonEnabledVisualState = "FirstPageButtonEnabled";
    private const string c_firstPageButtonDisabledVisualState = "FirstPageButtonDisabled";

    private const string c_previousPageButtonVisibleVisualState = "PreviousPageButtonVisible";
    private const string c_previousPageButtonNotVisibleVisualState = "PreviousPageButtonCollapsed";
    private const string c_previousPageButtonHiddenVisualState = "PreviousPageButtonHidden";
    private const string c_previousPageButtonEnabledVisualState = "PreviousPageButtonEnabled";
    private const string c_previousPageButtonDisabledVisualState = "PreviousPageButtonDisabled";

    private const string c_nextPageButtonVisibleVisualState = "NextPageButtonVisible";
    private const string c_nextPageButtonNotVisibleVisualState = "NextPageButtonCollapsed";
    private const string c_nextPageButtonHiddenVisualState = "NextPageButtonHidden";
    private const string c_nextPageButtonEnabledVisualState = "NextPageButtonEnabled";
    private const string c_nextPageButtonDisabledVisualState = "NextPageButtonDisabled";

    private const string c_lastPageButtonVisibleVisualState = "LastPageButtonVisible";
    private const string c_lastPageButtonNotVisibleVisualState = "LastPageButtonCollapsed";
    private const string c_lastPageButtonHiddenVisualState = "LastPageButtonHidden";
    private const string c_lastPageButtonEnabledVisualState = "LastPageButtonEnabled";
    private const string c_lastPageButtonDisabledVisualState = "LastPageButtonDisabled";

    private const string c_finiteItemsModeState = "FiniteItems";
    private const string c_infiniteItemsModeState = "InfiniteItems";

    private const string c_rootGridName = "RootGrid";
    private const string c_comboBoxName = "ComboBoxDisplay";
    private const string c_numberBoxName = "NumberBoxDisplay";

    private const string c_numberPanelRepeaterName = "NumberPanelItemsRepeater";
    private const string c_numberPanelIndicatorName = "NumberPanelCurrentPageIndicator";
    private const string c_firstPageButtonName = "FirstPageButton";
    private const string c_previousPageButtonName = "PreviousPageButton";
    private const string c_nextPageButtonName = "NextPageButton";
    private const string c_lastPageButtonName = "LastPageButton";

    private const string c_numberPanelButtonStyleName = "PagerControlNumberPanelButtonStyle";
    // This integer determines when to switch between NumberBoxDisplayMode and ComboBoxDisplayMode 
    private const int c_AutoDisplayModeNumberOfPagesThreshold = 10;
    private const int c_infiniteModeComboBoxItemsIncrement = 100;
}
