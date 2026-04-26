using Microsoft.CodeAnalysis;

namespace DevWinUI;

internal static partial class Constants
{
    internal const string StringsClassName = "Strings";
    internal const string HelperNamespace = "DevWinUI";
    internal const string StringsNamespace = "StringsNamespace";
    internal const string NavigationMappingsNamespace = "NavigationMappingsNamespace";
    internal const string BreadcrumbMappingsNamespace = "BreadcrumbMappingsNamespace";
    internal const char ConstantSeparator = '/';
    internal const string ConstantNameProperty = nameof(ConstantNameProperty);
    internal const string CodeFixProviderTitle = $"Replace with constant from {StringsClassName}";

    /// <summary>
    /// Diagnostic descriptor for a scenario where multiple files with the same name are detected.
    /// </summary>
    internal static readonly DiagnosticDescriptor DEVGEN1001 = new(
#pragma warning disable RS2008 // Enable analyzer release tracking
                id: nameof(DEVGEN1001),
#pragma warning restore RS2008 // Enable analyzer release tracking
                title: "Multiple files with the same name detected",
                messageFormat: "Multiple files named '{0}' were detected. Ensure all generated localization string files have unique names.",
                category: "FileGeneration",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "This diagnostic detects cases where multiple localization string files are being generated with the same name," +
                "which can cause conflicts and overwrite issues.");

    /// <summary>
    /// Diagnostic descriptor for a refactoring suggestion to replace string literals with constants from the Strings class.
    /// </summary>
    internal static readonly DiagnosticDescriptor DEVGEN1002 = new(
#pragma warning disable RS2008 // Enable analyzer release tracking
        id: nameof(DEVGEN1002),
#pragma warning restore RS2008 // Enable analyzer release tracking
        title: "String literal can be replaced with constant",
        messageFormat: $"Replace '{{0}}' with '{StringsClassName}.{{1}}'",
        category: "Refactoring",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: $"Detects string literals that can be replaced with constants from the {StringsClassName} class.");

}
