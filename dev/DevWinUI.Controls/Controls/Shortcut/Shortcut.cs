using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_OpenDialog), Type = typeof(Button))]
public partial class Shortcut : BaseShortcut
{
    private const string PART_OpenDialog = "PART_OpenDialog";
    private Button openDialog;

    private ShortcutPreview shortcut;
    private ContentDialog contentDialog;
    private bool canCloseDialog = false;
    private List<object> defaultKeys;
    private readonly HashSet<VirtualKey> _pressedKeys = new();
    private readonly List<VirtualKey> _keyOrder = new();
    private bool _windowsKeyPressed = false;

    public event EventHandler<ContentDialogButtonClickEventArgs> CloseButtonClick;
    public event EventHandler<ContentDialogButtonClickEventArgs> PrimaryButtonClick;
    public event EventHandler<ContentDialogButtonClickEventArgs> SecondaryButtonClick;
    public event EventHandler<ContentDialogClosingEventArgs> ClosingContentDialog;

    public IconElement Icon
    {
        get { return (IconElement)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(Shortcut), new PropertyMetadata(null));
    
    public new bool IsEnabled
    {
        get { return (bool)GetValue(IsEnabledProperty); }
        set { SetValue(IsEnabledProperty, value); }
    }

    public static new readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(Shortcut), new PropertyMetadata(true, OnIsEnabledChanged));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Shortcut)d;
        if (ctl != null && ctl.openDialog != null)
        {
            ctl.openDialog.IsEnabled = (bool)e.NewValue;
        }
    }

    public new List<object> Keys
    {
        get { return (List<object>)GetValue(KeysProperty); }
        set { SetValue(KeysProperty, value); }
    }

    public static new readonly DependencyProperty KeysProperty =
        DependencyProperty.Register(nameof(Keys), typeof(List<object>), typeof(Shortcut), new PropertyMetadata(null, OnKeysChanged));

    private static void OnKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Shortcut)d;
        if (ctl != null)
        {
            ctl.UpdateItemsSource((List<object>)e.NewValue);
        }
    }

    private void UpdateItemsSource(List<object> keys)
    {
        if (shortcut != null)
        {
            shortcut.Keys = keys;
        }
    }

    public string ContentDialogTitle
    {
        get { return (string)GetValue(ContentDialogTitleProperty); }
        set { SetValue(ContentDialogTitleProperty, value); }
    }

    public static readonly DependencyProperty ContentDialogTitleProperty =
        DependencyProperty.Register(nameof(ContentDialogTitle), typeof(string), typeof(Shortcut), new PropertyMetadata("Activation shortcut"));

    public string PrimaryButtonText
    {
        get { return (string)GetValue(PrimaryButtonTextProperty); }
        set { SetValue(PrimaryButtonTextProperty, value); }
    }

    public static readonly DependencyProperty PrimaryButtonTextProperty =
        DependencyProperty.Register(nameof(PrimaryButtonText), typeof(string), typeof(Shortcut), new PropertyMetadata("Save"));

    public string SecondaryButtonText
    {
        get { return (string)GetValue(SecondaryButtonTextProperty); }
        set { SetValue(SecondaryButtonTextProperty, value); }
    }

    public static readonly DependencyProperty SecondaryButtonTextProperty =
        DependencyProperty.Register(nameof(SecondaryButtonText), typeof(string), typeof(Shortcut), new PropertyMetadata("Reset"));

    public string CloseButtonText
    {
        get { return (string)GetValue(CloseButtonTextProperty); }
        set { SetValue(CloseButtonTextProperty, value); }
    }

    public static readonly DependencyProperty CloseButtonTextProperty =
        DependencyProperty.Register(nameof(CloseButtonText), typeof(string), typeof(Shortcut), new PropertyMetadata("Cancel"));

    public Shortcut()
    {
        DefaultStyleKey = typeof(Shortcut);

        if (Icon == null)
        {
            Icon = CreateDefaultIcon();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        openDialog = GetTemplateChild(PART_OpenDialog) as Button;

        if (openDialog != null)
        {
            openDialog.Click -= OpenDialog_Click;
            openDialog.Click += OpenDialog_Click;
        }
    }

    private async void OpenDialog_Click(object sender, RoutedEventArgs e)
    {
        shortcut = new ShortcutPreview();
        shortcut.Loaded -= OnShortcutDialogLoaded;
        shortcut.Loaded += OnShortcutDialogLoaded;
        shortcut.KeyDown -= OnKeyDown;
        shortcut.KeyDown += OnKeyDown;
        shortcut.KeyUp -= OnKeyUp;
        shortcut.KeyUp += OnKeyUp;
        shortcut.LostFocus -= OnLostFocus;
        shortcut.LostFocus += OnLostFocus;

        shortcut.Keys = null;
        shortcut.Keys = Keys;

        shortcut.InfoTitle = InfoTitle;
        shortcut.InfoToolTip = InfoToolTip;
        shortcut.WarningTitle = WarningTitle;
        shortcut.WarningToolTip = WarningToolTip;
        shortcut.ErrorTitle = ErrorTitle;
        shortcut.ErrorToolTip = ErrorToolTip;
        shortcut.Title = Title;
        contentDialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = ContentDialogTitle,
            Content = shortcut,
            PrimaryButtonText = PrimaryButtonText,
            SecondaryButtonText = SecondaryButtonText,
            CloseButtonText = CloseButtonText,
            DefaultButton = ContentDialogButton.Primary,
            RequestedTheme = this.ActualTheme
        };
        contentDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;

        contentDialog.Closing -= OnClosingContentDialog;
        contentDialog.Closing += OnClosingContentDialog;
        contentDialog.PrimaryButtonClick -= OnSaveContentDialog;
        contentDialog.PrimaryButtonClick += OnSaveContentDialog;
        contentDialog.SecondaryButtonClick -= OnResetContentDialog;
        contentDialog.SecondaryButtonClick += OnResetContentDialog;
        contentDialog.CloseButtonClick -= OnCancelContentDialog;
        contentDialog.CloseButtonClick += OnCancelContentDialog;
        contentDialog.SizeChanged -= OnContentDialogSizeChanged;
        contentDialog.SizeChanged += OnContentDialogSizeChanged;
        await contentDialog.ShowAsyncQueue();
    }

    private void OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (_windowsKeyPressed)
        {
            _pressedKeys?.Clear();
            _keyOrder?.Clear();
            UpdatePreviewKeys();
            _windowsKeyPressed = false;
        }
    }

    private void OnContentDialogSizeChanged(object sender, SizeChangedEventArgs e)
    {
        shortcut?.SetMinWidth(e.NewSize.Width);
    }

    private IconElement CreateDefaultIcon()
    {
        return new FontIcon
        {
            Glyph = GeneralHelper.GetGlyph("E70F"),
            FontSize = 16,
            FontFamily = Application.Current.Resources["SymbolThemeFontFamily"] as FontFamily
        };
    }

    private void OnShortcutDialogLoaded(object sender, RoutedEventArgs e)
    {
        defaultKeys = Keys;
        shortcut.Unloaded -= OnShortcutDialogUnloaded;
        shortcut.Unloaded += OnShortcutDialogUnloaded;
    }
    private void OnShortcutDialogUnloaded(object sender, RoutedEventArgs e)
    {
        if (shortcut != null)
        {
            shortcut.KeyDown -= OnKeyDown;
            shortcut.KeyUp -= OnKeyUp;
        }
    }

    private void OnCancelContentDialog(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        canCloseDialog = true;
        CloseButtonClick?.Invoke(this, args);
    }

    private void OnClosingContentDialog(ContentDialog sender, ContentDialogClosingEventArgs args)
    {
        contentDialog?.SizeChanged -= OnContentDialogSizeChanged;
        args.Cancel = !canCloseDialog;
        canCloseDialog = false;
        ClosingContentDialog?.Invoke(this, args);
    }
    private void OnSaveContentDialog(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        PrimaryButtonClick?.Invoke(this, args);
    }
    private void OnResetContentDialog(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (shortcut != null)
        {
            shortcut.Keys = defaultKeys;
        }

        SecondaryButtonClick?.Invoke(this, args);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var key = e.Key;

        if (e.Key == VirtualKey.LeftWindows || e.Key == VirtualKey.RightWindows)
            _windowsKeyPressed = true;

        if (_pressedKeys.Contains(key))
            return;

        if (IsModifierKey(key))
        {
            _pressedKeys.Add(key);
            _keyOrder.Add(key);
        }
        else
        {
            // Remove existing non-modifier keys
            var nonModifiers = _pressedKeys.Where(k => !IsModifierKey(k)).ToList();
            foreach (var k in nonModifiers)
            {
                _pressedKeys.Remove(k);
                _keyOrder.Remove(k);
            }

            _pressedKeys.Add(key);
            _keyOrder.Add(key);
        }

        var keyNames = _keyOrder.Where(_pressedKeys.Contains).Select(key=> new KeyVisualInfo { Key = key, KeyName = GetKeyName(key) }).ToList();
        shortcut.Keys = keyNames.Cast<object>().ToList();

        int modifierCount = _pressedKeys.Count(IsModifierKey);
        int nonModifierCount = _pressedKeys.Count(k => !IsModifierKey(k));

        if (modifierCount >= 1 && modifierCount <= 4 && nonModifierCount == 1)
        {
            shortcut.IsError = false;
            shortcut.IsWarning = false;
            shortcut.IsInfo = true;
            contentDialog.IsPrimaryButtonEnabled = true;
        }
        else if (modifierCount == 0 && nonModifierCount == 1)
        {
            shortcut.IsInfo = false;
            shortcut.IsError = false;
            shortcut.IsWarning = true;
            contentDialog.IsPrimaryButtonEnabled = true;
        }
        else
        {
            shortcut.IsError = true;
            shortcut.IsInfo = false;
            shortcut.IsWarning = false;
            contentDialog.IsPrimaryButtonEnabled = false;
        }

        e.Handled = true;
    }

    public void UpdatePreviewKeys()
    {
        Keys = shortcut?.Keys;
    }
    public void CanCloseContentDialog(bool canClose)
    {
        canCloseDialog = canClose;
    }
    public void CloseContentDialog()
    {
        canCloseDialog = true;
        contentDialog?.Hide();
    }
    private void OnKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.LeftWindows || e.Key == VirtualKey.RightWindows)
            _windowsKeyPressed = false;

        _pressedKeys.Remove(e.Key);
        _keyOrder.Remove(e.Key); // also remove from ordered list
        e.Handled = true;
    }

    private bool IsModifierKey(VirtualKey key)
    {
        return key is VirtualKey.Control
            or VirtualKey.Shift
            or VirtualKey.Menu
            or VirtualKey.LeftWindows
            or VirtualKey.RightWindows;
    }

    private string GetKeyName(VirtualKey key)
    {
        return key switch
        {
            VirtualKey.Control => "Ctrl",
            VirtualKey.LeftControl => "Left Ctrl",
            VirtualKey.RightControl => "Right Ctrl",
            VirtualKey.Shift => "Shift",
            VirtualKey.LeftShift => "Left Shift",
            VirtualKey.RightShift => "Right Shift",
            VirtualKey.Menu => "Alt",
            VirtualKey.LeftMenu => "Left Alt",
            VirtualKey.RightMenu => "Right Alt",
            VirtualKey.LeftWindows or VirtualKey.RightWindows => "Win",
            VirtualKey.CapitalLock => "CapsLock",
            VirtualKey.NumberKeyLock => "NumLock",
            VirtualKey.Decimal => ".",
            VirtualKey.Scroll => "ScrollLock",
            VirtualKey.NumberPad0 => "Num0",
            VirtualKey.NumberPad1 => "Num1",
            VirtualKey.NumberPad2 => "Num2",
            VirtualKey.NumberPad3 => "Num3",
            VirtualKey.NumberPad4 => "Num4",
            VirtualKey.NumberPad5 => "Num5",
            VirtualKey.NumberPad6 => "Num6",
            VirtualKey.NumberPad7 => "Num7",
            VirtualKey.NumberPad8 => "Num8",
            VirtualKey.NumberPad9 => "Num9",
            VirtualKey.Number0 => "0",
            VirtualKey.Number1 => "1",
            VirtualKey.Number2 => "2",
            VirtualKey.Number3 => "3",
            VirtualKey.Number4 => "4",
            VirtualKey.Number5 => "5",
            VirtualKey.Number6 => "6",
            VirtualKey.Number7 => "7",
            VirtualKey.Number8 => "8",
            VirtualKey.Number9 => "9",
            (VirtualKey)191 => "/",
            (VirtualKey)192 => "`",
            (VirtualKey)189 => "-",
            (VirtualKey)187 => "=",
            (VirtualKey)219 => "[",
            (VirtualKey)221 => "]",
            (VirtualKey)220 => "\\",
            (VirtualKey)222 => "'",
            (VirtualKey)186 => ";",
            (VirtualKey)188 => ",",
            (VirtualKey)190 => ".",
            (VirtualKey)226 => "\\",
            (VirtualKey)173 => "Volume Mute",
            (VirtualKey)174 => "Volume Down",
            (VirtualKey)175 => "Volume Up",
            _ => key.ToString()
        };
    }
}

