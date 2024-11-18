using System.Windows.Input;
using Windows.Foundation;

namespace DevWinUI;

public partial class PagerControl
{
	public PagerControlDisplayMode DisplayMode
	{
		get { return (PagerControlDisplayMode)GetValue(DisplayModeProperty); }
		set { SetValue(DisplayModeProperty, value); }
	}
	public static readonly DependencyProperty DisplayModeProperty =
		DependencyProperty.Register("DisplayMode", typeof(PagerControlDisplayMode), typeof(PagerControl), new PropertyMetadata(PagerControlDisplayMode.Auto, OnPropertyChanged));

	public int NumberOfPages
	{
		get { return (int)GetValue(NumberOfPagesProperty); }
		set { SetValue(NumberOfPagesProperty, value); }
	}
	public static readonly DependencyProperty NumberOfPagesProperty =
		DependencyProperty.Register("NumberOfPages", typeof(int), typeof(PagerControl), new PropertyMetadata(0, OnPropertyChanged));

	public int SelectedPageIndex
	{
		get { return (int)GetValue(SelectedPageIndexProperty); }
		set { SetValue(SelectedPageIndexProperty, value); }
	}
	public static readonly DependencyProperty SelectedPageIndexProperty =
		DependencyProperty.Register("SelectedPageIndex", typeof(int), typeof(PagerControl), new PropertyMetadata(0, OnPropertyChanged));

	public bool ButtonPanelAlwaysShowFirstLastPageIndex
	{
		get { return (bool)GetValue(ButtonPanelAlwaysShowFirstLastPageIndexProperty); }
		set { SetValue(ButtonPanelAlwaysShowFirstLastPageIndexProperty, value); }
	}
	public static readonly DependencyProperty ButtonPanelAlwaysShowFirstLastPageIndexProperty =
		DependencyProperty.Register("ButtonPanelAlwaysShowFirstLastPageIndex", typeof(bool), typeof(PagerControl), new PropertyMetadata(true, OnPropertyChanged));

	public event TypedEventHandler<PagerControl, PagerControlSelectedIndexChangedEventArgs>? SelectedIndexChanged;


	public PagerControlButtonVisibility FirstButtonVisibility
	{
		get { return (PagerControlButtonVisibility)GetValue(FirstButtonVisibilityProperty); }
		set { SetValue(FirstButtonVisibilityProperty, value); }
	}
	public static readonly DependencyProperty FirstButtonVisibilityProperty =
		DependencyProperty.Register("FirstButtonVisibility", typeof(PagerControlButtonVisibility), typeof(PagerControl), new PropertyMetadata(PagerControlButtonVisibility.Visible, OnPropertyChanged));

	public PagerControlButtonVisibility PreviousButtonVisibility
	{
		get { return (PagerControlButtonVisibility)GetValue(PreviousButtonVisibilityProperty); }
		set { SetValue(PreviousButtonVisibilityProperty, value); }
	}
	public static readonly DependencyProperty PreviousButtonVisibilityProperty =
		DependencyProperty.Register("PreviousButtonVisibility", typeof(PagerControlButtonVisibility), typeof(PagerControl), new PropertyMetadata(PagerControlButtonVisibility.Visible, OnPropertyChanged));

	public PagerControlButtonVisibility NextButtonVisibility
	{
		get { return (PagerControlButtonVisibility)GetValue(NextButtonVisibilityProperty); }
		set { SetValue(NextButtonVisibilityProperty, value); }
	}
	public static readonly DependencyProperty NextButtonVisibilityProperty =
		DependencyProperty.Register("NextButtonVisibility", typeof(PagerControlButtonVisibility), typeof(PagerControl), new PropertyMetadata(PagerControlButtonVisibility.Visible, OnPropertyChanged));

	public PagerControlButtonVisibility LastButtonVisibility
	{
		get { return (PagerControlButtonVisibility)GetValue(LastButtonVisibilityProperty); }
		set { SetValue(LastButtonVisibilityProperty, value); }
	}
	public static readonly DependencyProperty LastButtonVisibilityProperty =
		DependencyProperty.Register("LastButtonVisibility", typeof(PagerControlButtonVisibility), typeof(PagerControl), new PropertyMetadata(PagerControlButtonVisibility.Visible, OnPropertyChanged));


	public ICommand FirstButtonCommand
	{
		get { return (ICommand)GetValue(FirstButtonCommandProperty); }
		set { SetValue(FirstButtonCommandProperty, value); }
	}
	public static readonly DependencyProperty FirstButtonCommandProperty =
		DependencyProperty.Register("FirstButtonCommand", typeof(ICommand), typeof(PagerControl), new PropertyMetadata(null));

	public ICommand PreviousButtonCommand
	{
		get { return (ICommand)GetValue(PreviousButtonCommandProperty); }
		set { SetValue(PreviousButtonCommandProperty, value); }
	}
	public static readonly DependencyProperty PreviousButtonCommandProperty =
		DependencyProperty.Register("PreviousButtonCommand", typeof(ICommand), typeof(PagerControl), new PropertyMetadata(null));

	public ICommand NextButtonCommand
	{
		get { return (ICommand)GetValue(NextButtonCommandProperty); }
		set { SetValue(NextButtonCommandProperty, value); }
	}
	public static readonly DependencyProperty NextButtonCommandProperty =
		DependencyProperty.Register("NextButtonCommand", typeof(ICommand), typeof(PagerControl), new PropertyMetadata(null));

	public ICommand LastButtonCommand
	{
		get { return (ICommand)GetValue(LastButtonCommandProperty); }
		set { SetValue(LastButtonCommandProperty, value); }
	}
	public static readonly DependencyProperty LastButtonCommandProperty =
		DependencyProperty.Register("LastButtonCommand", typeof(ICommand), typeof(PagerControl), new PropertyMetadata(null));


	public Style FirstButtonStyle
	{
		get { return (Style)GetValue(FirstButtonStyleProperty); }
		set { SetValue(FirstButtonStyleProperty, value); }
	}
	public static readonly DependencyProperty FirstButtonStyleProperty =
		DependencyProperty.Register("FirstButtonStyle", typeof(Style), typeof(PagerControl), new PropertyMetadata(null));

	public Style PreviousButtonStyle
	{
		get { return (Style)GetValue(PreviousButtonStyleProperty); }
		set { SetValue(PreviousButtonStyleProperty, value); }
	}
	public static readonly DependencyProperty PreviousButtonStyleProperty =
		DependencyProperty.Register("PreviousButtonStyle", typeof(Style), typeof(PagerControl), new PropertyMetadata(null));

	public Style NextButtonStyle
	{
		get { return (Style)GetValue(NextButtonStyleProperty); }
		set { SetValue(NextButtonStyleProperty, value); }
	}
	public static readonly DependencyProperty NextButtonStyleProperty =
		DependencyProperty.Register("NextButtonStyle", typeof(Style), typeof(PagerControl), new PropertyMetadata(null));

	public Style LastButtonStyle
	{
		get { return (Style)GetValue(LastButtonStyleProperty); }
		set { SetValue(LastButtonStyleProperty, value); }
	}
	public static readonly DependencyProperty LastButtonStyleProperty =
		DependencyProperty.Register("LastButtonStyle", typeof(Style), typeof(PagerControl), new PropertyMetadata(null));

	public string PrefixText
	{
		get { return (string)GetValue(PrefixTextProperty); }
		set { SetValue(PrefixTextProperty, value); }
	}
	public static readonly DependencyProperty PrefixTextProperty =
		DependencyProperty.Register("PrefixText", typeof(string), typeof(PagerControl), new PropertyMetadata(null));

	public string SuffixText
	{
		get { return (string)GetValue(SuffixTextProperty); }
		set { SetValue(SuffixTextProperty, value); }
	}
	public static readonly DependencyProperty SuffixTextProperty =
		DependencyProperty.Register("SuffixText", typeof(string), typeof(PagerControl), new PropertyMetadata(null));

	public PagerControlTemplateSettings TemplateSettings
	{
		get { return (PagerControlTemplateSettings)GetValue(TemplateSettingsProperty); }
		set { SetValue(TemplateSettingsProperty, value); }
	}
	public static readonly DependencyProperty TemplateSettingsProperty =
		DependencyProperty.Register("TemplateSettings", typeof(PagerControlTemplateSettings), typeof(PagerControl), new PropertyMetadata(null));



	private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
	{
		if (sender is PagerControl pagerControl)
		{
			pagerControl.OnPropertyChanged(args);
		}
	}
}
