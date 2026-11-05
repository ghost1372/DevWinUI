namespace DevWinUI;

internal sealed partial class EyeDropperPreviewWindow : Window
{
    private readonly SolidColorBrush previewBrush = new SolidColorBrush();
    private Color lastColor;

    public IntPtr hwnd { get; private set; }
    public EyeDropperPreviewWindow()
    {
        this.InitializeComponent();

        hwnd = WindowNative.GetWindowHandle(this);

        Title = "EyeDropper Preview Window";
        AppWindow.Title = Title;

        var presenter = (OverlappedPresenter)AppWindow.Presenter;
        presenter.IsMinimizable = false;
        presenter.IsMaximizable = false;
        presenter.IsResizable = false;
        presenter.IsAlwaysOnTop = true;
        presenter.PreferredMaximumHeight = 65;
        presenter.PreferredMaximumWidth = 200;
        presenter.PreferredMinimumHeight = 65;
        presenter.PreferredMinimumWidth = 200;

        AppWindow.IsShownInSwitchers = false;

        WindowHelper.RemoveWindowBorderAndTitleBar(this);

        PreviewBox.Background = previewBrush;
    }

    public void UpdateColor(Color color)
    {
        if (color == lastColor)
        {
            return;
        }

        lastColor = color;
        previewBrush.Color = color;
        TxtHex.Text = ColorHelper.GetHexFromColor(color);
    }
}
