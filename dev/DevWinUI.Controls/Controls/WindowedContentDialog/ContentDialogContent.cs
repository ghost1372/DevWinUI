using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

internal sealed partial class ContentDialogContent : ContentControl
{
    public ContentDialogContent() : base()
    {
        DefaultStyleKey = typeof(ContentDialogContent);

        VerticalAlignment = VerticalAlignment.Center;
        HorizontalAlignment = HorizontalAlignment.Center;

        CommandSpace = null!;
        PrimaryButton = null!;
        SecondaryButton = null!;
        CloseButton = null!;
        TitleArea = null!;
    }

    private Button PrimaryButton;
    private Button SecondaryButton;
    private Button CloseButton;

    public event RoutedEventHandler? PrimaryButtonClick;
    public event RoutedEventHandler? SecondaryButtonClick;
    public event RoutedEventHandler? CloseButtonClick;

    public UIElement TitleArea { get; private set; }
    public Grid CommandSpace { get; private set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TitleArea = (UIElement)GetTemplateChild(nameof(TitleArea));
        CommandSpace = (Grid)GetTemplateChild(nameof(CommandSpace));

        PrimaryButton = (Button)GetTemplateChild(nameof(PrimaryButton));
        SecondaryButton = (Button)GetTemplateChild(nameof(SecondaryButton));
        CloseButton = (Button)GetTemplateChild(nameof(CloseButton));

        PrimaryButton.Click += PrimaryButtonClick;
        SecondaryButton.Click += SecondaryButtonClick;
        CloseButton.Click += CloseButtonClick;

        VisualStateManager.GoToState(this, "DialogShowingWithoutSmokeLayer", false);
        DetermineButtonsVisibilityState();
        DetermineDefaultButtonStates();
        DetermineWidthLimit();
    }

    public void AfterGotFocus()
    {
        DetermineDefaultButtonStates();
    }

    public void AfterLostFocus()
    {
        VisualStateManager.GoToState(this, "NoDefaultButton", false);
    }

    private void DetermineButtonsVisibilityState()
    {
        if (!string.IsNullOrEmpty(PrimaryButtonText) && !string.IsNullOrEmpty(SecondaryButtonText) && !string.IsNullOrEmpty(CloseButtonText))
        {
            VisualStateManager.GoToState(this, "AllVisible", false);
        }
        else if (!string.IsNullOrEmpty(PrimaryButtonText))
        {
            if (!string.IsNullOrEmpty(SecondaryButtonText))
            {
                VisualStateManager.GoToState(this, "PrimaryAndSecondaryVisible", false);
                IsSecondaryButtonEnabled = true;
            }
            else if (!string.IsNullOrEmpty(CloseButtonText))
            {
                VisualStateManager.GoToState(this, "PrimaryAndCloseVisible", false);
                IsSecondaryButtonEnabled = false;
            }
            else
            {
                VisualStateManager.GoToState(this, "PrimaryVisible", false);
                IsSecondaryButtonEnabled = false;
            }
        }
        else if (!string.IsNullOrEmpty(SecondaryButtonText))
        {
            if (!string.IsNullOrEmpty(CloseButtonText))
            {
                VisualStateManager.GoToState(this, "SecondaryAndCloseVisible", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "SecondaryVisible", false);
            }
            IsPrimaryButtonEnabled = false;
        }
        else if (!string.IsNullOrEmpty(CloseButtonText))
        {
            VisualStateManager.GoToState(this, "CloseVisible", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "NoneVisible", false);
            IsPrimaryButtonEnabled = false;
            IsSecondaryButtonEnabled = false;
        }
    }

    private void DetermineDefaultButtonStates()
    {
        switch (DefaultButton)
        {
            case ContentDialogButton.None:
                VisualStateManager.GoToState(this, "NoDefaultButton", false);
                break;
            case ContentDialogButton.Primary:
                VisualStateManager.GoToState(this, "PrimaryAsDefaultButton", false);
                PrimaryButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.Enter });
                PrimaryButton.Focus(FocusState.Programmatic);
                break;
            case ContentDialogButton.Secondary:
                VisualStateManager.GoToState(this, "SecondaryAsDefaultButton", false);
                SecondaryButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.Enter });
                SecondaryButton.Focus(FocusState.Programmatic);
                break;
            case ContentDialogButton.Close:
                VisualStateManager.GoToState(this, "CloseAsDefaultButton", false);
                CloseButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = Windows.System.VirtualKey.Enter });
                CloseButton.Focus(FocusState.Programmatic);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Determine min/max width.
    /// Should be used after DetermineDefaultButtonStates() because it depends on visibilies of buttons.
    /// </summary>
    private void DetermineWidthLimit()
    {
        int countButtons = 0;
        double buttonLongestWidth = 0.0;
        double buttonMaxWidth = (double)Application.Current.Resources["ContentDialogButtonMaxWidth"];
        if (PrimaryButton.Visibility is Visibility.Visible)
        {
            PrimaryButton.InvalidateMeasure();
            PrimaryButton.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            buttonLongestWidth = Math.Min(Math.Max(buttonLongestWidth, PrimaryButton.DesiredSize.Width), buttonMaxWidth);
            countButtons++;
        }
        if (SecondaryButton.Visibility is Visibility.Visible)
        {
            SecondaryButton.InvalidateMeasure();
            SecondaryButton.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            buttonLongestWidth = Math.Min(Math.Max(buttonLongestWidth, SecondaryButton.DesiredSize.Width), buttonMaxWidth);
            countButtons++;
        }
        if (CloseButton.Visibility is Visibility.Visible)
        {
            CloseButton.InvalidateMeasure();
            CloseButton.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            buttonLongestWidth = Math.Min(Math.Max(buttonLongestWidth, CloseButton.DesiredSize.Width), buttonMaxWidth);
            countButtons++;
        }

        Thickness padding = (Thickness)Application.Current.Resources["ContentDialogPadding"];
        double expectedWidth = padding.Left + padding.Right;
        expectedWidth += countButtons * buttonLongestWidth;
        expectedWidth += (countButtons - 1) * ((GridLength)Application.Current.Resources["ContentDialogButtonSpacing"]).Value;

        MinWidth = Math.Max(expectedWidth, (double)Application.Current.Resources["ContentDialogMinWidth"]);
        MaxWidth = Math.Max(expectedWidth, (double)Application.Current.Resources["ContentDialogMaxWidth"]);

        Loaded += (o, e) => RemoveSizeLimit();
    }

    private void RemoveSizeLimit()
    {
        MaxWidth = double.PositiveInfinity;
        MaxHeight = double.PositiveInfinity;
        MinWidth = 0.0;
        MinHeight = 0.0;

        VerticalAlignment = VerticalAlignment.Stretch;
        HorizontalAlignment = HorizontalAlignment.Stretch;
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
