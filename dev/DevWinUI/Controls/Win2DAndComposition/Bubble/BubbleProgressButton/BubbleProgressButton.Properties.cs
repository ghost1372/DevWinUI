// https://github.com/DinoChan

namespace DevWinUI;

public partial class BubbleProgressButton
{
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(BubbleProgressButton), new PropertyMetadata(default));
    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(BubbleProgressState), typeof(BubbleProgressButton), new PropertyMetadata(BubbleProgressState.Idle, OnStateChanged));
    public BubbleProgressState State
    {
        get => (BubbleProgressState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly DependencyProperty CompletedBackgroundProperty =
        DependencyProperty.Register(nameof(CompletedBackground), typeof(Brush), typeof(BubbleProgressButton), new PropertyMetadata(null));
    public Brush CompletedBackground
    {
        get { return (Brush)GetValue(CompletedBackgroundProperty); }
        set { SetValue(CompletedBackgroundProperty, value); }
    }

    public static readonly DependencyProperty BubbleForegroundProperty =
        DependencyProperty.Register(nameof(BubbleForeground), typeof(Brush), typeof(BubbleProgressButton), new PropertyMetadata(null));
    public Brush BubbleForeground
    {
        get { return (Brush)GetValue(BubbleForegroundProperty); }
        set { SetValue(BubbleForegroundProperty, value); }
    }

    public static readonly DependencyProperty ProgressForegroundProperty =
        DependencyProperty.Register(nameof(ProgressForeground), typeof(Brush), typeof(BubbleProgressButton), new PropertyMetadata(null));
    public Brush ProgressForeground
    {
        get { return (Brush)GetValue(ProgressForegroundProperty); }
        set { SetValue(ProgressForegroundProperty, value); }
    }

    public static readonly DependencyProperty IsBubbingProperty =
        DependencyProperty.Register(nameof(IsBubbing), typeof(bool), typeof(BubbleProgressButton), new PropertyMetadata(true));
    public bool IsBubbing
    {
        get { return (bool)GetValue(IsBubbingProperty); }
        set { SetValue(IsBubbingProperty, value); }
    }
}
