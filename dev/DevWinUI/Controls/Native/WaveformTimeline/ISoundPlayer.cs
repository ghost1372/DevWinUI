using System.ComponentModel;

namespace DevWinUI;

public interface ISoundPlayer : INotifyPropertyChanged
{
    bool IsPlaying { get; }
}

