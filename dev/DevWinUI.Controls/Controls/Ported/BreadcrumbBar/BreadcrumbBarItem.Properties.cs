// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class BreadcrumbBarItem
{
    public static readonly DependencyProperty IsEllipsisProperty =
    DependencyProperty.Register(
        nameof(IsEllipsis),
        typeof(bool),
        typeof(BreadcrumbBarItem),
        new PropertyMetadata(false, OnIsEllipsisChanged));

    public bool IsEllipsis
    {
        get => (bool)GetValue(IsEllipsisProperty);
        set => SetValue(IsEllipsisProperty, value);
    }

    private static void OnIsEllipsisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BreadcrumbBarItem)d;
        bool newValue = (bool)e.NewValue;

        VisualStateManager.GoToState(
            control,
            newValue ? "ChevronCollapsed" : "ChevronVisible",
            true);
    }


    public static readonly DependencyProperty IsLastItemProperty =
        DependencyProperty.Register(
            nameof(IsLastItem),
            typeof(bool),
            typeof(BreadcrumbBarItem),
            new PropertyMetadata(false, OnIsLastItemChanged));

    public bool IsLastItem
    {
        get => (bool)GetValue(IsLastItemProperty);
        set => SetValue(IsLastItemProperty, value);
    }

    private static void OnIsLastItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BreadcrumbBarItem)d;
        bool newValue = (bool)e.NewValue;

        VisualStateManager.GoToState(
            control,
            newValue ? "ChevronCollapsed" : "ChevronVisible",
            true);
    }


    public static readonly DependencyProperty ItemToolTipProperty =
        DependencyProperty.Register(
            nameof(ItemToolTip),
            typeof(string),
            typeof(BreadcrumbBarItem),
            new PropertyMetadata(null));

    public string? ItemToolTip
    {
        get => (string?)GetValue(ItemToolTipProperty);
        set => SetValue(ItemToolTipProperty, value);
    }


    public static readonly DependencyProperty ChevronToolTipProperty =
        DependencyProperty.Register(
            nameof(ChevronToolTip),
            typeof(string),
            typeof(BreadcrumbBarItem),
            new PropertyMetadata(null));

    public string? ChevronToolTip
    {
        get => (string?)GetValue(ChevronToolTipProperty);
        set => SetValue(ChevronToolTipProperty, value);
    }

}
