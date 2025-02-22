namespace DevWinUIGallery.Views;

public sealed partial class SampleModalWindow : ModalWindow
{
    public SampleModalWindow(IntPtr parentHwnd) : base(parentHwnd)
    {
        this.InitializeComponent();
        SystemBackdrop = new MicaSystemBackdrop();
        ExtendsContentIntoTitleBar = true;
    }
}
