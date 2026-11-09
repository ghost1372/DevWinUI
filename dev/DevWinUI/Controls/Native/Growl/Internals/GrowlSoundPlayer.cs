using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

internal static partial class GrowlSoundPlayer
{
    public static void Play(GrowlSound sound, GrowlCardSeverity severity, Uri? uri)
    {
        if (sound == GrowlSound.None)
        {
            return;
        }

        try
        {
            if (uri is not null)
            {
                PlayUri(uri);
                return;
            }

            PInvoke.MessageBeep(Resolve(sound, severity));
        }
        catch (Exception)
        {
        }
    }

    private static MESSAGEBOX_STYLE Resolve(GrowlSound sound, GrowlCardSeverity severity)
    {
        return sound switch
        {
            GrowlSound.Auto => severity switch
            {
                GrowlCardSeverity.Info => MESSAGEBOX_STYLE.MB_ICONASTERISK,
                GrowlCardSeverity.Warning => MESSAGEBOX_STYLE.MB_ICONEXCLAMATION,
                GrowlCardSeverity.Error => MESSAGEBOX_STYLE.MB_ICONHAND,
                _ => MESSAGEBOX_STYLE.MB_OK
            },
            GrowlSound.Information => MESSAGEBOX_STYLE.MB_ICONASTERISK,
            GrowlSound.Warning => MESSAGEBOX_STYLE.MB_ICONEXCLAMATION,
            GrowlSound.Error => MESSAGEBOX_STYLE.MB_ICONHAND,
            GrowlSound.Question => MESSAGEBOX_STYLE.MB_ICONQUESTION,
            _ => MESSAGEBOX_STYLE.MB_OK
        };
    }

    private static void PlayUri(Uri uri)
    {
        var source = MediaSource.CreateFromUri(uri);
        var player = new MediaPlayer { Source = source };

        void Release(MediaPlayer sender, object args)
        {
            sender.Dispose();
            source.Dispose();
        }

        player.MediaEnded += Release;
        player.MediaFailed += (sender, _) => Release(sender, null!);
        player.Play();
    }
}
