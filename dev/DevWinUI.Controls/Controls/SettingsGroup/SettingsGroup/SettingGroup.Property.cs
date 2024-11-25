using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DevWinUI;
public partial class SettingsGroup : Control
{
    internal CornerRadius RootGridCornerRadius
    {
        get { return (CornerRadius)GetValue(RootGridCornerRadiusProperty); }
        set { SetValue(RootGridCornerRadiusProperty, value); }
    }

    internal static readonly DependencyProperty RootGridCornerRadiusProperty =
        DependencyProperty.Register(nameof(RootGridCornerRadius), typeof(CornerRadius), typeof(SettingsGroup), new PropertyMetadata(new CornerRadius(0)));
    private CornerRadius GetRootGridCornerRadius()
    {
        return (CornerRadius)new Bool2CornerRadiusConverter().Convert(Items?.Count, null, null, null);
    }
    public ObservableCollection<object> Items
    {
        get { return (ObservableCollection<object>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<object>), typeof(SettingsGroup), new PropertyMetadata(null, OnItemsConnectedPropertyChanged));

    private static void OnItemsConnectedPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is SettingsGroup settingGroup)
        {
            settingGroup.OnItemsChanged();
        }
    }

    private void OnItemsChanged()
    {
        if (_itemsRepeater is not null)
        {
            RootGridCornerRadius = GetRootGridCornerRadius();
            if (_rootGrid != null)
            {
                _rootGrid.CornerRadius = GetRootGridCornerRadius();
            }

            foreach (var item in Items)
            {
                if (item != null && (item is SettingsCard || item is SettingsExpander))
                {
                    ChangeBorderAndCornerRadius(item);
                }
            }

            _itemsRepeater.ItemsSource = Items;
        }
    }

    private void ChangeBorderAndCornerRadius(object item)
    {
        if (item is SettingsCard settingsCard)
        {
            settingsCard.Padding = new Thickness(25, 0, 25, 0);
            settingsCard.CornerRadius = new CornerRadius(0);
            settingsCard.BorderThickness = new Thickness(1, 1, 1, 0);
        }
        else if (item is SettingsExpander settingsExpander)
        {
            settingsExpander.Padding = new Thickness(25, 0, 25, 0);
            settingsExpander.CornerRadius = new CornerRadius(0);
            settingsExpander.BorderThickness = new Thickness(1, 1, 1, 0);
        }
        
        var firstItem = Items.FirstOrDefault();

        if (firstItem != null)
        {
            if (firstItem is SettingsCard firstSettingsCard)
            {
                firstSettingsCard.BorderThickness = new Thickness(1, 0, 1, 0);
            }
            else if (firstItem is SettingsExpander firstSettingsExpander)
            {
                firstSettingsExpander.BorderThickness = new Thickness(1, 0, 1, 0);
            }
        }

        var lastItem = Items.LastOrDefault();

        if (lastItem != null && Items.Count == 1)
        {
            if (lastItem is SettingsCard lastSettingsCard)
            {
                lastSettingsCard.CornerRadius = new CornerRadius(0, 0, 4, 4);
                lastSettingsCard.BorderThickness = new Thickness(1, 0, 1, 1);
            }
            else if (lastItem is SettingsExpander lastSettingsExpander)
            {
                lastSettingsExpander.CornerRadius = new CornerRadius(0, 0, 4, 4);
                lastSettingsExpander.BorderThickness = new Thickness(1, 0, 1, 1);
            }
        }
        else if (lastItem != null)
        {
            if (lastItem is SettingsCard lastSettingsCard)
            {
                lastSettingsCard.CornerRadius = new CornerRadius(0, 0, 4, 4);
                lastSettingsCard.BorderThickness = new Thickness(1);
            }
            else if (lastItem is SettingsExpander lastSettingsExpander)
            {
                lastSettingsExpander.CornerRadius = new CornerRadius(0, 0, 4, 4);
                lastSettingsExpander.BorderThickness = new Thickness(1);
            }
        }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(SettingsGroup), new PropertyMetadata(default(object), OnContentPropertyChanged));

    public object Content
    {
        get => (object)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SettingsGroup)d;
        if (ctl == null)
        {
            return;
        }
        ctl.OnContentChanged();
    }

    private void OnContentChanged()
    {
        if (_contentPresenter != null)
        {
            _contentPresenter.Visibility = Content == null ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public static readonly DependencyProperty HeaderIconProperty =
        DependencyProperty.Register(nameof(HeaderIcon), typeof(IconElement), typeof(SettingsGroup), new PropertyMetadata(defaultValue: null, (d, e) => ((SettingsGroup)d).OnHeaderIconPropertyChanged((IconElement)e.OldValue, (IconElement)e.NewValue)));

    public IconElement HeaderIcon
    {
        get => (IconElement)GetValue(HeaderIconProperty);
        set => SetValue(HeaderIconProperty, value);
    }

    protected virtual void OnHeaderIconPropertyChanged(IconElement oldValue, IconElement newValue)
    {
        OnHeaderIconChanged();
    }

    private void OnHeaderIconChanged()
    {
        if (GetTemplateChild(PartHeaderIconPresenter) is FrameworkElement headerIconPresenter)
        {
            headerIconPresenter.Visibility = HeaderIcon != null
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(SettingsGroup), new PropertyMetadata(null, OnHeaderPropertyChanged));

    [Localizable(true)]
    public object Header
    {
        get => (object)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SettingsGroup)d;
        if (ctl == null)
        {
            return;
        }
        ctl.OnHeaderChanged();
    }

    private void OnHeaderChanged()
    {
        if (_headerPresenter != null)
        {
            _headerPresenter.Visibility = Header == null ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(object), typeof(SettingsGroup), new PropertyMetadata(null, OnDescriptionPropertyChanged));

    [Localizable(true)]
    public object Description
    {
        get => (object)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    private static void OnDescriptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SettingsGroup)d;

        if (ctl == null)
        {
            return;
        }
        ctl.OnDescriptionChanged();
    }

    private void OnDescriptionChanged()
    {
        if (_descriptionPresenter != null)
        {
            _descriptionPresenter.Visibility = Description == null ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
