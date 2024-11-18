namespace DevWinUIGallery.Views;
public sealed partial class InlineAutoCompleteTextBoxPage : Page
{
    public InlineAutoCompleteTextBoxPage()
    {
        this.InitializeComponent();
        UpdateDemoSuggestions();
    }

    private List<string> DemoSuggestions { get; } = new();

    private void UpdateDemoSuggestions()
    {
        DemoSuggestions.Clear();

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        string[]? additionals = AdditionalSuggestions.Text.Split('\u002C');
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        Random random = new();

        foreach (string item in additionals)
        {
            int index = random.Next(0, DemoSuggestions.Count);
            DemoSuggestions.Insert(index, item);
        }
    }

    private void UpdateDemoSuggestionsButton_Click(object sender, RoutedEventArgs e)
    {
        UpdateDemoSuggestions();
    }
}
