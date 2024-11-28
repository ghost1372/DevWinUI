using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Wpf.Ui.Controls;

namespace DevWinUI_Template;

[TemplatePart(Name = nameof(PART_ToggleSwitch), Type = typeof(ToggleSwitch))]
[TemplatePart(Name = nameof(PART_Content), Type = typeof(ContentControl))]
[ContentProperty(nameof(Item))]
public class SettingsControl : Control
{
    private string PART_ToggleSwitch = "PART_ToggleSwitch";
    private string PART_Content = "PART_Content";
    private ToggleSwitch _ToggleSwitch;
    private ContentControl _ContentControl;

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SettingsControl), new PropertyMetadata(default(string)));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(SettingsControl), new PropertyMetadata(default(string)));

    public bool IsOn
    {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }

    public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(SettingsControl), new PropertyMetadata(false));

    public IconElement Icon
    {
        get { return (IconElement)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(SettingsControl), new PropertyMetadata(null));
    public string OnContent
    {
        get { return (string)GetValue(OnContentProperty); }
        set { SetValue(OnContentProperty, value); }
    }

    public static readonly DependencyProperty OnContentProperty =
        DependencyProperty.Register(nameof(OnContent), typeof(string), typeof(SettingsControl), new PropertyMetadata(default(string)));

    public string OffContent
    {
        get { return (string)GetValue(OffContentProperty); }
        set { SetValue(OffContentProperty, value); }
    }

    public static readonly DependencyProperty OffContentProperty =
        DependencyProperty.Register(nameof(OffContent), typeof(string), typeof(SettingsControl), new PropertyMetadata(default(string)));

    public FrameworkElement Item
    {
        get { return (FrameworkElement)GetValue(ItemProperty); }
        set { SetValue(ItemProperty, value); }
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(nameof(Item), typeof(FrameworkElement), typeof(SettingsControl), new PropertyMetadata(null, OnItemChanged));

    private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SettingsControl)d;
        if (ctl != null)
        {
            ctl.ChangeItemVisibility();
        }
    }
    private void ChangeItemVisibility()
    {
        if (_ToggleSwitch != null && _ContentControl != null)
        {
            if (Item == null)
            {
                _ContentControl.Visibility = Visibility.Collapsed;
                _ToggleSwitch.Visibility = Visibility.Visible;
            }
            else
            {
                _ToggleSwitch.Visibility = Visibility.Collapsed;
                _ContentControl.Visibility = Visibility.Visible;
            }
        }
    }

    public event EventHandler<RoutedEventArgs> Toggled;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _ToggleSwitch = GetTemplateChild(nameof(PART_ToggleSwitch)) as ToggleSwitch;
        _ContentControl = GetTemplateChild(nameof(PART_Content)) as ContentControl;
        if (_ToggleSwitch != null)
        {
            _ToggleSwitch.Checked -= OnToggleChecked;
            _ToggleSwitch.Unchecked -= OnToggleChecked;
            _ToggleSwitch.Checked += OnToggleChecked;
            _ToggleSwitch.Unchecked += OnToggleChecked;
        }
        ChangeItemVisibility();
    }

    private void OnToggleChecked(object sender, RoutedEventArgs e)
    {
        Toggled?.Invoke(this, e);
    }
}
