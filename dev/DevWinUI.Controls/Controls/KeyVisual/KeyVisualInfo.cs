using Windows.System;

namespace DevWinUI;

public partial class KeyVisualInfo
{
    public VirtualKey? Key { get; set; }
    public string KeyName { get; set; }

    public override string ToString()
    {
        return KeyName;
    }
}
