using Windows.Foundation.Metadata;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;

namespace DevWinUI;

[Experimental]
public class FolderPicker
{
    public PickerOptions Options { get; set; } = PickerOptions.None;

    public string? CommitButtonText { get; set; }
    public string? SuggestedFileName { get; set; }
    public string? InitialDirectory { get; set; }
    public PickerLocationId SuggestedStartLocation { get; set; } = PickerLocationId.Unspecified;
    public string? Title { get; set; }

    public string PickSingleFolder(Microsoft.UI.Xaml.Window window) => PickSingleFolder(WindowNative.GetWindowHandle(window));
    public string PickSingleFolder(IntPtr hwnd)
    {
        var folderPaths = OpenFolderDialog(hwnd, false);
        return folderPaths.Count > 0 ? folderPaths[0] : null;
    }
    public async Task<StorageFolder?> PickSingleFolderAsync(Microsoft.UI.Xaml.Window window) => await PickSingleFolderAsync(WindowNative.GetWindowHandle(window));
    public async Task<StorageFolder?> PickSingleFolderAsync(IntPtr hwnd)
    {
        var folderPaths = OpenFolderDialog(hwnd, false);
        return folderPaths.Count > 0 ? await StorageFolder.GetFolderFromPathAsync(folderPaths[0]) : null;
    }

    public List<string> PickMultipleFolders(Microsoft.UI.Xaml.Window window) => PickMultipleFolders(WindowNative.GetWindowHandle(window));
    public List<string> PickMultipleFolders(IntPtr hwnd)
    {
        return OpenFolderDialog(hwnd, true);
    }

    public async Task<List<StorageFolder>> PickMultipleFoldersAsync(Microsoft.UI.Xaml.Window window) => await PickMultipleFoldersAsync(WindowNative.GetWindowHandle(window));
    public async Task<List<StorageFolder>> PickMultipleFoldersAsync(IntPtr hwnd)
    {
        var folderPaths = OpenFolderDialog(hwnd, true);
        var storageFolders = new List<StorageFolder>();
        foreach (var path in folderPaths)
        {
            storageFolders.Add(await StorageFolder.GetFolderFromPathAsync(path));
        }
        return storageFolders;
    }

    private unsafe List<string> OpenFolderDialog(IntPtr hwnd, bool allowMultiple)
    {
        int hr = PInvoke.CoCreateInstance<IFileOpenDialog>(
                            typeof(FileOpenDialog).GUID,
                            null,
                            CLSCTX.CLSCTX_INPROC_SERVER,
                            out var fpd);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        IntPtr dialogPtr = (IntPtr)fpd;
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

            if (SuggestedStartLocation != PickerLocationId.Unspecified)
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

            Options |= PickerOptions.FOS_PICKFOLDERS;

            if (allowMultiple)
            {
                Options |= PickerOptions.FOS_ALLOWMULTISELECT;
            }
            
            dialog->SetOptions(PickerHelper.MapPickerOptionsToFOS(Options));

            try
            {
                dialog->Show(new HWND(hwnd));
            }
            catch (Exception ex) when ((uint)(ex.HResult) == 0x800704C7) // ERROR_CANCELLED
            {
                // User canceled the dialog, return an empty list
                return new List<string>();
            }

            List<string> folderPaths = new List<string>();

            if (allowMultiple)
            {
                IShellItemArray* resultsPtr = null;
                dialog->GetResults((IShellItemArray**)(&resultsPtr));

                if (resultsPtr != null)
                {
                    resultsPtr->GetCount(out uint count);

                    for (uint i = 0; i < count; i++)
                    {
                        IShellItem* itemPtr = null;
                        resultsPtr->GetItemAt(i, (IShellItem**)(&itemPtr));
                        if (itemPtr != null)
                        {
                            itemPtr->GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out PWSTR filePath);
                            if (filePath != null)
                            {
                                folderPaths.Add(filePath.ToString());
                            }
                        }
                    }
                }
            }
            else
            {
                IShellItem* resultsPtr = null;
                dialog->GetResult((IShellItem**)(&resultsPtr));

                if (resultsPtr != null)
                {
                    resultsPtr->GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out PWSTR filePath);
                    if (filePath != null)
                    {
                        folderPaths.Add(filePath.ToString());
                    }
                }
            }

            return folderPaths;
        }
        finally
        {
            dialog->Close(new HRESULT(0));
            dialog->Release();
        }
    }
}
