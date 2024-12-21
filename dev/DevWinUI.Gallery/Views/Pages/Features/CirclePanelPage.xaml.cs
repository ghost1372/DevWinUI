namespace DevWinUIGallery.Views;

public sealed partial class CirclePanelPage : Page
{
    private int currentIndex = 0;
    public CirclePanelPage()
    {
        this.InitializeComponent();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        currentIndex += 1;
        CirclePanelSample2.Children.Add(new ClockRadioButton { Content = $"{currentIndex}" });
    }

    private void BtnRemove_Click(object sender, RoutedEventArgs e)
    {
        var item = CirclePanelSample2.Children.LastOrDefault();
        if (item != null)
        {
            CirclePanelSample2.Children.Remove(item);
            currentIndex -= 1;
        }
    }

    private void TGKeepVertical_Toggled(object sender, RoutedEventArgs e)
    {
        UpdateCirclePanelSampleTemplate();
    }

    private void OnValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        UpdateCirclePanelSampleTemplate();
    }

    private void UpdateCirclePanelSampleTemplate()
    {
        CirclePanelSample?.InvalidateArrange();
        CirclePanelSample?.InvalidateMeasure();
    }
    private void UpdateCirclePanelSample2Template()
    {
        CirclePanelSample2?.InvalidateArrange();
        CirclePanelSample2?.InvalidateMeasure();
    }

    private void TGKeepVertical2_Toggled(object sender, RoutedEventArgs e)
    {
        UpdateCirclePanelSample2Template();
    }

    private void OnValueChanged2(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        UpdateCirclePanelSample2Template();
    }
}
