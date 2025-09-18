using System.Collections.ObjectModel;
using DevWinUIGallery.Models;

namespace DevWinUIGallery.Views;
public sealed partial class DigitalSegmentPage : Page
{
    public ObservableCollection<DigitalSegmentOption> AvailableModels { get; } =
        new ObservableCollection<DigitalSegmentOption>
        {
            new DigitalSegmentOption { Name = "Sixteen Segment", Value = new SixteenSegmentChar() },
            new DigitalSegmentOption { Name = "Wide Sixteen Segment (Style)", Value = new SixteenSegmentChar() { Style = Application.Current.Resources["WideSixteenSegmentCharStyle"] as Style } },
            new DigitalSegmentOption { Name = "Fourteen Segment", Value = new FourteenSegmentChar() },
            new DigitalSegmentOption { Name = "Wide Fourteen Segment (Style)", Value = new FourteenSegmentChar() { Style = Application.Current.Resources["WideFourteenSegmentCharStyle"] as Style } },
            new DigitalSegmentOption { Name = "Matrix5x7 Segment", Value = new Matrix5x7SegmentChar() },
            new DigitalSegmentOption { Name = "Matrix5x8 Segment", Value = new Matrix5x8SegmentChar() },
            new DigitalSegmentOption { Name = "Matrix8x14 Segment", Value = new Matrix8x14SegmentChar() }
        };

    public BaseViewModel ViewModel { get; }
    public DigitalSegmentPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }
}
