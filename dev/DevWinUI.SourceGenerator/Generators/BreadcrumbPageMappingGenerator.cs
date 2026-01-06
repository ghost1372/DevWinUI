using System.Collections.Immutable;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DevWinUI;

[Generator]
internal sealed class BreadcrumbPageMappingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var additionalFiles = context.AdditionalTextsProvider
            .Where(at =>
                at.Path.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) &&
                !at.Path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") &&
                !at.Path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
            )
            .Collect();

        var compilationProvider = context.CompilationProvider;

        var buildProperties = context.AnalyzerConfigOptionsProvider
            .Select((provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.ProjectDir", out var projectDir);
                provider.GlobalOptions.TryGetValue($"build_property.{Constants.BreadcrumbMappingsNamespace}", out var stringsNamespace);

                return (projectDir, stringsNamespace);
            });

        var combined =
            additionalFiles
                .Combine(compilationProvider)
                .Combine(buildProperties);

        context.RegisterSourceOutput(combined, (ctx, data) =>
        {
            var ((files, compilation), (projectDir, stringsNamespace)) = data;

            Execute(ctx, files, compilation, stringsNamespace, projectDir);
        });
    }

    private void Execute(SourceProductionContext ctx, ImmutableArray<AdditionalText> files, Compilation compilation, string @namespace, string projectDir)
    {
        var entries = new List<string>();
        var fileNamesStringBuilder = new StringBuilder();

        foreach (var file in files)
        {
            string content;
            try
            {
                content = file.GetText(ctx.CancellationToken)?.ToString() ?? "";
            }
            catch { continue; }

            XDocument xamlDoc;
            try { xamlDoc = XDocument.Parse(content); }
            catch { continue; }

            var root = xamlDoc.Root;
            if (root == null) continue;

            // Skip non-page XAML
            if (root.Name.LocalName == "Window" || root.Name.LocalName == "Application" || root.Name.LocalName == "ResourceDictionary")
                continue;

            var xClassAttr = root.Attribute(XName.Get("Class", "http://schemas.microsoft.com/winfx/2006/xaml"));
            if (xClassAttr == null) continue;

            string pageType = xClassAttr.Value;
            string? pageTitle = GetAttachedProperty(root, "BreadcrumbNavigator.PageTitle");
            bool? isHeaderVisible = GetAttachedPropertyNullableBool(root, "BreadcrumbNavigator.IsHeaderVisible");
            bool? clearNavigation = GetAttachedPropertyNullableBool(root, "BreadcrumbNavigator.ClearCache");

            // If NONE of them exist → skip this page
            if (pageTitle is null && isHeaderVisible is null && clearNavigation is null)
                continue;

            // Apply defaults for missing values
            bool finalIsHeaderVisible = isHeaderVisible ?? false;
            bool finalClearNavigation = clearNavigation ?? false;

            entries.Add(
                $"{{ typeof({pageType}), new BreadcrumbPageConfig " +
                $"{{ PageTitle = {(pageTitle == null ? "null" : $"\"{pageTitle}\"")}, " +
                $"IsHeaderVisible = {finalIsHeaderVisible.ToString().ToLower()}, " +
                $"ClearNavigation = {finalClearNavigation.ToString().ToLower()} }} }}"
            );

            fileNamesStringBuilder.AppendLine($"//{file.Path}");
        }

        if (entries.Count == 0) return;

        var projectNamespace = @namespace;
        if (string.IsNullOrEmpty(projectNamespace))
        {
            projectNamespace = compilation.AssemblyName ?? Constants.HelperNamespace;
        }

        var sb = new StringBuilder();
        var sourceFiles = fileNamesStringBuilder.ToString().Replace(projectDir, "");

        _ = sb.AppendFullHeader(sourceFiles);
        sb.AppendLine();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine($"namespace {projectNamespace};");
        sb.AppendLine("public partial class BreadcrumbPageMappings");
        sb.AppendLine("{");
        sb.AppendLine("    public static Dictionary<Type, BreadcrumbPageConfig> PageDictionary = new()");
        sb.AppendLine("    {");

        foreach (var e in entries)
            sb.AppendLine($"        {e},");

        sb.AppendLine("    };");
        sb.AppendLine("}");

        ctx.AddSource("BreadcrumbPageMappings.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static string? GetAttachedProperty(XElement element, string propertyName)
    {
        foreach (var attr in element.Attributes())
        {
            if (attr.Name.LocalName.EndsWith(propertyName))
                return attr.Value;
        }
        return null;
    }

    private static bool? GetAttachedPropertyNullableBool(XElement element, string propertyName)
    {
        var val = GetAttachedProperty(element, propertyName);
        if (string.IsNullOrEmpty(val)) return null;

        return val.Equals("True", StringComparison.OrdinalIgnoreCase);
    }

}
