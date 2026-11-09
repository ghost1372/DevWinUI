using Microsoft.UI.Xaml.Automation;

namespace DevWinUI;

public sealed partial class ColorArchiveScroll
{
    public IList<ColorArchiveProject>? Projects
    {
        get { return (IList<ColorArchiveProject>?)GetValue(ProjectsProperty); }
        set { SetValue(ProjectsProperty, value); }
    }

    public static readonly DependencyProperty ProjectsProperty =
        DependencyProperty.Register(nameof(Projects), typeof(IList<ColorArchiveProject>), typeof(ColorArchiveScroll),
            new PropertyMetadata(null, (d, e) => ((ColorArchiveScroll)d).OnProjectsChanged(
                (IList<ColorArchiveProject>?)e.OldValue, (IList<ColorArchiveProject>?)e.NewValue)));

    public bool ShowPreview
    {
        get { return (bool)GetValue(ShowPreviewProperty); }
        set { SetValue(ShowPreviewProperty, value); }
    }

    public static readonly DependencyProperty ShowPreviewProperty =
        DependencyProperty.Register(nameof(ShowPreview), typeof(bool), typeof(ColorArchiveScroll),
            new PropertyMetadata(true, (d, e) => ((ColorArchiveScroll)d).DoLayout()));

    public ColorArchivePreviewSize PreviewSize
    {
        get { return (ColorArchivePreviewSize)GetValue(PreviewSizeProperty); }
        set { SetValue(PreviewSizeProperty, value); }
    }

    public static readonly DependencyProperty PreviewSizeProperty =
        DependencyProperty.Register(nameof(PreviewSize), typeof(ColorArchivePreviewSize), typeof(ColorArchiveScroll),
            new PropertyMetadata(ColorArchivePreviewSize.Large, (d, e) => ((ColorArchiveScroll)d).DoLayout()));

    public bool EnableSound
    {
        get { return (bool)GetValue(EnableSoundProperty); }
        set { SetValue(EnableSoundProperty, value); }
    }

    public static readonly DependencyProperty EnableSoundProperty =
        DependencyProperty.Register(nameof(EnableSound), typeof(bool), typeof(ColorArchiveScroll),
            new PropertyMetadata(false, (d, e) =>
            {
                if ((bool)e.NewValue) ((ColorArchiveScroll)d).EnsureSound();
            }));

    public Uri? TickSource
    {
        get { return (Uri?)GetValue(TickSourceProperty); }
        set { SetValue(TickSourceProperty, value); }
    }

    public static readonly DependencyProperty TickSourceProperty =
        DependencyProperty.Register(nameof(TickSource), typeof(Uri), typeof(ColorArchiveScroll),
            new PropertyMetadata(null, (d, e) => ((ColorArchiveScroll)d).ResetSound()));

    public bool InfiniteScroll
    {
        get { return (bool)GetValue(InfiniteScrollProperty); }
        set { SetValue(InfiniteScrollProperty, value); }
    }

    public static readonly DependencyProperty InfiniteScrollProperty =
        DependencyProperty.Register(nameof(InfiniteScroll), typeof(bool), typeof(ColorArchiveScroll),
            new PropertyMetadata(true, (d, e) => ((ColorArchiveScroll)d).OnInfiniteChanged()));

    public double ScrollThreshold
    {
        get { return (double)GetValue(ScrollThresholdProperty); }
        set { SetValue(ScrollThresholdProperty, value); }
    }

    public static readonly DependencyProperty ScrollThresholdProperty =
        DependencyProperty.Register(nameof(ScrollThreshold), typeof(double), typeof(ColorArchiveScroll), new PropertyMetadata(90.0));

    public bool ShowRuler
    {
        get { return (bool)GetValue(ShowRulerProperty); }
        set { SetValue(ShowRulerProperty, value); }
    }

    public static readonly DependencyProperty ShowRulerProperty =
        DependencyProperty.Register(nameof(ShowRuler), typeof(bool), typeof(ColorArchiveScroll),
            new PropertyMetadata(true, (d, e) => ((ColorArchiveScroll)d).DoLayout()));

    public double ActiveAnchor
    {
        get { return (double)GetValue(ActiveAnchorProperty); }
        set { SetValue(ActiveAnchorProperty, value); }
    }

    public static readonly DependencyProperty ActiveAnchorProperty =
        DependencyProperty.Register(nameof(ActiveAnchor), typeof(double), typeof(ColorArchiveScroll),
            new PropertyMetadata(0.68, (d, e) => ((ColorArchiveScroll)d).DoLayout()));

    public double RowLeading
    {
        get { return (double)GetValue(RowLeadingProperty); }
        set { SetValue(RowLeadingProperty, value); }
    }

    public static readonly DependencyProperty RowLeadingProperty =
        DependencyProperty.Register(nameof(RowLeading), typeof(double), typeof(ColorArchiveScroll),
            new PropertyMetadata(1.9, (d, e) => ((ColorArchiveScroll)d).DoLayout()));

    public int? ActiveIndex
    {
        get { return (int?)GetValue(ActiveIndexProperty); }
        set { SetValue(ActiveIndexProperty, value); }
    }

    public static readonly DependencyProperty ActiveIndexProperty =
        DependencyProperty.Register(nameof(ActiveIndex), typeof(int?), typeof(ColorArchiveScroll),
            new PropertyMetadata(null, (d, e) => ((ColorArchiveScroll)d).OnActiveIndexChanged()));

    public int DefaultActiveIndex
    {
        get { return (int)GetValue(DefaultActiveIndexProperty); }
        set { SetValue(DefaultActiveIndexProperty, value); }
    }

    public static readonly DependencyProperty DefaultActiveIndexProperty =
        DependencyProperty.Register(nameof(DefaultActiveIndex), typeof(int), typeof(ColorArchiveScroll), new PropertyMetadata(0));

    public bool ReduceMotion
    {
        get { return (bool)GetValue(ReduceMotionProperty); }
        set { SetValue(ReduceMotionProperty, value); }
    }

    public static readonly DependencyProperty ReduceMotionProperty =
        DependencyProperty.Register(nameof(ReduceMotion), typeof(bool), typeof(ColorArchiveScroll), new PropertyMetadata(false));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(ColorArchiveScroll),
            new PropertyMetadata("Project archive", (d, e) =>
                AutomationProperties.SetName((ColorArchiveScroll)d, (string)e.NewValue)));
}
