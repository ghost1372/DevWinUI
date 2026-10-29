using System.Buffers;
using Microsoft.UI.Xaml.Documents;

namespace DevWinUI;

internal static partial class IntraLineDiff
{
    private const int MaxLength = 2000;
    private const int MaxCells = 40_000;
    private const double MinSimilarity = 0.3;

    public static List<TextRange>? RangesFor(DiffLine line, DiffLine other)
    {
        bool isRemove = line.Kind == DiffKind.Remove;
        string oldText = DiffRow.ToDisplay(isRemove ? line.Text : other.Text);
        string newText = DiffRow.ToDisplay(isRemove ? other.Text : line.Text);

        if (oldText.Length > MaxLength || newText.Length > MaxLength) return null;
        if (!TryCompare(oldText, newText, out List<TextRange> oldRanges, out List<TextRange> newRanges)) return null;

        return isRemove ? oldRanges : newRanges;
    }

    private static bool TryCompare(string a, string b, out List<TextRange> aRanges, out List<TextRange> bRanges)
    {
        aRanges = new List<TextRange>();
        bRanges = new List<TextRange>();

        List<(int Start, int Length)> ta = Tokenize(a);
        List<(int Start, int Length)> tb = Tokenize(b);
        int n = ta.Count, m = tb.Count;

        int prefix = 0;
        while (prefix < n && prefix < m && Same(a, ta[prefix], b, tb[prefix])) prefix++;

        int suffix = 0;
        while (suffix < n - prefix && suffix < m - prefix &&
               Same(a, ta[n - 1 - suffix], b, tb[m - 1 - suffix])) suffix++;

        var aMatched = new bool[n];
        var bMatched = new bool[m];
        for (int i = 0; i < prefix; i++)
        {
            aMatched[i] = true;
            bMatched[i] = true;
        }
        for (int i = 0; i < suffix; i++)
        {
            aMatched[n - 1 - i] = true;
            bMatched[m - 1 - i] = true;
        }

        int aLen = n - prefix - suffix;
        int bLen = m - prefix - suffix;
        if (aLen > 0 && bLen > 0 && (long)aLen * bLen <= MaxCells)
        {
            MatchMiddle(a, ta, b, tb, prefix, aLen, bLen, aMatched, bMatched);
        }

        if (Similarity(a, ta, aMatched, b, tb, bMatched) < MinSimilarity) return false;

        BuildRanges(a, ta, aMatched, aRanges);
        BuildRanges(b, tb, bMatched, bRanges);
        return aRanges.Count > 0 || bRanges.Count > 0;
    }

    private static void MatchMiddle(
        string a, List<(int Start, int Length)> ta,
        string b, List<(int Start, int Length)> tb,
        int offset, int aLen, int bLen,
        bool[] aMatched, bool[] bMatched)
    {
        int w = bLen + 1;
        int size = (aLen + 1) * w;
        int[] dp = ArrayPool<int>.Shared.Rent(size);
        Array.Clear(dp, 0, size);

        try
        {
            for (int i = aLen - 1; i >= 0; i--)
            {
                for (int j = bLen - 1; j >= 0; j--)
                {
                    dp[i * w + j] = Same(a, ta[offset + i], b, tb[offset + j])
                        ? dp[(i + 1) * w + j + 1] + 1
                        : Math.Max(dp[(i + 1) * w + j], dp[i * w + j + 1]);
                }
            }

            int x = 0, y = 0;
            while (x < aLen && y < bLen)
            {
                if (Same(a, ta[offset + x], b, tb[offset + y]))
                {
                    aMatched[offset + x] = true;
                    bMatched[offset + y] = true;
                    x++;
                    y++;
                }
                else if (dp[(x + 1) * w + y] >= dp[x * w + y + 1]) x++;
                else y++;
            }
        }
        finally
        {
            ArrayPool<int>.Shared.Return(dp);
        }
    }

    private static double Similarity(
        string a, List<(int Start, int Length)> ta, bool[] aMatched,
        string b, List<(int Start, int Length)> tb, bool[] bMatched)
    {
        int matched = 0, aTotal = 0, bTotal = 0;

        for (int i = 0; i < ta.Count; i++)
        {
            if (char.IsWhiteSpace(a[ta[i].Start])) continue;
            aTotal += ta[i].Length;
            if (aMatched[i]) matched += ta[i].Length;
        }

        for (int j = 0; j < tb.Count; j++)
        {
            if (char.IsWhiteSpace(b[tb[j].Start])) continue;
            bTotal += tb[j].Length;
        }

        int max = Math.Max(aTotal, bTotal);
        return max == 0 ? 1 : (double)matched / max;
    }

    private static void BuildRanges(string text, List<(int Start, int Length)> tokens, bool[] matched, List<TextRange> ranges)
    {
        int start = -1, end = -1;

        for (int i = 0; i < tokens.Count; i++)
        {
            if (matched[i]) continue;

            (int s, int l) = tokens[i];
            if (start >= 0 && IsBlank(text, end, s))
            {
                end = s + l;
                continue;
            }

            if (start >= 0) ranges.Add(new TextRange { StartIndex = start, Length = end - start });
            start = s;
            end = s + l;
        }

        if (start >= 0) ranges.Add(new TextRange { StartIndex = start, Length = end - start });
    }

    private static bool IsBlank(string text, int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            if (!char.IsWhiteSpace(text[i])) return false;
        }
        return true;
    }

    private static bool Same(string a, (int Start, int Length) x, string b, (int Start, int Length) y)
    {
        return x.Length == y.Length && string.CompareOrdinal(a, x.Start, b, y.Start, x.Length) == 0;
    }

    private static List<(int Start, int Length)> Tokenize(string text)
    {
        var tokens = new List<(int Start, int Length)>();
        int i = 0;

        while (i < text.Length)
        {
            int start = i;
            char c = text[i];

            if (char.IsLetterOrDigit(c) || c == '_')
            {
                while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
            }
            else if (char.IsWhiteSpace(c))
            {
                while (i < text.Length && char.IsWhiteSpace(text[i])) i++;
            }
            else if (char.IsHighSurrogate(c) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
            {
                i += 2;
            }
            else
            {
                i++;
            }

            tokens.Add((start, i - start));
        }

        return tokens;
    }
}
