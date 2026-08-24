using DevWinUI_Template.Models;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace DevWinUI_Template;

public partial class MainWindow : FluentWindow, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly Dictionary<Type, object> _pageCache = new();
    private object? _currentPage;
    public object? CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public ObservableCollection<NavigationMenuModel> MenuItems { get; } = new ObservableCollection<NavigationMenuModel>
    {
        new NavigationMenuModel { StepNumber = 1, Name = "Project Info", Title = "Basic Information", PageType = typeof(ProjectInfoPage) },
        new NavigationMenuModel { StepNumber = 2, Name = "Framework", Title = "Choose .Net Version", PageType = typeof(FrameworkPage) },
        new NavigationMenuModel { StepNumber = 3, Name = "Package Mode", Title = "Select Packaging Option", PageType = typeof(PackageModePage) },
        new NavigationMenuModel { StepNumber = 4, Name = "Pages", Title = "Select Pages/Views", PageType = typeof(PagesPage) },
        new NavigationMenuModel { StepNumber = 5, Name = "Packages", Title = "Add NuGet Packages", PageType = typeof(NugetPackagePage) },
    };

    public MainWindow()
    {
        AddDictionary("pack://application:,,,/Wpf.Ui;component/Resources/Theme/Dark.xaml");
        AddDictionary("/DevWinUI_Template;component/Theme/Generic.xaml");
        AddDictionary("/DevWinUI_Template;component/Theme/TextBlockStyle.xaml");
        AddDictionary("/DevWinUI_Template;component/Theme/Controls/Card.xaml");
        AddDictionary("/DevWinUI_Template;component/Theme/Controls/CheckCard.xaml");
        AddDictionary("/DevWinUI_Template;component/Theme/Controls/RadioCard.xaml");
        AddDictionary("/DevWinUI_Template;component/Theme/Controls/TileCard.xaml");
        AddDictionary("/DevWinUI_Template;component/Theme/Controls/Styles.xaml");

        SystemThemeWatcher.Watch(this);
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);

        ApplicationAccentColorManager.Apply(Color.FromArgb(255, 185, 140, 255));

        InitializeComponent();

        MainLV.ItemsSource = MenuItems;

        MainLV.SelectionChanged += (_, _) =>
        {
            UpdateStepStates();
            LoadCachedPage();
            UpdateNavigationButtons();
        };

        MainLV.SelectedIndex = 0;

        UpdateStepStates();
    }
    private void AddDictionary(string path)
    {
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new System.Uri(path, UriKind.RelativeOrAbsolute)
        });
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (DialogResult.HasValue && DialogResult.Value)
        {
        }
        else
        {
            Cancel();
        }
    }

    private void btnCreate_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;

        Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        Cancel();
    }

    private void Cancel()
    {
        Resources?.Clear();
        Application.Current?.Resources?.Clear();
        DialogResult = false;
    }

    private void LoadCachedPage()
    {
        if (MainLV.SelectedItem is not NavigationMenuModel selected || selected.PageType is null)
        {
            CurrentPage = null;
            return;
        }

        if (!_pageCache.TryGetValue(selected.PageType, out var cachedPage))
        {
            try
            {
                cachedPage = Activator.CreateInstance(selected.PageType);
                _pageCache[selected.PageType] = cachedPage;
            }
            catch
            {
                CurrentPage = null;
                return;
            }
        }

        CurrentPage = cachedPage;
    }
    private void UpdateStepStates()
    {
        if (MainLV.SelectedItem is not NavigationMenuModel selected)
        {
            return;
        }

        foreach (var item in MenuItems)
        {
            item.IsCurrent = ReferenceEquals(item, selected);
            item.IsCompleted = item.StepNumber < selected.StepNumber;
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        Cancel();
        Close();
        throw new WizardCancelledException();
    }
    private void UpdateNavigationButtons()
    {
        BtnPrev.IsEnabled = MainLV.SelectedIndex > 0;
        BtnNext.IsEnabled =
            MainLV.SelectedIndex >= 0 &&
            MainLV.SelectedIndex < MainLV.Items.Count - 1;
    }
    private void BtnPrev_Click(object sender, RoutedEventArgs e)
    {
        if (MainLV.SelectedIndex > 0)
        {
            MainLV.SelectedIndex--;
            MainLV.ScrollIntoView(MainLV.SelectedItem);
        }

        UpdateNavigationButtons();
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
        if (MainLV.SelectedIndex < MainLV.Items.Count - 1)
        {
            MainLV.SelectedIndex++;
            MainLV.ScrollIntoView(MainLV.SelectedItem);
        }

        UpdateNavigationButtons();
    }

    private void BtnCreate_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
