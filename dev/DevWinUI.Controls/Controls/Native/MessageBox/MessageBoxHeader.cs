//https://github.com/SuGar0218/WindowedContentDialog

namespace DevWinUI;
public partial class MessageBoxHeader : Control
{
    public MessageBoxHeader()
    {
        DefaultStyleKey = typeof(MessageBoxHeader);
    }

    public string? Text
    {
        get { return (string?)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(MessageBoxHeader), new PropertyMetadata(null));



    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        DetermineIconState();
    }

    public MessageBoxIcon Icon
    {
        get => (MessageBoxIcon)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(MessageBoxIcon),
        typeof(MessageBoxHeader),
        new PropertyMetadata(MessageBoxIcon.None, (d, e) =>
        {
            MessageBoxHeader self = (MessageBoxHeader)d;
            if (self.IsLoaded)
            {
                self.DetermineIconState();
            }
        })
    );

    private void DetermineIconState()
    {
        switch (Icon)
        {
            case MessageBoxIcon.None:
                VisualStateManager.GoToState(this, "NoIconVisible", false);
                break;
            case MessageBoxIcon.Error:
                VisualStateManager.GoToState(this, "Error", false);
                VisualStateManager.GoToState(this, "StandardIconVisible", false);
                break;
            case MessageBoxIcon.Question:
                VisualStateManager.GoToState(this, "Questional", false);
                VisualStateManager.GoToState(this, "StandardIconVisible", false);
                break;
            case MessageBoxIcon.Warning:
                VisualStateManager.GoToState(this, "Warning", false);
                VisualStateManager.GoToState(this, "StandardIconVisible", false);
                break;
            case MessageBoxIcon.Information:
                VisualStateManager.GoToState(this, "Informational", false);
                VisualStateManager.GoToState(this, "StandardIconVisible", false);
                break;
            case MessageBoxIcon.Success:
                VisualStateManager.GoToState(this, "Success", false);
                VisualStateManager.GoToState(this, "StandardIconVisible", false);
                break;
            default:
                VisualStateManager.GoToState(this, "NoIconVisible", false);
                break;
        }
    }
}
