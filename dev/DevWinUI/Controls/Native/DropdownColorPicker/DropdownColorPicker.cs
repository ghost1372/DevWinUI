using Microsoft.UI.Xaml.Shapes;
using System.Runtime.InteropServices;
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
    private bool isRaisingColorChanged;

    public event EventHandler<DropdownColorPickerColorChangedEventArgs> ColorChanged;

    public DropdownColorPicker()
    {
        DefaultStyleKey = typeof(DropdownColorPicker);
        Loaded += OnDropdownColorPickerLoaded;
        Unloaded += OnDropdownColorPickerUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (colorPicker != null)
            colorPicker.ColorChanged -= OnColorPickerColorChanged;
        if (flyout != null)
            flyout.Closed -= OnFlyoutClosed;

        tintBox = GetTemplateChild(PART_Rectangle) as Rectangle;
        colorPicker = GetTemplateChild(PART_ColorPicker) as ColorPicker;
        contentPresenter = GetTemplateChild(PART_ContentPresenter) as ContentPresenter;
        flyout = GetTemplateChild(PART_Flyout) as Flyout;

        if (colorPicker != null)
            colorPicker.ColorChanged += OnColorPickerColorChanged;
        if (flyout != null)
            flyout.Closed += OnFlyoutClosed;

        UpdateColorPalette();
        UpdateFlyoutOptions();
        UpdateColor();
    }

    private void OnDropdownColorPickerLoaded(object sender, RoutedEventArgs e)
    {
        if (colorPicker != null)
        {
            colorPicker.ColorChanged -= OnColorPickerColorChanged;
            colorPicker.ColorChanged += OnColorPickerColorChanged;
        }
        if (ColorPalette != null)
        {
            ColorPalette.ColorChanged -= OnColorPaletteColorChanged;
            ColorPalette.ColorChanged += OnColorPaletteColorChanged;
        }
    }

    private void OnDropdownColorPickerUnloaded(object sender, RoutedEventArgs e)
    {
        if (colorPicker != null)
            colorPicker.ColorChanged -= OnColorPickerColorChanged;
        if (ColorPalette != null)
            ColorPalette.ColorChanged -= OnColorPaletteColorChanged;
    }

    private void OnFlyoutClosed(object sender, object e)
    {
        UpdateFlyoutOptions();
    }

    private void UpdateFlyoutOptions()
    {
        if (flyout == null || flyout.IsOpen)
            return;

        flyout.ShouldConstrainToRootBounds = FlyoutShouldConstrainToRootBounds;
        flyout.LightDismissOverlayMode = FlyoutLightDismissOverlayMode;
        flyout.Placement = FlyoutPlacement;
        flyout.ShowMode = FlyoutShowMode;
    }

    private void UpdateColorPalette()
    {
        if (contentPresenter == null || colorPicker == null)
            return;

        var hasPalette = ColorPalette != null;
        colorPicker.Visibility = hasPalette ? Visibility.Collapsed : Visibility.Visible;
        contentPresenter.Visibility = hasPalette ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnColorPickerColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        if (isRaisingColorChanged)
            return;

        RaiseColorChanged(new DropdownColorPickerColorChangedEventArgs(args.NewColor, args.NewColor.ToString(), null));
    }

    private void OnColorPaletteColorChanged(object sender, ColorPaletteColorChangedEventArgs e)
    {
        if (isRaisingColorChanged)
            return;

        RaiseColorChanged(new DropdownColorPickerColorChangedEventArgs(e.Color, e.ColorName, e.ColorPaletteItem));
    }

    private void RaiseColorChanged(DropdownColorPickerColorChangedEventArgs args)
    {
        isRaisingColorChanged = true;
        try
        {
            Color = args.Color;
        }
        finally
        {
            isRaisingColorChanged = false;
        }

        ColorChanged?.Invoke(this, args);

        if (IsCopyColorCodeOnSelectEnabled)
            CopyToClipboard(args.Color.ToString());
    }

    private static void CopyToClipboard(string text)
    {
        try
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }
        catch (COMException)
        {
        }
    }

    private void UpdateTintBox()
    {
        if (tintBox == null)
            return;

        if (tintBox.Fill is SolidColorBrush brush)
            brush.Color = Color;
        else
            tintBox.Fill = new SolidColorBrush(Color);
    }

    private void UpdateColor()
    {
        UpdateTintBox();

        if (colorPicker != null && !colorPicker.Color.Equals(Color))
            colorPicker.Color = Color;

        if (ColorPalette != null && !Equals(ColorPalette.SelectedColor, Color))
            ColorPalette.SelectedColor = Color;
    }
}
