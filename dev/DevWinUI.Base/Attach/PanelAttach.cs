namespace DevWinUI;

public enum PanelTransitionCollection
{
    None,
    Default,
    AddDeleteThemeTransition,
    ContentThemeTransition,
    EdgeUIThemeTransition,
    EntranceThemeTransition,
    NavigationThemeTransition,
    PaneThemeTransition,
    PopupThemeTransition,
    ReorderThemeTransition,
    RepositionThemeTransition,
    SettingsCardTransition
}
public partial class PanelAttach
{
    /// <summary>
    /// Retrieves the collection of transitions associated with a specified panel.
    /// </summary>
    /// <param name="obj">The dependency object from which to retrieve the transitions.</param>
    /// <returns>A collection of transitions related to the specified dependency object.</returns>
    public static PanelTransitionCollection GetChildrenTransitions(DependencyObject obj)
    {
        return (PanelTransitionCollection)obj.GetValue(ChildrenTransitionsProperty);
    }

    /// <summary>
    /// Sets the collection of transitions for the children of a specified panel.
    /// </summary>
    /// <param name="obj">The target dependency object whose children will have transitions applied.</param>
    /// <param name="value">The collection of transitions to be set for the children of the specified dependency object.</param>
    public static void SetChildrenTransitions(DependencyObject obj, PanelTransitionCollection value)
    {
        obj.SetValue(ChildrenTransitionsProperty, value);
    }

    public static readonly DependencyProperty ChildrenTransitionsProperty =
        DependencyProperty.RegisterAttached("ChildrenTransitions", typeof(PanelTransitionCollection), typeof(PanelAttach), new PropertyMetadata(PanelTransitionCollection.Default, OnChildrenTransitionsPropertyChanged));

    private static void OnChildrenTransitionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Panel ctl && ctl != null)
        {
            TransitionCollection transition = new TransitionCollection();
            switch ((PanelTransitionCollection)e.NewValue)
            {
                case PanelTransitionCollection.None:
                    transition = new TransitionCollection();
                    break;
                case PanelTransitionCollection.Default:
                case PanelTransitionCollection.SettingsCardTransition:
                    transition.Add(new EntranceThemeTransition() { FromVerticalOffset = 50 });
                    transition.Add(new RepositionThemeTransition() { IsStaggeringEnabled = false });
                    break;
                case PanelTransitionCollection.AddDeleteThemeTransition:
                    transition.Add(new AddDeleteThemeTransition());
                    break;
                case PanelTransitionCollection.ContentThemeTransition:
                    transition.Add(new ContentThemeTransition());
                    break;
                case PanelTransitionCollection.EdgeUIThemeTransition:
                    transition.Add(new EdgeUIThemeTransition());
                    break;
                case PanelTransitionCollection.EntranceThemeTransition:
                    transition.Add(new EntranceThemeTransition());
                    break;
                case PanelTransitionCollection.NavigationThemeTransition:
                    transition.Add(new NavigationThemeTransition());
                    break;
                case PanelTransitionCollection.PaneThemeTransition:
                    transition.Add(new PaneThemeTransition());
                    break;
                case PanelTransitionCollection.PopupThemeTransition:
                    transition.Add(new PopupThemeTransition());
                    break;
                case PanelTransitionCollection.ReorderThemeTransition:
                    transition.Add(new ReorderThemeTransition());
                    break;
                case PanelTransitionCollection.RepositionThemeTransition:
                    transition.Add(new RepositionThemeTransition());
                    break;
            }

            ctl.ChildrenTransitions = transition;
        }
    }
}
