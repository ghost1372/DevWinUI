using Microsoft.UI.Windowing;

namespace DevWinUI;
public partial class GrowlWindow : Window
{
    internal Panel GrowlPanel { get; set; }
    internal bool WindowClosed;
    private int GrowlWidth { get; set; }
    public GrowlWindow()
    {
        SystemBackdrop = new TransparentBackdrop();
        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Collapsed;
        AppWindow.IsShownInSwitchers = false;

        WindowHelper.RemoveWindowBorderAndTitleBar(this);

        GrowlPanel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            Spacing = 5,
            Tag = "x"
        };
        
        GrowlPanel.SizeChanged += (sender, args) =>
        {
            MoveAndResizeWindow();
        };

        Content = new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            Content = GrowlPanel
        };

        Closed += GrowlWindow_Closed;
    }

    private void GrowlWindow_Closed(object sender, WindowEventArgs args)
    {
        WindowClosed = true;
    }

    internal void MoveAndResizeWindow(bool isRightSide = true)
    {
        GrowlWidth = (int)(double)Application.Current.Resources["GrowlWidth"];

        int margin = 10;
        var displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest);
        var scale = GeneralHelper.GetElementRasterizationScale(GrowlPanel);
        GrowlWidth = (int)(GrowlWidth * scale);
        var xPosition = displayArea.WorkArea.Width - GrowlWidth - margin;

        if (!isRightSide)
        {
            xPosition = displayArea.WorkArea.X + margin;
        }

        double stackPanelHeight = 0;
        foreach (UIElement element in GrowlPanel.Children)
        {
            if (element is FrameworkElement frameworkElement)
            {
                double elementHeightWithMargin = (frameworkElement.ActualHeight + ((StackPanel)GrowlPanel).Spacing) * scale + frameworkElement.Margin.Top + frameworkElement.Margin.Bottom;
                stackPanelHeight += elementHeightWithMargin;
            }
        }

        if (stackPanelHeight > displayArea.WorkArea.Height)
        {
            stackPanelHeight = displayArea.WorkArea.Height;
        }

        if (GrowlPanel.Children.Count == 0 && Visible)
        {
            Close();
        }
        else
        {
            var position = new Windows.Graphics.RectInt32
            {
                Height = (int)stackPanelHeight,
                Width = GrowlWidth,
                X = xPosition,
                Y = displayArea.WorkArea.Y + margin
            };

            AppWindow?.MoveAndResize(position);
        }
    }
}
