namespace DevWinUI;
public static partial class NativeValues
{
    public delegate IntPtr WNDPROC(IntPtr hWnd, NativeValues.WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    public static partial class ExternDll
    {
        public const string
            User32 = "user32.dll",
            Gdi32 = "gdi32.dll",
            GdiPlus = "gdiplus.dll",
            Kernel32 = "kernel32.dll",
            Shell32 = "shell32.dll",
            MsImg = "msimg32.dll",
            NTdll = "ntdll.dll",
            DwmApi = "dwmapi.dll",
            UxTheme = "uxtheme.dll",
            ComCtl32 = "comctl32.dll";
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }
    internal struct POINT
    {
        /// <summary>
        ///  The x-coordinate of the point.
        /// </summary>
        public int x;

        /// <summary>
        /// The x-coordinate of the point.
        /// </summary>
        public int y;

        public static implicit operator System.Drawing.Point(POINT point)
        {
            return new(point.x, point.y);
        }

        public static implicit operator POINT(System.Drawing.Point point)
        {
            return new() { x = point.X, y = point.Y };
        }
    }

    [Flags]
    internal enum DWM_BLURBEHIND_Mask
    {
        Enable = 0x00000001,
        BlurRegion = 0x00000002,
        TransitionMaximized = 0x00000004,
    }

    [Flags]
    internal enum WindowStyle : uint
    {
        WS_SYSMENU = 0x80000,
        WS_BORDER = 0x00800000,
        WS_CAPTION = 0x00C00000,
        WS_THICKFRAME = 0x00040000,
        WS_EX_LAYERED = 0x00080000,
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_LAYOUTLTR = 0x00000000,
        WS_EX_LAYOUTRTL = 0x00400000
    }
    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }

    public enum PreferredAppMode
    {
        Default,
        AllowDark,
        ForceDark,
        ForceLight,
        Max
    };

    public enum WindowMessage : int
    {
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_SYSCOMMAND = 0x0112,
        WM_SYSMENU = 0x0313,
        WM_GETMINMAXINFO = 0x0024,
        WM_PAINT = 0x000F,
        WM_ERASEBKGND = 0x0014,
        WM_MOVE = 3,
        WM_CLOSE = 0x0010,
        WM_SETCURSOR = 0x20,
        WM_NCMOUSEMOVE = 0x00a0,
        WM_ACTIVATE = 0x0006,
        WM_ACTIVATEAPP = 0x001c,
        WM_SHOWWINDOW = 0x018,
        WM_WINDOWPOSCHANGING = 0x0046,
        WM_WINDOWPOSCHANGED = 0x0047,
        WM_SETTEXT = 0x000c,
        WM_GETTEXT = 0x000d,
        WM_GETTEXTLENGTH = 0x000e,
        WM_NCACTIVATE = 0x0086,
        WM_CAPTURECHANGED = 0x0215,
        WM_NCMOUSELEAVE = 0x02a2,
        WM_MOVING = 0x0216,
        WM_POINTERLEAVE = 0x024A,
        WM_POINTERUPDATE = 0x0245,
        WM_NCPOINTERUPDATE = 0x0241,
        WM_SIZE = 0x0005,
        WM_NCUAHDRAWCAPTION = 0x00AE,
        WM_NCHITTEST = 0x0084,
        WM_SIZING = 0x0214,
        WM_ENABLE = 0x000A,
        WM_ENTERSIZEMOVE = 0x0231,
        WM_EXITSIZEMOVE = 0x0232,
        WM_CONTEXTMENU = 0x007b,
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_USER = 0x0400,
        WM_GETICON = 0x007f,
        WM_SETICON = 0x0080,
        WM_DPICHANGED = 0x02E0,
        WM_DISPLAYCHANGE = 0x007E,
        WM_SETTINGCHANGE = 0x001A,
        WM_THEMECHANGE = 0x031A,
        WM_NCCALCSIZE = 0x0083,
        WM_NCPAINT = 0x0085,
        WM_NCPOINTERDOWN = 0x0242,
        WM_NCPOINTERUP = 0x0243,
        NIN_SELECT = WM_USER,
        NIN_KEYSELECT = WM_USER + 1,
        NIN_BALLOONSHOW = WM_USER + 2,
        NIN_BALLOONHIDE = WM_USER + 3,
        NIN_BALLOONTIMEOUT = WM_USER + 4,
        NIN_BALLOONUSERCLICK = WM_USER + 5,
        NIN_POPUPOPEN = WM_USER + 6,
        NIN_POPUPCLOSE = WM_USER + 7,
        WA_ACTIVE = 0x01,
        WA_INACTIVE = 0x00
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DispatcherQueueOptions
    {
        internal int dwSize;
        internal int threadType;
        internal int apartmentType;
    }
}
