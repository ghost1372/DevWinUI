namespace DevWinUIGallery.Views;

public sealed partial class ShowMorePage : Page
{
    public string LongText { get; } =
        "This release focuses on performance and polish. Startup time has been reduced by lazy-loading rarely used modules, and the rendering pipeline now batches updates so animations stay smooth on lower-end hardware. " +
        "Several controls received accessibility improvements, including better narrator announcements, clearer focus visuals and full keyboard navigation. " +
        "Theming is more consistent across light, dark and high contrast modes, with every brush resolved from theme dictionaries instead of hard-coded values. " +
        "We also fixed a number of layout issues that appeared when text scaling was set above 200 percent, and resolved several memory leaks that occurred when controls were removed from the visual tree while animations were still running. " +
        "Finally, the documentation has been rewritten with runnable samples for every control, and the gallery now remembers your last visited page between sessions. " +
        "Thank you to everyone who reported issues and contributed pull requests; your feedback shapes every release.";

    public ShowMorePage()
    {
        InitializeComponent();
    }

    private int GetLines(double value) => (int)value;
}
