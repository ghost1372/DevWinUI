namespace DevWinUI;
public partial class HeaderCarouselItem
{
    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(HeaderCarouselItem), new PropertyMetadata(Stretch.UniformToFill));

    public string ImageUrl
    {
        get => (string)GetValue(ImageUrlProperty);
        set => SetValue(ImageUrlProperty, value);
    }

    public static readonly DependencyProperty ImageUrlProperty =
        DependencyProperty.Register(nameof(ImageUrl), typeof(string), typeof(HeaderCarouselItem), new PropertyMetadata(null));

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(string), typeof(HeaderCarouselItem), new PropertyMetadata(defaultValue: null, (d, e) => ((HeaderCarouselItem)d).HeaderChanged((string)e.OldValue, (string)e.NewValue)));
    protected virtual void HeaderChanged(string oldValue, string newValue)
    {
        SetAccessibleName();
    }
    
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(HeaderCarouselItem), new PropertyMetadata(defaultValue: null));

    public string Id
    {
        get => (string)GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }
    public static readonly DependencyProperty IdProperty =
        DependencyProperty.Register(nameof(Id), typeof(string), typeof(HeaderCarouselItem), new PropertyMetadata(defaultValue: string.Empty));

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(HeaderCarouselItem), new PropertyMetadata(defaultValue: false, (d, e) => ((HeaderCarouselItem)d).IsSelectedChanged((bool)e.OldValue, (bool)e.NewValue)));
    protected virtual void IsSelectedChanged(object oldValue, object newValue)
    {
        OnIsSelectedChanged();
    }
}
