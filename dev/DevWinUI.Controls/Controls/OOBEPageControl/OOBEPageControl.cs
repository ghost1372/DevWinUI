namespace DevWinUI;
public partial class OOBEPageControl : Control
{
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(OOBEPageControl), new PropertyMetadata(default(string)));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(OOBEPageControl), new PropertyMetadata(default(string)));

    public string HeroImage
    {
        get => (string)GetValue(HeroImageProperty);
        set => SetValue(HeroImageProperty, value);
    }
    public static readonly DependencyProperty HeroImageProperty =
        DependencyProperty.Register(nameof(HeroImage), typeof(string), typeof(OOBEPageControl), new PropertyMetadata(default(string)));

    public double HeroImageHeight
    {
        get { return (double)GetValue(HeroImageHeightProperty); }
        set { SetValue(HeroImageHeightProperty, value); }
    }
    public static readonly DependencyProperty HeroImageHeightProperty =
        DependencyProperty.Register(nameof(HeroImageHeight), typeof(double), typeof(OOBEPageControl), new PropertyMetadata(280.0));

    public object PageContent
    {
        get { return (object)GetValue(PageContentProperty); }
        set { SetValue(PageContentProperty, value); }
    }
    public static readonly DependencyProperty PageContentProperty =
        DependencyProperty.Register(nameof(PageContent), typeof(object), typeof(OOBEPageControl), new PropertyMetadata(new Grid()));

    public OOBEPageControl()
    {
        DefaultStyleKey = typeof(OOBEPageControl);

        if (string.IsNullOrEmpty(HeroImage))
        {
            HeroImage = "ms-appx:///nothing.png";
        }
    }
}
