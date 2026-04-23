namespace DevWinUIGallery.Views;

public sealed partial class DragMoveAndResizePage : Page
{
    private DragMoveHelper dragMoveHelper;
    public DragMoveAndResizePage()
    {
        this.InitializeComponent();
        dragMoveHelper = new DragMoveHelper(App.Hwnd);
        Loaded += DragMoveAndResizePage_Loaded;

    }

    private void DragMoveAndResizePage_Loaded(object sender, RoutedEventArgs e)
    {
        dragMoveHelper.SetDragMove(DragElement);
    }

    private void SampleTG_Toggled(object sender, RoutedEventArgs e)
    {
        if (DragElement == null)
        {
            return;
        }

        if (SampleTG.IsOn)
        {
            dragMoveHelper.SetDragMove(DragElement);
        }
        else
        {
            dragMoveHelper.UnSetDragMove(DragElement);
        }
    }
}
