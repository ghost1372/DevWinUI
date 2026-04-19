using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class Card : Control
{
    public object TitleContent
    {
        get => (object)GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }

    public static readonly DependencyProperty TitleContentProperty =
        DependencyProperty.Register(nameof(TitleContent), typeof(object), typeof(Card), new PropertyMetadata(defaultValue: null, OnVisualPropertyChanged));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(Card), new PropertyMetadata(defaultValue: null, OnVisualPropertyChanged));

    public object Content
    {
        get => (object)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(Card), new PropertyMetadata(defaultValue: null));

    public Visibility DividerVisibility
    {
        get => (Visibility)GetValue(DividerVisibilityProperty);
        set => SetValue(DividerVisibilityProperty, value);
    }
    public static readonly DependencyProperty DividerVisibilityProperty =
        DependencyProperty.Register(nameof(DividerVisibility), typeof(Visibility), typeof(Card), new PropertyMetadata(defaultValue: null));

    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Card card)
        {
            card.SetVisualStates();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        SetVisualStates();
    }

    private void SetVisualStates()
    {
        if (string.IsNullOrEmpty(Title) && TitleContent == null)
        {
            VisualStateManager.GoToState(this, "TitleGridCollapsed", true);
            DividerVisibility = Visibility.Collapsed;
        }
        else
        {
            VisualStateManager.GoToState(this, "TitleGridVisible", true);
            DividerVisibility = Visibility.Visible;
        }
    }
}
