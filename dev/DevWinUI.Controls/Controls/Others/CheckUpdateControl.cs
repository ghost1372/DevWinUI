namespace DevWinUI;

[TemplatePart(Name = nameof(PART_UpdateAvailableButton), Type = typeof(Button))]
public partial class CheckUpdateControl : Control
{
    private const string PART_UpdateAvailableButton = "PART_UpdateAvailableButton";
    private Button updateAvailableButton;

    public event EventHandler<RoutedEventArgs> Click;

    public bool IsUpdateAvailable
    {
        get { return (bool)GetValue(IsUpdateAvailableProperty); }
        set { SetValue(IsUpdateAvailableProperty, value); }
    }

    public static readonly DependencyProperty IsUpdateAvailableProperty =
        DependencyProperty.Register(nameof(IsUpdateAvailable), typeof(bool), typeof(CheckUpdateControl), new PropertyMetadata(false));

    public string UpdateAvailableTitle
    {
        get { return (string)GetValue(UpdateAvailableTitleProperty); }
        set { SetValue(UpdateAvailableTitleProperty, value); }
    }

    public static readonly DependencyProperty UpdateAvailableTitleProperty =
        DependencyProperty.Register(nameof(UpdateAvailableTitle), typeof(string), typeof(CheckUpdateControl), new PropertyMetadata(null));

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

    public DataTemplate UpdateAvailableTemplate
    {
        get { return (DataTemplate)GetValue(UpdateAvailableTemplateProperty); }
        set { SetValue(UpdateAvailableTemplateProperty, value); }
    }

    public static readonly DependencyProperty UpdateAvailableTemplateProperty =
        DependencyProperty.Register(nameof(UpdateAvailableTemplate), typeof(DataTemplate), typeof(CheckUpdateControl), new PropertyMetadata(null));

    public DataTemplate UpdateAvailableIconTemplate
    {
        get { return (DataTemplate)GetValue(UpdateAvailableIconTemplateProperty); }
        set { SetValue(UpdateAvailableIconTemplateProperty, value); }
    }

    public static readonly DependencyProperty UpdateAvailableIconTemplateProperty =
        DependencyProperty.Register(nameof(UpdateAvailableIconTemplate), typeof(DataTemplate), typeof(CheckUpdateControl), new PropertyMetadata(null));

    public string UpdateNotAvailableTitle
    {
        get { return (string)GetValue(UpdateNotAvailableTitleProperty); }
        set { SetValue(UpdateNotAvailableTitleProperty, value); }
    }

    public static readonly DependencyProperty UpdateNotAvailableTitleProperty =
        DependencyProperty.Register(nameof(UpdateNotAvailableTitle), typeof(string), typeof(CheckUpdateControl), new PropertyMetadata(null));


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

    public DataTemplate UpdateNotAvailableTemplate
    {
        get { return (DataTemplate)GetValue(UpdateNotAvailableTemplateProperty); }
        set { SetValue(UpdateNotAvailableTemplateProperty, value); }
    }

    public static readonly DependencyProperty UpdateNotAvailableTemplateProperty =
        DependencyProperty.Register(nameof(UpdateNotAvailableTemplate), typeof(DataTemplate), typeof(CheckUpdateControl), new PropertyMetadata(null));

    public DataTemplate UpdateNotAvailableIconTemplate
    {
        get { return (DataTemplate)GetValue(UpdateNotAvailableIconTemplateProperty); }
        set { SetValue(UpdateNotAvailableIconTemplateProperty, value); }
    }

    public static readonly DependencyProperty UpdateNotAvailableIconTemplateProperty =
        DependencyProperty.Register(nameof(UpdateNotAvailableIconTemplate), typeof(DataTemplate), typeof(CheckUpdateControl), new PropertyMetadata(null));

    public CheckUpdateControl()
    {
        DefaultStyleKey = typeof(CheckUpdateControl);
    }
    
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        updateAvailableButton = GetTemplateChild(PART_UpdateAvailableButton) as Button;
        updateAvailableButton.Click -= OnUpdateAvailableButton;
        updateAvailableButton.Click += OnUpdateAvailableButton;
    }

    private void OnUpdateAvailableButton(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }
}
