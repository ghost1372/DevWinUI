using Windows.System;

namespace $safeprojectname$.Views;

public sealed partial class AppUpdateSettingPage : Page
{
    public string CurrentVersion { get; set; }
    public string ChangeLog { get; set; }

    public AppUpdateSettingPage()
    {
        this.InitializeComponent();
        CurrentVersion = $"Current Version {ProcessInfoHelper.VersionWithPrefix}";$AppUpdateGetDateTime$

        BtnReleaseNote.Visibility = Visibility.Collapsed;
        BtnDownloadUpdate.Visibility = Visibility.Collapsed;
    }

    private async void CheckForUpdateAsync()
    {
        PrgLoading.IsActive = true;
        BtnCheckUpdate.IsEnabled = false;
        BtnReleaseNote.Visibility = Visibility.Collapsed;
        BtnDownloadUpdate.Visibility = Visibility.Collapsed;
        StatusCard.Header = "Checking for new version";
        if (NetworkHelper.IsNetworkAvailable())
        {
            try
            {
                //Todo: Fix UserName and Repo
                string username = "";
                string repo = "";
                TxtLastUpdateCheck.Text = DateTime.Now.ToShortDateString();$AppUpdateSetDateTime$
                var update = await UpdateHelper.CheckUpdateAsync(username, repo, new Version(ProcessInfoHelper.Version));
                if (update.StableRelease.IsExistNewVersion)
                {
                    BtnReleaseNote.Visibility = Visibility.Visible;
                    BtnDownloadUpdate.Visibility = Visibility.Visible;
                    ChangeLog = update.StableRelease.Changelog;
                    StatusCard.Header = $"We found a new version {update.StableRelease.TagName} Created at {update.StableRelease.CreatedAt} and Published at {update.StableRelease.PublishedAt}";
                }
                else if (update.PreRelease.IsExistNewVersion)
                {
                    BtnReleaseNote.Visibility = Visibility.Visible;
                    BtnDownloadUpdate.Visibility = Visibility.Visible;
                    ChangeLog = update.PreRelease.Changelog;
                    StatusCard.Header = $"We found a new version {update.PreRelease.TagName} Created at {update.PreRelease.CreatedAt} and Published at {update.PreRelease.PublishedAt}";
                }
                else
                {
                    StatusCard.Header = "You are using latest version";
                }
            }
            catch (Exception ex)
            {
                StatusCard.Header = ex.Message;
                PrgLoading.IsActive = false;
                BtnCheckUpdate.IsEnabled = true;
            }
        }
        else
        {
            StatusCard.Header = "Error Connection";
        }
        PrgLoading.IsActive = false;
        BtnCheckUpdate.IsEnabled = true;
    }

    private async void GoToUpdateAsync()
    {
        //Todo: Change Uri
        await Launcher.LaunchUriAsync(new Uri("https://github.com/Ghost1372/DevWinUI/releases"));
    }

    private async void GetReleaseNotesAsync()
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

        await dialog.ShowAsync();
    }
}


