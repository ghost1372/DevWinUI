namespace DevWinUI;
public partial class TitleBarAttach
{
    public static bool GetIsMaximizable(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsMaximizableProperty);
    }

    public static void SetIsMaximizable(DependencyObject obj, bool value)
    {
        obj.SetValue(IsMaximizableProperty, value);
    }

    public static readonly DependencyProperty IsMaximizableProperty =
        DependencyProperty.RegisterAttached("IsMaximizable", typeof(bool), typeof(TitleBarAttach), new PropertyMetadata(true, OnIsMaximizableChanged));

    private static void OnIsMaximizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement appTitleBar && appTitleBar != null)
        {
            var appWindow = WindowHelper.GetAppWindow(appTitleBar);

            ((OverlappedPresenter)appWindow.Presenter).IsMaximizable = (bool)e.NewValue;
        }
    }

    public static bool GetIsMinimizable(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsMinimizableProperty);
    }

    public static void SetIsMinimizable(DependencyObject obj, bool value)
    {
        obj.SetValue(IsMinimizableProperty, value);
    }

    public static readonly DependencyProperty IsMinimizableProperty =
        DependencyProperty.RegisterAttached("IsMinimizable", typeof(bool), typeof(TitleBarAttach), new PropertyMetadata(true, OnIsMinimizableChanged));


    private static void OnIsMinimizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement appTitleBar && appTitleBar != null)
        {
            var appWindow = WindowHelper.GetAppWindow(appTitleBar);

            ((OverlappedPresenter)appWindow.Presenter).IsMinimizable = (bool)e.NewValue;
        }
    }

    public static bool GetIsAlwaysOnTop(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsAlwaysOnTopProperty);
    }

    public static void SetIsAlwaysOnTop(DependencyObject obj, bool value)
    {
        obj.SetValue(IsAlwaysOnTopProperty, value);
    }

    public static readonly DependencyProperty IsAlwaysOnTopProperty =
        DependencyProperty.RegisterAttached("IsAlwaysOnTop", typeof(bool), typeof(TitleBarAttach), new PropertyMetadata(false, OnIsAlwaysOnTopChanged));


    private static void OnIsAlwaysOnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement appTitleBar && appTitleBar != null)
        {
            var appWindow = WindowHelper.GetAppWindow(appTitleBar);

            ((OverlappedPresenter)appWindow.Presenter).IsAlwaysOnTop = (bool)e.NewValue;
        }
    }

    public static bool GetIsResizable(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsResizableProperty);
    }

    public static void SetIsResizable(DependencyObject obj, bool value)
    {
        obj.SetValue(IsResizableProperty, value);
    }

    public static readonly DependencyProperty IsResizableProperty =
        DependencyProperty.RegisterAttached("IsResizable", typeof(bool), typeof(TitleBarAttach), new PropertyMetadata(true, OnIsResizableChanged));


    private static void OnIsResizableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement appTitleBar && appTitleBar != null)
        {
            var appWindow = WindowHelper.GetAppWindow(appTitleBar);

            ((OverlappedPresenter)appWindow.Presenter).IsResizable = (bool)e.NewValue;
        }
    }

    public static bool GetHasTitleBar(DependencyObject obj)
    {
        return (bool)obj.GetValue(HasTitleBarProperty);
    }

    public static void SetHasTitleBar(DependencyObject obj, bool value)
    {
        obj.SetValue(HasTitleBarProperty, value);
    }

    public static readonly DependencyProperty HasTitleBarProperty =
        DependencyProperty.RegisterAttached("HasTitleBar", typeof(bool), typeof(TitleBarAttach), new PropertyMetadata(true, OnHasTitleBarChanged));

    private static void OnHasTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement appTitleBar && appTitleBar != null)
        {
            var appWindow = WindowHelper.GetAppWindow(appTitleBar);

            var value = (bool)e.NewValue;

            if (value)
            {
                ((OverlappedPresenter)appWindow.Presenter).SetBorderAndTitleBar(true, true);
                ((OverlappedPresenter)appWindow.Presenter).IsResizable = true;
            }
            else
            {
                ((OverlappedPresenter)appWindow.Presenter).SetBorderAndTitleBar(false, false);
                ((OverlappedPresenter)appWindow.Presenter).IsResizable = false;
            }
        }
    }

    public static TitleBarHeightOption GetTitleBarHeightOption(DependencyObject obj)
    {
        return (TitleBarHeightOption)obj.GetValue(TitleBarHeightOptionProperty);
    }

    public static void SetTitleBarHeightOption(DependencyObject obj, TitleBarHeightOption value)
    {
        obj.SetValue(TitleBarHeightOptionProperty, value);
    }

    public static readonly DependencyProperty TitleBarHeightOptionProperty =
        DependencyProperty.RegisterAttached("TitleBarHeightOption", typeof(TitleBarHeightOption), typeof(TitleBarAttach), new PropertyMetadata(TitleBarHeightOption.Standard, OnTitleBarHeightOptionChanged));

    private static void OnTitleBarHeightOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement appTitleBar && appTitleBar != null)
        {
            var appWindow = WindowHelper.GetAppWindow(appTitleBar);

            appWindow.TitleBar.PreferredHeightOption = (TitleBarHeightOption)e.NewValue;
        }
    }
}
