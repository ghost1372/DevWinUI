// LICENSE https://github.com/AndrewKeepCoding/AK.Toolkit

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;

using Windows.System;

namespace DevWinUI;
/// <summary>
/// A TextBox control that shows a suggestion "inside it self".
/// Suggestions need to be provided by the SuggestionsSource property.
/// </summary>
[TemplatePart(Name = ContentElementControlName, Type = typeof(ContentControl))]
public sealed partial class InlineAutoCompleteTextBox : TextBox
{
    /// <summary>
    /// Identifies the <see cref="IsSuggestionCaseSensitive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsSuggestionCaseSensitiveProperty =
        DependencyProperty.Register(
            nameof(IsSuggestionCaseSensitive),
            typeof(bool),
            typeof(InlineAutoCompleteTextBox),
            new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="SuggestionForeground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionForegroundProperty =
        DependencyProperty.Register(
            nameof(SuggestionForeground),
            typeof(Brush),
            typeof(InlineAutoCompleteTextBox),
            new PropertyMetadata(null, (d, e) =>
            {
                if (d is InlineAutoCompleteTextBox control && e.NewValue is Brush foreground)
                {
                    control.SuggestionControl.Foreground = foreground;
                }
            }));

    /// <summary>
    /// Identifies the <see cref="SuggestionsSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionsSourceProperty =
        DependencyProperty.Register(
            nameof(SuggestionsSource),
            typeof(IEnumerable<string>),
            typeof(InlineAutoCompleteTextBox),
            new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="SuggestionPrefix"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionPrefixProperty =
        DependencyProperty.Register(
            nameof(SuggestionPrefix),
            typeof(string),
            typeof(InlineAutoCompleteTextBox),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Identifies the <see cref="SuggestionSuffix"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SuggestionSuffixProperty =
        DependencyProperty.Register(
            nameof(SuggestionSuffix),
            typeof(string),
            typeof(InlineAutoCompleteTextBox),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Name of the control inside the TextControl that presents the input text.
    /// </summary>
    private const string ContentElementControlName = "ContentElement";

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAutoCompleteTextBox"/> class.
    /// </summary>
    public InlineAutoCompleteTextBox()
    {
        DefaultStyleKey = typeof(InlineAutoCompleteTextBox);
    }

    /// <summary>
    /// Get or sets a value indicating whether the suggestion is case sensitive.
    /// </summary>
    public bool IsSuggestionCaseSensitive
    {
        get => (bool)GetValue(IsSuggestionCaseSensitiveProperty);
        set => SetValue(IsSuggestionCaseSensitiveProperty, value);
    }

    /// <summary>
    /// Gets or sets a brush that describes the suggestion foreground color.
    /// </summary>
    public Brush SuggestionForeground
    {
        get => (Brush)GetValue(SuggestionForegroundProperty);
        set => SetValue(SuggestionForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets a collection of strings as a source of suggestions.
    /// </summary>
    public IEnumerable<string> SuggestionsSource
    {
        get => (IEnumerable<string>)GetValue(SuggestionsSourceProperty);
        set => SetValue(SuggestionsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets a prefix string for the suggestion.
    /// </summary>
    public string SuggestionPrefix
    {
        get => (string)GetValue(SuggestionPrefixProperty);
        set => SetValue(SuggestionPrefixProperty, value);
    }

    /// <summary>
    /// Gets or sets a suffix string for the suggestion.
    /// </summary>
    public string SuggestionSuffix
    {
        get => (string)GetValue(SuggestionSuffixProperty);
        set => SetValue(SuggestionSuffixProperty, value);
    }

    private Grid SuggestionGrid { get; } = new();

    private ScrollViewer SuggestionControl { get; set; } = new();

    private string LastAcceptedSuggestion { get; set; } = string.Empty;

    private Brush? SuggestionForegroundDefaultBrush { get; set; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (Resources.TryGetValue("SystemColorGrayTextColor", out var value) is true)
        {
            if (value is Color defaultColor)
            {
                var defaultBrush = new SolidColorBrush(defaultColor);
                SuggestionForegroundDefaultBrush = defaultBrush;
            }
        }

        CustomizeInnerControls();
    }

    private void CustomizeInnerControls()
    {
        if (GetTemplateChild(ContentElementControlName) is ScrollViewer inputControl &&
            VisualTreeHelper.GetParent(inputControl) is Grid rootGrid)
        {
            InitializeSuggestionControl();

            InitializeSuggestionGrid(rootGrid, inputControl);

            RegisterEvents();
        }
    }

    /// <summary>
    /// Initializes the SuggestionControl based on the ContentElement control in the default TextBox style.
    /// </summary>
    private void InitializeSuggestionControl()
    {
        AutomationProperties.SetAccessibilityView(SuggestionControl, AccessibilityView.Raw);
        SuggestionControl.HorizontalScrollBarVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(this);
        SuggestionControl.HorizontalScrollMode = ScrollViewer.GetHorizontalScrollMode(this);
        SuggestionControl.IsDeferredScrollingEnabled = ScrollViewer.GetIsDeferredScrollingEnabled(this);
        SuggestionControl.IsHorizontalRailEnabled = ScrollViewer.GetIsHorizontalRailEnabled(this);
        SuggestionControl.IsTabStop = false;
        SuggestionControl.IsVerticalRailEnabled = ScrollViewer.GetIsVerticalRailEnabled(this);
        SuggestionControl.VerticalScrollBarVisibility = ScrollViewer.GetVerticalScrollBarVisibility(this);
        SuggestionControl.VerticalScrollMode = ScrollViewer.GetVerticalScrollMode(this);
        SuggestionControl.ZoomMode = ZoomMode.Disabled;
        SuggestionControl.Margin = new Thickness(0, -1, 0, 0);
        SuggestionControl.Padding = new Thickness(0);

        if (SuggestionForeground is null && SuggestionForegroundDefaultBrush is not null)
        {
            SuggestionForeground = SuggestionForegroundDefaultBrush;
            SuggestionControl.Foreground = SuggestionForeground;
        }
    }

    /// <summary>
    /// Initializes the <see cref="SuggestionGrid"/>, a Grid that contains the ContentElement (input text) and <see cref="SuggestionControl"/> (suggestion text).
    /// </summary>
    /// <param name="rootGrid"></param>
    /// <param name="inputControl"></param>
    private void InitializeSuggestionGrid(Grid rootGrid, ScrollViewer inputControl)
    {
        SuggestionGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
        SuggestionGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
        SuggestionGrid.BorderThickness = BorderThickness;
        SuggestionGrid.ColumnSpacing = 0;
        SuggestionGrid.Padding = new Thickness(0, 0, 0, 0);

        Grid.SetRow(SuggestionGrid, Grid.GetRow(inputControl));
        Grid.SetColumn(SuggestionGrid, Grid.GetColumn(inputControl));

        var rowIndex = rootGrid.Children.IndexOf(inputControl);
        rootGrid.Children.Remove(inputControl);
        rootGrid.Children.Insert(rowIndex, SuggestionGrid);

        SuggestionGrid.Children.Add(inputControl);
        Grid.SetColumn(inputControl, 0);
        SuggestionGrid.Children.Add(SuggestionControl);
        Grid.SetColumn(SuggestionControl, 1);

        SuggestionControl.Padding = new Thickness(
            left: 0,
            inputControl.Padding.Top,
            inputControl.Padding.Right,
            inputControl.Padding.Bottom);

        inputControl.Margin = new Thickness(0, 0, 0, 0);
        inputControl.Padding = new Thickness(
            inputControl.Padding.Left,
            inputControl.Padding.Top,
            right: 0,
            inputControl.Padding.Bottom);
    }

    private void RegisterEvents()
    {
        BeforeTextChanging += (s, e) => DismissSuggestion();
        TextChanged += (s, e) => ShowSuggestion();
        LostFocus += (s, e) => DismissSuggestion();
        GotFocus += (s, e) => ShowSuggestion();
        KeyDown += (s, e) =>
        {
            if (e.Key is VirtualKey.Right)
            {
                AcceptSuggestion();
            }
        };
    }

    private string GetSuggestion()
    {
        if (Text.Length > 0 && SuggestionsSource is not null)
        {
            var result = SuggestionsSource.FirstOrDefault(
                x => x.StartsWith(
                    Text,
                    IsSuggestionCaseSensitive is false,
                    culture: null));

            if (result is not null && result.Equals(Text) is not true)
            {
                return result;
            }
        }

        return string.Empty;
    }

    private void AcceptSuggestion()
    {
        DismissSuggestion();

        var suggestion = GetSuggestion();

        if (suggestion.Length > 0)
        {
            Text = suggestion;
            LastAcceptedSuggestion = Text;
            SelectionStart = Text.Length;
        }
    }

    private void DismissSuggestion()
    {
        SuggestionControl.Visibility = Visibility.Collapsed;
        SuggestionControl.Content = string.Empty;
        LastAcceptedSuggestion = string.Empty;
    }

    private void ShowSuggestion()
    {
        var suggestion = string.Empty;

        if (LastAcceptedSuggestion.Equals(Text) is not true)
        {
            suggestion = GetSuggestion();

            if (suggestion.Length > 0)
            {
                SuggestionControl.Content = $"{SuggestionPrefix}{suggestion[Text.Length..]}{SuggestionSuffix}";
                SuggestionControl.Visibility = Visibility.Visible;
            }
        }

        if (suggestion.Length == 0)
        {
            DismissSuggestion();
        }
    }
}
