namespace DevWinUI;

public static partial class AutoSuggestBoxHelper
{
    private static string NoResult = "No result found";

    public static void LoadSuggestions(AutoSuggestBox autoSuggestBox, AutoSuggestBoxTextChangedEventArgs args, IList<string> suggestList)
    {
        List<string> list = new List<string>();
        if (args.Reason != 0)
        {
            return;
        }

        string[] querySplit = autoSuggestBox.Text.Split(' ');

        foreach (string item in suggestList)
        {
            bool result = true;
            foreach (string value in querySplit)
            {
                if (!item.Contains(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = false;
                    break;
                }
            }

            if (result)
            {
                list.Add(item);
            }
        }

        if (list.Count > 0)
        {
            autoSuggestBox.ItemsSource = list;
        }
        else
        {
            autoSuggestBox.ItemsSource = new string[1] { NoResult };
        }
    }

    public static void LoadSuggestions(AutoSuggestBox autoSuggestBox, AutoSuggestBoxTextChangedEventArgs args, IList<string> suggestList, string noResultString)
    {
        NoResult = noResultString;
        LoadSuggestions(autoSuggestBox, args, suggestList);
    }

    public static void OnITitleBarAutoSuggestBoxQuerySubmittedEvent(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args, Frame frame)
    {
        HandleITitleBarAutoSuggestBoxEvents(sender, null, args, frame, false);
    }

    public static void OnITitleBarAutoSuggestBoxTextChangedEvent(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args, Frame frame)
    {
        HandleITitleBarAutoSuggestBoxEvents(sender, args, null, frame, true);
    }

    private static void HandleITitleBarAutoSuggestBoxEvents(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs textChangedArgs, AutoSuggestBoxQuerySubmittedEventArgs querySubmittedArgs, Frame frame, bool isTextChangedEvent)
    {
        var frameContentAOTSafe = frame?.Content;
        if (frameContentAOTSafe is Page page && page?.DataContext is ITitleBarAutoSuggestBoxAware viewModelAOTSafe)
        {
            if (isTextChangedEvent)
            {
                viewModelAOTSafe.OnAutoSuggestBoxTextChanged(sender, textChangedArgs);
            }
            else
            {
                viewModelAOTSafe.OnAutoSuggestBoxQuerySubmitted(sender, querySubmittedArgs);
            }
        }
    }
}
