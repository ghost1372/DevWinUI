namespace DevWinUI;

[TemplatePart(Name = PART_ConfirmButton, Type = typeof(Button))]
[TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
public partial class Growl : InfoBar
{
    private const string PART_ConfirmButton = "PART_ConfirmButton";
    private const string PART_CloseButton = "PART_CloseButton";
    private Button _confirmButton;
    private Button _closeButton;

    private DispatcherTimer timer;
    private Func<object, RoutedEventArgs, bool> ConfirmButtonClicked;
    private Func<object, RoutedEventArgs, bool> CloseButtonClicked;
    private static Dictionary<string, Panel> PanelDic = new Dictionary<string, Panel>();
    public static GrowlWindow GrowlWindow { get; private set; }
    public static Panel GrowlPanel { get; set; }

    public Growl()
    {
        Closed += Growl_Closed;
        Closing += Growl_Closing;
        Translation += new Vector3(0, 0, 4);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _confirmButton = GetTemplateChild(PART_ConfirmButton) as Button;
        _closeButton = GetTemplateChild(PART_CloseButton) as Button;

        if (_confirmButton != null)
        {
            _confirmButton.Click -= OnConfirmButtonClick;
            _confirmButton.Click += OnConfirmButtonClick;
        }

        if (_closeButton != null)
        {
            _closeButton.Click -= OnCloseButtonClick;
            _closeButton.Click += OnCloseButtonClick;
        }
    }

    private void Growl_Closing(InfoBar sender, InfoBarClosingEventArgs args)
    {
        IsOpen = true;
    }

    private void Growl_Closed(InfoBar sender, InfoBarClosedEventArgs args)
    {
        var panel = sender.Parent as Panel;
        RemoveGrowlFromPanel(panel);
    }

    public static void Register(string token, Panel panel)
    {
        if (string.IsNullOrEmpty(token) || panel == null) return;
        PanelDic[token] = panel;
    }

    public static void Unregister(Panel panel)
    {
        if (panel == null) return;
        var first = PanelDic.FirstOrDefault(item => ReferenceEquals(panel, item.Value));
        if (!string.IsNullOrEmpty(first.Key))
        {
            PanelDic.Remove(first.Key);
        }
    }

    private static void SetDefaultPanelTransition(Panel panel)
    {
        if (panel.ChildrenTransitions != null && panel.ChildrenTransitions.Count == 0)
        {
            var growlTransition = GetGrowlEnterTransition(panel);
            SetPanelTransition(growlTransition, panel);
        }
    }
    private static bool HasToken(DependencyObject element) => GetToken(element) != null;

    private static void SetPanelTransition(GrowlTransition growlTransition, Panel panel)
    {
        var transitions = new TransitionCollection();
        switch (growlTransition)
        {
            case GrowlTransition.AddDeleteThemeTransition:
                transitions.Add(new AddDeleteThemeTransition());
                break;
            case GrowlTransition.ContentThemeTransition:
                transitions.Add(new ContentThemeTransition());
                break;
            case GrowlTransition.EdgeUIThemeTransition:
                transitions.Add(new EdgeUIThemeTransition());
                break;
            case GrowlTransition.EntranceThemeTransition:
                transitions.Add(new EntranceThemeTransition());
                break;
            case GrowlTransition.NavigationThemeTransition:
                transitions.Add(new NavigationThemeTransition());
                break;
            case GrowlTransition.PaneThemeTransition:
                transitions.Add(new PaneThemeTransition());
                break;
            case GrowlTransition.PopupThemeTransition:
                transitions.Add(new PopupThemeTransition());
                break;
            case GrowlTransition.ReorderThemeTransition:
                transitions.Add(new ReorderThemeTransition());
                break;
            case GrowlTransition.RepositionThemeTransition:
                transitions.Add(new RepositionThemeTransition());
                break;
        }

        if (panel != null)
        {
            panel.ChildrenTransitions = transitions;
        }
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        if (CloseButtonClicked == null)
        {
            IsOpen = false;
        }
        else
        {
            bool result = CloseButtonClicked.Invoke(sender, e);
            if (result)
            {
                IsOpen = false;
            }
        }
    }

    private void RemoveGrowlFromPanel(Panel panel)
    {
        if (panel == null)
        {
            return;
        }

        var growlExitTransition = GetGrowlExitTransition(panel);
        SetPanelTransition(growlExitTransition, panel);

        panel.Children?.Remove(this);

        var growlEnterTransition = GetGrowlEnterTransition(panel);
        SetPanelTransition(growlEnterTransition, panel);
    }
    private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
    {
        if (ConfirmButtonClicked == null)
        {
            IsOpen = false;
        }
        else
        {
            bool result = ConfirmButtonClicked.Invoke(sender, e);
            if (result)
            {
                IsOpen = false;
            }
        }
    }

    private void AddBlueInfoResource()
    {
        Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri("ms-appx:///DevWinUI.Controls/Themes/InfoBarInformationalColor.xaml")
        });
    }

    private static void InitGrowl(GrowlInfo growlInfo, bool isForceSeverity = false, InfoBarSeverity forceSeverity = InfoBarSeverity.Informational)
    {
        var ctl = new Growl();
        ctl.Title = growlInfo.Title;
        ctl.Message = growlInfo.Message;
        ctl.IsIconVisible = growlInfo.IsIconVisible;
        ctl.IconSource = growlInfo.IconSource;
        ctl.IsClosable = growlInfo.IsClosable;
        ctl.Severity = isForceSeverity ? forceSeverity : growlInfo.Severity;
        ctl.DateTime = growlInfo.DateTime;
        if (string.IsNullOrEmpty(ctl.DateTime))
        {
            ctl.DateTime = DateTimeOffset.Now.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        ctl.ConfirmButtonText = growlInfo.ConfirmButtonText;
        ctl.CloseButtonText = growlInfo.CloseButtonText;
        ctl.ShowConfirmButton = growlInfo.ShowConfirmButton ? Visibility.Visible : Visibility.Collapsed;
        ctl.ShowCloseButton = growlInfo.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
        ctl.ShowDateTime = growlInfo.ShowDateTime && !string.IsNullOrEmpty(ctl.DateTime) ? Visibility.Visible : Visibility.Collapsed;

        if (growlInfo.ShowConfirmButton || growlInfo.ShowCloseButton)
        {
            ctl.RootGridMargin = new Thickness(0, 0, 0, 10);
        }

        ctl.CloseButtonClicked = growlInfo.CloseButtonClicked;
        ctl.ConfirmButtonClicked = growlInfo.ConfirmButtonClicked;

        if (growlInfo.UseBlueColorForInfo)
        {
            ctl.AddBlueInfoResource();
        }

        if (!string.IsNullOrEmpty(growlInfo.Token) && PanelDic.TryGetValue(growlInfo.Token, out var panel))
        {
            panel.Children.Insert(0, ctl);
            if (!growlInfo.StaysOpen)
            {
                ctl.SetupTimer(growlInfo.WaitTime, panel);
            }
        }
        else if (GrowlPanel != null)
        {
            GrowlPanel.Children.Insert(0, ctl);
            if (!growlInfo.StaysOpen)
            {
                ctl.SetupTimer(growlInfo.WaitTime, GrowlPanel);
            }
        }
    }
    private static void InitGrowlGlobal(GrowlInfo growlInfo, bool isForceSeverity = false, InfoBarSeverity forceSeverity = InfoBarSeverity.Informational)
    {
        var ctl = new Growl();
        ctl.Title = growlInfo.Title;
        ctl.Message = growlInfo.Message;
        ctl.IsIconVisible = growlInfo.IsIconVisible;
        ctl.IconSource = growlInfo.IconSource;
        ctl.IsClosable = growlInfo.IsClosable;
        ctl.Severity = isForceSeverity ? forceSeverity : growlInfo.Severity;
        ctl.DateTime = growlInfo.DateTime;
        if (string.IsNullOrEmpty(ctl.DateTime))
        {
            ctl.DateTime = DateTimeOffset.Now.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        ctl.ConfirmButtonText = growlInfo.ConfirmButtonText;
        ctl.CloseButtonText = growlInfo.CloseButtonText;
        ctl.ShowConfirmButton = growlInfo.ShowConfirmButton ? Visibility.Visible : Visibility.Collapsed;
        ctl.ShowCloseButton = growlInfo.ShowCloseButton ? Visibility.Visible : Visibility.Collapsed;
        ctl.ShowDateTime = growlInfo.ShowDateTime && !string.IsNullOrEmpty(ctl.DateTime) ? Visibility.Visible : Visibility.Collapsed;

        if (growlInfo.ShowConfirmButton || growlInfo.ShowCloseButton)
        {
            ctl.RootGridMargin = new Thickness(0, 0, 0, 10);
        }

        ctl.CloseButtonClicked = growlInfo.CloseButtonClicked;
        ctl.ConfirmButtonClicked = growlInfo.ConfirmButtonClicked;

        if (growlInfo.UseBlueColorForInfo)
        {
            ctl.AddBlueInfoResource();
        }

        if (GrowlWindow == null || GrowlWindow.WindowClosed)
        {
            GrowlWindow = new GrowlWindow();
            GrowlWindow.Init();
        }

        switch (growlInfo.Severity)
        {
            case InfoBarSeverity.Informational:
                ctl.Background = Application.Current.Resources["InfoBarInformationalSeverityBackgroundBrush"] as Brush;
                break;
            case InfoBarSeverity.Success:
                ctl.Background = Application.Current.Resources["InfoBarSuccessSeverityBackgroundBrush"] as Brush;
                break;
            case InfoBarSeverity.Warning:
                ctl.Background = Application.Current.Resources["InfoBarWarningSeverityBackgroundBrush"] as Brush;
                break;
            case InfoBarSeverity.Error:
                ctl.Background = Application.Current.Resources["InfoBarErrorSeverityBackgroundBrush"] as Brush;
                break;
        }

        SetPanelTransition(GrowlTransition.AddDeleteThemeTransition, GrowlWindow.GrowlPanel);

        if (GrowlWindow.GrowlPanel != null)
        {
            GrowlWindow.GrowlPanel.Children.Insert(0, ctl);
            if (!growlInfo.StaysOpen)
            {
                ctl.SetupTimer(growlInfo.WaitTime, GrowlWindow.GrowlPanel);
            }
        }

        GrowlWindow?.Activate();
    }

    private void SetupTimer(TimeSpan timeSpan, Panel panel)
    {
        timer = new DispatcherTimer();
        timer.Interval = timeSpan;
        timer.Tick += (sender, e) =>
        {
            RemoveGrowlFromPanel(panel);
            timer.Stop(); // Stop the timer after it's ticked
        };
        timer.Start();
    }

    private static void Clear(Panel panel) => panel?.Children.Clear();
    public static void Clear(string token = "")
    {
        if (!string.IsNullOrEmpty(token))
        {
            if (PanelDic.TryGetValue(token, out var panel))
            {
                Clear(panel);
            }
        }
        else
        {
            Clear(GrowlPanel);
        }
    }

    public static void ClearGlobal()
    {
        if (GrowlWindow == null) return;
        Clear(GrowlWindow.GrowlPanel);
        GrowlWindow.Close();
        GrowlWindow = null;
    }
}
