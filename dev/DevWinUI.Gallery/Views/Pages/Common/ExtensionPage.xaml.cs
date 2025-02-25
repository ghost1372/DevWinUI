namespace DevWinUIGallery.Views;

public sealed partial class ExtensionPage : Page
{
    public ExtensionViewModel ViewModel { get; }
    public ExtensionPage()
    {
        ViewModel = App.GetService<ExtensionViewModel>();
        this.InitializeComponent();
    }

    private void PhoneNumberValidator_TextChanging(Microsoft.UI.Xaml.Controls.TextBox sender, TextBoxTextChangingEventArgs args)
    {
        ViewModel.IsPhoneNumberValid = TextBoxExtensions.GetIsValid(sender);
    }

    private void CharactValidator_TextChanging(Microsoft.UI.Xaml.Controls.TextBox sender, TextBoxTextChangingEventArgs args)
    {
        ViewModel.IsCharacterValid = TextBoxExtensions.GetIsValid(sender);
    }

    private void EmailValidator_TextChanging(Microsoft.UI.Xaml.Controls.TextBox sender, TextBoxTextChangingEventArgs args)
    {
        ViewModel.IsEmailValid = TextBoxExtensions.GetIsValid(sender);
    }

    private void DecimalValidatorForce_TextChanging(Microsoft.UI.Xaml.Controls.TextBox sender, TextBoxTextChangingEventArgs args)
    {
        ViewModel.IsDecimalValid = TextBoxExtensions.GetIsValid(sender);
    }

    private void NumberValidatorDynamic_TextChanging(Microsoft.UI.Xaml.Controls.TextBox sender, TextBoxTextChangingEventArgs args)
    {
        ViewModel.IsNumberValid = TextBoxExtensions.GetIsValid(sender);
    }
}
