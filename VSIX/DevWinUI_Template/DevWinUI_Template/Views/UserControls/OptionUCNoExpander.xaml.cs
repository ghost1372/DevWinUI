using System;
using System.Windows;
using System.Windows.Controls;

namespace DevWinUI_Template;

public partial class OptionUCNoExpander : UserControl
{
    public event EventHandler<RoutedEventArgs> Toggled;
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(OptionUCNoExpander), new PropertyMetadata(default(string)));

    public bool IsOn
    {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }

    public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register("IsOn", typeof(bool), typeof(OptionUCNoExpander), new PropertyMetadata(false));

    public OptionUCNoExpander()
    {
        InitializeComponent();
    }

    private void tgSettings_Toggled(object sender, RoutedEventArgs e)
    {
        Toggled?.Invoke(this, e);
    }
}
