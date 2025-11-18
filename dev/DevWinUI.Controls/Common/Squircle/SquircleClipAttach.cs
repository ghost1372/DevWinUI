//https://github.com/cnbluefire/Squircle.Windows

namespace DevWinUI;

public static partial class SquircleClipAttach
{
    private readonly static object BoxedCornerRadius = new Microsoft.UI.Xaml.CornerRadius(0);
    private readonly static object BoxedDouble = 0d;

    public static Microsoft.UI.Xaml.CornerRadius GetCornerRadius(FrameworkElement obj)
    {
        return (Microsoft.UI.Xaml.CornerRadius)obj.GetValue(CornerRadiusProperty);
    }

    public static void SetCornerRadius(FrameworkElement obj, Microsoft.UI.Xaml.CornerRadius value)
    {
        obj.SetValue(CornerRadiusProperty, value);
    }

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.RegisterAttached("CornerRadius", typeof(Microsoft.UI.Xaml.CornerRadius), typeof(SquircleClipAttach), new PropertyMetadata(BoxedCornerRadius, OnPropertyChanged));

    public static double GetCornerSmoothing(FrameworkElement obj)
    {
        return (double)obj.GetValue(CornerSmoothingProperty);
    }

    public static void SetCornerSmoothing(FrameworkElement obj, double value)
    {
        obj.SetValue(CornerSmoothingProperty, value);
    }

    public static readonly DependencyProperty CornerSmoothingProperty =
        DependencyProperty.RegisterAttached("CornerSmoothing", typeof(double), typeof(SquircleClipAttach), new PropertyMetadata(BoxedDouble, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement sender && !Equals(e.NewValue, e.OldValue))
        {
            var cornerRadius = GetCornerRadius(sender);
            var cornerSmoothing = GetCornerSmoothing(sender);

            var props = new SquircleProperties(
                sender.ActualWidth,
                sender.ActualHeight,
                cornerRadius.ToCornerRadius(),
                cornerSmoothing,
                true);

            sender.SizeChanged -= OnSizeChanged;

            if (SquircleFactory.IsValidProperties(in props, false))
            {
                sender.SizeChanged += OnSizeChanged;
            }

            UpdateElementCorner(sender, in props);
        }

        static void OnSizeChanged(object _sender, SizeChangedEventArgs _e)
        {
            if (_sender is FrameworkElement _element)
            {
                var _cornerRadius = GetCornerRadius(_element);
                var _cornerSmoothing = GetCornerSmoothing(_element);

                UpdateElementCorner(
                    _element,
                    new SquircleProperties(
                        _e.NewSize.Width,
                        _e.NewSize.Height,
                        _cornerRadius.ToCornerRadius(),
                        _cornerSmoothing,
                        true));
            }
        }

        static void UpdateElementCorner(FrameworkElement? _element, in SquircleProperties _props)
        {
            const string SquircleClipCommit = "_SQUIRCLE_CLIP";

            if (_element == null) return;

            var _visual = ElementCompositionPreview.GetElementVisual(_element);
            var _compositor = _visual.Compositor;

            var _pathBuilder = SquircleFactory.CreateGeometry(in _props, () => new CompositionPathBuilder());
            var _path = _pathBuilder?.CreateGeometry();
            if (_path == null)
            {
                if (_visual.Clip is CompositionGeometricClip _clip
                    && _visual.Clip.Comment == SquircleClipCommit
                    && _clip.Geometry is CompositionPathGeometry _pathGeometry)
                {
                    _pathGeometry.Path = null;
                }
            }
            else
            {
                if (_visual.Clip is CompositionGeometricClip _clip
                    && _visual.Clip.Comment == SquircleClipCommit
                    && _clip.Geometry is CompositionPathGeometry _pathGeometry)
                {
                    _pathGeometry.Path = _path;
                }
                else
                {
                    _pathGeometry = _compositor.CreatePathGeometry(_path);
                    _clip = _compositor.CreateGeometricClip(_pathGeometry);
                    _clip.Comment = SquircleClipCommit;
                    _visual.Clip = _clip;
                }
            }
        }
    }
}
