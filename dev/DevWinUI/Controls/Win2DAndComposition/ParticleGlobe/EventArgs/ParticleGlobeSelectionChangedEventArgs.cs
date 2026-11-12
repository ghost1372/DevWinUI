namespace DevWinUI;

public sealed partial class ParticleGlobeSelectionChangedEventArgs : EventArgs
{
    public ParticleGlobeSelectionChangedEventArgs(string oldCode, string oldName, string newCode, string newName)
    {
        OldCode = oldCode;
        OldName = oldName;
        NewCode = newCode;
        NewName = newName;
    }

    public string OldCode { get; }
    public string OldName { get; }
    public string NewCode { get; }
    public string NewName { get; }
}
