using DevWinUI;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;

public sealed partial class CompareSliderPage : Page
{
    public CompareSliderPage()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var currentTarget = CompareSlider.TargetImage as BitmapImage;

        if (currentTarget.UriSource.ToString().Contains("GirlBlur.jpg"))
        {
            var before = new BitmapImage(new Uri("ms-appx:///Assets/Others/GirlBefore.png"));
            var after = new BitmapImage(new Uri("ms-appx:///Assets/Others/GirlAfter.png"));
            CompareSlider.SourceImage = before;
            CompareSlider.TargetImage = after;
            CompareSlider2.SourceImage = before;
            CompareSlider2.TargetImage = after;
        }
        else 
        {
            var before = new BitmapImage(new Uri("ms-appx:///Assets/Others/Girl.jpg"));
            var after = new BitmapImage(new Uri("ms-appx:///Assets/Others/GirlBlur.jpg"));
            CompareSlider.SourceImage = before;
            CompareSlider.TargetImage = after;
            CompareSlider2.SourceImage = before;
            CompareSlider2.TargetImage = after;
        }
    }
}
