using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;
public partial class Shortcut : BaseShortcut
{
    private Shortcut shortcut;
    private ContentDialog contentDialog;
    private bool canCloseDialog = false;
    private List<object> defaultKeys;
    private readonly HashSet<VirtualKey> _pressedKeys = new();
    private readonly List<VirtualKey> _keyOrder = new();

    public event EventHandler<ContentDialogButtonClickEventArgs> CloseButtonClick;
    public event EventHandler<ContentDialogButtonClickEventArgs> PrimaryButtonClick;
    public event EventHandler<ContentDialogButtonClickEventArgs> SecondaryButtonClick;
    public event EventHandler<ContentDialogClosingEventArgs> ClosingContentDialog;
    public IDelegateCommand OpenDialogCommand { get; }

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
        if (ctl != null)
        {
            ctl.OpenDialogCommand?.RaiseCanExecuteChanged();
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
        OpenDialogCommand = DelegateCommand.Create(OnOpenDialogCommand, CanExecuteOpenDialog);
        if (Icon == null)
        {
            Icon = CreateDefaultIcon();
        }
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
    private bool CanExecuteOpenDialog()
    {
        return IsEnabled;
    }

    private async void OnOpenDialogCommand()
    {
        shortcut = new Shortcut();
        shortcut.Loaded -= OnShortcutDialogLoaded;
        shortcut.Loaded += OnShortcutDialogLoaded;
        shortcut.KeyDown -= OnKeyDown;
        shortcut.KeyDown += OnKeyDown;
        shortcut.KeyUp -= OnKeyUp;
        shortcut.KeyUp += OnKeyUp;

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

        contentDialog.Closing += OnClosingContentDialog;
        contentDialog.PrimaryButtonClick += OnSaveContentDialog;
        contentDialog.SecondaryButtonClick += OnResetContentDialog;
        contentDialog.CloseButtonClick += OnCancelContentDialog;

        await contentDialog.ShowAsyncQueue();
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

        // Now use _keyOrder for display — but only include keys still in _pressedKeys
        var keyNames = _keyOrder.Where(_pressedKeys.Contains).Select(GetKeyName).ToList();
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
            VirtualKey.Shift => "Shift",
            VirtualKey.Menu => "Alt",
            VirtualKey.LeftWindows or VirtualKey.RightWindows => "Win",
            _ => key.ToString()
        };
    }
}

