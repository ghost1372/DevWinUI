using Microsoft.UI.Xaml.Data;

namespace DevWinUI;

/// <summary>
/// Provides a mean to bind to an ancestor of a specific type.
/// </summary>
public partial class AncestorBindingExtension : MarkupExtension
{
    /// <summary>
    /// Binding path from the ancestor.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Type of ancestor to bind from.
    /// </summary>
    public Type AncestorType { get; set; } = typeof(object);

    /// <summary>
    /// Gets or sets the converter object that is called by the binding engine to modify the data as it is passed between the source and target, or vice versa.
    /// </summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>
    /// Gets or sets a parameter that can be used in the Converter logic.
    /// </summary>
    public object? ConverterParameter { get; set; }

    /// <summary>
    /// Gets or sets a value that names the language to pass to any converter specified by the Converter property.
    /// </summary>
    public string? ConverterLanguage { get; set; }

    public AncestorBindingExtension()
    {
    }

    protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
    {
        // until we can reach/resolve 'property', we have no option but to return null.
        // while, this would crash if this is set on a dp that doesn't accept null,
        // there is really nothing else we can salvage from here.
        // but usually that is also indicative of another problem: using this on an invalid target/property.
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget pvt) return null;
        if (pvt.TargetObject is not FrameworkElement owner) return null;
        if (pvt.TargetProperty is not ProvideValueTargetProperty { DeclaringType: { } ownerType, Name: { } propertyName }) return null;
        if (ownerType.FindDependencyProperty(propertyName) is not { } property) return null;

        owner.Loaded += OnTargetLoaded;
        if (owner.IsLoaded)
        {
            OnTargetLoaded(owner, default!);
        }

        void OnTargetLoaded(object s, RoutedEventArgs e)
        {
            if (s is FrameworkElement fe)
            {
                // normally, this is a one-shot installation, so we should self-unsubscribe. but we don't here, because
                // it is possible that we are in a data-template that gets recyled from one content-presenter to another.
                //fe.Loaded -= OnTargetLoaded;
                if (GetAncestors(fe).FirstOrDefault(x => AncestorType?.IsAssignableFrom(x.GetType()) == true) is { } source)
                {
                    var binding = new Binding
                    {
                        Path = new PropertyPath(Path),
                        Source = source,
                        Mode = BindingMode.OneWay,
                        Converter = Converter,
                        ConverterLanguage = ConverterLanguage,
                        ConverterParameter = ConverterParameter,
                    };
                    fe.SetBinding(property, binding);
                    return;
                }
            }

            (s as DependencyObject)?.ClearValue(property);
        }

        // return current value, until the binding comes online.
        return owner.GetValue(property);
    }

    private static IEnumerable<DependencyObject> GetAncestors(DependencyObject x)
    {
        if (x is null) yield break;
        while (VisualTreeHelper.GetParent(x) is { } parent)
        {
            yield return x = parent;
        }
    }
}
