using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevWinUI;

public partial class Root
{
    public ObservableCollection<DataGroup> Groups { get; set; }
}
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Root))]
internal partial class RootContext : JsonSerializerContext
{
}

public sealed partial class DataSource
{
    private string _jsonFilePath;
    private PathType _pathType;
    private string _loadedJsonFilePath;
    private PathType _loadedPathType;
    private int _loadedGroupCount;
    private bool _isLoaded;
    private long _resetVersion;

    private static readonly object _lock = new();
    private readonly SemaphoreSlim _loadSemaphore = new(1, 1);
    public bool UseTaskForDeserialization { get; set; }

    /// <summary>Raised after parsed navigation data has been committed to <see cref="Groups"/>.</summary>
    public event EventHandler? ParsingCompleted;

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
        ArgumentException.ThrowIfNullOrWhiteSpace(jsonFilePath);
        await _instance.GetControlInfoDataAsync(jsonFilePath, pathType, UseTaskForDeserialization);

        return _instance.Groups;
    }

    public void Reset()
    {
        lock (_lock)
        {
            Groups.Clear();
            _isLoaded = false;
            _resetVersion++;
        }
    }

    public static async Task<DataGroup> GetGroupAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        // Simple linear search is acceptable for small data sets
        DataGroup match = null;
        foreach (var group in _instance.Groups)
        {
            if (string.Equals(group.UniqueId, uniqueId, StringComparison.Ordinal))
            {
                if (match != null) return null;
                match = group;
            }
        }
        return match;
    }

    public static async Task<DataItem> GetItemAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        foreach (var group in _instance.Groups)
        {
            foreach (var item in group.Items)
            {
                if (string.Equals(item.UniqueId, uniqueId, StringComparison.Ordinal)) return item;
            }
        }
        return null;
    }

    public static async Task<DataGroup> GetGroupFromItemAsync(string uniqueId)
    {
        await _instance.GetControlInfoDataAsync();
        DataGroup match = null;
        foreach (var group in _instance.Groups)
        {
            foreach (var item in group.Items)
            {
                if (string.Equals(item.UniqueId, uniqueId, StringComparison.Ordinal))
                {
                    if (match != null) return null;
                    match = group;
                    break;
                }
            }
        }
        return match;
    }

    private async Task GetControlInfoDataAsync()
    {
        string jsonFilePath;
        PathType pathType;
        bool useTaskForDeserialization;
        lock (_lock)
        {
            if (_isLoaded && Groups.Count == _loadedGroupCount)
            {
                return;
            }

            jsonFilePath = _jsonFilePath;
            pathType = _pathType;
            useTaskForDeserialization = UseTaskForDeserialization;
        }

        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            throw new InvalidOperationException("Configure a JSON file before requesting navigation data.");
        }

        await GetControlInfoDataAsync(jsonFilePath, pathType, useTaskForDeserialization);
    }

    private async Task GetControlInfoDataAsync(string jsonFilePath, PathType pathType, bool useTaskForDeserialization)
    {
        var parsingCompleted = false;
        await _loadSemaphore.WaitAsync();
        try
        {
            long resetVersion;
            lock (_lock)
            {
                if (_isLoaded && Groups.Count == _loadedGroupCount && _loadedPathType == pathType && string.Equals(_loadedJsonFilePath, jsonFilePath, StringComparison.Ordinal))
                {
                    return;
                }

                resetVersion = _resetVersion;
            }

            var jsonText = await LoadText(jsonFilePath, pathType);
            var controlInfoData = useTaskForDeserialization
                ? await Task.Run(() => JsonSerializer.Deserialize(jsonText, typeof(Root), RootContext.Default) as Root)
                : JsonSerializer.Deserialize(jsonText, typeof(Root), RootContext.Default) as Root;

            if (controlInfoData?.Groups == null)
            {
                throw new JsonException("The navigation JSON must contain a Groups array.");
            }

            var groupIds = new HashSet<string>(StringComparer.Ordinal);
            var itemIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var group in controlInfoData.Groups)
            {
                if (group == null)
                {
                    throw new JsonException("The navigation JSON contains a null group.");
                }
                if (string.IsNullOrWhiteSpace(group.UniqueId) || !groupIds.Add(group.UniqueId))
                {
                    throw new JsonException($"Navigation group ID '{group.UniqueId}' is empty or duplicated.");
                }

                group.Items ??= new ObservableCollection<DataItem>();
                if (group.UsexUid)
                {
                    group.Title = InternalLocalizationHelper.GetLocalizedText(group.LocalizeId, ResourceType.Title);
                    group.SecondaryTitle = InternalLocalizationHelper.GetLocalizedText(group.LocalizeId, ResourceType.SecondaryTitle);
                    group.Subtitle = InternalLocalizationHelper.GetLocalizedText(group.LocalizeId, ResourceType.Subtitle);
                    group.Description = InternalLocalizationHelper.GetLocalizedText(group.LocalizeId, ResourceType.Description);
                }

                foreach (var item in group.Items)
                {
                    if (item == null)
                    {
                        throw new JsonException($"Navigation group '{group.UniqueId}' contains a null item.");
                    }
                    if (string.IsNullOrWhiteSpace(item.UniqueId) || !itemIds.Add(item.UniqueId))
                    {
                        throw new JsonException($"Navigation item ID '{item.UniqueId}' is empty or duplicated.");
                    }

                    string? badgeString = item switch
                    {
                        { IsNew: true } => "New",
                        { IsUpdated: true } => "Updated",
                        { IsPreview: true } => "Preview",
                        _ => null
                    };

                    item.DataInfoBadge ??= new DataInfoBadge();
                    if (group.UseBuiltInLandingPageInfoBadgeStyle &&
                        string.IsNullOrEmpty(item.DataInfoBadge.LandingPageInfoBadgeStyle) &&
                        !string.IsNullOrEmpty(badgeString))
                    {
                        item.DataInfoBadge.LandingPageInfoBadgeStyle = group.DefaultBuiltInLandingPageInfoBadgeStyle;
                    }

                    item.DataInfoBadge.IsLandingPageInfoBadgeHidden =
                        item.DataInfoBadge.IsLandingPageInfoBadgeHidden == true || string.IsNullOrEmpty(badgeString) ? false : true;
                    item.BadgeString = badgeString;

                    if (item.UsexUid)
                    {
                        item.Title = InternalLocalizationHelper.GetLocalizedText(item.LocalizeId, ResourceType.Title);
                        item.SecondaryTitle = InternalLocalizationHelper.GetLocalizedText(item.LocalizeId, ResourceType.SecondaryTitle);
                        item.Subtitle = InternalLocalizationHelper.GetLocalizedText(item.LocalizeId, ResourceType.Subtitle);
                        item.Description = InternalLocalizationHelper.GetLocalizedText(item.LocalizeId, ResourceType.Description);
                    }
                }
            }

            lock (_lock)
            {
                if (resetVersion != _resetVersion)
                {
                    return;
                }

                Groups.Clear();
                foreach (var group in controlInfoData.Groups)
                {
                    Groups.Add(group);
                }

                _jsonFilePath = jsonFilePath;
                _pathType = pathType;
                _loadedJsonFilePath = jsonFilePath;
                _loadedPathType = pathType;
                _loadedGroupCount = Groups.Count;
                _isLoaded = true;
                parsingCompleted = true;
            }
        }
        finally
        {
            _loadSemaphore.Release();
        }

        if (parsingCompleted)
        {
            ParsingCompleted?.Invoke(this, EventArgs.Empty);
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
