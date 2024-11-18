using Windows.System;

namespace $safeprojectname$.ViewModels;
public partial class AppUpdateSettingViewModel : ObservableObject
{
    [ObservableProperty]
    public string currentVersion;

    [ObservableProperty]
    public string lastUpdateCheck;

    [ObservableProperty]
    public bool isUpdateAvailable;

    [ObservableProperty]
    public bool isLoading;

    [ObservableProperty]
    public bool isCheckButtonEnabled = true;

    [ObservableProperty]
    public string loadingStatus = "Status";

    private string ChangeLog = string.Empty;

    public AppUpdateSettingViewModel()
    {
        CurrentVersion = $"Current Version {ProcessInfoHelper.VersionWithPrefix}";$AppUpdateMVVMGetDateTime$
    }

    [RelayCommand]
    private async Task CheckForUpdateAsync()
    {
        IsLoading = true;
        IsUpdateAvailable = false;
        IsCheckButtonEnabled = false;
        LoadingStatus = "Checking for new version";
        if (NetworkHelper.IsNetworkAvailable())
        {
            try
            {
                //Todo: Fix UserName and Repo
                string username = "";
                string repo = "";
                LastUpdateCheck = DateTime.Now.ToShortDateString();$AppUpdateMVVMSetDateTime$
                var update = await UpdateHelper.CheckUpdateAsync(username, repo, new Version(ProcessInfoHelper.Version));
                if (update.IsExistNewVersion)
                {
                    IsUpdateAvailable = true;
                    ChangeLog = update.Changelog;
                    LoadingStatus = $"We found a new version {update.TagName} Created at {update.CreatedAt} and Published at {update.PublishedAt}";
                }
                else
                {
                    LoadingStatus = "You are using latest version";
                }
            }
            catch (Exception ex)
            {
                LoadingStatus = ex.Message;
                IsLoading = false;
                IsCheckButtonEnabled = true;
            }
        }
        else
        {
            LoadingStatus = "Error Connection";
        }
        IsLoading = false;
        IsCheckButtonEnabled = true;
    }

    [RelayCommand]
    private async Task GoToUpdateAsync()
    {
        //Todo: Change Uri
        await Launcher.LaunchUriAsync(new Uri("https://github.com/Ghost1372/DevWinUI/releases"));
    }

    [RelayCommand]
    private async Task GetReleaseNotesAsync()
    {
        ContentDialog dialog = new ContentDialog()
        {
            Title = "Release Note",
            CloseButtonText = "Close",
            Content = new ScrollViewer
            {
                Content = new TextBlock
                {
                    Text = ChangeLog,
                    Margin = new Thickness(10)
                },
                Margin = new Thickness(10)
            },
            Margin = new Thickness(10),
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        await dialog.ShowAsyncQueue();
    }
}
