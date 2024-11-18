using System.Reflection;

namespace DevWinUI;

public partial class ContextMenuService
{
    private const string MenusFolderName = "custom_commands";

    public async Task<List<ContextMenuItem>> QueryAllAsync()
    {
        var configFolder = await GetMenusFolderAsync();
        var files = await configFolder.GetFilesAsync();
        var result = new List<ContextMenuItem>(files.Count);
        foreach (var file in files)
        {
            if (!file.Name.EndsWith(".json") && !file.Name.EndsWith(".json.disabled"))
            {
                continue;
            }

            var content = await FileIO.ReadTextAsync(file);
            try
            {
                var item = ConvertMenuFromJson(content);
                item.File = file;
                item.Enabled = IsEnabled(item);
                result.Add(item);
            }
            catch (Exception)
            {
                var item = new ContextMenuItem
                {
                    Title = $"<Error> config file:{file.Name}",
                    File = file
                };
                result.Add(item);
            }
        }

        result.Sort((l, r) => l.Index - r.Index);
        return result;
    }

    private async Task<StorageFile> CreateMenuFileAsync(string name, CreationCollisionOption creationCollisionOption)
    {
        var folder = await GetMenusFolderAsync();
        return await folder.CreateFileAsync(name, creationCollisionOption);
    }

    public async Task<StorageFolder> GetMenusFolderAsync()
    {
        var storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(MenusFolderName);
        switch (storageItem)
        {
            case StorageFile _:
                throw new Exception($"Menus Folder Error,\"{storageItem.Path}\" is not a folder");
            case StorageFolder storageFolder:
                return storageFolder;
            default:
                return await ApplicationData.Current.LocalFolder.CreateFolderAsync(MenusFolderName, CreationCollisionOption.OpenIfExists);
        }
    }

    public async Task SaveAsync(ContextMenuItem item)
    {
        await SaveAsyncInternal(item, CreationCollisionOption.ReplaceExisting);
    }

    public async Task SaveAsync(ContextMenuItem item, CreationCollisionOption creationCollisionOption)
    {
        await SaveAsyncInternal(item, creationCollisionOption);
    }

    private async Task SaveAsyncInternal(ContextMenuItem item, CreationCollisionOption creationCollisionOption)
    {
        if (null == item)
        {
            throw new Exception("Menu is null");
        }

        var (result, message) = CheckMenu(item);
        if (!result)
        {
            throw new Exception(message);
        }

        var menuFile = item.File;
        if (menuFile == null)
        {
            var fileName = $"{item.Title}.json";
            menuFile = await CreateMenuFileAsync(fileName, creationCollisionOption);
        }

        var content = ConvertMenuToJson(item);
        await FileIO.WriteTextAsync(menuFile, content);

        item.File = menuFile;
        item.Enabled = true;
    }
    public async Task<ContextMenuItem> ReadAsync(StorageFile menuFile)
    {
        if (null == menuFile)
        {
            throw new Exception("Menu file is null");
        }

        var content = await FileIO.ReadTextAsync(menuFile);
        try
        {
            var item = ConvertMenuFromJson(content);
            item.File = menuFile;
            return item;
        }
        catch (Exception e)
        {
            throw new Exception("Read From Menu file error", e);
        }
    }

    public async Task<StorageFile> RenameMenuFile(ContextMenuItem item, string name)
    {
        if (null == item)
        {
            throw new Exception("Menu is null");
        }

        var file = (item?.File) ?? throw new Exception("Menu file is null");

        string newName = null;
        if (!string.IsNullOrEmpty(name))
        {
            newName = Path.GetFileName(name);
        }
        if (string.IsNullOrEmpty(newName))
        {
            throw new Exception("New Name is empty");
        }

        await file.RenameAsync(newName, NameCollisionOption.GenerateUniqueName);
        return file;
    }

    public async Task DeleteAsync(ContextMenuItem item)
    {
        if (null == item)
        {
            throw new Exception("Menu is null");
        }

        var menuFile = item.File;
        if (null != menuFile)
        {
            await menuFile.DeleteAsync();
        }
    }

    public bool IsEnabled(ContextMenuItem item)
    {
        if (null == item)
        {
            throw new Exception("Menu is null");
        }
        return item.File?.Name.EndsWith(".json") == true;
    }

    public async Task<StorageFile> EnableAsync(ContextMenuItem item, bool enabled)
    {
        if (null == item)
        {
            throw new Exception("Menu is null");
        }

        var file = (item?.File) ?? throw new Exception("Menu file is null");
        var fileName = file.Name;

        if (enabled)
        {
            if (fileName.EndsWith(".json.disabled"))
            {
                fileName = fileName.Substring(0, fileName.Length - ".disabled".Length);
            }
        }
        else
        {
            if (fileName.EndsWith(".json"))
            {
                fileName += ".disabled";
            }
        }

        if (file.Name != fileName)
        {
            await file.RenameAsync(fileName, NameCollisionOption.FailIfExists);
        }

        return file;
    }

    public async Task BuildToCacheAsync()
    {
        var configFolder = await GetMenusFolderAsync();
        var files = await configFolder.GetFilesAsync();

        var menus = ApplicationData.Current.LocalSettings.CreateContainer("menus", ApplicationDataCreateDisposition.Always).Values;
        menus.Clear();

        for (var i = 0; i < files.Count; i++)
        {
            var file = files[i];
            if (!file.Name.EndsWith(".json"))
            {
                continue;
            }
            var content = await FileIO.ReadTextAsync(file);
            menus[i.ToString()] = content;
        }
    }

    public void ClearCache()
    {
        var menus = ApplicationData.Current.LocalSettings.CreateContainer("menus", ApplicationDataCreateDisposition.Always).Values;
        menus.Clear();
    }

    public ContextMenuItem ConvertMenuFromJson(string content)
    {
        var menu = JsonUtil.Deserialize(content);

        //update from old version v3.6
        if (menu.AcceptFileFlag == (int)FileMatchFlagEnum.None && menu.AcceptFile)
        {
            menu.AcceptFileFlag = (int)FileMatchFlagEnum.Ext;
        }

        //update from old version v3.8
        if (menu.AcceptDirectoryFlag == (int)DirectoryMatchFlagEnum.None && menu.AcceptDirectory)
        {
            menu.AcceptDirectoryFlag = (int)DirectoryMatchFlagEnum.Directory |
                                        (int)DirectoryMatchFlagEnum.Background |
                                        (int)DirectoryMatchFlagEnum.Desktop;
        }

        return menu;
    }

    public string ConvertMenuToJson(ContextMenuItem content)
    {
        var json = JsonUtil.Serialize(content);
        return json;
    }

    private (bool, string) CheckMenu(ContextMenuItem content)
    {
        if (string.IsNullOrEmpty(content.Title))
        {
            return (false, "Title is empty");
        }

        if (string.IsNullOrEmpty(content.Exe))
        {
            return (false, "Exe is empty");
        }

        return (true, string.Empty);
    }

    public async void OpenMenusFolderAsync()
    {
        var folder = await GetMenusFolderAsync();
        _ = await Launcher.LaunchFolderAsync(folder);
    }

    public async void OpenMenuFileAsync(ContextMenuItem item)
    {
        if (item.File == null)
        {
            return;
        }
        _ = await Launcher.LaunchFileAsync(item.File);
    }

    public string GetCustomMenuName()
    {
        var value = ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"];
        return (value as string) ?? "Open With";
    }

    public async void SetCustomMenuName(string name)
    {
        await Task.Run(() =>
        {
            ApplicationData.Current.LocalSettings.Values["Custom_Menu_Name"] = name ?? "Open With";
        });
    }

    public async void ClearAllMenus()
    {
        var folder = await GetMenusFolderAsync();
        await folder.DeleteAsync();
    }

    public void ReplaceMenu(ContextMenuItem menuItem, ContextMenuItem newMenuItem)
    {
        PropertyInfo[] propsSource = typeof(ContextMenuItem).GetProperties();
        foreach (PropertyInfo infoSource in propsSource)
        {
            object value = infoSource.GetValue(newMenuItem, null);
            infoSource.SetValue(menuItem, value, null);
        }
    }

    public async Task RefreshMenuAsync(ContextMenuItem menuItem)
    {
        var newMenuItem = await ReadAsync(menuItem.File);
        ReplaceMenu(menuItem, newMenuItem);
    }

    public async Task<bool> CheckMenuExistsAsync(ContextMenuItem item)
    {
        var configFolder = await GetMenusFolderAsync();
        var files = await configFolder.GetFilesAsync();
        return files.Any(x => x.Name.Equals($"{item?.Title}.json", StringComparison.OrdinalIgnoreCase));
    }
}
