namespace DevWinUI;

internal sealed partial class DiagramTypeRef
{
    public string Name = string.Empty;
    public int Var = -1;
    public List<DiagramTypeRef> Args = new();

    public bool IsVar => Var >= 0;

    public bool IsAny => !IsVar && (Name is "any" or "*" or "");

    public override string ToString() =>
        IsVar ? $"var({Var})" : Args.Count == 0 ? Name : $"{Name}[{string.Join(", ", Args)}]";

    public static DiagramTypeRef Parse(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return new DiagramTypeRef { Name = "any" };
        int i = 0;
        return ParseOne(s, ref i);
    }

    private static DiagramTypeRef ParseOne(string s, ref int i)
    {
        while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
        int start = i;
        while (i < s.Length && s[i] is not ('[' or ']' or ',' or '(' or ')')) i++;

        var t = new DiagramTypeRef { Name = s[start..i].Trim() };

        if (i < s.Length && s[i] == '(')
        {
            int close = s.IndexOf(')', i);
            if (close < 0) close = s.Length;
            if (t.Name == "var" && int.TryParse(s[(i + 1)..close].Trim(), out int n)) t.Var = n;
            i = Math.Min(close + 1, s.Length);
            return t;
        }

        if (i < s.Length && s[i] == '[')
        {
            i++;
            while (i < s.Length)
            {
                t.Args.Add(ParseOne(s, ref i));
                while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
                if (i < s.Length && s[i] == ',')
                {
                    i++;
                    continue;
                }

                if (i < s.Length && s[i] == ']') i++;
                break;
            }
        }

        return t;
    }

    public static DiagramTypeRef Subst(DiagramTypeRef t, Dictionary<int, DiagramTypeRef> b, bool erase = false)
    {
        if (t.IsVar)
        {
            if (b.TryGetValue(t.Var, out var r)) return r;
            return erase ? new DiagramTypeRef { Name = "any" } : t;
        }

        if (t.Args.Count == 0) return t;

        var c = new DiagramTypeRef { Name = t.Name };
        foreach (var a in t.Args) c.Args.Add(Subst(a, b, erase));
        return c;
    }

    public static bool Unify(DiagramTypeRef p, DiagramTypeRef c, Dictionary<int, DiagramTypeRef> b, out string? err)
    {
        err = null;
        p = Subst(p, b);

        if (p.IsVar)
        {
            if (!c.IsAny && !c.IsVar) b[p.Var] = c;
            return true;
        }

        if (p.IsAny || c.IsAny || c.IsVar) return true;

        if (!string.Equals(p.Name, c.Name, StringComparison.OrdinalIgnoreCase) || p.Args.Count != c.Args.Count)
        {
            err = $"{p} vs {c}";
            return false;
        }

        for (int i = 0; i < p.Args.Count; i++)
            if (!Unify(p.Args[i], c.Args[i], b, out err)) return false;

        return true;
    }

    public static bool TryUnify(DiagramTypeRef p, DiagramTypeRef c, Dictionary<int, DiagramTypeRef> b, out string? err)
    {
        var tmp = new Dictionary<int, DiagramTypeRef>(b);
        if (!Unify(p, c, tmp, out err)) return false;

        foreach (var kv in tmp) b[kv.Key] = kv.Value;
        return true;
    }
}
