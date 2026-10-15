using System.Collections.Immutable;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DevWinUI;

[Generator]
internal sealed partial class NavigationPageMapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var additionalFiles = context
            .AdditionalTextsProvider
            .Where(af => af.Path.EndsWith(".json", StringComparison.OrdinalIgnoreCase) && !string.Equals(Path.GetFileName(af.Path), "NativeMethods.json", StringComparison.OrdinalIgnoreCase))
            .Collect();

        var compilationProvider = context.CompilationProvider;

        var buildProperties = context.AnalyzerConfigOptionsProvider
            .Select((provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.ProjectDir", out var projectDir);
                provider.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
                provider.GlobalOptions.TryGetValue($"build_property.{Constants.NavigationMappingsNamespace}", out var stringsNamespace);

                return (projectDir, rootNamespace, stringsNamespace);
            });

        var combined =
            additionalFiles
                .Combine(compilationProvider)
                .Combine(buildProperties);

        context.RegisterSourceOutput(combined, (ctx, data) =>
        {
            var ((files, compilation), (projectDir, rootNamespace, stringsNamespace)) = data;

            Execute(ctx, files, compilation, stringsNamespace, projectDir, rootNamespace);
        });
    }

    private void Execute(SourceProductionContext ctx, ImmutableArray<AdditionalText> files, Compilation compilation, string @namespace, string projectDir, string rootNamespace)
    {
        var parsedFiles = new List<(AdditionalText File, Root Root)>();
        foreach (var file in files
                     .GroupBy(file => file.Path, StringComparer.OrdinalIgnoreCase)
                     .Select(group => group.First())
                     .OrderBy(file => file.Path, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(file => file.Path, StringComparer.Ordinal))
        {
            var jsonText = file.GetText(ctx.CancellationToken)?.ToString();
            if (!TryDeserializeJson(jsonText ?? "", out var root))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor(
                        "DEVGEN1001",
                        "NavigationPageMappings Skipped",
                        $"Skipping {file.Path} because it's not valid JSON for NavigationPageMappings.",
                        "SourceGenerator",
                        DiagnosticSeverity.Warning,
                        isEnabledByDefault: true),
                    Location.None));
                continue;
            }

            parsedFiles.Add((file, root!));
        }

        if (parsedFiles.Count == 0)
            return;

        var primaryFile = parsedFiles
            .FirstOrDefault(item => string.Equals(Path.GetFileNameWithoutExtension(item.File.Path), "AppData", StringComparison.OrdinalIgnoreCase))
            .File ?? parsedFiles[0].File;

        var projectNamespace = @namespace;
        if (string.IsNullOrEmpty(projectNamespace))
        {
            projectNamespace = rootNamespace;

            if (string.IsNullOrEmpty(projectNamespace))
            {
                projectNamespace = compilation.AssemblyName ?? Constants.HelperNamespace;
            }
        }

        var generatedClassNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (file, root) in parsedFiles)
        {
            var className = ReferenceEquals(file, primaryFile)
                ? "NavigationPageMappings"
                : $"NavigationPageMappings{GetIdentifierSuffix(Path.GetFileNameWithoutExtension(file.Path))}";

            var uniqueClassName = className;
            var suffix = 2;
            while (!generatedClassNames.Add(uniqueClassName))
            {
                uniqueClassName = $"{className}{suffix++}";
            }

            var pageNames = new List<string>();
            foreach (var group in root.Groups)
            {
                foreach (var item in group.Items)
                {
                    if (!string.IsNullOrEmpty(item.UniqueId))
                        pageNames.Add(item.UniqueId);
                }
            }

            var sb = new StringBuilder();
            var sourceFiles = file.Path.Replace(projectDir, "");

            _ = sb.AppendFullHeader($"//{sourceFiles}");
            sb.AppendLine();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {projectNamespace};");
            sb.AppendLine($"public partial class {uniqueClassName}");
            sb.AppendLine("{");
            sb.AppendLine("    public static Dictionary<string, Type> PageDictionary { get; } = new Dictionary<string, Type>");
            sb.AppendLine("    {");

            foreach (var pageName in pageNames)
                sb.AppendLine($"        {{\"{pageName}\", typeof({pageName})}},");

            sb.AppendLine("    };");
            sb.AppendLine("}");

            ctx.AddSource($"{uniqueClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }

    private static string GetIdentifierSuffix(string value)
    {
        var suffix = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            suffix.Append(char.IsLetterOrDigit(character) || character == '_' ? character : '_');
        }

        return suffix.Length == 0 ? "File" : suffix.ToString();
    }

    private static bool TryDeserializeJson(string jsonText, out DevWinUI.Root? root)
    {
        root = null;
        if (string.IsNullOrWhiteSpace(jsonText))
            return false;

        try
        {
            root = JsonSerializer.Deserialize(jsonText, typeof(Root), RootContext.Default) as Root;
            return root != null && root.Groups != null;
        }
        catch (JsonException)
        {
            // Skip invalid JSON without failing
            return false;
        }
    }
}
