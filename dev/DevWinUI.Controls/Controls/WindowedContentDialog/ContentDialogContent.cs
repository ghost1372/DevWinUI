using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

internal sealed partial class ContentDialogContent : ContentControl
{
    public ContentDialogContent() : base()
    {
        DefaultStyleKey = typeof(ContentDialogContent);
        Unloaded += (o, e) => needsCustomMeasure = false;
    }

    private Button PrimaryButton;
    private Button SecondaryButton;
    private Button CloseButton;

    public event RoutedEventHandler? PrimaryButtonClick;
    public event RoutedEventHandler? SecondaryButtonClick;
    public event RoutedEventHandler? CloseButtonClick;

    public UIElement TitleArea { get; private set; }
    public Grid DialogSpace { get; private set; }
    public Grid CommandSpace { get; private set; }

    /// <summary>
    /// Whether customized measurement in MeasureOverride is needed.
    /// <br/>
    /// This variable is set to avoid redundant calculations.
    /// <br/>
    /// If the first measurement after Loaded is finished, there will be no need for customized measurement until Unloaded.
    /// </summary>
    private bool needsCustomMeasure;
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TitleArea = (UIElement)GetTemplateChild(nameof(TitleArea));
        DialogSpace = (Grid)GetTemplateChild(nameof(DialogSpace));
        CommandSpace = (Grid)GetTemplateChild(nameof(CommandSpace));

        PrimaryButton = (Button)GetTemplateChild(nameof(PrimaryButton));
        SecondaryButton = (Button)GetTemplateChild(nameof(SecondaryButton));
        CloseButton = (Button)GetTemplateChild(nameof(CloseButton));

        PrimaryButton.Click += PrimaryButtonClick;
        SecondaryButton.Click += SecondaryButtonClick;
        CloseButton.Click += CloseButtonClick;

        buttonsVisibilityState = DetermineButtonsVisibilityState();
        defaultButtonState = DetermineDefaultButtonState();
    }
    protected override Size MeasureOverride(Size availableSize)
    {
        if (needsCustomMeasure)
            return base.MeasureOverride(availableSize);

        needsCustomMeasure = IsLoaded;

        int countButtons = 0;
        double buttonLongestWidth = 0.0;
        double buttonMaxWidth = (double)Application.Current.Resources["ContentDialogButtonMaxWidth"];
        if (PrimaryButton.Visibility is Visibility.Visible)
        {
            PrimaryButton.Measure(availableSize);
            buttonLongestWidth = Math.Min(Math.Max(buttonLongestWidth, PrimaryButton.DesiredSize.Width), buttonMaxWidth);
            countButtons++;
        }
        if (SecondaryButton.Visibility is Visibility.Visible)
        {
            SecondaryButton.Measure(availableSize);
            buttonLongestWidth = Math.Min(Math.Max(buttonLongestWidth, SecondaryButton.DesiredSize.Width), buttonMaxWidth);
            countButtons++;
        }
        if (CloseButton.Visibility is Visibility.Visible)
        {
            CloseButton.Measure(availableSize);
            buttonLongestWidth = Math.Min(Math.Max(buttonLongestWidth, CloseButton.DesiredSize.Width), buttonMaxWidth);
            countButtons++;
        }

        double commandSpaceExpectedWidth = CommandSpace.Padding.Left + CommandSpace.Padding.Right
            + countButtons * buttonLongestWidth
            + (countButtons - 1) * ((GridLength)Application.Current.Resources["ContentDialogButtonSpacing"]).Value;

        double minWidth = Math.Max((double)Application.Current.Resources["ContentDialogMinWidth"], commandSpaceExpectedWidth);
        double maxWidth = Math.Max((double)Application.Current.Resources["ContentDialogMaxWidth"], commandSpaceExpectedWidth);
        if (availableSize.Width > maxWidth)
        {
            availableSize.Width = maxWidth;
        }
        Size desiredSize = base.MeasureOverride(availableSize);
        if (desiredSize.Width < minWidth)
        {
            desiredSize.Width = minWidth;
        }
        return desiredSize;
    }
    public void AfterGotFocus()
    {
        VisualStateManager.GoToState(this, defaultButtonState, false);
    }

    public void AfterLostFocus()
    {
        VisualStateManager.GoToState(this, "NoDefaultButton", false);
    }

    private string buttonsVisibilityState = string.Empty;
    private string defaultButtonState = string.Empty;
    private string DetermineButtonsVisibilityState()
    {
        if (!string.IsNullOrEmpty(PrimaryButtonText) && !string.IsNullOrEmpty(SecondaryButtonText) && !string.IsNullOrEmpty(CloseButtonText))
        {
            VisualStateManager.GoToState(this, "AllVisible", false);
            return "AllVisible";
        }
        else if (!string.IsNullOrEmpty(PrimaryButtonText))
        {
            if (!string.IsNullOrEmpty(SecondaryButtonText))
            {
                VisualStateManager.GoToState(this, "PrimaryAndSecondaryVisible", false);
                IsSecondaryButtonEnabled = true;
                return "PrimaryAndSecondaryVisible";
            }
            else if (!string.IsNullOrEmpty(CloseButtonText))
            {
                VisualStateManager.GoToState(this, "PrimaryAndCloseVisible", false);
                IsSecondaryButtonEnabled = false;
                return "PrimaryAndCloseVisible";
            }
            else
            {
                VisualStateManager.GoToState(this, "PrimaryVisible", false);
                IsSecondaryButtonEnabled = false;
                return "PrimaryVisible";
            }
        }
        else if (!string.IsNullOrEmpty(SecondaryButtonText))
        {
            IsPrimaryButtonEnabled = false;
            if (!string.IsNullOrEmpty(CloseButtonText))
            {
                VisualStateManager.GoToState(this, "SecondaryAndCloseVisible", false);
                return "SecondaryAndCloseVisible";
            }
            else
            {
                VisualStateManager.GoToState(this, "SecondaryVisible", false);
                return "SecondaryAndCloseVisible";
            }
        }
        else if (!string.IsNullOrEmpty(CloseButtonText))
        {
            VisualStateManager.GoToState(this, "CloseVisible", false);
            return "CloseVisible";
        }
        else
        {
            VisualStateManager.GoToState(this, "NoneVisible", false);
            IsPrimaryButtonEnabled = false;
            IsSecondaryButtonEnabled = false;
            return "NoneVisible";
        }
    }

    private string DetermineDefaultButtonState()
    {
        switch (DefaultButton)
        {
            case ContentDialogButton.Primary:
                VisualStateManager.GoToState(this, "PrimaryAsDefaultButton", false);
                PrimaryButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.Enter });
                PrimaryButton.Focus(FocusState.Programmatic);
                return "PrimaryAsDefaultButton";
            case ContentDialogButton.Secondary:
                VisualStateManager.GoToState(this, "SecondaryAsDefaultButton", false);
                SecondaryButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.Enter });
                SecondaryButton.Focus(FocusState.Programmatic);
                return "SecondaryAsDefaultButton";
            case ContentDialogButton.Close:
                VisualStateManager.GoToState(this, "CloseAsDefaultButton", false);
                CloseButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.Enter });
                CloseButton.Focus(FocusState.Programmatic);
                return "CloseAsDefaultButton";
            case ContentDialogButton.None:
                VisualStateManager.GoToState(this, "NoDefaultButton", false);
                return "NoDefaultButton";
            default:
                VisualStateManager.GoToState(this, "NoDefaultButton", false);
                return "NoDefaultButton";
        }
    }

    #region Title Property
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(object),
            typeof(ContentDialogContent),
            new PropertyMetadata(default(string)));

    public object? Title
    {
        get => (object?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    #endregion

    #region TitleTemplate Property
    public static readonly DependencyProperty TitleTemplateProperty =
        DependencyProperty.Register(
            nameof(TitleTemplate),
            typeof(DataTemplate),
            typeof(ContentDialogContent),
            new PropertyMetadata(default(DataTemplate)));

    public DataTemplate? TitleTemplate
    {
        get => (DataTemplate?)GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
    }
    #endregion

    #region PrimaryButtonText Property
    public static readonly DependencyProperty PrimaryButtonTextProperty =
        DependencyProperty.Register(
            nameof(PrimaryButtonText),
            typeof(string),
            typeof(ContentDialogContent),
            new PropertyMetadata(default(string)));

    public string? PrimaryButtonText
    {
        get => (string?)GetValue(PrimaryButtonTextProperty);
        set => SetValue(PrimaryButtonTextProperty, value);
    }
    #endregion

    #region SecondaryButtonText Property
    public static readonly DependencyProperty SecondaryButtonTextProperty =
        DependencyProperty.Register(
            nameof(SecondaryButtonText),
            typeof(string),
            typeof(ContentDialogContent),
            new PropertyMetadata(default(string)));

    public string? SecondaryButtonText
    {
        get => (string?)GetValue(SecondaryButtonTextProperty);
        set => SetValue(SecondaryButtonTextProperty, value);
    }
    #endregion

    #region CloseButtonText Property
    public static readonly DependencyProperty CloseButtonTextProperty =
        DependencyProperty.Register(
            nameof(CloseButtonText),
            typeof(string),
            typeof(ContentDialogContent),
            new PropertyMetadata(default(string)));

    public string? CloseButtonText
    {
        get => (string?)GetValue(CloseButtonTextProperty);
        set => SetValue(CloseButtonTextProperty, value);
    }
    #endregion

    #region IsPrimaryButtonEnabled Property
    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty =
        DependencyProperty.Register(
            nameof(IsPrimaryButtonEnabled),
            typeof(bool),
            typeof(ContentDialogContent),
            new PropertyMetadata(true));

    public bool IsPrimaryButtonEnabled
    {
        get => (bool)GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }
    #endregion

    #region IsSecondaryButtonEnabled Property
    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty =
        DependencyProperty.Register(
            nameof(IsSecondaryButtonEnabled),
            typeof(bool),
            typeof(ContentDialogContent),
            new PropertyMetadata(true));

    public bool IsSecondaryButtonEnabled
    {
        get => (bool)GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }
    #endregion

    #region DefaultButton Property
    public static readonly DependencyProperty DefaultButtonProperty =
        DependencyProperty.Register(
            nameof(DefaultButton),
            typeof(ContentDialogButton),
            typeof(ContentDialogContent),
            new PropertyMetadata(ContentDialogButton.Close));

    public ContentDialogButton DefaultButton
    {
        get => (ContentDialogButton)GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }
    #endregion

    #region PrimaryButtonStyle Property
    public static readonly DependencyProperty PrimaryButtonStyleProperty =
        DependencyProperty.Register(
            nameof(PrimaryButtonStyle),
            typeof(Style),
            typeof(ContentDialogContent),
            new PropertyMetadata(DefaultButtonStyle));

    public Style? PrimaryButtonStyle
    {
        get => (Style?)GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }
    #endregion

    #region SecondaryButtonStyle Property
    public static readonly DependencyProperty SecondaryButtonStyleProperty =
        DependencyProperty.Register(
            nameof(SecondaryButtonStyle),
            typeof(Style),
            typeof(ContentDialogContent),
            new PropertyMetadata(DefaultButtonStyle));

    public Style? SecondaryButtonStyle
    {
        get => (Style?)GetValue(SecondaryButtonStyleProperty);
        set => SetValue(SecondaryButtonStyleProperty, value);
    }
    #endregion

    #region CloseButtonStyle Property
    public static readonly DependencyProperty CloseButtonStyleProperty =
        DependencyProperty.Register(
            nameof(CloseButtonStyle),
            typeof(Style),
            typeof(ContentDialogContent),
            new PropertyMetadata(DefaultButtonStyle));

    public Style? CloseButtonStyle
    {
        get => (Style?)GetValue(CloseButtonStyleProperty);
        set => SetValue(CloseButtonStyleProperty, value);
    }
    #endregion

    private static Style DefaultButtonStyle => (Style)Application.Current.Resources["DefaultButtonStyle"];
}
