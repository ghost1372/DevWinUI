
namespace DevWinUI;
public partial class OptionsPageControl : Control
{
    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(OptionsPageControl), new PropertyMetadata(null, OnHeaderTextChanged));

    public static readonly DependencyProperty MainCardCornerRadiusProperty =
          DependencyProperty.Register(nameof(MainCardCornerRadius), typeof(CornerRadius), typeof(OptionsPageControl), new PropertyMetadata(new CornerRadius(8)));

    public static readonly DependencyProperty MainCardBorderThicknessProperty =
        DependencyProperty.Register(nameof(MainCardBorderThickness), typeof(Thickness), typeof(OptionsPageControl), new PropertyMetadata(new Thickness(1, 1, 1, 1)));

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(OptionsPageControl), new PropertyMetadata(null));

    public static readonly DependencyProperty FooterProperty =
        DependencyProperty.Register(nameof(Footer), typeof(object), typeof(OptionsPageControl), new PropertyMetadata(null));

    public static readonly DependencyProperty IsFooterEnabledProperty =
        DependencyProperty.Register(nameof(IsFooterEnabled), typeof(bool), typeof(OptionsPageControl), new PropertyMetadata(true));

    public static readonly DependencyProperty IsFooterExpandedProperty =
       DependencyProperty.Register(nameof(IsFooterExpanded), typeof(bool), typeof(OptionsPageControl), new PropertyMetadata(false, OnIsFooterExpanderChanged));

    public static readonly DependencyProperty FooterVisibilityProperty =
        DependencyProperty.Register(nameof(FooterVisibility), typeof(Visibility), typeof(OptionsPageControl), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty FooterHeaderProperty =
        DependencyProperty.Register(nameof(FooterHeader), typeof(object), typeof(OptionsPageControl), new PropertyMetadata(null));

    public static readonly DependencyProperty FooterMaxHeightProperty =
        DependencyProperty.Register(nameof(FooterMaxHeight), typeof(double), typeof(OptionsPageControl), new PropertyMetadata(400.0));

    public static readonly DependencyProperty FooterHorizontalAlignmentProperty =
        DependencyProperty.Register(nameof(FooterHorizontalAlignment), typeof(HorizontalAlignment), typeof(OptionsPageControl), new PropertyMetadata(HorizontalAlignment.Left));

    public static readonly DependencyProperty FooterContentMarginProperty =
        DependencyProperty.Register(nameof(FooterContentMargin), typeof(Thickness), typeof(OptionsPageControl), new PropertyMetadata(new Thickness(16)));

    public static readonly DependencyProperty PaneProperty =
        DependencyProperty.Register(nameof(Pane), typeof(object), typeof(OptionsPageControl), new PropertyMetadata(null, OnPaneChanged));

    public static readonly DependencyProperty PaneMinWidthProperty =
       DependencyProperty.Register(nameof(PaneMinWidth), typeof(double), typeof(OptionsPageControl), new PropertyMetadata(286.0));

    public static readonly DependencyProperty PaneMaxWidthProperty =
       DependencyProperty.Register(nameof(PaneMaxWidth), typeof(double), typeof(OptionsPageControl), new PropertyMetadata(320.0));

    public static readonly DependencyProperty PanePaddingProperty =
        DependencyProperty.Register(nameof(PanePadding), typeof(Thickness), typeof(OptionsPageControl), new PropertyMetadata(new Thickness(16)));

    public static readonly DependencyProperty PaneVisibilityProperty =
        DependencyProperty.Register(nameof(PaneVisibility), typeof(Visibility), typeof(OptionsPageControl), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty BarProperty =
       DependencyProperty.Register(nameof(Bar), typeof(object), typeof(OptionsPageControl), new PropertyMetadata(null));

    public static readonly DependencyProperty BarFooterProperty =
       DependencyProperty.Register(nameof(BarFooter), typeof(object), typeof(OptionsPageControl), new PropertyMetadata(null));

    public static readonly DependencyProperty BarHorizontalContentAlignmentProperty =
       DependencyProperty.Register(nameof(BarHorizontalContentAlignment), typeof(HorizontalAlignment), typeof(OptionsPageControl), new PropertyMetadata(HorizontalAlignment.Center));

    public static readonly DependencyProperty BarMinWidthProperty =
        DependencyProperty.Register(nameof(BarMinWidth), typeof(double), typeof(OptionsPageControl), new PropertyMetadata(32.0));

    public static readonly DependencyProperty IsBarEnabledProperty =
        DependencyProperty.Register(nameof(IsBarEnabled), typeof(bool), typeof(OptionsPageControl), new PropertyMetadata(true));

    public string HeaderText
    {
        get { return (string)GetValue(HeaderTextProperty); }
        set { SetValue(HeaderTextProperty, value); }
    }

    public CornerRadius MainCardCornerRadius
    {
        get { return (CornerRadius)GetValue(MainCardCornerRadiusProperty); }
        set { SetValue(MainCardCornerRadiusProperty, value); }
    }

    public Thickness MainCardBorderThickness
    {
        get { return (Thickness)GetValue(MainCardBorderThicknessProperty); }
        set { SetValue(MainCardBorderThicknessProperty, value); }
    }

    public object? Content
    {
        get => (object?)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public object? Footer
    {
        get => (object?)GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public bool IsFooterExpanded
    {
        get { return (bool)GetValue(IsFooterExpandedProperty); }
        set { SetValue(IsFooterExpandedProperty, value); }
    }

    public bool IsFooterEnabled
    {
        get { return (bool)GetValue(IsFooterEnabledProperty); }
        set { SetValue(IsFooterEnabledProperty, value); }
    }
    public Visibility FooterVisibility
    {
        get { return (Visibility)GetValue(FooterVisibilityProperty); }
        set { SetValue(FooterVisibilityProperty, value); }
    }

    public object FooterHeader
    {
        get { return (object)GetValue(FooterHeaderProperty); }
        set { SetValue(FooterHeaderProperty, value); }
    }

    public double FooterMaxHeight
    {
        get { return (double)GetValue(FooterMaxHeightProperty); }
        set { SetValue(FooterMaxHeightProperty, value); }
    }
    public HorizontalAlignment FooterHorizontalAlignment
    {
        get { return (HorizontalAlignment)GetValue(FooterHorizontalAlignmentProperty); }
        set { SetValue(FooterHorizontalAlignmentProperty, value); }
    }

    public Thickness FooterContentMargin
    {
        get { return (Thickness)GetValue(FooterContentMarginProperty); }
        set { SetValue(FooterContentMarginProperty, value); }
    }

    public object? Pane
    {
        get => (object?)GetValue(PaneProperty);
        set => SetValue(PaneProperty, value);
    }

    public double PaneMinWidth
    {
        get { return (double)GetValue(PaneMinWidthProperty); }
        set { SetValue(PaneMinWidthProperty, value); }
    }

    public double PaneMaxWidth
    {
        get { return (double)GetValue(PaneMaxWidthProperty); }
        set { SetValue(PaneMaxWidthProperty, value); }
    }

    public Thickness PanePadding
    {
        get { return (Thickness)GetValue(PanePaddingProperty); }
        set { SetValue(PanePaddingProperty, value); }
    }

    public Visibility PaneVisibility
    {
        get { return (Visibility)GetValue(PaneVisibilityProperty); }
        set { SetValue(PaneVisibilityProperty, value); }
    }

    public object? Bar
    {
        get { return (object?)GetValue(BarProperty); }
        set { SetValue(BarProperty, value); }
    }

    public object? BarFooter
    {
        get { return (object?)GetValue(BarFooterProperty); }
        set { SetValue(BarFooterProperty, value); }
    }

    public HorizontalAlignment BarHorizontalContentAlignment
    {
        get { return (HorizontalAlignment)GetValue(BarHorizontalContentAlignmentProperty); }
        set { SetValue(BarHorizontalContentAlignmentProperty, value); }
    }

    public double BarMinWidth
    {
        get { return (double)GetValue(BarMinWidthProperty); }
        set { SetValue(BarMinWidthProperty, value); }
    }

    public bool IsBarEnabled
    {
        get { return (bool)GetValue(IsBarEnabledProperty); }
        set { SetValue(IsBarEnabledProperty, value); }
    }
}
