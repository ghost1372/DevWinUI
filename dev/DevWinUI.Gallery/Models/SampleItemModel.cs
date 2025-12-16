using System.Xml.Linq;

namespace DevWinUIGallery.Models;

public partial class SampleItemModel : DepthLayerViewItem
{
    private static readonly Random _random = new Random();

    public int Id { get; set; }
    public int IntValue { get; set; }
    public string IntValueString => IntValue.ToString();
    public bool BoolValue { get; set; }
    public double DoubleValue { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }

    public SampleItemModel(string imagePath)
    {
        ImagePath = imagePath;
        ImageUri = new Uri(imagePath);
    }
    public SampleItemModel(int value, string imagePath)
    {
        IntValue = value;
        ImagePath = imagePath;
        ImageUri = new Uri(imagePath);
    }
    public SampleItemModel(string title, string imagePath)
    {
        this.Title = title;
        this.ImagePath = imagePath;
        ImageUri = new Uri(imagePath);
    }

    public SampleItemModel(string title, string description, string imagePath)
    {
        this.Title = title;
        this.Description = description;
        this.ImagePath = imagePath;
        ImageUri = new Uri(imagePath);

        int year = _random.Next(1980, 2027);
        IntValue = year;
    }
    public SampleItemModel(string title, string description, int year, string imagePath)
    {
        this.Title = title;
        this.Description = description;
        this.ImagePath = imagePath;
        ImageUri = new Uri(imagePath);
        this.IntValue = year;
    }

    public override string ToString()
    {
        return Title;
    }
}
