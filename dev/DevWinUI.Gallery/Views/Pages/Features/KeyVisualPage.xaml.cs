namespace DevWinUIGallery.Views;

public sealed partial class KeyVisualPage : Page
{
    public BaseViewModel ViewModel { get;}
    public KeyVisualPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainKeyVisual != null && MainKeyVisual2 != null && MainKeyVisual3 != null)
        {
            MainKeyVisual.VisualType = (VisualType)VisualTypePicker.SelectedItem;
            MainKeyVisual2.VisualType = (VisualType)VisualTypePicker.SelectedItem;
            MainKeyVisual3.VisualType = (VisualType)VisualTypePicker.SelectedItem;
        }
    }
}
