using DevWinUI_Template.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUI_Template;

public sealed class WizardConfig : INotifyPropertyChanged
{
    private static WizardConfig? _current;

    public static WizardConfig Current => _current ??= new WizardConfig();
    private Dictionary<string, string> _solutionFiles = new Dictionary<string, string>();
    public Dictionary<string, string> SolutionFiles
    {
        get => _solutionFiles;
        set => SetField(ref _solutionFiles, value);
    }
    private List<NugetPackageModel> _nugetPackages = new List<NugetPackageModel>();
    public List<NugetPackageModel> NuGetPackages
    {
        get => _nugetPackages;
        set => SetField(ref _nugetPackages, value);
    }

    private string _dotNetVersion = "net10.0";
    public string DotNetVersion
    {
        get => _dotNetVersion;
        set => SetField(ref _dotNetVersion, value);
    }

    private string _targetFrameworkVersion = "26100";
    public string TargetFrameworkVersion
    {
        get => _targetFrameworkVersion;
        set => SetField(ref _targetFrameworkVersion, value);
    }

    private bool _useJsonSettings = true;
    public bool UseJsonSettings
    {
        get => _useJsonSettings;
        set => SetField(ref _useJsonSettings, value);
    }

    private bool _enableNullableReferenceTypes = false;
    public bool EnableNullableReferenceTypes
    {
        get => _enableNullableReferenceTypes;
        set => SetField(ref _enableNullableReferenceTypes, value);
    }

    private bool _useGithubWorkflowFile = false;
    public bool UseGithubWorkflowFile
    {
        get => _useGithubWorkflowFile;
        set => SetField(ref _useGithubWorkflowFile, value);
    }

    private bool _showStartupToolWindow = true;
    public bool ShowStartupToolWindow
    {
        get => _showStartupToolWindow;
        set => SetField(ref _showStartupToolWindow, value);
    }

    private bool _useEditorConfigFile = true;
    public bool UseEditorConfigFile
    {
        get => _useEditorConfigFile;
        set => SetField(ref _useEditorConfigFile, value);
    }

    private bool _useXamlStylerFile = false;
    public bool UseXamlStylerFile
    {
        get => _useXamlStylerFile;
        set => SetField(ref _useXamlStylerFile, value);
    }

    private bool _registryVirtualizationDisabled = false;
    public bool RegistryVirtualizationDisabled
    {
        get => _registryVirtualizationDisabled;
        set => SetField(ref _registryVirtualizationDisabled, value);
    }

    private bool _fileVirtualizationDisabled = false;
    public bool FileVirtualizationDisabled
    {
        get => _fileVirtualizationDisabled;
        set => SetField(ref _fileVirtualizationDisabled, value);
    }

    private bool _useSolutionFolder = false;
    public bool UseSolutionFolder
    {
        get => _useSolutionFolder;
        private set => SetField(ref _useSolutionFolder, value);
    }

    private string _solutionFolderName = string.Empty;
    public string SolutionFolderName
    {
        get => _solutionFolderName;
        set
        {
            if (SetField(ref _solutionFolderName, value))
            {
                UseSolutionFolder = !string.IsNullOrWhiteSpace(value);
            }
        }
    }

    private string _windowsPackageMode = "MSIX";

    public string WindowsPackageMode
    {
        get => _windowsPackageMode;
        set
        {
            if (SetField(ref _windowsPackageMode, value))
            {
                OnPropertyChanged(nameof(IsPackaged));
                OnPropertyChanged(nameof(IsUnPackaged));
            }
        }
    }

    public bool IsPackaged => WindowsPackageMode.Equals("MSIX", StringComparison.OrdinalIgnoreCase);

    public bool IsUnPackaged => WindowsPackageMode.Equals("None", StringComparison.OrdinalIgnoreCase);

    public bool HasPages => UseHomeLandingPage || UseSettingsPage || UseGeneralSettingPage || UseThemeSettingPage || UseAboutPage || UseAppUpdatePage || UseStartupSetting || UseDeveloperModeSetting;

    private bool _useHomeLandingPage;
    public bool UseHomeLandingPage
    {
        get => _useHomeLandingPage;
        set
        {
            if (SetField(ref _useHomeLandingPage, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }

    private bool _useSettingsPage;
    public bool UseSettingsPage
    {
        get => _useSettingsPage;
        set
        {
            if (SetField(ref _useSettingsPage, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }

    private bool _useGeneralSettingPage;
    public bool UseGeneralSettingPage
    {
        get => _useGeneralSettingPage;
        set
        {
            if (SetField(ref _useGeneralSettingPage, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }

    private bool _useThemeSettingPage;
    public bool UseThemeSettingPage
    {
        get => _useThemeSettingPage;
        set
        {
            if (SetField(ref _useThemeSettingPage, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }

    private bool _useAboutPage;
    public bool UseAboutPage
    {
        get => _useAboutPage;
        set
        {
            if (SetField(ref _useAboutPage, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }

    private bool _useAppUpdatePage;
    public bool UseAppUpdatePage
    {
        get => _useAppUpdatePage;
        set
        {
            if (SetField(ref _useAppUpdatePage, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }

    private bool _useStartupSetting;
    public bool UseStartupSetting
    {
        get => _useStartupSetting;
        set
        {
            if (SetField(ref _useStartupSetting, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }

    private bool _useDeveloperModeSetting;
    public bool UseDeveloperModeSetting
    {
        get => _useDeveloperModeSetting;
        set
        {
            if (SetField(ref _useDeveloperModeSetting, value))
                OnPropertyChanged(nameof(HasPages));
        }
    }
    private bool _UseDebugLogger;
    public bool UseDebugLogger
    {
        get => _UseDebugLogger;
        set => SetField(ref _UseDebugLogger, value);
    }
    private bool _UseFileLogger;
    public bool UseFileLogger
    {
        get => _UseFileLogger;
        set => SetField(ref _UseFileLogger, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
