using System.Collections.Specialized;

namespace DevWinUI;

public sealed partial class Diagram
{
    public ObservableCollection<DiagramPinType> PinTypes { get; }

    private Dictionary<string, DiagramPinType>? _typeMap;
    private readonly Dictionary<string, SolidColorBrush> _typeBrushes = new(StringComparer.OrdinalIgnoreCase);

    private void OnPinTypesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _typeMap = null;
        _typeBrushes.Clear();
        BuildLegend();
        QueueRebuild();
    }

    private DiagramPinType? LookupType(string name)
    {
        _typeMap ??= PinTypes
            .Where(t => !string.IsNullOrEmpty(t.Name))
            .GroupBy(t => t.Name!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        return _typeMap.TryGetValue(name, out var t) ? t : null;
    }

    private Color TypeColor(string name)
    {
        if (LookupType(name) is { } t) return t.Color;
        if (string.IsNullOrEmpty(name) || name == "any") return Color.FromArgb(255, 0x9A, 0xA0, 0xA6);

        int h = 17;
        foreach (char ch in name.ToLowerInvariant()) h = unchecked(h * 31 + ch);
        return FromHsl(((h % 360) + 360) % 360, 0.6, 0.55);
    }

    private SolidColorBrush TypeBrush(string name)
    {
        if (!_typeBrushes.TryGetValue(name, out var b))
        {
            b = new SolidColorBrush(TypeColor(name));
            _typeBrushes[name] = b;
        }

        return b;
    }

    private static Color FromHsl(double h, double s, double l)
    {
        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        double m = l - c / 2;
        (double r, double g, double b) = h < 60 ? (c, x, 0d)
            : h < 120 ? (x, c, 0d)
            : h < 180 ? (0d, c, x)
            : h < 240 ? (0d, x, c)
            : h < 300 ? (x, 0d, c)
            : (c, 0d, x);
        return Color.FromArgb(255, (byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b + m) * 255));
    }

    private static DiagramTypeRef ResolvedType(DiagramNodeView v, DiagramPin pin) =>
        DiagramTypeRef.Subst(DiagramTypeRef.Parse(pin.Type), v.Bindings);

    private void ResolveTypes(List<DiagramNodeView> nodes, List<DiagramEdgeView> links)
    {
        foreach (var v in nodes)
        {
            v.Bindings.Clear();
            foreach (var p in v.Node.Inputs) p.Connected = false;
            foreach (var p in v.Node.Outputs) p.Connected = false;
        }

        var pinned = links.Where(l => l.FromPin != null && l.ToPin != null).ToList();
        foreach (var l in pinned)
        {
            l.FromPin!.Connected = true;
            l.ToPin!.Connected = true;
        }

        for (int pass = 0; pass < pinned.Count + 3; pass++)
        {
            int before = nodes.Sum(n => n.Bindings.Count);
            foreach (var l in pinned) Evaluate(l);
            if (nodes.Sum(n => n.Bindings.Count) == before) break;
        }

        foreach (var l in pinned)
        {
            var outer = DiagramTypeRef.Subst(DiagramTypeRef.Parse(l.FromPin!.Type), l.From.Bindings);
            l.TypeName = l.FromPin.IsExec ? "exec" : outer.IsVar ? string.Empty : outer.Name;
            if (l.Edge != null) l.Edge.Conflict = l.Conflict;
        }
    }

    private static void Evaluate(DiagramEdgeView l)
    {
        l.Conflict = null;
        var from = l.FromPin!;
        var to = l.ToPin!;

        if (from.IsExec != to.IsExec)
        {
            l.Conflict = "kind mismatch: exec vs data";
            return;
        }

        if (from.IsExec) return;

        var src = DiagramTypeRef.Subst(DiagramTypeRef.Parse(from.Type), l.From.Bindings, true);
        if (!DiagramTypeRef.TryUnify(DiagramTypeRef.Parse(to.Type), src, l.To.Bindings, out var err))
        {
            l.Conflict = $"type mismatch: {err}";
            return;
        }

        var dst = DiagramTypeRef.Subst(DiagramTypeRef.Parse(to.Type), l.To.Bindings, true);
        DiagramTypeRef.TryUnify(DiagramTypeRef.Parse(from.Type), dst, l.From.Bindings, out _);
    }
}
