namespace DevWinUIGallery.Views;
public sealed partial class ParticlePage : Page
{
    public ParticlePage()
    {
        this.InitializeComponent();
    }

    private void nbDensity_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (particle != null)
        {
            particle.Density = Convert.ToInt32(args.NewValue);
        }
    }
}
