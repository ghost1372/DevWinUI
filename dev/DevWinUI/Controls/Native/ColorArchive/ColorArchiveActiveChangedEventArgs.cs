namespace DevWinUI;

public sealed partial class ColorArchiveActiveChangedEventArgs : EventArgs
{
    public ColorArchiveActiveChangedEventArgs(int index, ColorArchiveProject project)
    {
        Index = index;
        Project = project;
    }

    public int Index { get; }
    public ColorArchiveProject Project { get; }
}
