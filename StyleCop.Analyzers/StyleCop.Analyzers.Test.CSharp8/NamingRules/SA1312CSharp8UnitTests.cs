// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp8.NamingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp7.NamingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.NamingRules.SA1312VariableNamesMustBeginWithLowerCaseLetter,
        StyleCop.Analyzers.NamingRules.RenameToLowerCaseCodeFixProvider>;

    public partial class SA1312CSharp8UnitTests : SA1312CSharp7UnitTests
    {
        [Fact]
        public async Task TestUnderscoreOnlyUsingDeclarationResourceIsNotReportedAsync()
        {
            var testCode = @"using System;
using System.IO;
using System.Threading.Tasks;

public class TypeName
{
    public async Task MethodNameAsync()
    {
        using var _ = new MemoryStream();
        await using var __ = new AsyncDisposable();
        using var _Stream = new MemoryStream();
        var ___ = new MemoryStream();
    }

    private sealed class AsyncDisposable : IAsyncDisposable
    {
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithArguments("_Stream").WithLocation(11, 19),
                Diagnostic().WithArguments("___").WithLocation(12, 13),
            };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
