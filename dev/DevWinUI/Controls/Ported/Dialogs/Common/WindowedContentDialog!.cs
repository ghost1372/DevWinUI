namespace DevWinUI;

public partial class WindowedContentDialog
{
    private DragMoveHelper dragMoveHelper;
    public bool HasTitleBar
    {
        get => (bool)GetValue(HasTitleBarProperty);
        set => SetValue(HasTitleBarProperty, value);
    }

    public static readonly DependencyProperty HasTitleBarProperty =
        DependencyProperty.Register(nameof(HasTitleBar), typeof(bool), typeof(WindowedContentDialog), new PropertyMetadata(true, OnHasTitleBarChanged));

    private static void OnHasTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WindowedContentDialog)d;
        if (ctl != null && ctl._window != null)
        {
            ctl._window?.HasTitleBar = (bool)e.NewValue;
        }
    }

    public FlowDirection FlowDirection
    {
        get { return (FlowDirection)GetValue(FlowDirectionProperty); }
        set { SetValue(FlowDirectionProperty, value); }
    }

    public static readonly DependencyProperty FlowDirectionProperty =
        DependencyProperty.Register(nameof(FlowDirection), typeof(FlowDirection), typeof(WindowedContentDialog), new PropertyMetadata(FlowDirection.LeftToRight, OnFlowDirectionChanged));

    private static void OnFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WindowedContentDialog)d;
        if (ctl != null && ctl._window != null)
        {
            ctl._window?.FlowDirection = (FlowDirection)e.NewValue;
        }
    }

    public bool CanResize
    {
        get { return (bool)GetValue(CanResizeProperty); }
        set { SetValue(CanResizeProperty, value); }
    }

    public static readonly DependencyProperty CanResizeProperty =
        DependencyProperty.Register(nameof(CanResize), typeof(bool), typeof(WindowedContentDialog), new PropertyMetadata(true, OnCanResizeChanged));

    private static void OnCanResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WindowedContentDialog)d;
        if (ctl != null && ctl._window != null)
        {
            ctl._window?.CanResize = (bool)e.NewValue;
        }
    }

    public bool CanDragMoveWindow
    {
        get { return (bool)GetValue(CanDragMoveWindowProperty); }
        set { SetValue(CanDragMoveWindowProperty, value); }
    }

    public static readonly DependencyProperty CanDragMoveWindowProperty =
        DependencyProperty.Register(nameof(CanDragMoveWindow), typeof(bool), typeof(WindowedContentDialog), new PropertyMetadata(false, OnCanDragMoveWindowChanged));

    private static void OnCanDragMoveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WindowedContentDialog)d;
        if (ctl != null && ctl._window != null)
        {
            if ((bool)e.NewValue)
            {
                ctl.dragMoveHelper.SetDragMove(ctl._view);
            }
            else
            {
                ctl.dragMoveHelper.UnSetDragMove(ctl._view);
            }
        }
    }

    public double MinWidth
    {
        get { return (double)GetValue(MinWidthProperty); }
        set { SetValue(MinWidthProperty, value); }
    }

    public static readonly DependencyProperty MinWidthProperty =
        DependencyProperty.Register(nameof(MinWidth), typeof(double), typeof(WindowedContentDialog), new PropertyMetadata(0.0d, OnMinWidthChanged));

    private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WindowedContentDialog)d;
        if (ctl != null && ctl._window != null)
        {
            ctl._window.MinWidth = (double)e.NewValue;
        }
    }

    public double MinHeight
    {
        get { return (double)GetValue(MinHeightProperty); }
        set { SetValue(MinHeightProperty, value); }
    }

    public static readonly DependencyProperty MinHeightProperty =
        DependencyProperty.Register(nameof(MinHeight), typeof(double), typeof(WindowedContentDialog), new PropertyMetadata(double.NaN, OnMinHeightChanged));

    private static void OnMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WindowedContentDialog)d;
        if (ctl != null && ctl._window != null)
        {
            ctl._window.MinHeight = (double)e.NewValue;
        }
    }
}
