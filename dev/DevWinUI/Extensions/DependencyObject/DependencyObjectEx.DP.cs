namespace DevWinUI;

public partial class DependencyObjectEx
{
    internal static string RemoveTail(this string value, string? tail) => tail?.Length > 0 && value.EndsWith(tail) ? value[..^tail.Length] : value;

    private static Dictionary<(Type Type, string Property), DependencyPropertyInfo?> _dependencyPropertyReflectionCache = new(2);
    internal static IDisposable RegisterDisposablePropertyChangedCallback(this DependencyObject? instance, DependencyProperty property, DependencyPropertyChangedCallback callback)
    {
        if (instance == null)
        {
            return Disposable.Empty;
        }

        var token = instance.RegisterPropertyChangedCallback(property, callback);
        return Disposable.Create(() => instance.UnregisterPropertyChangedCallback(property, token));
    }

    public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject element, bool includeCurrent = true)
    {
        if (includeCurrent)
        {
            yield return element;
        }

        if (element is null) yield break;
        while (VisualTreeHelper.GetParent(element) is { } parent)
        {
            yield return element = parent;
        }
    }

    /// <summary>
    /// ** Recursively ** gets an enumerable sequence of all the parent objects of a given element.
    /// Parents are ordered from bottom to the top, i.e. from direct parent to the root of the window.
    /// </summary>
    /// <param name="element">The element to search from</param>
    /// <param name="includeCurrent">Determines if the current <paramref name="element"/> should be included or not.</param>
    public static IEnumerable<DependencyObject> GetAllParents(this DependencyObject element, bool includeCurrent = true)
    {
        if (includeCurrent)
        {
            yield return element;
        }

        for (var parent = (element as FrameworkElement)?.Parent ?? VisualTreeHelper.GetParent(element);
            parent != null;
            parent = (parent as FrameworkElement)?.Parent ?? VisualTreeHelper.GetParent(parent))
        {
            yield return parent;
        }
    }

    /// <summary>
    /// Search for the first parent of the given type.
    /// </summary>
    /// <typeparam name="T">The type of child we are looking for</typeparam>
    /// <param name="element">The element to search from</param>
    /// <param name="includeCurrent">Determines if the current <paramref name="element"/> should be tested or not.</param>
    /// <returns>The first found parent that is of the given type.</returns>
    public static T? FindFirstParent<T>(this DependencyObject element, bool includeCurrent = true)
        where T : DependencyObject
        => element.GetAllParents(includeCurrent).OfType<T>().FirstOrDefault();

    public static DependencyProperty? FindDependencyProperty<TProperty>(this DependencyObject owner, string propertyName) =>
        owner.GetType().FindDependencyProperty<TProperty>(propertyName);

    public static DependencyProperty? FindDependencyProperty(this DependencyObject owner, string propertyName) =>
        owner.GetType().FindDependencyProperty(propertyName);

    public static DependencyProperty? FindDependencyProperty<TProperty>(this Type ownerOrDescendantType, string propertyName)
    {
        var propertyType = typeof(TProperty);
        var property = FindDependencyPropertyInfo(ownerOrDescendantType, propertyName);

        if (property is { } && property.PropertyType != typeof(TProperty))
        {
            property = null;
        }

        return property?.Definition;
    }

    public static DependencyProperty? FindDependencyProperty(this Type ownerOrDescendantType, string propertyName) =>
        FindDependencyPropertyInfo(ownerOrDescendantType, propertyName)?.Definition;

    internal static DependencyPropertyInfo? FindDependencyPropertyInfo(this Type ownerOrDescendantType, string propertyName)
    {
        propertyName = propertyName.RemoveTail("Property");

        var type = ownerOrDescendantType;
        var key = (ownerType: type, propertyName);

        // given that we are doing FlattenHierarchy lookup, it is fine that we are storing multiple pairs of (types-to-same-dp)
        // since it is not worth the trouble to handle the type hierarchy...
        if (!_dependencyPropertyReflectionCache.TryGetValue(key, out var property))
        {
            property = GetDetails(
                type.GetProperty($"{propertyName}Property", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy) as MemberInfo ??
                type.GetField($"{propertyName}Property", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            );
            _dependencyPropertyReflectionCache[key] = property;

            DependencyPropertyInfo? GetDetails(MemberInfo? dpInfo)
            {
                if (dpInfo is { } && GetValue(dpInfo) is DependencyProperty dp)
                {
                    // DeclaredOnly: Specifies flags that control binding and the way in which the search for members and types is conducted by reflection.
                    // because 'dpInfo.DeclaringType' is the guaranteed type, and we don't want an overridden property from a random base to throw AmbiguousMatchException
                    var holderProperty = dpInfo.DeclaringType?.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    var propertyType =
                        holderProperty?.PropertyType ??
                        dpInfo.DeclaringType?.GetMethod($"Get{propertyName}", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.ReturnType ??
                        dpInfo.DeclaringType?.GetMethod($"Set{propertyName}", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.GetParameters().ElementAtOrDefault(1)?.ParameterType ??
                        null;

                    return new(dp, propertyName, propertyType, dpInfo.DeclaringType!, holderProperty is null);
                }

                return null;
            }
            static object? GetValue(MemberInfo member) => member switch
            {
                PropertyInfo pi => pi.GetValue(null),
                FieldInfo fi => fi.GetValue(null),

                _ => throw new ArgumentOutOfRangeException(member?.GetType().Name),
            };
        }

        return property;
    }

    public static bool TryGetValue(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, out DependencyObject? value)
    {
        value = default;

        try
        {
            value = dependencyObject.GetValue(dependencyProperty) as DependencyObject;
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    internal record class DependencyPropertyInfo(DependencyProperty Definition, string PropertyName, Type? PropertyType, Type OwnerType, bool IsAttached);
}
