// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements a code fix for <see cref="SA1138IndentElementsCorrectly"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1138CodeFixProvider))]
    [Shared]
    internal class SA1138CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1138IndentElementsCorrectly.DiagnosticId);

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider() =>
            FixAll.Instance;

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.IndentationCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1138CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxTree, cancellationToken);
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            if (!TryGetTextChange(settings.Indentation, text, diagnostic, out TextChange textChange))
            {
                return document;
            }

            return document.WithText(text.WithChanges(textChange));
        }

        private static bool TryGetTextChange(IndentationSettings indentationSettings, SourceText text, Diagnostic diagnostic, out TextChange textChange)
        {
            textChange = default;

            if (!diagnostic.Properties.TryGetValue(SA1138IndentElementsCorrectly.ExpectedIndentationKey, out string expectedText)
                || !int.TryParse(expectedText, NumberStyles.None, CultureInfo.InvariantCulture, out int expectedWidth))
            {
                return false;
            }

            // The diagnostic is reported either on the leading whitespace of the line or, when there is none, on the
            // first token of the line.
            TextLine line = text.Lines.GetLineFromPosition(diagnostic.Location.SourceSpan.Start);
            int end = line.Start;
            while (end < line.End && (text[end] == ' ' || text[end] == '\t'))
            {
                end++;
            }

            string indentation = IndentationHelper.GenerateIndentationStringForWidth(indentationSettings, expectedWidth);
            textChange = new TextChange(TextSpan.FromBounds(line.Start, end), indentation);
            return true;
        }

        private class FixAll : StyleCopDocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; }
                = new FixAll();

            protected override string CodeActionTitle =>
                ReadabilityResources.IndentationCodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxTree = await document.GetSyntaxTreeAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxTree, fixAllContext.CancellationToken);
                var text = await document.GetTextAsync(fixAllContext.CancellationToken).ConfigureAwait(false);

                var changes = new List<TextChange>();
                foreach (var diagnostic in diagnostics)
                {
                    if (TryGetTextChange(settings.Indentation, text, diagnostic, out TextChange textChange))
                    {
                        changes.Add(textChange);
                    }
                }

                changes.Sort(static (left, right) => left.Span.Start.CompareTo(right.Span.Start));

                return await syntaxTree.WithChangedText(text.WithChanges(changes)).GetRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
            }
        }
    }
}
