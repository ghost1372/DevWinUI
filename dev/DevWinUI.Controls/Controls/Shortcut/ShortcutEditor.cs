using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace DevWinUI;
public partial class ShortcutEditor : Control
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
        DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(ShortcutEditor), new PropertyMetadata(null));

    public new bool IsEnabled
    {
        get { return (bool)GetValue(IsEnabledProperty); }
        set { SetValue(IsEnabledProperty, value); }
    }

    public static new readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(ShortcutEditor), new PropertyMetadata(true, OnIsEnabledChanged));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShortcutEditor)d;
        if (ctl != null)
        {
            ctl.OpenDialogCommand?.RaiseCanExecuteChanged();
        }
    }

    public List<object> Keys
    {
        get { return (List<object>)GetValue(KeysProperty); }
        set { SetValue(KeysProperty, value); }
    }

    public static readonly DependencyProperty KeysProperty =
        DependencyProperty.Register(nameof(Keys), typeof(List<object>), typeof(ShortcutEditor), new PropertyMetadata(null, OnKeysChanged));

    private static void OnKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShortcutEditor)d;
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

    public ShortcutEditor()
    {
        OpenDialogCommand = DelegateCommand.Create(OnOpenDialogCommand, CanExecuteOpenDialog);
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

        contentDialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Title = "Activation shortcut",
            Content = shortcut,
            PrimaryButtonText = "Save",
            SecondaryButtonText = "Reset",
            CloseButtonText = "Cancel",
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

            shortcut.InfoTitle = "Only shortcuts that start with Windows key, Ctrl, Alt or Shift are valid.";
            shortcut.IsInfo = true;
            contentDialog.IsPrimaryButtonEnabled = true;
        }
        else if (modifierCount == 0 && nonModifierCount == 1)
        {
            shortcut.IsInfo = false;
            shortcut.IsError = false;
            shortcut.WarningTitle = "Using a single key as a shortcut may interfere with regular typing or system behavior.";
            shortcut.WarningToolTip = "It's recommended to use a modifier key (like Ctrl, Alt, or Win) along with it.";
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

