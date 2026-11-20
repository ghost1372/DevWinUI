namespace DevWinUIGallery.Views;

public sealed partial class PasswordGeneratorPage : Page
{
    private PasswordGenerator passwordGenerator = new PasswordGenerator();
    public PasswordGeneratorPage()
    {
        InitializeComponent();
    }

    private void BtnGenerate_Click(object sender, RoutedEventArgs e)
    {
        TxtPassResult.Text = passwordGenerator.Generate(new PasswordOptions
        {
            Length = (int)NBLength.Value,
            CharacterSets = GetSelectedCharacterSets(),
            ExcludeAmbiguousCharacters = TglExcludeAmb.IsOn,
            ReadablePassword = TglReadable.IsOn,
            AllowRepeatingCharacters = TglRepeat.IsOn,
            EnsureEachSelectedSet = TglEachChar.IsOn
        });
    }

    private PasswordCharacterSet GetSelectedCharacterSets()
    {
        PasswordCharacterSet sets = PasswordCharacterSet.None;

        if (ChkUpper.IsChecked == true) sets |= PasswordCharacterSet.UpperCase;
        if (ChkLower.IsChecked == true) sets |= PasswordCharacterSet.LowerCase;
        if (ChkNumbers.IsChecked == true) sets |= PasswordCharacterSet.Numbers;
        if (ChkMath.IsChecked == true) sets |= PasswordCharacterSet.Math;
        if (ChkPunctuation.IsChecked == true) sets |= PasswordCharacterSet.Punctuation;
        if (ChkBrackets.IsChecked == true) sets |= PasswordCharacterSet.Brackets;
        if (ChkQuotes.IsChecked == true) sets |= PasswordCharacterSet.Quotes;
        if (ChkSpecial.IsChecked == true) sets |= PasswordCharacterSet.Special;
        if (ChkSpaces.IsChecked == true) sets |= PasswordCharacterSet.Space;
        return sets;
    }

    private void TxtPass_TextChanged(object sender, TextChangedEventArgs e)
    {
        var result = PasswordGenerator.GetPasswordStrength(TxtPass.Text);

        switch (result)
        {
            case PasswordStrengthLevel.VeryStrong:
                InfoBarResult.Title = "Very Strong Password";
                InfoBarResult.Severity = InfoBarSeverity.Success;
                break;
            case PasswordStrengthLevel.Strong:
                InfoBarResult.Title = "Strong Password";
                InfoBarResult.Severity = InfoBarSeverity.Success;
                break;
            case PasswordStrengthLevel.Normal:
                InfoBarResult.Title = "Normal Password";
                InfoBarResult.Severity = InfoBarSeverity.Informational;
                break;
            case PasswordStrengthLevel.Weak:
                InfoBarResult.Title = "Weak Password";
                InfoBarResult.Severity = InfoBarSeverity.Warning;
                break;
            case PasswordStrengthLevel.VeryWeak:
                InfoBarResult.Title = "Very Weak Password";
                InfoBarResult.Severity = InfoBarSeverity.Error;
                break;
        }

        InfoBarResult.IsOpen = true;
    }
}
