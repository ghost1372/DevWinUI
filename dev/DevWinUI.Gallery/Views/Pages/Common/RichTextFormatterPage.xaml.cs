namespace DevWinUIGallery.Views;

public sealed partial class RichTextFormatterPage : Page
{
    public RichTextFormatterPage()
    {
        this.InitializeComponent();
        Loaded += RichTextFormatterPage_Loaded;
    }

    private void RichTextFormatterPage_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateTextBlock();
        UpdateRichTextBlock();
    }

    private void Txt_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateTextBlock();
    }

    private void Txt2_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateRichTextBlock();
    }

    private void UpdateTextBlock()
    {
        RichTextFormatterHelper.FormatTextBlock(Txt.Text, txtBlock);
    }

    private void UpdateRichTextBlock()
    {
        RichTextFormatterHelper.FormatRichTextBlock(Txt2.Text, txtRichBlock);
    }
}
