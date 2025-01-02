using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;
public sealed partial class HoneycombPanelPage : Page
{
    public ObservableCollection<PersonPicture> PictureList;
    public HoneycombPanelPage()
    {
        this.InitializeComponent();
        PictureList = new ObservableCollection<PersonPicture>();
        for (int i = 0; i < 7; i++)
        {
            PictureList.Add(item: new PersonPicture { ProfilePicture = new BitmapImage(new Uri("ms-appx:///Assets/Others/Profile.png")) });
        }
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        PictureList.Add(item: new PersonPicture { ProfilePicture = new BitmapImage(new Uri("ms-appx:///Assets/Others/Profile.png")) });
    }
    private void BtnRemove_Click(object sender, RoutedEventArgs e)
    {
        if (PictureList.Count > 0)
        {
            PictureList.Remove(PictureList.LastOrDefault());
        }
    }
}
