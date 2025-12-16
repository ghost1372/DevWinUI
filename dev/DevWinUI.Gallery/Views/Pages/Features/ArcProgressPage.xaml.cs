using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.Views;

public sealed partial class ArcProgressPage : Page
{
    public ObservableCollection<ArcProgressFillAnimationState> FillAnimationStateItems { get; set; } = new ObservableCollection<ArcProgressFillAnimationState>(Enum.GetValues<ArcProgressFillAnimationState>());
    public ObservableCollection<SweepDirection> SweepDirectionItems { get; set; } = new ObservableCollection<SweepDirection>(Enum.GetValues<SweepDirection>());

    public ArcProgressPage()
    {
        InitializeComponent();
    }
}
