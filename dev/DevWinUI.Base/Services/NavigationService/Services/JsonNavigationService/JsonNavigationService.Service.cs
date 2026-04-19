using System.Diagnostics.CodeAnalysis;

namespace DevWinUI;
public partial class JsonNavigationService
{
    private Frame? _frame;
    public Frame? Frame
    {
        get
        {
            return _frame;
        }

        set
        {
            UnregisterFrameEvents();
            _frame = value;
            RegisterFrameEvents();
        }
    }

    [MemberNotNullWhen(true, nameof(Frame), nameof(_frame))]
    public bool CanGoBack => Frame != null && Frame.CanGoBack;

    private void RegisterFrameEvents()
    {
        if (Frame != null)
        {
            Frame.Navigated += OnFrameNavigated;
        }
    }

    private void UnregisterFrameEvents()
    {
        if (Frame != null)
        {
            Frame.Navigated -= OnFrameNavigated;
        }
    }

    public bool GoBack()
    {
        if (CanGoBack)
        {
            var frameContentBeforeNavigationAOTSafe = Frame?.Content;

            Frame.GoBack();

            if (frameContentBeforeNavigationAOTSafe is Page page && page?.DataContext is INavigationAwareEx viewModel)
            {
                viewModel.OnNavigatedFrom();
            }

            return true;
        }

        return false;
    }

    public bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null)
    {
        var pageType = GetPageType(pageKey);
        return Navigate(pageType, parameter, clearNavigation, transitionInfo);
    }

    public bool NavigateTo(Type pageType, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null)
    {
        return Navigate(pageType, parameter, clearNavigation, transitionInfo);
    }

    public bool Navigate(Type pageType, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null)
    {
        if (pageType == null)
        {
            return false;
        }

        if (Frame != null && (Frame.CurrentSourcePageType != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed))))
        {
            Frame.Tag = clearNavigation;

            if (_useBreadcrumbBar)
            {
                _mainBreadcrumb.AddNewItem(pageType, parameter);
            }

            var frameContentBeforeNavigationAOTSafe = Frame.Content;

            var navigated = Frame.Navigate(pageType, parameter, transitionInfo);
            if (navigated)
            {
                _lastParameterUsed = parameter;

                if (frameContentBeforeNavigationAOTSafe is Page page && page?.DataContext is INavigationAwareEx viewModel)
                {
                    viewModel.OnNavigatedFrom();
                }
            }

            return navigated;
        }

        return false;
    }
    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is Frame frame)
        {
            var clearNavigation = (bool)frame.Tag;
            if (clearNavigation)
            {
                frame.BackStack.Clear();
            }

            // This is AOT Safe
            if (Frame?.Content is Page page && page?.DataContext is INavigationAwareEx viewModel)
            {
                viewModel.OnNavigatedTo(e.Parameter);
            }

            FrameNavigated?.Invoke(sender, e);
        }
    }
}
