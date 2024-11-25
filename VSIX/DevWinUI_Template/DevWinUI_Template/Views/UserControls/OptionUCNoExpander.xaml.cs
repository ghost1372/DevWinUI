using System;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace DevWinUI_Template;

public partial class OptionUCNoExpander : UserControl
{
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(OptionUCNoExpander), new PropertyMetadata(default(string)));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(OptionUCNoExpander), new PropertyMetadata(default(string)));

    public bool IsOn
    {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }

    public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(OptionUCNoExpander), new PropertyMetadata(false));

    public IconElement Icon
    {
        get { return (IconElement)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(OptionUCNoExpander), new PropertyMetadata(null));
    public string OnContent
    {
        get { return (string)GetValue(OnContentProperty); }
        set { SetValue(OnContentProperty, value); }
    }

    public static readonly DependencyProperty OnContentProperty =
        DependencyProperty.Register(nameof(OnContent), typeof(string), typeof(OptionUCNoExpander), new PropertyMetadata(default(string)));

    public string OffContent
    {
        get { return (string)GetValue(OffContentProperty); }
        set { SetValue(OffContentProperty, value); }
    }

    public static readonly DependencyProperty OffContentProperty =
        DependencyProperty.Register(nameof(OffContent), typeof(string), typeof(OptionUCNoExpander), new PropertyMetadata(default(string)));

    public FrameworkElement Item
    {
        get { return (FrameworkElement)GetValue(ItemProperty); }
        set { SetValue(ItemProperty, value); }
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(nameof(Item), typeof(FrameworkElement), typeof(OptionUCNoExpander), new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OptionUCNoExpander)d;
        if (ctl != null)
        {
            ctl.ChangeItemVisibility();
        }
    }

    public event EventHandler<RoutedEventArgs> Toggled;

    public OptionUCNoExpander()
    {
        InitializeComponent();
        Loaded += OptionUCNoExpander_Loaded;
    }

    private void ChangeItemVisibility()
    {
        if (Item == null)
        {
            tgContent.Visibility = Visibility.Collapsed;
            tgSettings.Visibility = Visibility.Visible;
        }
        else
        {
            tgSettings.Visibility = Visibility.Collapsed;
            tgContent.Visibility = Visibility.Visible;
        }
    }
    private void OptionUCNoExpander_Loaded(object sender, RoutedEventArgs e)
    {
        ChangeItemVisibility();
    }

    private void tgSettings_Checked(object sender, RoutedEventArgs e)
    {
        Toggled?.Invoke(this, e);
    }
}
