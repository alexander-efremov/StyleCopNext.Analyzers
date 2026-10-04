// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp13.LayoutRules
{
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp12.LayoutRules;

    public partial class SA1504CSharp13UnitTests : SA1504CSharp12UnitTests
    {
        protected override DiagnosticResult[] GetExpectedResultAccessorWithoutBody()
        {
            return new DiagnosticResult[]
            {
#if ROSLYN_5_0_OR_GREATER
                // The compiler shipped with Roslyn 5.0 treats the field keyword as a C# 14 feature instead of a preview feature.
                DiagnosticResult.CompilerError("CS9260").WithMessage("Feature 'field keyword' is not available in C# 13.0. Please use language version 14.0 or greater.").WithLocation(4, 16),
#else
                DiagnosticResult.CompilerError("CS8652").WithMessage("The feature 'field keyword' is currently in Preview and *unsupported*. To use Preview features, use the 'preview' language version.").WithLocation(4, 16),
#endif
            };
        }
    }
}
