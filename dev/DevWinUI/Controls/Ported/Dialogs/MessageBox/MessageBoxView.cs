// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_UacStyleDialogView), Type = typeof(UacStyleDialogView))]
[TemplateVisualState(GroupName = "IconStates", Name = "None")]
[TemplateVisualState(GroupName = "IconStates", Name = "Information")]
[TemplateVisualState(GroupName = "IconStates", Name = "Question")]
[TemplateVisualState(GroupName = "IconStates", Name = "Error")]
[TemplateVisualState(GroupName = "IconStates", Name = "Warning")]
[TemplateVisualState(GroupName = "IconStates", Name = "Success")]
public partial class MessageBoxView : Control
{
    public MessageBoxView()
    {
        DefaultStyleKey = typeof(MessageBoxView);
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    #region DependencyProperty

    public string? Title
    {
        get => (string?) GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(MessageBoxView),
        new PropertyMetadata(default(string))
    );

    public string? Message
    {
        get => (string?) GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(
        nameof(Message),
        typeof(string),
        typeof(MessageBoxView),
        new PropertyMetadata(default(string))
    );

    public MessageBoxIcon Icon
    {
        get => (MessageBoxIcon) GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(MessageBoxIcon),
        typeof(MessageBoxView),
        new PropertyMetadata(default(MessageBoxIcon), OnIconChanged)
    );

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        MessageBoxView self = (MessageBoxView) d;
        self.DetermineIconState();
    }

    public MessageBoxButtons Buttons
    {
        get => (MessageBoxButtons) GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
    }

    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(
        nameof(Buttons),
        typeof(MessageBoxButtons),
        typeof(MessageBoxView),
        new PropertyMetadata(default(MessageBoxButtons))
    );

    public MessageBoxDefaultButton DefaultButton
    {
        get => (MessageBoxDefaultButton) GetValue(DefaultButtonProperty);
        set => SetValue(DefaultButtonProperty, value);
    }

    public static readonly DependencyProperty DefaultButtonProperty = DependencyProperty.Register(
        nameof(DefaultButton),
        typeof(MessageBoxDefaultButton),
        typeof(MessageBoxView),
        new PropertyMetadata(default(MessageBoxDefaultButton))
    );

    #endregion

    public event EventHandler? ResultChanged;

    public MessageBoxResult Result
    {
        get => field;
        private set
        {
            if (field == value)
                return;

            field = value;
            ResultChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private UacStyleDialogView PART_UacStyleDialogView;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        PART_UacStyleDialogView = (UacStyleDialogView) GetTemplateChild(nameof(PART_UacStyleDialogView));
        PART_UacStyleDialogView.PrimaryButtonClick += OnPrimaryButtonClick;
        PART_UacStyleDialogView.SecondaryButtonClick += OnSecondaryButtonClick;
        PART_UacStyleDialogView.CloseButtonClick += OnCloseButtonClick;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
        //PART_UacStyleDialogView.MaxWidth = double.PositiveInfinity;
        DetermineIconState();
        DetermineButtons();
	}

	private void OnSizeChanged(object sender, SizeChangedEventArgs e)
	{
        if (IsLoaded)
        {
			PART_UacStyleDialogView.MaxWidth = double.PositiveInfinity;
		}
	}

	private void OnPrimaryButtonClick(object? sender, EventArgs e)
    {
        Result = Buttons switch
        {
            MessageBoxButtons.OK => MessageBoxResult.OK,
            MessageBoxButtons.OKCancel => MessageBoxResult.OK,
            MessageBoxButtons.AbortRetryIgnore => MessageBoxResult.Abort,
            MessageBoxButtons.YesNoCancel => MessageBoxResult.Yes,
            MessageBoxButtons.YesNo => MessageBoxResult.Yes,
            MessageBoxButtons.RetryCancel => MessageBoxResult.Retry,
            MessageBoxButtons.CancelTryContinue => MessageBoxResult.Continue,
            _ => MessageBoxResult.None,
        };
    }

    private void OnSecondaryButtonClick(object? sender, EventArgs e)
    {
        Result = Buttons switch
        {
            MessageBoxButtons.OK => MessageBoxResult.OK,
            MessageBoxButtons.OKCancel => MessageBoxResult.Cancel,
            MessageBoxButtons.AbortRetryIgnore => MessageBoxResult.Retry,
            MessageBoxButtons.YesNoCancel => MessageBoxResult.No,
            MessageBoxButtons.YesNo => MessageBoxResult.No,
            MessageBoxButtons.RetryCancel => MessageBoxResult.Cancel,
            MessageBoxButtons.CancelTryContinue => MessageBoxResult.TryAgain,
            _ => MessageBoxResult.None,
        };
    }

    private void OnCloseButtonClick(object? sender, EventArgs e)
    {
        Result = Buttons switch
        {
            MessageBoxButtons.OK => MessageBoxResult.OK,
            MessageBoxButtons.OKCancel => MessageBoxResult.Cancel,
            MessageBoxButtons.AbortRetryIgnore => MessageBoxResult.Ignore,
            MessageBoxButtons.YesNoCancel => MessageBoxResult.Cancel,
            MessageBoxButtons.YesNo => MessageBoxResult.No,
            MessageBoxButtons.RetryCancel => MessageBoxResult.Cancel,
            MessageBoxButtons.CancelTryContinue => MessageBoxResult.Cancel,
            _ => MessageBoxResult.None,
        };
    }

    private void DetermineIconState()
    {
        IconState = Icon.ToString();
    }

    private string? IconState
    {
        get => field;
        set
        {
            field = value;
            VisualStateManager.GoToState(this, value, false);
        }
    }

    private void DetermineButtons()
    {
        switch (Buttons)
        {
            case MessageBoxButtons.OK:
                {
                    MessageBoxButtonString okString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.OK);
                    PART_UacStyleDialogView.CloseButtonContent = okString.Text;
                    PART_UacStyleDialogView.CloseButtonAccessKey = okString.Key;
                    PART_UacStyleDialogView.DefaultButton = ContentDialogButton.Close;
                    break;
                }

            case MessageBoxButtons.OKCancel:
                {
                    MessageBoxButtonString okString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.OK);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    PART_UacStyleDialogView.PrimaryButtonContent = okString.Text;
                    PART_UacStyleDialogView.PrimaryButtonAccessKey = okString.Key;
                    PART_UacStyleDialogView.CloseButtonContent = cancelString.Text;
                    PART_UacStyleDialogView.CloseButtonAccessKey = cancelString.Key;
                    PART_UacStyleDialogView.DefaultButton = ContentDialogButton.Close;
                    break;
                }

            case MessageBoxButtons.YesNo:
                {
                    MessageBoxButtonString yesString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Yes);
                    MessageBoxButtonString noString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.No);
                    PART_UacStyleDialogView.PrimaryButtonContent = yesString.Text;
                    PART_UacStyleDialogView.PrimaryButtonAccessKey = yesString.Key;
                    PART_UacStyleDialogView.SecondaryButtonContent = noString.Text;
                    PART_UacStyleDialogView.SecondaryButtonAccessKey = noString.Key;
                    break;
                }

            case MessageBoxButtons.YesNoCancel:
                {
                    MessageBoxButtonString yesString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Yes);
                    MessageBoxButtonString noString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.No);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    PART_UacStyleDialogView.PrimaryButtonContent = yesString.Text;
                    PART_UacStyleDialogView.PrimaryButtonAccessKey = yesString.Key;
                    PART_UacStyleDialogView.SecondaryButtonContent = noString.Text;
                    PART_UacStyleDialogView.SecondaryButtonAccessKey = noString.Key;
                    PART_UacStyleDialogView.CloseButtonContent = cancelString.Text;
                    PART_UacStyleDialogView.CloseButtonAccessKey = cancelString.Key;
                    break;
                }

            case MessageBoxButtons.AbortRetryIgnore:
                {
                    MessageBoxButtonString abortString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Abort);
                    MessageBoxButtonString retryString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Retry);
                    MessageBoxButtonString ignoreString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Ignore);
                    PART_UacStyleDialogView.PrimaryButtonContent = abortString.Text;
                    PART_UacStyleDialogView.PrimaryButtonAccessKey = abortString.Key;
                    PART_UacStyleDialogView.SecondaryButtonContent = retryString.Text;
                    PART_UacStyleDialogView.SecondaryButtonAccessKey = retryString.Key;
                    PART_UacStyleDialogView.CloseButtonContent = ignoreString.Text;
                    PART_UacStyleDialogView.CloseButtonAccessKey = ignoreString.Key;
                    break;
                }

            case MessageBoxButtons.RetryCancel:
                {
                    MessageBoxButtonString retryString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Retry);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    PART_UacStyleDialogView.PrimaryButtonContent = retryString.Text;
                    PART_UacStyleDialogView.PrimaryButtonAccessKey = retryString.Key;
                    PART_UacStyleDialogView.SecondaryButtonContent = cancelString.Text;
                    PART_UacStyleDialogView.SecondaryButtonAccessKey = cancelString.Key;
                    break;
                }

            case MessageBoxButtons.CancelTryContinue:
                {
                    MessageBoxButtonString continueString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Continue);
                    MessageBoxButtonString tryString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.TryAgain);
                    MessageBoxButtonString cancelString = MessageBoxButtonString.FromUser32(NativeMessageBoxButtonStringLoader.Cancel);
                    PART_UacStyleDialogView.PrimaryButtonContent = continueString.Text;
                    PART_UacStyleDialogView.PrimaryButtonAccessKey = continueString.Key;
                    PART_UacStyleDialogView.SecondaryButtonContent = tryString.Text;
                    PART_UacStyleDialogView.SecondaryButtonAccessKey = tryString.Key;
                    PART_UacStyleDialogView.CloseButtonContent = cancelString.Text;
                    PART_UacStyleDialogView.CloseButtonAccessKey = cancelString.Key;
                    PART_UacStyleDialogView.DefaultButton = ContentDialogButton.Close;
                    break;
                }
        }
    }

    private readonly struct MessageBoxButtonString
    {
        public MessageBoxButtonString(string text, string key)
        {
            Text = text;
            Key = key;
        }

        public static MessageBoxButtonString FromUser32(string loadedString)
        {
            string text;
            string key;
            int i = loadedString.IndexOf('&');
            if (i == -1)
            {
                text = loadedString;
                key = string.Empty;
            }
            else
            {
                text = loadedString.Remove(i, 1);
                //text = text.Substring(0, text.IndexOf('('));
                key = loadedString[i + 1].ToString();  // For letters, VirtualKey enum value is the same as unicode.
            }
            return new MessageBoxButtonString(text, key);
        }

        public string Text { get; }

        public string Key { get; }
    }
}
