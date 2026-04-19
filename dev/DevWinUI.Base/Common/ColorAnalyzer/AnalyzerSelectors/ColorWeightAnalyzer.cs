namespace DevWinUI;

public partial class ColorWeightAnalyzer : ColorAnalyzerSelector
{
    public override void SelectColors(IEnumerable<AnalyzedColor> colors)
    {
        // Order by weight and ensure we have at least MinColorCount colors
        SelectedColors = colors
            .OrderByDescending(x => x.Weight)
            .Select(x => x.Color)
            .ToList()
            .EnsureMinColorCount(MinColorCount);
    }
}
