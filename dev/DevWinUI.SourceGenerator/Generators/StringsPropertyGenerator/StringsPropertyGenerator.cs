using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SystemIO = global::System.IO;

namespace DevWinUI;

[Generator]
internal sealed partial class StringsPropertyGenerator : IIncrementalGenerator
{
    // Static HashSet to track generated file names
    private readonly HashSet<string> _generatedFileNames = [];

    /// <summary>
    /// Initializes the generator and registers source output based on resource files.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var additionalFiles = context
            .AdditionalTextsProvider
            .Where(af => af.Path.Contains(@"en-US\Resources"));

        var compilationProvider = context.CompilationProvider;

        var buildProperties = context.AnalyzerConfigOptionsProvider
            .Select((provider, _) =>
            {
                provider.GlobalOptions.TryGetValue("build_property.ProjectDir", out var projectDir);
                provider.GlobalOptions.TryGetValue($"build_property.{Constants.StringsNamespace}", out var stringsNamespace);

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

    private void Execute(SourceProductionContext ctx, AdditionalText file, Compilation compilation, string @namespace, string projectDir)
    {
        var fileName = SystemIO.Path.GetFileNameWithoutExtension(file.Path);

        lock (_generatedFileNames)
        {
            if (_generatedFileNames.Contains(fileName))
                ctx.ReportDiagnostic(Diagnostic.Create(Constants.DEVGEN1002, Location.None, fileName));

            _ = _generatedFileNames.Add(fileName);
        }

        var projectNamespace = @namespace;
        if (string.IsNullOrEmpty(projectNamespace))
        {
            projectNamespace = compilation.AssemblyName ?? Constants.HelperNamespace;
        }

        var sb = new StringBuilder();

        var sourceFiles = file.Path.Replace(projectDir, "");
        _ = sb.AppendFullHeader($"//{sourceFiles}");
        _ = sb.AppendLine();
        _ = sb.AppendLine($"namespace {projectNamespace};");
        _ = sb.AppendLine();
        _ = sb.AppendLine($"/// <summary>");
        _ = sb.AppendLine($"/// Represents a collection of string resources used throughout the application.");
        _ = sb.AppendLine($"/// </summary>");
        _ = sb.AppendLine($"public sealed partial class {Constants.StringsClassName}");
        _ = sb.AppendLine($"{{");

        foreach (var key in ReadAllKeys(file)) // Write all keys from file
            AddKey(
                buffer: sb,
                key: key.Key,
                comment: key.Comment,
                exampleValue: key.Value
            );

        _ = sb.AppendLine($"}}");

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);

        ctx.AddSource($"{Constants.StringsClassName}.{fileName}.g.cs", sourceText);
    }

    /// <summary>
    /// Adds a constant string key to the buffer with an optional comment.
    /// </summary>
    /// <param name="buffer">The string builder buffer.</param>
    /// <param name="key">The key name.</param>
    /// <param name="comment">Optional comment describing the key.</param>
    /// <param name="value">Optional value assigned to the key. If null, the key will be used as the value.</param>
    /// <param name="exampleValue">Optional example value for the key.</param>
    /// <param name="tabPos">Position of the tab.</param>
    private void AddKey(StringBuilder buffer, string key, string? comment = null, string? value = null, string? exampleValue = null, int tabPos = 1)
    {
        var tabString = Spacing(tabPos);

        if (comment is not null || exampleValue is not null)
        {
            _ = buffer.AppendLine();
            _ = buffer.AppendLine($"{tabString}/// <summary>");

            if (comment is not null)
                _ = buffer.AppendLine($@"{tabString}/// {comment}");

            _ = buffer.AppendLine($"{tabString}/// </summary>");

            if (exampleValue is not null)
            {
                _ = buffer.AppendLine($"{tabString}/// <remarks>");
                _ = buffer.AppendLine($"{tabString}/// e.g.: <b>{exampleValue}</b>");
                _ = buffer.AppendLine($"{tabString}/// </remarks>");
            }
        }

        _ = buffer.AppendLine($@"{tabString}public const string {KeyNameValidator(key)} = ""{value ?? key}"";");
    }

    /// <summary>
    /// Reads all keys from the provided file based on its extension.
    /// </summary>
    /// <param name="file">The additional text file to read keys from.</param>
    /// <returns>An enumerable of <see cref="ParserItem"/> objects containing the keys and their associated values.</returns>
    private IEnumerable<ParserItem> ReadAllKeys(AdditionalText file)
    {
        return SystemIO.Path.GetExtension(file.Path) switch
        {
            ".resw" => ReswParser.GetKeys(file),
            ".json" => JsonParser.GetKeys(file),
            _ => []
        };
    }

    /// <summary>
    /// Validates and returns a valid C# identifier name for the given key.
    /// </summary>
    /// <param name="key">The key to validate.</param>
    /// <returns>A valid C# identifier based on the key.</returns>
    private string KeyNameValidator(string key)
    {
        Span<char> resultSpan = key.Length <= 256 ? stackalloc char[key.Length] : new char[key.Length];
        var keySpan = key.AsSpan();

        for (var i = 0; i < keySpan.Length; i++)
        {
            resultSpan[i] = keySpan[i] switch
            {
                '+' => 'P',
                ' ' or '.' or Constants.ConstantSeparator => '_',
                _ => keySpan[i],
            };
        }

        return resultSpan.ToString();
    }

    internal static string Spacing(int n)
    {
        Span<char> spaces = stackalloc char[n * 4];
        spaces.Fill(' ');

        var sb = new StringBuilder(n * 4);
        foreach (var c in spaces)
            _ = sb.Append(c);

        return sb.ToString();
    }
}
