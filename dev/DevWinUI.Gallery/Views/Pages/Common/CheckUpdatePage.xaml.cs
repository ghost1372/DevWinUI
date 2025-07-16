namespace DevWinUIGallery.Views;

public sealed partial class CheckUpdatePage : Page
{
    public CheckUpdatePage()
    {
        this.InitializeComponent();
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtRepo.Text))
        {
            txtUser.Text = "Ghost1372";
            txtRepo.Text = "DevWinUI";
        }
        var ver = await UpdateHelper.CheckUpdateAsync(txtUser.Text, txtRepo.Text);
        if (ver.StableRelease.IsExistNewVersion)
        {
            foreach (var item in ver.StableRelease.Assets)
            {
                listView.Items.Add($"{item.Url}{Environment.NewLine}Size: {item.Size}");
            }
            txtChangelog.Text = $"Changelog: {ver.StableRelease.Changelog}";

            txtReleaseUrl.Text = $"Release Url: {ver.StableRelease.HtmlUrl}";
            txtCreatedAt.Text = $"Created At: {ver.StableRelease.CreatedAt}";
            txtPublishedAt.Text = $"Published At {ver.StableRelease.PublishedAt}";
            txtIsPreRelease.Text = $"Is PreRelease: {ver.StableRelease.IsPreRelease}";
            txtTagName.Text = $"Tag Name: {ver.StableRelease.TagName}";
        }
        else if (ver.PreRelease.IsExistNewVersion)
        {
            foreach (var item in ver.PreRelease.Assets)
            {
                listView.Items.Add($"{item.Url}{Environment.NewLine}Size: {item.Size}");
            }
            txtChangelog.Text = $"Changelog: {ver.PreRelease.Changelog}";

            txtReleaseUrl.Text = $"Release Url: {ver.PreRelease.HtmlUrl}";
            txtCreatedAt.Text = $"Created At: {ver.PreRelease.CreatedAt}";
            txtPublishedAt.Text = $"Published At {ver.PreRelease.PublishedAt}";
            txtIsPreRelease.Text = $"Is PreRelease: {ver.PreRelease.IsPreRelease}";
            txtTagName.Text = $"Tag Name: {ver.PreRelease.TagName}";
        }
        else
        {
            listView.Items.Clear();
            var noUpdate = "There is no new version available.";
            txtChangelog.Text = noUpdate;
            txtReleaseUrl.Text = noUpdate;
            txtCreatedAt.Text = noUpdate;
            txtPublishedAt.Text = noUpdate;
            txtIsPreRelease.Text = noUpdate;
            txtTagName.Text = noUpdate;
        }
    }
}
