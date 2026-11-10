namespace DevWinUI;

public sealed partial class ParticleGlobeCountryEventArgs : EventArgs
{
    public ParticleGlobeCountryEventArgs(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; }
    public string Name { get; }
}
