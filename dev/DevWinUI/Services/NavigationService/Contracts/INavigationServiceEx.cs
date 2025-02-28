namespace DevWinUI;

public interface INavigationServiceEx
{
    event NavigatedEventHandler Navigated;

    bool CanGoBack
    {
        get;
    }

    Frame? Frame
    {
        get; set;
    }

    Microsoft.UI.Xaml.Window? Window
    {
        get; set;
    }

    bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null);
    bool NavigateTo(Type pageType, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null);

    bool GoBack();
}
