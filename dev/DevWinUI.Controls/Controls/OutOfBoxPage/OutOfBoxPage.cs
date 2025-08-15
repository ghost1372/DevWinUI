using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class OutOfBoxPage : Control
{
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public DataTemplate TitleTemplate
    {
        get { return (DataTemplate)GetValue(TitleTemplateProperty); }
        set { SetValue(TitleTemplateProperty, value); }
    }

    public static readonly DependencyProperty TitleTemplateProperty =
        DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public DataTemplate DescriptionTemplate
    {
        get { return (DataTemplate)GetValue(DescriptionTemplateProperty); }
        set { SetValue(DescriptionTemplateProperty, value); }
    }

    public static readonly DependencyProperty DescriptionTemplateProperty =
        DependencyProperty.Register(nameof(DescriptionTemplate), typeof(DataTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public string HeroImage
    {
        get => (string)GetValue(HeroImageProperty);
        set => SetValue(HeroImageProperty, value);
    }
    public static readonly DependencyProperty HeroImageProperty =
        DependencyProperty.Register(nameof(HeroImage), typeof(string), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public DataTemplate HeroImageTemplate
    {
        get { return (DataTemplate)GetValue(HeroImageTemplateProperty); }
        set { SetValue(HeroImageTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeroImageTemplateProperty =
        DependencyProperty.Register(nameof(HeroImageTemplate), typeof(DataTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

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

    public string SecondaryLinksHeader
    {
        get { return (string)GetValue(SecondaryLinksHeaderProperty); }
        set { SetValue(SecondaryLinksHeaderProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinksHeaderProperty =
        DependencyProperty.Register(nameof(SecondaryLinksHeader), typeof(string), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public DataTemplate SecondaryLinksHeaderTemplate
    {
        get { return (DataTemplate)GetValue(SecondaryLinksHeaderTemplateProperty); }
        set { SetValue(SecondaryLinksHeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinksHeaderTemplateProperty =
        DependencyProperty.Register(nameof(SecondaryLinksHeaderTemplate), typeof(DataTemplate), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public Layout PrimaryLinksLayout
    {
        get { return (Layout)GetValue(PrimaryLinksLayoutProperty); }
        set { SetValue(PrimaryLinksLayoutProperty, value); }
    }

    public static readonly DependencyProperty PrimaryLinksLayoutProperty =
        DependencyProperty.Register(nameof(PrimaryLinksLayout), typeof(Layout), typeof(OutOfBoxPage), new PropertyMetadata(null));

    public Layout SecondaryLinksLayout
    {
        get { return (Layout)GetValue(SecondaryLinksLayoutProperty); }
        set { SetValue(SecondaryLinksLayoutProperty, value); }
    }

    public static readonly DependencyProperty SecondaryLinksLayoutProperty =
        DependencyProperty.Register(nameof(SecondaryLinksLayout), typeof(Layout), typeof(OutOfBoxPage), new PropertyMetadata(null));

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
        UpdateTemplate();
    }
}
