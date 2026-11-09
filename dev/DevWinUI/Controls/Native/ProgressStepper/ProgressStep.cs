using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUI;

public partial class ProgressStep : INotifyPropertyChanged
{
    private string _id = "";
    private string _title = "";
    private string? _description;
    private ProgressStepperStepStatus? _status;
    private IList<ProgressStep> _children = new ObservableCollection<ProgressStep>();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id
    {
        get => _id;
        set => Set(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => Set(ref _title, value);
    }

    public string? Description
    {
        get => _description;
        set => Set(ref _description, value);
    }

    public ProgressStepperStepStatus? Status
    {
        get => _status;
        set => Set(ref _status, value);
    }

    public IList<ProgressStep> Children
    {
        get => _children;
        set => Set(ref _children, value ?? new ObservableCollection<ProgressStep>());
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
