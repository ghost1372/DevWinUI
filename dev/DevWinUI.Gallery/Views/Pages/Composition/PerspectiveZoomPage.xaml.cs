namespace DevWinUIGallery.Views;

public sealed partial class PerspectiveZoomPage : Page
{
    public BaseViewModel ViewModel { get; }
    public PerspectiveZoomPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private void OnItemClick(object sender, ItemClickEventArgs e)
    {
        ListView listView = (ListView)sender;
        ListViewItem listItem = (ListViewItem)listView.ContainerFromItem(e.ClickedItem);

        PerspectiveSample.ToggleZoom(listItem);
    }
}
