//https://github.com/SuGar0218/WindowedContentDialog

using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = nameof(PrimaryButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(SecondaryButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(CloseButton), Type = typeof(Button))]
[TemplatePart(Name = nameof(TitleArea), Type = typeof(UIElement))]
[TemplatePart(Name = nameof(DialogSpace), Type = typeof(Grid))]
[TemplatePart(Name = nameof(CommandSpace), Type = typeof(Grid))]
[TemplateVisualState(Name = "AllVisible", GroupName = "ButtonsVisibilityStates")]
[TemplateVisualState(Name = "NoneVisible", GroupName = "ButtonsVisibilityStates")]
[TemplateVisualState(Name = "PrimaryVisible", GroupName = "ButtonsVisibilityStates")]
[TemplateVisualState(Name = "SecondaryVisible", GroupName = "ButtonsVisibilityStates")]
[TemplateVisualState(Name = "CloseVisible", GroupName = "ButtonsVisibilityStates")]
[TemplateVisualState(Name = "PrimaryAndSecondaryVisible", GroupName = "ButtonsVisibilityStates")]
[TemplateVisualState(Name = "PrimaryAndCloseVisible", GroupName = "ButtonsVisibilityStates")]
[TemplateVisualState(Name = "SecondaryAndCloseVisible", GroupName = "ButtonsVisibilityStates")]
public partial class ContentDialogContent : ContentControl
{
    public ContentDialogContent() : base()
    {
        DefaultStyleKey = typeof(ContentDialogContent);
        Loaded += OnLoaded;
        Unloaded += (o, e) => isCustomMeasureFinishedAfterLoaded = false;
    }

    #region properties
    public object? Title
    {
        get { return (object?)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(object), typeof(ContentDialogContent), new PropertyMetadata(null));

    public DataTemplate? TitleTemplate
    {
        get { return (DataTemplate?)GetValue(TitleTemplateProperty); }
        set { SetValue(TitleTemplateProperty, value); }
    }

    public static readonly DependencyProperty TitleTemplateProperty =
        DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(ContentDialogContent), new PropertyMetadata(null));

    public string? PrimaryButtonText
    {
        get { return (string?)GetValue(PrimaryButtonTextProperty); }
        set { SetValue(PrimaryButtonTextProperty, value); }
    }

    public static readonly DependencyProperty PrimaryButtonTextProperty =
        DependencyProperty.Register(nameof(PrimaryButtonText), typeof(string), typeof(ContentDialogContent), new PropertyMetadata(null, OnButtonTextChanged));

    public string? SecondaryButtonText
    {
        get { return (string?)GetValue(SecondaryButtonTextProperty); }
        set { SetValue(SecondaryButtonTextProperty, value); }
    }

    public static readonly DependencyProperty SecondaryButtonTextProperty =
        DependencyProperty.Register(nameof(SecondaryButtonText), typeof(string), typeof(ContentDialogContent), new PropertyMetadata(null, OnButtonTextChanged));

    public string? CloseButtonText
    {
        get { return (string?)GetValue(CloseButtonTextProperty); }
        set { SetValue(CloseButtonTextProperty, value); }
    }

    public static readonly DependencyProperty CloseButtonTextProperty =
        DependencyProperty.Register(nameof(CloseButtonText), typeof(string), typeof(ContentDialogContent), new PropertyMetadata(null, OnButtonTextChanged));

    public bool IsPrimaryButtonEnabled
    {
        get { return (bool)GetValue(IsPrimaryButtonEnabledProperty); }
        set { SetValue(IsPrimaryButtonEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsPrimaryButtonEnabled), typeof(bool), typeof(ContentDialogContent), new PropertyMetadata(true));

    public bool IsSecondaryButtonEnabled
    {
        get { return (bool)GetValue(IsSecondaryButtonEnabledProperty); }
        set { SetValue(IsSecondaryButtonEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsSecondaryButtonEnabled), typeof(bool), typeof(ContentDialogContent), new PropertyMetadata(true));

    public ContentDialogButton DefaultButton
    {
        get { return (ContentDialogButton)GetValue(DefaultButtonProperty); }
        set { SetValue(DefaultButtonProperty, value); }
    }

    public static readonly DependencyProperty DefaultButtonProperty =
        DependencyProperty.Register(nameof(DefaultButton), typeof(ContentDialogButton), typeof(ContentDialogContent), new PropertyMetadata(ContentDialogButton.Close, OnDefaultButtonChanged));

    public Style? PrimaryButtonStyle
    {
        get { return (Style?)GetValue(PrimaryButtonStyleProperty); }
        set { SetValue(PrimaryButtonStyleProperty, value); }
    }

    public static readonly DependencyProperty PrimaryButtonStyleProperty =
        DependencyProperty.Register(nameof(PrimaryButtonStyle), typeof(Style), typeof(ContentDialogContent), new PropertyMetadata(DefaultButtonStyle));

    public Style? SecondaryButtonStyle
    {
        get { return (Style?)GetValue(SecondaryButtonStyleProperty); }
        set { SetValue(SecondaryButtonStyleProperty, value); }
    }

    public static readonly DependencyProperty SecondaryButtonStyleProperty =
        DependencyProperty.Register(nameof(SecondaryButtonStyle), typeof(Style), typeof(ContentDialogContent), new PropertyMetadata(DefaultButtonStyle));

    public Style? CloseButtonStyle
    {
        get { return (Style?)GetValue(CloseButtonStyleProperty); }
        set { SetValue(CloseButtonStyleProperty, value); }
    }

    public static readonly DependencyProperty CloseButtonStyleProperty =
        DependencyProperty.Register(nameof(CloseButtonStyle), typeof(Style), typeof(ContentDialogContent), new PropertyMetadata(DefaultButtonStyle));
    #endregion

    public event TypedEventHandler<ContentDialogContent, EventArgs>? PrimaryButtonClick;
    public event TypedEventHandler<ContentDialogContent, EventArgs>? SecondaryButtonClick;
    public event TypedEventHandler<ContentDialogContent, EventArgs>? CloseButtonClick;

    public IList<KeyboardAccelerator> PrimaryButtonKeyboardAccelerators => field ??= [];
    public IList<KeyboardAccelerator> SecondaryButtonKeyboardAccelerators => field ??= [];
    public IList<KeyboardAccelerator> CloseButtonKeyboardAccelerators => field ??= [];

    public string PrimaryButtonAccessKey
    {
        get { return (string)GetValue(PrimaryButtonAccessKeyProperty); }
        set { SetValue(PrimaryButtonAccessKeyProperty, value); }
    }

    public static readonly DependencyProperty PrimaryButtonAccessKeyProperty =
        DependencyProperty.Register(nameof(PrimaryButtonAccessKey), typeof(string), typeof(ContentDialogContent), new PropertyMetadata(null));

    public string SecondaryButtonAccessKey
    {
        get { return (string)GetValue(SecondaryButtonAccessKeyProperty); }
        set { SetValue(SecondaryButtonAccessKeyProperty, value); }
    }

    public static readonly DependencyProperty SecondaryButtonAccessKeyProperty =
        DependencyProperty.Register(nameof(SecondaryButtonAccessKey), typeof(string), typeof(ContentDialogContent), new PropertyMetadata(null));

    public string CloseButtonAccessKey
    {
        get { return (string)GetValue(CloseButtonAccessKeyProperty); }
        set { SetValue(CloseButtonAccessKeyProperty, value); }
    }

    public static readonly DependencyProperty CloseButtonAccessKeyProperty =
        DependencyProperty.Register(nameof(CloseButtonAccessKey), typeof(string), typeof(ContentDialogContent), new PropertyMetadata(null));

    public UIElement TitleArea { get; private set; }
    public Grid DialogSpace { get; private set; }
    public Grid CommandSpace { get; private set; }

    private Button PrimaryButton;
    private Button SecondaryButton;
    private Button CloseButton;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TitleArea = (UIElement)GetTemplateChild(nameof(TitleArea));
        DialogSpace = (Grid)GetTemplateChild(nameof(DialogSpace));
        CommandSpace = (Grid)GetTemplateChild(nameof(CommandSpace));

        PrimaryButton = (Button)GetTemplateChild(nameof(PrimaryButton));
        SecondaryButton = (Button)GetTemplateChild(nameof(SecondaryButton));
        CloseButton = (Button)GetTemplateChild(nameof(CloseButton));

        PrimaryButton.Click += (sender, args) => PrimaryButtonClick?.Invoke(this, EventArgs.Empty);
        SecondaryButton.Click += (sender, args) => SecondaryButtonClick?.Invoke(this, EventArgs.Empty);
        CloseButton.Click += (sender, args) => CloseButtonClick?.Invoke(this, EventArgs.Empty);

        foreach (KeyboardAccelerator keyboardAccelerator in PrimaryButtonKeyboardAccelerators)
        {
            PrimaryButton.KeyboardAccelerators.Add(keyboardAccelerator);
        }
        foreach (KeyboardAccelerator keyboardAccelerator in SecondaryButtonKeyboardAccelerators)
        {
            SecondaryButton.KeyboardAccelerators.Add(keyboardAccelerator);
        }
        foreach (KeyboardAccelerator keyboardAccelerator in CloseButtonKeyboardAccelerators)
        {
            CloseButton.KeyboardAccelerators.Add(keyboardAccelerator);
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        buttonsVisibilityState = DetermineButtonsVisibilityState();
        defaultButtonState = DetermineDefaultButtonState();
    }

    private bool isCustomMeasureFinishedAfterLoaded;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (isCustomMeasureFinishedAfterLoaded)
            return base.MeasureOverride(availableSize);

        isCustomMeasureFinishedAfterLoaded = IsLoaded;
        return CustomMeasure(availableSize);
    }

    private Size CustomMeasure(Size availableSize)
    {
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

    private static void OnButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentDialogContent self = (ContentDialogContent)d;
        if (self.IsLoaded)
        {
            self.buttonsVisibilityState = self.DetermineButtonsVisibilityState();
            self.isCustomMeasureFinishedAfterLoaded = false;
        }
    }

    private static void OnDefaultButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentDialogContent self = (ContentDialogContent)d;
        if (self.IsLoaded)
        {
            self.defaultButtonState = self.DetermineDefaultButtonState();
        }
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

    private static Style DefaultButtonStyle => field ??= (Style)Application.Current.Resources["DefaultButtonStyle"];
}
