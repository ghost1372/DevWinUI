using Windows.Foundation.Metadata;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;

namespace DevWinUI;

[Experimental]
public class SavePicker
{
    public PickerOptions Options { get; set; } = PickerOptions.None;

    public bool ShowDetailedExtension { get; set; } = true;
    public string? CommitButtonText { get; set; }
    public string? SuggestedFileName { get; set; }
    public string? DefaultFileExtension { get; set; }
    public string? InitialDirectory { get; set; }
    public Windows.Storage.Pickers.PickerLocationId SuggestedStartLocation { get; set; } = Windows.Storage.Pickers.PickerLocationId.Unspecified;
    public string? Title { get; set; }
    public Dictionary<string, IList<string>> FileTypeChoices { get; set; } = new();
    public bool ShowAllFilesOption { get; set; } = true;

    /// <summary>
    /// Allows the user to select a file location for saving data through a dialog interface.
    /// </summary>
    /// <param name="window">Specifies the owner window.</param>
    /// <returns>Returns the selected file path as a string, or null if the operation is canceled.</returns>
    public string? PickSaveFile(Microsoft.UI.Xaml.Window window) => PickSaveFile(WindowNative.GetWindowHandle(window));

    /// <summary>
    /// Prompts the user to select a file to save and returns the selected file path as a string.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window.</param>
    /// <returns>Returns the path of the selected file or null if no file was selected.</returns>
    public string? PickSaveFile(IntPtr hwnd)
    {
        return SaveFileDialog(hwnd);
    }

    /// <summary>
    /// Asynchronously prompts the user to select a file location for saving a file.
    /// </summary>
    /// <param name="window">Specifies the owner window.</param>
    /// <returns>Returns the selected storage file if the user makes a selection.</returns>
    public async Task<StorageFile> PickSaveFileAsync(Microsoft.UI.Xaml.Window window) => await PickSaveFileAsync(WindowNative.GetWindowHandle(window));

    /// <summary>
    /// Asynchronously prompts the user to select a location to save a file and returns the corresponding storage file.
    /// </summary>
    /// <param name="hwnd">Specifies the owner window.</param>
    /// <returns>Returns the selected storage file or null if no file was chosen.</returns>
    public async Task<StorageFile> PickSaveFileAsync(IntPtr hwnd)
    {
        var filePath = SaveFileDialog(hwnd);
        return filePath != null ? await GetStorageFileOrCreateAsync(filePath) : null;
    }
    private async Task<StorageFile> GetStorageFileOrCreateAsync(string filePath)
    {
        if (File.Exists(filePath))
        {
            return await StorageFile.GetFileFromPathAsync(filePath);
        }
        else
        {
            var folder = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(folder);

            var storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await storageFile.DeleteAsync();
            return storageFile;
        }
    }

    private unsafe string? SaveFileDialog(IntPtr hwnd)
    {
        int hr = PInvoke.CoCreateInstance<IFileSaveDialog>(
                                    typeof(FileSaveDialog).GUID,
                                    null,
                                    CLSCTX.CLSCTX_INPROC_SERVER,
                                    out var fsd);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        IntPtr dialogPtr = (IntPtr)fsd;
        var dialog = (IFileOpenDialog*)dialogPtr;

        try
        {
            if (!string.IsNullOrEmpty(Title))
            {
                dialog->SetTitle(Title);
            }

            if (!string.IsNullOrEmpty(CommitButtonText))
            {
                dialog->SetOkButtonLabel(CommitButtonText);
            }

            if (SuggestedStartLocation != Windows.Storage.Pickers.PickerLocationId.Unspecified)
            {
                InitialDirectory = PathHelper.GetKnownFolderPath(SuggestedStartLocation);
            }

            if (!string.IsNullOrEmpty(InitialDirectory))
            {
                PInvoke.SHCreateItemFromParsingName(InitialDirectory, null, typeof(IShellItem).GUID, out void* ppv);
                IShellItem* psi = (IShellItem*)ppv;

                dialog->SetFolder(psi);
            }

            if (!string.IsNullOrEmpty(SuggestedFileName))
            {
                dialog->SetFileName(SuggestedFileName);
            }

            var filters = new List<COMDLG_FILTERSPEC>();

            if (ShowAllFilesOption)
            {
                filters.Add(new COMDLG_FILTERSPEC { pszName = (char*)Marshal.StringToHGlobalUni("All Files (*.*)"), pszSpec = (char*)Marshal.StringToHGlobalUni("*.*") });
            }

            foreach (var kvp in FileTypeChoices)
            {
                string displayName = kvp.Key;

                if (ShowDetailedExtension)
                {
                    string extensions = string.Join(", ", kvp.Value);
                    displayName = $"{kvp.Key} ({extensions})";
                }

                string spec = string.Join(";", kvp.Value);
                filters.Add(new COMDLG_FILTERSPEC { pszName = (char*)Marshal.StringToHGlobalUni(displayName), pszSpec = (char*)Marshal.StringToHGlobalUni(spec) });
            }

            dialog->SetFileTypes(filters.ToArray());

            if (!string.IsNullOrEmpty(DefaultFileExtension) && FileTypeChoices.ContainsKey(DefaultFileExtension))
            {
                int defaultIndex = new List<string>(FileTypeChoices.Keys).IndexOf(DefaultFileExtension) + (ShowAllFilesOption ? 1 : 0);
                dialog->SetFileTypeIndex((uint)(defaultIndex + 1));
            }

            dialog->SetOptions(PickerHelper.MapPickerOptionsToFOS(Options));

            try
            {
                dialog->Show(new HWND(hwnd));
            }
            catch (Exception ex) when ((uint)(ex.HResult) == 0x800704C7) // ERROR_CANCELLED
            {
                // User canceled the dialog, return null
                return null;
            }

            IShellItem* resultsPtr = null;
            dialog->GetResult((IShellItem**)(&resultsPtr));
            if (resultsPtr != null)
            {
                resultsPtr->GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out PWSTR filePath);
                return filePath.ToString();

            }
            return null;
        }
        finally
        {
            dialog->Close(new HRESULT(0));
            dialog->Release();
        }
    }
}
