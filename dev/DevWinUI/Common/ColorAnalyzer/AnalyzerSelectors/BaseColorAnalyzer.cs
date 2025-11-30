namespace DevWinUI;

public partial class BaseColorAnalyzer : ColorAnalyzerSelector
{
    public override void SelectColors(IEnumerable<AnalyzedColor> palettes)
    {
        SelectedColors = palettes
            .Select(x => x.Color)
            .OrderBy(ColorExtensions.FindColorfulness)
            .ToList()
            .EnsureMinColorCount(MinColorCount);
    }
}
