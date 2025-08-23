using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_HeroImageImage), Type = typeof(Image))]
[TemplatePart(Name = nameof(PART_HeroImageBorder), Type = typeof(Border))]
[TemplatePart(Name = nameof(PART_SecondaryLinksHeaderTextBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_DescriptionTextBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_TitleTextBlock), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_SecondaryLinksHeaderPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_DescriptionPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_HeroImagePresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_TitlePresenter), Type = typeof(ContentPresenter))]
[ContentProperty(Name = nameof(Content))]
public partial class OutOfBoxPage : Control
{
    private const string PART_TitlePresenter = "PART_TitlePresenter";
    private const string PART_TitleTextBlock = "PART_TitleTextBlock";
    private const string PART_HeroImagePresenter = "PART_HeroImagePresenter";
    private const string PART_HeroImageBorder = "PART_HeroImageBorder";
    private const string PART_HeroImageImage = "PART_HeroImageImage";
    private const string PART_DescriptionPresenter = "PART_DescriptionPresenter";
    private const string PART_DescriptionTextBlock = "PART_DescriptionTextBlock";
    private const string PART_SecondaryLinksHeaderPresenter = "PART_SecondaryLinksHeaderPresenter";
    private const string PART_SecondaryLinksHeaderTextBlock = "PART_SecondaryLinksHeaderTextBlock";

    private ContentPresenter titlePresenter;
    private TextBlock titleTextBlock;
    private ContentPresenter descriptionPresenter;
    private TextBlock descriptionTextBlock;
    private ContentPresenter heroImagePresenter;
    private Border heroImageBorder;
    private Image heroImageImage;
    private ContentPresenter secondaryLinksHeaderPresenter;
    private TextBlock secondaryLinksHeaderTextBlock;
    public object Title
    {
        get { return (object)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(object), typeof(OutOfBoxPage), new PropertyMetadata(null, OnTitleChanged));

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OutOfBoxPage)d;
        if (ctl != null)
        {
            if (e.NewValue == null)
            {
                ctl.titlePresenter.Visibility = Visibility.Collapsed;
                ctl.titleTextBlock.Visibility = Visibility.Collapsed;
            }
            else if (e.NewValue is string)
            {
                ctl.titlePresenter.Visibility = Visibility.Collapsed;
                ctl.titleTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ctl.titlePresenter.Visibility = Visibility.Visible;
                ctl.titleTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }

    public object Description
    {
        get => (object)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(object), typeof(OutOfBoxPage), new PropertyMetadata(null, OnDescriptionChanged));

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OutOfBoxPage)d;
        if (ctl != null)
        {
            if (e.NewValue == null)
            {
                ctl.descriptionPresenter.Visibility = Visibility.Collapsed;
                ctl.descriptionTextBlock.Visibility = Visibility.Collapsed;
            }
            else if (e.NewValue is string)
            {
                ctl.descriptionPresenter.Visibility = Visibility.Collapsed;
                ctl.descriptionTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ctl.descriptionPresenter.Visibility = Visibility.Visible;
                ctl.descriptionTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }
    public object HeroImage
    {
        get => (object)GetValue(HeroImageProperty);
        set => SetValue(HeroImageProperty, value);
    }
    public static readonly DependencyProperty HeroImageProperty =
        DependencyProperty.Register(nameof(HeroImage), typeof(object), typeof(OutOfBoxPage), new PropertyMetadata(null, OnHeroImageChanged));
    private static void OnHeroImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OutOfBoxPage)d;
        if (ctl != null)
        {
            if (e.NewValue == null)
            {
                ctl.heroImagePresenter.Visibility = Visibility.Collapsed;
                if (ctl.UseTopHeroImage)
                {
                    ctl.heroImageImage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ctl.heroImageBorder.Visibility = Visibility.Collapsed;
                }
            }
            else if (e.NewValue is string || e.NewValue is Uri)
            {
                ctl.heroImagePresenter.Visibility = Visibility.Collapsed;
                if (ctl.UseTopHeroImage)
                {
                    ctl.heroImageImage.Visibility = Visibility.Visible;
                }
                else
                {
                    ctl.heroImageBorder.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ctl.heroImagePresenter.Visibility = Visibility.Visible;
                if (ctl.UseTopHeroImage)
                {
                    ctl.heroImageImage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ctl.heroImageBorder.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
    public double HeroImageHeight
    {
        get { return (double)GetValue(HeroImageHeightProperty); }
        set { SetValue(HeroImageHeightProperty, value); }
    }
    public static readonly DependencyProperty HeroImageHeightProperty =
        DependencyProperty.Register(nameof(HeroImageHeight), typeof(double), typeof(OutOfBoxPage), new PropertyMetadata(280.0));

    public object ActionContent
    {
        get { return (object)GetValue(ActionContentProperty); }
        set { SetValue(ActionContentProperty, value); }
    }

    public static readonly DependencyProperty ActionContentProperty =
        DependencyProperty.Register(nameof(ActionContent), typeof(object), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public object Content
    {
        get { return (object)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(OutOfBoxPage), new PropertyMetadata(new Grid()));

    public ObservableCollection<OutOfBoxPageLink> PrimaryLinks
    {
        get { return (ObservableCollection<OutOfBoxPageLink>)GetValue(PrimaryLinksProperty); }
        set { SetValue(PrimaryLinksProperty, value); }
    }

    public static readonly DependencyProperty PrimaryLinksProperty =
        DependencyProperty.Register(nameof(PrimaryLinks), typeof(ObservableCollection<OutOfBoxPageLink>), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public ObservableCollection<OutOfBoxPageLink> SecondaryLinks
    {
        get { return (ObservableCollection<OutOfBoxPageLink>)GetValue(SecondaryLinksProperty); }
        set { SetValue(SecondaryLinksProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinksProperty =
        DependencyProperty.Register(nameof(SecondaryLinks), typeof(ObservableCollection<OutOfBoxPageLink>), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public object SecondaryLinksHeader
    {
        get { return (object)GetValue(SecondaryLinksHeaderProperty); }
        set { SetValue(SecondaryLinksHeaderProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinksHeaderProperty =
        DependencyProperty.Register(nameof(SecondaryLinksHeader), typeof(object), typeof(OutOfBoxPage), new PropertyMetadata(null, OnSecondaryLinksHeaderChanged));
    private static void OnSecondaryLinksHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OutOfBoxPage)d;
        if (ctl != null)
        {
            if (e.NewValue == null)
            {
                ctl.secondaryLinksHeaderPresenter.Visibility = Visibility.Collapsed;
                ctl.secondaryLinksHeaderTextBlock.Visibility = Visibility.Collapsed;
            }
            else if (e.NewValue is string)
            {
                ctl.secondaryLinksHeaderPresenter.Visibility = Visibility.Collapsed;
                ctl.secondaryLinksHeaderTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                ctl.secondaryLinksHeaderPresenter.Visibility = Visibility.Visible;
                ctl.secondaryLinksHeaderTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }
    public ItemsPanelTemplate PrimaryLinkItemsPanelTemplate
    {
        get { return (ItemsPanelTemplate)GetValue(PrimaryLinkItemsPanelTemplateProperty); }
        set { SetValue(PrimaryLinkItemsPanelTemplateProperty, value); }
    }

    public static readonly DependencyProperty PrimaryLinkItemsPanelTemplateProperty =
        DependencyProperty.Register(nameof(PrimaryLinkItemsPanelTemplate), typeof(ItemsPanelTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public ItemsPanelTemplate SecondaryLinkItemsPanelTemplate
    {
        get { return (ItemsPanelTemplate)GetValue(SecondaryLinkItemsPanelTemplateProperty); }
        set { SetValue(SecondaryLinkItemsPanelTemplateProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinkItemsPanelTemplateProperty =
        DependencyProperty.Register(nameof(SecondaryLinkItemsPanelTemplate), typeof(ItemsPanelTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public Thickness SecondaryLinksMargin
    {
        get { return (Thickness)GetValue(SecondaryLinksMarginProperty); }
        set { SetValue(SecondaryLinksMarginProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinksMarginProperty =
        DependencyProperty.Register(nameof(SecondaryLinksMargin), typeof(Thickness), typeof(OutOfBoxPage), new PropertyMetadata(new Thickness()));

    public Thickness ContentMargin
    {
        get { return (Thickness)GetValue(ContentMarginProperty); }
        set { SetValue(ContentMarginProperty, value); }
    }

    public static readonly DependencyProperty ContentMarginProperty =
        DependencyProperty.Register(nameof(ContentMargin), typeof(Thickness), typeof(OutOfBoxPage), new PropertyMetadata(new Thickness()));

    public DataTemplate PrimaryLinksItemTemplate
    {
        get { return (DataTemplate)GetValue(PrimaryLinksItemTemplateProperty); }
        set { SetValue(PrimaryLinksItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty PrimaryLinksItemTemplateProperty =
        DependencyProperty.Register(nameof(PrimaryLinksItemTemplate), typeof(DataTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public DataTemplate SecondaryLinksItemTemplate
    {
        get { return (DataTemplate)GetValue(SecondaryLinksItemTemplateProperty); }
        set { SetValue(SecondaryLinksItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinksItemTemplateProperty =
        DependencyProperty.Register(nameof(SecondaryLinksItemTemplate), typeof(DataTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public bool UseTopHeroImage
    {
        get { return (bool)GetValue(UseTopHeroImageProperty); }
        set { SetValue(UseTopHeroImageProperty, value); }
    }

    public static readonly DependencyProperty UseTopHeroImageProperty =
        DependencyProperty.Register(nameof(UseTopHeroImage), typeof(bool), typeof(OutOfBoxPage), new PropertyMetadata(false, OnUseTopHeroImageChanged));

    private static void OnUseTopHeroImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OutOfBoxPage)d;
        if (ctl != null)
        {
            ctl.UpdateTemplate();
        }
    }
    private ControlTemplate? TopHeroImageTemplate { get; set; }
    private ControlTemplate? LeftHeroImageTemplate { get; set; }

    private void UpdateTemplate()
    {
        Template = UseTopHeroImage ? TopHeroImageTemplate : LeftHeroImageTemplate;
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ControlTemplate))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(OutOfBoxPage))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DataTemplate))]
    public OutOfBoxPage()
    {
        DefaultStyleKey = typeof(OutOfBoxPage);

        if (Application.Current.Resources["OutOfBoxTopHeroImageTemplate"] is ControlTemplate topHeroImageTemplate)
            this.TopHeroImageTemplate = topHeroImageTemplate;

        if (Application.Current.Resources["OutOfBoxLeftHeroImageTemplate"] is ControlTemplate leftHeroImageTemplate)
            this.LeftHeroImageTemplate = leftHeroImageTemplate;

        Application.Current.Resources.MergedDictionaries.AddIfNotExists(new OutOfBoxItemTemplate());

        if (PrimaryLinks == null)
        {
            PrimaryLinks = new ObservableCollection<OutOfBoxPageLink>();
        }

        if (SecondaryLinks == null)
        {
            SecondaryLinks = new ObservableCollection<OutOfBoxPageLink>();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        titlePresenter = GetTemplateChild(PART_TitlePresenter) as ContentPresenter;
        titleTextBlock = GetTemplateChild(PART_TitleTextBlock) as TextBlock;
        descriptionPresenter = GetTemplateChild(PART_DescriptionPresenter) as ContentPresenter;
        descriptionTextBlock = GetTemplateChild(PART_DescriptionTextBlock) as TextBlock;
        heroImagePresenter = GetTemplateChild(PART_HeroImagePresenter) as ContentPresenter;
        heroImageBorder = GetTemplateChild(PART_HeroImageBorder) as Border;
        heroImageImage = GetTemplateChild(PART_HeroImageImage) as Image;
        secondaryLinksHeaderPresenter = GetTemplateChild(PART_SecondaryLinksHeaderPresenter) as ContentPresenter;
        secondaryLinksHeaderTextBlock = GetTemplateChild(PART_SecondaryLinksHeaderTextBlock) as TextBlock;

        UpdateTemplate();
    }
}
