using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public static partial class MaterialAttach
{
    private static void HandleComboBox(ComboBox comboBox, bool newValue)
    {
        comboBox.Loaded -= OnComboBoxLoaded;
        comboBox.Loaded += OnComboBoxLoaded;

        if (!newValue)
        {
            var popup = DependencyObjectExtensions.FindDescendant<Popup>(comboBox);
            if (popup?.Child != null)
            {
                var scrollViewer = DependencyObjectExtensions.FindDescendant<ScrollViewer>(popup.Child);
                if (scrollViewer != null)
                {
                    scrollViewer.Style = Application.Current.Resources["DefaultScrollViewerStyle"] as Style;
                }
            }
        }
    }

    private static void OnComboBoxLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox comboBox) return;

        var popup = DependencyObjectExtensions.FindDescendant<Popup>(comboBox);
        if (popup == null) return;

        popup.Opened -= Popup_Opened;
        popup.Opened += Popup_Opened;

        void Popup_Opened(object s, object args)
        {
            var popupInstance = s as Popup;
            if (popupInstance?.Child == null) return;

            var scrollViewer = DependencyObjectExtensions.FindDescendant<ScrollViewer>(popupInstance.Child);
            if (scrollViewer == null) return;

            bool useBackdrop = GetUseAcrylicBackground(comboBox);

            scrollViewer.Style = useBackdrop
                ? Application.Current.Resources["ScrollViewerWithSystemBackdropElementStyle"] as Style
                : Application.Current.Resources["DefaultScrollViewerStyle"] as Style;
        }
    }
}
