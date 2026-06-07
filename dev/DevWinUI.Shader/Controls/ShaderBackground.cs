using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Renderers))]
public partial class ShaderBackground : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private CanvasAnimatedControl canvas;

    public event EventHandler<CanvasAnimatedUpdateEventArgs> Update;
    public event EventHandler<CanvasAnimatedDrawEventArgs> Draw;
    public event EventHandler<CanvasCreateResourcesEventArgs> CreateResources;

    private readonly EdgeFadeMaskRenderer _edgeFadeMaskRenderer = new();
    public ObservableCollection<RendererBase> Renderers { get; }
        = new ObservableCollection<RendererBase>();
    public ShaderBackground()
    {
        DefaultStyleKey = typeof(ShaderBackground);

        Renderers.CollectionChanged += OnRenderersChanged;
    }
    private void OnRenderersChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (RendererBase r in e.NewItems)
            { }

        if (e.OldItems != null)
            foreach (RendererBase r in e.OldItems)
                r.Dispose();
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        canvas = GetTemplateChild(PART_Canvas) as CanvasAnimatedControl;

        canvas.CreateResources -= OnCreateResources;
        canvas.CreateResources += OnCreateResources;

        canvas.Draw -= OnDraw;
        canvas.Draw += OnDraw;

        canvas.Update -= OnUpdate;
        canvas.Update += OnUpdate;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;

        foreach (var item in Renderers)
        {
            item.OnApplyTemplate();
        }
    }

    private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        canvas.Draw -= OnDraw;
        canvas.Update -= OnUpdate;
        canvas.CreateResources -= OnCreateResources;

        canvas.Paused = true;

        canvas.RemoveFromVisualTree();
        canvas = null;

        foreach (var item in Renderers)
        {
            item.Dispose();
        }
        _edgeFadeMaskRenderer.Dispose();
    }
    private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        Update?.Invoke(sender, args);

        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        foreach (var item in Renderers)
        {
            item.Update(sender, args);
        }

        if (isEdgeFeatheringEnabled)
        {
            _edgeFadeMaskRenderer.Update(sender, (float)sender.Size.Width, (float)sender.Size.Height, edgeFeatheringLeft, edgeFeatheringTop, edgeFeatheringRight, edgeFeatheringBottom);
        }
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        Draw?.Invoke(sender, args);

        var ds = args.DrawingSession;

        if (isEdgeFeatheringEnabled && _edgeFadeMaskRenderer.Brush != null)
        {
            using (ds.CreateLayer(_edgeFadeMaskRenderer.Brush))
            {
                foreach (var item in Renderers)
                {
                    item.Draw(sender, args);
                }
            }
        }
        else
        {
            foreach (var item in Renderers)
            {
                item.Draw(sender, args);
            }
        }
    }

    private void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        CreateResources?.Invoke(sender, args);
        foreach (var item in Renderers)
        {
            item.CreateResources(sender, args);
        }
    }
}
