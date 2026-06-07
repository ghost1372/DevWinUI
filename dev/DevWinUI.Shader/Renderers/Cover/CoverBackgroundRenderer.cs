using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace DevWinUI;

public partial class CoverBackgroundRenderer : RendererBase
{
    private CanvasBitmap? _currentBitmap;
    private CanvasBitmap? _previousBitmap;

    private CanvasRenderTarget? _currentTargetCache;
    private CanvasRenderTarget? _previousTargetCache;

    private Size _lastScreenSize;
    private bool _lastWasRotating = false;

    private readonly ValueTransition<double> _crossfadeTransition;
    private float _rotationAngle = 0f;

    private bool _needsCacheUpdate = false;

    public CoverBackgroundRenderer()
    {
        _crossfadeTransition = new ValueTransition<double>(1.0, AnimationEasingHelper.GetInterpolatorByEasingType<double>(AnimationEasingType.Linear), 0.7);
    }

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        var tasks = new Task[]
       {
            ReloadCoverBackgroundResourcesAsync()
       };
        args.TrackAsyncAction(Task.WhenAll(tasks).AsAsyncAction());
    }

    public void SetCoverBitmap(CanvasBitmap? newBitmap)
    {
        if (_currentBitmap == newBitmap) return;

        _previousBitmap = _currentBitmap;
        _previousTargetCache = _currentTargetCache;
        _currentTargetCache = null;

        _currentBitmap = newBitmap;

        if (_currentBitmap == null)
        {
            _crossfadeTransition.JumpTo(1.0);
        }
        else
        {
            if (_previousBitmap == null)
            {
                _crossfadeTransition.JumpTo(1.0);
            }
            else
            {
                _crossfadeTransition.JumpTo(0.0);
                _crossfadeTransition.Start(1.0);
            }
        }

        _needsCacheUpdate = true;
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (!isEnabled) return;

        UpdateBreathing(currentBassEnergy, breathingIntensity);

        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        _crossfadeTransition.Update(elapsedTime);

        if (coverOverlaySpeed > 0)
        {
            float baseSpeed = 0.6f;
            float currentSpeed = (coverOverlaySpeed / 100.0f) * baseSpeed;
            _rotationAngle += currentSpeed * (float)elapsedTime.TotalSeconds;
            _rotationAngle %= (float)(2 * Math.PI);
        }

        if (_crossfadeTransition.Value >= 1.0 && _previousBitmap != null)
        {
            _previousBitmap = null;
            _previousTargetCache?.Dispose();
            _previousTargetCache = null;
        }

    }
    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (!isEnabled || coverOverlayOpacity <= 0) return;

        var ds = args.DrawingSession;

        if (_lastScreenSize != sender.Size)
        {
            _lastScreenSize = sender.Size;
            _needsCacheUpdate = true;
        }

        bool isRotating = coverOverlaySpeed > 0;
        if (_lastWasRotating != isRotating)
        {
            _lastWasRotating = isRotating;
            _needsCacheUpdate = true;
        }

        EnsureCachedLayer(sender, _currentBitmap, ref _currentTargetCache);

        float baseAlpha = coverOverlayOpacity / 100.0f;
        float angle = isRotating ? _rotationAngle : 0f;
        double fadeProgress = _crossfadeTransition.Value;
        bool isCrossfading = fadeProgress < 1.0 && _previousTargetCache != null;

        Vector2 screenCenter = new Vector2((float)sender.Size.Width / 2f, (float)sender.Size.Height / 2f);

        ApplyBreathingTransform(ds, screenCenter, isBreathingEffectEnabled);

        if (isCrossfading)
        {
            DrawCachedLayer(ds, _previousTargetCache, screenCenter, angle, baseAlpha);

            float newLayerAlpha = baseAlpha * (float)fadeProgress;
            DrawCachedLayer(ds, _currentTargetCache, screenCenter, angle, newLayerAlpha);
        }
        else if (_currentTargetCache != null)
        {
            DrawCachedLayer(ds, _currentTargetCache, screenCenter, angle, baseAlpha);
        }

        ResetTransform(ds, isBreathingEffectEnabled);
    }

    private void EnsureCachedLayer(ICanvasResourceCreator resourceCreator, CanvasBitmap? sourceBitmap, ref CanvasRenderTarget? targetCache)
    {
        if (sourceBitmap == null)
        {
            targetCache?.Dispose();
            targetCache = null;
            return;
        }

        bool deviceMismatch = targetCache != null && targetCache.Device != resourceCreator.Device;

        if (_needsCacheUpdate || targetCache == null || deviceMismatch)
        {
            targetCache?.Dispose();

            float imgW = sourceBitmap.SizeInPixels.Width;
            float imgH = sourceBitmap.SizeInPixels.Height;
            Size screenSize = _lastScreenSize;

            float scale;
            if (_lastWasRotating) // Speed > 0
            {
                float screenDiagonal = (float)Math.Sqrt(screenSize.Width * screenSize.Width + screenSize.Height * screenSize.Height);
                scale = Math.Max(screenDiagonal / imgW, screenDiagonal / imgH);
            }
            else
            {
                float scaleX = (float)screenSize.Width / imgW;
                float scaleY = (float)screenSize.Height / imgH;
                scale = Math.Max(scaleX, scaleY);
            }

            float targetW = imgW * scale;
            float targetH = imgH * scale;

            targetCache = new CanvasRenderTarget(resourceCreator, targetW, targetH, sourceBitmap.Dpi);

            using (var ds = targetCache.CreateDrawingSession())
            {
                ds.Clear(Windows.UI.Color.FromArgb(0, 0, 0, 0));

                using (var transformEffect = new Transform2DEffect())
                using (var blurEffect = new GaussianBlurEffect())
                {
                    transformEffect.Source = sourceBitmap;
                    transformEffect.TransformMatrix = Matrix3x2.CreateScale(scale);
                    transformEffect.InterpolationMode = CanvasImageInterpolation.Linear;

                    blurEffect.Source = transformEffect;
                    blurEffect.BlurAmount = coverOverlayBlurAmount;
                    blurEffect.BorderMode = EffectBorderMode.Hard;

                    ds.DrawImage(blurEffect);
                }
            }

            if (sourceBitmap == _currentBitmap)
            {
                _needsCacheUpdate = false;
            }
        }
    }

    private static void DrawCachedLayer(CanvasDrawingSession ds, CanvasRenderTarget? cachedTexture, Vector2 screenCenter, float rotationRadians, float alpha)
    {
        if (cachedTexture == null) return;

        Vector2 textureCenter = new Vector2((float)cachedTexture.Size.Width / 2f, (float)cachedTexture.Size.Height / 2f);

        Matrix3x2 transform =
            Matrix3x2.CreateTranslation(-textureCenter) * Matrix3x2.CreateRotation(rotationRadians) * Matrix3x2.CreateTranslation(screenCenter);

        Matrix3x2 previousTransform = ds.Transform;

        ds.Transform = transform * previousTransform;
        ds.DrawImage(cachedTexture, 0, 0, new Rect(0, 0, cachedTexture.Size.Width, cachedTexture.Size.Height), alpha);

        ds.Transform = previousTransform;
    }

    public override void Dispose()
    {
        _currentBitmap?.Dispose();
        _previousBitmap?.Dispose();

        _currentTargetCache?.Dispose();
        _previousTargetCache?.Dispose();

        _currentBitmap = null;
        _previousBitmap = null;
        _currentTargetCache = null;
        _previousTargetCache = null;
    }

    private async Task ReloadCoverBackgroundResourcesAsync()
    {
        try
        {
            if (coverAlbumArtBytes == null || coverAlbumArtBytes.Length == 0) return;

            using (var localMemoryStream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(localMemoryStream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(coverAlbumArtBytes);
                    await writer.StoreAsync();
                }

                localMemoryStream.Seek(0);

                CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), localMemoryStream);
                SetCoverBitmap(bitmap);
            }
        }
        catch (Exception)
        {
        }
    }
}
