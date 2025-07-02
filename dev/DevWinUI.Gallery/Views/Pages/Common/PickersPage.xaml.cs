using System.Text;
using WinRT.Interop;

namespace DevWinUIGallery.Views;
public sealed partial class PickersPage : Page
{
    public BaseViewModel ViewModel { get; }
    public PickersPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        this.InitializeComponent();
    }

    private async void btnPickSaveFileAsync_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var picker = new SavePicker(WindowNative.GetWindowHandle(App.MainWindow));
        picker.Title = txtSavePickerTitle.Text;
        picker.SuggestedFileName = txtSavePickerSuggestedFileName.Text;
        picker.DefaultFileExtension = txtSavePickerDefaultFileExtension.Text;
        picker.ShowAllFilesOption = TGSavePickerShowAllFilesOption.IsOn;
        picker.CommitButtonText = txtSavePickerCommitButtonText.Text;
        picker.FileTypeChoices = new Dictionary<string, IList<string>>
        {
            { "Images", new List<string> { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" } },
            { "Text Files", new List<string> { "*.txt", "*.md", "*.log" } }
        };

        picker.InitialDirectory = txtSavePickerInitialDirectory.Text;

        var locationId = ViewModel.SuggestedStartLocationSelectedItem;
        if (locationId == null)
        {
            locationId = Windows.Storage.Pickers.PickerLocationId.Unspecified;
        }
        picker.SuggestedStartLocation = GeneralHelper.GetEnum<Windows.Storage.Pickers.PickerLocationId>(locationId.ToString());

        var file = await picker.PickSaveFileAsync();
        if (file != null)
        {
            txtRes1.Text = file.Path;
        }
    }

    private async void btnPickMultipleFilesAsync_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var picker = new FilePicker(WindowNative.GetWindowHandle(App.MainWindow));
        picker.Title = txtFilePickerMultipleTitle.Text;
        picker.SuggestedFileName = txtFilePickerMultipleSuggestedFileName.Text;
        picker.DefaultFileExtension = txtFilePickerMultipleDefaultFileExtension.Text;
        picker.ShowAllFilesOption = TGFilePickerMultipleShowAllFilesOption.IsOn;
        picker.CommitButtonText = txtFilePickerMultipleCommitButtonText.Text;
        picker.FileTypeChoices = new Dictionary<string, IList<string>>
        {
            { "Images", new List<string> { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" } },
            { "Text Files", new List<string> { "*.txt", "*.md", "*.log" } }
        };

        picker.InitialDirectory = txtFilePickerMultipleInitialDirectory.Text;

        var locationId = ViewModel.SuggestedStartLocationSelectedItem;
        if (locationId == null)
        {
            locationId = Windows.Storage.Pickers.PickerLocationId.Unspecified;
        }
        picker.SuggestedStartLocation = GeneralHelper.GetEnum<Windows.Storage.Pickers.PickerLocationId>(locationId.ToString());

        var files = await picker.PickMultipleFilesAsync();
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var item in files)
        {
            stringBuilder.AppendLine(item.Path.ToString());
        }
        txtRes2.Text = stringBuilder.ToString();
    }

    private async void btnPickSingleFileAsync_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var picker = new FilePicker(WindowNative.GetWindowHandle(App.MainWindow));
        picker.Title = txtFilePickerSingleTitle.Text;
        picker.SuggestedFileName = txtFilePickerSingleSuggestedFileName.Text;
        picker.DefaultFileExtension = txtFilePickerSingleDefaultFileExtension.Text;
        picker.ShowAllFilesOption = TGFilePickerSingleShowAllFilesOption.IsOn;
        picker.CommitButtonText = txtFilePickerSingleCommitButtonText.Text;
        picker.FileTypeChoices = new Dictionary<string, IList<string>>
        {
            { "Images", new List<string> { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" } },
            { "Text Files", new List<string> { "*.txt", "*.md", "*.log" } }
        };
        picker.InitialDirectory = txtFilePickerSingleInitialDirectory.Text;

        var locationId = ViewModel.SuggestedStartLocationSelectedItem;
        if (locationId == null)
        {
            locationId = Windows.Storage.Pickers.PickerLocationId.Unspecified;
        }
        picker.SuggestedStartLocation = GeneralHelper.GetEnum<Windows.Storage.Pickers.PickerLocationId>(locationId.ToString());

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            txtRes3.Text = file.Path;
        }
    }

    private async void btnPickSingleFolderAsync_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var picker = new FolderPicker(WindowNative.GetWindowHandle(App.MainWindow));
        picker.Title = txtFolderPickerSingleTitle.Text;
        picker.SuggestedFileName = txtFolderPickerSingleSuggestedFileName.Text;
        picker.CommitButtonText = txtFolderPickerSingleCommitButtonText.Text;
        picker.InitialDirectory = txtFolderPickerSingleInitialDirectory.Text;

        var locationId = ViewModel.SuggestedStartLocationSelectedItem;
        if (locationId == null)
        {
            locationId = Windows.Storage.Pickers.PickerLocationId.Unspecified;
        }
        picker.SuggestedStartLocation = GeneralHelper.GetEnum<Windows.Storage.Pickers.PickerLocationId>(locationId.ToString());

        var folder = await picker.PickSingleFolderAsync();
        if (folder != null)
        {
            txtRes4.Text = folder.Path;
        }
    }

    private async void btnPickMultipleFoldersAsync_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FolderPicker(WindowNative.GetWindowHandle(App.MainWindow));
        picker.Title = txtFolderPickerMultipleTitle.Text;
        picker.SuggestedFileName = txtFolderPickerMultipleSuggestedFileName.Text;
        picker.CommitButtonText = txtFolderPickerMultipleCommitButtonText.Text;
        picker.InitialDirectory = txtFolderPickerMultipleInitialDirectory.Text;

        var locationId = ViewModel.SuggestedStartLocationSelectedItem;
        if (locationId == null)
        {
            locationId = Windows.Storage.Pickers.PickerLocationId.Unspecified;
        }
        picker.SuggestedStartLocation = GeneralHelper.GetEnum<Windows.Storage.Pickers.PickerLocationId>(locationId.ToString());

        var folders = await picker.PickMultipleFoldersAsync();

        StringBuilder stringBuilder = new StringBuilder();
        foreach (var item in folders)
        {
            stringBuilder.AppendLine(item.Path.ToString());
        }
        txtRes5.Text = stringBuilder.ToString();
    }
}
