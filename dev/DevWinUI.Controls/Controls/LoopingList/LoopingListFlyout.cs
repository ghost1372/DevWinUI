namespace DevWinUI;

internal partial class LoopingListFlyout : Flyout
{
    public event EventHandler<LoopingListEventArgs> Accepted;
    public event EventHandler<RoutedEventArgs> Dismissed;

    private LoopingListFlyoutPresenter _presenter;
    public LoopingListFlyoutPresenter Presenter => _presenter;

    private LoopingSelector primarySelector;
    private LoopingSelector secondarySelector;
    private LoopingSelector tertiarySelector;
    public LoopingListFlyout(LoopingSelector primarySelector, LoopingSelector secondarySelector, LoopingSelector tertiarySelector)
    {
        this.primarySelector = primarySelector;
        this.secondarySelector = secondarySelector;
        this.tertiarySelector = tertiarySelector;
    }
    protected override Control CreatePresenter()
    {
        if (_presenter == null)
            _presenter = new LoopingListFlyoutPresenter(primarySelector, secondarySelector, tertiarySelector);

        _presenter.Accepted -= OnAcceptButton;
        _presenter.Accepted += OnAcceptButton;

        _presenter.Dismissed -= OnDismissButton;
        _presenter.Dismissed += OnDismissButton;
        return _presenter;
    }

    private void OnDismissButton(object sender, RoutedEventArgs e)
    {
        Dismissed?.Invoke(this, null);
    }

    private void OnAcceptButton(object sender, LoopingListEventArgs e)
    {
        Accepted?.Invoke(this, e);
    }
}
