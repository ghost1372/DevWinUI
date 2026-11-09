namespace DevWinUIGallery.Views;

public sealed partial class ProgressStepperPage : Page
{
    public ObservableCollection<ProgressStepperStepperAlign> AlignItems { get; set; } = new ObservableCollection<ProgressStepperStepperAlign>(Enum.GetValues<ProgressStepperStepperAlign>());

    public ObservableCollection<ProgressStep> Steps { get; set; } = new ObservableCollection<ProgressStep>
    {
        new()
        {
            Id = "account",
            Title = "Create account",
            Description = "Email and password"
        },
        new()
        {
            Id = "profile",
            Title = "Set up profile",
            Description = "Tell us about yourself",
            Children =
            {
                new ProgressStep { Id = "avatar", Title = "Upload avatar" },
                new ProgressStep { Id = "bio", Title = "Write a short bio" },
                new ProgressStep { Id = "links", Title = "Add social links" }
            }
        },
        new()
        {
            Id = "verify",
            Title = "Verify identity",
            Description = "Optional for personal accounts",
            Status = ProgressStepperStepStatus.Skipped
        },
        new()
        {
            Id = "plan",
            Title = "Choose a plan"
        },
        new()
        {
            Id = "done",
            Title = "All done"
        }
    };

    public ProgressStepperPage()
    {
        InitializeComponent();
    }

    private void OnNext(object sender, RoutedEventArgs e) => Stepper.Next();

    private void OnBack(object sender, RoutedEventArgs e) => Stepper.Previous();

    private void OnReset(object sender, RoutedEventArgs e) => Stepper.Reset();
}
