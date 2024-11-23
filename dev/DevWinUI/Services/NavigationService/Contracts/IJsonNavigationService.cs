using Microsoft.UI.Xaml.Media.Animation;

namespace DevWinUI;

public interface IJsonNavigationService
{
    void UnregisterEvents();

    event NavigatedEventHandler FrameNavigated;
    IList<object>? MenuItems { get; }
    object? SettingsItem { get; }
    IEnumerable<DataItem> SearchNavigationViewItems(IEnumerable<DataItem> items, string query);

    bool CanGoBack { get; }
    Frame? Frame { get; set; }
    Window? Window { get; set; }
    bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null);
    bool NavigateTo(Type pageType, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null);
    bool GoBack();
    void GetMenuItemsAsync(string jsonFilePath, PathType pathType = PathType.Relative);
}
