using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.UI.Dispatching;

namespace DevWinUI;

public sealed partial class SidebarViewMenuSource : ObservableCollection<FlatSidebarItem>, IDisposable
{
    private readonly DispatcherQueue _dispatcher = DispatcherQueue.GetForCurrentThread();
    private readonly Dictionary<ISidebarItemModel, FlatSidebarItem> _lookup = new();
    private readonly Dictionary<ISidebarItemModel, INotifyCollectionChanged> _childHooks = new();
    private readonly Dictionary<INotifyCollectionChanged, ISidebarItemModel> _childOwners = new();
    private readonly IEnumerable<ISidebarItemModel> _roots;
    private readonly Action<ISidebarItemModel>? _loadChildren;

    public SidebarViewMenuSource(IEnumerable<ISidebarItemModel> roots, Action<ISidebarItemModel>? loadChildren = null)
    {
        _roots = roots;
        _loadChildren = loadChildren;

        if (roots is INotifyCollectionChanged observable)
            observable.CollectionChanged += OnRootsChanged;

        Rebuild();
    }

    public ISidebarItemModel? FindByPath(string path)
        => _lookup.Keys.FirstOrDefault(e => e.Path == path);

    public void Dispose()
    {
        if (_roots is INotifyCollectionChanged observable)
            observable.CollectionChanged -= OnRootsChanged;

        UnhookAll();
        Clear();
    }

    private void Rebuild()
    {
        UnhookAll();
        Clear();

        var batch = new List<FlatSidebarItem>();
        foreach (var root in _roots)
            Collect(root, 0, batch);

        foreach (var node in batch)
        {
            Hook(node);
            Add(node);
        }

        UpdateSectionGaps();
    }

    private void Resync(ISidebarItemModel item)
    {
        if (!_lookup.TryGetValue(item, out var node))
            return;

        var start = IndexOf(node);
        if (start < 0)
            return;

        var end = start + 1;
        while (end < Count && this[end].Depth > node.Depth)
            end++;

        for (var i = end - 1; i > start; i--)
        {
            Unhook(this[i]);
            RemoveAt(i);
        }

        if (item.IsExpanded)
        {
            var batch = new List<FlatSidebarItem>();
            CollectChildren(item, node.Depth, batch);

            for (var i = 0; i < batch.Count; i++)
            {
                Hook(batch[i]);
                Insert(start + 1 + i, batch[i]);
            }
        }

        UpdateSectionGaps();
    }

    private void Collect(ISidebarItemModel item, int depth, List<FlatSidebarItem> sink)
    {
        sink.Add(new FlatSidebarItem(item, depth));

        if (item.IsExpanded)
            CollectChildren(item, depth, sink);
    }

    private void CollectChildren(ISidebarItemModel item, int depth, List<FlatSidebarItem> sink)
    {
        if (item.HasUnrealizedChildren)
            _loadChildren?.Invoke(item);

        foreach (var child in item.Children as IEnumerable<ISidebarItemModel> ?? [])
            Collect(child, depth + 1, sink);
    }

    private void Hook(FlatSidebarItem node)
    {
        _lookup[node.Item] = node;
        node.Item.PropertyChanged += OnItemPropertyChanged;
        HookChildren(node.Item);
    }

    private void Unhook(FlatSidebarItem node)
    {
        node.Item.PropertyChanged -= OnItemPropertyChanged;
        UnhookChildren(node.Item);
        _lookup.Remove(node.Item);
    }

    private void UnhookAll()
    {
        foreach (var item in _lookup.Keys)
            item.PropertyChanged -= OnItemPropertyChanged;

        foreach (var children in _childOwners.Keys)
            children.CollectionChanged -= OnChildrenChanged;

        _lookup.Clear();
        _childHooks.Clear();
        _childOwners.Clear();
    }

    private void HookChildren(ISidebarItemModel item)
    {
        if (item.Children is not INotifyCollectionChanged children)
            return;

        children.CollectionChanged += OnChildrenChanged;
        _childHooks[item] = children;
        _childOwners[children] = item;
    }

    private void UnhookChildren(ISidebarItemModel item)
    {
        if (!_childHooks.Remove(item, out var children))
            return;

        children.CollectionChanged -= OnChildrenChanged;
        _childOwners.Remove(children);
    }

    private void OnRootsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => _dispatcher.TryEnqueue(Rebuild);

    private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is INotifyCollectionChanged children && _childOwners.TryGetValue(children, out var owner))
            _dispatcher.TryEnqueue(() => Resync(owner));
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ISidebarItemModel item)
            return;

        switch (e.PropertyName)
        {
            case nameof(ISidebarItemModel.IsExpanded):
                _dispatcher.TryEnqueue(() => Resync(item));
                break;

            case nameof(ISidebarItemModel.Children):
                _dispatcher.TryEnqueue(() =>
                {
                    UnhookChildren(item);
                    if (_lookup.ContainsKey(item))
                        HookChildren(item);

                    Resync(item);
                });
                break;
        }
    }

    private void UpdateSectionGaps()
    {
        var prevWasExpanded = false;
        foreach (var node in this)
        {
            if (node.Depth != 0)
                continue;

            node.HasExpandedPredecessor = prevWasExpanded;
            prevWasExpanded = node.Item.Children is null || node.Item.IsExpanded;
        }
    }
}
