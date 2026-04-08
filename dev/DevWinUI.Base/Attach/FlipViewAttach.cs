using System.Collections;
using System.Collections.Specialized;

namespace DevWinUI;

public static partial class FlipViewAttach
{
    protected partial class FlipViewList : List<object>
    {
        /// <summary>
        /// Initializes a new instance of the FlipViewList class.
        /// </summary>
        /// <param name="collection">the initial collection</param>
        public FlipViewList(IEnumerable<object> collection) :
            base(collection)
        {
        }
    }

    public static bool GetIsLooping(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsLoopingProperty);
    }

    public static void SetIsLooping(DependencyObject obj, bool value)
    {
        obj.SetValue(IsLoopingProperty, value);
    }

    public static readonly DependencyProperty IsLoopingProperty =
        DependencyProperty.RegisterAttached("IsLooping", typeof(bool), typeof(FlipViewAttach), new PropertyMetadata(true, OnIsLoopingChanged));

    private static void OnIsLoopingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        var flipView = dependencyObject as FlipView;

        if ((bool)args.NewValue)
        {
            var enumerable = flipView.ItemsSource as IEnumerable;

            if (enumerable != null)
            {
                Initialize(flipView);
            }
            else
            {
                flipView.SelectionChanged += flipView_SelectionChanged;
            }
        }
        else
        {
            var flipViewList = flipView.ItemsSource as FlipViewList;

            flipView.SelectionChanged -= flipView_SelectionChanged;

            if (flipViewList != null)
            {
                var selectedItem = flipView.SelectedItem;

                flipViewList.RemoveAt(0);
                flipViewList.Remove(flipViewList.Last());

                flipView.ItemsSource = flipViewList.ToArray();

                flipView.SelectedItem = selectedItem;
            }
        }
    }

    /// <summary>
    /// Replace the FlipView.ItemsSource with a FlipView list that has
    /// duplicate items at the head and tail
    /// </summary>
    /// <param name="flipView">the FlipView</param>
    /// <returns>the number of items in the FlipView.ItemsSource</returns>
    private static int Initialize(FlipView flipView)
    {
        flipView.SelectionChanged -= flipView_SelectionChanged;

        var collectionChanged = flipView.ItemsSource as INotifyCollectionChanged;

        if (collectionChanged != null)
        {
            collectionChanged.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
            {
                UpdateList(flipView, sender as IEnumerable);
            };
        }

        var enumerable = flipView.ItemsSource as IEnumerable;

        if (enumerable != null)
        {
            var enumerableObjects = enumerable.OfType<object>();

            var loopingList = new FlipViewList(enumerableObjects);

            loopingList.Insert(0, enumerableObjects.Last());

            loopingList.Add(enumerableObjects.First());

            flipView.ItemsSource = loopingList;

            flipView.SelectedItem = loopingList[1];

            flipView.SelectionChanged += flipView_SelectionChanged;

            return loopingList.Count;
        }

        return 0;
    }

    /// <summary>
    /// Update the list
    /// </summary>
    /// <param name="flipView">the FlipView</param>
    /// <param name="enumerable">the enumerable collection</param>
    private static void UpdateList(FlipView flipView, IEnumerable enumerable)
    {
        var selection = flipView.SelectedItem;

        var enumerableObjects = enumerable.OfType<object>();

        var flipViewList = flipView.ItemsSource as FlipViewList;

        flipViewList.Clear();
        flipViewList.Add(enumerableObjects.Last());
        flipViewList.AddRange(enumerableObjects);
        flipViewList.Add(enumerableObjects.First());

        flipView.SelectedItem = selection;
    }

    /// <summary>
    /// swaps the selected index if the beginning or end is reached
    /// </summary>
    /// <param name="sender">the FlipView</param>
    /// <param name="e">the selection changed event arguments</param>
    private static void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var flipView = sender as FlipView;

        var list = flipView.ItemsSource as FlipViewList;

        int count = 0;

        if (list == null)
        {
            count = Initialize(flipView);
        }
        else
        {
            count = list.Count;
        }

        if (count < 3)
        {
            return;
        }

        if (flipView.SelectedIndex == 0)
        {
            flipView.SelectedIndex = count - 2;
        }
        else if (flipView.SelectedIndex == count - 1)
        {
            flipView.SelectedIndex = 1;
        }
        else if (flipView.SelectedIndex == -1)
        {
            flipView.SelectedIndex = 1;
        }
    }
}
