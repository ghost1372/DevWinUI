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
        public int X;
        public int Y;

        public static implicit operator System.Drawing.Point(POINT point)
        {
            return new(point.X, point.Y);
        }

        public static implicit operator POINT(System.Drawing.Point point)
        {
            return new() { X = point.X, Y = point.Y };
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
        WS_EX_LAYOUTRTL = 0x00400000,
        Popup = unchecked(0x80000000),
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
        WM_NCRBUTTONUP = 0x00A5,
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
        WA_INACTIVE = 0x00,
        WM_INITMENUPOPUP = 0x0117
    }
    
    internal enum NOTIFYICON : uint
    {
        NIM_ADD = 0x00000000,
        NIM_MODIFY = 0x00000001,
        NIM_DELETE = 0x00000002,
        NIM_SETVERSION = 0x00000004,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NOTIFYICONIDENTIFIER
    {
        public uint cbSize;
        public nint hWnd;
        public uint uID;
        public Guid guidItem;
    }

    internal partial struct NOTIFYICONDATAW64
    {
        internal uint cbSize;
        internal HWND hWnd;
        internal uint uID;
        internal uint uFlags;
        internal uint uCallbackMessage;
        internal Windows.Win32.UI.WindowsAndMessaging.HICON hIcon;
        internal __ushort_128 szTip;
        internal uint dwState;
        internal uint dwStateMask;
        internal __ushort_256 szInfo;
        internal uint VersionOrTimeout;
        internal __ushort_64 szInfoTitle;
        internal uint dwInfoFlags;
        internal global::System.Guid guidItem;
        internal Windows.Win32.UI.WindowsAndMessaging.HICON hBalloonIcon;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal partial struct NOTIFYICONDATAW32
    {
        internal uint cbSize;
        internal HWND hWnd;
        internal uint uID;
        internal uint uFlags;
        internal uint uCallbackMessage;
        internal Windows.Win32.UI.WindowsAndMessaging.HICON hIcon;
        internal __ushort_128 szTip;
        internal uint dwState;
        internal uint dwStateMask;
        internal __ushort_256 szInfo;
        internal uint VersionOrTimeout;
        internal __ushort_64 szInfoTitle;
        internal uint dwInfoFlags;
        internal global::System.Guid guidItem;
        internal Windows.Win32.UI.WindowsAndMessaging.HICON hBalloonIcon;

    }

    internal struct __ushort_64
    {
        internal ushort _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31, _32, _33, _34, _35, _36, _37, _38, _39, _40, _41, _42, _43, _44, _45, _46, _47, _48, _49, _50, _51, _52, _53, _54, _55, _56, _57, _58, _59, _60, _61, _62, _63;
        internal int Length => 64;
        internal ref ushort this[int index] => ref AsSpan()[index];
        internal Span<ushort> AsSpan() => MemoryMarshal.CreateSpan(ref _0, 64);
    }

    internal struct __ushort_256
    {
        internal ushort _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31, _32, _33, _34, _35, _36, _37, _38, _39, _40, _41, _42, _43, _44, _45, _46, _47, _48, _49, _50, _51, _52, _53, _54, _55, _56, _57, _58, _59, _60, _61, _62, _63, _64, _65, _66, _67, _68, _69, _70, _71, _72, _73, _74, _75, _76, _77, _78, _79, _80, _81, _82, _83, _84, _85, _86, _87, _88, _89, _90, _91, _92, _93, _94, _95, _96, _97, _98, _99, _100, _101, _102, _103, _104, _105, _106, _107, _108, _109, _110, _111, _112, _113, _114, _115, _116, _117, _118, _119, _120, _121, _122, _123, _124, _125, _126, _127, _128, _129, _130, _131, _132, _133, _134, _135, _136, _137, _138, _139, _140, _141, _142, _143, _144, _145, _146, _147, _148, _149, _150, _151, _152, _153, _154, _155, _156, _157, _158, _159, _160, _161, _162, _163, _164, _165, _166, _167, _168, _169, _170, _171, _172, _173, _174, _175, _176, _177, _178, _179, _180, _181, _182, _183, _184, _185, _186, _187, _188, _189, _190, _191, _192, _193, _194, _195, _196, _197, _198, _199, _200, _201, _202, _203, _204, _205, _206, _207, _208, _209, _210, _211, _212, _213, _214, _215, _216, _217, _218, _219, _220, _221, _222, _223, _224, _225, _226, _227, _228, _229, _230, _231, _232, _233, _234, _235, _236, _237, _238, _239, _240, _241, _242, _243, _244, _245, _246, _247, _248, _249, _250, _251, _252, _253, _254, _255;
        internal int Length => 256;
        internal ref ushort this[int index] => ref AsSpan()[index];
        internal Span<ushort> AsSpan() => MemoryMarshal.CreateSpan(ref _0, 256);
    }
    internal struct __ushort_128
    {
        internal ushort _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11, _12, _13, _14, _15, _16, _17, _18, _19, _20, _21, _22, _23, _24, _25, _26, _27, _28, _29, _30, _31, _32, _33, _34, _35, _36, _37, _38, _39, _40, _41, _42, _43, _44, _45, _46, _47, _48, _49, _50, _51, _52, _53, _54, _55, _56, _57, _58, _59, _60, _61, _62, _63, _64, _65, _66, _67, _68, _69, _70, _71, _72, _73, _74, _75, _76, _77, _78, _79, _80, _81, _82, _83, _84, _85, _86, _87, _88, _89, _90, _91, _92, _93, _94, _95, _96, _97, _98, _99, _100, _101, _102, _103, _104, _105, _106, _107, _108, _109, _110, _111, _112, _113, _114, _115, _116, _117, _118, _119, _120, _121, _122, _123, _124, _125, _126, _127;
        internal int Length => 128;
        internal ref ushort this[int index] => ref AsSpan()[index];
        internal Span<ushort> AsSpan() => MemoryMarshal.CreateSpan(ref _0, 128);
    }
}
