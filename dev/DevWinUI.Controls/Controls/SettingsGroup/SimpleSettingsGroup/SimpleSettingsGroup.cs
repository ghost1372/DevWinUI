using System.ComponentModel;
using Microsoft.UI.Xaml.Automation.Peers;

namespace DevWinUI;

/// <summary>
/// Represents a control that can contain multiple settings (or other) controls
/// </summary>
[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = PartDescriptionPresenter, Type = typeof(ContentPresenter))]
public partial class SimpleSettingsGroup : ItemsControl
{
    private const string PartDescriptionPresenter = "DescriptionPresenter";
    private ContentPresenter _descriptionPresenter;
    private SimpleSettingsGroup _SimpleSettingsGroup;

    public SimpleSettingsGroup()
    {
        DefaultStyleKey = typeof(SimpleSettingsGroup);
    }

    [Localizable(true)]
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        "Header",
        typeof(string),
        typeof(SimpleSettingsGroup),
        new PropertyMetadata(default(string)));

    [Localizable(true)]
    public object Description
    {
        get => (object)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        "Description",
        typeof(object),
        typeof(SimpleSettingsGroup),
        new PropertyMetadata(null, OnDescriptionChanged));

    protected override void OnApplyTemplate()
    {
        IsEnabledChanged -= SimpleSettingsGroup_IsEnabledChanged;
        _SimpleSettingsGroup = (SimpleSettingsGroup)this;
        _descriptionPresenter = (ContentPresenter)_SimpleSettingsGroup.GetTemplateChild(PartDescriptionPresenter);
        SetEnabledState();
        IsEnabledChanged += SimpleSettingsGroup_IsEnabledChanged;
        base.OnApplyTemplate();
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((SimpleSettingsGroup)d).Update();
    }

    private void SimpleSettingsGroup_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SetEnabledState();
    }

    private void SetEnabledState()
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
    }

    private void Update()
    {
        if (_SimpleSettingsGroup == null)
        {
            return;
        }

        _SimpleSettingsGroup._descriptionPresenter.Visibility = _SimpleSettingsGroup.Description == null ? Visibility.Collapsed : Visibility.Visible;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SimpleSettingsGroupAutomationPeer(this);
    }
}
