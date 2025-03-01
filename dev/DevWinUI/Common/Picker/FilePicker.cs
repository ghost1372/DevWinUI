﻿using Windows.Foundation.Metadata;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.Shell.Common;

namespace DevWinUI;

[Experimental]
public class FilePicker
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

    public string PickSingleFile(Microsoft.UI.Xaml.Window window) => PickSingleFile(WindowNative.GetWindowHandle(window));
    public string PickSingleFile(IntPtr hwnd)
    {
        var files = OpenFileDialog(hwnd, false);
        return files.Count > 0 ? files[0] : null;
    }

    public async Task<StorageFile?> PickSingleFileAsync(Microsoft.UI.Xaml.Window window) => await PickSingleFileAsync(WindowNative.GetWindowHandle(window));
    public async Task<StorageFile?> PickSingleFileAsync(IntPtr hwnd)
    {
        var files = OpenFileDialog(hwnd, false);
        return files.Count > 0 ? await StorageFile.GetFileFromPathAsync(files[0]) : null;
    }

    public List<string> PickMultipleFiles(Microsoft.UI.Xaml.Window window) => PickMultipleFiles(WindowNative.GetWindowHandle(window));
    public List<string> PickMultipleFiles(IntPtr hwnd)
    {
        return OpenFileDialog(hwnd, true);
    }

    public async Task<List<StorageFile>> PickMultipleFilesAsync(Microsoft.UI.Xaml.Window window) => await PickMultipleFilesAsync(WindowNative.GetWindowHandle(window));
    public async Task<List<StorageFile>> PickMultipleFilesAsync(IntPtr hwnd)
    {
        var filePaths = OpenFileDialog(hwnd, true);
        var storageFiles = new List<StorageFile>();
        foreach (var path in filePaths)
        {
            storageFiles.Add(await StorageFile.GetFileFromPathAsync(path));
        }
        return storageFiles;
    }

    private unsafe List<string> OpenFileDialog(IntPtr hwnd, bool allowMultiple)
    {
        int hr = PInvoke.CoCreateInstance<IFileOpenDialog>(
                            typeof(FileOpenDialog).GUID,
                            null,
                            CLSCTX.CLSCTX_INPROC_SERVER,
                            out var fod);
        if (hr < 0)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        IntPtr dialogPtr = (IntPtr)fod;
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

            if (allowMultiple)
            {
                Options |= PickerOptions.FOS_ALLOWMULTISELECT;
            }

            dialog->SetOptions(PickerOptionHelper.MapPickerOptionsToFOS(Options));

            try
            {
                dialog->Show(new HWND(hwnd));
            }
            catch (Exception ex) when ((uint)(ex.HResult) == 0x800704C7) // ERROR_CANCELLED
            {
                // User canceled the dialog, return an empty list
                return new List<string>();
            }

            List<string> filePaths = new List<string>();

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
                                filePaths.Add(filePath.ToString());
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
                        filePaths.Add(filePath.ToString());
                    }
                }
            }

            return filePaths;
        }
        finally
        {
            dialog->Close(new HRESULT(0));
            dialog->Release();
        }
    }
}
