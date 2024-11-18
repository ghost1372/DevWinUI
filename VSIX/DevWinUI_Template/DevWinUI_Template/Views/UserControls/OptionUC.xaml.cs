using System;
using System.Windows;
using System.Windows.Controls;

namespace DevWinUI_Template;

public partial class OptionUC : UserControl
{
    public event EventHandler<RoutedEventArgs> Toggled;
    public string InfoBarMessage
    {
        get { return (string)GetValue(InfoBarMessageProperty); }
        set { SetValue(InfoBarMessageProperty, value); }
    }

    public static readonly DependencyProperty InfoBarMessageProperty =
        DependencyProperty.Register("InfoBarMessage", typeof(string), typeof(OptionUC), new PropertyMetadata(default(string)));

    public string InfoBarTitle
    {
        get { return (string)GetValue(InfoBarTitleProperty); }
        set { SetValue(InfoBarTitleProperty, value); }
    }

    public static readonly DependencyProperty InfoBarTitleProperty =
        DependencyProperty.Register("InfoBarTitle", typeof(string), typeof(OptionUC), new PropertyMetadata(default(string)));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(OptionUC), new PropertyMetadata(default(string)));

    public bool IsOn
    {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }

    public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register("IsOn", typeof(bool), typeof(OptionUC), new PropertyMetadata(false));

    public string OnContent
    {
        get { return (string)GetValue(OnContentProperty); }
        set { SetValue(OnContentProperty, value); }
    }

    public static readonly DependencyProperty OnContentProperty =
        DependencyProperty.Register("OnContent", typeof(string), typeof(OptionUC), new PropertyMetadata(null));

    public string OffContent
    {
        get { return (string)GetValue(OffContentProperty); }
        set { SetValue(OffContentProperty, value); }
    }

    public static readonly DependencyProperty OffContentProperty =
        DependencyProperty.Register("OffContent", typeof(string), typeof(OptionUC), new PropertyMetadata(null));

    public OptionUC()
    {
        InitializeComponent();
    }

    private void tgSettings_Toggled(object sender, RoutedEventArgs e)
    {
        Toggled?.Invoke(this, e);
    }
}
