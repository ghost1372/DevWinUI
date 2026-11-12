namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const float FieldRadius = 0.42f;
    private const float FieldCutoffCos = 0.78f; // cos(FieldRadius * 1.6)
    private const int MaxLinks = 8;
    private const float LinkGrow = 0.9f, LinkLife = 9f, LinkFade = 1.2f;
    private const int PacketCount = 6;

    private bool _forceField = true, _clickToConnect = true;

    // ---- cursor force field (render thread) ----
    private Vector3 _fieldCenter = Vector3.UnitZ;
    private float _fieldAmt;

    // ---- click-to-connect (render thread) ----
    private readonly List<ParticleGlobeLink> _links = new();
    private readonly List<Vector3> _packetPts = new(); // x, y, alpha
    private int _linkA;
    private float _linkPulse;

    private void UpdateForceField(float dt, bool onGlobe, Vector3 cursor)
    {
        if (!_forceField || !onGlobe)
        {
            _fieldAmt = Math.Max(0f, _fieldAmt - dt * 5f);
            return;
        }

        if (_fieldAmt <= 0.001f)
        {
            _fieldCenter = Vector3.Normalize(cursor);
        }
        else
        {
            float k = 1f - MathF.Exp(-dt * 16f);
            _fieldCenter = Vector3.Normalize(Vector3.Lerp(_fieldCenter, Vector3.Normalize(cursor), k));
        }

        _fieldAmt = Math.Min(1f, _fieldAmt + dt * 6f);
    }

    private bool TryGetCountryPoint(int id, out Vector3 point)
    {
        point = default;
        var list = _countries;
        if (id <= 0 || id >= list.Length) return false;

        var e = list[id];
        if (e.Spread <= 0f && e.LatRad == 0f && e.LonRad == 0f) return false;

        float cl = MathF.Cos(e.LatRad);
        point = new Vector3(cl * MathF.Sin(e.LonRad), MathF.Sin(e.LatRad), cl * MathF.Cos(e.LonRad));
        return true;
    }

    private void HandleConnectClick(int id)
    {
        if (!_clickToConnect) return;

        if (id <= 0 || !TryGetCountryPoint(id, out _))
        {
            _linkA = 0;
            return;
        }

        if (_linkA == 0)
        {
            _linkA = id;
            return;
        }

        if (_linkA == id)
        {
            _linkA = 0;
            return;
        }

        if (TryGetCountryPoint(_linkA, out var a) && TryGetCountryPoint(id, out var b))
        {
            float omega = MathF.Acos(Math.Clamp(Vector3.Dot(a, b), -1f, 1f));
            if (omega > 0.03f && omega < MathF.PI - 0.03f)
            {
                if (_links.Count >= MaxLinks) _links.RemoveAt(0);
                _links.Add(new ParticleGlobeLink
                {
                    A = a,
                    B = b,
                    Path = new ParticleGlobeArcState { A = a, B = b, Omega = omega, Height = 0.12f + 0.3f * (omega / MathF.PI) }
                });
            }
        }
        _linkA = 0;
    }

    private void AdvanceLinks(float dt)
    {
        _linkPulse += dt;
        for (int i = _links.Count - 1; i >= 0; i--)
        {
            _links[i].Age += dt;
            if (_links[i].Age >= LinkLife) _links.RemoveAt(i);
        }

        if (!_clickToConnect)
        {
            _links.Clear();
            _linkA = 0;
        }
    }

    private void DrawMarker(CanvasDrawingSession ds, Vector3 world, float amt, float phase)
    {
        var r = Rot(world * 1.004f);
        if (r.Z < 0.02f) return;

        var p = Proj(r);
        float fade = MathF.Min(1f, r.Z * 3f) * amt;
        float pulse = 0.5f + 0.5f * MathF.Sin(phase * 5f);
        float rad = _radius * (0.025f + 0.02f * pulse);
        ds.DrawCircle(p, rad, ToColor(_arc, 0.9f * fade), 1.5f);
        ds.DrawCircle(p, rad * 1.8f, ToColor(_arc, 0.35f * fade * (1f - pulse)), 1f);
    }

    private void DrawLinks(CanvasDrawingSession ds)
    {
        _packetPts.Clear();
        if (!_clickToConnect) return;

        const int Seg = 48;

        if (_linkA != 0 && TryGetCountryPoint(_linkA, out var pending))
        {
            DrawMarker(ds, pending, 1f, _linkPulse);
        }

        foreach (var l in _links)
        {
            float fade = l.Age > LinkLife - LinkFade ? (LinkLife - l.Age) / LinkFade : 1f;
            float grow = Math.Clamp(l.Age / LinkGrow, 0f, 1f);
            grow = 1f - (1f - grow) * (1f - grow);

            Vector2 prev = default;
            bool havePrev = false;
            for (int k = 0; k <= Seg; k++)
            {
                float u = grow * k / Seg;
                var r = Rot(ArcPoint(l.Path, u));
                if (Occluded(r)) { havePrev = false; continue; }

                var p = Proj(r);
                if (havePrev && Jumped(prev, p)) havePrev = false;
                if (havePrev)
                {
                    ds.DrawLine(prev, p, ToColor(_arc, 0.18f * fade), 4f);
                    ds.DrawLine(prev, p, ToColor(_arc, 0.7f * fade), 1.4f);
                }
                prev = p;
                havePrev = true;
            }

            DrawMarker(ds, l.A, fade, _linkPulse);
            if (grow >= 1f) DrawMarker(ds, l.B, fade, _linkPulse + 1.5f);

            if (l.Age < LinkGrow * 0.6f) continue;

            // packets stream A -> B
            float flow = (l.Age - LinkGrow * 0.6f) * 0.42f;
            for (int i = 0; i < PacketCount; i++)
            {
                float u = (flow + i / (float)PacketCount) % 1f;
                for (int tr = 0; tr < 4; tr++)
                {
                    float ut = u - tr * 0.012f;
                    if (ut < 0f) break;

                    var r = Rot(ArcPoint(l.Path, ut));
                    if (Occluded(r)) continue;

                    var p = Proj(r);
                    float edge = MathF.Min(1f, MathF.Min(u, 1f - u) * 12f);
                    _packetPts.Add(new Vector3(p.X, p.Y, fade * edge * (1f - tr * 0.25f)));
                }
            }
        }
    }

    private void DrawPackets(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost)
    {
        if (_packetPts.Count == 0) return;

        var halo = new Vector3(_arc.X, _arc.Y, _arc.Z);
        var core = Vector3.Lerp(halo, Vector3.One, 0.7f);
        foreach (var h in _packetPts)
        {
            var pos = new Vector2(h.X, h.Y);
            batch.Draw(_dot, pos, Tint(halo, 0.7f * h.Z), origin, 0f, new Vector2(sizeBoost * 13f), CanvasSpriteFlip.None);
            batch.Draw(_dot, pos, Tint(core, h.Z), origin, 0f, new Vector2(sizeBoost * 4.5f), CanvasSpriteFlip.None);
        }
    }
}
