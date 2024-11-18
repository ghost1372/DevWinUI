using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
[TemplatePart(Name = PartDescriptionPresenter, Type = typeof(ContentPresenter))]
[ContentProperty(Name = nameof(Content))]
public partial class SettingsGroup : Control
{
    private const string PART_ItemsRepeater = "PART_ItemsRepeater";

    private ItemsRepeater? _itemsRepeater;
    private const string PartContentPresenter = "PART_ContentPresenter";
    private const string PartHeaderIconPresenter = "PART_HeaderIconPresenterHolder";
    private const string PartHeaderPresenter = "PART_HeaderPresenter";
    private const string PartDescriptionPresenter = "PART_DescriptionPresenter";
    private const string PartRootGrid = "PART_RootGrid";
    private ContentPresenter _contentPresenter;
    private ContentPresenter _descriptionPresenter;
    private ContentPresenter _headerPresenter;
    private Grid _rootGrid;
    private SettingsGroup _settingsGroup;
    
    public SettingsGroup()
    {
        DefaultStyleKey = typeof(SettingsGroup);
        Items = new ObservableCollection<object>();
        Items.CollectionChanged += Items_CollectionChanged;
    }

    private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnItemsChanged();
    }

    protected override void OnApplyTemplate()
    {
        _settingsGroup = (SettingsGroup)this;

        _itemsRepeater = GetTemplateChild(PART_ItemsRepeater) as ItemsRepeater;
        _contentPresenter = (ContentPresenter)_settingsGroup.GetTemplateChild(PartContentPresenter);
        _headerPresenter = (ContentPresenter)_settingsGroup.GetTemplateChild(PartHeaderPresenter);
        _descriptionPresenter = (ContentPresenter)_settingsGroup.GetTemplateChild(PartDescriptionPresenter);
        _rootGrid = (Grid)_settingsGroup.GetTemplateChild(PartRootGrid);

        if (_itemsRepeater != null)
        {
            OnItemsConnectedPropertyChanged(this, null!);
        }

        IsEnabledChanged -= SettingsGroup_IsEnabledChanged;
        SetEnabledState();
        IsEnabledChanged += SettingsGroup_IsEnabledChanged;
        OnHeaderIconChanged();
        OnHeaderChanged();
        OnDescriptionChanged();
        OnContentChanged();
        base.OnApplyTemplate();
    }

    private void SettingsGroup_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        SetEnabledState();
    }

    private void SetEnabledState()
    {
        VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
    }
}
