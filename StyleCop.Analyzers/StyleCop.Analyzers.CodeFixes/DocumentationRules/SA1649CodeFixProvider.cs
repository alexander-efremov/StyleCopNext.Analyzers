// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.DocumentationRules
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1649FileNameMustMatchTypeName"/>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1649CodeFixProvider))]
    [Shared]
    internal class SA1649CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1649FileNameMustMatchTypeName.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            // The batch fixer can't handle code fixes that create new files
            return null;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            // Replacing a document cannot update the other projects that share the file (Shared Projects and linked
            // files), so the code fix is not offered for them.
            if (!context.Document.GetLinkedDocumentIds().IsEmpty)
            {
                return SpecializedTasks.CompletedTask;
            }

            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        DocumentationResources.SA1649CodeFix,
                        cancellationToken => GetTransformedSolutionAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1649CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static Task<Solution> GetTransformedSolutionAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var expectedFileName = diagnostic.Properties[SA1649FileNameMustMatchTypeName.ExpectedFileNameKey];

            // Rename the document in place so that source control sees a rename instead of a delete and an add.
            return Task.FromResult(document.Project.Solution.WithDocumentName(document.Id, expectedFileName));
        }
    }
}
