namespace DevWinUI;

public interface INavigationPageRefreshable
{
    bool IsEmpty { get; }

    void Refresh();
}
