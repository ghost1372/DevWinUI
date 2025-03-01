using Windows.Foundation.Metadata;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;

namespace DevWinUI;

[Experimental]
public class SavePicker
{
    public PickerOptions Options { get; set; } = PickerOptions.None;
    public string? OkButtonLabel { get; set; }
    public string? DefaultFileName { get; set; }
    public string? DefaultFileType { get; set; }
    public string? InitialDirectory { get; set; }
    public KnownFolderOption? InitialKnownFolder { get; set; }
    public string? Title { get; set; }
    public Dictionary<string, List<string>> FileTypeChoices { get; set; } = new();
    public bool ShowAllFilesFileType { get; set; } = true;

    public string? PickSaveFile(IntPtr hwnd)
    {
        return SaveFileDialog(hwnd);
    }

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

            if (!string.IsNullOrEmpty(OkButtonLabel))
            {
                dialog->SetOkButtonLabel(OkButtonLabel);
            }

            if (InitialKnownFolder.HasValue)
            {
                InitialDirectory = PathHelper.GetKnownFolderPath(InitialKnownFolder.Value);
            }

            if (!string.IsNullOrEmpty(InitialDirectory))
            {
                PInvoke.SHCreateItemFromParsingName(InitialDirectory, null, typeof(IShellItem).GUID, out void* ppv);
                IShellItem* psi = (IShellItem*)ppv;

                dialog->SetFolder(psi);
            }


            if (!string.IsNullOrEmpty(DefaultFileName))
            {
                dialog->SetFileName(DefaultFileName);
            }

            var filters = new List<COMDLG_FILTERSPEC>();

            if (ShowAllFilesFileType)
            {
                filters.Add(new COMDLG_FILTERSPEC { pszName = (char*)Marshal.StringToHGlobalUni("All Files (*.*)"), pszSpec = (char*)Marshal.StringToHGlobalUni("*.*") });
            }

            foreach (var kvp in FileTypeChoices)
            {
                string extensions = string.Join(", ", kvp.Value);
                string displayName = $"{kvp.Key} ({extensions})";
                string spec = string.Join(";", kvp.Value);
                filters.Add(new COMDLG_FILTERSPEC { pszName = (char*)Marshal.StringToHGlobalUni(displayName), pszSpec = (char*)Marshal.StringToHGlobalUni(spec) });
            }

            dialog->SetFileTypes(filters.ToArray());

            if (!string.IsNullOrEmpty(DefaultFileType) && FileTypeChoices.ContainsKey(DefaultFileType))
            {
                int defaultIndex = new List<string>(FileTypeChoices.Keys).IndexOf(DefaultFileType) + (ShowAllFilesFileType ? 1 : 0);
                dialog->SetFileTypeIndex((uint)(defaultIndex + 1));
            }

            dialog->SetOptions(PickerOptionHelper.MapPickerOptionsToFOS(Options));

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
