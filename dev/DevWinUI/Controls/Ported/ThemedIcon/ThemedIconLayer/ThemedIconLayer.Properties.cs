// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class ThemedIconLayer
{
    public static readonly DependencyProperty LayerTypeProperty =
        DependencyProperty.Register(
            nameof(LayerType),
            typeof(ThemedIconLayerType),
            typeof(ThemedIconLayer),
            new PropertyMetadata(ThemedIconLayerType.Base, OnLayerTypeChanged));

    public ThemedIconLayerType LayerType
    {
        get => (ThemedIconLayerType)GetValue(LayerTypeProperty);
        set => SetValue(LayerTypeProperty, value);
    }

    private static void OnLayerTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIconLayer)d).LayerTypeChanged((ThemedIconLayerType)e.NewValue);
    }


    public static readonly DependencyProperty PathDataProperty =
        DependencyProperty.Register(
            nameof(PathData),
            typeof(string),
            typeof(ThemedIconLayer),
            new PropertyMetadata(string.Empty, OnPathDataChanged));

    public string PathData
    {
        get => (string)GetValue(PathDataProperty);
        set => SetValue(PathDataProperty, value);
    }

    private static void OnPathDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIconLayer)d).LayerPathDataChanged((string)e.NewValue);
    }


    public static readonly DependencyProperty LayerSizeProperty =
        DependencyProperty.Register(
            nameof(LayerSize),
            typeof(double),
            typeof(ThemedIconLayer),
            new PropertyMetadata(16.0d, OnLayerSizeChanged));

    public double LayerSize
    {
        get => (double)GetValue(LayerSizeProperty);
        set => SetValue(LayerSizeProperty, value);
    }

    private static void OnLayerSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIconLayer)d).LayerSizePropertyChanged((double)e.NewValue);
    }

    public static readonly DependencyProperty LayerColorProperty =
        DependencyProperty.Register(
            nameof(LayerColor),
            typeof(Brush),
            typeof(ThemedIconLayer),
            new PropertyMetadata(null, OnLayerColorChanged));

    public Brush LayerColor
    {
        get => (Brush)GetValue(LayerColorProperty);
        set => SetValue(LayerColorProperty, value);
    }

    private static void OnLayerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIconLayer)d).IconColorTypeChanged();
    }


    public static readonly DependencyProperty IconColorTypeProperty =
        DependencyProperty.Register(
            nameof(IconColorType),
            typeof(ThemedIconColorType),
            typeof(ThemedIconLayer),
            new PropertyMetadata(ThemedIconColorType.Normal, OnIconColorTypeChanged));

    public ThemedIconColorType IconColorType
    {
        get => (ThemedIconColorType)GetValue(IconColorTypeProperty);
        set => SetValue(IconColorTypeProperty, value);
    }

    private static void OnIconColorTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIconLayer)d).IconColorTypeChanged();
    }
}
