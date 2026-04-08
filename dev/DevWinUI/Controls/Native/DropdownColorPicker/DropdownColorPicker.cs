using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.DataTransfer;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Rectangle), Type = typeof(Rectangle))]
[TemplatePart(Name = nameof(PART_ColorPicker), Type = typeof(ColorPicker))]
[TemplatePart(Name = nameof(PART_ContentPresenter), Type = typeof(ContentPresenter))]
[TemplatePart(Name = nameof(PART_Flyout), Type = typeof(Flyout))]
public partial class DropdownColorPicker : Control
{
    private const string PART_Rectangle = "PART_Rectangle";
    private const string PART_ColorPicker = "PART_ColorPicker";
    private const string PART_ContentPresenter = "PART_ContentPresenter";
    private const string PART_Flyout = "PART_Flyout";

    private Rectangle tintBox;
    private ColorPicker colorPicker;
    private ContentPresenter contentPresenter;
    private Flyout flyout;

    public event EventHandler<DropdownColorPickerColorChangedEventArgs> ColorChanged;

    public DropdownColorPicker()
    {
        DefaultStyleKey = typeof(DropdownColorPicker);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        tintBox = GetTemplateChild(PART_Rectangle) as Rectangle;
        colorPicker = GetTemplateChild(PART_ColorPicker) as ColorPicker;
        contentPresenter = GetTemplateChild(PART_ContentPresenter) as ContentPresenter;
        flyout = GetTemplateChild(PART_Flyout) as Flyout;

        colorPicker.ColorChanged -= OnColorPickerColorChanged;
        colorPicker.ColorChanged += OnColorPickerColorChanged;

        UpdateColorPalette();
        UpdateFlyoutOptions();
        OnColorChanged();

        Unloaded -= OnDropdownColorPickerUnlaoded;
        Unloaded += OnDropdownColorPickerUnlaoded;
    }

    private void OnDropdownColorPickerUnlaoded(object sender, RoutedEventArgs e)
    {
        if (colorPicker != null)
            colorPicker.ColorChanged -= OnColorPickerColorChanged;
        if (ColorPalette != null)
            ColorPalette.ColorChanged -= OnColorPaletteColorChanged;
    }

    private void UpdateFlyoutOptions()
    {
        if (flyout == null)
            return;

        if (!flyout.IsOpen)
        {
            flyout.ShouldConstrainToRootBounds = FlyoutShouldConstrainToRootBounds;
            flyout.LightDismissOverlayMode = FlyoutLightDismissOverlayMode;
            flyout.Placement = FlyoutPlacement;
            flyout.ShowMode = FlyoutShowMode;
        }
    }

    private void UpdateColorPalette()
    {
        if (contentPresenter == null || colorPicker == null)
            return;

        if (ColorPalette == null)
        {
            colorPicker.Visibility = Visibility.Visible;
            contentPresenter.Visibility = Visibility.Collapsed;
        }
        else
        {
            contentPresenter.Visibility = Visibility.Visible;
            colorPicker.Visibility = Visibility.Collapsed;
        }
    }

    private void OnColorPickerColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        RaiseColorChanged(new DropdownColorPickerColorChangedEventArgs(args.NewColor, args.ToString(), null));
    }

    private void OnColorPaletteColorChanged(object sender, ColorPaletteColorChangedEventArgs e)
    {
        RaiseColorChanged(new DropdownColorPickerColorChangedEventArgs(e.Color, e.ColorName, e.ColorPaletteItem));
    }

    private void RaiseColorChanged(DropdownColorPickerColorChangedEventArgs args)
    {
        Color = args.Color;
        ColorChanged?.Invoke(this, args);

        if (IsCopyColorCodeOnSelectEnabled)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(args.Color.ToString());
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }

        if (tintBox == null)
            return;

        tintBox.Fill = new SolidColorBrush(args.Color);
    }

    private void OnColorChanged()
    {
        if (colorPicker != null && !colorPicker.Color.Equals(Color))
        {
            colorPicker.Color = Color;
        }

        if (ColorPalette != null && !Equals(ColorPalette.SelectedColor, Color))
        {
            ColorPalette.SelectedColor = Color;
        }
    }
}
