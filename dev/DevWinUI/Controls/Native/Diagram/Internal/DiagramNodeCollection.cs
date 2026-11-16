namespace DevWinUI;

internal sealed partial class DiagramNodeCollection : ObservableCollection<DiagramNode>
{
    private readonly DiagramNode _owner;

    public DiagramNodeCollection(DiagramNode owner)
    {
        _owner = owner;
    }

    protected override void InsertItem(int index, DiagramNode item)
    {
        _owner.Attach(item);
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, DiagramNode item)
    {
        _owner.Detach(this[index]);
        _owner.Attach(item);
        base.SetItem(index, item);
    }

    protected override void RemoveItem(int index)
    {
        _owner.Detach(this[index]);
        base.RemoveItem(index);
    }

    protected override void ClearItems()
    {
        foreach (var child in this)
        {
            _owner.Detach(child);
        }

        base.ClearItems();
    }
}
