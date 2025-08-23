namespace DevWinUI;

[TemplatePart(Name = nameof(PART_UpdateNotAvailableIconBorder), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_UpdateAvailableIconBorder), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_UpdateNotAvailableTitleStackPanel), Type = typeof(StackPanel))]
[TemplatePart(Name = nameof(PART_UpdateAvailableTitleStackPanel), Type = typeof(StackPanel))]
[TemplatePart(Name = nameof(PART_UpdateNotAvailableIconPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_UpdateNotAvailableTitlePresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_UpdateAvailableIconPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_UpdateAvailableTitlePresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_UpdateAvailableButton), Type = typeof(Button))]
public partial class CheckUpdateControl : Control
{
    private const string PART_UpdateAvailableButton = "PART_UpdateAvailableButton";
    private const string PART_UpdateAvailableTitlePresenter = "PART_UpdateAvailableTitlePresenter";
    private const string PART_UpdateAvailableTitleStackPanel = "PART_UpdateAvailableTitleStackPanel";
    private const string PART_UpdateAvailableIconPresenter = "PART_UpdateAvailableIconPresenter";
    private const string PART_UpdateAvailableIconBorder = "PART_UpdateAvailableIconBorder";

    private const string PART_UpdateNotAvailableTitlePresenter = "PART_UpdateNotAvailableTitlePresenter";
    private const string PART_UpdateNotAvailableTitleStackPanel = "PART_UpdateNotAvailableTitleStackPanel";
    private const string PART_UpdateNotAvailableIconPresenter = "PART_UpdateNotAvailableIconPresenter";
    private const string PART_UpdateNotAvailableIconBorder = "PART_UpdateNotAvailableIconBorder";

    private ContentPresenter iconAvailablePresenter;
    private Border IconAvailableBorder;
    private ContentPresenter iconNotAvailablePresenter;
    private Border iconNotAvailableBorder;
    private ContentPresenter titleAvailablePresenter;
    private StackPanel titleAvailableStackPanel;
    private ContentPresenter titleNotAvailablePresenter;
    private StackPanel titleNotAvailableStackPanel;

    private Button updateAvailableButton;

    public event EventHandler<RoutedEventArgs> Click;

    public bool IsUpdateAvailable
    {
        get { return (bool)GetValue(IsUpdateAvailableProperty); }
        set { SetValue(IsUpdateAvailableProperty, value); }
    }

    public static readonly DependencyProperty IsUpdateAvailableProperty =
        DependencyProperty.Register(nameof(IsUpdateAvailable), typeof(bool), typeof(CheckUpdateControl), new PropertyMetadata(false));

    public object UpdateAvailableTitle
    {
        get { return (object)GetValue(UpdateAvailableTitleProperty); }
        set { SetValue(UpdateAvailableTitleProperty, value); }
    }

    public static readonly DependencyProperty UpdateAvailableTitleProperty =
        DependencyProperty.Register(nameof(UpdateAvailableTitle), typeof(object), typeof(CheckUpdateControl), new PropertyMetadata(null, OnUpdateAvailableTitleChanged));

    private static void OnUpdateAvailableTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CheckUpdateControl)d;
        if (ctl != null)
        {
            ctl.OnUpdateAvailableTitleChanged();
        }
    }

    public string UpdateAvailableVersionTitle
    {
        get { return (string)GetValue(UpdateAvailableVersionTitleProperty); }
        set { SetValue(UpdateAvailableVersionTitleProperty, value); }
    }

    public static readonly DependencyProperty UpdateAvailableVersionTitleProperty =
        DependencyProperty.Register(nameof(UpdateAvailableVersionTitle), typeof(string), typeof(CheckUpdateControl), new PropertyMetadata(null));

    public string UpdateAvailableVersion
    {
        get { return (string)GetValue(UpdateAvailableVersionProperty); }
        set { SetValue(UpdateAvailableVersionProperty, value); }
    }

    public static readonly DependencyProperty UpdateAvailableVersionProperty =
        DependencyProperty.Register(nameof(UpdateAvailableVersion), typeof(string), typeof(CheckUpdateControl), new PropertyMetadata(null));

    public object UpdateAvailableIcon
    {
        get { return (object)GetValue(UpdateAvailableIconProperty); }
        set { SetValue(UpdateAvailableIconProperty, value); }
    }

    public static readonly DependencyProperty UpdateAvailableIconProperty =
        DependencyProperty.Register(nameof(UpdateAvailableIcon), typeof(object), typeof(CheckUpdateControl), new PropertyMetadata(null, OnUpdateAvailableIconChanged));
    private static void OnUpdateAvailableIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CheckUpdateControl)d;
        if (ctl != null)
        {
            ctl.OnUpdateAvailableIconChanged();
        }
    }
    public object UpdateNotAvailableTitle
    {
        get { return (object)GetValue(UpdateNotAvailableTitleProperty); }
        set { SetValue(UpdateNotAvailableTitleProperty, value); }
    }

    public static readonly DependencyProperty UpdateNotAvailableTitleProperty =
        DependencyProperty.Register(nameof(UpdateNotAvailableTitle), typeof(object), typeof(CheckUpdateControl), new PropertyMetadata(null, OnUpdateNotAvailableTitleChanged));

    private static void OnUpdateNotAvailableTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CheckUpdateControl)d;
        if (ctl != null)
        {
            ctl.OnUpdateNotAvailableTitleChanged();
        }
    }
    public string LastUpdateCheckTitle
    {
        get { return (string)GetValue(LastUpdateCheckTitleProperty); }
        set { SetValue(LastUpdateCheckTitleProperty, value); }
    }

    public static readonly DependencyProperty LastUpdateCheckTitleProperty =
        DependencyProperty.Register(nameof(LastUpdateCheckTitle), typeof(string), typeof(CheckUpdateControl), new PropertyMetadata(null));


    public string LastUpdateCheckDate
    {
        get { return (string)GetValue(LastUpdateCheckDateProperty); }
        set { SetValue(LastUpdateCheckDateProperty, value); }
    }

    public static readonly DependencyProperty LastUpdateCheckDateProperty =
        DependencyProperty.Register(nameof(LastUpdateCheckDate), typeof(string), typeof(CheckUpdateControl), new PropertyMetadata(null));

    public object UpdateNotAvailableIcon
    {
        get { return (object)GetValue(UpdateNotAvailableIconProperty); }
        set { SetValue(UpdateNotAvailableIconProperty, value); }
    }

    public static readonly DependencyProperty UpdateNotAvailableIconProperty =
        DependencyProperty.Register(nameof(UpdateNotAvailableIcon), typeof(object), typeof(CheckUpdateControl), new PropertyMetadata(null, OnUpdateNotAvailableIconChanged));
    private static void OnUpdateNotAvailableIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CheckUpdateControl)d;
        if (ctl != null)
        {
            ctl.OnUpdateNotAvailableIconChanged();
        }
    }
    public CheckUpdateControl()
    {
        DefaultStyleKey = typeof(CheckUpdateControl);
    }
    
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        titleAvailablePresenter = GetTemplateChild(PART_UpdateAvailableTitlePresenter) as ContentPresenter;
        titleAvailableStackPanel = GetTemplateChild(PART_UpdateAvailableTitleStackPanel) as StackPanel;
        iconAvailablePresenter = GetTemplateChild(PART_UpdateAvailableIconPresenter) as ContentPresenter;
        IconAvailableBorder = GetTemplateChild(PART_UpdateAvailableIconBorder) as Border;

        titleNotAvailablePresenter = GetTemplateChild(PART_UpdateNotAvailableTitlePresenter) as ContentPresenter;
        titleNotAvailableStackPanel = GetTemplateChild(PART_UpdateNotAvailableTitleStackPanel) as StackPanel;
        iconNotAvailablePresenter = GetTemplateChild(PART_UpdateNotAvailableIconPresenter) as ContentPresenter;
        iconNotAvailableBorder = GetTemplateChild(PART_UpdateNotAvailableIconBorder) as Border;

        updateAvailableButton = GetTemplateChild(PART_UpdateAvailableButton) as Button;
        updateAvailableButton.Click -= OnUpdateAvailableButton;
        updateAvailableButton.Click += OnUpdateAvailableButton;

        OnUpdateAvailableTitleChanged();
        OnUpdateAvailableIconChanged();
        OnUpdateNotAvailableTitleChanged();
        OnUpdateNotAvailableIconChanged();
    }

    private void OnUpdateAvailableButton(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }

    private void OnUpdateAvailableTitleChanged()
    {
        if (titleAvailablePresenter == null || titleAvailableStackPanel == null)
            return;

        if (UpdateAvailableTitle == null)
        {
            titleAvailablePresenter.Visibility = Visibility.Collapsed;
            titleAvailableStackPanel.Visibility = Visibility.Collapsed;
        }
        else if (UpdateAvailableTitle is string)
        {
            titleAvailablePresenter.Visibility = Visibility.Collapsed;
            titleAvailableStackPanel.Visibility = Visibility.Visible;
        }
        else
        {
            titleAvailablePresenter.Visibility = Visibility.Visible;
            titleAvailableStackPanel.Visibility = Visibility.Collapsed;
        }
    }
    private void OnUpdateAvailableIconChanged()
    {
        if (iconAvailablePresenter == null || IconAvailableBorder == null)
            return;

        if (UpdateAvailableIcon == null)
        {
            iconAvailablePresenter.Visibility = Visibility.Collapsed;
            IconAvailableBorder.Visibility = Visibility.Visible;
        }
        else
        {
            iconAvailablePresenter.Visibility = Visibility.Visible;
            IconAvailableBorder.Visibility = Visibility.Collapsed;
        }
    }
    private void OnUpdateNotAvailableTitleChanged()
    {
        if (titleNotAvailablePresenter == null || titleNotAvailableStackPanel == null)
            return;

        if (UpdateNotAvailableTitle == null)
        {
            titleNotAvailablePresenter.Visibility = Visibility.Collapsed;
            titleNotAvailableStackPanel.Visibility = Visibility.Collapsed;
        }
        else if (UpdateNotAvailableTitle is string)
        {
            titleNotAvailablePresenter.Visibility = Visibility.Collapsed;
            titleNotAvailableStackPanel.Visibility = Visibility.Visible;
        }
        else
        {
            titleNotAvailablePresenter.Visibility = Visibility.Visible;
            titleNotAvailableStackPanel.Visibility = Visibility.Collapsed;
        }
    }
    private void OnUpdateNotAvailableIconChanged()
    {
        if (iconNotAvailablePresenter == null || iconNotAvailableBorder == null)
            return;

        if (UpdateNotAvailableIcon == null)
        {
            iconNotAvailablePresenter.Visibility = Visibility.Collapsed;
            iconNotAvailableBorder.Visibility = Visibility.Visible;
        }
        else
        {
            iconNotAvailablePresenter.Visibility = Visibility.Visible;
            iconNotAvailableBorder.Visibility = Visibility.Collapsed;
        }
    }
}
