namespace DevWinUI;

public sealed partial class MagicKeyboard
{
    public MagicKeyboardSkin Skin
    {
        get { return (MagicKeyboardSkin)GetValue(SkinProperty); }
        set { SetValue(SkinProperty, value); }
    }

    public static readonly DependencyProperty SkinProperty =
        DependencyProperty.Register(nameof(Skin), typeof(MagicKeyboardSkin), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardSkin.System, OnRebuildChanged));

    public MagicKeyboardSkin LightSkin
    {
        get { return (MagicKeyboardSkin)GetValue(LightSkinProperty); }
        set { SetValue(LightSkinProperty, value); }
    }

    public static readonly DependencyProperty LightSkinProperty =
        DependencyProperty.Register(nameof(LightSkin), typeof(MagicKeyboardSkin), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardSkin.Silver, OnRebuildChanged));

    public MagicKeyboardSkin DarkSkin
    {
        get { return (MagicKeyboardSkin)GetValue(DarkSkinProperty); }
        set { SetValue(DarkSkinProperty, value); }
    }

    public static readonly DependencyProperty DarkSkinProperty =
        DependencyProperty.Register(nameof(DarkSkin), typeof(MagicKeyboardSkin), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardSkin.Midnight, OnRebuildChanged));

    public MagicKeyboardLayout Layout
    {
        get { return (MagicKeyboardLayout)GetValue(LayoutProperty); }
        set { SetValue(LayoutProperty, value); }
    }

    public static readonly DependencyProperty LayoutProperty =
        DependencyProperty.Register(nameof(Layout), typeof(MagicKeyboardLayout), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardLayout.Ansi, OnRebuildChanged));

    public MagicKeyboardFormFactor FormFactor
    {
        get { return (MagicKeyboardFormFactor)GetValue(FormFactorProperty); }
        set { SetValue(FormFactorProperty, value); }
    }

    public static readonly DependencyProperty FormFactorProperty =
        DependencyProperty.Register(nameof(FormFactor), typeof(MagicKeyboardFormFactor), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardFormFactor.Full, OnRebuildChanged));

    public MagicKeyboardPlatform Platform
    {
        get { return (MagicKeyboardPlatform)GetValue(PlatformProperty); }
        set { SetValue(PlatformProperty, value); }
    }

    public static readonly DependencyProperty PlatformProperty =
        DependencyProperty.Register(nameof(Platform), typeof(MagicKeyboardPlatform), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardPlatform.Windows, OnRebuildChanged));

    public MagicKeyboardKeyCase LetterCase
    {
        get { return (MagicKeyboardKeyCase)GetValue(LetterCaseProperty); }
        set { SetValue(LetterCaseProperty, value); }
    }

    public static readonly DependencyProperty LetterCaseProperty =
        DependencyProperty.Register(nameof(LetterCase), typeof(MagicKeyboardKeyCase), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardKeyCase.Lower, (d, _) => ((MagicKeyboard)d).UpdateCase()));

    public bool LinkModifierKeys
    {
        get { return (bool)GetValue(LinkModifierKeysProperty); }
        set { SetValue(LinkModifierKeysProperty, value); }
    }

    public static readonly DependencyProperty LinkModifierKeysProperty =
        DependencyProperty.Register(nameof(LinkModifierKeys), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(true, (d, _) => ((MagicKeyboard)d).RefreshModifierPairs()));

    public MagicKeyboardKeyModifierMode ModifierMode
    {
        get { return (MagicKeyboardKeyModifierMode)GetValue(ModifierModeProperty); }
        set { SetValue(ModifierModeProperty, value); }
    }

    public static readonly DependencyProperty ModifierModeProperty =
        DependencyProperty.Register(nameof(ModifierMode), typeof(MagicKeyboardKeyModifierMode), typeof(MagicKeyboard), new PropertyMetadata(MagicKeyboardKeyModifierMode.Sticky, (d, _) => ((MagicKeyboard)d).ReleaseLatched()));

    public bool IsSoundEnabled
    {
        get { return (bool)GetValue(IsSoundEnabledProperty); }
        set { SetValue(IsSoundEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsSoundEnabledProperty =
        DependencyProperty.Register(nameof(IsSoundEnabled), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(false, OnSoundChanged));

    public string KeyDownSoundUri
    {
        get { return (string)GetValue(KeyDownSoundUriProperty); }
        set { SetValue(KeyDownSoundUriProperty, value); }
    }

    public static readonly DependencyProperty KeyDownSoundUriProperty =
        DependencyProperty.Register(nameof(KeyDownSoundUri), typeof(string), typeof(MagicKeyboard), new PropertyMetadata("", OnSoundChanged));

    public string KeyUpSoundUri
    {
        get { return (string)GetValue(KeyUpSoundUriProperty); }
        set { SetValue(KeyUpSoundUriProperty, value); }
    }

    public static readonly DependencyProperty KeyUpSoundUriProperty =
        DependencyProperty.Register(nameof(KeyUpSoundUri), typeof(string), typeof(MagicKeyboard), new PropertyMetadata("", OnSoundChanged));

    public double SoundVolume
    {
        get { return (double)GetValue(SoundVolumeProperty); }
        set { SetValue(SoundVolumeProperty, value); }
    }

    public static readonly DependencyProperty SoundVolumeProperty =
        DependencyProperty.Register(nameof(SoundVolume), typeof(double), typeof(MagicKeyboard), new PropertyMetadata(0.5));

    public bool IsPreviewVisible
    {
        get { return (bool)GetValue(IsPreviewVisibleProperty); }
        set { SetValue(IsPreviewVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsPreviewVisibleProperty =
        DependencyProperty.Register(nameof(IsPreviewVisible), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(false, (d, _) => ((MagicKeyboard)d).UpdatePreviewVisibility()));

    public bool IsPhysicalKeyboardEnabled
    {
        get { return (bool)GetValue(IsPhysicalKeyboardEnabledProperty); }
        set { SetValue(IsPhysicalKeyboardEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsPhysicalKeyboardEnabledProperty =
        DependencyProperty.Register(nameof(IsPhysicalKeyboardEnabled), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(true, OnListenersChanged));

    public bool IsAlwaysActive
    {
        get { return (bool)GetValue(IsAlwaysActiveProperty); }
        set { SetValue(IsAlwaysActiveProperty, value); }
    }

    public static readonly DependencyProperty IsAlwaysActiveProperty =
        DependencyProperty.Register(nameof(IsAlwaysActive), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(false, OnListenersChanged));

    public bool FollowSystemKeyboardLayout
    {
        get { return (bool)GetValue(FollowSystemKeyboardLayoutProperty); }
        set { SetValue(FollowSystemKeyboardLayoutProperty, value); }
    }

    public static readonly DependencyProperty FollowSystemKeyboardLayoutProperty =
        DependencyProperty.Register(nameof(FollowSystemKeyboardLayout), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(true, (d, _) => ((MagicKeyboard)d).OnLanguageModeChanged()));

    public bool SyncCapsLock
    {
        get { return (bool)GetValue(SyncCapsLockProperty); }
        set { SetValue(SyncCapsLockProperty, value); }
    }

    public static readonly DependencyProperty SyncCapsLockProperty =
        DependencyProperty.Register(nameof(SyncCapsLock), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(true, (d, _) => ((MagicKeyboard)d).SyncCapsFromSystem()));

    public bool SyncNumLock
    {
        get { return (bool)GetValue(SyncNumLockProperty); }
        set { SetValue(SyncNumLockProperty, value); }
    }

    public static readonly DependencyProperty SyncNumLockProperty =
        DependencyProperty.Register(nameof(SyncNumLock), typeof(bool), typeof(MagicKeyboard), new PropertyMetadata(true, (d, _) => ((MagicKeyboard)d).SyncNumFromSystem()));

    public event TypedEventHandler<MagicKeyboard, MagicKeyEventArgs>? KeyEvent;

    private static void OnRebuildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MagicKeyboard)d).Rebuild();

    private static void OnSoundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MagicKeyboard)d).EnsureSounds();

    private static void OnListenersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((MagicKeyboard)d).UpdateListeners();
}
