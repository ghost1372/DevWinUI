using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = PART_FooterExpander, Type = typeof(Expander))]
[TemplatePart(Name = PART_HeaderTextPresenter, Type = typeof(TextBlock))]
[ContentProperty(Name = nameof(Content))]
public partial class OptionsPageControl : Control
{
    private const string PART_FooterExpander = "PART_FooterExpander";
    private const string PART_HeaderTextPresenter = "PART_HeaderTextPresenter";
    private Expander _footerExpander;
    private TextBlock _headerTextBlock;

    public OptionsPageControl()
    {
        DefaultStyleKey = typeof(OptionsPageControl);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _footerExpander = GetTemplateChild(PART_FooterExpander) as Expander;
        _headerTextBlock = GetTemplateChild(PART_HeaderTextPresenter) as TextBlock;

        if (_headerTextBlock != null)
        {
            if (string.IsNullOrEmpty(_headerTextBlock.Text))
            {
                _headerTextBlock.Visibility = Visibility.Collapsed;
            }
        }
    }

    private static void OnHeaderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OptionsPageControl)d;
        if (ctl != null && ctl._headerTextBlock != null)
        {
            ctl._headerTextBlock.Visibility = string.IsNullOrEmpty((string)e.NewValue) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
    private static void OnPaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OptionsPageControl)d;
        if (ctl != null)
        {
            if (e.NewValue == null)
            {
                ctl.PaneVisibility = Visibility.Collapsed;
            }
            else
            {
                ctl.PaneVisibility = Visibility.Visible;
            }
        }
    }

    private static void OnIsFooterExpanderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (OptionsPageControl)d;
        if (ctl != null && ctl._footerExpander != null)
        {
            ctl._footerExpander.IsExpanded = (bool)e.NewValue;
        }
    }
}
