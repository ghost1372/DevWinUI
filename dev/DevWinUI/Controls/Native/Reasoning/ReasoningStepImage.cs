using Microsoft.UI.Xaml.Automation;

namespace DevWinUI;

public sealed partial class ReasoningStepImage : Control
{
    public Uri? Source
    {
        get { return (Uri?)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(ReasoningStepImage), new PropertyMetadata(null, (d, _) => ((ReasoningStepImage)d).Apply()));

    public string Alt
    {
        get { return (string)GetValue(AltProperty); }
        set { SetValue(AltProperty, value); }
    }

    public static readonly DependencyProperty AltProperty =
        DependencyProperty.Register(nameof(Alt), typeof(string), typeof(ReasoningStepImage), new PropertyMetadata("", (d, e) => AutomationProperties.SetName((ReasoningStepImage)d, (string)e.NewValue)));

    readonly ImageBrush _brush = new() { Stretch = Stretch.UniformToFill };
    Border? _frame;

    public ReasoningStepImage()
    {
        DefaultStyleKey = typeof(ReasoningStepImage);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _frame = GetTemplateChild("PART_Frame") as Border;
        Apply();
    }

    void Apply()
    {
        if (_frame is null) return;

        if (Source is { } uri)
        {
            _brush.ImageSource = new BitmapImage(uri);
            _frame.Background = _brush;
        }
        else
        {
            _brush.ImageSource = null;
            _frame.ClearValue(Border.BackgroundProperty);
        }
    }
}
