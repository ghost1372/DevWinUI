using Microsoft.UI.Xaml.Automation;

namespace DevWinUI;

public sealed partial class LoadingState
{
    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(LoadingState), new PropertyMetadata("Working", OnLabelChanged));

    public LoadingStateMode Mode
    {
        get { return (LoadingStateMode)GetValue(VariantProperty); }
        set { SetValue(VariantProperty, value); }
    }

    public static readonly DependencyProperty VariantProperty =
        DependencyProperty.Register(nameof(Mode), typeof(LoadingStateMode), typeof(LoadingState), new PropertyMetadata(LoadingStateMode.Drive, OnVariantChanged));

    public bool ShowElapsed
    {
        get { return (bool)GetValue(ShowElapsedProperty); }
        set { SetValue(ShowElapsedProperty, value); }
    }

    public static readonly DependencyProperty ShowElapsedProperty =
        DependencyProperty.Register(nameof(ShowElapsed), typeof(bool), typeof(LoadingState), new PropertyMetadata(true, OnShowElapsedChanged));

    private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (LoadingState)d;
        string label = e.NewValue as string ?? string.Empty;
        if (control._label is not null) control._label.Text = label;
        AutomationProperties.SetName(control, label);
    }

    private static void OnVariantChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (LoadingState)d;
        control.BuildGrid();
        if (control._loaded) control.ApplyMotion();
    }

    private static void OnShowElapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (LoadingState)d;
        if (control._timerText is not null)
        {
            control._timerText.Visibility = control.ShowElapsed ? Visibility.Visible : Visibility.Collapsed;
        }
        control.UpdateTimer();
    }
}
