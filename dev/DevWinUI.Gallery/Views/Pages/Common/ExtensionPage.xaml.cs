using Microsoft.UI.Xaml.Media;

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

    private async void OnFadeIn(object sender, RoutedEventArgs e)
    {
        await SampleGrid.AnimateDoublePropertyAsync("Opacity", 1.0, 0.0, 500);
        await SampleGrid.AnimateDoublePropertyAsync("Opacity", 0.0, 1.0, 500);
    }

    private async void OnAnimateWidth(object sender, RoutedEventArgs e)
    {
        await SampleGrid.AnimateDoublePropertyAsync("Width", 200.0, 400.0, 500);
        await SampleGrid.AnimateDoublePropertyAsync("Width", 400.0, 200.0, 500);
    }

    private async void OnAnimateHeight(object sender, RoutedEventArgs e)
    {
        await SampleGrid.AnimateDoublePropertyAsync("Height", 200.0, 400.0, 500);
        await SampleGrid.AnimateDoublePropertyAsync("Height", 400.0, 200.0, 500);
    }

    private async void OnGrow(object sender, RoutedEventArgs e)
    {
        SampleGrid.AnimateDoubleProperty("Width", 200.0, 250.0, 300);
        SampleGrid.AnimateDoubleProperty("Height", 200.0, 250.0, 300);
        await Task.Delay(300);
        SampleGrid.AnimateDoubleProperty("Width", 250.0, 150.0, 300);
        SampleGrid.AnimateDoubleProperty("Height", 250.0, 150.0, 300);
        await Task.Delay(300);
        SampleGrid.AnimateDoubleProperty("Width", 150.0, 200.0, 300);
        SampleGrid.AnimateDoubleProperty("Height", 150.0, 200.0, 300);
    }

    private async void OnRotate(object sender, RoutedEventArgs e)
    {
        RotateTransform rotateTransform = new RotateTransform() { CenterX = 100, CenterY = 100 };
        SampleGrid.RenderTransform = rotateTransform;
        await rotateTransform.AnimateDoublePropertyAsync("Angle", 0.0, -360.0, 1000);
    }

    private async void OnTranslate(object sender, RoutedEventArgs e)
    {
        TranslateTransform translateTransform = new TranslateTransform();
        SampleGrid.RenderTransform = translateTransform;
        translateTransform.AnimateDoubleProperty("X", 0.0, 100.0, 300);
        translateTransform.AnimateDoubleProperty("Y", 0.0, 100.0, 300);
        await Task.Delay(500);
        translateTransform.AnimateDoubleProperty("X", 100.0, 0.0, 300);
        translateTransform.AnimateDoubleProperty("Y", 100.0, 0.0, 300);
    }

    private async void OnSkew(object sender, RoutedEventArgs e)
    {
        SkewTransform skewTransform = new SkewTransform() { CenterX = 100, CenterY = 100 };
        SampleGrid.RenderTransform = skewTransform;
        await skewTransform.AnimateDoublePropertyAsync("AngleX", 0.0, 20.0, 300);
        await skewTransform.AnimateDoublePropertyAsync("AngleX", 20.0, -20.0, 600);
        await skewTransform.AnimateDoublePropertyAsync("AngleX", -20.0, 0.0, 300);
        await skewTransform.AnimateDoublePropertyAsync("AngleY", 0.0, 20.0, 300);
        await skewTransform.AnimateDoublePropertyAsync("AngleY", 20.0, -20.0, 600);
        await skewTransform.AnimateDoublePropertyAsync("AngleY", -20.0, 0.0, 300);
    }
}
