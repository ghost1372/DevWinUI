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
        Txt_TextChanged(null, null);
        Txt2_TextChanged(null, null);
    }

    private void Txt_TextChanged(object sender, TextChangedEventArgs e)
    {
        RichTextFormatterHelper.FormatTextBlock(Txt.Text, txtBlock);
    }

    private void Txt2_TextChanged(object sender, TextChangedEventArgs e)
    {
        RichTextFormatterHelper.FormatRichTextBlock(Txt2.Text, txtRichBlock);
    }
}
