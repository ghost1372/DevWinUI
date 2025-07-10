using DevWinUIGallery.Models;

namespace DevWinUIGallery.Views;

public sealed partial class ForegroundFocusEffectsPage : Page
{
    public BaseViewModel ViewModel { get; }

    public ForegroundFocusEffectsPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private async void OnItemClick(object sender, ItemClickEventArgs e)
    {
        var item = (SampleData)e.ClickedItem;

        ForegroundFocusEffectsSample.ApplyEffect();

        ContentDialog contentDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = item.Name,
            Content = item.Description,
            PrimaryButtonText = "OK",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary
        };
        contentDialog.PrimaryButtonClick += ContentDialog_PrimaryButtonClick;
        contentDialog.CloseButtonClick += ContentDialog_PrimaryButtonClick;
        await contentDialog.ShowAsyncQueue();
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ForegroundFocusEffectsSample.RemoveEffect();
    }
}
