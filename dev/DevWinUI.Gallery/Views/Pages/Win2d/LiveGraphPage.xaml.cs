using System.Numerics;
using Microsoft.Graphics.Canvas.Brushes;

namespace DevWinUIGallery.Views;

public sealed partial class LiveGraphPage : Page
{
    public ObservableCollection<LiveGraphBackgroundMode> LiveGraphBackgroundModeItems { get; set; } = new ObservableCollection<LiveGraphBackgroundMode>(Enum.GetValues<LiveGraphBackgroundMode>());
    public ObservableCollection<HighlightLineBehavior> HighlightLineBehaviorItems { get; set; } = new ObservableCollection<HighlightLineBehavior>(Enum.GetValues<HighlightLineBehavior>());

    private Random random = new Random();

    private string liveGraphKey;
    private (CanvasLinearGradientBrush Brush, CanvasLinearGradientBrush OpacityBrush, CanvasSolidColorBrush BorderBrush) brush;

    public LiveGraphPage()
    {
        InitializeComponent();
    }

    private void LiveGraphSample_Draw(object sender, LiveGraphEventArgs e)
    {
        float cpuValue = (float)random.NextDouble() * 100f; // CPU usage 0-100%
        LiveGraphSample.AddLivePoint(liveGraphKey, new GraphPoint { Value = cpuValue, Space = 6f });
    }

    private void LiveGraphSample_CreateResources(object sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl e)
    {
        brush = LiveGraphSample.GetGreenBrush(e);

        LiveGraphSample.SizeChanged -= OnCanvasResize;
        LiveGraphSample.SizeChanged += OnCanvasResize;

        Unloaded -= LiveGraphPage_Unloaded;
        Unloaded += LiveGraphPage_Unloaded;

        void OnCanvasResize(object s, SizeChangedEventArgs e)
        {
            brush.Brush.StartPoint = new Vector2(0, (float)e.NewSize.Height);
            brush.Brush.EndPoint = new Vector2(0, 0);

            brush.OpacityBrush.StartPoint = new Vector2(0, (float)e.NewSize.Height);
            brush.OpacityBrush.EndPoint = new Vector2(0, 0);
        }
        

        void LiveGraphPage_Unloaded(object sender, RoutedEventArgs e)
        {
            brush.Brush?.Dispose();
            brush.OpacityBrush?.Dispose();
            brush.BorderBrush?.Dispose();
        }

        liveGraphKey = LiveGraphSample.RegisterGraphBrush(null);
        UpdateBrush();
    }

    private void UpdateBrush()
    {
        if(TGBorderLess == null || TGFillLess == null || TGDashDot == null || NBStrokeWidth == null || LiveGraphSample == null)
        {
            return;
        }
        var data = new GraphBrushData
        {
            Brush = brush.Brush,
            OpacityBrush = brush.OpacityBrush,
            BorderBrush = brush.BorderBrush,
            StrokeWidth = (float)NBStrokeWidth?.Value,            
        };

        if (!TGBorderLess.IsOn)
        {
            data.BorderBrush = null;
            data.StrokeStyle = null;
        }

        if (TGFillLess.IsOn)
        {
            data.Brush = null;
            data.OpacityBrush = null;
            data.BorderBrush = brush.BorderBrush;
        }

        if (TGDashDot.IsOn)
        {
            data.StrokeStyle = new Microsoft.Graphics.Canvas.Geometry.CanvasStrokeStyle()
            {
                DashStyle = Microsoft.Graphics.Canvas.Geometry.CanvasDashStyle.Dash,
                DashOffset = 10
            };
        }

        LiveGraphSample.UpdateGraphBrush(liveGraphKey, data);
    }

    private void BtnPurple_Click(object sender, RoutedEventArgs e)
    {
        brush = LiveGraphSample.GetPurpleBrush(LiveGraphSample.GetCanvasAnimatedControl());
        UpdateBrush();
    }

    private void BtnRed_Click(object sender, RoutedEventArgs e)
    {
        brush = LiveGraphSample.GetRedBrush(LiveGraphSample.GetCanvasAnimatedControl());
        UpdateBrush();
    }

    private void BtnBlue_Click(object sender, RoutedEventArgs e)
    {
        brush = LiveGraphSample.GetBlueBrush(LiveGraphSample.GetCanvasAnimatedControl());
        UpdateBrush();
    }

    private void BtnGreen_Click(object sender, RoutedEventArgs e)
    {
        brush = LiveGraphSample.GetGreenBrush(LiveGraphSample.GetCanvasAnimatedControl());
        UpdateBrush();
    }

    private void OnToggled(object sender, RoutedEventArgs e)
    {
        UpdateBrush();
    }

    private void NBStrokeWidth_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        UpdateBrush();
    }
}
