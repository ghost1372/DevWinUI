using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Wpf.Ui.Controls;

namespace DevWinUI_Template;
[TemplatePart(Name = nameof(PART_ToggleSwitch), Type = typeof(ToggleSwitch))]
[ContentProperty(nameof(Item))]
public class SettingsControlWithExpander : Control
{
    private string PART_ToggleSwitch = "PART_ToggleSwitch";
    private ToggleSwitch _ToggleSwitch;

    public bool IsExpanded
    {
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(SettingsControlWithExpander), new PropertyMetadata(false));

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SettingsControlWithExpander), new PropertyMetadata(default(string)));

    public string Description
    {
        get { return (string)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(SettingsControlWithExpander), new PropertyMetadata(default(string)));

    public bool IsOn
    {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }

    public static readonly DependencyProperty IsOnProperty =
        DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(SettingsControlWithExpander), new PropertyMetadata(false));

    public IconElement Icon
    {
        get { return (IconElement)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(SettingsControlWithExpander), new PropertyMetadata(null));
    public string OnContent
    {
        get { return (string)GetValue(OnContentProperty); }
        set { SetValue(OnContentProperty, value); }
    }

    public static readonly DependencyProperty OnContentProperty =
        DependencyProperty.Register(nameof(OnContent), typeof(string), typeof(SettingsControlWithExpander), new PropertyMetadata(default(string)));

    public string OffContent
    {
        get { return (string)GetValue(OffContentProperty); }
        set { SetValue(OffContentProperty, value); }
    }

    public static readonly DependencyProperty OffContentProperty =
        DependencyProperty.Register(nameof(OffContent), typeof(string), typeof(SettingsControlWithExpander), new PropertyMetadata(default(string)));

    public FrameworkElement Item
    {
        get { return (FrameworkElement)GetValue(ItemProperty); }
        set { SetValue(ItemProperty, value); }
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(nameof(Item), typeof(FrameworkElement), typeof(SettingsControlWithExpander), new PropertyMetadata(null));

    public event EventHandler<RoutedEventArgs> Toggled;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _ToggleSwitch = GetTemplateChild(nameof(PART_ToggleSwitch)) as ToggleSwitch;
        if (_ToggleSwitch != null)
        {
            _ToggleSwitch.Checked -= OnToggleChecked;
            _ToggleSwitch.Unchecked -= OnToggleChecked;
            _ToggleSwitch.Checked += OnToggleChecked;
            _ToggleSwitch.Unchecked += OnToggleChecked;
        }
    }

    private void OnToggleChecked(object sender, RoutedEventArgs e)
    {
        Toggled?.Invoke(this, e);
    }
}
