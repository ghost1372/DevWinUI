// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class BreadcrumbBar : Control
{
    public static readonly DependencyProperty RootItemProperty =
    DependencyProperty.Register(
        nameof(RootItem),
        typeof(FrameworkElement),
        typeof(BreadcrumbBar),
        new PropertyMetadata(null));

    public FrameworkElement? RootItem
    {
        get => (FrameworkElement?)GetValue(RootItemProperty);
        set => SetValue(RootItemProperty, value);
    }


    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(
            nameof(ItemsSource),
            typeof(object),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null));

    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }


    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(
            nameof(ItemTemplate),
            typeof(object),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null));

    public object? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }


    public static readonly DependencyProperty EllipsisButtonToolTipProperty =
        DependencyProperty.Register(
            nameof(EllipsisButtonToolTip),
            typeof(string),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null));

    public string? EllipsisButtonToolTip
    {
        get => (string?)GetValue(EllipsisButtonToolTipProperty);
        set => SetValue(EllipsisButtonToolTipProperty, value);
    }


    public static readonly DependencyProperty RootItemToolTipProperty =
        DependencyProperty.Register(
            nameof(RootItemToolTip),
            typeof(string),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null));

    public string? RootItemToolTip
    {
        get => (string?)GetValue(RootItemToolTipProperty);
        set => SetValue(RootItemToolTipProperty, value);
    }


    public static readonly DependencyProperty RootItemChevronToolTipProperty =
        DependencyProperty.Register(
            nameof(RootItemChevronToolTip),
            typeof(string),
            typeof(BreadcrumbBar),
            new PropertyMetadata(null));

    public string? RootItemChevronToolTip
    {
        get => (string?)GetValue(RootItemChevronToolTipProperty);
        set => SetValue(RootItemChevronToolTipProperty, value);
    }
}
