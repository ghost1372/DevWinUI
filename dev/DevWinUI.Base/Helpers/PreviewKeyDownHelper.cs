using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

namespace DevWinUI;
public partial class PreviewKeyDownHelper
{
    private static readonly Func<VirtualKey, CoreVirtualKeyStates> _keyStateFunction = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread;

    private static bool IsModifierKeyDown()
    {
        return _keyStateFunction(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down) ||
               _keyStateFunction(VirtualKey.LeftMenu).HasFlag(CoreVirtualKeyStates.Down) ||
               _keyStateFunction(VirtualKey.RightMenu).HasFlag(CoreVirtualKeyStates.Down) ||
               _keyStateFunction(VirtualKey.LeftWindows).HasFlag(CoreVirtualKeyStates.Down) ||
               _keyStateFunction(VirtualKey.RightWindows).HasFlag(CoreVirtualKeyStates.Down);
    }

    public static void RejectLetters(object sender, KeyRoutedEventArgs e)
    {
        if (IsModifierKeyDown())
        {
            return;
        }

        e.Handled = e.Key switch
        {
            >= VirtualKey.A and <= VirtualKey.Z => true,
            _ => false,
        };
    }

    public static void AcceptLettersOnly(object sender, KeyRoutedEventArgs e)
    {
        if (IsModifierKeyDown()) return;

        e.Handled = e.Key switch
        {
            >= VirtualKey.A and <= VirtualKey.Z => false,
            VirtualKey.Space => false,
            VirtualKey.Escape => false,
            VirtualKey.Enter => false,
            VirtualKey.Back => false,
            VirtualKey.Tab => false,
            VirtualKey.Delete => false,
            VirtualKey.Left => false,
            VirtualKey.Right => false,
            VirtualKey.Up => false,
            VirtualKey.Down => false,
            VirtualKey.PageUp => false,
            VirtualKey.PageDown => false,
            VirtualKey.Home => false,
            VirtualKey.End => false,
            VirtualKey.Shift => false,
            (VirtualKey)188 => false, // Comma
            (VirtualKey)190 => false, // Dot
            _ => true,
        };
    }

    public static void AcceptNumbersOnly(object sender, KeyRoutedEventArgs e)
    {
        if (IsModifierKeyDown())
        {
            return;
        }

        e.Handled = e.Key switch
        {
            >= VirtualKey.Number0 and <= VirtualKey.Number9 => false,
            >= VirtualKey.NumberPad0 and <= VirtualKey.NumberPad9 => false,
            VirtualKey.Left => false,
            VirtualKey.Right => false,
            VirtualKey.Up => false,
            VirtualKey.Down => false,
            VirtualKey.PageUp => false,
            VirtualKey.PageDown => false,
            VirtualKey.Tab => false,
            VirtualKey.Home => false,
            VirtualKey.End => false,
            VirtualKey.Shift => false,
            VirtualKey.Back => false,
            VirtualKey.Delete => false,
            VirtualKey.Enter => false,
            VirtualKey.Decimal => false,
            (VirtualKey)188 => false, // Comma
            (VirtualKey)189 => false, // Minus
            (VirtualKey)190 => false, // Dot
            _ => true,
        };
    }

    public static void AcceptNumbersAndOperators(object sender, KeyRoutedEventArgs e)
    {
        if (IsModifierKeyDown())
        {
            return;
        }

        AcceptNumbersOnly(sender, e);
        if (e.Handled)
        {
            e.Handled = e.Key switch
            {
                VirtualKey.Multiply => false,
                VirtualKey.Divide => false,
                VirtualKey.Add => false,
                VirtualKey.Subtract => false,
                VirtualKey.Space => false,
                (VirtualKey)187 => false, // +
                _ => true,
            };
        }
    }
}
