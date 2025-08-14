namespace DevWinUI;
public partial class BaseShortcut : Control
{
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(BaseShortcut), new PropertyMetadata("Press a combination of keys to change this shortcut"));


    public List<object> Keys
    {
        get { return (List<object>)GetValue(KeysProperty); }
        set { SetValue(KeysProperty, value); }
    }

    public static readonly DependencyProperty KeysProperty =
        DependencyProperty.Register(nameof(Keys), typeof(List<object>), typeof(BaseShortcut), new PropertyMetadata(null));

    public bool IsError
    {
        get => (bool)GetValue(IsErrorProperty);
        set => SetValue(IsErrorProperty, value);
    }

    public static readonly DependencyProperty IsErrorProperty =
        DependencyProperty.Register(nameof(IsError), typeof(bool), typeof(BaseShortcut), new PropertyMetadata(false));

    public string ErrorTitle
    {
        get { return (string)GetValue(ErrorTitleProperty); }
        set { SetValue(ErrorTitleProperty, value); }
    }

    public static readonly DependencyProperty ErrorTitleProperty =
        DependencyProperty.Register(nameof(ErrorTitle), typeof(string), typeof(BaseShortcut), new PropertyMetadata("Invalid shortcut"));
    public string ErrorToolTip
    {
        get { return (string)GetValue(ErrorToolTipProperty); }
        set { SetValue(ErrorToolTipProperty, value); }
    }

    public static readonly DependencyProperty ErrorToolTipProperty =
        DependencyProperty.Register(nameof(ErrorToolTip), typeof(string), typeof(BaseShortcut), new PropertyMetadata(null));

    public bool IsWarning
    {
        get { return (bool)GetValue(IsWarningProperty); }
        set { SetValue(IsWarningProperty, value); }
    }

    public static readonly DependencyProperty IsWarningProperty =
        DependencyProperty.Register(nameof(IsWarning), typeof(bool), typeof(BaseShortcut), new PropertyMetadata(false));

    public string WarningTitle
    {
        get { return (string)GetValue(WarningTitleProperty); }
        set { SetValue(WarningTitleProperty, value); }
    }

    public static readonly DependencyProperty WarningTitleProperty =
        DependencyProperty.Register(nameof(WarningTitle), typeof(string), typeof(BaseShortcut), new PropertyMetadata("Possible shortcut interference with Alt Gr"));

    public string WarningToolTip
    {
        get { return (string)GetValue(WarningToolTipProperty); }
        set { SetValue(WarningToolTipProperty, value); }
    }

    public static readonly DependencyProperty WarningToolTipProperty =
        DependencyProperty.Register(nameof(WarningToolTip), typeof(string), typeof(BaseShortcut), new PropertyMetadata("Shortcuts with **Ctrl** and **Alt** may remove functionality from some international keyboards, because **Ctrl** + **Alt** = **Alt Gr** in those keyboards."));
    public bool IsInfo
    {
        get { return (bool)GetValue(IsInfoProperty); }
        set { SetValue(IsInfoProperty, value); }
    }

    public static readonly DependencyProperty IsInfoProperty =
        DependencyProperty.Register(nameof(IsInfo), typeof(bool), typeof(BaseShortcut), new PropertyMetadata(true));

    public string InfoTitle
    {
        get { return (string)GetValue(InfoTitleProperty); }
        set { SetValue(InfoTitleProperty, value); }
    }

    public static readonly DependencyProperty InfoTitleProperty =
        DependencyProperty.Register(nameof(InfoTitle), typeof(string), typeof(BaseShortcut), new PropertyMetadata("Only shortcuts that start with Windows key, Ctrl, Alt or Shift are valid."));

    public string InfoToolTip
    {
        get { return (string)GetValue(InfoToolTipProperty); }
        set { SetValue(InfoToolTipProperty, value); }
    }

    public static readonly DependencyProperty InfoToolTipProperty =
        DependencyProperty.Register(nameof(InfoToolTip), typeof(string), typeof(BaseShortcut), new PropertyMetadata(null));
}
