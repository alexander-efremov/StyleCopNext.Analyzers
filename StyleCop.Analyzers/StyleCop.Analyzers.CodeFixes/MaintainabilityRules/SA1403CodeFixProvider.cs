// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.MaintainabilityRules
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
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;
    using StyleCop.Analyzers.Helpers;

    /// <summary>
    /// Implements a code fix for <see cref="SA1403FileMayOnlyContainASingleNamespace"/>.
    /// </summary>
    /// <remarks>
    /// <para>To fix a violation of this rule, move each extra namespace into its own file. The new file is placed in
    /// the same folder as the original file and is named after the fully qualified name of the namespace. A nested
    /// namespace is moved as a top-level namespace declared with its fully qualified name, and the using directives
    /// of its enclosing namespaces are carried along.</para>
    /// </remarks>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SA1403CodeFixProvider))]
    [Shared]
    internal class SA1403CodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(SA1403FileMayOnlyContainASingleNamespace.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            // The batch fixer can't handle code fixes that create new files
            return null;
        }

        /// <inheritdoc/>
        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        MaintainabilityResources.SA1403CodeFix,
                        cancellationToken => GetTransformedSolutionAsync(context.Document, diagnostic, cancellationToken),
                        nameof(SA1403CodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private static async Task<Solution> GetTransformedSolutionAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            SyntaxNode node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            NamespaceDeclarationSyntax namespaceDeclaration = node as NamespaceDeclarationSyntax
                ?? node.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();
            if (namespaceDeclaration == null)
            {
                return document.Project.Solution;
            }

            // Mark the namespace so it can be found again after other nodes were removed from the tree
            var annotation = new SyntaxAnnotation();
            SyntaxNode annotatedRoot = root.ReplaceNode(namespaceDeclaration, namespaceDeclaration.WithAdditionalAnnotations(annotation));
            var annotatedNamespace = (NamespaceDeclarationSyntax)annotatedRoot.GetAnnotatedNodes(annotation).Single();

            List<SyntaxNode> nodesToRemoveFromExtracted = new List<SyntaxNode>();
            SyntaxNode previous = annotatedNamespace;
            for (SyntaxNode current = annotatedNamespace.Parent; current != null; previous = current, current = current.Parent)
            {
                foreach (SyntaxNode child in current.ChildNodes())
                {
                    if (child == previous)
                    {
                        continue;
                    }

                    switch (child.Kind())
                    {
                    case SyntaxKind.NamespaceDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.DelegateDeclaration:
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.RecordStructDeclaration:
                        nodesToRemoveFromExtracted.Add(child);
                        break;

                    default:
                        break;
                    }
                }
            }

            SyntaxNode extractedRoot = annotatedRoot.RemoveNodes(nodesToRemoveFromExtracted, SyntaxRemoveOptions.KeepUnbalancedDirectives);
            var extractedNamespace = (NamespaceDeclarationSyntax)extractedRoot.GetAnnotatedNodes(annotation).Single();

            string qualifiedName = GetQualifiedName(extractedNamespace);
            if (extractedNamespace.Parent is NamespaceDeclarationSyntax)
            {
                extractedRoot = HoistNestedNamespace(extractedRoot, extractedNamespace, qualifiedName);
            }

            DocumentId extractedDocumentId = DocumentId.CreateNewId(document.Project.Id);
            string suffix;
            FileNameHelpers.GetFileNameAndSuffix(document.Name, out suffix);
            string extractedDocumentName = qualifiedName + suffix;

            // Add the new file
            Solution updatedSolution = document.Project.Solution.AddDocument(extractedDocumentId, extractedDocumentName, extractedRoot, document.Folders);

            // Make sure to also add the file to linked projects
            foreach (var linkedDocumentId in document.GetLinkedDocumentIds())
            {
                DocumentId linkedExtractedDocumentId = DocumentId.CreateNewId(linkedDocumentId.ProjectId);
                updatedSolution = updatedSolution.AddDocument(linkedExtractedDocumentId, extractedDocumentName, extractedRoot, document.Folders);
            }

            // Remove the namespace from its original location
            updatedSolution = updatedSolution.WithDocumentSyntaxRoot(document.Id, root.RemoveNode(namespaceDeclaration, SyntaxRemoveOptions.KeepUnbalancedDirectives));

            return updatedSolution;
        }

        private static string GetQualifiedName(NamespaceDeclarationSyntax namespaceDeclaration)
        {
            string name = namespaceDeclaration.Name.WithoutTrivia().ToString();
            for (var parent = namespaceDeclaration.Parent as NamespaceDeclarationSyntax; parent != null; parent = parent.Parent as NamespaceDeclarationSyntax)
            {
                name = parent.Name.WithoutTrivia().ToString() + "." + name;
            }

            return name;
        }

        private static SyntaxNode HoistNestedNamespace(SyntaxNode extractedRoot, NamespaceDeclarationSyntax nestedNamespace, string qualifiedName)
        {
            var enclosing = nestedNamespace.Ancestors().OfType<NamespaceDeclarationSyntax>().Reverse().ToList();
            NamespaceDeclarationSyntax outermost = enclosing[0];

            var externs = new List<ExternAliasDirectiveSyntax>();
            var usings = new List<UsingDirectiveSyntax>();
            foreach (NamespaceDeclarationSyntax ancestor in enclosing)
            {
                externs.AddRange(ancestor.Externs);
                usings.AddRange(ancestor.Usings);
            }

            externs.AddRange(nestedNamespace.Externs);
            usings.AddRange(nestedNamespace.Usings);

            NamespaceDeclarationSyntax hoisted = nestedNamespace
                .WithName(SyntaxFactory.ParseName(qualifiedName).WithTriviaFrom(nestedNamespace.Name))
                .WithExterns(SyntaxFactory.List(externs))
                .WithUsings(SyntaxFactory.List(usings))
                .WithLeadingTrivia(outermost.GetLeadingTrivia())
                .WithTrailingTrivia(outermost.GetTrailingTrivia())
                .WithAdditionalAnnotations(Formatter.Annotation);

            return extractedRoot.ReplaceNode(outermost, hoisted);
        }
    }
}
