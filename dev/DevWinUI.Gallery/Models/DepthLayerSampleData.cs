namespace DevWinUIGallery.Models;
public partial class DepthLayerSampleData : DepthLayerViewItem
{
    public int Year { get; set; }
    public string YearLabel => Year.ToString();

    public DepthLayerSampleData(int year, string imageUri)
    {
        this.Year = year;
        this.ImageUri = new Uri(imageUri);
    }
}
