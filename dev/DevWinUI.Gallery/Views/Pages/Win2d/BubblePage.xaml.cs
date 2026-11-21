using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Animation;

namespace DevWinUIGallery.Views;

public sealed partial class BubblePage : Page
{
    private bool _isAnimateBegin;
    private readonly Storyboard _progressStoryboard;
    public BubblePage()
    {
        this.InitializeComponent();
        _progressStoryboard = CreateStoryboard();
    }

    private void OnLikeButtonTapped(object sender, TappedRoutedEventArgs e)
    {
        if (LikeButton.State != BubbleProgressState.Completed)
            LikeButton.State = BubbleProgressState.Completed;
        else
            LikeButton.State = BubbleProgressState.Idle;
    }


    private void OnGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BubbleProgressButton progressButton)
            return;

        if (progressButton.State == BubbleProgressState.Idle)
            progressButton.State = BubbleProgressState.Completed;
        else
            progressButton.State = BubbleProgressState.Idle;
    }


    private void OnGestureRecognizerHolding(object sender, HoldingEventArgs e)
    {
        if (e.HoldingState == HoldingState.Started)
        {
            if (!_isAnimateBegin &&
                sender is BubbleProgressButton progressButton &&
                _progressStoryboard.Children[0] is DoubleAnimation animation)
            {
                _isAnimateBegin = true;
                animation.From = progressButton.Minimum;
                animation.To = progressButton.Maximum;
                Storyboard.SetTarget(animation, progressButton);
                _progressStoryboard.Begin();
            }
        }
        else
        {
            _isAnimateBegin = false;
            _progressStoryboard.Stop();
        }
    }


    private Storyboard CreateStoryboard()
    {
        var animation = new DoubleAnimation
        {
            EnableDependentAnimation = true,
            Duration = TimeSpan.FromSeconds(2)
        };

        Storyboard.SetTargetProperty(animation, nameof(BubbleProgressButton.Value));
        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Completed += OnProgressStoryboardCompleted;
        storyboard.FillBehavior = FillBehavior.Stop;
        return storyboard;
    }

    private void OnProgressStoryboardCompleted(object? sender, object e)
    {
        LikeButton.State = BubbleProgressState.Completed;
        CoinButton.State = BubbleProgressState.Completed;
        FavoriteButton.State = BubbleProgressState.Completed;
    }
}
