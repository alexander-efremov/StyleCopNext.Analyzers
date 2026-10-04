// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.SpacingRules
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    /// <summary>
    /// A null-conditional operator (<c>?.</c> or <c>?[</c>) is split by whitespace, a line break, or a comment.
    /// </summary>
    /// <remarks>
    /// <para>A violation of this rule occurs when anything appears between the <c>?</c> and the following <c>.</c> or
    /// <c>[</c> of a null-conditional operator.</para>
    /// </remarks>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class SA1029DoNotSplitNullConditionalOperators : DiagnosticAnalyzer
    {
        /// <summary>
        /// The ID for diagnostics produced by the <see cref="SA1029DoNotSplitNullConditionalOperators"/> analyzer.
        /// </summary>
        public const string DiagnosticId = "SA1029";
        private const string HelpLink = "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1029.md";
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(SpacingResources.SA1029Title), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(SpacingResources.SA1029MessageFormat), SpacingResources.ResourceManager, typeof(SpacingResources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(SpacingResources.SA1029Description), SpacingResources.ResourceManager, typeof(SpacingResources));

        private static readonly DiagnosticDescriptor Descriptor =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, AnalyzerCategory.SpacingRules, DiagnosticSeverity.Warning, AnalyzerConstants.EnabledByDefault, Description, HelpLink);

        private static readonly Action<SyntaxNodeAnalysisContext> ConditionalAccessAction = HandleConditionalAccess;

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(ConditionalAccessAction, SyntaxKind.ConditionalAccessExpression);
        }

        private static void HandleConditionalAccess(SyntaxNodeAnalysisContext context)
        {
            var conditionalAccess = (ConditionalAccessExpressionSyntax)context.Node;
            var operatorToken = conditionalAccess.OperatorToken;
            if (operatorToken.IsMissing)
            {
                return;
            }

            // The right side of a conditional access is a chain such as .A.B[0]; its first token is the . or [ of the operator.
            var nextToken = conditionalAccess.WhenNotNull.GetFirstToken();
            if (nextToken.IsMissing
                || (!nextToken.IsKind(SyntaxKind.DotToken) && !nextToken.IsKind(SyntaxKind.OpenBracketToken)))
            {
                return;
            }

            if (operatorToken.HasTrailingTrivia || nextToken.HasLeadingTrivia)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, operatorToken.GetLocation()));
            }
        }
    }
}
