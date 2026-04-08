using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Items))]
public partial class MenuFlyoutSecondaryItems : DependencyObject
{
    public ObservableCollection<object> Items { get; } = new ObservableCollection<object>();
}
