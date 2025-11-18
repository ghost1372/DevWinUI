//https://github.com/benthorner/radial_controls

using System.Collections.Specialized;

namespace DevWinUI;

public partial class HaloRingCluster : Control
{
    private ObservableCollection<UIElement> _children;
    private Queue<NotifyCollectionChangedEventArgs> _updates;

    public HaloRingCluster()
    {
        _updates = new Queue<NotifyCollectionChangedEventArgs>();
        _children = new ObservableCollection<UIElement>();
        _children.CollectionChanged += (o, e) => _updates.Enqueue(e);
    }

    public ICollection<UIElement> Children
    {
        get { return _children; }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        while (_updates.Count > 0)
        {
            Synchronise(_updates.Dequeue());
        }

        return new Size(0, 0);
    }

    private void Synchronise(NotifyCollectionChangedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var parent = Parent as HaloRing;
            if (parent == null) return;

            if (args.OldItems != null)
            {
                var oldItems = args.OldItems.OfType<UIElement>();

                foreach (var item in oldItems)
                {
                    parent.Children.Remove(item);
                }
            }

            if (args.NewItems != null)
            {
                var newItems = args.NewItems.OfType<UIElement>();

                foreach (var item in newItems)
                {
                    parent.Children.Add(item);
                }
            }
        });
    }
}
