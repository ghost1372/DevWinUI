// https://github.com/cnbluefire

using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.Graphics.Display;

namespace DevWinUI;

public partial class BubbleView : Control
{
    public static readonly DependencyProperty IsBubbingProperty =
        DependencyProperty.Register("IsBubbing", typeof(bool), typeof(BubbleView), new PropertyMetadata(false, (s, a) =>
        {
            if (a.NewValue != a.OldValue)
                if (s is BubbleView sender)
                    if (a.NewValue is true)
                    {
                        sender.ShowBubbles();
                        sender.SetValue(IsBubbingProperty, false);
                    }
        }));

    private ContainerVisual _BubblesVisual;

    private CanvasDevice _canvasDevice;

    private Compositor _Compositor;
    private CompositionGraphicsDevice _graphicsDevice;
    private Visual _HostVisual;

    private Rectangle BubbleHost;

    private List<Bubble> Bubbles;
    private Color ForegroundColor;

    private long ForegroundPropertyChangedToken;

    public BubbleView()
    {
        DefaultStyleKey = typeof(BubbleView);
        ForegroundPropertyChangedToken = RegisterPropertyChangedCallback(ForegroundProperty, ForegroundPropertyChanged);
        Unloaded += BubbleView_Unloaded;
    }

    public bool IsBubbing
    {
        get => (bool)GetValue(IsBubbingProperty);
        set => SetValue(IsBubbingProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        BubbleHost = GetTemplateChild("BubbleHost") as Rectangle;
        BubbleHost.SizeChanged += BubbleHost_SizeChanged;
        SetupComposition();
        SetupDevices();
    }

    private void SetupComposition()
    {
        _HostVisual = ElementCompositionPreview.GetElementVisual(BubbleHost);
        _Compositor = _HostVisual.Compositor;

        _BubblesVisual = _Compositor.CreateContainerVisual();
        _BubblesVisual.BindSize(_HostVisual);

        ElementCompositionPreview.SetElementChildVisual(BubbleHost, _BubblesVisual);
    }

    private void SetupDevices()
    {
        _canvasDevice = CanvasDevice.GetSharedDevice();
        _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_Compositor, _canvasDevice);

        _canvasDevice.DeviceLost += _canvasDevice_DeviceLost;
        _graphicsDevice.RenderingDeviceReplaced += _graphicsDevice_RenderingDeviceReplaced;
    }

    private void ResetDevices(bool IsDeviceLost)
    {
        try
        {
            if (IsDeviceLost)
            {
                _canvasDevice.DeviceLost -= _canvasDevice_DeviceLost;
                _canvasDevice = CanvasDevice.GetSharedDevice();
                _canvasDevice.DeviceLost += _canvasDevice_DeviceLost;
            }

            CanvasComposition.SetCanvasDevice(_graphicsDevice, _canvasDevice);
        }
        catch (Exception e) when (_canvasDevice != null && _canvasDevice.IsDeviceLost(e.HResult))
        {
            _canvasDevice.RaiseDeviceLost();
        }
    }

    private void ClearBubbles()
    {
        if (Bubbles != null)
        {
            foreach (var bubble in Bubbles) bubble.Dispose();
            Bubbles.Clear();
            Bubbles = null;
        }

        if (_BubblesVisual != null) _BubblesVisual.Children.RemoveAll();
    }

    private void CreateBubbles()
    {
        ClearBubbles();
        if (DesignMode.DesignModeEnabled) return;
        if (_canvasDevice == null || _graphicsDevice == null) return;
        if (ForegroundColor != Colors.Transparent && ActualWidth > 0 && ActualHeight > 0)
        {
            var count = 20;

            var _Bubbles = new List<Bubble>();

            var duration = TimeSpan.FromSeconds(1d);

            for (var i = 0; i < count; i++)
            {
                var bubble = new Bubble(_Compositor, _canvasDevice, _graphicsDevice,
                    new Size(ActualWidth, ActualHeight), ForegroundColor, duration, true);
                bubble.AddTo(_BubblesVisual);
                _Bubbles.Add(bubble);
            }

            for (var i = 0; i < count; i++)
            {
                var bubble = new Bubble(_Compositor, _canvasDevice, _graphicsDevice,
                    new Size(ActualWidth, ActualHeight), ForegroundColor, duration, false);
                bubble.AddTo(_BubblesVisual);
                _Bubbles.Add(bubble);
            }

            Bubbles = _Bubbles;
        }
    }

    public void ShowBubbles()
    {
        if (Bubbles != null)
            foreach (var bubble in Bubbles)
                bubble.Start();
    }

    private void DisplayInformation_DisplayContentsInvalidated(DisplayInformation sender, object args)
    {
        ResetDevices(false);
    }

    private void _canvasDevice_DeviceLost(CanvasDevice sender, object args)
    {
        ResetDevices(true);
    }

    private void _graphicsDevice_RenderingDeviceReplaced(CompositionGraphicsDevice sender,
        RenderingDeviceReplacedEventArgs args)
    {
        CreateBubbles();
    }

    private void BubbleHost_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        CreateBubbles();
    }

    private void BubbleView_Unloaded(object sender, RoutedEventArgs e)
    {
        ClearBubbles();
        _BubblesVisual?.Dispose();
        _BubblesVisual = null;
        _canvasDevice = null;
        _graphicsDevice?.Dispose();
        _graphicsDevice = null;
    }

    private void ForegroundPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (Foreground is SolidColorBrush brush) ForegroundColor = brush.Color;
        CreateBubbles();
    }
}
