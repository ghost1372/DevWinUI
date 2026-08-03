using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DevWinUI;

/// <summary>
/// Provides a set of properties that aid in implementations of input validation.
/// </summary>
public sealed partial class Validation : DependencyObject
{
    private sealed partial class ValidationBindingState
    {
        public INotifyDataErrorInfo Provider;
        public EventHandler<DataErrorsChangedEventArgs> ErrorsChangedHandler;
        public bool LoadedHooked;
    }

    private static readonly ConditionalWeakTable<DependencyObject, ValidationBindingState> ValidationStates = new();

    /// <summary>
    /// Gets or sets a provider that implements input validation through
    /// <see cref="INotifyDataErrorInfo"/>. Must be used along with the
    /// <see cref="ValidationPropertyNameProperty"/>.
    /// </summary>
    public static readonly DependencyProperty ValidationProviderProperty
        = DependencyProperty.RegisterAttached("ValidationProvider", typeof(INotifyDataErrorInfo),
            typeof(Validation), new(null, OnValidationProviderChanged));

    /// <summary>
    /// Gets or sets the name of the property to validate. The actual
    /// validation is done through the validation provider (see <see cref="ValidationProviderProperty"/>).
    /// </summary>
    public static readonly DependencyProperty ValidationPropertyNameProperty
        = DependencyProperty.RegisterAttached("ValidationPropertyName", typeof(string),
            typeof(Validation), new(null, OnValidationPresentationPropertyChanged));

    /// <summary>
    /// Gets an enumerable of all active validation errors from the provider.
    /// </summary>
    public static readonly DependencyProperty ErrorsProperty
        = DependencyProperty.RegisterAttached("Errors", typeof(IEnumerable),
            typeof(Validation), new(null, OnValidationPresentationPropertyChanged));

    /// <summary>
    /// Gets or sets a template used to display validation errors
    /// on the attached control. The control must handle showing the
    /// items on its own.
    /// </summary>
    public static readonly DependencyProperty ErrorTemplateProperty
        = DependencyProperty.RegisterAttached("ErrorTemplate", typeof(object),
            typeof(Validation), new(null, OnValidationPresentationPropertyChanged));

    public static string GetValidationPropertyName(DependencyObject obj)
    {
        return (string)obj.GetValue(ValidationPropertyNameProperty);
    }

    public static void SetValidationPropertyName(DependencyObject obj, string value)
    {
        obj.SetValue(ValidationPropertyNameProperty, value);
    }

    public static IEnumerable GetErrors(DependencyObject obj)
    {
        return (IEnumerable)obj.GetValue(ErrorsProperty);
    }

    public static void SetErrors(DependencyObject obj, IEnumerable errors)
    {
        obj.SetValue(ErrorsProperty, errors);
    }

    public static object GetErrorTemplate(DependencyObject obj)
    {
        return obj.GetValue(ErrorTemplateProperty);
    }

    public static void SetErrorTemplate(DependencyObject obj, object value)
    {
        obj.SetValue(ErrorTemplateProperty, value);
    }

    public static INotifyDataErrorInfo GetValidationProvider(DependencyObject obj)
    {
        return (INotifyDataErrorInfo)obj.GetValue(ValidationProviderProperty);
    }

    public static void SetValidationProvider(DependencyObject obj, INotifyDataErrorInfo value)
    {
        obj.SetValue(ValidationProviderProperty, value);
    }

    private static void OnValidationProviderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var state = ValidationStates.GetValue(sender, _ => new ValidationBindingState());

        if (state.Provider != null && state.ErrorsChangedHandler != null)
        {
            state.Provider.ErrorsChanged -= state.ErrorsChangedHandler;
            state.Provider = null;
            state.ErrorsChangedHandler = null;
        }

        sender.SetValue(ErrorsProperty, null);

        if (args.NewValue is INotifyDataErrorInfo info)
        {
            string propName = GetValidationPropertyName(sender);
            if (!string.IsNullOrEmpty(propName))
            {
                EventHandler<DataErrorsChangedEventArgs> handler = (source, eventArgs) =>
                {
                    if (eventArgs.PropertyName == propName)
                    {
                        sender.SetValue(ErrorsProperty, info.GetErrors(propName));
                    }
                };

                state.Provider = info;
                state.ErrorsChangedHandler = handler;
                info.ErrorsChanged += handler;

                sender.SetValue(ErrorsProperty, info.GetErrors(propName));
            }
        }

        EnsureNativeTextBoxValidationPresentation(sender);
    }

    private static void OnValidationPresentationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        EnsureNativeTextBoxValidationPresentation(sender);
    }

    private static void EnsureNativeTextBoxValidationPresentation(DependencyObject sender)
    {
        if (sender is not Control textBox)
        {
            return;
        }

        var state = ValidationStates.GetValue(sender, _ => new ValidationBindingState());

        if (!state.LoadedHooked)
        {
            textBox.Loaded += OnNativeTextBoxLoaded;
            state.LoadedHooked = true;
        }

        UpdateNativeTextBoxValidationPresentation(textBox);
    }

    private static void OnNativeTextBoxLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Control textBox)
        {
            UpdateNativeTextBoxValidationPresentation(textBox);
        }
    }

    private static void UpdateNativeTextBoxValidationPresentation(Control textBox)
    {
        var errorsRepeater = FindNamedChild<ItemsRepeater>(textBox, "ErrorsRepeater");
        if (errorsRepeater == null)
        {
            return;
        }

        bool canValidate = GetValidationProvider(textBox) != null;
        var errorTemplate = GetErrorTemplate(textBox) as IElementFactory;

        if (canValidate && errorTemplate != null)
        {
            errorsRepeater.ItemTemplate = errorTemplate;
            errorsRepeater.ItemsSource = GetErrors(textBox);
            VisualStateManager.GoToState(textBox, "ValidationEnabled", false);
            return;
        }

        errorsRepeater.ItemsSource = null;
        errorsRepeater.ItemTemplate = null;
        VisualStateManager.GoToState(textBox, "NoValidation", false);
    }

    private static T FindNamedChild<T>(DependencyObject root, string name) where T : FrameworkElement
    {
        if (root == null)
        {
            return null;
        }

        int childCount = VisualTreeHelper.GetChildrenCount(root);
        for (int i = 0; i < childCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(root, i);

            if (child is T element && element.Name == name)
            {
                return element;
            }

            T match = FindNamedChild<T>(child, name);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }
}
