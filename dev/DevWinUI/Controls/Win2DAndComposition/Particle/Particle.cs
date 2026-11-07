namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasControl))]
public partial class Particle : Control
{
    private string PART_Canvas = "PART_Canvas";
    private CanvasControl _Canvas;

    private DispatcherTimer dispatcherTimer = new DispatcherTimer();

    private List<ParticleModel> Particles = new List<ParticleModel>();
    private bool MouseHandled;
    private SpinLock ParticleLock = new SpinLock();
    private Vector2 PointerPosition;
    private long PointerDrawCount = 0;
    private bool IsPointerIn = false;
    private bool _IsPointerEnable = true;
    private Color _LineColor = Colors.DarkGray;
    private Color _ParticleColor = Colors.Gray;
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (_Canvas != null)
        {
            _Canvas.Draw -= OnDraw;
            _Canvas.SizeChanged -= OnCanvasSizeChanged;
        }

        _Canvas = GetTemplateChild(PART_Canvas) as CanvasControl;
        if (_Canvas != null)
        {
            _Canvas.Draw -= OnDraw;
            _Canvas.Draw += OnDraw;
            _Canvas.SizeChanged -= OnCanvasSizeChanged;
            _Canvas.SizeChanged += OnCanvasSizeChanged;

            Loaded -= Particle_Loaded;
            Loaded += Particle_Loaded;
            Unloaded -= Particle_Unloaded;
            Unloaded += Particle_Unloaded;

            if (_IsPointerEnable)
            {
                AddMouseHandle();
            }

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            dispatcherTimer.Tick -= DispatcherTimer_Tick;
            dispatcherTimer.Tick += DispatcherTimer_Tick;

            if (Paused)
            {
                _Canvas.Visibility = Visibility.Collapsed;
            }
            else
            {
                dispatcherTimer.Start();
            }
        }
    }

    private void Particle_Loaded(object sender, RoutedEventArgs e)
    {
        if (_Canvas != null && !Paused)
        {
            dispatcherTimer.Start();
        }
    }

    private void Particle_Unloaded(object sender, RoutedEventArgs e)
    {
        dispatcherTimer.Stop();
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ReDrawCanvas();
    }

    private void Particle_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        IsPointerIn = false;
    }

    private void Particle_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        IsPointerIn = true;
        PointerDrawCount = 0;
        PointerPosition = e.GetCurrentPoint(this).Position.ToVector2();
    }

    private void DispatcherTimer_Tick(object sender, object e)
    {
        ReDrawCanvas();
    }

    private void ReDrawCanvas()
    {
        if (_Canvas != null)
        {
            _Canvas.Invalidate();
        }
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var GotLock = false;
        try
        {
            ParticleLock.Enter(ref GotLock);
            UpdateParticles(finalSize);
        }
        finally
        {
            if (GotLock)
            {
                ParticleLock.Exit();
            }
        }
        return base.ArrangeOverride(finalSize);
    }

    private int GetParticleCount(Size finalSize)
    {
        var density = Density;
        if (density > 9)
        {
            density = 9;
        }

        if (density < 0)
        {
            density = 5;
        }
        return Convert.ToInt32(Math.Min(finalSize.Width, finalSize.Height) / (10 - density));
    }

    private void UpdateParticles(Size finalSize)
    {
        var Count = GetParticleCount(finalSize);
        var ChangedCount = Count - Particles.Count;
        var Container = finalSize.ToVector2();
        if (ChangedCount > 0)
        {
            for (int i = 0; i < ChangedCount; i++)
            {
                Particles.Add(ParticleModel.CreateParticle(Container));
            }
        }
        else if (ChangedCount < 0)
        {
            Particles.RemoveRange(0, -ChangedCount);
        }
        for (int i = 0; i < Particles.Count; i++)
        {
            Particles[i].Container = new Vector3(Container.X, Container.Y, Particles[i].Container.Z);
            if (Particles[i].Offset.X > Container.X || Particles[i].Offset.Y > Container.Y) Particles[i].ResetOffset(Container);
        }

        ReDrawCanvas();
    }

    private void AddMouseHandle()
    {
        bool GotLock = false;
        try
        {
            ParticleLock.Enter(ref GotLock);
            if (!MouseHandled)
            {
                PointerMoved += Particle_PointerMoved;
                PointerExited += Particle_PointerExited;
                MouseHandled = true;
            }
        }
        finally
        {
            if (GotLock) ParticleLock.Exit();
        }
    }

    private void RemoveMouseHandle()
    {
        bool GotLock = false;
        try
        {
            ParticleLock.Enter(ref GotLock);
            if (MouseHandled)
            {
                PointerMoved -= Particle_PointerMoved;
                PointerExited -= Particle_PointerExited;
                MouseHandled = false;
            }
        }
        finally
        {
            if (GotLock) ParticleLock.Exit();
        }
    }

    private static void DensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            if (d is Particle sender)
            {
                bool GotLock = false;
                try
                {
                    sender.ParticleLock.Enter(ref GotLock);
                    sender.UpdateParticles(sender.RenderSize);
                }
                finally
                {
                    if (GotLock) sender.ParticleLock.Exit();
                }
            }
        }
    }

    private static void IsPointerEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            if (d is Particle sender)
            {
                sender._IsPointerEnable = (bool)e.NewValue;
                if (sender._IsPointerEnable)
                {
                    sender.AddMouseHandle();
                }
                else
                {
                    sender.RemoveMouseHandle();
                    sender.IsPointerIn = false;
                }

                sender.ReDrawCanvas();
            }
        }
    }


    private static void IsPausedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            if (d is Particle sender)
            {
                var paused = (bool)e.NewValue;
                if (paused)
                {
                    if (sender._Canvas != null)
                    {
                        sender._Canvas.Visibility = Visibility.Collapsed;
                    }
                    sender.dispatcherTimer?.Stop();
                }
                else
                {
                    if (sender._Canvas != null)
                    {
                        sender._Canvas.Visibility = Visibility.Visible;
                        sender.dispatcherTimer?.Start();
                    }
                }
                sender.ReDrawCanvas();
            }
        }
    }

    private static void ParticleColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            if (d is Particle sender)
            {
                if (e.NewValue is Color color)
                    sender._ParticleColor = color;
                sender.ReDrawCanvas();
            }
        }
    }

    private static void LineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            if (d is Particle sender)
            {
                if (e.NewValue is Color color)
                    sender._LineColor = color;
                sender.ReDrawCanvas();
            }
        }
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var GotLock = false;
        try
        {
            ParticleLock.Enter(ref GotLock);
            var session = args.DrawingSession;
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                var offset = particle.Offset.ToVector2();
                for (int j = i + 1; j < Particles.Count; j++)
                {
                    var other = Particles[j].Offset.ToVector2();
                    var rangeSquared = Vector2.DistanceSquared(offset, other);
                    if (rangeSquared < 14400f)
                    {
                        session.DrawLine(offset, other, _LineColor, (120f - MathF.Sqrt(rangeSquared)) / 80f);
                    }
                }
                if (_IsPointerEnable && IsPointerIn)
                {
                    var pointerRange = particle.GetRange(PointerPosition);
                    if (pointerRange < 120)
                    {
                        session.DrawLine(offset, PointerPosition, _LineColor, (120f - pointerRange) / 80f);
                        if (pointerRange > 80)
                        {
                            var tmp = PointerPosition - offset;
                            var pointerHead = tmp / Math.Max(Math.Abs(tmp.X), Math.Abs(tmp.Y));
                            particle.Offset += new Vector3(pointerHead.X, pointerHead.Y, 0);
                            offset = particle.Offset.ToVector2();
                        }
                    }
                    if (PointerDrawCount > 36000)
                    {
                        IsPointerIn = false;
                    }
                    PointerDrawCount++;
                }
                session.FillCircle(offset, particle.Offset.Z / 30, _ParticleColor);
                particle.NextStep();
            }
        }
        finally
        {
            if (GotLock) ParticleLock.Exit();
        }
    }
}
