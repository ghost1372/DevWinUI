using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUIGallery.Views;
public sealed partial class SegmentCharPage : Page
{
    public SegmentCharPage()
    {
        InitializeComponent();
    }

    private void OnSixteenPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource is Shape shape)
        {
            SixSeg.ToggleSegmentState(shape);
        }
    }
    private void OnFourteenPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource is Shape shape)
        {
            FourSeg.ToggleSegmentState(shape);
        }
    }
    private void OnMatrixPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.OriginalSource is Shape shape)
        {
            MatrixSeg.ToggleSegmentState(shape);
        }
    }
    private void OnSixteenPatternChanged(object sender, SegmentEventArgs e)
    {
        TxtPattern.Text = e.Pattern;
    }
    private void OnFourteenPatternChanged(object sender, SegmentEventArgs e)
    {
        TxtPattern2.Text = e.Pattern;
    }
    private void OnMatrixPatternChanged(object sender, SegmentEventArgs e)
    {
        TxtPattern3.Text = e.Pattern;
    }
}
