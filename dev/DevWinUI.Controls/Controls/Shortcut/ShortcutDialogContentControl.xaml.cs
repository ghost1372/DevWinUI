// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace DevWinUI;

public sealed partial class ShortcutDialogContentControl : UserControl
{
    public ShortcutDialogContentControl()
    {
        InitializeComponent();
    }

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ShortcutDialogContentControl), new PropertyMetadata("Press a combination of keys to change this shortcut"));


    public List<object> Keys
    {
        get { return (List<object>)GetValue(KeysProperty); }
        set { SetValue(KeysProperty, value); }
    }

    public static readonly DependencyProperty KeysProperty =
        DependencyProperty.Register(nameof(Keys), typeof(List<object>), typeof(ShortcutDialogContentControl), new PropertyMetadata(default(string)));

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public static readonly DependencyProperty IsErrorProperty =
        DependencyProperty.Register(nameof(IsError), typeof(bool), typeof(ShortcutDialogContentControl), new PropertyMetadata(false));

    public string ErrorTitle
    {
        get { return (string)GetValue(ErrorTitleProperty); }
        set { SetValue(ErrorTitleProperty, value); }
    }

    public static readonly DependencyProperty ErrorTitleProperty =
        DependencyProperty.Register(nameof(ErrorTitle), typeof(string), typeof(ShortcutDialogContentControl), new PropertyMetadata("Invalid shortcut"));
    public string ErrorToolTip
    {
        get { return (string)GetValue(ErrorToolTipProperty); }
        set { SetValue(ErrorToolTipProperty, value); }
    }

    public static readonly DependencyProperty ErrorToolTipProperty =
        DependencyProperty.Register(nameof(ErrorToolTip), typeof(string), typeof(ShortcutDialogContentControl), new PropertyMetadata(null));

    public bool IsWarning
    {
        get { return (bool)GetValue(IsWarningProperty); }
        set { SetValue(IsWarningProperty, value); }
    }

    public static readonly DependencyProperty IsWarningProperty =
        DependencyProperty.Register(nameof(IsWarning), typeof(bool), typeof(ShortcutDialogContentControl), new PropertyMetadata(false));

    public string WarningTitle
    {
        get { return (string)GetValue(WarningTitleProperty); }
        set { SetValue(WarningTitleProperty, value); }
    }

    public static readonly DependencyProperty WarningTitleProperty =
        DependencyProperty.Register(nameof(WarningTitle), typeof(string), typeof(ShortcutDialogContentControl), new PropertyMetadata("Possible shortcut interference with Alt Gr"));

    public string WarningToolTip
    {
        get { return (string)GetValue(WarningToolTipProperty); }
        set { SetValue(WarningToolTipProperty, value); }
    }

    public static readonly DependencyProperty WarningToolTipProperty =
        DependencyProperty.Register(nameof(WarningToolTip), typeof(string), typeof(ShortcutDialogContentControl), new PropertyMetadata("Shortcuts with **Ctrl** and **Alt** may remove functionality from some international keyboards, because **Ctrl** + **Alt** = **Alt Gr** in those keyboards."));
}
