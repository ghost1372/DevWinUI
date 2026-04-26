using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DevWinUI;

/// <summary>
/// Code fix provider that replaces string literals with constants from the <c>Strings</c> class.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(StringsPropertyCodeFixProvider)), Shared]
internal sealed class StringsPropertyCodeFixProvider : CodeFixProvider
{
    /// <summary>
    /// Gets a list of diagnostic IDs that this provider can fix.
    /// </summary>
    public sealed override ImmutableArray<string> FixableDiagnosticIds => [Constants.DEVGEN1002.Id];

    /// <summary>
    /// Gets the fix all provider for this code fix provider.
    /// </summary>
    /// <returns>A <see cref="FixAllProvider"/>.</returns>
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <summary>
    /// Registers code fixes for the specified diagnostic.
    /// </summary>
    /// <param name="context">The context for the code fix.</param>
    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null)
            return;

        var diagnosticSpan = diagnostic.Location.SourceSpan;

        SyntaxNode? node = null;

        if (root.FindNode(diagnosticSpan) is LiteralExpressionSyntax literalExpression)
            node = literalExpression;

        if (root.FindNode(diagnosticSpan) is InterpolatedStringTextSyntax interpolatedStringText)
            node = interpolatedStringText.Parent;

        if (node is null)
            return;

        var constantName = diagnostic.Properties[Constants.ConstantNameProperty];
        var newExpression = SyntaxFactory.ParseExpression($"{Constants.StringsClassName}.{constantName}").WithTriviaFrom(node);
        var newRoot = root.ReplaceNode(node, newExpression);
        var newDocument = context.Document.WithSyntaxRoot(newRoot);

#pragma warning disable RS1010 // Create code actions should have a unique EquivalenceKey for FixAll occurrences support
        context.RegisterCodeFix(
            CodeAction.Create(
                Constants.CodeFixProviderTitle,
                c => Task.FromResult(newDocument),
                null),
            diagnostic);
#pragma warning restore RS1010 // Create code actions should have a unique EquivalenceKey for FixAll occurrences support
    }
}
