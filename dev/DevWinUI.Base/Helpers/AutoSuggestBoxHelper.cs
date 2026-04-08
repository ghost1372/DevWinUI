namespace DevWinUI;

public static partial class AutoSuggestBoxHelper
{
    private static string NoResult = "No result found";

    /// <summary>
    /// Loads suggestions into an AutoSuggestBox.
    /// </summary>
    /// <param name="autoSuggestBox">The control that displays suggestions to the user based on their input.</param>
    /// <param name="args">Contains information about the text change event that triggered the suggestion loading.</param>
    /// <param name="suggestList">A collection of potential suggestions to be filtered and displayed to the user.</param>
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

    /// <summary>
    /// Loads suggestions into an AutoSuggestBox.
    /// </summary>
    /// <param name="autoSuggestBox">The component that displays suggestions to the user as they type.</param>
    /// <param name="args">Contains information about the text change event that triggered the suggestion loading.</param>
    /// <param name="suggestList">A collection of strings that represent the suggestions to be displayed.</param>
    /// <param name="noResultString">A message displayed when there are no suggestions available.</param>
    public static void LoadSuggestions(AutoSuggestBox autoSuggestBox, AutoSuggestBoxTextChangedEventArgs args, IList<string> suggestList, string noResultString)
    {
        NoResult = noResultString;
        LoadSuggestions(autoSuggestBox, args, suggestList);
    }

    /// <summary>
    /// Handles the event when a query is submitted in the AutoSuggestBox.
    /// </summary>
    /// <param name="sender">Represents the AutoSuggestBox that triggered the event.</param>
    /// <param name="args">Contains the details of the submitted query.</param>
    /// <param name="frame">Represents the navigation frame associated with the event.</param>
    public static void OnITitleBarAutoSuggestBoxQuerySubmittedEvent(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args, Frame frame)
    {
        HandleITitleBarAutoSuggestBoxEvents(sender, null, args, frame, false);
    }

    /// <summary>
    /// Handles the event when a text is changed in the AutoSuggestBox.
    /// </summary>
    /// <param name="sender">Represents the AutoSuggestBox that triggered the text change event.</param>
    /// <param name="args">Contains the event data related to the text change in the AutoSuggestBox.</param>
    /// <param name="frame">Indicates the frame context in which the event is being handled.</param>
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
