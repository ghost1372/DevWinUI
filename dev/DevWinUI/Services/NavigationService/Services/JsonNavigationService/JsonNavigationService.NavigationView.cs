using Microsoft.UI.Xaml.Automation;

namespace DevWinUI;
public partial class JsonNavigationService
{
    private void AddNavigationMenuItems()
    {
        AddNavigationMenuItems(OrderItemsType.None);
    }
    private void AddNavigationMenuItems(OrderItemsType orderItems)
    {
        foreach (var group in GetOrderedDataGroups(orderItems).Where(i => !i.IsSpecialSection && !i.HideGroup))
        {
            var itemGroup = new NavigationViewItem()
            {
                Content = group.Title,
                IsExpanded = group.IsExpanded,
                Tag = group.UniqueId,
                DataContext = group,
                InfoBadge = GetInfoBadge(group, null, group.DefaultBuiltInNavigationViewInfoBadgeStyle, group.UseBuiltInNavigationViewInfoBadgeStyle)
            };
            var icon = GetIcon(group.ImagePath, group.IconGlyph);
            if (icon != null)
            {
                itemGroup.Icon = icon;
            }

            AutomationProperties.SetName(itemGroup, group.Title);
            AutomationProperties.SetAutomationId(itemGroup, group.UniqueId);

            foreach (var item in GetOrderedDataItems(group.Items, orderItems).Where(i => !i.HideNavigationViewItem))
            {
                var itemInGroup = new NavigationViewItem()
                {
                    IsEnabled = item.IncludedInBuild,
                    Content = item.Title,
                    Tag = item.UniqueId,
                    DataContext = item,
                    InfoBadge = GetInfoBadge(item, item.BadgeString, group.DefaultBuiltInNavigationViewInfoBadgeStyle, group.UseBuiltInNavigationViewInfoBadgeStyle)
                };

                var iconInGroup = GetIcon(item.ImagePath, item.IconGlyph);
                if (iconInGroup != null)
                {
                    itemInGroup.Icon = iconInGroup;
                }

                AutomationProperties.SetName(itemInGroup, item.Title);
                AutomationProperties.SetAutomationId(itemInGroup, item.UniqueId);

                if (group.ShowItemsWithoutGroup)
                {
                    if (group.IsFooterNavigationViewItem)
                    {
                        _navigationView.FooterMenuItems.Add(itemInGroup);
                    }
                    else
                    {
                        if (group.IsNavigationViewItemHeader)
                        {
                            _navigationView.MenuItems.Add(new NavigationViewItemHeader { Content = itemInGroup.Content });
                        }
                        else
                        {
                            _navigationView.MenuItems.Add(itemInGroup);
                        }
                    }
                }
                else
                {
                    itemGroup.MenuItems.Add(itemInGroup);
                }
            }

            if (!group.ShowItemsWithoutGroup)
            {
                if (group.IsFooterNavigationViewItem)
                {
                    _navigationView.FooterMenuItems.Add(itemGroup);
                }
                else
                {
                    if (group.IsNavigationViewItemHeader)
                    {
                        _navigationView.MenuItems.Add(new NavigationViewItemHeader { Content = itemGroup.Content });
                    }
                    else
                    {
                        _navigationView.MenuItems.Add(itemGroup);
                    }
                }
            }
        }

        EnsureNavigationSelection(_defaultPage?.ToString());
    }

    private IEnumerable<DataGroup> GetOrderedDataGroups(OrderItemsType orderItems)
    {
        switch (orderItems)
        {
            case OrderItemsType.None:
                return DataSource.Instance.Groups;
            case OrderItemsType.AscendingBoth:
            case OrderItemsType.AscendingTopLevel:
                return DataSource.Instance.Groups.OrderBy(i => i.Title);
            case OrderItemsType.DescendingBoth:
            case OrderItemsType.DescendingTopLevel:
                return DataSource.Instance.Groups.OrderByDescending(i => i.Title);
            default:
                return DataSource.Instance.Groups;
        }
    }
    private IEnumerable<DataItem> GetOrderedDataItems(IEnumerable<DataItem> dataItems, OrderItemsType orderItems)
    {
        switch (orderItems)
        {
            case OrderItemsType.None:
                return dataItems;
            case OrderItemsType.AscendingBoth:
            case OrderItemsType.AscendingSubLevel:
                return dataItems.OrderBy(i => i.Title);
            case OrderItemsType.DescendingBoth:
            case OrderItemsType.DescendingSubLevel:
                return dataItems.OrderByDescending(i => i.Title);
            default:
                return dataItems;
        }
    }

    private InfoBadge GetInfoBadge(BaseDataInfo item, string badgeString, string defaultBuiltInInfoBadgeStyle, bool useBuiltInStyle)
    {
        if (item.DataInfoBadge == null)
        {
            item.DataInfoBadge = new DataInfoBadge();
        }

        if (useBuiltInStyle)
        {
            if (string.IsNullOrEmpty(item.DataInfoBadge.NavigationViewInfoBadgeStyle) && !string.IsNullOrEmpty(badgeString))
            {
                item.DataInfoBadge.NavigationViewInfoBadgeStyle = defaultBuiltInInfoBadgeStyle;

                if (string.IsNullOrEmpty(item.DataInfoBadge.InfoBadgeValue))
                {
                    item.DataInfoBadge.InfoBadgeValue = badgeString.ToUpper();
                }
            }
        }

        var styleKey = item.DataInfoBadge.NavigationViewInfoBadgeStyle;
        var isHidden = item.DataInfoBadge.IsNavigationViewInfoBadgeHidden;

        if (!string.IsNullOrEmpty(styleKey) && !isHidden)
        {
            if (Application.Current.Resources.TryGetValue(styleKey, out var resource) && resource is Style style)
            {
                return new InfoBadge { Style = style, Tag = item.DataInfoBadge.InfoBadgeValue };
            }
        }

        return null;
    }
    private IconElement GetIcon(string imagePath, string iconGlyph)
    {
        if (string.IsNullOrEmpty(imagePath) && string.IsNullOrEmpty(iconGlyph))
        {
            return null;
        }

        if (!string.IsNullOrEmpty(iconGlyph))
        {
            return GetFontIcon(iconGlyph);
        }

        if (!string.IsNullOrEmpty(imagePath))
        {
            return new BitmapIcon() { UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute), ShowAsMonochrome = false };
        }

        return null;
    }

    private FontIcon GetFontIcon(string glyph)
    {
        var fontIcon = new FontIcon();
        if (!string.IsNullOrEmpty(_fontFamilyForGlyph))
        {
            fontIcon.FontFamily = new FontFamily(_fontFamilyForGlyph);
        }
        var _glyph = GeneralHelper.GetGlyph(glyph);
        if (!string.IsNullOrEmpty(_glyph))
        {
            fontIcon.Glyph = _glyph; // Set the Glyph property
        }
        else
        {
            fontIcon.Glyph = glyph;
        }

        return fontIcon;
    }
}
