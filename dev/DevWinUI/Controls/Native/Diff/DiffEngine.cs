namespace DevWinUI;

public sealed partial class DiffResult
{
    public DiffResult(List<DiffLine> lines, bool approximate)
    {
        Lines = lines;
        Approximate = approximate;
    }

    /// <summary>Every line of both files in unified order, with real old/new line numbers.</summary>
    public List<DiffLine> Lines { get; }

    /// <summary>
    /// True when the files differed so much that the exact (minimal) diff was abandoned and the
    /// differing middle section is reported as one remove block followed by one add block.
    /// </summary>
    public bool Approximate { get; }
}

/// <summary>
/// Line diff based on Myers' O(ND) algorithm, with the tricks that make it fast on large files:
///  - identical leading and trailing lines are skipped without any diffing,
///  - lines are interned to integer ids so comparisons are int compares,
///  - the edit distance is capped (see maxEditDistance) so memory and time stay bounded,
///  - cancellation is honoured, so it is safe to run on a background thread.
/// </summary>
public static class DiffEngine
{
    /// <summary>Memory is roughly D^2 ints in the worst case, so 4000 is about 64 MB at most.</summary>
    public const int DefaultMaxEditDistance = 4000;

    public static DiffResult Compute(
        IReadOnlyList<string> oldLines,
        IReadOnlyList<string> newLines,
        int maxEditDistance = DefaultMaxEditDistance,
        CancellationToken ct = default)
    {
        int n = oldLines.Count, m = newLines.Count;
        var result = new List<DiffLine>(Math.Max(n, m));

        // 1. Common prefix / suffix.
        int prefix = 0;
        while (prefix < n && prefix < m && oldLines[prefix] == newLines[prefix]) prefix++;

        int suffix = 0;
        while (suffix < n - prefix && suffix < m - prefix &&
               oldLines[n - 1 - suffix] == newLines[m - 1 - suffix]) suffix++;

        for (int i = 0; i < prefix; i++)
        {
            if ((i & 0xFFFF) == 0) ct.ThrowIfCancellationRequested();
            result.Add(new DiffLine(DiffKind.Context, oldLines[i], i + 1, i + 1));
        }

        // 2. The differing middle.
        int aLen = n - prefix - suffix;
        int bLen = m - prefix - suffix;
        bool approximate = false;

        if (aLen == 0 || bLen == 0)
        {
            for (int i = 0; i < aLen; i++)
                result.Add(new DiffLine(DiffKind.Remove, oldLines[prefix + i], prefix + i + 1, null));
            for (int j = 0; j < bLen; j++)
                result.Add(new DiffLine(DiffKind.Add, newLines[prefix + j], null, prefix + j + 1));
        }
        else
        {
            // Intern middle lines to ints.
            var ids = new Dictionary<string, int>(StringComparer.Ordinal);
            var x = new int[aLen];
            var y = new int[bLen];
            for (int i = 0; i < aLen; i++) x[i] = Intern(ids, oldLines[prefix + i]);
            for (int j = 0; j < bLen; j++) y[j] = Intern(ids, newLines[prefix + j]);

            var ops = Myers(x, y, maxEditDistance, ct);
            if (ops == null)
            {
                approximate = true;
                for (int i = 0; i < aLen; i++)
                    result.Add(new DiffLine(DiffKind.Remove, oldLines[prefix + i], prefix + i + 1, null));
                for (int j = 0; j < bLen; j++)
                    result.Add(new DiffLine(DiffKind.Add, newLines[prefix + j], null, prefix + j + 1));
            }
            else
            {
                EmitOps(ops, oldLines, newLines, prefix, result);
            }
        }

        // 3. Common suffix.
        for (int k = 0; k < suffix; k++)
        {
            int oi = n - suffix + k, ni = m - suffix + k;
            result.Add(new DiffLine(DiffKind.Context, oldLines[oi], oi + 1, ni + 1));
        }

        PairChanges(result);
        return new DiffResult(result, approximate);
    }

    private static void PairChanges(List<DiffLine> lines)
    {
        int i = 0;
        while (i < lines.Count)
        {
            if (lines[i].Kind == DiffKind.Context)
            {
                i++;
                continue;
            }

            int removeStart = i;
            while (i < lines.Count && lines[i].Kind == DiffKind.Remove) i++;
            int addStart = i;
            while (i < lines.Count && lines[i].Kind == DiffKind.Add) i++;

            int count = Math.Min(addStart - removeStart, i - addStart);
            for (int k = 0; k < count; k++)
            {
                lines[removeStart + k] = lines[removeStart + k] with { Pair = addStart + k };
                lines[addStart + k] = lines[addStart + k] with { Pair = removeStart + k };
            }
        }
    }

    private static int Intern(Dictionary<string, int> ids, string s)
    {
        if (!ids.TryGetValue(s, out int id))
        {
            id = ids.Count;
            ids[s] = id;
        }
        return id;
    }

    private enum Op : byte { Equal, Delete, Insert }

    private readonly struct Step
    {
        public Step(Op op, int a, int b) { Op = op; A = a; B = b; }
        public readonly Op Op;
        public readonly int A; // index into x (old middle)
        public readonly int B; // index into y (new middle)
    }

    /// <summary>Returns the edit script in forward order, or null if D exceeds the cap.</summary>
    private static List<Step>? Myers(int[] x, int[] y, int maxEditDistance, CancellationToken ct)
    {
        int N = x.Length, M = y.Length;
        int max = Math.Min(N + M, Math.Max(1, maxEditDistance));
        int off = max + 2;
        var v = new int[2 * max + 5];
        var trace = new List<int[]>();
        int foundD = -1;

        v[off + 1] = 0;
        for (int d = 0; d <= max && foundD < 0; d++)
        {
            ct.ThrowIfCancellationRequested();

            // Snapshot of V before round d, covering k = -d-1 .. d+1.
            var snap = new int[2 * d + 3];
            Array.Copy(v, off - d - 1, snap, 0, snap.Length);
            trace.Add(snap);

            for (int k = -d; k <= d; k += 2)
            {
                int px;
                if (k == -d || (k != d && v[off + k - 1] < v[off + k + 1]))
                    px = v[off + k + 1];       // move down (insert)
                else
                    px = v[off + k - 1] + 1;   // move right (delete)
                int py = px - k;

                while (px < N && py < M && x[px] == y[py]) { px++; py++; }
                v[off + k] = px;

                if (px >= N && py >= M) { foundD = d; break; }
            }
        }

        if (foundD < 0) return null;

        // Backtrack from (N, M) to (0, 0).
        var rev = new List<Step>(N + M);
        int cx = N, cy = M;
        for (int d = foundD; d >= 1; d--)
        {
            var prev = trace[d];
            int k = cx - cy;
            int prevK = (k == -d || (k != d && prev[k - 1 + d + 1] < prev[k + 1 + d + 1])) ? k + 1 : k - 1;
            int prevX = prev[prevK + d + 1];
            int prevY = prevX - prevK;

            // Where the single edit lands before the trailing diagonal run.
            int midX = prevK == k - 1 ? prevX + 1 : prevX;
            int midY = prevK == k - 1 ? prevY : prevY + 1;

            while (cx > midX && cy > midY)
            {
                cx--; cy--;
                rev.Add(new Step(Op.Equal, cx, cy));
            }

            if (prevK == k - 1) rev.Add(new Step(Op.Delete, prevX, -1));
            else rev.Add(new Step(Op.Insert, -1, prevY));

            cx = prevX; cy = prevY;
        }
        while (cx > 0 && cy > 0)
        {
            cx--; cy--;
            rev.Add(new Step(Op.Equal, cx, cy));
        }

        rev.Reverse();
        return rev;
    }

    /// <summary>
    /// Writes the script into <paramref name="result"/>. Inside each run of changes, all removed
    /// lines come before all added lines, which is how people expect a diff to read.
    /// </summary>
    private static void EmitOps(
        List<Step> ops,
        IReadOnlyList<string> oldLines,
        IReadOnlyList<string> newLines,
        int baseIndex,
        List<DiffLine> result)
    {
        var removes = new List<DiffLine>();
        var adds = new List<DiffLine>();

        void Flush()
        {
            result.AddRange(removes);
            result.AddRange(adds);
            removes.Clear();
            adds.Clear();
        }

        foreach (var s in ops)
        {
            switch (s.Op)
            {
                case Op.Delete:
                    removes.Add(new DiffLine(DiffKind.Remove, oldLines[baseIndex + s.A], baseIndex + s.A + 1, null));
                    break;
                case Op.Insert:
                    adds.Add(new DiffLine(DiffKind.Add, newLines[baseIndex + s.B], null, baseIndex + s.B + 1));
                    break;
                default:
                    Flush();
                    result.Add(new DiffLine(DiffKind.Context, oldLines[baseIndex + s.A],
                        baseIndex + s.A + 1, baseIndex + s.B + 1));
                    break;
            }
        }
        Flush();
    }

    /// <summary>Splits text into lines on \n, \r\n or \r. A trailing newline does not add an empty line.</summary>
    public static List<string> SplitLines(string text)
    {
        var lines = new List<string>();
        if (string.IsNullOrEmpty(text)) return lines;

        int start = 0;
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (c == '\n' || c == '\r')
            {
                lines.Add(text.Substring(start, i - start));
                if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                start = i + 1;
            }
        }
        if (start < text.Length) lines.Add(text.Substring(start));
        return lines;
    }
}
