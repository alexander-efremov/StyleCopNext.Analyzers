// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1029DoNotSplitNullConditionalOperators"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, remove the whitespace and line breaks between the <c>?</c> and the
    /// following <c>.</c> or <c>[</c>. No fix is offered when a comment, a preprocessor directive, or other
    /// non-whitespace trivia is located between them, because removing it would change the meaning of the code.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1029CodeFixProvider))]
    [Shared]
    internal class SA1029CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1029DoNotSplitNullConditionalOperators.DiagnosticId);

        /// <inheritdoc/>
        public sealed override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var operatorToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!CanFix(operatorToken))
                {
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        SpacingResources.SA1029CodeFix,
                        ct => GetTransformedDocumentAsync(context.Document, diagnostic, ct),
                        nameof(SA1029CodeFixProvider)),
                    diagnostic);
            }
        }

        private static bool CanFix(SyntaxToken operatorToken)
        {
            if (!operatorToken.IsKind(SyntaxKind.QuestionToken))
            {
                return false;
            }

            return ContainsOnlyWhitespaceAndLineBreaks(operatorToken.TrailingTrivia)
                && ContainsOnlyWhitespaceAndLineBreaks(operatorToken.GetNextToken().LeadingTrivia);
        }

        private static bool ContainsOnlyWhitespaceAndLineBreaks(SyntaxTriviaList triviaList)
        {
            foreach (var trivia in triviaList)
            {
                if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia) && !trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    return false;
                }
            }

            return true;
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            return document.WithSyntaxRoot(Fix(root, [diagnostic]));
        }

        private static SyntaxNode Fix(SyntaxNode root, IEnumerable<Diagnostic> diagnostics)
        {
            var replacements = new Dictionary<SyntaxToken, SyntaxToken>();
            foreach (var diagnostic in diagnostics)
            {
                var operatorToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (!CanFix(operatorToken))
                {
                    continue;
                }

                var nextToken = operatorToken.GetNextToken();
                replacements[operatorToken] = operatorToken.WithTrailingTrivia(SyntaxFactory.TriviaList());
                replacements[nextToken] = nextToken.WithLeadingTrivia(SyntaxFactory.TriviaList());
            }

            return root.ReplaceTokens(replacements.Keys, (original, rewritten) => replacements[original]);
        }

        private class FixAll : StyleCopDocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } =
                new FixAll();

            protected override string CodeActionTitle =>
                SpacingResources.SA1029CodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var root = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                return Fix(root, diagnostics);
            }
        }
    }
}
