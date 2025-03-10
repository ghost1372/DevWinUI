namespace DevWinUI;
public partial class PathHelper
{
    public static async Task<string> GetFilePath(string filePath, PathType pathType = PathType.Relative)
    {
        var file = await FileHelper.GetStorageFile(filePath, pathType);

        return file.Path;
    }

    /// <summary>
    /// Return Full Path to Assets folder even if your app is SelfContained/SingleFile
    /// Normal Mode: App/bin/Assets
    /// SelfContained/SingleFile: Temp/net/xx/Assets
    /// </summary>
    /// <returns></returns>
    public static string GetFullPathToAsset(string assetName)
    {
        return Path.Combine(AppContext.BaseDirectory, "Assets", assetName);
    }

    /// <summary>
    /// Return Full Path to Exe even if your app is SelfContained/SingleFile
    /// Normal Mode: App/bin/App.exe
    /// SelfContained/SingleFile: Temp/net/xx/App.exe
    /// </summary>
    /// <returns></returns>
    public static string GetFullPathToExe()
    {
        return Path.Combine(AppContext.BaseDirectory, ProcessInfoHelper.GetProcess().MainModule.ModuleName);
    }

    /// <summary>
    /// Return ApplicationData Folder Path for Packaged and UnPackaged Mode Automatically
    /// </summary>
    /// <param name="forceUnpackagedMode"></param>
    /// <returns></returns>
    public static string GetAppDataFolderPath(bool forceUnpackagedMode = false)
    {
        var unpackaged = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (forceUnpackagedMode)
            return unpackaged;

        return PackageHelper.IsPackaged ? Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.Path : unpackaged;
    }

    /// <summary>
    /// Use P/Invoke for getting Full Path To Exe
    /// Normal Mode: App/bin/App.exe
    /// SelfContained/SingleFile: App/bin/App.exe
    /// </summary>
    /// <returns></returns>
    public static string GetExecutablePathNative()
    {
        const uint MAX_Length = 255;
        Span<char> buffer = stackalloc char[(int)MAX_Length];

        unsafe
        {
            fixed (char* pBuffer = buffer)
            {
                uint result = PInvoke.GetModuleFileName(HMODULE.Null, new PWSTR(pBuffer), MAX_Length);

                if (result == 0)
                {
                    throw new Win32Exception();
                }

                return new string(pBuffer, 0, (int)result);
            }
        }
    }

    public static string GetKnownFolderPath(PickerLocationId pickerLocationId)
    {
        Guid folderId;

        switch (pickerLocationId)
        {
            case PickerLocationId.DocumentsLibrary:
                folderId = new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7"); // FOLDERID_Documents
                break;
            case PickerLocationId.ComputerFolder:
                folderId = new Guid("0AC0837C-BBF8-452A-850D-79D08E667CA7"); // FOLDERID_ComputerFolder
                break;
            case PickerLocationId.Desktop:
                folderId = new Guid("B4BFCC3A-DB2C-424C-B029-7FE99A87C641"); // FOLDERID_Desktop
                break;
            case PickerLocationId.Downloads:
                folderId = new Guid("374DE290-123F-4565-9164-39C4925E467B"); // FOLDERID_Downloads
                break;
            case PickerLocationId.HomeGroup:
                folderId = new Guid("52528A6B-B9E3-4ADD-B60D-588C2DBA842D"); // FOLDERID_HomeGroup
                break;
            case PickerLocationId.MusicLibrary:
                folderId = new Guid("4BD8D571-6D19-48D3-BE97-422220080E43"); // FOLDERID_MusicLibrary
                break;
            case PickerLocationId.PicturesLibrary:
                folderId = new Guid("33E28130-4E1E-4676-835A-98395C3BC3BB"); // FOLDERID_PicturesLibrary
                break;
            case PickerLocationId.VideosLibrary:
                folderId = new Guid("18989B1D-99B5-455B-841C-AB7C74E4DDFC"); // FOLDERID_VideosLibrary
                break;
            case PickerLocationId.Objects3D:
                folderId = new Guid("31C0DD25-9439-4F12-BF41-7FF4EDA38722"); // FOLDERID_Objects3D
                break;
            case PickerLocationId.Unspecified:
            default:
                return string.Empty;
        }

        PWSTR pszPath;
        int hr = PInvoke.SHGetKnownFolderPath(ref folderId, 0, null, out pszPath);
        if (hr != 0) return string.Empty;

        return pszPath.ToString();
    }
}
public enum PickerLocationId
{
    DocumentsLibrary,
    ComputerFolder,
    Desktop,
    Downloads,
    HomeGroup,
    MusicLibrary,
    PicturesLibrary,
    VideosLibrary,
    Objects3D,
    Unspecified
}
