using Windows.Foundation.Collections;

namespace DevWinUI;

public sealed partial class ReasoningSteps : ItemsControl
{
    readonly List<ReasoningStep> _hooked = new();

    public ReasoningSteps()
    {
        DefaultStyleKey = typeof(ReasoningSteps);
        Items.VectorChanged += OnItemsChanged;
    }

    internal event Action? Changed;

    public ItemCollection Steps => Items;

    public string? ActiveLabel
    {
        get
        {
            for (int i = Items.Count - 1; i >= 0; i--)
                if (Items[i] is ReasoningStep { Status: ReasoningStepStatus.Active } step) return step.Label;
            return null;
        }
    }

    void OnItemsChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
    {
        foreach (var step in _hooked) step.Changed -= OnStepChanged;
        _hooked.Clear();

        foreach (var item in Items)
        {
            if (item is not ReasoningStep step) continue;
            step.Changed += OnStepChanged;
            _hooked.Add(step);
        }

        Changed?.Invoke();
    }

    void OnStepChanged() => Changed?.Invoke();
}
