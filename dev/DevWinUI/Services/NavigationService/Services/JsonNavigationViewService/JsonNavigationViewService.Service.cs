using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Media.Animation;

namespace DevWinUI;
public partial class JsonNavigationViewService : IJsonNavigationViewService
{
    public Frame? Frame
    {
        get
        {
            if (_frame == null)
            {
                _frame = Window?.Content as Frame;
                RegisterFrameEvents();
            }

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
        if (_frame != null)
        {
            _frame.Navigated += OnFrameNavigated;
        }
    }

    private void UnregisterFrameEvents()
    {
        if (_frame != null)
        {
            _frame.Navigated -= OnFrameNavigated;
        }
    }

    public bool GoBack()
    {
        if (CanGoBack)
        {
            var frameContentBeforeNavigationAOTSafe = _frame?.Content;

            _frame.GoBack();

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

        if (_frame != null && (_frame.CurrentSourcePageType != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed))))
        {
            _frame.Tag = clearNavigation;
            
            if (_useBreadcrumbBar)
            {
                _mainBreadcrumb.AddNewItem(pageType, null, parameter, parameter, _allowDuplication, null);
            }

            var frameContentBeforeNavigationAOTSafe = _frame?.Content;

            var navigated = _frame.Navigate(pageType, parameter, transitionInfo);
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
                if (_useBreadcrumbBar)
                {
                    _mainBreadcrumb?.BreadCrumbs?.Clear();
                }
            }

            // This is AOT Safe
            if (_frame?.Content is Page page && page?.DataContext is INavigationAwareEx viewModel)
            {
                viewModel.OnNavigatedTo(e.Parameter);
            }

            FrameNavigated?.Invoke(sender, e);
        }
    }
}
