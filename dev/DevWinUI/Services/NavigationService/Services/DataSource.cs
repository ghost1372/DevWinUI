using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DevWinUI;

public class Root
{
    public ObservableCollection<DataGroup> Groups { get; set; }
}
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Root))]
internal partial class RootContext : JsonSerializerContext
{
}

public sealed class DataSource
{
    private string _jsonFilePath;
    private PathType _pathType;

    private static readonly object _lock = new();

    #region Singleton

    private static readonly DataSource _instance;

    public static DataSource Instance
    {
        get
        {
            return _instance;
        }
    }

    static DataSource()
    {
        _instance = new DataSource();
    }

    private DataSource() { }

    #endregion

    private readonly IList<DataGroup> _groups = new List<DataGroup>();
    public IList<DataGroup> Groups
    {
        get { return this._groups; }
    }
    
    public async Task<IEnumerable<DataGroup>> GetGroupsAsync(string jsonFilePath, PathType pathType = PathType.Relative)
    {
        _jsonFilePath = jsonFilePath;
        _pathType = pathType;
        await _instance.GetControlInfoDataAsync();

        return _instance.Groups;
    }

    /// <summary>
    /// Load Assets\NavViewMenu\AppData.json
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<DataGroup>> GetGroupsAsync()
    {
        _jsonFilePath = @"Assets\NavViewMenu\AppData.json";
        _pathType = PathType.Relative;
        await _instance.GetControlInfoDataAsync();

        return _instance.Groups;
    }
    
    public static async Task<DataGroup> GetGroupAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        // Simple linear search is acceptable for small data sets
        var matches = _instance.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
        if (matches.Count() == 1) return matches.First();
        return null;
    }
    
    public static async Task<DataItem> GetItemAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        var matches = _instance.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
        if (matches.Count() > 0) return matches.First();
        return null;
    }

    public static async Task<DataGroup> GetGroupFromItemAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        var matches = _instance.Groups.Where((group) => group.Items.FirstOrDefault(item => item.UniqueId.Equals(uniqueId)) != null);
        if (matches.Count() == 1) return matches.First();
        return null;
    }

    private async Task GetControlInfoDataAsync()
    {
        lock (_lock)
        {
            if (this.Groups.Count() != 0)
            {
                return;
            }
        }

        var jsonText = await LoadText(_jsonFilePath, _pathType);
        var controlInfoDataGroup = JsonSerializer.Deserialize(jsonText, typeof(Root), RootContext.Default) as Root;

        lock (_lock)
        {
            controlInfoDataGroup.Groups.SelectMany(g => g.Items).ToList().ForEach(item =>
            {
#nullable enable
                string? badgeString = item switch
                {
                    { IsNew: true } => "New",
                    { IsUpdated: true } => "Updated",
                    { IsPreview: true } => "Preview",
                    _ => null
                };

                if (item.DataInfoBadge != null && item.DataInfoBadge.BadgeStyle == null)
                {
                    item.DataInfoBadge.BadgeStyle = "AttentionValueInfoBadgeStyle";
                }

                item.BadgeString = badgeString;

#nullable disable
            });

            foreach (var group in controlInfoDataGroup.Groups)
            {
                if (!Groups.Any(g => g.Title == group.Title))
                {
                    Groups.Add(group);
                }
            }
        }
    }
    public static async Task<string> LoadText(string filePath, PathType pathType)
    {
        StorageFile file = null;
        if (!PackageHelper.IsPackaged)
        {
            if (pathType == PathType.Relative)
            {
                var sourcePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ProcessInfoHelper.GetFileVersionInfo().FileName), filePath));
                file = await StorageFile.GetFileFromPathAsync(sourcePath);
            }
            else
            {
                file = await StorageFile.GetFileFromPathAsync(filePath);
            }
        }
        else
        {
            if (pathType == PathType.Relative)
            {
                Uri sourceUri = new Uri("ms-appx:///" + filePath);
                file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);
            }
            else
            {
                Uri sourceUri = new Uri(filePath);
                file = await StorageFile.GetFileFromApplicationUriAsync(sourceUri);
            }
        }
        return await FileIO.ReadTextAsync(file);
    }
}