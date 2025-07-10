namespace DevWinUIGallery.Models;
public class SampleData
{
    public SampleData()
    {

    }

    public SampleData(string name, string url, string description)
    {
        Name = name;
        ImageUrl = url;
        Description = description;
    }

    public string Name
    {
        get; set;
    }

    public string ImageUrl
    {
        get; set;
    }

    public string Description
    {
        get; set;
    }

}
