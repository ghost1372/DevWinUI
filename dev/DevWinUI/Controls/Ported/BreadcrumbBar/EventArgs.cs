// Copyright (c) Files Community
// Licensed under the MIT License.

using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public record class BreadcrumbBarItemClickedEventArgs(BreadcrumbBarItem Item, int Index, bool IsRootItem = false, PointerRoutedEventArgs? PointerRoutedEventArgs = null);

public record class BreadcrumbBarItemDropDownFlyoutEventArgs(MenuFlyout Flyout, BreadcrumbBarItem? Item = null, int Index = -1, bool IsRootItem = false);
