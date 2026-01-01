// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public sealed partial class SamplePanel
{
    public static readonly DependencyProperty HeaderProperty =
    DependencyProperty.Register(
        nameof(Header),
        typeof(string),
        typeof(SamplePanel),
        new PropertyMetadata(null));

    public string? Header
    {
        get => (string?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }


    public static readonly DependencyProperty MainContentProperty =
        DependencyProperty.Register(
            nameof(MainContent),
            typeof(UIElement),
            typeof(SamplePanel),
            new PropertyMetadata(null));

    public UIElement? MainContent
    {
        get => (UIElement?)GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
    }


    public static readonly DependencyProperty SideContentProperty =
        DependencyProperty.Register(
            nameof(SideContent),
            typeof(UIElement),
            typeof(SamplePanel),
            new PropertyMetadata(null));

    public UIElement? SideContent
    {
        get => (UIElement?)GetValue(SideContentProperty);
        set => SetValue(SideContentProperty, value);
    }
}
