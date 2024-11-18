using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;
[TemplatePart(Name = nameof(PART_ColumnStart), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_ColumnEnd), Type = typeof(ColumnDefinition))]
[TemplatePart(Name = nameof(PART_StretchLine), Type = typeof(Line))]
[TemplatePart(Name = nameof(PART_LeftLine), Type = typeof(Line))]
[TemplatePart(Name = nameof(PART_RightLine), Type = typeof(Line))]
[TemplatePart(Name = nameof(PART_Content), Type = typeof(ContentPresenter))]
[ContentProperty(Name = nameof(Content))]
public partial class Divider : Control
{
    private string PART_ColumnStart = "PART_ColumnStart";
    private string PART_ColumnEnd = "PART_ColumnEnd";
    private string PART_StretchLine = "PART_StretchLine";
    private string PART_LeftLine = "PART_LeftLine";
    private string PART_RightLine = "PART_RightLine";
    private string PART_Content = "PART_Content";

    private ColumnDefinition _PART_ColumnStart;
    private ColumnDefinition _PART_ColumnEnd;
    private Line _PART_StretchLine;
    private Line _PART_LeftLine;
    private Line _PART_RightLine;
    private ContentPresenter _PART_Content;

    private long _horizontalContentAlignmentPropertyToken;
    private Thickness _oldContentPadding;

    public Thickness ContentPadding
    {
        get { return (Thickness)GetValue(ContentPaddingProperty); }
        set { SetValue(ContentPaddingProperty, value); }
    }

    public static readonly DependencyProperty ContentPaddingProperty =
        DependencyProperty.Register("ContentPadding", typeof(Thickness), typeof(Divider), new PropertyMetadata(new Thickness(24,0,24,0)));

    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(Divider), new PropertyMetadata(null, OnContentPropertyChanged));

    private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Divider)d;
        if (ctl != null)
        {
            ctl.UpdateContent(e.NewValue);
        }
    }

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Divider), new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

    private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Divider)d;
        if (ctl != null)
        {
            ctl.UpdateOrientation((Orientation)e.NewValue);
            ctl.UpdateHorizontalContentAlignment();
        }
    }

    public Brush LineStroke
    {
        get => (Brush)GetValue(LineStrokeProperty);
        set => SetValue(LineStrokeProperty, value);
    }

    public static readonly DependencyProperty LineStrokeProperty =
        DependencyProperty.Register(nameof(LineStroke), typeof(Brush), typeof(Divider), new PropertyMetadata(default(Brush)));

    public double LineStrokeThickness
    {
        get => (double)GetValue(LineStrokeThicknessProperty);
        set => SetValue(LineStrokeThicknessProperty, value);
    }

    public static readonly DependencyProperty LineStrokeThicknessProperty =
        DependencyProperty.Register(nameof(LineStrokeThickness), typeof(double), typeof(Divider), new PropertyMetadata(1.0));

    public DoubleCollection LineStrokeDashArray
    {
        get { return (DoubleCollection)GetValue(LineStrokeDashArrayProperty); }
        set { SetValue(LineStrokeDashArrayProperty, value); }
    }

    public static readonly DependencyProperty LineStrokeDashArrayProperty =
        DependencyProperty.Register("LineStrokeDashArray", typeof(DoubleCollection), typeof(Divider), new PropertyMetadata(default(DoubleCollection)));

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _PART_ColumnStart = GetTemplateChild(PART_ColumnStart) as ColumnDefinition;
        _PART_ColumnEnd = GetTemplateChild(PART_ColumnEnd) as ColumnDefinition;
        _PART_StretchLine = GetTemplateChild(PART_StretchLine) as Line;
        _PART_LeftLine = GetTemplateChild(PART_LeftLine) as Line;
        _PART_RightLine = GetTemplateChild(PART_RightLine) as Line;
        _PART_Content = GetTemplateChild(PART_Content) as ContentPresenter;

        UnRegisterHorizontalContentAlignmentChangedCallback();
        RegisterHorizontalContentAlignmentChangedCallback();
        UpdateOrientation(Orientation);
        UpdateContent(Content);
    }

    private void UnRegisterHorizontalContentAlignmentChangedCallback()
    {
        UnregisterPropertyChangedCallback(HorizontalContentAlignmentProperty, _horizontalContentAlignmentPropertyToken);
    }
    private void RegisterHorizontalContentAlignmentChangedCallback()
    {
        _horizontalContentAlignmentPropertyToken = RegisterPropertyChangedCallback(HorizontalContentAlignmentProperty, OnHorizontalContentAlignmentProperty);
    }

    private void OnHorizontalContentAlignmentProperty(DependencyObject sender, DependencyProperty dp)
    {
        UpdateHorizontalContentAlignment();
    }

    private void UpdateHorizontalContentAlignment()
    {
        if (_PART_ColumnStart == null || _PART_ColumnEnd == null)
        {
            return;
        }

        if (Orientation == Orientation.Vertical || Content == null || (Content is string value && string.IsNullOrEmpty(value)))
        {
            _PART_ColumnStart.Width = new GridLength(1, GridUnitType.Star);
            _PART_ColumnEnd.Width = new GridLength(1, GridUnitType.Star);
            return;
        }
        switch (HorizontalContentAlignment)
        {
            case HorizontalAlignment.Left:
                _PART_ColumnStart.Width = new GridLength(20, GridUnitType.Pixel);
                _PART_ColumnEnd.Width = new GridLength(1, GridUnitType.Star);
                break;
            case HorizontalAlignment.Right:
                _PART_ColumnStart.Width = new GridLength(1, GridUnitType.Star);
                _PART_ColumnEnd.Width = new GridLength(20, GridUnitType.Pixel);
                break;
            case HorizontalAlignment.Center:
            case HorizontalAlignment.Stretch:
                _PART_ColumnStart.Width = new GridLength(1, GridUnitType.Star);
                _PART_ColumnEnd.Width = new GridLength(1, GridUnitType.Star);
                break;
        }
    }

    private void UpdateOrientation(Orientation orientation)
    {
        if (_PART_StretchLine == null || _PART_LeftLine == null || _PART_RightLine == null || _PART_Content == null)
        {
            return;
        }
        if (orientation == Orientation.Vertical)
        {
            Margin = new Thickness(6, 0, 6, 0);
            _PART_StretchLine.Visibility = Visibility.Visible;
            _PART_LeftLine.Visibility = Visibility.Collapsed;
            _PART_RightLine.Visibility = Visibility.Collapsed;
            _PART_Content.Visibility = Visibility.Collapsed;
        }
        else
        {
            Margin = new Thickness(0, 24, 0, 24);
            _PART_StretchLine.Visibility = Visibility.Collapsed;
            _PART_LeftLine.Visibility = Visibility.Visible;
            _PART_RightLine.Visibility = Visibility.Visible;
            _PART_Content.Visibility = Visibility.Visible;
        }
    }

    private void UpdateContent(object content)
    {
        if (ContentPadding != new Thickness(0))
        {
            _oldContentPadding = ContentPadding;
        }

        if (content == null || (content is string newContent && string.IsNullOrEmpty(newContent)))
        {
            ContentPadding = new Thickness(uniformLength: 0);
        }
        else
        {
            ContentPadding = _oldContentPadding;
        }

        UpdateHorizontalContentAlignment();
    }
}
