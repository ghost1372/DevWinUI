using System.Globalization;

namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const float PadX = 12f, PadY = 6f;
    private const float ReadoutPadX = 10f, ReadoutPadY = 4f;

    private bool _showReticle = true, _cursorOn;
    private float _cursorLat, _cursorLon, _reticleAmt, _lockAmt;

    private CanvasTextFormat _readoutFormat;
    private Vector2 _readoutSize;

    private static string FormatLatLon(float lat, float lon)
    {
        float la = lat * 180f / MathF.PI;
        float lo = lon * 180f / MathF.PI;
        string ns = la >= 0f ? "N" : "S";
        string ew = lo >= 0f ? "E" : "W";
        return $"{MathF.Abs(la).ToString("00.00", CultureInfo.InvariantCulture)}\u00B0{ns}  {MathF.Abs(lo).ToString("000.00", CultureInfo.InvariantCulture)}\u00B0{ew}";
    }

    private void EnsureReadoutFormat(CanvasDrawingSession ds)
    {
        if (_readoutFormat != null && _readoutSize.X > 0f) return;

        _readoutFormat = new CanvasTextFormat
        {
            FontFamily = "Consolas",
            FontSize = 11f,
            WordWrapping = CanvasWordWrapping.NoWrap
        };

        using var layout = new CanvasTextLayout(ds, FormatLatLon(0f, 0f), _readoutFormat, 600f, 40f);
        _readoutSize = new Vector2((float)layout.LayoutBounds.Width, (float)layout.LayoutBounds.Height);
    }

    private void DrawPanel(CanvasDrawingSession ds, Windows.Foundation.Rect rect, float amt)
    {
        ds.FillRoundedRectangle(rect, 3f, 3f, Color.FromArgb((byte)(215 * amt), 4, 10, 24));
        ds.DrawRoundedRectangle(rect, 3f, 3f, ToColor(_accent, 0.65f * amt), 1f);
        ds.FillRectangle((float)rect.X, (float)rect.Y + 3f, 2f, (float)rect.Height - 6f, ToColor(_accent, amt));
    }

    private void DrawReticle(CanvasDrawingSession ds, Vector2 p, float t)
    {
        if (!_showReticle || _reticleAmt <= 0f) return;

        float amt = _reticleAmt;
        float lockAmt = _lockAmt;
        float ease = 1f - (1f - amt) * (1f - amt);
        float size = (18f - 6f * lockAmt) * (1f + (1f - ease) * 1.1f);
        float rot = (1f - lockAmt) * t * 0.9f;
        float cr = MathF.Cos(rot), sr = MathF.Sin(rot);
        var color = Vector4.Lerp(_accent, Vector4.One, 0.5f * lockAmt);

        Vector2 Rotate(float x, float y) => new(x * cr - y * sr, x * sr + y * cr);

        ds.DrawCircle(p, size * 0.55f, ToColor(color, 0.3f * ease), 1f);

        float arm = size * 0.45f;
        for (int sx = -1; sx <= 1; sx += 2)
        {
            for (int sy = -1; sy <= 1; sy += 2)
            {
                var corner = p + Rotate(sx * size, sy * size);
                ds.DrawLine(corner, corner + Rotate(-sx * arm, 0f), ToColor(color, 0.95f * ease), 1.5f);
                ds.DrawLine(corner, corner + Rotate(0f, -sy * arm), ToColor(color, 0.95f * ease), 1.5f);
            }
        }

        float inner = size * 0.18f, outer = size * 0.38f;
        for (int q = 0; q < 4; q++)
        {
            float ang = q * MathF.PI * 0.5f;
            var d = new Vector2(MathF.Cos(ang), MathF.Sin(ang));
            ds.DrawLine(p + d * inner, p + d * outer, ToColor(color, 0.7f * ease), 1f);
        }

        ds.FillCircle(p, 1.6f, ToColor(color, ease));

        if (lockAmt > 0f)
        {
            float pulse = 0.5f + 0.5f * MathF.Sin(t * 6f);
            ds.DrawCircle(p, size * (1.5f + 0.25f * pulse), ToColor(color, 0.35f * lockAmt * (1f - 0.5f * pulse)), 1f);
        }
    }

    private void DrawCursorHud(CanvasDrawingSession ds, float w, float h, float t)
    {
        Vector2 p;
        lock (_inputLock) { p = _pointerPos; }

        DrawReticle(ds, p, t);

        float offset = _showReticle ? 30f : 16f;
        float above = p.Y - offset * 0.7f;
        float below = p.Y + offset * 0.7f;

        if (_showReticle && _cursorOn && _reticleAmt > 0f)
        {
            EnsureReadoutFormat(ds);

            float rw = _readoutSize.X + ReadoutPadX * 2f;
            float rh = _readoutSize.Y + ReadoutPadY * 2f;

            float x = p.X + offset;
            if (x + rw > w - 4f) x = p.X - offset - rw;
            x = Math.Max(4f, x);

            float y = below;
            if (y + rh > h - 4f)
            {
                y = above - rh;
                above = y - 4f;
            }
            else
            {
                below = y + rh + 4f;
            }
            y = Math.Max(4f, y);

            DrawPanel(ds, new Windows.Foundation.Rect(x, y, rw, rh), _reticleAmt);
            ds.DrawText(FormatLatLon(_cursorLat, _cursorLon), x + ReadoutPadX, y + ReadoutPadY,
                ToColor(Vector4.Lerp(_accent, Vector4.One, 0.35f), _reticleAmt), _readoutFormat);
        }

        DrawCountryTooltip(ds, w, h, p, offset, above, below);
    }

    private void DrawCountryTooltip(CanvasDrawingSession ds, float w, float h, Vector2 pointer, float offset, float above, float below)
    {
        int id = _hoverId;
        var list = _countries;
        if (!_showTooltip || id <= 0 || id >= list.Length || _hoverAmt <= 0f) return;

        var entry = list[id];

        if (_tipFormat == null)
        {
            _tipFormat = new CanvasTextFormat
            {
                FontSize = 12f,
                WordWrapping = CanvasWordWrapping.NoWrap
            };
        }

        if (_tipLayout == null || _tipLayoutId != id)
        {
            _tipLayout?.Dispose();
            bool showCode = !string.IsNullOrEmpty(entry.Code) && entry.Code != entry.Name;
            string text = showCode ? $"{entry.Name}  \u00B7  {entry.Code}" : entry.Name;
            _tipLayout = new CanvasTextLayout(ds, text ?? string.Empty, _tipFormat, 600f, 40f);
            _tipLayoutId = id;
        }

        var bounds = _tipLayout.LayoutBounds;
        float bw = (float)bounds.Width + PadX * 2f;
        float bh = (float)bounds.Height + PadY * 2f;

        float x = pointer.X + offset;
        if (x + bw > w - 4f) x = pointer.X - offset - bw;
        x = Math.Max(4f, x);

        float y = above - bh;
        if (y < 4f) y = below;
        y = Math.Clamp(y, 4f, Math.Max(4f, h - 4f - bh));

        float amt = _hoverAmt;
        DrawPanel(ds, new Windows.Foundation.Rect(x, y, bw, bh), amt);
        ds.DrawTextLayout(_tipLayout, x + PadX, y + PadY, ToColor(Vector4.Lerp(_accent, Vector4.One, 0.6f), amt));
    }
}
