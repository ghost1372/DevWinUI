using Windows.Storage.Pickers;
using WinRT.Interop;

namespace DevWinUI;
public partial class FileAndFolderPickerHelper
{
    private static FileSavePicker FileSavePicker(Window window, IntPtr windowHandle, IDictionary<string, IList<string>> fileTypeChoices, string suggestedFileName, string defaultFileExtension, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder)
    {
        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = suggestedStartLocation;
        foreach (var item in fileTypeChoices)
        {
            savePicker.FileTypeChoices.Add(item.Key, item.Value);
        }

        if (!string.IsNullOrEmpty(suggestedFileName))
        {
            savePicker.SuggestedFileName = suggestedFileName;
        }

        if (!string.IsNullOrEmpty(defaultFileExtension))
        {
            savePicker.DefaultFileExtension = defaultFileExtension;
        }

        if (window == null)
        {
            InitializeWithWindow.Initialize(savePicker, windowHandle);
        }

        if (windowHandle == IntPtr.Zero)
        {
            InitializeWithWindow.Initialize(savePicker, WindowNative.GetWindowHandle(window));
        }

        return savePicker;
    }

    private static FileOpenPicker FileOpenPicker(Window window, IntPtr windowHandle, IList<string> fileTypeFilter, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        var picker = new FileOpenPicker();
        picker.ViewMode = pickerViewMode;
        picker.SuggestedStartLocation = suggestedStartLocation;

        if (window == null)
        {
            InitializeWithWindow.Initialize(picker, windowHandle);
        }

        if (windowHandle == IntPtr.Zero)
        {
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(window));
        }

        foreach (var item in fileTypeFilter)
        {
            picker.FileTypeFilter.Add(item);
        }

        return picker;
    }

    private static FolderPicker FolderPicker(Window window, IntPtr windowHandle, IList<string> fileTypeFilter = null, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        FolderPicker folderPicker = new();

        if (window == null)
        {
            InitializeWithWindow.Initialize(folderPicker, windowHandle);
        }

        if (windowHandle == IntPtr.Zero)
        {
            InitializeWithWindow.Initialize(folderPicker, WindowNative.GetWindowHandle(window));
        }

        if (fileTypeFilter != null)
        {
            foreach (var item in fileTypeFilter)
            {
                folderPicker.FileTypeFilter.Add(item);
            }
        }
        else
        {
            folderPicker.FileTypeFilter.Add("*");
        }

        folderPicker.SuggestedStartLocation = suggestedStartLocation;
        folderPicker.ViewMode = pickerViewMode;
        return folderPicker;
    }

    public static async Task<StorageFile> PickSaveFileAsync(Window window, IDictionary<string, IList<string>> fileTypeChoices, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder)
    {
        return await FileSavePicker(window, IntPtr.Zero, fileTypeChoices, null, null, suggestedStartLocation).PickSaveFileAsync();
    }
    public static async Task<StorageFile> PickSaveFileAsync(IntPtr windowHandle, IDictionary<string, IList<string>> fileTypeChoices, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder)
    {
        return await FileSavePicker(null, windowHandle, fileTypeChoices, null, null, suggestedStartLocation).PickSaveFileAsync();
    }

    public static async Task<StorageFile> PickSaveFileAsync(Window window, IDictionary<string, IList<string>> fileTypeChoices, string suggestedFileName, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder)
    {
        return await FileSavePicker(window, IntPtr.Zero, fileTypeChoices, suggestedFileName, null, suggestedStartLocation).PickSaveFileAsync();
    }
    public static async Task<StorageFile> PickSaveFileAsync(IntPtr windowHandle, IDictionary<string, IList<string>> fileTypeChoices, string suggestedFileName, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder)
    {
        return await FileSavePicker(null, windowHandle, fileTypeChoices, suggestedFileName, null, suggestedStartLocation).PickSaveFileAsync();
    }

    public static async Task<StorageFile> PickSaveFileAsync(Window window, IDictionary<string, IList<string>> fileTypeChoices, string suggestedFileName, string defaultFileExtension, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder)
    {
        return await FileSavePicker(window, IntPtr.Zero, fileTypeChoices, suggestedFileName, defaultFileExtension, suggestedStartLocation).PickSaveFileAsync();
    }
    public static async Task<StorageFile> PickSaveFileAsync(IntPtr windowHandle, IDictionary<string, IList<string>> fileTypeChoices, string suggestedFileName, string defaultFileExtension, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder)
    {
        return await FileSavePicker(null, windowHandle, fileTypeChoices, suggestedFileName, defaultFileExtension, suggestedStartLocation).PickSaveFileAsync();
    }

    public static async Task<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(Window window, IList<string> fileTypeFilter, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        return await FileOpenPicker(window, IntPtr.Zero, fileTypeFilter, suggestedStartLocation, pickerViewMode).PickMultipleFilesAsync();
    }
    public static async Task<IReadOnlyList<StorageFile>> PickMultipleFilesAsync(IntPtr windowHandle, IList<string> fileTypeFilter, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        return await FileOpenPicker(null, windowHandle, fileTypeFilter, suggestedStartLocation, pickerViewMode).PickMultipleFilesAsync();
    }

    public static async Task<StorageFile> PickSingleFileAsync(Window window, IList<string> fileTypeFilter, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        return await FileOpenPicker(window, IntPtr.Zero, fileTypeFilter, suggestedStartLocation, pickerViewMode).PickSingleFileAsync();
    }
    public static async Task<StorageFile> PickSingleFileAsync(IntPtr windowHandle, IList<string> fileTypeFilter, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        return await FileOpenPicker(null, windowHandle, fileTypeFilter, suggestedStartLocation, pickerViewMode).PickSingleFileAsync();
    }

    public static async Task<StorageFolder> PickSingleFolderAsync(Window window, IList<string> fileTypeFilter = null, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        return await FolderPicker(window, IntPtr.Zero, fileTypeFilter, suggestedStartLocation, pickerViewMode).PickSingleFolderAsync();
    }
    public static async Task<StorageFolder> PickSingleFolderAsync(IntPtr windowHandle, IList<string> fileTypeFilter = null, PickerLocationId suggestedStartLocation = PickerLocationId.ComputerFolder, PickerViewMode pickerViewMode = PickerViewMode.Thumbnail)
    {
        return await FolderPicker(null, windowHandle, fileTypeFilter, suggestedStartLocation, pickerViewMode).PickSingleFolderAsync();
    }
}
