namespace DevWinUIGallery.Views;

public sealed partial class CredentialHelperPage : Page
{
    public CredentialHelperPage()
    {
        this.InitializeComponent();
    }

    private async void btnRequestOSPin_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var result = await CredentialHelper.RequestWindowsPIN(txtRequestOSPin.Text);
        txtRequestOSPinResult.Text = result.ToString();
    }

    private async void btnPickCredential_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var result = await CredentialHelper.PickCredential(txtPickCredentialCaption.Text, txtPickCredentialMessage.Text);
        txtPickCredentialResult.Text = $"Username: {result.CredentialUserName} | Password: {result.CredentialPassword}";
    }

    private void btnAddPassword_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CredentialHelper.AddPasswordCredential(txtResource.Text, txtUsername.Text, txtPassword.Text);
    }

    private void btnGetPassword_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var result = CredentialHelper.GetPasswordCredential(txtResource.Text, txtUsername.Text);
        txtGetPasswordResult.Text = $"Resource: {result?.Resource} | Username: {result?.UserName} | Password: {result?.UserName}";
    }

    private void btnRemovePassword_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        CredentialHelper.RemovePasswordCredential(txtResource.Text, txtUsername.Text);
    }
}
