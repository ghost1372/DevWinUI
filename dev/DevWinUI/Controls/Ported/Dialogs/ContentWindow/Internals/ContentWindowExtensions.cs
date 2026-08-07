namespace DevWinUI;

internal static partial class ContentWindowExtensions
{
    extension(Window window)
    {
        public Color? BorderColor() => Win32DwmWindowAttributes.GetBorderColor(window.Handle());
        public void BorderColor(Color? color) => Win32DwmWindowAttributes.SetBorderColor(window.Handle(), color);
        public WindowCornerRoundness CornerRoundness() => Win32DwmWindowAttributes.GetCornerRoundness(window.Handle());
        public void CornerRoundness(WindowCornerRoundness cornerRoundness) => Win32DwmWindowAttributes.SetCornerRoundness(window.Handle(), cornerRoundness);
        private nint Handle() => WinRT.Interop.WindowNative.GetWindowHandle(window);
    }
}
