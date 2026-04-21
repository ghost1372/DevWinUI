// https://github.com/SuGar0218/SuGarToolkit.WinUI3

using Microsoft.UI.Xaml.Markup;
using System.Windows.Input;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class WindowedContentDialog : DependencyObject, IContentDialog, IContentDialogInteraction
{
    public WindowedContentDialog()
    {
        _view.PrimaryButtonClick += OnPrimaryButtonClick;
        _view.SecondaryButtonClick += OnSecondaryButtonClick;
        _view.CloseButtonClick += OnCloseButtonClick;
    }

    #region DependencyProperty

    public object? Header
    {
        get => (object?) GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(object), OnHeaderChanged)
    );

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.Header = e.NewValue;
    }

    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?) GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
        nameof(HeaderTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate), OnHeaderTemplateChanged)
    );

    private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.HeaderTemplate = (DataTemplate?) e.NewValue;
    }

    public object? Content
    {
        get => (object?) GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
        nameof(Content),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(object), OnContentChanged)
    );

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.Content = e.NewValue;
    }

    public DataTemplate? ContentTemplate
    {
        get => (DataTemplate?) GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(
        nameof(ContentTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate), OnContentTemplateChanged)
    );

    private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.ContentTemplate = (DataTemplate?) e.NewValue;
    }

    public object? PrimaryButtonContent
    {
        get => (object?) GetValue(PrimaryButtonContentProperty);
        set => SetValue(PrimaryButtonContentProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonContentProperty = DependencyProperty.Register(
        nameof(PrimaryButtonContent),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(object), OnPrimaryButtonContentChanged)
    );

    private static void OnPrimaryButtonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.PrimaryButtonContent = e.NewValue;
    }

    public DataTemplate? PrimaryButtonTemplate
    {
        get => (DataTemplate?) GetValue(PrimaryButtonTemplateProperty);
        set => SetValue(PrimaryButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonTemplateProperty = DependencyProperty.Register(
        nameof(PrimaryButtonTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate))
    );

    public object? SecondaryButtonContent
    {
        get => (object?) GetValue(SecondaryButtonContentProperty);
        set => SetValue(SecondaryButtonContentProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonContentProperty = DependencyProperty.Register(
        nameof(SecondaryButtonContent),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(object), OnSecondaryButtonContentChanged)
    );

    private static void OnSecondaryButtonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.SecondaryButtonContent = e.NewValue;
    }

    public DataTemplate? SecondaryButtonTemplate
    {
        get => (DataTemplate?) GetValue(SecondaryButtonTemplateProperty);
        set => SetValue(SecondaryButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonTemplateProperty = DependencyProperty.Register(
        nameof(SecondaryButtonTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate), OnSecondaryButtonTemplateChanged)
    );

    private static void OnSecondaryButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.SecondaryButtonTemplate = (DataTemplate?) e.NewValue;
    }

    public object? CloseButtonContent
    {
        get => (object?) GetValue(CloseButtonContentProperty);
        set => SetValue(CloseButtonContentProperty, value);
    }

    public static readonly DependencyProperty CloseButtonContentProperty = DependencyProperty.Register(
        nameof(CloseButtonContent),
        typeof(object),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(object), OnCloseButtonContentChanged)
    );

    private static void OnCloseButtonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.CloseButtonContent = e.NewValue;
    }

    public DataTemplate? CloseButtonTemplate
    {
        get => (DataTemplate?) GetValue(CloseButtonTemplateProperty);
        set => SetValue(CloseButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty CloseButtonTemplateProperty = DependencyProperty.Register(
        nameof(CloseButtonTemplate),
        typeof(DataTemplate),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(DataTemplate), OnCloseButtonTemplateChanged)
    );

    private static void OnCloseButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.CloseButtonTemplate = (DataTemplate?) e.NewValue;
    }

    public ICommand? PrimaryButtonCommand
    {
        get => (ICommand?) GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonCommandProperty = DependencyProperty.Register(
        nameof(PrimaryButtonCommand),
        typeof(ICommand),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(ICommand), OnPrimaryButtonCommandChanged)
    );

    private static void OnPrimaryButtonCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.PrimaryButtonCommand = (ICommand?) e.NewValue;
    }

    public ICommand? SecondaryButtonCommand
    {
        get => (ICommand?) GetValue(SecondaryButtonCommandProperty);
        set => SetValue(SecondaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonCommandProperty = DependencyProperty.Register(
        nameof(SecondaryButtonCommand),
        typeof(ICommand),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(ICommand), OnSecondaryButtonCommandChanged)
    );

    private static void OnSecondaryButtonCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.SecondaryButtonCommand = (ICommand?) e.NewValue;
    }

    public ICommand? CloseButtonCommand
    {
        get => (ICommand?) GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    public static readonly DependencyProperty CloseButtonCommandProperty = DependencyProperty.Register(
        nameof(CloseButtonCommand),
        typeof(ICommand),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(ICommand), OnCloseButtonCommandChanged)
    );

    private static void OnCloseButtonCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.CloseButtonCommand = (ICommand?) e.NewValue;
    }

    public bool IsPrimaryButtonEnabled
    {
        get => (bool) GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsPrimaryButtonEnabled),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(bool), OnIsPrimaryButtonEnabledChanged)
    );

    private static void OnIsPrimaryButtonEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.IsPrimaryButtonEnabled = (bool) e.NewValue;
    }

    public bool IsSecondaryButtonEnabled
    {
        get => (bool) GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsSecondaryButtonEnabled),
        typeof(bool),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(bool), OnIsSecondaryButtonEnabledChanged)
    );

    private static void OnIsSecondaryButtonEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.IsSecondaryButtonEnabled = (bool) e.NewValue;
    }

    public Orientation ButtonOrientation
    {
        get => (Orientation) GetValue(ButtonOrientationProperty);
        set => SetValue(ButtonOrientationProperty, value);
    }

    public static readonly DependencyProperty ButtonOrientationProperty = DependencyProperty.Register(
        nameof(ButtonOrientation),
        typeof(Orientation),
        typeof(WindowedContentDialog),
        new PropertyMetadata(Orientation.Horizontal, OnButtonOrientationChanged)
    );

    private static void OnButtonOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.ButtonOrientation = (Orientation) e.NewValue;
    }

    public Style? PrimaryButtonStyle
    {
        get => (Style?) GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register(
        nameof(PrimaryButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style), OnPrimaryButtonStyleChanged)
    );

    private static void OnPrimaryButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.PrimaryButtonStyle = (Style?) e.NewValue;
    }

    public Style? SecondaryButtonStyle
    {
        get => (Style?) GetValue(SecondaryButtonStyleProperty);
        set => SetValue(SecondaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonStyleProperty = DependencyProperty.Register(
        nameof(SecondaryButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style), OnSecondaryButtonStyleChanged)
    );

    private static void OnSecondaryButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.SecondaryButtonStyle = (Style?) e.NewValue;
    }

    public Style? CloseButtonStyle
    {
        get => (Style?) GetValue(CloseButtonStyleProperty);
        set => SetValue(CloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(
        nameof(CloseButtonStyle),
        typeof(Style),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(Style), OnCloseButtonStyleChanged)
    );

    private static void OnCloseButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.CloseButtonStyle = (Style?) e.NewValue;
    }

    public string? PrimaryButtonAccessKey
    {
        get => (string?) GetValue(PrimaryButtonAccessKeyProperty);
        set => SetValue(PrimaryButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(PrimaryButtonAccessKey),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(string), OnPrimaryButtonAccessKeyChanged)
    );

    private static void OnPrimaryButtonAccessKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.PrimaryButtonAccessKey = (string?) e.NewValue;
    }

    public string? SecondaryButtonAccessKey
    {
        get => (string?) GetValue(SecondaryButtonAccessKeyProperty);
        set => SetValue(SecondaryButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(SecondaryButtonAccessKey),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(string), OnSecondaryButtonAccessKeyChanged)
    );

    private static void OnSecondaryButtonAccessKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.SecondaryButtonAccessKey = (string?) e.NewValue;
    }

    public string? CloseButtonAccessKey
    {
        get => (string?) GetValue(CloseButtonAccessKeyProperty);
        set => SetValue(CloseButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty CloseButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(CloseButtonAccessKey),
        typeof(string),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(string), OnCloseButtonAccessKeyChanged)
    );

    private static void OnCloseButtonAccessKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.CloseButtonAccessKey = (string?) e.NewValue;
    }

    public ContentDialogButton DefaultButton
    {
        get => (ContentDialogButton) GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(
        nameof(DefaultButton),
        typeof(ContentDialogButton),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(ContentDialogButton), OnDefaultButtonChanged)
    );

    private static void OnDefaultButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WindowedContentDialog) d)._view.DefaultButton = (ContentDialogButton) e.NewValue;
    }

    public SystemBackdrop? SystemBackdrop
    {
        get => (SystemBackdrop?) GetValue(SystemBackdropProperty);
        set => SetValue(SystemBackdropProperty, value);
    }

    public static readonly DependencyProperty SystemBackdropProperty = DependencyProperty.Register(
        nameof(SystemBackdrop),
        typeof(SystemBackdrop),
        typeof(WindowedContentDialog),
        new PropertyMetadata(default(SystemBackdrop))
    );

    #endregion

    public Window? Owner { get; set; }

    public ContentDialogResult Result => _view.Result;

    private readonly ContentDialogView _view = new();
    private DialogWindowBase? _window;
    private TaskCompletionSource<ContentDialogResult>? _taskCompletionSource;

    public event EventHandler? PrimaryButtonClick;
    public event EventHandler? SecondaryButtonClick;
    public event EventHandler? CloseButtonClick;

    public Task<ContentDialogResult> ShowAsync()
    {
        _taskCompletionSource = new TaskCompletionSource<ContentDialogResult>();
        _window = new DialogWindowBase
        {
            Content = _view,
            SystemBackdrop = SystemBackdrop,
            Owner = Owner,
            CanResize = CanResize,
            FlowDirection = FlowDirection,
            HasTitleBar = HasTitleBar,
            MinHeight = MinHeight,
            MinWidth = MinWidth,
        };

        if (CanDragMoveWindow)
        {
            DragMoveAndResizeHelper.SetDragMove(_window.Window, _view);
        }
        _window.Closed += OnWindowClosed;
        _window.ShowDialog();
        return _taskCompletionSource.Task;
    }

    private void OnPrimaryButtonClick(object? sender, EventArgs e)
    {
        _window!.TryClose();
        PrimaryButtonClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnSecondaryButtonClick(object? sender, EventArgs e)
    {
        _window!.TryClose();
        SecondaryButtonClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnCloseButtonClick(object? sender, EventArgs e)
    {
        _window!.TryClose();
        CloseButtonClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        _taskCompletionSource?.SetResult(Result);
    }
}
