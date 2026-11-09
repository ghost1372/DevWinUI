using System.Diagnostics;

namespace DevWinUIGallery.Views;

public sealed partial class ReasoningPage : Page
{
    private CancellationTokenSource? _simulation;

    public ObservableCollection<ReasoningKind> KindItems { get; set; } = new ObservableCollection<ReasoningKind>(Enum.GetValues<ReasoningKind>());

    public Uri SampleImage { get; } = new("ms-appx:///Assets/AppIcon.png");

    public IReadOnlyList<ReasoningSource> Sources { get; } = new[]
    {
        new ReasoningSource("Windows App SDK documentation", new Uri("https://learn.microsoft.com/windows/apps/")),
        new ReasoningSource("Microsoft UI XAML repository", new Uri("https://github.com/microsoft/microsoft-ui-xaml")),
        new ReasoningSource("DevWinUI", new Uri("https://github.com/Ghost1372/DevWinUI")),
    };

    public IReadOnlyList<ReasoningRow> StepRows { get; } = new[]
    {
        new ReasoningRow("Read the control source"),
        new ReasoningRow("List the public properties"),
        new ReasoningRow("Map each property to a pane control", ReasoningStepStatus.Active),
        new ReasoningRow("Write the sample page", ReasoningStepStatus.Pending),
    };

    public IReadOnlyList<ReasoningRow> SearchRows { get; } = new[]
    {
        new ReasoningRow("Composition animations overview", ReasoningStepStatus.Done, new Uri("https://learn.microsoft.com/windows/apps/")),
        new ReasoningRow("Reducing layout passes in custom panels", ReasoningStepStatus.Done, new Uri("https://github.com/microsoft/microsoft-ui-xaml")),
        new ReasoningRow("Windows developer blog", ReasoningStepStatus.Done, new Uri("https://devblogs.microsoft.com/windows-developer/")),
    };

    public IReadOnlyList<ReasoningRow> CodingRows { get; } = new[]
    {
        new ReasoningRow("Controls/Reasoning.cs", ReasoningStepStatus.Done, null, 42, 7),
        new ReasoningRow("Controls/ReasoningRows.cs", ReasoningStepStatus.Done, null, 18, 3),
        new ReasoningRow("Themes/Generic.xaml", ReasoningStepStatus.Done, null, 96, 12),
    };

    public ReasoningPage()
    {
        InitializeComponent();
        Unloaded += (_, _) => _simulation?.Cancel();
    }

    private void OnAnimationToggled(object sender, RoutedEventArgs e)
    {
        ReasoningOptions.IsAnimationEnabled = ((ToggleSwitch)sender).IsOn;
    }

    private async void OnSimulateClick(object sender, RoutedEventArgs e)
    {
        _simulation?.Cancel();
        _simulation = new CancellationTokenSource();
        var token = _simulation.Token;

        ReasoningStep[] steps = { Step1, Step2, Step3, Step4 };
        foreach (var step in steps)
        {
            step.Status = ReasoningStepStatus.Pending;
        }

        ReasoningMain.Duration = double.NaN;
        ReasoningMain.Thinking = true;
        var clock = Stopwatch.StartNew();

        try
        {
            foreach (var step in steps)
            {
                step.Status = ReasoningStepStatus.Active;
                await Task.Delay(1400, token);
                step.Status = ReasoningStepStatus.Done;
            }

            ReasoningMain.Duration = Math.Round(clock.Elapsed.TotalSeconds);
            ReasoningMain.Thinking = false;
        }
        catch (OperationCanceledException)
        {
        }
    }
}
