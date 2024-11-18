namespace DevWinUI;

public sealed partial class OOBEPageControl : UserControl
{
    public OOBEPageControl()
    {
        InitializeComponent();
        if (string.IsNullOrEmpty(HeroImage))
        {
            HeroImage = "ms-appx:///nothing.png";
        }
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string HeroImage
    {
        get => (string)GetValue(HeroImageProperty);
        set => SetValue(HeroImageProperty, value);
    }

    public double HeroImageHeight
    {
        get => (double)GetValue(HeroImageHeightProperty);
        set => SetValue(HeroImageHeightProperty, value);
    }
    public object PageContent
    {
        get => (object)GetValue(PageContentProperty);
        set => SetValue(PageContentProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SettingsPageControl), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(SettingsPageControl), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty HeroImageProperty = DependencyProperty.Register("HeroImage", typeof(string), typeof(SettingsPageControl), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty PageContentProperty = DependencyProperty.Register("PageContent", typeof(object), typeof(SettingsPageControl), new PropertyMetadata(new Grid()));
    public static readonly DependencyProperty HeroImageHeightProperty = DependencyProperty.Register("HeroImageHeight", typeof(double), typeof(SettingsPageControl), new PropertyMetadata(280.0));
}
