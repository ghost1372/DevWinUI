using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class ShaderBackground
{
    private bool isEdgeFeatheringEnabled = false;
    public bool IsEdgeFeatheringEnabled
    {
        get { return (bool)GetValue(IsEdgeFeatheringEnabledProperty); }
        set { SetValue(IsEdgeFeatheringEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsEdgeFeatheringEnabledProperty =
        DependencyProperty.Register(nameof(IsEdgeFeatheringEnabled), typeof(bool), typeof(ShaderBackground), new PropertyMetadata(false, OnIsEdgeFeatheringEnabledChanged));

    private static void OnIsEdgeFeatheringEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ShaderBackground)d;
        ctl.isEdgeFeatheringEnabled = (bool)e.NewValue;
    }
}
