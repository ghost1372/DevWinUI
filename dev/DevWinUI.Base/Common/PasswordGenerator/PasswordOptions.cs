namespace DevWinUI;

public partial class PasswordOptions
{
    public int Length { get; set; } = 12;

    public PasswordCharacterSet CharacterSets { get; set; } =
        PasswordCharacterSet.UpperCase |
        PasswordCharacterSet.LowerCase |
        PasswordCharacterSet.Numbers;

    public bool AllowRepeatingCharacters { get; set; } = true;

    public bool EnsureEachSelectedSet { get; set; } = true;

    public bool ExcludeAmbiguousCharacters { get; set; } = false;

    public bool ReadablePassword { get; set; } = false;
}
