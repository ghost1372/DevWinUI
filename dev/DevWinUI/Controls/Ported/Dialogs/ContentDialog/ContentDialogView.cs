// https://github.com/SuGar0218/SuGarToolkit.WinUI3
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

using Windows.System;

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
public partial class ContentDialogView : ContentControl, IContentDialog
{
    public ContentDialogView()
    {
        DefaultStyleKey = typeof(ContentDialogView);
        Loaded += OnLoaded;
    }

    #region DependencyProperty

    public object? Header
    {
        get => (object?) GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header),
        typeof(object),
        typeof(ContentDialogView),
        new PropertyMetadata(default(object), OnHeaderChanged)
    );

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentDialogView self = (ContentDialogView) d;
        self.DetermineTitleVisibilityState();
    }

    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?) GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
        nameof(HeaderTemplate),
        typeof(DataTemplate),
        typeof(ContentDialogView),
        new PropertyMetadata(default(DataTemplate))
    );

    public object? PrimaryButtonContent
    {
        get => (object?) GetValue(PrimaryButtonContentProperty);
        set => SetValue(PrimaryButtonContentProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonContentProperty = DependencyProperty.Register(
        nameof(PrimaryButtonContent),
        typeof(object),
        typeof(ContentDialogView),
        new PropertyMetadata(default(object), OnButtonContentChanged)
    );

    public DataTemplate? PrimaryButtonTemplate
    {
        get => (DataTemplate?) GetValue(PrimaryButtonTemplateProperty);
        set => SetValue(PrimaryButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonTemplateProperty = DependencyProperty.Register(
        nameof(PrimaryButtonTemplate),
        typeof(DataTemplate),
        typeof(ContentDialogView),
        new PropertyMetadata(default(DataTemplate))
    );

    public object? SecondaryButtonContent
    {
        get => (object?) GetValue(SecondaryButtonContentProperty);
        set => SetValue(SecondaryButtonContentProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonContentProperty = DependencyProperty.Register(
        nameof(SecondaryButtonContent),
        typeof(object),
        typeof(ContentDialogView),
        new PropertyMetadata(default(object), OnButtonContentChanged)
    );

    public DataTemplate? SecondaryButtonTemplate
    {
        get => (DataTemplate?) GetValue(SecondaryButtonTemplateProperty);
        set => SetValue(SecondaryButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonTemplateProperty = DependencyProperty.Register(
        nameof(SecondaryButtonTemplate),
        typeof(DataTemplate),
        typeof(ContentDialogView),
        new PropertyMetadata(default(DataTemplate))
    );

    public object? CloseButtonContent
    {
        get => (object?) GetValue(CloseButtonContentProperty);
        set => SetValue(CloseButtonContentProperty, value);
    }

    public static readonly DependencyProperty CloseButtonContentProperty = DependencyProperty.Register(
        nameof(CloseButtonContent),
        typeof(object),
        typeof(ContentDialogView),
        new PropertyMetadata(default(object), OnButtonContentChanged)
    );

    public DataTemplate? CloseButtonTemplate
    {
        get => (DataTemplate?) GetValue(CloseButtonTemplateProperty);
        set => SetValue(CloseButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty CloseButtonTemplateProperty = DependencyProperty.Register(
        nameof(CloseButtonTemplate),
        typeof(DataTemplate),
        typeof(ContentDialogView),
        new PropertyMetadata(default(DataTemplate))
    );
    
    public ICommand? PrimaryButtonCommand
    {
        get => (ICommand?) GetValue(PrimaryButtonCommandProperty);
        set => SetValue(PrimaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonCommandProperty = DependencyProperty.Register(
        nameof(PrimaryButtonCommand),
        typeof(ICommand),
        typeof(ContentDialogView),
        new PropertyMetadata(default(ICommand))
    );

    public ICommand? SecondaryButtonCommand
    {
        get => (ICommand?) GetValue(SecondaryButtonCommandProperty);
        set => SetValue(SecondaryButtonCommandProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonCommandProperty = DependencyProperty.Register(
        nameof(SecondaryButtonCommand),
        typeof(ICommand),
        typeof(ContentDialogView),
        new PropertyMetadata(default(ICommand))
    );

    public ICommand? CloseButtonCommand
    {
        get => (ICommand?) GetValue(CloseButtonCommandProperty);
        set => SetValue(CloseButtonCommandProperty, value);
    }

    public static readonly DependencyProperty CloseButtonCommandProperty = DependencyProperty.Register(
        nameof(CloseButtonCommand),
        typeof(ICommand),
        typeof(ContentDialogView),
        new PropertyMetadata(default(ICommand))
    );

    public bool IsPrimaryButtonEnabled
    {
        get => (bool) GetValue(IsPrimaryButtonEnabledProperty);
        set => SetValue(IsPrimaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsPrimaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsPrimaryButtonEnabled),
        typeof(bool),
        typeof(ContentDialogView),
        new PropertyMetadata(true)
    );

    public bool IsSecondaryButtonEnabled
    {
        get => (bool) GetValue(IsSecondaryButtonEnabledProperty);
        set => SetValue(IsSecondaryButtonEnabledProperty, value);
    }

    public static readonly DependencyProperty IsSecondaryButtonEnabledProperty = DependencyProperty.Register(
        nameof(IsSecondaryButtonEnabled),
        typeof(bool),
        typeof(ContentDialogView),
        new PropertyMetadata(true)
    );

    public Orientation ButtonOrientation
    {
        get => (Orientation) GetValue(ButtonOrientationProperty);
        set => SetValue(ButtonOrientationProperty, value);
    }

    public static readonly DependencyProperty ButtonOrientationProperty = DependencyProperty.Register(
        nameof(ButtonOrientation),
        typeof(Orientation),
        typeof(ContentDialogView),
        new PropertyMetadata(Orientation.Horizontal, OnButtonOrientationChanged)
    );

    public Style? PrimaryButtonStyle
    {
        get => (Style) GetValue(PrimaryButtonStyleProperty);
        set => SetValue(PrimaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register(
        nameof(PrimaryButtonStyle),
        typeof(Style),
        typeof(ContentDialogView),
        new PropertyMetadata(default(Style))
    );

    public Style? SecondaryButtonStyle
    {
        get => (Style) GetValue(SecondaryButtonStyleProperty);
        set => SetValue(SecondaryButtonStyleProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonStyleProperty = DependencyProperty.Register(
        nameof(SecondaryButtonStyle),
        typeof(Style),
        typeof(ContentDialogView),
        new PropertyMetadata(default(Style))
    );

    public Style? CloseButtonStyle
    {
        get => (Style) GetValue(CloseButtonStyleProperty);
        set => SetValue(CloseButtonStyleProperty, value);
    }

    public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register(
        nameof(CloseButtonStyle),
        typeof(Style),
        typeof(ContentDialogView),
        new PropertyMetadata(default(Style))
    );

    public string? PrimaryButtonAccessKey
    {
        get => (string?) GetValue(PrimaryButtonAccessKeyProperty);
        set => SetValue(PrimaryButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty PrimaryButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(PrimaryButtonAccessKey),
        typeof(string),
        typeof(ContentDialogView),
        new PropertyMetadata(default(string))
    );

    public string? SecondaryButtonAccessKey
    {
        get => (string?) GetValue(SecondaryButtonAccessKeyProperty);
        set => SetValue(SecondaryButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty SecondaryButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(SecondaryButtonAccessKey),
        typeof(string),
        typeof(ContentDialogView),
        new PropertyMetadata(default(string))
    );

    public string? CloseButtonAccessKey
    {
        get => (string?) GetValue(CloseButtonAccessKeyProperty);
        set => SetValue(CloseButtonAccessKeyProperty, value);
    }

    public static readonly DependencyProperty CloseButtonAccessKeyProperty = DependencyProperty.Register(
        nameof(CloseButtonAccessKey),
        typeof(string),
        typeof(ContentDialogView),
        new PropertyMetadata(default(string))
    );

    public ContentDialogButton DefaultButton
    {
        get => (ContentDialogButton) GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(
        nameof(DefaultButton),
        typeof(ContentDialogButton),
        typeof(ContentDialogView),
        new PropertyMetadata(default(ContentDialogButton), OnDefaultButtonChanged)
    );

    #endregion

    public ContentDialogResult Result { get; private set; }

    public event EventHandler? PrimaryButtonClick;
    public event EventHandler? SecondaryButtonClick;
    public event EventHandler? CloseButtonClick;

    public UIElement? TitleArea { get; private set; }
    private Grid? DialogSpace;
    private Border? CommandSpace;

    private Button? PrimaryButton;
    private Button? SecondaryButton;
    private Button? CloseButton;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TitleArea = (UIElement) GetTemplateChild(nameof(TitleArea));
        DialogSpace = (Grid) GetTemplateChild(nameof(DialogSpace));
        CommandSpace = (Border) GetTemplateChild(nameof(CommandSpace));

        PrimaryButton = (Button) GetTemplateChild(nameof(PrimaryButton));
        SecondaryButton = (Button) GetTemplateChild(nameof(SecondaryButton));
        CloseButton = (Button) GetTemplateChild(nameof(CloseButton));

        PrimaryButton.Click += OnPrimaryButtonClick;
        SecondaryButton.Click += OnSecondaryButtonClick;
        CloseButton.Click += OnCloseButtonClick;
    }

    private void OnPrimaryButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.Primary;
        PrimaryButtonClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnSecondaryButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.Secondary;
        SecondaryButtonClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.None;
        CloseButtonClick?.Invoke(this, EventArgs.Empty);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DetermineButtonsVisibilityState();
        DetermineDefaultButtonState();
        DetermineCommandSpacePlaceholderState();
        DetermineTitleVisibilityState();
    }

    private static void OnButtonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentDialogView self = (ContentDialogView) d;
        if (self.IsLoaded)
        {
            self.DetermineButtonsVisibilityState();
        }
    }

    private static void OnDefaultButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentDialogView self = (ContentDialogView) d;
        if (self.IsLoaded)
        {
            self.DetermineDefaultButtonState();
        }
    }

    private static void OnButtonOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentDialogView self = (ContentDialogView) d;
        if (self.IsLoaded)
        {
            self.DetermineCommandSpacePlaceholderState();
        }
    }

    private void DetermineButtonsVisibilityState()
    {
        bool isPrimaryButtonVisible = DeterminePrimaryButtonVisibility();
        bool isSecondaryButtonVisible = DetermineSecondaryButtonVisibility();
        bool isCloseButtonVisible = DetermineCloseButtonVisibility();

        if (isPrimaryButtonVisible)
        {
            if (isSecondaryButtonVisible)
            {
                if (isCloseButtonVisible)
                {
                    ButtonsVisibilityState = "AllVisible";
                }
                else
                {
                    ButtonsVisibilityState = "PrimaryAndSecondaryVisible";
                }
            }
            else
            {
                if (isCloseButtonVisible)
                {
                    ButtonsVisibilityState = "PrimaryAndCloseVisible";
                }
                else
                {
                    ButtonsVisibilityState = "PrimaryVisible";
                }
            }
        }
        else
        {
            if (isSecondaryButtonVisible)
            {
                if (isCloseButtonVisible)
                {
                    ButtonsVisibilityState = "SecondaryAndCloseVisible";
                }
                else
                {
                    ButtonsVisibilityState = "SecondaryVisible";
                }
            }
            else
            {
                if (isCloseButtonVisible)
                {
                    ButtonsVisibilityState = "CloseVisible";
                }
                else
                {
                    ButtonsVisibilityState = "NoneVisible";
                }
            }
        }
    }

    private void DetermineDefaultButtonState()
    {
        switch (DefaultButton)
        {
            case ContentDialogButton.Primary:
                PrimaryButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = VirtualKey.Enter });
                PrimaryButton.Focus(FocusState.Programmatic);
                DefaultButtonState = "PrimaryAsDefaultButton";
                break;

            case ContentDialogButton.Secondary:
                SecondaryButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = VirtualKey.Enter });
                SecondaryButton.Focus(FocusState.Programmatic);
                DefaultButtonState = "SecondaryAsDefaultButton";
                break;

            case ContentDialogButton.Close:
                CloseButton.KeyboardAccelerators.Add(new KeyboardAccelerator { Key = VirtualKey.Enter });
                CloseButton.Focus(FocusState.Programmatic);
                DefaultButtonState = "CloseAsDefaultButton";
                break;

            default:
                DefaultButtonState = "NoDefaultButton";
                break;
        }
    }

    private void DetermineCommandSpacePlaceholderState()
    {
        bool isPrimaryButtonVisible = DeterminePrimaryButtonVisibility();
        bool isSecondaryButtonVisible = DetermineSecondaryButtonVisibility();
        bool isCloseButtonVisible = DetermineCloseButtonVisibility();

        if (ButtonOrientation is Orientation.Horizontal)
        {
            int countVisible = 0;
            if (isPrimaryButtonVisible)
            {
                countVisible++;
            }
            if (isSecondaryButtonVisible)
            {
                countVisible++;
            }
            if (isCloseButtonVisible)
            {
                countVisible++;
            }
            if (countVisible == 1)
            {
                CommandSpacePlaceholderState = "CommandSpacePlaceholderVisible";
            }
            else
            {
                CommandSpacePlaceholderState = "CommandSpacePlaceholderCollapsed";
            }
        }
        else
        {
            CommandSpacePlaceholderState = "CommandSpacePlaceholderCollapsed";
        }
    }

    private void DetermineTitleVisibilityState()
    {
        if (Header is null)
        {
            TitleVisibilityState = "TitleCollapsed";
        }
        else
        {
            TitleVisibilityState = "TitleVisible";
        }
    }

    private bool DeterminePrimaryButtonVisibility() => PrimaryButtonContent is not null && (PrimaryButtonContent is not string text || !string.IsNullOrEmpty(text));
    private bool DetermineSecondaryButtonVisibility() => SecondaryButtonContent is not null && (SecondaryButtonContent is not string text || !string.IsNullOrEmpty(text));
    private bool DetermineCloseButtonVisibility() => CloseButtonContent is not null && (CloseButtonContent is not string text || !string.IsNullOrEmpty(text));

    private string? ButtonsVisibilityState
    {
        get => field;
        set
        {
            if (field == value)
                return;

            field = value;
            VisualStateManager.GoToState(this, value, false);
        }
    }

    private string? DefaultButtonState
    {
        get => field;
        set
        {
            if (field == value)
                return;

            field = value;
            VisualStateManager.GoToState(this, value, false);
        }
    }

    private string? CommandSpacePlaceholderState
    {
        get => field;
        set
        {
            if (field == value)
                return;

            field = value;
            VisualStateManager.GoToState(this, value, false);
        }
    }

    private string? TitleVisibilityState
    {
        get => field;
        set
        {
            if (field == value)
                return;

            field = value;
            VisualStateManager.GoToState(this, value, false);
        }
    }
}
