using System.Text;

namespace DevWinUIGallery.Views;

public sealed partial class SemanticVersionPage : Page
{
    public SemanticVersionPage()
    {
        this.InitializeComponent();
    }

    private void SampleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SampleTextBlock1 == null)
        {
            return;
        }

        var item = SampleComboBox.SelectedItem as ComboBoxItem;
        ParseVersion(item?.Content?.ToString());
    }

    private void SampleTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ParseVersion(SampleTextBox.Text);
    }

    private void ParseVersion(string stringVersion)
    {
        if (SemanticVersion.TryParse(stringVersion, out SemanticVersion version))
        {
            SampleTextBlock1.Text = $"{version.HasMetadata}";

            StringBuilder metadata = new StringBuilder();
            foreach (var item in version.Metadata)
            {
                metadata.AppendLine(item);
            }
            SampleTextBlock2.Text = metadata.ToString();

            SampleTextBlock3.Text = $"{version.IsPrerelease}";

            StringBuilder preReleaseLabels = new StringBuilder();
            foreach (var item in version.PrereleaseLabels)
            {
                preReleaseLabels.AppendLine(item);
            }
            SampleTextBlock4.Text = preReleaseLabels.ToString();

            SampleTextBlock5.Text = $"{version.Major}";
            SampleTextBlock6.Text = $"{version.Minor}";
            SampleTextBlock7.Text = $"{version.Patch}";
        }
    }
}
