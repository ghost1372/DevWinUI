// Copyright (c) Files Community
// Licensed under the MIT License.

namespace DevWinUI;

public partial class ThemedIcon : Control
{
    public static readonly DependencyProperty FilledIconDataProperty =
            DependencyProperty.Register(
                nameof(FilledIconData),
                typeof(string),
                typeof(ThemedIcon),
                new PropertyMetadata(null, OnFilledIconDataChanged));

    public string FilledIconData
    {
        get => (string)GetValue(FilledIconDataProperty);
        set => SetValue(FilledIconDataProperty, value);
    }

    private static void OnFilledIconDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).OnFilledIconChanged();
    }


    public static readonly DependencyProperty OutlineIconDataProperty =
        DependencyProperty.Register(
            nameof(OutlineIconData),
            typeof(string),
            typeof(ThemedIcon),
            new PropertyMetadata(null, OnOutlineIconDataChanged));

    public string OutlineIconData
    {
        get => (string)GetValue(OutlineIconDataProperty);
        set => SetValue(OutlineIconDataProperty, value);
    }

    private static void OnOutlineIconDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).OnOutlineIconChanged();
    }


    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(
            nameof(Color),
            typeof(Brush),
            typeof(ThemedIcon),
            new PropertyMetadata(null, OnColorChanged));

    public Brush Color
    {
        get => (Brush)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ThemedIcon)d;
        control.OnIconTypeChanged();
        control.OnIconColorChanged();
    }


    public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register(
                nameof(IconType),
                typeof(ThemedIconTypes),
                typeof(ThemedIcon),
                new PropertyMetadata(ThemedIconTypes.Layered, OnIconTypeChanged));

    public ThemedIconTypes IconType
    {
        get => (ThemedIconTypes)GetValue(IconTypeProperty);
        set => SetValue(IconTypeProperty, value);
    }

    private static void OnIconTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).OnIconTypeChanged();
    }


    public static readonly DependencyProperty IconColorTypeProperty =
        DependencyProperty.Register(
            nameof(IconColorType),
            typeof(ThemedIconColorType),
            typeof(ThemedIcon),
            new PropertyMetadata(ThemedIconColorType.None, OnIconColorTypeChanged));

    public ThemedIconColorType IconColorType
    {
        get => (ThemedIconColorType)GetValue(IconColorTypeProperty);
        set => SetValue(IconColorTypeProperty, value);
    }

    private static void OnIconColorTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).OnIconColorTypeChanged();
    }


    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(
            nameof(IconSize),
            typeof(double),
            typeof(ThemedIcon),
            new PropertyMetadata(16.0d, OnIconSizeChanged));

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    private static void OnIconSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ThemedIcon)d;
        control.UpdateVisualStates();
        control.OnIconSizeChanged();
    }


    public static readonly DependencyProperty IsToggledProperty =
        DependencyProperty.Register(
            nameof(IsToggled),
            typeof(bool),
            typeof(ThemedIcon),
            new PropertyMetadata(false, OnIsToggledChanged));

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    private static void OnIsToggledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).UpdateVisualStates();
    }


    public static readonly DependencyProperty IsFilledProperty =
        DependencyProperty.Register(
            nameof(IsFilled),
            typeof(bool),
            typeof(ThemedIcon),
            new PropertyMetadata(false, OnIsFilledChanged));

    public bool IsFilled
    {
        get => (bool)GetValue(IsFilledProperty);
        set => SetValue(IsFilledProperty, value);
    }

    private static void OnIsFilledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).UpdateVisualStates();
    }

    public static readonly DependencyProperty IsHighContrastProperty =
        DependencyProperty.Register(
            nameof(IsHighContrast),
            typeof(bool),
            typeof(ThemedIcon),
            new PropertyMetadata(false, OnIsHighContrastChanged));

    public bool IsHighContrast
    {
        get => (bool)GetValue(IsHighContrastProperty);
        set => SetValue(IsHighContrastProperty, value);
    }

    private static void OnIsHighContrastChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).UpdateVisualStates();
    }


    public static readonly DependencyProperty LayersProperty =
        DependencyProperty.Register(
            nameof(Layers),
            typeof(object),
            typeof(ThemedIcon),
            new PropertyMetadata(null, OnLayersChanged));

    public object Layers
    {
        get => GetValue(LayersProperty);
        set => SetValue(LayersProperty, value);
    }

    private static void OnLayersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).UpdateVisualStates();
    }


    public static readonly DependencyProperty ToggleBehaviorProperty =
        DependencyProperty.Register(
            nameof(ToggleBehavior),
            typeof(ToggleBehaviors),
            typeof(ThemedIcon),
            new PropertyMetadata(ToggleBehaviors.Auto, OnToggleBehaviorChanged));

    public ToggleBehaviors ToggleBehavior
    {
        get => (ToggleBehaviors)GetValue(ToggleBehaviorProperty);
        set => SetValue(ToggleBehaviorProperty, value);
    }

    private static void OnToggleBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ThemedIcon)d).UpdateVisualStates();
    }


    private void OnStylePropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (dp != StyleProperty)
            return;

        DispatcherQueue.TryEnqueue(() =>
        {
            GetTemplateParts();
            OnFilledIconChanged();
            OnOutlineIconChanged();
            OnLayeredIconChanged();
            OnIconTypeChanged();
            OnIconColorTypeChanged();
            OnIconSizeChanged();
        });
    }
}
