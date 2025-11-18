namespace DevWinUI;

public partial class HaloArcSlider : HaloSlider
{
    public HaloArcSlider()
    {
        DefaultStyleKey = typeof(HaloArcSlider);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        VisualStateManager.GoToState(this, "Resting", false);

        SlideStart += (sender, args) => VisualStateManager.GoToState(this, "Sliding", false);

        SlideStop += (sender, args) => VisualStateManager.GoToState(this, "Resting", false);

        SetValue(Halo.ThicknessProperty, 30.0);
    }
}
