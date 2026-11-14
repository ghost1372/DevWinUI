namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const float GeoOriginX = 2824f;
    private const float GeoOriginY = 2780f;
    private const float GeoScale = 913f;
    private const float GeoRasterScale = 0.5f;

    private byte[] _geoMask;
    private ushort[] _geoIds;
    private int _geoW, _geoH;
    private Uri _geoSource;
    private Uri _geoLoadedSource;
    private bool _geoLoaded;

    private async void ReloadGeoMask()
    {
        var device = _canvas?.Device;
        if (device == null) return;

        await LoadGeoMaskAsync(device, _geoSource);
    }

    private async Task LoadGeoMaskAsync(ICanvasResourceCreator rc, Uri source)
    {
        if (_geoMask != null && _geoLoaded && _geoLoadedSource == source)
        {
            _needsRebuild = true;
            return;
        }

        try
        {
            var data = await WorldMapData.LoadAsync(source);
            int w = Math.Max(1, (int)(data.MapSize.Width * GeoRasterScale));
            int h = Math.Max(1, (int)(data.MapSize.Height * GeoRasterScale));

            var countries = data.Countries.ToList();
            var geometries = new List<CanvasGeometry>(countries.Count);
            byte[] landBytes, idBytes;

            try
            {
                foreach (var country in countries)
                {
                    geometries.Add(BuildCountryGeometry(rc, country));
                }

                using (var target = new CanvasRenderTarget(rc, w, h, 96))
                {
                    using (var ds = target.CreateDrawingSession())
                    {
                        ds.Clear(Colors.Black);
                        ds.Transform = Matrix3x2.CreateScale(GeoRasterScale);

                        foreach (var geometry in geometries)
                        {
                            ds.FillGeometry(geometry, Colors.White);
                            ds.DrawGeometry(geometry, Colors.White, 1.5f);
                        }
                    }

                    landBytes = target.GetPixelBytes();
                }

                using (var idTarget = new CanvasRenderTarget(rc, w, h, 96))
                {
                    using (var ds = idTarget.CreateDrawingSession())
                    {
                        ds.Antialiasing = CanvasAntialiasing.Aliased;
                        ds.Clear(Colors.Black);
                        ds.Transform = Matrix3x2.CreateScale(GeoRasterScale);

                        for (int i = 0; i < geometries.Count; i++)
                        {
                            int id = i + 1;
                            var color = Windows.UI.Color.FromArgb(255, (byte)(id & 0xFF), (byte)(id >> 8), 0);
                            ds.FillGeometry(geometries[i], color);
                            ds.DrawGeometry(geometries[i], color, 1.5f);
                        }
                    }

                    idBytes = idTarget.GetPixelBytes();
                }
            }
            finally
            {
                foreach (var geometry in geometries)
                {
                    geometry.Dispose();
                }
            }

            var mask = new byte[w * h];
            var ids = new ushort[w * h];
            for (int i = 0; i < mask.Length; i++)
            {
                mask[i] = landBytes[i * 4 + 2];
                ids[i] = (ushort)(idBytes[i * 4 + 2] | (idBytes[i * 4 + 1] << 8));
            }

            var geo = await Task.Run(() => AnalyzeCountries(ids, w, h, countries.Count));

            var entries = new ParticleGlobeCountryEntry[countries.Count + 1];
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < countries.Count; i++)
            {
                var info = geo[i + 1];
                var entry = CreateEntry(countries[i]) with { LatRad = info.Lat, LonRad = info.Lon, Spread = info.Spread, Border = info.Border };
                entries[i + 1] = entry;
                if (!string.IsNullOrEmpty(entry.Code)) map[entry.Code] = i + 1;
            }

            _geoW = w;
            _geoH = h;
            _geoIds = ids;
            _countries = entries;
            _codeToId = map;
            _geoMask = mask;
            _geoLoadedSource = source;
            _geoLoaded = true;
            ResolveSelectedId();

            int selected = _selectedId;
            if (selected != 0 && _flyOnSelect) FlyToEntry(entries[selected]);
        }
        catch
        {
            _geoMask = null;
            _geoIds = null;
            _countries = Array.Empty<ParticleGlobeCountryEntry>();
            _codeToId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            _geoLoaded = false;
            ResolveSelectedId();
        }

        _needsRebuild = true;
    }

    private static CanvasGeometry BuildCountryGeometry(ICanvasResourceCreator rc, WorldMapCountry country)
    {
        var parts = new List<CanvasGeometry>(country.Shapes.Count);

        foreach (var shape in country.Shapes)
        {
            using var builder = new CanvasPathBuilder(rc);
            builder.SetFilledRegionDetermination(CanvasFilledRegionDetermination.Alternate);

            foreach (var figure in shape.Figures)
            {
                AddGeoFigure(builder, figure);
            }

            parts.Add(CanvasGeometry.CreatePath(builder));
        }

        if (parts.Count == 1)
        {
            return parts[0];
        }

        var group = CanvasGeometry.CreateGroup(rc, parts.ToArray(), CanvasFilledRegionDetermination.Winding);
        foreach (var part in parts)
        {
            part.Dispose();
        }

        return group;
    }

    private static void AddGeoFigure(CanvasPathBuilder builder, WorldMapFigureData figure)
    {
        var ops = figure.Ops;
        var pts = figure.Points;
        int p = 0;
        bool open = false;

        for (int i = 0; i < ops.Length; i++)
        {
            switch (ops[i])
            {
                case 0:
                    if (open) builder.EndFigure(CanvasFigureLoop.Closed);
                    builder.BeginFigure(pts[p], pts[p + 1]);
                    p += 2;
                    open = true;
                    break;
                case 1:
                    builder.AddLine(pts[p], pts[p + 1]);
                    p += 2;
                    break;
                case 2:
                    builder.AddCubicBezier(
                        new Vector2(pts[p], pts[p + 1]),
                        new Vector2(pts[p + 2], pts[p + 3]),
                        new Vector2(pts[p + 4], pts[p + 5]));
                    p += 6;
                    break;
                default:
                    builder.EndFigure(CanvasFigureLoop.Closed);
                    open = false;
                    break;
            }
        }

        if (open) builder.EndFigure(CanvasFigureLoop.Closed);
    }

    private static ParticleGlobeCountryGeo[] AnalyzeCountries(ushort[] ids, int w, int h, int count)
    {
        var sinLon = new float[w];
        var cosLon = new float[w];
        for (int u = 0; u < w; u++)
        {
            float lon = ((u + 0.5f) / GeoRasterScale - GeoOriginX) / GeoScale;
            sinLon[u] = MathF.Sin(lon);
            cosLon[u] = MathF.Cos(lon);
        }

        var sinLat = new float[h];
        var cosLat = new float[h];
        for (int v = 0; v < h; v++)
        {
            float lat = 2f * MathF.Atan(MathF.Exp((GeoOriginY - (v + 0.5f) / GeoRasterScale) / GeoScale)) - MathF.PI * 0.5f;
            sinLat[v] = MathF.Sin(lat);
            cosLat[v] = MathF.Cos(lat);
        }

        var sx = new double[count + 1];
        var sy = new double[count + 1];
        var sz = new double[count + 1];
        var sw = new double[count + 1];
        var borders = new List<int>[count + 1];

        for (int v = 0; v < h; v++)
        {
            float cl = cosLat[v], sl = sinLat[v], weight = cl * cl;
            for (int u = 0; u < w; u++)
            {
                int i = v * w + u;
                int id = ids[i];
                if (id == 0 || id > count) continue;

                sx[id] += cl * sinLon[u] * weight;
                sy[id] += sl * weight;
                sz[id] += cl * cosLon[u] * weight;
                sw[id] += weight;

                bool edge = (u > 0 && ids[i - 1] != id) || (u < w - 1 && ids[i + 1] != id)
                         || (v > 0 && ids[i - w] != id) || (v < h - 1 && ids[i + w] != id);
                if (edge) (borders[id] ??= new List<int>()).Add(i);
            }
        }

        var geo = new ParticleGlobeCountryGeo[count + 1];
        var mx = new float[count + 1];
        var my = new float[count + 1];
        var mz = new float[count + 1];
        for (int id = 1; id <= count; id++)
        {
            var info = new ParticleGlobeCountryGeo { Border = borders[id]?.ToArray() ?? Array.Empty<int>() };
            geo[id] = info;

            double len = Math.Sqrt(sx[id] * sx[id] + sy[id] * sy[id] + sz[id] * sz[id]);
            if (sw[id] <= 0 || len < 1e-9) continue;

            mx[id] = (float)(sx[id] / len);
            my[id] = (float)(sy[id] / len);
            mz[id] = (float)(sz[id] / len);
            info.Lat = MathF.Asin(Math.Clamp(my[id], -1f, 1f));
            info.Lon = MathF.Atan2(mx[id], mz[id]);
        }

        var acc = new double[count + 1];
        for (int v = 0; v < h; v++)
        {
            float cl = cosLat[v], sl = sinLat[v], weight = cl * cl;
            for (int u = 0; u < w; u++)
            {
                int id = ids[v * w + u];
                if (id == 0 || id > count) continue;

                float dot = cl * sinLon[u] * mx[id] + sl * my[id] + cl * cosLon[u] * mz[id];
                acc[id] += weight * 2f * (1f - dot);
            }
        }

        for (int id = 1; id <= count; id++)
        {
            if (sw[id] > 0) geo[id].Spread = (float)Math.Sqrt(acc[id] / sw[id]);
        }

        return geo;
    }

    private bool TryGeoIndex(float x, float y, float z, out int index)
    {
        float lon = MathF.Atan2(x, z);
        float lat = Math.Clamp(MathF.Asin(Math.Clamp(y, -1f, 1f)), -1.55f, 1.55f);

        float mx = GeoOriginX + GeoScale * lon;
        float my = GeoOriginY - GeoScale * MathF.Log(MathF.Tan(MathF.PI * 0.25f + lat * 0.5f));

        int u = (int)(mx * GeoRasterScale);
        int v = (int)(my * GeoRasterScale);
        if (u < 0 || v < 0 || u >= _geoW || v >= _geoH)
        {
            index = -1;
            return false;
        }

        index = v * _geoW + u;
        return true;
    }

    private bool IsGeoLand(byte[] mask, float x, float y, float z)
    {
        return TryGeoIndex(x, y, z, out int index) && mask[index] > _landThreshold * 255f;
    }

    private int GeoCountryAt(ushort[] ids, float x, float y, float z)
    {
        return TryGeoIndex(x, y, z, out int index) && index < ids.Length ? ids[index] : 0;
    }
}
