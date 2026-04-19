using System.Windows.Input;

namespace DevWinUI;

public partial class RichButton : Control
{
    private const string PART_Button = "PART_Button";
    private const string PART_TitleStackPanel = "PART_TitleStackPanel";

    private Button button;
    private StackPanel stackPanel;

    public event EventHandler<RoutedEventArgs> Click;

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(RichButton), new PropertyMetadata(null));

    public object CommandParameter
    {
        get { return (object)GetValue(CommandParameterProperty); }
        set { SetValue(CommandParameterProperty, value); }
    }

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(RichButton), new PropertyMetadata(null));

    public object ActionIcon
    {
        get { return (object)GetValue(ActionIconProperty); }
        set { SetValue(ActionIconProperty, value); }
    }

    public static readonly DependencyProperty ActionIconProperty =
        DependencyProperty.Register(nameof(ActionIcon), typeof(object), typeof(RichButton), new PropertyMetadata(null));

    public object Icon
    {
        get { return (object)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(object), typeof(RichButton), new PropertyMetadata(null, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RichButton)d;
        if (ctl != null)
        {
            ctl.UpdateMargin();
        }
    }

    public object Title
    {
        get { return (object)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(object), typeof(RichButton), new PropertyMetadata(null, OnPropertyChanged));

    public object SubTitle
    {
        get { return (object)GetValue(SubTitleProperty); }
        set { SetValue(SubTitleProperty, value); }
    }

    public static readonly DependencyProperty SubTitleProperty =
        DependencyProperty.Register(nameof(SubTitle), typeof(object), typeof(RichButton), new PropertyMetadata(null, OnPropertyChanged));

    public Brush TitleForeground
    {
        get { return (Brush)GetValue(TitleForegroundProperty); }
        set { SetValue(TitleForegroundProperty, value); }
    }

    public static readonly DependencyProperty TitleForegroundProperty =
        DependencyProperty.Register(nameof(TitleForeground), typeof(Brush), typeof(RichButton), new PropertyMetadata(null));

    public Brush SubTitleForeground
    {
        get { return (Brush)GetValue(SubTitleForegroundProperty); }
        set { SetValue(SubTitleForegroundProperty, value); }
    }

    public static readonly DependencyProperty SubTitleForegroundProperty =
        DependencyProperty.Register(nameof(SubTitleForeground), typeof(Brush), typeof(RichButton), new PropertyMetadata(null));

    public RichButtonDisplayMode DisplayMode
    {
        get { return (RichButtonDisplayMode)GetValue(DisplayModeProperty); }
        set { SetValue(DisplayModeProperty, value); }
    }

    public static readonly DependencyProperty DisplayModeProperty =
        DependencyProperty.Register(nameof(DisplayMode), typeof(RichButtonDisplayMode), typeof(RichButton), new PropertyMetadata(RichButtonDisplayMode.Subtle, OnDisplayModeChanged));

    private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RichButton)d;
        if (ctl != null)
        {
            ctl.UpdateDisplayMode();
        }
    }


    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        button = GetTemplateChild(PART_Button) as Button;
        stackPanel = GetTemplateChild(PART_TitleStackPanel) as StackPanel;

        button.Click -= OnClick;
        button.Click += OnClick;

        UpdateDisplayMode();

        UpdateMargin();
    }
    private void UpdateMargin()
    {
        if (stackPanel == null)
            return;

        if (Icon != null && (Title != null || SubTitle != null))
            stackPanel.Margin = new(16, 0, 0, 0);
        else if (Icon != null)
            stackPanel.Margin = new(0, 0, 0, 0);
        else
            stackPanel.Margin = new(0, 0, 0, 0);
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }

    private void UpdateDisplayMode()
    {
        if (button == null)
            return;

        switch (DisplayMode)
        {
            case RichButtonDisplayMode.Normal:
                button.Style = Application.Current.Resources["DefaultButtonStyle"] as Style;

                break;
            case RichButtonDisplayMode.Subtle:
                button.Style = Application.Current.Resources["SubtleButtonStyle"] as Style;

                break;
            case RichButtonDisplayMode.ReadOnly:
                button.Style = Application.Current.Resources["NoEffectSubtleButtonStyle"] as Style;

                break;
        }
    }
}
