using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public sealed partial class BlinkingSquares : Control
{
    private const string PartDrawingCanvas = "PART_DrawingCanvas";
    private const double TwoPi = Math.PI * 2;
    private readonly List<Cell> _cells = [];
    private CanvasAnimatedControl? _drawingCanvas;
    private CancellationTokenSource? _resizeCancellation;
    private Color _renderColor = Color.FromArgb(255, 187, 41, 255);
    private RenderState _renderState = RenderState.Empty;
    private bool _hasGridLayout;
    private bool _isLoaded;

    public BlinkingSquares()
    {
        DefaultStyleKey = typeof(BlinkingSquares);
        SizeChanged += OnSizeChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public BlinkingSquaresDirection Direction
    {
        get => (BlinkingSquaresDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(
        nameof(Direction), typeof(BlinkingSquaresDirection), typeof(BlinkingSquares), new PropertyMetadata(BlinkingSquaresDirection.Right, OnLayoutPropertyChanged));

    public int GridSize
    {
        get => (int)GetValue(GridSizeProperty);
        set => SetValue(GridSizeProperty, value);
    }

    public static readonly DependencyProperty GridSizeProperty = DependencyProperty.Register(
        nameof(GridSize), typeof(int), typeof(BlinkingSquares), new PropertyMetadata(150, OnLayoutPropertyChanged));

    public double SquareSize
    {
        get => (double)GetValue(SquareSizeProperty);
        set => SetValue(SquareSizeProperty, value);
    }

    public static readonly DependencyProperty SquareSizeProperty = DependencyProperty.Register(
        nameof(SquareSize), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(0.72, OnLayoutPropertyChanged));

    public double SquareDensity
    {
        get => (double)GetValue(SquareDensityProperty);
        set => SetValue(SquareDensityProperty, value);
    }

    public static readonly DependencyProperty SquareDensityProperty = DependencyProperty.Register(
        nameof(SquareDensity), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(0.9, OnLayoutPropertyChanged));

    public double FadeStart
    {
        get => (double)GetValue(FadeStartProperty);
        set => SetValue(FadeStartProperty, value);
    }

    public static readonly DependencyProperty FadeStartProperty = DependencyProperty.Register(
        nameof(FadeStart), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(0.15, OnLayoutPropertyChanged));

    public double FadeEnd
    {
        get => (double)GetValue(FadeEndProperty);
        set => SetValue(FadeEndProperty, value);
    }

    public static readonly DependencyProperty FadeEndProperty = DependencyProperty.Register(
        nameof(FadeEnd), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(1.0, OnLayoutPropertyChanged));

    public double Falloff
    {
        get => (double)GetValue(FalloffProperty);
        set => SetValue(FalloffProperty, value);
    }

    public static readonly DependencyProperty FalloffProperty = DependencyProperty.Register(
        nameof(Falloff), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(0.8, OnLayoutPropertyChanged));

    public double MinBrightness
    {
        get => (double)GetValue(MinBrightnessProperty);
        set => SetValue(MinBrightnessProperty, value);
    }

    public static readonly DependencyProperty MinBrightnessProperty = DependencyProperty.Register(
        nameof(MinBrightness), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(0.55, OnAnimationPropertyChanged));

    public double TwinkleSpeed
    {
        get => (double)GetValue(TwinkleSpeedProperty);
        set => SetValue(TwinkleSpeedProperty, value);
    }

    public static readonly DependencyProperty TwinkleSpeedProperty = DependencyProperty.Register(
        nameof(TwinkleSpeed), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(1.4, OnAnimationPropertyChanged));

    public double TwinkleStrength
    {
        get => (double)GetValue(TwinkleStrengthProperty);
        set => SetValue(TwinkleStrengthProperty, value);
    }

    public static readonly DependencyProperty TwinkleStrengthProperty = DependencyProperty.Register(
        nameof(TwinkleStrength), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(0.94, OnAnimationPropertyChanged));

    public double Intensity
    {
        get => (double)GetValue(IntensityProperty);
        set => SetValue(IntensityProperty, value);
    }

    public static readonly DependencyProperty IntensityProperty = DependencyProperty.Register(
        nameof(Intensity), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(1.0, OnAppearancePropertyChanged));

    public Color SquareColor
    {
        get => (Color)GetValue(SquareColorProperty);
        set => SetValue(SquareColorProperty, value);
    }

    public static readonly DependencyProperty SquareColorProperty = DependencyProperty.Register(
        nameof(SquareColor), typeof(Color), typeof(BlinkingSquares), new PropertyMetadata(Color.FromArgb(255, 187, 41, 255), OnAppearancePropertyChanged));

    public double Dpr
    {
        get => (double)GetValue(DprProperty);
        set => SetValue(DprProperty, value);
    }

    public static readonly DependencyProperty DprProperty = DependencyProperty.Register(
        nameof(Dpr), typeof(double), typeof(BlinkingSquares), new PropertyMetadata(1.5, OnLayoutPropertyChanged));

    protected override void OnApplyTemplate()
    {
        CancelPendingResize();
        if (_drawingCanvas is not null)
        {
            _drawingCanvas.Draw -= OnCanvasDraw;
        }

        base.OnApplyTemplate();
        _drawingCanvas = GetTemplateChild(PartDrawingCanvas) as CanvasAnimatedControl;
        UpdateRenderColor();

        if (_isLoaded && _drawingCanvas is not null)
        {
            _drawingCanvas.Draw += OnCanvasDraw;
        }

        RebuildGrid();
    }

    private static void OnAnimationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((BlinkingSquares)sender).UpdateRenderState();
    }

    private static void OnAppearancePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((BlinkingSquares)sender).UpdateRenderColor();
    }

    private static void OnLayoutPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((BlinkingSquares)sender).RebuildGrid();
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        _isLoaded = true;
        if (_drawingCanvas is null)
        {
            return;
        }

        _drawingCanvas.Draw += OnCanvasDraw;
        RebuildGrid();
    }

    private void OnUnloaded(object sender, RoutedEventArgs args)
    {
        _isLoaded = false;
        CancelPendingResize();
        if (_drawingCanvas is not null)
        {
            _drawingCanvas.Draw -= OnCanvasDraw;
        }
    }

    private async void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        if (_drawingCanvas is null)
        {
            return;
        }

        UpdateRenderState(width: ActualWidth, height: ActualHeight);
        if (!_hasGridLayout)
        {
            RebuildGrid();
            return;
        }

        var cancellation = new CancellationTokenSource();
        var previousCancellation = _resizeCancellation;
        _resizeCancellation = cancellation;
        previousCancellation?.Cancel();

        try
        {
            await Task.Delay(120, cancellation.Token);
            if (ReferenceEquals(_resizeCancellation, cancellation))
            {
                _resizeCancellation = null;
                RebuildGrid();
            }
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
        finally
        {
            cancellation.Dispose();
        }
    }

    private void CancelPendingResize()
    {
        var cancellation = _resizeCancellation;
        _resizeCancellation = null;
        cancellation?.Cancel();
    }

    private void RebuildGrid()
    {
        CancelPendingResize();
        _cells.Clear();
        if (_drawingCanvas is null)
        {
            _hasGridLayout = false;
            UpdateRenderState(Array.Empty<Cell>());
            return;
        }

        var width = ActualWidth;
        var height = ActualHeight;
        if (width <= 0 || height <= 0)
        {
            _hasGridLayout = false;
            UpdateRenderState(Array.Empty<Cell>(), width, height);
            return;
        }

        var gridSize = Math.Clamp(GridSize, 8, 200);
        var cellSize = Math.Max(width, height) / gridSize;
        var columns = Math.Max(1, (int)Math.Ceiling(width / cellSize));
        var rows = Math.Max(1, (int)Math.Ceiling(height / cellSize));
        var fadeStart = Math.Clamp(FadeStart, 0, 0.999);
        var fadeEnd = Math.Clamp(FadeEnd, fadeStart + 0.001, 1);
        var falloff = Math.Clamp(Falloff, 0.3, 6);
        var squareSize = Math.Clamp(SquareSize, 0.05, 0.98);
        var squareDensity = Math.Clamp(SquareDensity, 0, 1);
        var pixelScale = GetPixelScale();
        var squareSide = Snap(cellSize * squareSize, pixelScale);
        var minimumBrightness = Math.Clamp(MinBrightness, 0, 1);
        var random = Random.Shared;

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var progress = GetProgress(column, row, columns, rows);
                var fade = Math.Clamp((progress - fadeStart) / (fadeEnd - fadeStart), 0, 1);
                var density = squareDensity * Math.Pow(fade, falloff);
                if (random.NextDouble() >= density)
                {
                    continue;
                }

                var left = Snap((column + 0.5) * cellSize - squareSide / 2, pixelScale);
                var top = Snap((row + 0.5) * cellSize - squareSide / 2, pixelScale);
                _cells.Add(new Cell(left / width, top / height, RandomBetween(minimumBrightness, 1, random), random.NextDouble() * TwoPi));
            }
        }

        _hasGridLayout = true;
        UpdateRenderState(_cells.ToArray(), width, height);
    }

    private void UpdateRenderState(Cell[]? cells = null, double? width = null, double? height = null)
    {
        var current = Volatile.Read(ref _renderState);
        var state = new RenderState(
            cells ?? current.Cells,
            width ?? ActualWidth,
            height ?? ActualHeight,
            Math.Clamp(GridSize, 8, 200),
            Math.Clamp(SquareSize, 0.05, 0.98),
            Math.Clamp(MinBrightness, 0, 1),
            Math.Clamp(TwinkleStrength, 0, 1),
            Math.Clamp(TwinkleSpeed, 0, 4),
            GetPixelScale(),
            _renderColor);
        Volatile.Write(ref _renderState, state);
    }

    private void OnCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var state = Volatile.Read(ref _renderState);
        if (state.Width <= 0 || state.Height <= 0)
        {
            return;
        }

        var cellSize = Math.Max(state.Width, state.Height) / state.GridSize;
        var squareSide = Snap(cellSize * state.SquareSize, state.PixelScale);
        var elapsedSeconds = args.Timing.TotalTime.TotalSeconds;

        foreach (var cell in state.Cells)
        {
            var wave = state.TwinkleSpeed == 0
                ? (Math.Sin(cell.Phase) + 1) / 2
                : (Math.Sin(elapsedSeconds * state.TwinkleSpeed * TwoPi + cell.Phase) + 1) / 2;
            var brightness = GetBrightness(cell, state.MinBrightness, state.TwinkleStrength, wave);
            var alpha = (byte)Math.Clamp((int)Math.Round(state.Color.A * brightness), 0, 255);
            var color = Color.FromArgb(alpha, state.Color.R, state.Color.G, state.Color.B);
            var left = Snap(cell.Left * state.Width, state.PixelScale);
            var top = Snap(cell.Top * state.Height, state.PixelScale);
            args.DrawingSession.FillRectangle(new Rect(left, top, squareSide, squareSide), color);
        }
    }

    private double GetProgress(int column, int row, int columns, int rows)
    {
        var isHorizontal = Direction is BlinkingSquaresDirection.Left or BlinkingSquaresDirection.Right;
        var progress = isHorizontal
            ? (column + 0.5) / columns
            : (row + 0.5) / rows;

        return Direction is BlinkingSquaresDirection.Left or BlinkingSquaresDirection.Top
            ? 1 - progress
            : progress;
    }

    private double GetPixelScale()
    {
        var rasterizationScale = XamlRoot?.RasterizationScale ?? 1;
        return Math.Max(0.5, Math.Min(Math.Clamp(Dpr, 1, 3), rasterizationScale));
    }

    private static double Snap(double value, double scale) => Math.Round(value * scale) / scale;

    private static double RandomBetween(double minimum, double maximum, Random random) =>
        minimum + random.NextDouble() * (maximum - minimum);

    private void UpdateRenderColor()
    {
        var color = SquareColor;
        var intensity = Math.Clamp(Intensity, 0, 2);
        if (intensity > 1)
        {
            color = ColorHelper.LightenColor(color, (float)(intensity - 1));
        }
        else if (intensity < 1)
        {
            color = ColorHelper.DarkenColor(color, (float)(1 - intensity));
        }

        _renderColor = color;
        UpdateRenderState();
    }

    private static double GetBrightness(Cell cell, double minimumBrightness, double strength, double wave)
    {
        var brightness = minimumBrightness + (cell.BaseBrightness - minimumBrightness) * (1 - strength * (1 - wave));
        return Math.Clamp(brightness, 0, 1);
    }

    private readonly record struct Cell(double Left, double Top, double BaseBrightness, double Phase);

    private sealed record RenderState(
        Cell[] Cells,
        double Width,
        double Height,
        int GridSize,
        double SquareSize,
        double MinBrightness,
        double TwinkleStrength,
        double TwinkleSpeed,
        double PixelScale,
        Color Color)
    {
        public static RenderState Empty { get; } = new(Array.Empty<Cell>(), 0, 0, 0, 0, 0, 0, 0, 1, default);
    }
}
