using PickerLocationId = Microsoft.Windows.Storage.Pickers.PickerLocationId;

namespace DevWinUI;

public partial class PathHelper
{
    private static readonly Guid FolderIdDocuments = new("FDD39AD0-238F-46AF-ADB4-6C85480369C7");
    private static readonly Guid FolderIdComputer = new("0AC0837C-BBF8-452A-850D-79D08E667CA7");
    private static readonly Guid FolderIdDesktop = new("B4BFCC3A-DB2C-424C-B029-7FE99A87C641");
    private static readonly Guid FolderIdDownloads = new("374DE290-123F-4565-9164-39C4925E467B");
    private static readonly Guid FolderIdMusic = new("4BD8D571-6D19-48D3-BE97-422220080E43");
    private static readonly Guid FolderIdPictures = new("33E28130-4E1E-4676-835A-98395C3BC3BB");
    private static readonly Guid FolderIdVideos = new("18989B1D-99B5-455B-841C-AB7C74E4DDFC");
    private static readonly Guid FolderIdObjects3D = new("31C0DD25-9439-4F12-BF41-7FF4EDA38722");

    /// <summary>
    /// Retrieves the file path of a specified file asynchronously based on the given path type.
    /// </summary>
    /// <param name="filePath">Specifies the location of the file to be accessed.</param>
    /// <param name="pathType">Indicates whether the file path is relative or absolute.</param>
    /// <returns>Returns the path of the file as a string.</returns>
    public static async Task<string> GetFilePath(string filePath, PathType pathType)
    {
        var file = await FileHelper.GetStorageFile(filePath, pathType);

        return file.Path;
    }

    /// <summary>
    /// Converts an input path or URI to a valid Uri, handling absolute paths, relative paths, 
    /// and ms-appx:// URIs correctly for both packaged and unpackaged applications.
    /// </summary>
    public static Uri GetFilePath(object input)
    {
        if (input == null) throw new ArgumentNullException(nameof(input));

        Uri uri = input as Uri ?? new Uri(input.ToString(), UriKind.RelativeOrAbsolute);

        // Case 1: Absolute file path (C:\...) or remote URLs (http/https)
        if (uri.IsAbsoluteUri)
        {
            if (uri.IsFile)
            {
                return uri; // Local file path
            }
            else if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                return uri; // Remote URL
            }
        }

        bool isPackaged = RuntimeHelper.IsPackaged();

        if (uri.IsAbsoluteUri && uri.Scheme == "ms-appx")
        {
            if (isPackaged)
            {
                return uri;
            }
            else
            {
                // Convert to unpackaged local path
                string path = uri.AbsolutePath.TrimStart('/');
                string fullPath = Path.Combine(AppContext.BaseDirectory, path.Replace('/', Path.DirectorySeparatorChar));
                return new Uri(fullPath);
            }
        }

        // Case 3: Relative path (e.g., Assets/Others/p1.jpg)
        string relativePath = uri.OriginalString.TrimStart('/');

        if (isPackaged)
        {
            // In packaged, convert relative path to ms-appx:///
            return new Uri($"ms-appx:///{relativePath.Replace('\\', '/')}");
        }
        else
        {
            // In unpackaged, convert relative path to full local path
            string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
            return new Uri(fullPath);
        }
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
    /// <param name="forceLegacyPath"></param>
    /// <returns></returns>
    public static string GetAppDataFolderPath(bool forceLegacyPath = false)
    {
        var applicationData = RuntimeHelper.IsPackaged()
            ? Microsoft.Windows.Storage.ApplicationData.GetDefault()
            : Microsoft.Windows.Storage.ApplicationData.GetForUnpackaged(ProcessInfoHelper.Publisher, ProcessInfoHelper.ProductName);

        var legacyPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (forceLegacyPath)
            return legacyPath;

        if (!Directory.Exists(applicationData.LocalPath))
        {
            Directory.CreateDirectory(applicationData.LocalPath);
        }

        return applicationData.LocalPath ?? legacyPath;
    }

    /// <summary>
    /// Use P/Invoke for getting Full Path To Exe
    /// Normal Mode: App/bin/App.exe
    /// SelfContained/SingleFile: App/bin/App.exe
    /// </summary>
    /// <returns></returns>
    public static string GetExecutablePathNative()
    {
        const int MAX_Length = 1024;
        Span<char> buffer = stackalloc char[MAX_Length];
        var hModule = PInvoke.GetModuleHandle(string.Empty);
        SafeHandle safeHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(hModule.DangerousGetHandle(), ownsHandle: false);

        uint result = PInvoke.GetModuleFileName(safeHandle, buffer);
        return result > 0 ? buffer.Slice(0, (int)result).ToString() : string.Empty;
    }

    /// <summary>
    /// Retrieves the file system path for a known folder based on the specified location identifier.
    /// </summary>
    /// <param name="pickerLocationId">Specifies the location of the known folder to retrieve the path for.</param>
    /// <returns>Returns the path as a string, or an empty string if the path cannot be retrieved.</returns>
    public static string GetKnownFolderPath(PickerLocationId pickerLocationId)
    {
        Guid? folderId = pickerLocationId switch
        {
            PickerLocationId.DocumentsLibrary => FolderIdDocuments,
            PickerLocationId.ComputerFolder => FolderIdComputer,
            PickerLocationId.Desktop => FolderIdDesktop,
            PickerLocationId.Downloads => FolderIdDownloads,
            PickerLocationId.MusicLibrary => FolderIdMusic,
            PickerLocationId.PicturesLibrary => FolderIdPictures,
            PickerLocationId.VideosLibrary => FolderIdVideos,
            PickerLocationId.Objects3D => FolderIdObjects3D,
            _ => null
        };

        return folderId is Guid id ? QueryKnownFolderPath(in id) : string.Empty;
    }

    private static string QueryKnownFolderPath(in Guid folderId)
    {
        HRESULT hr = PInvoke.SHGetKnownFolderPath(in folderId, 0, null, out CoTaskMemFreePWSTRSafeHandle handle);

        using (handle)
        {
            if (hr.Failed)
                return string.Empty;

            return Marshal.PtrToStringUni(handle.DangerousGetHandle()) ?? string.Empty;
        }
    }
}
