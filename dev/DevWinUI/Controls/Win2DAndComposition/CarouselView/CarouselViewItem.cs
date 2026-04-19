using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public sealed partial class CarouselViewItem : Control
{
    public ImageSource ImageSource
    {
        get { return (ImageSource)GetValue(ImageSourceProperty); }
        set { SetValue(ImageSourceProperty, value); }
    }

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(ImageSource), typeof(CarouselViewItem), new PropertyMetadata(null));
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(CarouselViewItem), new PropertyMetadata(""));

    public double BlackMaskOpacity
    {
        get { return (double)GetValue(BlackMaskOpacityProperty); }
        set { SetValue(BlackMaskOpacityProperty, value); }
    }

    public static readonly DependencyProperty BlackMaskOpacityProperty =
        DependencyProperty.Register(nameof(BlackMaskOpacity), typeof(double), typeof(CarouselViewItem), new PropertyMetadata(0.00d));

    public ICarouselViewItemSource ItemSource
    {
        get { return (ICarouselViewItemSource)GetValue(ItemSourceProperty); }
        set { SetValue(ItemSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.Register(nameof(ItemSource), typeof(ICarouselViewItemSource), typeof(CarouselViewItem), new PropertyMetadata(null, (s, e) =>
        {
            var item = (s as CarouselViewItem);
            if (item != null)
            {
                var source = e.NewValue as ICarouselViewItemSource;
                if (source != null)
                {
                    item.Title = source.Title;
                    item.ImageSource = new BitmapImage(new Uri(source.ImageSource));
                }
            }
        }));

    public CarouselViewItem()
    {
        this.DefaultStyleKey = typeof(CarouselViewItem);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }
    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        VisualStateManager.GoToState(this, "PointerOver", true);
    }
    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);
        VisualStateManager.GoToState(this, "Pressed", true);

    }
    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        VisualStateManager.GoToState(this, "Normal", true);

    }
    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        base.OnPointerReleased(e);
        VisualStateManager.GoToState(this, "PointerOver", true);
    }
}
