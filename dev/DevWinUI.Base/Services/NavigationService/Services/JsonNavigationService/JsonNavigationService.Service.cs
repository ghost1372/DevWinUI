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

        _pendingSelectionId = pageKey;
        _pendingSelectionPageType = pageType;
        var navigationSucceeded = false;
        try
        {
            navigationSucceeded = Navigate(pageType, parameter, clearNavigation, transitionInfo);
            return navigationSucceeded;
        }
        finally
        {
            if (!navigationSucceeded && _pendingSelectionId == pageKey && _pendingSelectionPageType == pageType)
            {
                _pendingSelectionId = null;
                _pendingSelectionPageType = null;
            }
        }
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
            var frameContentBeforeNavigationAOTSafe = Frame.Content;
            var pendingClearNavigationId = 0L;
            if (clearNavigation)
            {
                pendingClearNavigationId = ++_pendingClearNavigationSequence;
                _pendingClearNavigations.Add((pendingClearNavigationId, pageType, parameter));
            }

            var navigationSucceeded = false;
            try
            {
                var navigated = Frame.Navigate(pageType, parameter, transitionInfo);
                navigationSucceeded = navigated;
                if (navigated)
                {
                    if (_useBreadcrumbBar)
                    {
                        _mainBreadcrumb.AddNewItem(pageType, parameter);
                    }

                    _lastParameterUsed = parameter;

                    if (frameContentBeforeNavigationAOTSafe is Page page && page?.DataContext is INavigationAwareEx viewModel)
                    {
                        viewModel.OnNavigatedFrom();
                    }
                }

                return navigated;
            }
            finally
            {
                if (!navigationSucceeded && pendingClearNavigationId != 0)
                {
                    for (var i = 0; i < _pendingClearNavigations.Count; i++)
                    {
                        if (_pendingClearNavigations[i].Id == pendingClearNavigationId)
                        {
                            _pendingClearNavigations.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        return false;
    }
    private void OnFrameNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is not Frame frame)
        {
            return;
        }

        for (var i = 0; i < _pendingClearNavigations.Count; i++)
        {
            var pendingNavigation = _pendingClearNavigations[i];
            if (pendingNavigation.PageType == e.SourcePageType && Equals(pendingNavigation.Parameter, e.Parameter))
            {
                _pendingClearNavigations.RemoveAt(i);
                frame.BackStack.Clear();
                break;
            }
        }

        var pendingSelectionId = _pendingSelectionId;
        var pendingSelectionPageType = _pendingSelectionPageType;
        var completesPendingSelection = pendingSelectionPageType == e.SourcePageType;
        try
        {
            if (Frame?.Content is Page page && page?.DataContext is INavigationAwareEx viewModel)
            {
                viewModel.OnNavigatedTo(e.Parameter);
            }

            FrameNavigated?.Invoke(sender, e);
        }
        finally
        {
            if (completesPendingSelection &&
                _pendingSelectionId == pendingSelectionId &&
                _pendingSelectionPageType == pendingSelectionPageType)
            {
                _pendingSelectionId = null;
                _pendingSelectionPageType = null;
            }
        }
    }
}
