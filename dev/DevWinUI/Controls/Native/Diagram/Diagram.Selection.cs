using System.Collections;
using System.Collections.Specialized;

namespace DevWinUI;

public sealed partial class Diagram
{
    public void ClearSelection()
    {
        if (SelectedNodes.Count == 0) return;

        foreach (var node in SelectedNodes.ToArray())
        {
            SelectedNodes.Remove(node);
            NodeUnselect?.Invoke(this, new DiagramNodeEventArgs(node));
        }
    }

    public void ExpandAll() => SetAllExpanded(true);

    public void CollapseAll() => SetAllExpanded(false);

    private void SetAllExpanded(bool expanded)
    {
        foreach (var node in EnumerateNodes())
            node.Expanded = expanded;

        QueueRebuild();
    }

    private void ToggleExpanded(DiagramNode node)
    {
        if (_views.ContainsKey(node))
        {
            _anchor = node;
            _anchorScreen = new Point(node.Left * _zoom + _panX, node.Top * _zoom + _panY);
        }

        _pendingFocus = node;
        node.Expanded = !node.Expanded;
        QueueRebuild();

        var args = new DiagramNodeEventArgs(node);
        if (node.Expanded)
        {
            NodeExpand?.Invoke(this, args);
        }
        else
        {
            NodeCollapse?.Invoke(this, args);
        }
    }

    private void ToggleSelection(DiagramNode node)
    {
        if (SelectedNodes.Remove(node))
        {
            NodeUnselect?.Invoke(this, new DiagramNodeEventArgs(node));
            return;
        }

        if (SelectionMode == DiagramSelectionMode.Single)
        {
            foreach (var previous in SelectedNodes.ToArray())
            {
                SelectedNodes.Remove(previous);
                NodeUnselect?.Invoke(this, new DiagramNodeEventArgs(previous));
            }
        }

        SelectedNodes.Add(node);
        NodeSelect?.Invoke(this, new DiagramNodeEventArgs(node));
    }

    private void OnSelectedNodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                SetSelected(e.NewItems, true);
                break;
            case NotifyCollectionChangedAction.Remove:
                SetSelected(e.OldItems, false);
                break;
            default:
                SyncSelection();
                break;
        }

        if (_selectedEdge != null && SelectedNodes.Count > 0) _selectedEdge = null;
        ApplyEdgeBrushes();
    }

    private void SetSelected(IList? nodes, bool selected)
    {
        if (nodes == null) return;

        foreach (var entry in nodes)
        {
            if (entry is DiagramNode node && _views.TryGetValue(node, out var v))
            {
                v.Selected = selected;
                UpdateFace(v);
            }
        }
    }

    private void SyncSelection()
    {
        var selected = new HashSet<DiagramNode>(SelectedNodes);
        foreach (var v in _all)
        {
            v.Selected = selected.Contains(v.Node);
            UpdateFace(v);
        }
    }
}
