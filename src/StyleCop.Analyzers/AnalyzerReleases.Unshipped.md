; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/master/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SA1141 | StyleCop.CSharp.ReadabilityRules | Warning | SA1141UseTupleSyntax, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1141.md)
SA1142 | StyleCop.CSharp.ReadabilityRules | Warning | SA1142ReferToTupleElementsByName, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1142.md)
SA1143 | StyleCop.CSharp.ReadabilityRules | Warning | SA1143ConditionalExpressionsMustBeFormattedConsistently, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1143.md)
SA1315 | StyleCop.CSharp.NamingRules | Warning | SA1315ParametersShouldMatchInheritedNames, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1315.md)
SA1316 | StyleCop.CSharp.NamingRules | Warning | SA1316TupleElementNamesShouldUseCorrectCasing, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1316.md)
SA1317 | StyleCop.CSharp.NamingRules | Warning | SA1317IdentifierShouldBeNamedOnlyWithLatinLetters, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1317.md)
SA1414 | StyleCop.CSharp.ReadabilityRules | Warning | SA1414TupleTypesInSignaturesShouldHaveElementNames, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1414.md)
SX1412 | StyleCop.CSharp.MaintainabilityRules | Warning | SX1412StoreFilesAsUtf8WithoutByteOrderMark, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SX1412.md)
SA1518 | StyleCop.CSharp.LayoutRules | Warning | SA1518UseLineEndingsCorrectlyAtEndOfFile, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1518.md)
SA1653 | StyleCop.CSharp.DocumentationRules | Warning | SA1653OnlyOnePartialDeclarationShouldContainSummaryDocumentation, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1653.md)
SA1654 | StyleCop.CSharp.DocumentationRules | Warning | SA1654ProperlyEncodeGenericBrackets, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1654.md)

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|-------
SA1413 | StyleCop.CSharp.MaintainabilityRules | Warning | StyleCop.CSharp.ReadabilityRules | Warning | SA1413UseTrailingCommasInMultiLineInitializers, [Documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/SA1413.md)
