namespace DevWinUI;

public sealed partial class Timeline2Entry
{
    private string? _avatarUrl;
    private ImageSource? _resolvedAvatar;

    public string? Date { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public ImageSource? Avatar { get; set; }

    public string? AvatarUrl
    {
        get => _avatarUrl;
        set
        {
            _avatarUrl = value;
            _resolvedAvatar = null;
        }
    }

    public object? Value { get; set; }

    public Timeline2Color? Color { get; set; }

    public Timeline2MarkerShape? Shape { get; set; }

    public object? Tag { get; set; }

    internal ImageSource? ResolveAvatar()
    {
        if (Avatar is not null)
        {
            return Avatar;
        }

        if (_resolvedAvatar is null && Uri.TryCreate(_avatarUrl, UriKind.Absolute, out var uri))
        {
            _resolvedAvatar = new BitmapImage(uri);
        }

        return _resolvedAvatar;
    }
}
