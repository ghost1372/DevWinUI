using System.Security.Cryptography;

namespace DevWinUI;

public partial class PasswordGenerator
{
    private static readonly Dictionary<PasswordCharacterSet, string> _charPools =
        new()
        {
            { PasswordCharacterSet.UpperCase,   "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
            { PasswordCharacterSet.LowerCase,   "abcdefghijklmnopqrstuvwxyz" },
            { PasswordCharacterSet.Numbers,     "0123456789" },
            { PasswordCharacterSet.Math,        "+-*/<=>" },
            { PasswordCharacterSet.Punctuation, "!,.:;?" },
            { PasswordCharacterSet.Brackets,    "()[]{}" },
            { PasswordCharacterSet.Quotes,      "\"'`" },
            { PasswordCharacterSet.Special,     "#&$%@\\~^_|-" },
            { PasswordCharacterSet.Space,       " " }
        };

    private static readonly string _ambiguousChars = "O0l1I|";

    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public string Generate(PasswordOptions options)
    {
        if (options.Length <= 0)
            throw new ArgumentException("Password length must be greater than 0.");

        var selectedSets = _charPools
            .Where(kvp => options.CharacterSets.HasFlag(kvp.Key))
            .ToList();

        if (!selectedSets.Any())
            throw new ArgumentException("At least one character set must be selected.");

        var availableChars = string.Concat(selectedSets.Select(s => s.Value));

        if (options.ExcludeAmbiguousCharacters)
        {
            availableChars = new string(availableChars.Where(c => !_ambiguousChars.Contains(c)).ToArray());
        }

        if (options.ReadablePassword)
        {
            availableChars = new string(availableChars.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }

        if (!options.AllowRepeatingCharacters && options.Length > availableChars.Length)
            throw new ArgumentException("Not enough unique characters to satisfy length without repetition.");

        var passwordChars = new List<char>();

        if (options.EnsureEachSelectedSet)
        {
            foreach (var set in selectedSets)
            {
                var pool = set.Value;
                if (options.ExcludeAmbiguousCharacters)
                    pool = new string(pool.Where(c => !_ambiguousChars.Contains(c)).ToArray());

                if (options.ReadablePassword)
                    pool = new string(pool.Where(c => char.IsLetterOrDigit(c)).ToArray());

                passwordChars.Add(GetRandomChar(pool));
            }
        }

        while (passwordChars.Count < options.Length)
        {
            var ch = GetRandomChar(availableChars);
            if (!options.AllowRepeatingCharacters && passwordChars.Contains(ch))
                continue;

            passwordChars.Add(ch);
        }

        return Shuffle(passwordChars);
    }

    private static char GetRandomChar(string source)
    {
        byte[] buffer = new byte[4];
        _rng.GetBytes(buffer);
        int index = BitConverter.ToInt32(buffer, 0) & int.MaxValue;
        return source[index % source.Length];
    }

    private static string Shuffle(List<char> chars)
    {
        int n = chars.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = GetRandomInt(0, i + 1);
            var temp = chars[i];
            chars[i] = chars[j];
            chars[j] = temp;
        }
        return new string(chars.ToArray());
    }

    private static int GetRandomInt(int min, int max)
    {
        byte[] buffer = new byte[4];
        _rng.GetBytes(buffer);
        int value = BitConverter.ToInt32(buffer, 0) & int.MaxValue;
        return min + (value % (max - min));
    }

    public static PasswordStrengthLevel GetPasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
            return PasswordStrengthLevel.VeryWeak;

        int score = 0;

        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;
        if (password.Any(char.IsLower)) score++;
        if (password.Any(char.IsUpper)) score++;
        if (password.Any(char.IsDigit)) score++;
        if (password.Any(c => "!@#$%^&*()-_=+[]{};:,.<>?/\\|`~'\"".Contains(c))) score++;

        return score switch
        {
            >= 5 => PasswordStrengthLevel.VeryStrong,
            4 => PasswordStrengthLevel.Strong,
            3 => PasswordStrengthLevel.Normal,
            2 => PasswordStrengthLevel.Weak,
            _ => PasswordStrengthLevel.VeryWeak
        };
    }

    public static PasswordStrengthLevel GetPasswordStrength(string password, PasswordOptions options)
    {
        if (string.IsNullOrEmpty(password))
            return PasswordStrengthLevel.VeryWeak;

        int poolSize = 0;
        foreach (var kvp in _charPools)
        {
            if (options.CharacterSets.HasFlag(kvp.Key))
            {
                int setSize = kvp.Value.Length;

                if (options.ExcludeAmbiguousCharacters)
                    setSize -= kvp.Value.Count(c => _ambiguousChars.Contains(c));

                if (options.ReadablePassword)
                    setSize = kvp.Value.Count(c => char.IsLetterOrDigit(c));

                poolSize += setSize;
            }
        }

        if (poolSize <= 0)
            return PasswordStrengthLevel.VeryWeak;

        double entropy = password.Length * Math.Log(poolSize, 2);

        if (entropy >= 128)
            return PasswordStrengthLevel.VeryStrong;
        else if (entropy >= 60)
            return PasswordStrengthLevel.Strong;
        else if (entropy >= 36)
            return PasswordStrengthLevel.Normal;
        else if (entropy >= 28)
            return PasswordStrengthLevel.Weak;
        else
            return PasswordStrengthLevel.VeryWeak;
    }
}
