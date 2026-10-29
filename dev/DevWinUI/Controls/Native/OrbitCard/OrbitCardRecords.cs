namespace DevWinUI;

public sealed record OrbitCardItem(
    string Name,
    string? Role = null,
    string? Description = null,
    Uri? Image = null,
    string? Initials = null,
    string? Stat = null,
    Uri? Href = null)
{
    internal string GetInitials()
    {
        if (!string.IsNullOrEmpty(Initials)) return Initials;

        var parts = (Name ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..1].ToUpperInvariant(),
            _ => (parts[0][..1] + parts[^1][..1]).ToUpperInvariant(),
        };
    }
}

public sealed record OrbitCardLabels(string Group = "Cards", string Link = "Open");

internal readonly record struct OrbitPose(double X, double Y, double Rot, double Scale);
