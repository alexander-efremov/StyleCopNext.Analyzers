// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
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
    /// Implements a code fix for <see cref="SA1114ParameterListMustFollowDeclaration"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, the blank lines between the opening bracket and the first parameter or
    /// argument are removed. Comments and preprocessor directives between the bracket and the first parameter or
    /// argument are preserved.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1114CodeFixProvider))]
    [Shared]
    internal class SA1114CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1114ParameterListMustFollowDeclaration.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return CustomFixAllProviders.BatchFixer;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                SyntaxToken firstToken = root.FindToken(diagnostic.Location.SourceSpan.Start);
                if (RemoveBlankLines(firstToken.LeadingTrivia).Count == firstToken.LeadingTrivia.Count)
                {
                    // Nothing to remove (for example, comments keep the parameter on a later line).
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        ReadabilityResources.SA1114CodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1114CodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxToken firstToken = root.FindToken(diagnostic.Location.SourceSpan.Start);

            List<SyntaxTrivia> newTrivia = RemoveBlankLines(firstToken.LeadingTrivia);
            SyntaxToken updatedToken = firstToken.WithLeadingTrivia(SyntaxFactory.TriviaList(newTrivia));
            SyntaxNode updatedRoot = root.ReplaceToken(firstToken, updatedToken);
            return document.WithSyntaxRoot(updatedRoot);
        }

        private static List<SyntaxTrivia> RemoveBlankLines(SyntaxTriviaList leadingTrivia)
        {
            var newTrivia = new List<SyntaxTrivia>(leadingTrivia.Count);
            bool atLineStart = true;
            for (int i = 0; i < leadingTrivia.Count; i++)
            {
                SyntaxTrivia trivia = leadingTrivia[i];

                if (atLineStart)
                {
                    if (trivia.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        // blank line
                        continue;
                    }

                    if (trivia.IsKind(SyntaxKind.WhitespaceTrivia)
                        && (i + 1) < leadingTrivia.Count
                        && leadingTrivia[i + 1].IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        // whitespace-only line
                        i++;
                        continue;
                    }
                }

                newTrivia.Add(trivia);
                atLineStart = trivia.IsKind(SyntaxKind.EndOfLineTrivia);
            }

            return newTrivia;
        }
    }
}
