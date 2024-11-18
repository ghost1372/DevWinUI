using System.Text.Json;

namespace DevWinUI;

public static partial class UpdateHelper
{
    private const string GITHUB_API = "https://api.github.com/repos/{0}/{1}/releases/latest";

    public static async Task<UpdateInfo> CheckUpdateAsync(string username, string repository, Version currentVersion = null)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));

        if (string.IsNullOrEmpty(repository))
            throw new ArgumentNullException(nameof(repository));

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", username);
        var url = string.Format(GITHUB_API, username, repository);
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UpdateInfo>(responseBody, UpdateHelperJsonContext.Default.UpdateInfo);

        if (result != null)
        {
            if (currentVersion == null)
            {
                currentVersion = ProcessInfoHelper.GetVersion();
            }

            var newVersionInfo = GetAsVersionInfo(result.TagName);
            var major = currentVersion.Major == -1 ? 0 : currentVersion.Major;
            var minor = currentVersion.Minor == -1 ? 0 : currentVersion.Minor;
            var build = currentVersion.Build == -1 ? 0 : currentVersion.Build;
            var revision = currentVersion.Revision == -1 ? 0 : currentVersion.Revision;

            var currentVersionInfo = new SystemVersionInfo(major, minor, build, revision);

            return new UpdateInfo
            {
                Changelog = result?.Changelog,
                CreatedAt = Convert.ToDateTime(result?.CreatedAt),
                Assets = result?.Assets,
                IsPreRelease = result.IsPreRelease,
                PublishedAt = Convert.ToDateTime(result?.PublishedAt),
                TagName = result?.TagName,
                AssetsUrl = result?.AssetsUrl,
                Author = result?.Author,
                HtmlUrl = result?.HtmlUrl,
                Name = result?.Name,
                TarballUrl = result?.TarballUrl,
                TargetCommitish = result?.TargetCommitish,
                UploadUrl = result?.UploadUrl,
                Url = result?.Url,
                ZipballUrl = result?.ZipballUrl,
                IsExistNewVersion = newVersionInfo > currentVersionInfo
            };
        }

        return null;
    }

    private static SystemVersionInfo GetAsVersionInfo(string version)
    {
        var nums = GetVersionNumbers(version).Split('.').Select(int.Parse).ToList();

        return nums.Count <= 1
            ? new SystemVersionInfo(nums[0], 0, 0, 0)
            : nums.Count <= 2
            ? new SystemVersionInfo(nums[0], nums[1], 0, 0)
            : nums.Count <= 3
            ? new SystemVersionInfo(nums[0], nums[1], nums[2], 0)
            : new SystemVersionInfo(nums[0], nums[1], nums[2], nums[3]);
    }

    private static string GetVersionNumbers(string version)
    {
        var allowedChars = "01234567890.";
        return new string(version.Where(c => allowedChars.Contains(c)).ToArray());
    }
}
