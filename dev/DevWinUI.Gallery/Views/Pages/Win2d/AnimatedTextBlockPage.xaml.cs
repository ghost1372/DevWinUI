using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class AnimatedTextBlockPage : Page
{
    private readonly string[] SampleText = new[]
    {
            "Fly me to the 🌕moon",
            "And let me play among the 🌟stars",
            "Let me see what spring is like on",
            "Jupiter and Mars",
            "In other words, hold my hand",
            "In other words, darling, kiss me",
            "Fill my heart with 🎶song",
            "And let me sing forevermore",
            "You are all I long for",
            "All I worship and adore",
            "In other words, please be true",
            "In other words, I ❤️love you",
            "故天将降大任于斯人也",
            "必先苦其心志",
            "劳其筋骨",
            "饿其体肤",
            "空乏其身",
            "行拂乱其所为",
            "所以动心忍性",
            "曾益其所不能"
        };
    public List<BuiltInEffect> BuiltInEffects => new List<BuiltInEffect>()
    {
        new BuiltInEffect("Default", new TextDefaultEffect()),
        new BuiltInEffect("Motion Blur", new TextMotionBlurEffect()),
        new BuiltInEffect("Blur", new TextBlurEffect()),
        new BuiltInEffect("Elastic", new TextElasticEffect()),
        new BuiltInEffect("Zoom", new TextZoomEffect()),
        new BuiltInEffect("Pivot", new TextPivotEffect())
    };
    public AnimatedTextBlockPage()
    {
        InitializeComponent();
    }

    private void CmbEffects_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var effect = CmbEffects.SelectedItem.As<BuiltInEffect>();
        AnimatedTextBlockSample.TextEffect = effect.Effect;
    }
}
public partial class BuiltInEffect
{
    public string Name { get; }

    public ITextEffect Effect { get; }

    public BuiltInEffect(string name, ITextEffect effect)
    {
        Name = name;
        Effect = effect;
    }
}
