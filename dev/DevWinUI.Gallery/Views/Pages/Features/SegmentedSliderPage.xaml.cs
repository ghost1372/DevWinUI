namespace DevWinUIGallery.Views;

public sealed partial class SegmentedSliderPage : Page
{
    public BaseViewModel ViewModel { get; }
    public SegmentedSliderPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();

        SegmentedSliderSample.SegmentTitles = new List<string>
        {
            "Very Low",
            "Low",
            "Medium",
            "High",
            "Very High",
        };

        SegmentedSliderTimeSample.TimeSegments = new List<SegmentedSliderTimeInfo>
        {
            new SegmentedSliderTimeInfo
            {
                StartTime = TimeSpan.Zero,
                EndTime = new TimeSpan(0, 30, 0), // first 30 minutes
                Title = "Intro"
            },
            new SegmentedSliderTimeInfo
            {
                StartTime = new TimeSpan(0, 20, 0),
                EndTime = new TimeSpan(1, 0, 0), // 30-60 minutes
                Title = "Main Part"
            },
            new SegmentedSliderTimeInfo
            {
                StartTime = new TimeSpan(1, 0, 0),
                EndTime = new TimeSpan(1, 20, 0), // 60-80 minutes
                Title = "Ending"
            }
        };
    }
}
