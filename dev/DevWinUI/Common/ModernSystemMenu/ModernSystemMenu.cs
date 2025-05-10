using Microsoft.UI.Content;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;
public partial class ModernSystemMenu : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private const int SC_MOUSEMENU = 0xF090;
    private const int SC_KEYMENU = 0xF100;

    private Microsoft.UI.Xaml.Window window;
    private MenuFlyout titleBarMenuFlyout;
    private readonly ContentCoordinateConverter contentCoordinateConverter;
    private readonly OverlappedPresenter overlappedPresenter;
    public bool IsModernSystemMenuEnabled = true;

    private bool _isWindowMaximized;
    public bool IsWindowMaximized
    {
        get { return _isWindowMaximized; }

        set
        {
            if (!Equals(_isWindowMaximized, value))
            {
                _isWindowMaximized = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                CanExecuteRestore();
                CanExecuteMove(null);
                CanExecuteSize(null);
                CanExecuteMaximize();
                CanExecuteMinimize();
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the ModernSystemMenu with a specified MenuFlyout.
    /// </summary>
    /// <param name="window">The window where the modern system menu will be displayed.</param>
    /// <param name="menuFlyout">The menu that will be shown as a flyout in the title bar.</param>
    public ModernSystemMenu(Microsoft.UI.Xaml.Window window, MenuFlyout menuFlyout) : this(window)
    {
        titleBarMenuFlyout = menuFlyout;
    }

    /// <summary>
    /// Initializes a new instance of the ModernSystemMenu.
    /// </summary>
    /// <param name="window">The window instance for which the modern system menu is being created.</param>
    public ModernSystemMenu(Microsoft.UI.Xaml.Window window)
    {
        this.window = window;
        IsModernSystemMenuEnabled = true;

        RestoreCommand = DelegateCommand.Create(Restore, CanExecuteRestore);
        MoveCommand = DelegateCommand.Create(Move, CanExecuteMove);
        SizeCommand = DelegateCommand.Create(Size, CanExecuteSize);
        MinimizeCommand = DelegateCommand.Create(Minimize, CanExecuteMinimize);
        MaximizeCommand = DelegateCommand.Create(Maximize, CanExecuteMaximize);
        CloseCommand = DelegateCommand.Create(Close, CanExecuteClose);

        contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(this.window.AppWindow.Id);
        overlappedPresenter = this.window.AppWindow.Presenter as OverlappedPresenter;

        if (titleBarMenuFlyout == null)
        {
            CreateMenuFlyout();
        }

        IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;

        this.window.SizeChanged -= OnSizeChanged;
        this.window.SizeChanged += OnSizeChanged;
        this.window.AppWindow.Changed -= OnAppWindowChanged;
        this.window.AppWindow.Changed += OnAppWindowChanged;

        RegisterWindowMonitor();
    }

    private void CreateMenuFlyout()
    {
        titleBarMenuFlyout = new MenuFlyout()
        {
            Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft
        };

        Style menuFlyoutItemStyle = null;
        if (Application.Current.Resources.TryGetValue("ModernSystemMenuFlyoutItemStyle", out var styleObj) && styleObj is Style style)
        {
            menuFlyoutItemStyle = style;
        }

        var restoreItem = new MenuFlyoutItem
        {
            Style = menuFlyoutItemStyle,
            Text = "Restore",
            Icon = new FontIcon { Glyph = GeneralHelper.GetGlyph("E923") },
            Command = RestoreCommand
        };

        var moveItem = new MenuFlyoutItem
        {
            Style = menuFlyoutItemStyle,
            Text = "Move",
            Command = MoveCommand
        };
        moveItem.CommandParameter = moveItem;

        var sizeItem = new MenuFlyoutItem
        {
            Style = menuFlyoutItemStyle,
            Text = "Size",
            Command = SizeCommand
        };
        sizeItem.CommandParameter = sizeItem;

        var minimizeItem = new MenuFlyoutItem
        {
            Style = menuFlyoutItemStyle,
            Text = "Minimize",
            Icon = new FontIcon { Glyph = GeneralHelper.GetGlyph("E921") },
            Command = MinimizeCommand
        };

        var maximizeItem = new MenuFlyoutItem
        {
            Style = menuFlyoutItemStyle,
            Text = "Maximize",
            Icon = new FontIcon { Glyph = GeneralHelper.GetGlyph("E922") },
            Command = MaximizeCommand
        };

        var closeItem = new MenuFlyoutItem
        {
            Style = menuFlyoutItemStyle,
            Text = "Close",
            Icon = new FontIcon { Glyph = GeneralHelper.GetGlyph("E8BB") },
            Command = CloseCommand
        };
        closeItem.KeyboardAccelerators.Add(new Microsoft.UI.Xaml.Input.KeyboardAccelerator
        {
            Key = Windows.System.VirtualKey.F4,
            Modifiers = Windows.System.VirtualKeyModifiers.Menu
        });

        titleBarMenuFlyout.Items.Add(restoreItem);
        titleBarMenuFlyout.Items.Add(moveItem);
        titleBarMenuFlyout.Items.Add(sizeItem);
        titleBarMenuFlyout.Items.Add(minimizeItem);
        titleBarMenuFlyout.Items.Add(maximizeItem);
        titleBarMenuFlyout.Items.Add(new MenuFlyoutSeparator() { Width = 200 });
        titleBarMenuFlyout.Items.Add(closeItem);
    }

    private void OnSizeClicked(MenuFlyoutItem menuItem)
    {
        HideMenuFlyout();
        PInvoke.SendMessage(new HWND((IntPtr)window.AppWindow.Id.Value), (uint)NativeValues.WindowMessage.WM_SYSCOMMAND, 0xF000, 0);
    }

    private void OnMoveClicked(MenuFlyoutItem menuItem)
    {
        HideMenuFlyout();
        PInvoke.SendMessage(new HWND((IntPtr)window.AppWindow.Id.Value), (uint)NativeValues.WindowMessage.WM_SYSCOMMAND, 0xF010, 0);
    }

    private void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        if (titleBarMenuFlyout.IsOpen)
        {
            HideMenuFlyout();
        }

        if (overlappedPresenter is not null)
        {
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
        }
    }

    private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidPositionChange)
        {
            if (titleBarMenuFlyout.IsOpen)
            {
                HideMenuFlyout();
            }

            if (overlappedPresenter is not null)
            {
                IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            }
        }
    }

    private void RegisterWindowMonitor()
    {
        var monitor = new WindowMessageMonitor(window);
        monitor.WindowMessageReceived += OnWindowMessageReceived;

        var inputNonClientPointerSourceHandle = PInvoke.FindWindowEx(new HWND((IntPtr)window.AppWindow.Id.Value), HWND.Null, "InputNonClientPointerSource", null);

        if (inputNonClientPointerSourceHandle != HWND.Null)
        {
            var monitorNonClient = new WindowMessageMonitor(inputNonClientPointerSourceHandle);
            monitorNonClient.WindowMessageReceived += OnWindowMessageReceivedNonClient;
        }
    }

    private void OnWindowMessageReceived(object sender, WindowMessageEventArgs e)
    {
        if (IsModernSystemMenuEnabled)
        {
            //Destroy Legacy SystemContextMenu by Clicking on App Icon
            if (e.MessageType == (uint)NativeValues.WindowMessage.WM_INITMENUPOPUP)
            {
                IntPtr hWndMenu = PInvoke.FindWindow("#32768", null);
                PInvoke.DestroyWindow(new HWND(hWndMenu));
            }

            if (e.MessageType == (uint)NativeValues.WindowMessage.WM_SYSCOMMAND)
            {
                var sysCommand = e.Message.WParam.ToUInt32() & 0xFFF0;

                if (sysCommand is SC_MOUSEMENU)
                {
                    FlyoutShowOptions options = new()
                    {
                        Position = new Point(0, 15),
                        ShowMode = FlyoutShowMode.Standard
                    };

                    ShowMenuFlyout(options);

                    e.Result = 0;
                    e.Handled = true;
                }
                else if (sysCommand is SC_KEYMENU)
                {
                    FlyoutShowOptions options = new()
                    {
                        Position = new Point(0, 45),
                        ShowMode = FlyoutShowMode.Standard
                    };

                    ShowMenuFlyout(options);

                    e.Result = 0;
                    e.Handled = true;
                }
            }
        }
    }

    private void OnWindowMessageReceivedNonClient(object sender, WindowMessageEventArgs e)
    {
        if (IsModernSystemMenuEnabled)
        {
            if (e.MessageType == (uint)NativeValues.WindowMessage.WM_NCLBUTTONDOWN)
            {
                if (titleBarMenuFlyout.IsOpen)
                {
                    HideMenuFlyout();
                }
            }
            else if (e.MessageType == (uint)NativeValues.WindowMessage.WM_NCRBUTTONUP)
            {
                if (e.Message.WParam.ToUInt32() is 2 && window.Content is not null && window.Content.XamlRoot is not null)
                {
                    PointInt32 screenPoint = new(e.Message.LParam.ToInt32() & 0xFFFF, e.Message.LParam.ToInt32() >> 16);
                    Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                    FlyoutShowOptions options = new()
                    {
                        ShowMode = FlyoutShowMode.Standard,
                        Position = OSVersionHelper.IsWindows11_22000_OrGreater ? new Point(localPoint.X / window.Content.XamlRoot.RasterizationScale, localPoint.Y / window.Content.XamlRoot.RasterizationScale) : new Point(localPoint.X, localPoint.Y)
                    };

                    ShowMenuFlyout(options);
                }
                e.Result = 0;
                e.Handled = true;
            }
        }
    }

    private void SetMenuFlyoutXamlRoot()
    {
        if (titleBarMenuFlyout.XamlRoot == null)
        {
            titleBarMenuFlyout.XamlRoot = window.Content.XamlRoot;
        }
    }

    private void ShowMenuFlyout(FlyoutShowOptions options)
    {
        if (titleBarMenuFlyout.Items.Count > 0)
        {
            SetMenuFlyoutXamlRoot();
            titleBarMenuFlyout.ShowAt(null, options);
        }
    }

    private void HideMenuFlyout()
    {
        SetMenuFlyoutXamlRoot();
        titleBarMenuFlyout.Hide();
    }
}
