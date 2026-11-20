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
            var visualType = (VisualType)VisualTypePicker.Value!;
            MainKeyVisual.VisualType = visualType;
            MainKeyVisual2.VisualType = visualType;
            MainKeyVisual3.VisualType = visualType;
        }
    }
}
