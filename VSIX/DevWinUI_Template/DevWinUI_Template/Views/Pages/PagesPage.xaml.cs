using DevWinUI_Template.Common;
using DevWinUI_Template.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace DevWinUI_Template;
public partial class PagesPage : Page
{
    public ObservableCollection<CheckCardModel> CheckCardItems { get; } = new ObservableCollection<CheckCardModel>
    {
        new CheckCardModel { Value = nameof(WizardConfig.UseHomeLandingPage), Title = "Add Home Page", Subtitle = "Will create a landing page for your application's home screen.", Icon = new SymbolIcon(){ Symbol = SymbolRegular.ChannelAdd24, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
        new CheckCardModel { Value = nameof(WizardConfig.UseSettingsPage), Title = "Add Setting Page", Subtitle = "Will create a settings page with a BreadCrumbBar for your application", Icon = new SymbolIcon(){ Symbol = SymbolRegular.CubeAdd20, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
        new CheckCardModel { Value = nameof(WizardConfig.UseGeneralSettingPage), Title = "Add General Setting Page", Subtitle = "Will create an empty general settings page within your application.", Icon = new SymbolIcon(){ Symbol = SymbolRegular.AppGeneric24, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
        new CheckCardModel { Value = nameof(WizardConfig.UseStartupSetting), Title = "Add Startup app Setting Option", Subtitle = "Will create a Startup app setting in General page", Icon = new SymbolIcon(){ Symbol = SymbolRegular.Window24, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
        new CheckCardModel { Value = nameof(WizardConfig.UseDeveloperModeSetting), Title = "Add Developer Mode Setting Option", Subtitle = "Will create a developer mode setting in General page", Icon = new SymbolIcon(){ Symbol = SymbolRegular.DeveloperBoard24, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
        new CheckCardModel { Value = nameof(WizardConfig.UseThemeSettingPage), Title = "Add Theme Setting Page", Subtitle = "Will create a theme settings page", Icon = new SymbolIcon(){ Symbol = SymbolRegular.DarkTheme24, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
        new CheckCardModel { Value = nameof(WizardConfig.UseAppUpdatePage), Title = "Add App Update Page", Subtitle = "Will create an App Update page that user check for updates", Icon = new SymbolIcon(){ Symbol = SymbolRegular.PhoneUpdate24, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
        new CheckCardModel { Value = nameof(WizardConfig.UseAboutPage), Title = "Add About Page", Subtitle = "By activating this option, your application will include an 'About' page.", Icon = new SymbolIcon(){ Symbol = SymbolRegular.AppsListDetail24, FontSize = 24, Margin = new Thickness(0,0,10,0) } },
    };
    public PagesPage()
    {
        InitializeComponent();

        DataContext = WizardConfig.Current;

        CheckLV.ItemsSource = CheckCardItems;
        CheckLV.SelectedValuePath = "Value";

        UpdateCardStates();
        Helper.UpdateCornerRadius(CheckLV);

        PreviewMouseWheel += PagesPage_PreviewMouseWheel;
    }

    private void CheckLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        var opts = WizardConfig.Current;
        var selectedValues = CheckLV.SelectedItems.Cast<CheckCardModel>().Where(i => i.IsEnabled).Select(i => i.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

        opts.UseHomeLandingPage = selectedValues.Contains(nameof(WizardConfig.UseHomeLandingPage));
        opts.UseSettingsPage = selectedValues.Contains(nameof(WizardConfig.UseSettingsPage));
        opts.UseGeneralSettingPage = selectedValues.Contains(nameof(WizardConfig.UseGeneralSettingPage));
        opts.UseStartupSetting = selectedValues.Contains(nameof(WizardConfig.UseStartupSetting));
        opts.UseDeveloperModeSetting = selectedValues.Contains(nameof(WizardConfig.UseDeveloperModeSetting));
        opts.UseThemeSettingPage = selectedValues.Contains(nameof(WizardConfig.UseThemeSettingPage));
        opts.UseAppUpdatePage = selectedValues.Contains(nameof(WizardConfig.UseAppUpdatePage));
        opts.UseAboutPage = selectedValues.Contains(nameof(WizardConfig.UseAboutPage));

        UpdateCardStates();

        Helper.UpdateActiveCornerRadius(CheckLV);
    }

    private void UpdateCardStates()
    {
        var opts = WizardConfig.Current;

        var hasSettingPage = opts.UseSettingsPage;
        var hasGeneralPage = opts.UseGeneralSettingPage;

        SetCardEnabled(nameof(WizardConfig.UseHomeLandingPage), true);
        SetCardEnabled(nameof(WizardConfig.UseSettingsPage), true);
        SetCardEnabled(nameof(WizardConfig.UseGeneralSettingPage), hasSettingPage);
        SetCardEnabled(nameof(WizardConfig.UseStartupSetting), hasSettingPage && hasGeneralPage);
        SetCardEnabled(nameof(WizardConfig.UseDeveloperModeSetting), hasSettingPage && hasGeneralPage);
        SetCardEnabled(nameof(WizardConfig.UseThemeSettingPage), hasSettingPage);
        SetCardEnabled(nameof(WizardConfig.UseAppUpdatePage), hasSettingPage);
        SetCardEnabled(nameof(WizardConfig.UseAboutPage), hasSettingPage);

        if (!hasSettingPage)
        {
            opts.UseGeneralSettingPage = false;
            opts.UseStartupSetting = false;
            opts.UseDeveloperModeSetting = false;
            opts.UseThemeSettingPage = false;
            opts.UseAppUpdatePage = false;
            opts.UseAboutPage = false;
        }
        else if (!hasGeneralPage)
        {
            opts.UseStartupSetting = false;
            opts.UseDeveloperModeSetting = false;
        }

        var disabledItems = CheckCardItems.Where(x => !x.IsEnabled).ToList();
        foreach (var item in disabledItems)
        {
            if (CheckLV.SelectedItems.Contains(item))
            {
                CheckLV.SelectedItems.Remove(item);
            }
        }
    }

    private void SetCardEnabled(string value, bool enabled)
    {
        var item = CheckCardItems.FirstOrDefault(x => string.Equals(x.Value, value, StringComparison.OrdinalIgnoreCase));
        if (item is null) return;

        item.IsEnabled = enabled;

        if (!enabled && CheckLV.SelectedItems.Contains(item))
        {
            CheckLV.SelectedItems.Remove(item);
        }
    }

    private void PagesPage_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!(e.OriginalSource is DependencyObject source)) return;

        var parent = source as DependencyObject;
        while (parent != null && !(parent is ScrollViewer))
        {
            parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
        }

        if (parent is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }

    private void CheckLV_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
        var scrollViewer = FindParent<ScrollViewer>(CheckLV);
        if (scrollViewer != null)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        }
    }

    private static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
        if (parent == null) return null;
        return parent is T p ? p : FindParent<T>(parent);
    }
}
