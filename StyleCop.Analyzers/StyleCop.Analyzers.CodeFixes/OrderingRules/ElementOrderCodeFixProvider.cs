// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.OrderingRules
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
    using StyleCop.Analyzers.Helpers;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// Implements code fixes for element ordering rules.
    /// </summary>
    [NoCodeFix("Disabled until stable")]
    [Shared]
    internal class ElementOrderCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(
                SA1201ElementsMustAppearInTheCorrectOrder.DiagnosticId,
                SA1202ElementsMustBeOrderedByAccess.DiagnosticId,
                SA1203ConstantsMustAppearBeforeFields.DiagnosticId,
                SA1204StaticElementsMustAppearBeforeInstanceElements.DiagnosticId,
                SA1214ReadonlyElementsMustAppearBeforeNonReadonlyElements.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider()
        {
            return FixAll.Instance;
        }

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(context.Document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, context.CancellationToken);

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                if (memberDeclaration == null
                    || UpdateSyntaxRoot(memberDeclaration, settings.OrderingRules.ElementOrder, syntaxRoot, settings.Indentation) == syntaxRoot)
                {
                    // Nothing can be moved, for example because the move would cross a preprocessor directive.
                    continue;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        OrderingResources.ElementOrderCodeFix,
                        cancellationToken => GetTransformedDocumentAsync(context.Document, diagnostic, cancellationToken),
                        nameof(ElementOrderCodeFixProvider)),
                    diagnostic);
            }
        }

        private static async Task<Document> GetTransformedDocumentAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, cancellationToken);
            var elementOrder = settings.OrderingRules.ElementOrder;

            var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
            if (memberDeclaration == null)
            {
                return document;
            }

            syntaxRoot = UpdateSyntaxRoot(memberDeclaration, elementOrder, syntaxRoot, settings.Indentation);

            return document.WithSyntaxRoot(syntaxRoot);
        }

        private static SyntaxNode UpdateSyntaxRoot(MemberDeclarationSyntax memberDeclaration, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings)
        {
            var parentDeclaration = memberDeclaration.Parent;
            var memberToMove = new MemberOrderHelper(memberDeclaration, elementOrder);

            if (parentDeclaration is TypeDeclarationSyntax)
            {
                return HandleTypeDeclaration(memberToMove, (TypeDeclarationSyntax)parentDeclaration, elementOrder, syntaxRoot, indentationSettings);
            }

            if (parentDeclaration is BaseNamespaceDeclarationSyntax)
            {
                return HandleBaseNamespaceDeclaration(memberToMove, (BaseNamespaceDeclarationSyntax)parentDeclaration, elementOrder, syntaxRoot, indentationSettings);
            }

            if (parentDeclaration is CompilationUnitSyntax)
            {
                return HandleCompilationUnitDeclaration(memberToMove, (CompilationUnitSyntax)parentDeclaration, elementOrder, syntaxRoot, indentationSettings);
            }

            return syntaxRoot;
        }

        private static SyntaxNode HandleTypeDeclaration(MemberOrderHelper memberOrder, TypeDeclarationSyntax typeDeclarationNode, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings)
        {
            return OrderMember(memberOrder, typeDeclarationNode.Members, elementOrder, syntaxRoot, indentationSettings);
        }

        private static SyntaxNode HandleCompilationUnitDeclaration(MemberOrderHelper memberOrder, CompilationUnitSyntax compilationUnitDeclaration, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings)
        {
            return OrderMember(memberOrder, compilationUnitDeclaration.Members, elementOrder, syntaxRoot, indentationSettings);
        }

        private static SyntaxNode HandleBaseNamespaceDeclaration(MemberOrderHelper memberOrder, BaseNamespaceDeclarationSyntax namespaceDeclaration, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings)
        {
            return OrderMember(memberOrder, namespaceDeclaration.Members, elementOrder, syntaxRoot, indentationSettings);
        }

        private static SyntaxNode OrderMember(MemberOrderHelper memberOrder, SyntaxList<MemberDeclarationSyntax> members, ImmutableArray<OrderingTrait> elementOrder, SyntaxNode syntaxRoot, IndentationSettings indentationSettings)
        {
            var memberIndex = members.IndexOf(memberOrder.Member);
            MemberOrderHelper target = default;
            var targetIndex = -1;

            for (var i = memberIndex - 1; i >= 0; --i)
            {
                var orderHelper = new MemberOrderHelper(members[i], elementOrder);
                if (orderHelper.Priority < memberOrder.Priority)
                {
                    target = orderHelper;
                    targetIndex = i;
                }
                else
                {
                    break;
                }
            }

            if (target.Member == null || HasDirectiveTrivia(members, targetIndex, memberIndex) || ReordersInitializers(members, targetIndex, memberIndex))
            {
                return syntaxRoot;
            }

            return MoveMember(syntaxRoot, memberOrder.Member, target.Member, indentationSettings);
        }

        /// <summary>
        /// Determines whether any member in the inclusive range has preprocessor directive trivia (for example
        /// <c>#if</c> or <c>#region</c>) attached to it. Moving a member across such a directive would change the
        /// conditional compilation block or region the member belongs to, so such moves are not performed.
        /// </summary>
        private static bool HasDirectiveTrivia(SyntaxList<MemberDeclarationSyntax> members, int firstIndex, int lastIndex)
        {
            for (var i = firstIndex; i <= lastIndex; i++)
            {
                if (members[i].ContainsDirectives)
                {
                    foreach (var trivia in members[i].GetLeadingTrivia())
                    {
                        if (trivia.IsDirective)
                        {
                            return true;
                        }
                    }

                    foreach (var trivia in members[i].GetTrailingTrivia())
                    {
                        if (trivia.IsDirective)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether moving the member at <paramref name="memberIndex"/> before the member at
        /// <paramref name="targetIndex"/> changes the relative order of initializers that run at the same time (static
        /// initializers run in the static constructor, instance initializers in each constructor, both in textual
        /// order). Such a move could silently change behavior, so it is not performed.
        /// </summary>
        private static bool ReordersInitializers(SyntaxList<MemberDeclarationSyntax> members, int targetIndex, int memberIndex)
        {
            if (!HasInitializer(members[memberIndex], out var isStatic))
            {
                return false;
            }

            for (var i = targetIndex; i < memberIndex; i++)
            {
                if (HasInitializer(members[i], out var otherIsStatic) && otherIsStatic == isStatic)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasInitializer(MemberDeclarationSyntax member, out bool isStatic)
        {
            VariableDeclarationSyntax declaration;
            switch (member)
            {
            case FieldDeclarationSyntax field when !field.Modifiers.Any(SyntaxKind.ConstKeyword):
                declaration = field.Declaration;
                break;

            case EventFieldDeclarationSyntax eventField:
                declaration = eventField.Declaration;
                break;

            case PropertyDeclarationSyntax property when property.Initializer != null:
                isStatic = property.Modifiers.Any(SyntaxKind.StaticKeyword);
                return true;

            default:
                isStatic = false;
                return false;
            }

            isStatic = ((BaseFieldDeclarationSyntax)member).Modifiers.Any(SyntaxKind.StaticKeyword);
            foreach (var variable in declaration.Variables)
            {
                if (variable.Initializer != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static SyntaxNode MoveMember(SyntaxNode syntaxRoot, MemberDeclarationSyntax member, MemberDeclarationSyntax targetMember, IndentationSettings indentationSettings)
        {
            var firstToken = syntaxRoot.GetFirstToken();
            var fileHeader = GetFileHeader(firstToken.LeadingTrivia);
            syntaxRoot = syntaxRoot.TrackNodes(member, targetMember, firstToken.Parent);
            var memberToMove = syntaxRoot.GetCurrentNode(member);
            var targetMemberTracked = syntaxRoot.GetCurrentNode(targetMember);
            if (!memberToMove.HasLeadingTrivia)
            {
                var targetIndentationLevel = IndentationHelper.GetIndentationSteps(indentationSettings, targetMember);
                var indentationString = IndentationHelper.GenerateIndentationString(indentationSettings, targetIndentationLevel);
                memberToMove = memberToMove.WithLeadingTrivia(SyntaxFactory.Whitespace(indentationString));
            }

            if (!HasLeadingBlankLines(targetMember)
                && HasLeadingBlankLines(member))
            {
                memberToMove = memberToMove.WithTrailingTrivia(memberToMove.GetTrailingTrivia().Add(SyntaxFactory.CarriageReturnLineFeed));
                memberToMove = memberToMove.WithLeadingTrivia(GetLeadingTriviaWithoutLeadingBlankLines(memberToMove));
            }

            syntaxRoot = syntaxRoot.InsertNodesBefore(targetMemberTracked, new[] { memberToMove });
            var fieldToMoveTracked = syntaxRoot.GetCurrentNodes(member).Last();
            syntaxRoot = syntaxRoot.RemoveNode(fieldToMoveTracked, SyntaxRemoveOptions.KeepNoTrivia);
            if (fileHeader.Any())
            {
                var oldFirstToken = syntaxRoot.GetCurrentNode(firstToken.Parent).ChildTokens().First();
                syntaxRoot = syntaxRoot.ReplaceToken(oldFirstToken, oldFirstToken.WithLeadingTrivia(StripFileHeader(oldFirstToken.LeadingTrivia)));
                var newFirstToken = syntaxRoot.GetFirstToken();
                syntaxRoot = syntaxRoot.ReplaceToken(newFirstToken, newFirstToken.WithLeadingTrivia(fileHeader.AddRange(newFirstToken.LeadingTrivia)));
            }

            return syntaxRoot;
        }

        private static SyntaxTriviaList StripFileHeader(SyntaxTriviaList newLeadingTrivia)
        {
            var fileHeader = GetFileHeader(newLeadingTrivia);
            return SyntaxTriviaList.Empty.AddRange(newLeadingTrivia.Skip(fileHeader.Count));
        }

        private static SyntaxTriviaList GetFileHeader(SyntaxTriviaList newLeadingTrivia)
        {
            var onBlankLine = false;
            var hasHeader = false;
            var fileHeader = new List<SyntaxTrivia>();
            for (var i = 0; i < newLeadingTrivia.Count; i++)
            {
                bool done = false;
                switch (newLeadingTrivia[i].Kind())
                {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    fileHeader.Add(newLeadingTrivia[i]);
                    onBlankLine = false;
                    hasHeader = true;
                    break;

                case SyntaxKind.WhitespaceTrivia:
                    fileHeader.Add(newLeadingTrivia[i]);
                    break;

                case SyntaxKind.EndOfLineTrivia:
                    fileHeader.Add(newLeadingTrivia[i]);

                    if (onBlankLine)
                    {
                        done = true;
                    }
                    else
                    {
                        onBlankLine = true;
                    }

                    break;

                default:
                    done = true;
                    break;
                }

                if (done)
                {
                    break;
                }
            }

            return hasHeader ? SyntaxTriviaList.Empty.AddRange(fileHeader) : SyntaxTriviaList.Empty;
        }

        private static bool HasLeadingBlankLines(SyntaxNode node)
        {
            var firstTriviaIgnoringWhitespace = node.GetLeadingTrivia().FirstOrDefault(x => !x.IsKind(SyntaxKind.WhitespaceTrivia));
            return firstTriviaIgnoringWhitespace.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        private static SyntaxTriviaList GetLeadingTriviaWithoutLeadingBlankLines(SyntaxNode node)
        {
            var leadingTrivia = node.GetLeadingTrivia();

            var skipIndex = 0;
            for (var i = 0; i < leadingTrivia.Count; i++)
            {
                var currentTrivia = leadingTrivia[i];
                if (currentTrivia.IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    skipIndex = i + 1;
                }
                else if (!currentTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    // Preceded by whitespace
                    skipIndex = i > 0 && leadingTrivia[i - 1].IsKind(SyntaxKind.WhitespaceTrivia) ? i - 1 : i;
                    break;
                }
            }

            return SyntaxFactory.TriviaList(leadingTrivia.Skip(skipIndex));
        }

        private class FixAll : StyleCopDocumentBasedFixAllProvider
        {
            public static FixAllProvider Instance { get; } = new FixAll();

            protected override string CodeActionTitle => OrderingResources.ElementOrderCodeFix;

            protected override async Task<SyntaxNode> FixAllInDocumentAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
            {
                if (diagnostics.IsEmpty)
                {
                    return null;
                }

                var syntaxRoot = await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false);
                var settings = SettingsHelper.GetStyleCopSettingsInCodeFix(document.Project.AnalyzerOptions, syntaxRoot.SyntaxTree, fixAllContext.CancellationToken);
                var elementOrder = settings.OrderingRules.ElementOrder;

                var trackedDiagnosticMembers = new List<MemberDeclarationSyntax>();
                foreach (var diagnostic in diagnostics)
                {
                    var memberDeclaration = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<MemberDeclarationSyntax>();
                    if (memberDeclaration == null)
                    {
                        continue;
                    }

                    trackedDiagnosticMembers.Add(memberDeclaration);
                }

                syntaxRoot = syntaxRoot.TrackNodes(trackedDiagnosticMembers);

                foreach (var member in trackedDiagnosticMembers)
                {
                    var memberDeclaration = syntaxRoot.GetCurrentNode(member);
                    syntaxRoot = UpdateSyntaxRoot(memberDeclaration, elementOrder, syntaxRoot, settings.Indentation);
                }

                return syntaxRoot;
            }
        }
    }
}
