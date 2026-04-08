using System.Security;

namespace DevWinUI;

[TemplatePart(Name = PART_Panel, Type = typeof(UniformGrid))]
public partial class PinBox : Control
{
    private const string PART_Panel = "PART_Panel";
    private UniformGrid _uniformGrid;
    private List<SecureString> _passwordList;
    private long _foregroundPropertyToken;
    private long _backgroundPropertyToken;
    private long _borderBrushPropertyToken;
    private long _borderThicknessPropertyToken;
    private long _contextFlyoutPropertyToken;
    private long _cornerRadiusPropertyToken;

    public PinBox()
    {
        this.DefaultStyleKey = typeof(PinBox);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _uniformGrid = GetTemplateChild(PART_Panel) as UniformGrid;
        UpdateOrientation(Orientation);
        CreatePinBoxes(PasswordLength);
        UpdatePinBoxes(GetPassword(), PasswordLength);

        UnRegisterPropertyChanged(ForegroundProperty, _foregroundPropertyToken);
        UnRegisterPropertyChanged(BackgroundProperty, _backgroundPropertyToken);
        UnRegisterPropertyChanged(BorderBrushProperty, _borderBrushPropertyToken);
        UnRegisterPropertyChanged(BorderThicknessProperty, _borderThicknessPropertyToken);
        UnRegisterPropertyChanged(ContextFlyoutProperty, _contextFlyoutPropertyToken);
        UnRegisterPropertyChanged(CornerRadiusProperty, _cornerRadiusPropertyToken);
        RegisterPropertyChanged(ForegroundProperty, _foregroundPropertyToken, OnForegroundChanged);
        RegisterPropertyChanged(BackgroundProperty, _backgroundPropertyToken, OnBackgroundChanged);
        RegisterPropertyChanged(BorderBrushProperty, _borderBrushPropertyToken, OnBorderBrushChanged);
        RegisterPropertyChanged(BorderThicknessProperty, _borderThicknessPropertyToken, OnBorderThicknessChanged);
        RegisterPropertyChanged(ContextFlyoutProperty, _contextFlyoutPropertyToken, OnContextFlyoutChanged);
        RegisterPropertyChanged(CornerRadiusProperty, _cornerRadiusPropertyToken, OnCornerRadiusChanged);
    }

    #region Dependency Property Changed
    private void OnForegroundChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.Foreground = Foreground;
            }
        }
    }
    private void OnBackgroundChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.Background = Background;
            }
        }
    }
    private void OnBorderBrushChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.BorderBrush = BorderBrush;
            }
        }
    }
    private void OnBorderThicknessChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.BorderThickness = BorderThickness;
            }
        }
    }
    private void OnContextFlyoutChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.ContextFlyout = ContextFlyout;
            }
        }
    }
    private void OnCornerRadiusChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.CornerRadius = CornerRadius;
            }
        }
    }

    private static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl != null && ctl._uniformGrid != null)
        {
            string newValue = ctl.GetPassword();
            ctl.UpdatePinBoxes(newValue, ctl.PasswordLength);
            ctl.PasswordChanged?.Invoke(ctl, new PinBoxPasswordChangedEventArgs((string)e.OldValue, newValue));
        }
    }

    private static void OnPasswordLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;

        if (ctl != null)
        {
            ctl.CreatePinBoxes((int)e.NewValue);
            ctl.UpdatePinBoxes(ctl.GetPassword(), (int)e.NewValue);
            ctl.UpdateOrientation(ctl.Orientation);
        }
    }

    private static void OnPasswordRevealModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl._uniformGrid != null)
        {
            foreach (PasswordBox box in ctl._uniformGrid.Children)
            {
                box.PasswordRevealMode = (PasswordRevealMode)e.NewValue;
            }
        }
    }

    private static void OnPasswordCharChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl._uniformGrid != null)
        {
            foreach (PasswordBox box in ctl._uniformGrid.Children)
            {
                box.PasswordChar = ctl.GetPasswordChar((string)e.NewValue);
            }
        }
    }

    private static void OnItemSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl != null)
        {
            ctl.UpdateOrientation(ctl.Orientation);
        }
    }

    private static void OnItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl != null && ctl._uniformGrid != null)
        {
            ctl.CreatePinBoxes(ctl.PasswordLength);
            ctl.UpdatePinBoxes(ctl.GetPassword(), ctl.PasswordLength);
        }
    }

    private static void OnPlaceHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        ctl.UpdatePlaceHolderText((string)e.NewValue);
    }

    private static void OnSelectionHighlightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        ctl.UpdateSelectionHighlightColor();
    }

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl != null)
        {
            ctl.UpdateOrientation((Orientation)e.NewValue);
        }
    }

    private static void OnShowSuccessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl != null)
        {
            if ((bool)e.NewValue)
            {
                ctl.GoToSuccessMode();
            }
            else
            {
                ctl.GoToNormalMode();
            }
        }
    }

    private static void OnShowErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PinBox)d;
        if (ctl != null)
        {
            if ((bool)e.NewValue)
            {
                ctl.GoToErrorMode();
            }
            else
            {
                ctl.GoToNormalMode();
            }
        }
    }
    #endregion

    #region PinBox Events
    private void OnBoxPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_uniformGrid == null)
            return;

        var currentBox = sender as PasswordBox;
        var currentBoxIndex = GetCurrentPasswordBoxIndex(currentBox);

        int nextBoxIndex = currentBoxIndex + 1;

        if (string.IsNullOrEmpty(currentBox.Password))
        {
            nextBoxIndex = currentBoxIndex - 1;
        }

        FocusBoxAndSelectAll(nextBoxIndex);

        var password = string.Join(string.Empty, _uniformGrid.Children.OfType<PasswordBox>().Select(item => item.Password));
        Password = password;
    }
    private void OnBoxKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (_uniformGrid != null)
        {
            var currentBox = sender as PasswordBox;
            var currentBoxIndex = GetCurrentPasswordBoxIndex(currentBox);

            switch (Orientation)
            {
                case Orientation.Vertical:
                    if (e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.PageUp)
                    {
                        var nextBoxIndex = currentBoxIndex - 1;
                        FocusBoxAndSelectAll(nextBoxIndex);
                    }

                    if (e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.PageDown)
                    {
                        var nextBoxIndex = currentBoxIndex + 1;
                        FocusBoxAndSelectAll(nextBoxIndex);
                    }
                    break;
                case Orientation.Horizontal:
                    if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.PageDown)
                    {
                        var nextBoxIndex = currentBoxIndex - 1;
                        FocusBoxAndSelectAll(nextBoxIndex);
                    }

                    if (e.Key == Windows.System.VirtualKey.Right || e.Key == Windows.System.VirtualKey.PageUp)
                    {
                        var nextBoxIndex = currentBoxIndex + 1;
                        FocusBoxAndSelectAll(nextBoxIndex);
                    }
                    break;
            }

            if (e.Key == Windows.System.VirtualKey.Delete || e.Key == Windows.System.VirtualKey.Back)
            {
                var nextBoxIndex = currentBoxIndex - 1;
                currentBox.Password = string.Empty;
                FocusBoxAndSelectAll(nextBoxIndex);
            }
        }
    }

    private void OnBoxGettingFocus(UIElement sender, Microsoft.UI.Xaml.Input.GettingFocusEventArgs args)
    {
        var currentBox = sender as PasswordBox;
        currentBox.SelectAll();
        UpdateFocusMode(currentBox);
    }
    #endregion

    private void CreatePinBoxes(int passwordLength)
    {
        if (passwordLength == 0 || passwordLength < 0)
        {
            passwordLength = 4;
        }

        if (_uniformGrid == null)
            return;

        _uniformGrid.Children.Clear();

        for (int i = 1; i <= passwordLength; i++)
        {
            var pinBox = CreatePinBox(i);
            _uniformGrid.Children.Add(pinBox);
        }
    }
    private PasswordBox CreatePinBox(int index)
    {
        string password = null;

        if (_passwordList != null && index - 1 < _passwordList.Count)
        {
            var secureString = _passwordList[index - 1];
            password = new System.Net.NetworkCredential(string.Empty, secureString).Password;
        }

        var passwordBox = new PasswordBox
        {
            MaxLength = 1,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Width = ItemWidth,
            Padding = default,
            PasswordChar = GetPasswordChar(PasswordChar),
            Foreground = this.Foreground,
            Background = this.Background,
            SelectionHighlightColor = this.SelectionHighlightColor,
            PasswordRevealMode = this.PasswordRevealMode
        };

        if (!string.IsNullOrEmpty(password))
        {
            passwordBox.Password = password;
        }

        passwordBox.PasswordChanged += OnBoxPasswordChanged;
        passwordBox.KeyDown += OnBoxKeyDown;
        passwordBox.GettingFocus += OnBoxGettingFocus;
        return passwordBox;
    }
    private void UpdatePinBoxes(string value, int passwordLength)
    {
        if (_uniformGrid == null)
        {
            FillPasswordList(value);
            return;
        }

        FillPasswordList(value);

        if (string.IsNullOrEmpty(value) || passwordLength <= 0)
        {
            foreach (var passwordBox in _uniformGrid.Children.OfType<PasswordBox>())
            {
                passwordBox.Password = string.Empty;
            }
        }
        else
        {
            int count = Math.Min(passwordLength, value.Length);
            var passwordBoxes = _uniformGrid.Children.OfType<PasswordBox>().Take(count).ToList();

            for (int index = 0; index < count; index++)
            {
                passwordBoxes[index].Password = value[index].ToString();
            }

            var passwordBoxesToClear = _uniformGrid.Children
                .OfType<PasswordBox>()
                .Skip(value.Length)
                .Take(passwordLength - value.Length);

            foreach (var passwordBox in passwordBoxesToClear)
            {
                passwordBox.Password = string.Empty;
            }
        }

        UpdatePlaceHolderText(PlaceholderText);
    }
    private void UpdatePlaceHolderText(string newValue)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.PlaceholderText = string.Empty;
            }

            if (!string.IsNullOrEmpty(newValue))
            {
                string chars;

                if (newValue.Length < PasswordLength)
                {
                    chars = newValue.Substring(0, newValue.Length);
                }
                else
                {
                    chars = newValue.Substring(0, PasswordLength);

                }
                List<char> charsList = new List<char>(chars);
                for (int i = 0; i < charsList.Count; i++)
                {
                    PasswordBox box = _uniformGrid.Children[i] as PasswordBox;

                    box.PlaceholderText = charsList[i].ToString();
                }
            }
        }
    }
    private void UpdateSelectionHighlightColor()
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                box.SelectionHighlightColor = SelectionHighlightColor;
            }
        }
    }
    public void UpdateFocusMode(PasswordBox passwordBox)
    {
        switch (FocusMode)
        {
            case PinBoxFocusMode.Normal:
                VisualStateManager.GoToState(passwordBox, "NormalFocused", true);
                break;
            case PinBoxFocusMode.Complete:
                VisualStateManager.GoToState(passwordBox, "CompleteFocused", true);
                break;
            default:
                VisualStateManager.GoToState(passwordBox, "CompleteFocused", true);
                break;
        }
    }
    private void FillPasswordList(string value)
    {
        if (_passwordList == null)
        {
            _passwordList = new List<SecureString>();
        }
        else
        {
            _passwordList.Clear();
        }

        if (string.IsNullOrEmpty(value))
            return;

        foreach (var item in value)
        {
            var secureString = new SecureString();
            secureString.AppendChar(item);
            _passwordList.Add(secureString);
        }
    }
    private void FocusBoxAndSelectAll(int nextBoxIndex)
    {
        if (nextBoxIndex != -1 && nextBoxIndex <= _uniformGrid.Children.Count - 1)
        {
            PasswordBox nextBox = (PasswordBox)_uniformGrid.Children[nextBoxIndex];
            nextBox.Focus(FocusState.Programmatic);
            nextBox.SelectAll();
        }
    }
    public void FocusAllPinBoxes()
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                switch (FocusMode)
                {
                    case PinBoxFocusMode.Normal:
                        VisualStateManager.GoToState(box, "NormalFocused", true);
                        break;
                    case PinBoxFocusMode.Complete:
                        VisualStateManager.GoToState(box, "CompleteFocused", true);
                        break;
                }
            }
        }
    }
    public void GoToSuccessMode()
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                Border borderElement = GetBorderElement(box);
                if (borderElement != null)
                {
                    borderElement.BorderThickness = FocusMode switch
                    {
                        PinBoxFocusMode.Normal => new Thickness(0, 0, 0, 2),
                        PinBoxFocusMode.Complete => new Thickness(2),
                        _ => new Thickness(2),
                    };
                    borderElement.BorderBrush = Application.Current.Resources["SystemFillColorSuccessBrush"] as Brush;
                }
            }
        }
    }
    public void GoToErrorMode()
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                Border borderElement = GetBorderElement(box);
                if (borderElement != null)
                {
                    borderElement.BorderThickness = FocusMode switch
                    {
                        PinBoxFocusMode.Normal => new Thickness(0, 0, 0, 2),
                        PinBoxFocusMode.Complete => new Thickness(2),
                        _ => new Thickness(2),
                    };
                    borderElement.BorderBrush = Application.Current.Resources["SystemControlErrorTextForegroundBrush"] as Brush;
                }
            }
        }
    }
    public void GoToNormalMode()
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                Border borderElement = GetBorderElement(box);
                if (borderElement != null)
                {
                    var thickness = (Thickness)Application.Current.Resources["TextControlBorderThemeThickness"];
                    borderElement.BorderThickness = thickness;
                    borderElement.BorderBrush = Application.Current.Resources["TextControlBorderBrush"] as Brush;
                }
            }
        }
    }
    public void GoToCustomMode(Thickness borderThickness, Brush borderBrush)
    {
        if (_uniformGrid != null)
        {
            foreach (PasswordBox box in _uniformGrid.Children)
            {
                Border borderElement = GetBorderElement(box);
                if (borderElement != null)
                {
                    borderElement.BorderThickness = borderThickness;
                    borderElement.BorderBrush = borderBrush;
                }
            }
        }
    }
    private void UpdateOrientation(Orientation orientation)
    {
        if (_uniformGrid != null)
        {
            _uniformGrid.Orientation = orientation;

            switch (orientation)
            {
                case Orientation.Vertical:
                    _uniformGrid.Rows = PasswordLength;
                    _uniformGrid.Columns = 1;
                    _uniformGrid.RowSpacing = ItemSpacing;
                    _uniformGrid.ColumnSpacing = 0;
                    break;
                case Orientation.Horizontal:

                    _uniformGrid.Rows = 1;
                    _uniformGrid.Columns = PasswordLength;
                    _uniformGrid.RowSpacing = 0;
                    _uniformGrid.ColumnSpacing = ItemSpacing;
                    break;
            }
        }
    }
}
