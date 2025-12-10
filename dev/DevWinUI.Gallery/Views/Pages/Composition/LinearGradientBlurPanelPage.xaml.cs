using DevWinUI;
using Windows.Foundation;

namespace DevWinUIGallery.Views;

public sealed partial class LinearGradientBlurPanelPage : Page
{
    public LinearGradientBlurPanelPage()
    {
        InitializeComponent();

        PointRadioButtons.ItemsSource = new[]
        {
            new PointModel(new Point(0,0), new Point(0,1)),
            new PointModel(new Point(0,0), new Point(1,1)),
            new PointModel(new Point(0,0), new Point(1,0)),
            new PointModel(new Point(0,1), new Point(1,0)),
            new PointModel(new Point(0,1), new Point(0,0)),
            new PointModel(new Point(1,1), new Point(0,0)),
            new PointModel(new Point(1,0), new Point(0,0)),
        };
    }
    private void StartAnimationButton_Click(object sender, RoutedEventArgs e)
    {
        ((Button)sender).IsEnabled = false;

        var compositor = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(this).Compositor;

        var scope = compositor.CreateScopedBatch(Microsoft.UI.Composition.CompositionBatchTypes.Animation);

        var animation = compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(0f, 64f);
        animation.InsertKeyFrame(0.5f, 0.5f);
        animation.InsertKeyFrame(1f, 64f);
        animation.Duration = TimeSpan.FromSeconds(2);

        LinearGradientBlurPanelSample.StartMaxBlurAmountAnimation(animation);

        scope.End();

        scope.Completed += (s, a) =>
        {
            ((Button)sender).IsEnabled = true;
        };
    }
    private record class PointModel(Point StartPoint, Point EndPoint)
    {
        public override string ToString()
        {
            return $"({StartPoint.X}, {StartPoint.Y}) --> ({EndPoint.X}, {EndPoint.Y})";
        }
    }
}
