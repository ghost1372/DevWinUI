namespace DevWinUIGallery.Views;

public sealed partial class KeyVisualPage : Page
{
    public ObservableCollection<VisualType> VisualTypeItems { get; set; } = new ObservableCollection<VisualType>(Enum.GetValues<VisualType>());

    public KeyVisualPage()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }

    private void VisualTypePicker_ValueChanged(ChoiceOption sender, object args)
    {
        if (MainKeyVisual != null && MainKeyVisual2 != null && MainKeyVisual3 != null)
        {
            MainKeyVisual.VisualType = (VisualType)VisualTypePicker.Value;
            MainKeyVisual2.VisualType = (VisualType)VisualTypePicker.Value;
            MainKeyVisual3.VisualType = (VisualType)VisualTypePicker.Value;
        }
    }
}
