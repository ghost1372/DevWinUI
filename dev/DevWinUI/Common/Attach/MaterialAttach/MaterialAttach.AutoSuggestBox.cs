using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public static partial class MaterialAttach
{
    private static void HandleAutoSuggestBox(AutoSuggestBox autoSuggestBox, bool newValue)
    {
        autoSuggestBox.Loaded -= OnAutoSuggestBoxLoaded;
        autoSuggestBox.Loaded += OnAutoSuggestBoxLoaded;

        if (!newValue)
        {
            var popup = DependencyObjectExtensions.FindDescendant<Popup>(autoSuggestBox);
            if (popup?.Child != null)
            {
                var border = DependencyObjectExtensions.FindDescendant<Border>(popup.Child);
                if (border.Child is Grid grid)
                {
                    border.Child = null;
                    var listView = grid.Children[1];
                    border.Child = listView;
                }
            }
        }
    }

    private static void OnAutoSuggestBoxLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not AutoSuggestBox autoSuggestBox)
            return;

        var popup = DependencyObjectExtensions.FindDescendant<Popup>(autoSuggestBox);
        if (popup == null) return;

        popup.Opened -= Popup_Opened;
        popup.Opened += Popup_Opened;

        void Popup_Opened(object s, object args)
        {
            var popupInstance = s as Popup;
            if (popupInstance?.Child == null) return;

            bool useBackdrop = GetUseAcrylicBackground(autoSuggestBox);

            if (popupInstance.Child is Border border)
            {
                var element = border.Child;

                if (useBackdrop)
                {
                    if (border.Child is ListView listView)
                    {
                        Grid grid = new Grid();

                        SystemBackdropElement systemBackdropElement = new SystemBackdropElement();
                        SystemBackdrop systemBackdrop = new AcrylicSystemBackdrop();

                        border.Child = null;

                        if (systemBackdrop != null)
                        {
                            systemBackdropElement.SystemBackdrop = systemBackdrop;

                            grid.Children.Add(systemBackdropElement);
                            grid.Children.Add(listView);

                            border.Child = grid;
                        }
                        else
                        {
                            border.Child = listView;
                        }
                    }
                }
                else
                {
                    if (border.Child is Grid grid)
                    {
                        border.Child = null;
                        var listView = grid.Children[1];
                        border.Child = listView;
                    }
                }
            }
        }
    }
}
