namespace DevWinUI;
public partial class JsonNavigationService
{
    private void MainBreadcrumb_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (args.Index < _mainBreadcrumb.BreadCrumbs.Count - 1)
        {
            var crumb = (NavigationBreadcrumb)args.Item;
            NavigateFromBreadcrumb(crumb.Page, args.Index);
        }
    }
    private void NavigateFromBreadcrumb(Type TargetPageType, int BreadcrumbBarIndex)
    {
        SlideNavigationTransitionInfo info = new SlideNavigationTransitionInfo();
        info.Effect = SlideNavigationTransitionEffect.FromLeft;

        NavigateTo(TargetPageType, null, false, info);

        _mainBreadcrumb.FixIndex(BreadcrumbBarIndex);
    }
}
