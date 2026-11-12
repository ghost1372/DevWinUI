using System.Collections.Specialized;

namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public event EventHandler<ParticleGlobeMarkerEventArgs> MarkerClicked;

    public IEnumerable<ParticleGlobeMarker> Markers
    {
        get { return (IEnumerable<ParticleGlobeMarker>)GetValue(MarkersProperty); }
        set { SetValue(MarkersProperty, value); }
    }

    public static readonly DependencyProperty MarkersProperty =
        DependencyProperty.Register(nameof(Markers), typeof(IEnumerable<ParticleGlobeMarker>), typeof(ParticleGlobe), new PropertyMetadata(null, OnMarkersChanged));

    public bool ShowMarkerLabels
    {
        get { return (bool)GetValue(ShowMarkerLabelsProperty); }
        set { SetValue(ShowMarkerLabelsProperty, value); }
    }

    public static readonly DependencyProperty ShowMarkerLabelsProperty =
        DependencyProperty.Register(nameof(ShowMarkerLabels), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    private volatile ParticleGlobeMarker[] _markers = Array.Empty<ParticleGlobeMarker>();
    private volatile ParticleGlobeArc[] _userArcs = Array.Empty<ParticleGlobeArc>();
    private readonly object _arcLock = new();
    private ParticleGlobeMarker _hoverMarker;
    private bool _showMarkerLabels = true;
    private CanvasTextFormat _markerFormat;
    private readonly List<(Vector2 Pos, float Alpha, Vector3 Rgb)> _userPackets = new();

    private static void OnMarkersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var globe = (ParticleGlobe)d;
        if (e.OldValue is INotifyCollectionChanged oldNotify) oldNotify.CollectionChanged -= globe.OnMarkersCollectionChanged;
        if (e.NewValue is INotifyCollectionChanged newNotify) newNotify.CollectionChanged += globe.OnMarkersCollectionChanged;
        globe.SnapshotMarkers();
    }

    private void OnMarkersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => SnapshotMarkers();

    private void SnapshotMarkers()
    {
        ParticleGlobeMarker[] snap;
        try
        {
            snap = Markers?.Where(m => m != null).ToArray() ?? Array.Empty<ParticleGlobeMarker>();
        }
        catch (InvalidOperationException)
        {
            return; // collection mutated mid-copy; the next change notification refreshes it
        }

        _markers = snap;
        if (_hoverMarker != null && Array.IndexOf(snap, _hoverMarker) < 0) _hoverMarker = null;
    }

    public ParticleGlobeArc AddArc(ParticleGlobeMarker from, ParticleGlobeMarker to, Color? color = null)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        return AddArc(from.Latitude, from.Longitude, to.Latitude, to.Longitude, color);
    }

    public ParticleGlobeArc AddArc(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, Color? color = null)
    {
        var a = new ParticleGlobeMarker(fromLatitude, fromLongitude).ToVector();
        var b = new ParticleGlobeMarker(toLatitude, toLongitude).ToVector();
        float omega = MathF.Acos(Math.Clamp(Vector3.Dot(a, b), -1f, 1f));
        if (omega < 0.01f || omega > MathF.PI - 0.01f) return null;

        var arc = new ParticleGlobeArc
        {
            A = a,
            B = b,
            Omega = omega,
            Height = 0.12f + 0.3f * (omega / MathF.PI),
            Rgb = color is Color c ? new Vector3(c.R / 255f, c.G / 255f, c.B / 255f) : null
        };

        lock (_arcLock) _userArcs = _userArcs.Append(arc).ToArray();
        return arc;
    }

    public void RemoveArc(ParticleGlobeArc arc)
    {
        if (arc == null) return;
        lock (_arcLock) _userArcs = _userArcs.Where(a => !ReferenceEquals(a, arc)).ToArray();
    }

    public void ClearArcs()
    {
        lock (_arcLock) _userArcs = Array.Empty<ParticleGlobeArc>();
    }

    private ParticleGlobeMarker MarkerAt(Vector2 point)
    {
        var list = _markers;
        float reach = MathF.Max(12f, _radius * 0.03f);
        float best = reach * reach;
        ParticleGlobeMarker hit = null;

        foreach (var m in list)
        {
            var r = Rot(m.ToVector() * 1.004f);
            if (r.Z < 0.02f) continue;

            float d = Vector2.DistanceSquared(Proj(r), point);
            if (d <= best)
            {
                best = d;
                hit = m;
            }
        }
        return hit;
    }

    private void UpdateMarkerHover(bool inside, bool dragging, Vector2 pointer)
    {
        _hoverMarker = inside && !dragging && _showCities ? MarkerAt(pointer) : null;
    }

    private void DrawMarkers(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        var list = _markers;
        var accent = new Vector3(_accent.X, _accent.Y, _accent.Z);

        for (int i = 0; i < list.Length; i++)
        {
            var m = list[i];
            var r = Rot(m.ToVector() * 1.004f);
            if (r.Z < 0.02f) continue;

            var rgb = m.Color is Color c ? new Vector3(c.R / 255f, c.G / 255f, c.B / 255f) : accent;
            var pos = Proj(r);
            float hover = ReferenceEquals(m, _hoverMarker) ? 1f : 0f;
            float pulse = 0.5f + 0.5f * MathF.Sin(t * 3f + i * 1.7f);
            float fade = Math.Min(1f, r.Z * 3f);

            batch.Draw(_dot, pos, Tint(rgb, (0.35f + 0.4f * pulse + 0.25f * hover) * fade), origin, 0f, new Vector2(sizeBoost * (14f + 10f * pulse + 8f * hover)), CanvasSpriteFlip.None);
            batch.Draw(_dot, pos, Tint(Vector3.Lerp(rgb, Vector3.One, 0.7f), fade), origin, 0f, new Vector2(sizeBoost * (4f + 1.5f * hover)), CanvasSpriteFlip.None);
        }
    }

    private void DrawMarkerLabels(CanvasDrawingSession ds)
    {
        if (!_showMarkerLabels || !_showCities) return;

        var list = _markers;
        if (list.Length == 0) return;

        _markerFormat ??= new CanvasTextFormat { FontSize = 11f, FontFamily = "Segoe UI" };

        foreach (var m in list)
        {
            if (string.IsNullOrEmpty(m.Label)) continue;

            var r = Rot(m.ToVector() * 1.004f);
            if (r.Z < 0.15f) continue;

            var rgb = m.Color is Color c ? c : Color.FromArgb(255, (byte)(_accent.X * 255f), (byte)(_accent.Y * 255f), (byte)(_accent.Z * 255f));
            bool hover = ReferenceEquals(m, _hoverMarker);
            float fade = Math.Min(1f, (r.Z - 0.15f) * 4f) * (hover ? 1f : 0.8f);

            var p = Proj(r) + new Vector2(9f, -7f);
            ds.DrawText(m.Label, p + Vector2.One, Color.FromArgb((byte)(160 * fade), 0, 0, 0), _markerFormat);
            ds.DrawText(m.Label, p, Color.FromArgb((byte)(255 * fade), rgb.R, rgb.G, rgb.B), _markerFormat);
        }
    }

    private void AdvanceUserArcs(float dt)
    {
        foreach (var a in _userArcs) a.Age += dt;
    }

    private void DrawUserArcs(CanvasDrawingSession ds)
    {
        _userPackets.Clear();
        var arcs = _userArcs;
        if (arcs.Length == 0) return;

        const int Seg = 48;
        var path = new ParticleGlobeArcState();
        var defRgb = new Vector3(_arc.X, _arc.Y, _arc.Z);

        foreach (var a in arcs)
        {
            var rgb = a.Rgb ?? defRgb;
            var col = Color.FromArgb(255, (byte)(rgb.X * 255f), (byte)(rgb.Y * 255f), (byte)(rgb.Z * 255f));

            float grow = Math.Clamp(a.Age / LinkGrow, 0f, 1f);
            grow = 1f - (1f - grow) * (1f - grow);

            path.A = a.A; path.B = a.B; path.Omega = a.Omega; path.Height = a.Height;

            Vector2 prev = default;
            bool havePrev = false;
            for (int k = 0; k <= Seg; k++)
            {
                var r = Rot(ArcPoint(path, grow * k / Seg));
                if (Occluded(r)) { havePrev = false; continue; }

                var p = Proj(r);
                if (havePrev && Jumped(prev, p)) havePrev = false;
                if (havePrev)
                {
                    ds.DrawLine(prev, p, Color.FromArgb(46, col.R, col.G, col.B), 4f);
                    ds.DrawLine(prev, p, Color.FromArgb(180, col.R, col.G, col.B), 1.4f);
                }
                prev = p;
                havePrev = true;
            }

            if (a.Age < LinkGrow * 0.6f) continue;

            float flow = (a.Age - LinkGrow * 0.6f) * 0.42f;
            for (int i = 0; i < PacketCount; i++)
            {
                float u = (flow + i / (float)PacketCount) % 1f;
                for (int tr = 0; tr < 4; tr++)
                {
                    float ut = u - tr * 0.012f;
                    if (ut < 0f) break;

                    var r = Rot(ArcPoint(path, ut));
                    if (Occluded(r)) continue;

                    float edge = MathF.Min(1f, MathF.Min(u, 1f - u) * 12f);
                    _userPackets.Add((Proj(r), edge * (1f - tr * 0.25f), rgb));
                }
            }
        }
    }

    private void DrawUserPackets(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost)
    {
        foreach (var (pos, alpha, rgb) in _userPackets)
        {
            batch.Draw(_dot, pos, Tint(rgb, 0.7f * alpha), origin, 0f, new Vector2(sizeBoost * 13f), CanvasSpriteFlip.None);
            batch.Draw(_dot, pos, Tint(Vector3.Lerp(rgb, Vector3.One, 0.7f), alpha), origin, 0f, new Vector2(sizeBoost * 4.5f), CanvasSpriteFlip.None);
        }
    }
}
