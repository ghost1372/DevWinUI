namespace DevWinUI;

public abstract partial class ItemsPageBase
{
    public IJsonNavigationService JsonNavigationService
    {
        get { return (IJsonNavigationService)GetValue(JsonNavigationServiceProperty); }
        set { SetValue(JsonNavigationServiceProperty, value); }
    }

    public static readonly DependencyProperty JsonNavigationServiceProperty =
        DependencyProperty.Register(nameof(JsonNavigationService), typeof(IJsonNavigationService), typeof(ItemsPageBase), new PropertyMetadata(null));

    public double HeaderImageHeight
    {
        get => (double)GetValue(HeaderImageHeightProperty);
        set => SetValue(HeaderImageHeightProperty, value);
    }

    public static readonly DependencyProperty HeaderImageHeightProperty =
        DependencyProperty.Register(nameof(HeaderImageHeight), typeof(double), typeof(ItemsPageBase), new PropertyMetadata(200.0));

    public string HeaderImage
    {
        get => (string)GetValue(HeaderImageProperty);
        set => SetValue(HeaderImageProperty, value);
    }

    public static readonly DependencyProperty HeaderImageProperty =
        DependencyProperty.Register(nameof(HeaderImage), typeof(string), typeof(ItemsPageBase), new PropertyMetadata(default(string)));

    public VerticalAlignment HeaderVerticalAlignment
    {
        get { return (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty); }
        set { SetValue(HeaderVerticalAlignmentProperty, value); }
    }

    public static readonly DependencyProperty HeaderVerticalAlignmentProperty =
        DependencyProperty.Register(nameof(HeaderVerticalAlignment), typeof(VerticalAlignment), typeof(ItemsPageBase), new PropertyMetadata(VerticalAlignment.Top));

    public CornerRadius HeaderCornerRadius
    {
        get => (CornerRadius)GetValue(HeaderCornerRadiusProperty);
        set => SetValue(HeaderCornerRadiusProperty, value);
    }

    public static readonly DependencyProperty HeaderCornerRadiusProperty =
        DependencyProperty.Register(nameof(HeaderCornerRadius), typeof(CornerRadius), typeof(ItemsPageBase), new PropertyMetadata(new CornerRadius(8, 0, 0, 0)));

    public Thickness HeaderContentMargin
    {
        get { return (Thickness)GetValue(HeaderContentMarginProperty); }
        set { SetValue(HeaderContentMarginProperty, value); }
    }

    public static readonly DependencyProperty HeaderContentMarginProperty =
        DependencyProperty.Register(nameof(HeaderContentMargin), typeof(Thickness), typeof(ItemsPageBase), new PropertyMetadata(new Thickness(36, 100, 0, 0)));

    public double HeaderSubtitleFontSize
    {
        get => (double)GetValue(HeaderSubtitleFontSizeProperty);
        set => SetValue(HeaderSubtitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty HeaderSubtitleFontSizeProperty =
        DependencyProperty.Register(nameof(HeaderSubtitleFontSize), typeof(double), typeof(ItemsPageBase), new PropertyMetadata(18.0));

    public string HeaderSubtitleText
    {
        get => (string)GetValue(HeaderSubtitleTextProperty);
        set => SetValue(HeaderSubtitleTextProperty, value);
    }

    public static readonly DependencyProperty HeaderSubtitleTextProperty =
        DependencyProperty.Register(nameof(HeaderSubtitleText), typeof(string), typeof(ItemsPageBase), new PropertyMetadata(default(string)));

    public double HeaderFontSize
    {
        get => (double)GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    public static readonly DependencyProperty HeaderFontSizeProperty =
        DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(ItemsPageBase), new PropertyMetadata(28.0));

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(ItemsPageBase), new PropertyMetadata("All"));


    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ItemsPageBase), new PropertyMetadata(Stretch.UniformToFill));

    public Thickness GridViewPadding
    {
        get { return (Thickness)GetValue(GridViewPaddingProperty); }
        set { SetValue(GridViewPaddingProperty, value); }
    }

    public static readonly DependencyProperty GridViewPaddingProperty =
        DependencyProperty.Register(nameof(GridViewPadding), typeof(Thickness), typeof(ItemsPageBase), new PropertyMetadata(new Thickness(24, 0, 24, 72)));

    public bool IsTileImage
    {
        get { return (bool)GetValue(IsTileImageProperty); }
        set { SetValue(IsTileImageProperty, value); }
    }

    public static readonly DependencyProperty IsTileImageProperty =
        DependencyProperty.Register(nameof(IsTileImage), typeof(bool), typeof(ItemsPageBase), new PropertyMetadata(false, OnIsTileImageChanged));

    private static void OnIsTileImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ItemsPageBase)d;
        if (ctl != null)
        {
            ctl.OnIsTileImageChanged(e);
        }
    }

    public object FooterContent
    {
        get => (object)GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    public static readonly DependencyProperty FooterContentProperty =
        DependencyProperty.Register(nameof(FooterContent), typeof(object), typeof(ItemsPageBase), new PropertyMetadata(null));

    public Thickness FooterMargin
    {
        get => (Thickness)GetValue(FooterMarginProperty);
        set => SetValue(FooterMarginProperty, value);
    }

    public static readonly DependencyProperty FooterMarginProperty =
        DependencyProperty.Register(nameof(FooterMargin), typeof(Thickness), typeof(ItemsPageBase), new PropertyMetadata(new Thickness(16, 34, 48, 0)));
}
