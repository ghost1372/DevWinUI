namespace DevWinUIGallery.Views;

public sealed partial class KeyVisualPage : Page
{
    public KeyVisualViewModel ViewModel { get;}
    public KeyVisualPage()
    {
        ViewModel = App.GetService<KeyVisualViewModel>();
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
