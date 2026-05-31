// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

public partial class MessageBox : DependencyObject
{
    public MessageBox()
    {
        _window = new DialogWindowBase();
        _view = new MessageBoxView();
        _view.Loaded += OnViewLoaded;
        _window.SystemBackdrop = SystemBackdrop;
        _view.FlowDirection = FlowDirection;
        _window.Content = _view;
        _window.StartupLocation = StartupLocation;
        _window.CanResize = CanResize;
        _window.HasTitleBar = HasTitleBar;
        if (CanDragMoveWindow)
        {
            new DragMoveHelper(_window.Window).SetDragMove(_view);
        }
        _view.ResultChanged += OnResultChanged;
    }

    #region DependencyProperty

    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(default(string), OnTitleChanged)
    );

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MessageBox)d)._view.Title = (string?)e.NewValue;
    }

    public string? Message
    {
        get => (string?)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        nameof(Message),
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(default(string), OnMessageChanged)
    );

    private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MessageBox)d)._view.Message = (string?)e.NewValue;
    }

    public MessageBoxIcon Icon
    {
        get => (MessageBoxIcon)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(MessageBoxIcon),
        typeof(MessageBox),
        new PropertyMetadata(default(MessageBoxIcon), OnIconChanged)
    );

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MessageBox)d)._view.Icon = (MessageBoxIcon)e.NewValue;
    }

    public MessageBoxButtons Buttons
    {
        get => (MessageBoxButtons)GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(
        nameof(Buttons),
        typeof(MessageBoxButtons),
        typeof(MessageBox),
        new PropertyMetadata(default(MessageBoxButtons), OnButtonsChanged)
    );

    private static void OnButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MessageBox)d)._view.Buttons = (MessageBoxButtons)e.NewValue;
    }

    public MessageBoxDefaultButton DefaultButton
    {
        get => (MessageBoxDefaultButton)GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(
        nameof(DefaultButton),
        typeof(MessageBoxDefaultButton),
        typeof(MessageBox),
        new PropertyMetadata(default(MessageBoxDefaultButton), OnDefaultButtonChanged)
    );

    private static void OnDefaultButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MessageBox)d)._view.DefaultButton = (MessageBoxDefaultButton)e.NewValue;
    }

    #endregion

    public Window? Owner
    {
        get => _window.Owner;
        set => _window.Owner = value;
    }

    private readonly DialogWindowBase _window;
    private readonly MessageBoxView _view;

    public async Task<MessageBoxResult> ShowAsync()
    {
        await _window.ShowDialogAsync();
        return _view.Result;
    }

    private void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        _view.MaxWidth = double.PositiveInfinity;
    }

    private void OnResultChanged(object? sender, EventArgs e)
    {
        _window.TryClose();
    }
}
