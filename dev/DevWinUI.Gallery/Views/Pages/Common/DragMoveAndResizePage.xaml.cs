namespace DevWinUIGallery.Views;

public sealed partial class DragMoveAndResizePage : Page
{
    public DragMoveAndResizePage()
    {
        this.InitializeComponent();
        Loaded += DragMoveAndResizePage_Loaded;

    }

    private void DragMoveAndResizePage_Loaded(object sender, RoutedEventArgs e)
    {
        DragMoveAndResizeHelper.SetDragMove(App.MainWindow, DragElement);
    }

    private void SampleTG_Toggled(object sender, RoutedEventArgs e)
    {
        if (DragElement == null)
        {
            return;
        }

        if (SampleTG.IsOn)
        {
            DragMoveAndResizeHelper.SetDragMove(App.MainWindow, DragElement);
        }
        else
        {
            DragMoveAndResizeHelper.UnsetDragMove(DragElement);
        }
    }
}
