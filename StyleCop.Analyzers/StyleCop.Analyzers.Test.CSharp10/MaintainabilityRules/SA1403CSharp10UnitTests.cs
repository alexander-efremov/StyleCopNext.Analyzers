// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp9.MaintainabilityRules;
    using Xunit;

    public partial class SA1403CSharp10UnitTests : SA1403CSharp9UnitTests
    {
        [Fact]
        public async Task TestFileScopedNamespaceHasNoDiagnosticAsync()
        {
            var testCode = @"namespace Foo;

class A
{
}
";

            await this.VerifyCSharpDiagnosticAsync(testCode, this.GetSettings(), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
