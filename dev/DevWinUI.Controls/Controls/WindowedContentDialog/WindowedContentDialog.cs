//https://github.com/SuGar0218/WindowedContentDialog

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class WindowedContentDialog : Control, IStandaloneContentDialog
{
    private Rectangle smokeLayerCache;
    private Border backdropLayerCache;
    public WindowedContentDialog()
    {
        DefaultStyleKey = typeof(WindowedContentDialog);
        ContentDialogContent = new ContentDialogContent();
        InitializeRelayDependencyProperties();
    }

    private void InitializeContentDialogWindow()
    {
        ContentDialogWindow = ContentDialogWindow.CreateWithoutComponent();
        ContentDialogWindow.InitializeComponent(ContentDialogContent);
        ContentDialogWindow.RequestedTheme = DetermineTheme();
        ContentDialogWindow.Title = WindowTitle;
        ContentDialogWindow.SystemBackdrop = SystemBackdrop;
        ContentDialogWindow.BorderBrush = BorderBrush;
        ContentDialogWindow.BorderThickness = BorderThickness;
        ContentDialogWindow.PrimaryButtonClick += (sender, args) => PrimaryButtonClick?.Invoke(this, args);
        ContentDialogWindow.SecondaryButtonClick += (sender, args) => SecondaryButtonClick?.Invoke(this, args);
        ContentDialogWindow.CloseButtonClick += (sender, args) => CloseButtonClick?.Invoke(this, args);
        ContentDialogWindow.Opened += (sender, args) => Opened?.Invoke(this, args);
        ContentDialogWindow.Closed += (sender, args) => Closed?.Invoke(this, args);
    }

    public event TypedEventHandler<WindowedContentDialog, CancelEventArgs>? PrimaryButtonClick;
    public event TypedEventHandler<WindowedContentDialog, CancelEventArgs>? SecondaryButtonClick;
    public event TypedEventHandler<WindowedContentDialog, CancelEventArgs>? CloseButtonClick;
    public event TypedEventHandler<WindowedContentDialog, EventArgs>? Opened;
    public event TypedEventHandler<WindowedContentDialog, WindowEventArgs>? Closed;
    public IList<KeyboardAccelerator> PrimaryButtonKeyboardAccelerators => ContentDialogContent.PrimaryButtonKeyboardAccelerators;
    public IList<KeyboardAccelerator> SecondaryButtonKeyboardAccelerators => ContentDialogContent.SecondaryButtonKeyboardAccelerators;
    public IList<KeyboardAccelerator> CloseButtonKeyboardAccelerators => ContentDialogContent.CloseButtonKeyboardAccelerators;
    #region properties

    public object? Title
    {
        get => (object?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(null, (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.Title = (object?)e.NewValue;
        })
    );

    public object? Content
    {
        get => (object?)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(null, (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.Content = (object?)e.NewValue;
        })
    );

    public string? PrimaryButtonText
    {
        get => (string?)GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonTextProperty = DependencyProperty.Register(
        nameof(PrimaryButtonText),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(null, (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.PrimaryButtonText = (string?)e.NewValue;
        })
    );

    public string? SecondaryButtonText
    {
        get => (string?)GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonTextProperty = DependencyProperty.Register(
        nameof(SecondaryButtonText),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(null, (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.SecondaryButtonText = (string?)e.NewValue;
        })
    );

    public string? CloseButtonText
    {
        get => (string?)GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }

    public static readonly DependencyProperty CloseButtonTextProperty = DependencyProperty.Register(
        nameof(CloseButtonText),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(null, (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.CloseButtonText = (string?)e.NewValue;
        })
    );

    public DataTemplate? TitleTemplate
    {
        get => (DataTemplate)GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
    }

    public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
        nameof(TitleTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate), (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.TitleTemplate = (DataTemplate)e.NewValue;
        })
    );

    public DataTemplate? ContentTemplate
    {
        get => (DataTemplate)GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
        nameof(ContentTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate), (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.ContentTemplate = (DataTemplate)e.NewValue;
        })
    );

    public DataTemplateSelector? ContentTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
        set => SetValue(ContentTemplateSelectorProperty, value);
    }

    public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register(
        nameof(ContentTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplateSelector), (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.ContentTemplateSelector = (DataTemplateSelector)e.NewValue;
        })
    );

    public ContentDialogButton DefaultButton
    {
        get => (ContentDialogButton)GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(
        nameof(DefaultButton),
        typeof(ContentDialogButton),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(ContentDialogButton), (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.DefaultButton = (ContentDialogButton)e.NewValue;
        })
    );

    public bool IsPrimaryButtonEnabled
    {
        get => (bool)GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsPrimaryButtonEnabled),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(false, (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.IsPrimaryButtonEnabled = (bool)e.NewValue;
        })
    );

    public bool IsSecondaryButtonEnabled
    {
        get => (bool)GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsSecondaryButtonEnabled),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(false, (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.IsSecondaryButtonEnabled = (bool)e.NewValue;
        })
    );

    public Style? PrimaryButtonStyle
    {
        get => (Style)GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register(
        nameof(PrimaryButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style), (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.PrimaryButtonStyle = (Style)e.NewValue;
        })
    );

    public Style? SecondaryButtonStyle
    {
        get => (Style)GetValue(SecondaryButtonStyleProperty);
        set => SetValue(SecondaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonStyleProperty = DependencyProperty.Register(
        nameof(SecondaryButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style), (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.SecondaryButtonStyle = (Style)e.NewValue;
        })
    );

    public Style? CloseButtonStyle
    {
        get => (Style)GetValue(CloseButtonStyleProperty);
        set => SetValue(CloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(
        nameof(CloseButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style), (d, e) =>
        {
            WindowedContentDialog self = (WindowedContentDialog)d;
            self.ContentDialogContent.CloseButtonStyle = (Style)e.NewValue;
        })
    );

    /// <summary>
    /// Generated by RelayDependencyPropertyGenerator.
    /// <br/>
    /// Sync value from target properties to relay dependency properties.
    /// <br/>
    /// Please invoke this after target properties object is prepared to be accessed.
    /// </summary>
    private void InitializeRelayDependencyProperties()
    {
        Title = ContentDialogContent.Title;

        Content = ContentDialogContent.Content;

        PrimaryButtonText = ContentDialogContent.PrimaryButtonText;

        SecondaryButtonText = ContentDialogContent.SecondaryButtonText;

        CloseButtonText = ContentDialogContent.CloseButtonText;

        TitleTemplate = ContentDialogContent.TitleTemplate;

        ContentTemplate = ContentDialogContent.ContentTemplate;

        ContentTemplateSelector = ContentDialogContent.ContentTemplateSelector;

        DefaultButton = ContentDialogContent.DefaultButton;

        IsPrimaryButtonEnabled = ContentDialogContent.IsPrimaryButtonEnabled;

        IsSecondaryButtonEnabled = ContentDialogContent.IsSecondaryButtonEnabled;

        PrimaryButtonStyle = ContentDialogContent.PrimaryButtonStyle;

        SecondaryButtonStyle = ContentDialogContent.SecondaryButtonStyle;

        CloseButtonStyle = ContentDialogContent.CloseButtonStyle;
    }
    public UnderlayMode Underlay
    {
        get { return (UnderlayMode)GetValue(UnderlayProperty); }
        set { SetValue(UnderlayProperty, value); }
    }

    public static readonly DependencyProperty UnderlayProperty =
        DependencyProperty.Register(nameof(Underlay), typeof(UnderlayMode), typeof(WindowedContentDialog), new PropertyMetadata(UnderlayMode.SmokeLayer));

    public UnderlaySystemBackdropOptions UnderlaySystemBackdrop
    {
        get { return (UnderlaySystemBackdropOptions)GetValue(UnderlaySystemBackdropProperty); }
        set { SetValue(UnderlaySystemBackdropProperty, value); }
    }

    public static readonly DependencyProperty UnderlaySystemBackdropProperty =
        DependencyProperty.Register(nameof(UnderlaySystemBackdrop), typeof(UnderlaySystemBackdropOptions), typeof(WindowedContentDialog), new PropertyMetadata(new UnderlaySystemBackdropOptions()));

    public string? WindowTitle
    {
        get => (string?)GetValue(WindowTitleProperty);
        set => SetValue(WindowTitleProperty, value);
    }

    public static readonly DependencyProperty WindowTitleProperty = DependencyProperty.Register(
        nameof(WindowTitle),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(null)
    );

    public SystemBackdrop? SystemBackdrop
    {
        get => (SystemBackdrop)GetValue(SystemBackdropProperty);
        set => SetValue(SystemBackdropProperty, value);
    }

    public static readonly DependencyProperty SystemBackdropProperty = DependencyProperty.Register(
        nameof(SystemBackdrop),
        typeof(SystemBackdrop),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(SystemBackdrop))
    );

    public bool IsResizable
    {
        get { return (bool)GetValue(IsResizableProperty); }
        set { SetValue(IsResizableProperty, value); }
    }

    public static readonly DependencyProperty IsResizableProperty =
        DependencyProperty.Register(nameof(IsResizable), typeof(bool), typeof(WindowedContentDialog), new PropertyMetadata(false));

    public bool HasTitleBar
    {
        get => (bool)GetValue(HasTitleBarProperty);
        set => SetValue(HasTitleBarProperty, value);
    }

    public static readonly DependencyProperty HasTitleBarProperty = DependencyProperty.Register(
        nameof(HasTitleBar),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(true)
    );

    public bool CenterInParent
    {
        get => (bool)GetValue(CenterInParentProperty);
        set => SetValue(CenterInParentProperty, value);
    }

    public static readonly DependencyProperty CenterInParentProperty = DependencyProperty.Register(
        nameof(CenterInParent),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(true)
    );

    public bool IsModal { get; set; }

    public Window? OwnerWindow { get; set; }

    #endregion

    public ContentDialogResult Result { get; private set; }

    public async Task<ContentDialogResult> ShowAsync(bool isModal)
    {
        IsModal = isModal;
        return await ShowAsync();
    }

    public async Task<ContentDialogResult> ShowAsync()
    {
        InitializeContentDialogWindow();
        ContentDialogWindow.SetParent(OwnerWindow, IsModal, CenterInParent);
        ContentDialogWindow.HasTitleBar = HasTitleBar;
        ContentDialogWindow.IsResizable = IsResizable;

        SetUnderlay();        

        TaskCompletionSource<ContentDialogResult> resultCompletionSource = new();
        ContentDialogWindow.Loaded += (window, e) => window.Open();
        ContentDialogWindow.Closed += (o, e) => resultCompletionSource.SetResult(ContentDialogWindow.Result);
        Result = await resultCompletionSource.Task;
        return Result;
    }

    private void SetUnderlay()
    {
        switch (Underlay)
        {
            case UnderlayMode.SmokeLayer:
                HandleSmokeLayer();
                break;
            case UnderlayMode.SystemBackdrop:
                HandleSystemBackdrop();
                break;
        }
    }

    private void HandleSmokeLayer()
    {
        if (OwnerWindow?.Content?.XamlRoot == null)
            return;

        var popup = new Popup
        {
            XamlRoot = OwnerWindow.Content.XamlRoot,
            RequestedTheme = RequestedTheme
        };

        Rectangle darkLayer = new()
        {
            Opacity = 0.0,
            OpacityTransition = new ScalarTransition { Duration = TimeSpan.FromSeconds(0.25) },
            Fill = new SolidColorBrush(SmokeFillColor),
        };

        SizeToXamlRoot(darkLayer, OwnerWindow);
        popup.Child = darkLayer;
        smokeLayerCache = darkLayer;

        ContentDialogWindow.Opened -= DialogOpened;
        ContentDialogWindow.Opened += DialogOpened;

        ContentDialogWindow.Closed -= DialogClosed;
        ContentDialogWindow.Closed += DialogClosed;

        void DialogOpened(ContentDialogWindow o, EventArgs e)
        {
            popup.IsOpen = true;
            popup.Child.Opacity = 1.0;
            OwnerWindow.SizeChanged -= OnOwnerWindowSizeChanged;
            OwnerWindow.SizeChanged += OnOwnerWindowSizeChanged;
        }
        async void DialogClosed(object o, WindowEventArgs e)
        {
            popup.Child.Opacity = 0.0;
            await Task.Delay(popup.Child.OpacityTransition.Duration);
            popup.IsOpen = false;
            OwnerWindow.SizeChanged -= OnOwnerWindowSizeChanged;
        }
    }

    private void HandleSystemBackdrop()
    {
        if (OwnerWindow?.Content?.XamlRoot == null || UnderlaySystemBackdrop == null || UnderlaySystemBackdrop.Backdrop == BackdropType.None)
            return;

        var popup = PopupHelper.CreatePopup(
            OwnerWindow.Content.XamlRoot,
            UnderlaySystemBackdrop.Backdrop,
            UnderlaySystemBackdrop.CoverMode == UnderlayCoverMode.Full,
            GetTitleBarOffset(OwnerWindow));

        backdropLayerCache = popup.Child as Border;
        backdropLayerCache.OpacityTransition = UnderlaySystemBackdrop.OpacityTransition;

        SizeToXamlRoot(backdropLayerCache, OwnerWindow);

        ContentDialogWindow.Loaded -= DialogLoaded;
        ContentDialogWindow.Loaded += DialogLoaded;

        ContentDialogWindow.Closed -= DialogClosed;
        ContentDialogWindow.Closed += DialogClosed;

        void DialogLoaded(ContentDialogWindow o, EventArgs e)
        {
            popup.IsOpen = true;
            popup.Child.Opacity = 1.0;
            OwnerWindow.SizeChanged -= OnOwnerWindowSizeChanged;
            OwnerWindow.SizeChanged += OnOwnerWindowSizeChanged;
        }
        async void DialogClosed(object o, WindowEventArgs e)
        {
            popup.Child.Opacity = 0.0;
            await Task.Delay(popup.Child.OpacityTransition.Duration);
            popup.IsOpen = false;
            OwnerWindow.SizeChanged -= OnOwnerWindowSizeChanged;
        }
    }
    private void OnOwnerWindowSizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        switch (Underlay)
        {
            case UnderlayMode.SmokeLayer:
                SizeToXamlRoot(smokeLayerCache, OwnerWindow);
                break;

            case UnderlayMode.SystemBackdrop:
                SizeToXamlRoot(backdropLayerCache, OwnerWindow);
                break;
        }
    }
    /// <summary>
    /// ElementTheme.Default is treated as following owner window
    /// </summary>
    public ElementTheme DetermineTheme()
    {
        if (RequestedTheme is not ElementTheme.Default)
            return RequestedTheme;

        if (OwnerWindow?.Content is FrameworkElement element)
            return element.ActualTheme;

        return RequestedTheme;
    }

    protected void SizeToXamlRoot(FrameworkElement element, Window window)
    {
        element.Width = window.Content.XamlRoot.Size.Width;

        switch (Underlay)
        {
            case UnderlayMode.SmokeLayer:
                element.Height = window.Content.XamlRoot.Size.Height;
                break;

            case UnderlayMode.SystemBackdrop:
                element.Height = UnderlaySystemBackdrop.CoverMode == UnderlayCoverMode.Full ? window.Content.XamlRoot.Size.Height : window.Content.XamlRoot.Size.Height - GetTitleBarOffset(window);
                break;
        }
    }

    public int GetTitleBarOffset(Window window)
    {
        return window.AppWindow.TitleBar.PreferredHeightOption switch
        {
            TitleBarHeightOption.Standard => 32,
            TitleBarHeightOption.Tall => 48,
            _ => 0
        };
    }

    [DisallowNull]
    private ContentDialogContent ContentDialogContent { get; init; }
    private ContentDialogWindow ContentDialogWindow { get; set; }

    protected static Style DefaultButtonStyle => (Style)Application.Current.Resources["DefaultButtonStyle"];
    protected static Color SmokeFillColor => (Color)Application.Current.Resources["SmokeFillColorDefault"];

    public void Close()
    {
        ContentDialogWindow?.Close();
    }
}
