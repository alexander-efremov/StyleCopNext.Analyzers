// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// This class provides a code fix for the SA1102 diagnostic.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1102CodeFixProvider))]
    [Shared]
    internal class SA1102CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA110xQueryClauses.SA1102Descriptor.Id);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1102CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1102CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var text = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);
            var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);

            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);
            var indentationTrivia = QueryIndentationHelpers.GetQueryIndentationTrivia(settings.Indentation, token);

            var precedingToken = token.GetPreviousToken();
            var newPrecedingToken = precedingToken;
            var options = document.Project.Solution.Workspace.Options;
            var endOfLineTrivia = FormattingHelper.GetEndOfLineForCodeFix(token, text, options);

            if (!precedingToken.TrailingTrivia.Any(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) || trivia.IsKind(SyntaxKind.MultiLineCommentTrivia)))
            {
                newPrecedingToken = precedingToken.WithTrailingTrivia(endOfLineTrivia);
            }

            // Remove only the empty lines; comments and directives between the clauses are kept as written.
            var newLeadingTrivia = new List<SyntaxTrivia>();
            var currentLine = new List<SyntaxTrivia>();
            foreach (var trivia in token.LeadingTrivia)
            {
                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    if (!IsWhitespaceOnly(currentLine))
                    {
                        newLeadingTrivia.AddRange(currentLine);
                        newLeadingTrivia.Add(trivia);
                    }

                    currentLine.Clear();
                }
                else if (trivia.IsDirective)
                {
                    newLeadingTrivia.AddRange(currentLine);
                    newLeadingTrivia.Add(trivia);
                    currentLine.Clear();
                }
                else
                {
                    currentLine.Add(trivia);
                }
            }

            newLeadingTrivia.Add(indentationTrivia);

            var replaceMap = new Dictionary<SyntaxToken, SyntaxToken>()
            {
                [precedingToken] = newPrecedingToken,
                [token] = token.WithLeadingTrivia(newLeadingTrivia),
            };

            var newSyntaxRoot = syntaxRoot.ReplaceTokens(replaceMap.Keys, (t1, t2) => replaceMap[t1]).WithoutFormatting();
            return document.WithSyntaxRoot(newSyntaxRoot);
        }

        private static bool IsWhitespaceOnly(List<SyntaxTrivia> trivia)
        {
            foreach (var item in trivia)
            {
                if (!item.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
