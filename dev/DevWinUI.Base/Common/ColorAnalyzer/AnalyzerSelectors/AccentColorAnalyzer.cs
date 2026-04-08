namespace DevWinUI;

public partial class AccentColorAnalyzer : ColorAnalyzerSelector
{
    public override void SelectColors(IEnumerable<AnalyzedColor> palette)
    {
        SelectedColors = palette
            .Select(x => x.Color)
            .OrderByDescending(ColorExtensions.FindColorfulness)
            .ToList()
            .EnsureMinColorCount(MinColorCount);
    }
}
