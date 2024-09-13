﻿using Microsoft.UI.Xaml.Automation;

namespace WinUICommunity;
public partial class JsonNavigationViewService : IJsonNavigationViewService
{
    private void AddNavigationViewItemsRecursively(IEnumerable<DataItem> navItems, bool isFooterNavigationViewItem, bool hasTopLevel, bool order, bool orderByDescending, string pageKey, NavigationViewItem parentNavItem = null)
    {
        if (_navigationView == null)
            return;

        if (order && hasTopLevel)
        {
            if (orderByDescending)
            {
                navItems = navItems.OrderByDescending(i => i.Title);
            }
            else
            {
                navItems = navItems.OrderBy(i => i.Title);
            }
        }

        foreach (var navItem in navItems)
        {
            var navigationViewItem = new NavigationViewItem()
            {
                IsEnabled = navItem.IncludedInBuild,
                Content = GetLocalizedText(navItem.Title, navItem.UsexUid),
                Tag = navItem.UniqueId,
                DataContext = navItem
            };

            if (!string.IsNullOrEmpty(navItem.ImageIconPath))
            {
                var icon = GetIcon(navItem.ImageIconPath);
                if (icon != null)
                {
                    navigationViewItem.Icon = icon;
                }
            }

            NavigationHelperEx.SetNavigateTo(navigationViewItem, navItem.UniqueId + navItem.Parameter?.ToString());
            navigationViewItem.InfoBadge = GetInfoBadge(navItem);
            AutomationProperties.SetName(navigationViewItem, GetLocalizedText(navItem.Title, navItem.UsexUid));

            if (parentNavItem == null)
            {
                if (hasTopLevel)
                {
                    NavigationHelperEx.SetParent(navigationViewItem, topLevelItem);
                    topLevelItem.MenuItems.Add(navigationViewItem);
                }
                else
                {
                    if (isFooterNavigationViewItem)
                    {
                        _navigationView.FooterMenuItems.Add(navigationViewItem);
                    }
                    else
                    {
                        if (navItem.IsNavigationViewItemHeader)
                        {
                            _navigationView.MenuItems.Add(new NavigationViewItemHeader { Content = navItem.Content });
                        }
                        else
                        {
                            _navigationView.MenuItems.Add(navigationViewItem);
                        }
                    }
                }
            }
            else
            {
                NavigationHelperEx.SetParent(navigationViewItem, parentNavItem);
                if (navItem.IsNavigationViewItemHeader)
                {
                    parentNavItem.MenuItems.Add(new NavigationViewItemHeader { Content = navigationViewItem.Content });
                }
                else
                {
                    parentNavItem.MenuItems.Add(navigationViewItem);
                }
            }

            if (navItem.Items != null && navItem.Items.Count > 0)
            {
                AddNavigationViewItemsRecursively(navItem.Items, isFooterNavigationViewItem, hasTopLevel, order, orderByDescending, navItem.UniqueId, navigationViewItem);
            }
        }
    }

    private void AddNavigationMenuItems()
    {
        AddNavigationMenuItemsBase(true, false);
    }
    private void AddNavigationMenuItems(bool orderRootItems)
    {
        AddNavigationMenuItemsBase(orderRootItems, false);
    }
    private void AddNavigationMenuItems(bool orderRootItems, bool orderByDescending)
    {
        AddNavigationMenuItemsBase(orderRootItems, orderByDescending);
    }
    private void AddNavigationMenuItemsBase(bool orderRootItems, bool orderByDescending)
    {
        if (_navigationView == null)
            return;

        DataSource.GetDataAsync(JsonFilePath, _pathType, _autoIncludedInBuild).ContinueWith(t =>
        {
            var dataGroup = DataSource.Groups.Where(i => !i.IsSpecialSection && !i.HideGroup);
            if (orderRootItems)
            {
                if (orderByDescending)
                {
                    dataGroup = dataGroup.OrderByDescending(i => i.Title);
                }
                else
                {
                    dataGroup = dataGroup.OrderBy(i => i.Title);
                }
            }
            foreach (var group in dataGroup)
            {
                var dataItem = group.Items.Where(i => !i.HideNavigationViewItem);
                if (group.Order)
                {
                    if (group.OrderByDescending)
                    {
                        dataItem = dataItem.OrderByDescending(i => i.Title);
                    }
                    else
                    {
                        dataItem = dataItem.OrderBy(i => i.Title);
                    }
                }

                if (group.ShowItemsWithoutGroup)
                {
                    AddNavigationViewItemsRecursively(dataItem, group.IsFooterNavigationViewItem, false, group.Order, group.OrderByDescending, group.UniqueId);
                }
                else
                {
                    topLevelItem = new NavigationViewItem()
                    {
                        Content = GetLocalizedText(group.Title, group.UsexUid),
                        IsExpanded = group.IsExpanded,
                        Tag = group.UniqueId,
                        DataContext = group
                    };

                    if (!string.IsNullOrEmpty(group.ImageIconPath))
                    {
                        var icon = GetIcon(group.ImageIconPath);
                        if (icon != null)
                        {
                            topLevelItem.Icon = icon;
                        }
                    }

                    NavigationHelperEx.SetNavigateTo(topLevelItem, group.UniqueId);
                    AutomationProperties.SetName(topLevelItem, GetLocalizedText(group.Title, group.UsexUid));
                    topLevelItem.InfoBadge = GetInfoBadge(group);

                    AddNavigationViewItemsRecursively(dataItem, group.IsFooterNavigationViewItem, true, group.Order, group.OrderByDescending, group.UniqueId);

                    if (group.IsFooterNavigationViewItem)
                    {
                        _navigationView.FooterMenuItems.Add(topLevelItem);
                    }
                    else
                    {
                        if (group.IsNavigationViewItemHeader)
                        {
                            _navigationView.MenuItems.Add(new NavigationViewItemHeader { Content = topLevelItem.Content });
                        }
                        else
                        {
                            _navigationView.MenuItems.Add(topLevelItem);
                        }
                    }
                }
            }

            ConfigPages();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private InfoBadge GetInfoBadge(dynamic data)
    {
        if (data.DataInfoBadge is not null)
        {
            bool hideNavigationViewItemBadge = data.DataInfoBadge.HideNavigationViewItemBadge;
            string value = data.DataInfoBadge.BadgeValue;
            string style = data.DataInfoBadge.BadgeStyle;
            bool hasValue = !string.IsNullOrEmpty(value);
            if (style != null && style.Contains("Dot", StringComparison.OrdinalIgnoreCase) || style.Contains("Icon", StringComparison.OrdinalIgnoreCase))
            {
                hasValue = true;
            }
            if (!hideNavigationViewItemBadge && hasValue)
            {
                int badgeValue = Convert.ToInt32(data.DataInfoBadge.BadgeValue);
                int width = data.DataInfoBadge.BadgeWidth;
                int height = data.DataInfoBadge.BadgeHeight;

                InfoBadge infoBadge = new()
                {
                    Style = Application.Current.Resources[style] as Style
                };
                switch (style.ToLower())
                {
                    case string s when s.Contains("value"):
                        infoBadge.Value = badgeValue;
                        break;
                    case string s when s.Contains("icon"):
                        infoBadge.IconSource = GetIconSource(data.DataInfoBadge);
                        break;
                }

                if (width > 0 && height > 0)
                {
                    infoBadge.Width = width;
                    infoBadge.Height = height;
                }

                return infoBadge;
            }
        }
        return null;
    }

    private IconSource GetIconSource(DataInfoBadge infoBadge)
    {
        string symbol = infoBadge?.BadgeSymbolIcon;
        string image = infoBadge?.BadgeBitmapIcon;
        string glyph = infoBadge?.BadgeFontIconGlyph;
        string fontName = infoBadge?.BadgeFontIconFontName;

        if (!string.IsNullOrEmpty(symbol))
        {
            return new SymbolIconSource
            {
                Symbol = GeneralHelper.GetEnum<Symbol>(symbol),
                Foreground = Application.Current.Resources["SystemControlForegroundAltHighBrush"] as Brush,
            };
        }

        if (!string.IsNullOrEmpty(image))
        {
            return new BitmapIconSource
            {
                UriSource = new Uri(image),
                ShowAsMonochrome = false
            };
        }

        if (!string.IsNullOrEmpty(glyph))
        {
            var fontIcon = new FontIconSource
            {
                Glyph = GeneralHelper.GetGlyph(glyph),
                Foreground = Application.Current.Resources["SystemControlForegroundAltHighBrush"] as Brush,
            };
            if (!string.IsNullOrEmpty(fontName))
            {
                fontIcon.FontFamily = new FontFamily(fontName);
            }
            return fontIcon;
        }
        return null;
    }

    private IconElement GetIcon(string imagePath)
    {
        return string.IsNullOrEmpty(imagePath)
            ? null
            : imagePath.ToLowerInvariant().EndsWith(".png") || imagePath.ToLowerInvariant().EndsWith(".jpg")
                ? new BitmapIcon() { UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute), ShowAsMonochrome = false }
                : new FontIcon()
                {
                    Glyph = imagePath
                };
    }

    private string GetLocalizedText(string input, bool usexUid)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        try
        {
            if (usexUid)
            {
                if (ResourceManager != null && ResourceContext != null)
                {
                    var candidate = ResourceManager.MainResourceMap.TryGetValue($"Resources/{input}", ResourceContext);
                    return candidate != null ? candidate.ValueAsString : input;
                }
            }
        }
        catch (Exception)
        {
            return input;
        }
        return input;
    }
}
