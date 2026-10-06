// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.ReadabilityRules
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Settings.ObjectModel;

    /// <summary>
    /// A line of code is not indented the correct amount, according to the currently applied style settings for the
    /// project.
    /// </summary>
    /// <remarks>
    /// <para>The rule checks the first token of every statement and member that starts its own line inside of a block,
    /// a type body, a namespace body, an accessor list, an enum body, or a switch section. The expected indentation is
    /// derived from the (expected) indentation of the line which starts the containing construct.</para>
    ///
    /// <para>The rule is intentionally conservative. Continuation lines of multi-line expressions, lines which start
    /// inside of multi-line literals, lines preceded by a comment on the same line, comment-only lines, and blocks
    /// whose owner cannot be determined unambiguously (for example blocks of lambda expressions) are not
    /// reported.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1138IndentElementsCorrectly : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1138IndentElementsCorrectly"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1138";

        /// <summary>
        /// The key of the diagnostic property which contains the expected indentation width in columns.
        /// </summary>
        public const string ExpectedIndentationKey = "ExpectedIndentation";

        private const string HelpLink = "https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/2.x/documentation/SA1138.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(ReadabilityResources.SA1138Title), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(ReadabilityResources.SA1138MessageFormat), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(ReadabilityResources.SA1138Description), ReadabilityResources.ResourceManager, typeof(ReadabilityResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.ReadabilityRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(
                context =>
                {
                    context.RegisterSyntaxTreeAction(HandleSyntaxTree);
                });
        }

        private static void HandleSyntaxTree(SyntaxTreeAnalysisContext context, StyleCopSettings settings)
        {
            var root = context.Tree.GetRoot(context.CancellationToken);
            var checker = new IndentationChecker(context, context.Tree.GetText(context.CancellationToken), settings.Indentation);

            foreach (var node in root.DescendantNodesAndSelf())
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                checker.Visit(node);
            }
        }

        private sealed class IndentationChecker
        {
            private readonly SyntaxTreeAnalysisContext context;
            private readonly SourceText text;
            private readonly IndentationSettings settings;

            // The expected indentation of every line whose first token is an element which has been checked. Nodes are
            // visited parents first, so the expected indentation of a construct is known before its contents are
            // checked, and a misindented parent does not cause its children to be reported relative to a wrong
            // position.
            private readonly Dictionary<int, int> expectedByLine = new Dictionary<int, int>();

            public IndentationChecker(SyntaxTreeAnalysisContext context, SourceText text, IndentationSettings settings)
            {
                this.context = context;
                this.text = text;
                this.settings = settings;
            }

            public void Visit(SyntaxNode node)
            {
                switch (node)
                {
                case CompilationUnitSyntax compilationUnit:
                    this.VisitCompilationUnit(compilationUnit);
                    break;

                case BaseNamespaceDeclarationSyntax namespaceDeclaration:
                    this.VisitNamespace(namespaceDeclaration);
                    break;

                case TypeDeclarationSyntax typeDeclaration:
                    this.VisitTypeBody(typeDeclaration);
                    break;

                case EnumDeclarationSyntax enumDeclaration:
                    this.VisitEnum(enumDeclaration);
                    break;

                case AccessorListSyntax accessorList:
                    this.VisitAccessorList(accessorList);
                    break;

                case BlockSyntax block:
                    this.VisitBlock(block);
                    break;

                case SwitchStatementSyntax switchStatement:
                    this.VisitSwitch(switchStatement);
                    break;

                default:
                    break;
                }
            }

            private static SyntaxToken GetFirstTokenAfterAttributes(SyntaxNode node)
            {
                foreach (var child in node.ChildNodesAndTokens())
                {
                    if (child.IsNode && child.AsNode() is AttributeListSyntax)
                    {
                        continue;
                    }

                    return child.IsToken ? child.AsToken() : child.AsNode().GetFirstToken();
                }

                return node.GetFirstToken();
            }

            private static SyntaxToken GetAnchorToken(SyntaxNode owner)
            {
                switch (owner)
                {
                case ElseClauseSyntax elseClause when elseClause.Parent is IfStatementSyntax:
                    return GetAnchorToken(elseClause.Parent);

                case IfStatementSyntax ifStatement when ifStatement.Parent is ElseClauseSyntax:
                    return GetAnchorToken(ifStatement.Parent);

                case CatchClauseSyntax when owner.Parent is TryStatementSyntax:
                case FinallyClauseSyntax when owner.Parent is TryStatementSyntax:
                    return GetAnchorToken(owner.Parent);

                default:
                    return GetFirstTokenAfterAttributes(owner);
                }
            }

            private void VisitCompilationUnit(CompilationUnitSyntax compilationUnit)
            {
                foreach (var externAlias in compilationUnit.Externs)
                {
                    this.CheckToken(externAlias.GetFirstToken(), 0);
                }

                foreach (var usingDirective in compilationUnit.Usings)
                {
                    this.CheckToken(usingDirective.GetFirstToken(), 0);
                }

                foreach (var attributeList in compilationUnit.AttributeLists)
                {
                    this.CheckToken(attributeList.GetFirstToken(), 0);
                }

                foreach (var member in compilationUnit.Members)
                {
                    this.CheckElement(member, 0);
                }
            }

            private void VisitNamespace(BaseNamespaceDeclarationSyntax namespaceDeclaration)
            {
                int? reference = this.GetReferenceIndentation(namespaceDeclaration.NamespaceKeyword);
                if (reference is null)
                {
                    return;
                }

                int expected = reference.Value;
                if (namespaceDeclaration is NamespaceDeclarationSyntax blockNamespace)
                {
                    this.CheckBraces(blockNamespace.OpenBraceToken, blockNamespace.CloseBraceToken, reference.Value);
                    expected += this.settings.IndentationSize;
                }

                foreach (var externAlias in namespaceDeclaration.Externs)
                {
                    this.CheckToken(externAlias.GetFirstToken(), expected);
                }

                foreach (var usingDirective in namespaceDeclaration.Usings)
                {
                    this.CheckToken(usingDirective.GetFirstToken(), expected);
                }

                foreach (var member in namespaceDeclaration.Members)
                {
                    this.CheckElement(member, expected);
                }
            }

            private void VisitTypeBody(TypeDeclarationSyntax typeDeclaration)
            {
                int? reference = this.GetReferenceIndentation(GetFirstTokenAfterAttributes(typeDeclaration));
                if (reference is null)
                {
                    return;
                }

                this.CheckBraces(typeDeclaration.OpenBraceToken, typeDeclaration.CloseBraceToken, reference.Value);
                int expected = reference.Value + this.settings.IndentationSize;
                foreach (var member in typeDeclaration.Members)
                {
                    this.CheckElement(member, expected);
                }
            }

            private void VisitEnum(EnumDeclarationSyntax enumDeclaration)
            {
                int? reference = this.GetReferenceIndentation(GetFirstTokenAfterAttributes(enumDeclaration));
                if (reference is null)
                {
                    return;
                }

                this.CheckBraces(enumDeclaration.OpenBraceToken, enumDeclaration.CloseBraceToken, reference.Value);
                int expected = reference.Value + this.settings.IndentationSize;
                foreach (var member in enumDeclaration.Members)
                {
                    this.CheckElement(member, expected);
                }
            }

            private void VisitAccessorList(AccessorListSyntax accessorList)
            {
                if (accessorList.Parent is null)
                {
                    return;
                }

                int? reference = this.GetReferenceIndentation(GetAnchorToken(accessorList.Parent));
                if (reference is null)
                {
                    return;
                }

                this.CheckBraces(accessorList.OpenBraceToken, accessorList.CloseBraceToken, reference.Value);
                int expected = reference.Value + this.settings.IndentationSize;
                foreach (var accessor in accessorList.Accessors)
                {
                    this.CheckElement(accessor, expected);
                }
            }

            private void VisitBlock(BlockSyntax block)
            {
                int? reference;
                bool checkBraces = true;
                switch (block.Parent)
                {
                case SwitchSectionSyntax:
                    // The position of a block directly inside of a switch section is ambiguous, so only the actual
                    // indentation of its own line is used as the reference.
                    reference = this.TryGetLeadingWidth(block.OpenBraceToken, out int braceWidth, out _) ? braceWidth : null;
                    checkBraces = false;
                    break;

                case StatementSyntax:
                case BaseMethodDeclarationSyntax:
                case AccessorDeclarationSyntax:
                case ElseClauseSyntax:
                case CatchClauseSyntax:
                case FinallyClauseSyntax:
                    // Blocks which are owned by labels are ambiguous, since labels may be positioned differently from
                    // other statements.
                    if (block.Parent is LabeledStatementSyntax)
                    {
                        return;
                    }

                    // A block which is a statement of another block is an element itself, and starts at its own brace.
                    reference = this.GetReferenceIndentation(block.Parent is BlockSyntax ? block.OpenBraceToken : GetAnchorToken(block.Parent));
                    break;

                default:
                    // Blocks of lambda expressions and anonymous methods are part of an expression, and might be
                    // indented relative to a continuation line.
                    return;
                }

                if (reference is null)
                {
                    return;
                }

                if (checkBraces)
                {
                    this.CheckBraces(block.OpenBraceToken, block.CloseBraceToken, reference.Value);
                }

                int expected = reference.Value + (this.settings.IndentBlock ? this.settings.IndentationSize : 0);
                foreach (var statement in block.Statements)
                {
                    this.CheckStatement(statement, expected, skipBlocks: false);
                }
            }

            private void VisitSwitch(SwitchStatementSyntax switchStatement)
            {
                int? reference = this.GetReferenceIndentation(GetAnchorToken(switchStatement));
                if (reference is null)
                {
                    return;
                }

                this.CheckBraces(switchStatement.OpenBraceToken, switchStatement.CloseBraceToken, reference.Value);
                int labelIndentation = reference.Value + (this.settings.IndentSwitchSection ? this.settings.IndentationSize : 0);
                int statementIndentation = labelIndentation + (this.settings.IndentSwitchCaseSection ? this.settings.IndentationSize : 0);

                foreach (var section in switchStatement.Sections)
                {
                    foreach (var label in section.Labels)
                    {
                        this.CheckToken(label.GetFirstToken(), labelIndentation);
                    }

                    foreach (var statement in section.Statements)
                    {
                        this.CheckStatement(statement, statementIndentation, skipBlocks: true);
                    }
                }
            }

            private int GetLabelIndentation(int statementIndentation)
            {
                switch (this.settings.LabelPositioning)
                {
                case LabelPositioning.LeftMost:
                    return 0;

                case LabelPositioning.OneLess:
                    return Math.Max(0, statementIndentation - this.settings.IndentationSize);

                default:
                    return statementIndentation;
                }
            }

            private void CheckBraces(SyntaxToken openBrace, SyntaxToken closeBrace, int expected)
            {
                this.CheckToken(openBrace, expected);
                this.CheckToken(closeBrace, expected);
            }

            private void CheckStatement(StatementSyntax statement, int expected, bool skipBlocks)
            {
                while (statement is LabeledStatementSyntax labeledStatement)
                {
                    this.CheckToken(labeledStatement.Identifier, this.GetLabelIndentation(expected));
                    statement = labeledStatement.Statement;
                }

                if (skipBlocks && statement is BlockSyntax)
                {
                    return;
                }

                this.CheckElement(statement, expected);
            }

            // Checks the attribute lists of an element and the first token after them.
            private void CheckElement(SyntaxNode element, int expected)
            {
                foreach (var child in element.ChildNodesAndTokens())
                {
                    if (child.IsNode)
                    {
                        this.CheckToken(child.AsNode().GetFirstToken(), expected);
                        if (child.AsNode() is AttributeListSyntax)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        this.CheckToken(child.AsToken(), expected);
                    }

                    return;
                }
            }

            private int? GetReferenceIndentation(SyntaxToken anchor)
            {
                if (anchor.IsKind(SyntaxKind.None) || anchor.IsMissing)
                {
                    return null;
                }

                int line = this.text.Lines.GetLineFromPosition(anchor.SpanStart).LineNumber;
                if (this.expectedByLine.TryGetValue(line, out int expected))
                {
                    return expected;
                }

                return this.TryGetLeadingWidth(anchor, out int width, out _) ? width : null;
            }

            private bool TryGetLeadingWidth(SyntaxToken token, out int width, out TextLine line)
            {
                line = this.text.Lines.GetLineFromPosition(token.SpanStart);
                width = 0;
                for (int i = line.Start; i < token.SpanStart; i++)
                {
                    switch (this.text[i])
                    {
                    case ' ':
                        width++;
                        break;

                    case '\t':
                        int tabSize = Math.Max(1, this.settings.TabSize);
                        width += tabSize - (width % tabSize);
                        break;

                    default:
                        // The token is not the first thing on its line.
                        return false;
                    }
                }

                return true;
            }

            private void CheckToken(SyntaxToken token, int expected)
            {
                if (token.IsKind(SyntaxKind.None) || token.IsMissing)
                {
                    return;
                }

                if (!this.TryGetLeadingWidth(token, out int width, out TextLine line))
                {
                    return;
                }

                this.expectedByLine[line.LineNumber] = expected;
                if (width == expected)
                {
                    return;
                }

                Location location = line.Start == token.SpanStart
                    ? token.GetLocation()
                    : Location.Create(this.context.Tree, TextSpan.FromBounds(line.Start, token.SpanStart));
                ImmutableDictionary<string, string> properties = ImmutableDictionary<string, string>.Empty
                    .SetItem(ExpectedIndentationKey, expected.ToString(System.Globalization.CultureInfo.InvariantCulture));
                this.context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, properties));
            }
        }
    }
}
