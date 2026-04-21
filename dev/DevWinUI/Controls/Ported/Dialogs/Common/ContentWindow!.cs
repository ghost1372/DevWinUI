using Microsoft.UI.Windowing;

namespace DevWinUI;

public partial class ContentWindow
{
    private bool _hasTitleBar = true;
    public bool HasTitleBar
    {
        get => _hasTitleBar;
        set
        {
            _hasTitleBar = value;
            (Window.AppWindow.Presenter as OverlappedPresenter)?.SetBorderAndTitleBar(true, value);
        }
    }
}
