namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private List<ParticleGlobeTimelineFrame> _tlFrames;      // guarded by _valLock
    private float[][] _tlT;                      // normalized values per frame per country id (global min/max)
    private volatile int _tlCount;
    private double _tlPos;
    private double _tlSpeed = 1.0;
    private volatile bool _tlPlaying;
    private volatile bool _tlLoop = true;
    private bool _tlApplied;
    private float _tlEventTimer;
    private int _tlLastIndex = -1;

    public event EventHandler<string> TimelineChanged;

    public int TimelineFrameCount => _tlCount;

    public bool IsTimelinePlaying
    {
        get => _tlPlaying;
        set
        {
            if (value && _tlCount > 1 && !_tlLoop && _tlPos >= _tlCount - 1) _tlPos = 0;
            _tlPlaying = value && _tlCount > 1;
        }
    }

    public bool TimelineLoop
    {
        get => _tlLoop;
        set => _tlLoop = value;
    }

    public double TimelineSpeed
    {
        get => _tlSpeed;
        set => _tlSpeed = Math.Clamp(value, 0.05, 30.0);
    }

    public double TimelinePosition
    {
        get => _tlPos;
        set
        {
            int n = _tlCount;
            _tlPos = n > 1 ? Math.Clamp(double.IsFinite(value) ? value : 0, 0, n - 1) : 0;
        }
    }

    public string TimelineLabel
    {
        get
        {
            var frames = _tlFrames;
            if (frames == null || frames.Count == 0) return string.Empty;
            return frames[Math.Clamp((int)Math.Round(_tlPos), 0, frames.Count - 1)].Label;
        }
    }

    public void SetCountryValueTimeline(IReadOnlyList<(string Label, IReadOnlyDictionary<string, double> Values)> frames, Color low, Color high)
    {
        ArgumentNullException.ThrowIfNull(frames);

        var list = new List<ParticleGlobeTimelineFrame>(frames.Count);
        foreach (var (label, values) in frames)
        {
            if (values == null) continue;
            var copy = new Dictionary<string, double>(values.Count, StringComparer.OrdinalIgnoreCase);
            foreach (var pair in values)
            {
                if (!string.IsNullOrEmpty(pair.Key) && double.IsFinite(pair.Value)) copy[pair.Key] = pair.Value;
            }
            list.Add(new ParticleGlobeTimelineFrame { Label = label ?? string.Empty, Values = copy });
        }

        lock (_valLock)
        {
            _tlFrames = list.Count > 0 ? list : null;
            _tlCount = list.Count;
            _valRaw = null;
            _valLowColor = low;
            _valHighColor = high;
            _valDirty = true;
        }

        _tlPos = Math.Clamp(_tlPos, 0, Math.Max(0, list.Count - 1));
        _tlPlaying = false;
        _tlLastIndex = -1;
    }

    public void ClearCountryValueTimeline() => ClearCountryValues();

    private bool ResolveTimeline()
    {
        List<ParticleGlobeTimelineFrame> frames;
        lock (_valLock)
        {
            frames = _tlFrames;
        }
        if (frames == null) return false;

        var map = _codeToId;
        var entries = _countries;
        _valResolvedFor = map;

        double min = double.MaxValue, max = double.MinValue;
        var used = new SortedSet<int>();
        var resolved = new List<Dictionary<int, double>>(frames.Count);
        foreach (var f in frames)
        {
            var d = new Dictionary<int, double>();
            foreach (var pair in f.Values)
            {
                if (map.TryGetValue(pair.Key, out int id) && id > 0 && id < entries.Length)
                {
                    d[id] = pair.Value;
                    used.Add(id);
                    min = Math.Min(min, pair.Value);
                    max = Math.Max(max, pair.Value);
                }
            }
            resolved.Add(d);
        }

        if (used.Count == 0)
        {
            _values = null;
            _tlT = null;
            return true;
        }

        double span = max - min;
        var tl = new float[frames.Count][];
        for (int i = 0; i < frames.Count; i++)
        {
            var arr = new float[entries.Length];
            Array.Fill(arr, float.NaN);
            foreach (var pair in resolved[i]) arr[pair.Key] = span <= 0 ? 1f : (float)((pair.Value - min) / span);
            tl[i] = arr;
        }

        var ids = new int[used.Count];
        used.CopyTo(ids);

        var set = BuildValueSet(ids, new float[entries.Length]);
        Array.Fill(set.T, float.NaN);
        _tlT = tl;
        _values = set;
        _tlApplied = false;
        ApplyTimeline();
        return true;
    }

    private ParticleGlobeValueSet BuildValueSet(int[] ids, float[] t)
    {
        var entries = _countries;
        var set = new ParticleGlobeValueSet
        {
            T = t,
            Low = ToRgb(_valLowColor),
            High = ToRgb(_valHighColor),
            BarIds = ids,
            BarDir = new System.Numerics.Vector3[ids.Length],
            BarE1 = new System.Numerics.Vector3[ids.Length],
            BarE2 = new System.Numerics.Vector3[ids.Length]
        };

        for (int i = 0; i < ids.Length; i++)
        {
            var e = entries[ids[i]];
            float cl = MathF.Cos(e.LatRad);
            var dir = new System.Numerics.Vector3(cl * MathF.Sin(e.LonRad), MathF.Sin(e.LatRad), cl * MathF.Cos(e.LonRad));
            var up = MathF.Abs(dir.Y) > 0.95f ? System.Numerics.Vector3.UnitX : System.Numerics.Vector3.UnitY;
            var e1 = System.Numerics.Vector3.Normalize(System.Numerics.Vector3.Cross(dir, up));
            set.BarDir[i] = dir;
            set.BarE1[i] = e1;
            set.BarE2[i] = System.Numerics.Vector3.Cross(dir, e1);
        }
        return set;
    }

    private void ApplyTimeline()
    {
        var tl = _tlT;
        var set = _values;
        if (tl == null || set == null || tl.Length == 0) return;

        int last = tl.Length - 1;
        double pos = Math.Clamp(_tlPos, 0, last);
        int i0 = (int)Math.Floor(pos);
        int i1 = Math.Min(i0 + 1, last);
        float f = (float)(pos - i0);
        var a = tl[i0];
        var b = tl[i1];

        foreach (int id in set.BarIds)
        {
            float va = a[id], vb = b[id];
            set.T[id] = float.IsNaN(va) ? vb : float.IsNaN(vb) ? va : va + (vb - va) * f;
        }
    }

    private void AdvanceTimeline(float dt)
    {
        if (_tlT == null || _values == null) return;

        bool changed = !_tlApplied;
        double prev = _tlPos;

        if (_tlPlaying)
        {
            double last = _tlCount - 1;
            double pos = _tlPos + _tlSpeed * dt;
            if (pos >= last)
            {
                if (_tlLoop) pos = last > 0 ? pos % last : 0;
                else
                {
                    pos = last;
                    _tlPlaying = false;
                }
            }
            _tlPos = pos;
        }

        if (_tlPos != prev) changed = true;
        _tlApplied = true;

        if (changed) ApplyTimeline();

        _tlEventTimer += dt;
        int index = (int)Math.Round(_tlPos);
        if (changed && (_tlEventTimer >= 0.05f || !_tlPlaying || index != _tlLastIndex))
        {
            _tlEventTimer = 0f;
            _tlLastIndex = index;
            var handler = TimelineChanged;
            if (handler != null)
            {
                string label = TimelineLabel;
                DispatcherQueue?.TryEnqueue(() => handler(this, label));
            }
        }
    }
}
