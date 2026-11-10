namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public event EventHandler<ParticleGlobeCountryEventArgs> CountryClicked;
    public event EventHandler<ParticleGlobeSelectionChangedEventArgs> SelectionChanged;

    private ParticleGlobeCountryEntry[] _countries = Array.Empty<ParticleGlobeCountryEntry>();
    private Dictionary<string, int> _codeToId = new(StringComparer.OrdinalIgnoreCase);

    private bool _countryHover = true, _showTooltip = true;
    private volatile int _selectedId;
    private string _selectedCode;

    private int _hoverId, _fadeId, _selDrawId;
    private float _hoverAmt, _fadeAmt, _selAmt;

    private bool _pointerInside, _clickPending, _pressed, _pressMoved;
    private Vector2 _pointerPos, _clickPos;
    private Windows.Foundation.Point _pressPt;

    private CanvasTextFormat _tipFormat;
    private CanvasTextLayout _tipLayout;
    private int _tipLayoutId = -1;

    private static ParticleGlobeCountryEntry CreateEntry(WorldMapCountry country)
    {
        string code = country.Code;
        string name = country.Name;
        return new ParticleGlobeCountryEntry(string.IsNullOrEmpty(code) ? name : code, name ?? code);
    }

    private ParticleGlobeCountryEntry FindEntry(string code)
    {
        if (string.IsNullOrEmpty(code) || !_codeToId.TryGetValue(code, out int id)) return null;
        var list = _countries;
        return id < list.Length ? list[id] : null;
    }

    private void ResolveSelectedId()
    {
        var code = _selectedCode;
        _selectedId = !string.IsNullOrEmpty(code) && _codeToId.TryGetValue(code, out int id) ? id : 0;
    }

    private void OnSelectedCodeChanged(string oldCode, string newCode)
    {
        _selectedCode = newCode;
        ResolveSelectedId();

        SelectionChanged?.Invoke(this, new ParticleGlobeSelectionChangedEventArgs(
            oldCode, FindEntry(oldCode)?.Name,
            newCode, FindEntry(newCode)?.Name));

        if (!_flyOnSelect || _clickToConnect) return;

        var entry = FindEntry(newCode);
        if (entry != null) FlyToEntry(entry);
        else if (string.IsNullOrEmpty(newCode)) ResetView();
    }

    private void OnCountryClicked(int id)
    {
        if (!IsCountrySelectionEnabled) return;

        var list = _countries;
        if (id <= 0 || id >= list.Length)
        {
            SelectedCountryCode = null;
            return;
        }

        var entry = list[id];
        CountryClicked?.Invoke(this, new ParticleGlobeCountryEventArgs(entry.Code, entry.Name));

        if (IsCompareModeEnabled)
        {
            HandleComparePick(id);
            return;
        }

        bool alreadySelected = string.Equals(SelectedCountryCode, entry.Code, StringComparison.OrdinalIgnoreCase);
        SelectedCountryCode = entry.Code;
        if (alreadySelected && _flyOnSelect) FlyToEntry(entry);
    }

    private void PublishHover(int id)
    {
        var list = _countries;
        string code = id > 0 && id < list.Length ? list[id].Code : null;
        DispatcherQueue.TryEnqueue(() => SetValue(HoveredCountryCodeProperty, code));
    }

    private void ResetTooltipResources()
    {
        _tipLayout?.Dispose();
        _tipLayout = null;
        _tipLayoutId = -1;
    }

    private bool TryUnproject(Vector2 point, out Vector3 world)
    {
        world = default;
        if (_radius <= 0f) return false;
        if (_morphT >= 0.999f) return TryUnprojectFlat(point, out world);
        if (_morphT > 0f) return false;

        float sx = (point.X - _center.X) / _radius;
        float sy = -(point.Y - _center.Y) / _radius;
        float a = sx * sx + sy * sy;

        float disc = 1f - a * (1f - Perspective * Perspective);
        if (disc < 0f) return false;

        float z2 = (Perspective * a + MathF.Sqrt(disc)) / (Perspective * Perspective * a + 1f);
        float f = 1f - Perspective * z2;
        float x2 = sx * f;
        float y2 = sy * f;

        float y = y2 * _cX + z2 * _sX;
        float z1 = -y2 * _sX + z2 * _cX;
        float x = x2 * _cY - z1 * _sY;
        float z = x2 * _sY + z1 * _cY;

        world = new Vector3(x, y, z);
        return true;
    }

    private int CountryAt(Vector3 world)
    {
        var ids = _mask == null ? _geoIds : null;
        return ids == null ? 0 : GeoCountryAt(ids, world.X, world.Y, world.Z);
    }

    private void UpdateCountryState(float dt)
    {
        dt = Math.Min(dt, 0.1f);

        bool inside, dragging, click;
        Vector2 pointer, clickPoint;
        lock (_inputLock)
        {
            inside = _pointerInside;
            dragging = _dragging;
            pointer = _pointerPos;
            click = _clickPending;
            clickPoint = _clickPos;
            _clickPending = false;
        }

        Vector3 cursor = default;
        bool onGlobe = inside && TryUnproject(pointer, out cursor);
        _cursorOn = onGlobe;
        if (onGlobe)
        {
            _cursorLat = MathF.Asin(Math.Clamp(cursor.Y, -1f, 1f));
            _cursorLon = MathF.Atan2(cursor.X, cursor.Z);
        }

        UpdateMarkerHover(inside, dragging, pointer);

        int target = _countryHover && onGlobe && !dragging && _hoverMarker == null ? CountryAt(cursor) : 0;
        if (target != _hoverId)
        {
            _fadeId = _hoverId;
            _fadeAmt = _hoverAmt;
            _hoverId = target;
            _hoverAmt = 0f;
            PublishHover(target);
        }

        float step = dt * 7f;
        _hoverAmt = Math.Min(1f, _hoverAmt + step);
        _fadeAmt = Math.Max(0f, _fadeAmt - step * 1.5f);
        if (_fadeAmt <= 0f) _fadeId = 0;

        int selected = _selectedId;
        if (selected != _selDrawId)
        {
            _selDrawId = selected;
            _selAmt = 0f;
        }
        _selAmt = _selDrawId != 0 ? Math.Min(1f, _selAmt + dt * 6f) : 0f;

        var entries = _countries;
        var border = _selDrawId > 0 && _selDrawId < entries.Length ? entries[_selDrawId].Border : null;
        if (_selDrawId != _olId || !ReferenceEquals(border, _olRef)) BuildOutline(_selDrawId, border);

        _reticleAmt = _showReticle && onGlobe ? Math.Min(1f, _reticleAmt + dt * 6f) : Math.Max(0f, _reticleAmt - dt * 8f);
        _lockAmt = onGlobe && _hoverId != 0 ? Math.Min(1f, _lockAmt + dt * 7f) : Math.Max(0f, _lockAmt - dt * 7f);

        AdvanceMorph(dt);
        AdvanceUserArcs(dt);
        AdvanceValues(dt);
        AdvanceRipples(dt);
        AdvanceLinks(dt);
        UpdateForceField(dt, onGlobe, cursor);

        if (click)
        {
            var marker = _showCities ? MarkerAt(clickPoint) : null;
            if (marker != null)
            {
                DispatcherQueue.TryEnqueue(() => MarkerClicked?.Invoke(this, new ParticleGlobeMarkerEventArgs(marker)));
                click = false;
            }
        }

        if (click)
        {
            Vector3 clickWorld = default;
            bool hit = TryUnproject(clickPoint, out clickWorld);
            if (hit && _rippleEnabled) SpawnRipple(clickWorld);

            int id = hit ? CountryAt(clickWorld) : 0;
            HandleConnectClick(id);
            DispatcherQueue.TryEnqueue(() => OnCountryClicked(id));
        }

        BuildRippleFrames();
    }
}
