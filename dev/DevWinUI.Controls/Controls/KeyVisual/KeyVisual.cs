// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using Microsoft.UI.Xaml.Markup;

using Windows.System;

namespace DevWinUI;

[TemplatePart(Name = KeyPresenter, Type = typeof(ContentPresenter))]
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Default", GroupName = "StateStates")]
[TemplateVisualState(Name = "Error", GroupName = "StateStates")]
public sealed partial class KeyVisual : Control
{
    private const string KeyPresenter = "KeyPresenter";
    private KeyVisual _keyVisual;
    private ContentPresenter _keyPresenter;

    public object Content
    {
        get => (object)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(KeyVisual), new PropertyMetadata(default(string), OnContentChanged));

    public VisualType VisualType
    {
        get => (VisualType)GetValue(VisualTypeProperty);
        set => SetValue(VisualTypeProperty, value);
    }

    public static readonly DependencyProperty VisualTypeProperty = DependencyProperty.Register("VisualType", typeof(VisualType), typeof(KeyVisual), new PropertyMetadata(default(VisualType), OnSizeChanged));

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public static readonly DependencyProperty IsErrorProperty = DependencyProperty.Register("IsError", typeof(bool), typeof(KeyVisual), new PropertyMetadata(false, OnIsErrorChanged));

    public KeyVisual()
    {
        this.DefaultStyleKey = typeof(KeyVisual);
        this.Style = GetStyleSize("TextKeyVisualStyle");
    }

    protected override void OnApplyTemplate()
    {
        IsEnabledChanged -= KeyVisual_IsEnabledChanged;
        _keyVisual = (KeyVisual)this;
        _keyPresenter = (ContentPresenter)_keyVisual.GetTemplateChild(KeyPresenter);
        Update();
        SetEnabledState();
        SetErrorState();
        IsEnabledChanged += KeyVisual_IsEnabledChanged;
        base.OnApplyTemplate();
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((KeyVisual)d).Update();
    }

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((KeyVisual)d).Update();
    }

    private static void OnIsErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((KeyVisual)d).SetErrorState();
    }

    private void Update()
    {
        if (_keyVisual == null)
        {
            return;
        }

        if (_keyVisual.Content != null)
        {
            object content = _keyVisual.Content;
            VirtualKey key = content switch
            {
                string str => GetVirtualKeyFromString(str),
                VirtualKey vk => vk,
                _ => (VirtualKey)(int)content
            };

            if (TryGetIconForKey((int)key, out object iconContent))
            {
                _keyVisual.Style = GetStyleSize("IconKeyVisualStyle");
                _keyVisual._keyPresenter.Content = iconContent;
            }
            else
            {
                _keyVisual.Style = GetStyleSize("TextKeyVisualStyle");
                _keyVisual._keyPresenter.Content = content.ToString();
            }
        }
    }
    private bool TryGetIconForKey(int keyCode, out object content)
    {
        content = null;

        switch (keyCode)
        {
            case 13: content = "\uE751"; return true; // Enter
            case 8: content = "\uE750"; return true; // Backspace
            case 16:
            case 160:
            case 161: content = "\uE752"; return true; // Shift

            case 38: content = "\uE0E4"; return true; // Up
            case 40: content = "\uE0E5"; return true; // Down
            case 37: content = "\uE0E2"; return true; // Left
            case 39: content = "\uE0E3"; return true; // Right

            case 91:
            case 92:
                PathIcon winIcon = XamlReader.Load(
                    @"<PathIcon xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
                            Data=""M683 1229H0V546h683v683zm819 0H819V546h683v683zm-819 819H0v-683h683v683zm819 0H819v-683h683v683z"" />") as PathIcon;
                Viewbox container = new Viewbox
                {
                    Child = winIcon,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = GetIconSize(),
                    Height = GetIconSize()
                };
                content = container;
                return true;
        }

        return false;
    }

    private VirtualKey GetVirtualKeyFromString(string keyName)
    {
        return keyName switch
        {
            "Ctrl" => VirtualKey.Control,
            "Alt" => VirtualKey.Menu,
            "Shift" => VirtualKey.Shift,
            "Win" => VirtualKey.LeftWindows, // You can pick one, or add a separate mapping logic
            _ => Enum.TryParse<VirtualKey>(keyName, out var vk) ? vk : VirtualKey.None
        };
    }
    public Style GetStyleSize(string styleName)
    {
        if (VisualType == VisualType.Small)
        {
            return (Style)Application.Current.Resources["Small" + styleName];
        }
        else if (VisualType == VisualType.SmallOutline)
        {
            return (Style)Application.Current.Resources["SmallOutline" + styleName];
        }
        else if (VisualType == VisualType.TextOnly)
        {
            return (Style)Application.Current.Resources["Only" + styleName];
        }
        else
        {
            return (Style)Application.Current.Resources["Default" + styleName];
        }
    }

    public double GetIconSize()
    {
        if (VisualType == VisualType.Small || VisualType == VisualType.SmallOutline)
        {
            return (double)Application.Current.Resources["SmallIconSize"];
        }
        else
        {
            return (double)Application.Current.Resources["DefaultIconSize"];
        }
    }

    private void KeyVisual_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SetEnabledState();
    }

    private void SetErrorState()
    {
        VisualStateManager.GoToState(this, IsError ? "Error" : "Default", true);
    }

    private void SetEnabledState()
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
    }
}

public enum VisualType
{
    Small,
    SmallOutline,
    TextOnly,
    Large,
}
