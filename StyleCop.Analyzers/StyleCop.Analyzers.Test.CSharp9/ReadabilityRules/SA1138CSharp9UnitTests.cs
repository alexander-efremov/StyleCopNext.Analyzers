// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp9.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.Test.CSharp8.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1138IndentElementsCorrectly,
        StyleCop.Analyzers.ReadabilityRules.SA1138CodeFixProvider>;

    public partial class SA1138CSharp9UnitTests : SA1138CSharp8UnitTests
    {
        [Fact]
        public async Task TestRecordMembersAsync()
        {
            var testCode = @"
public record TestRecord
{
    public int A { get; init; }
[|  |]public int B { get; init; }
}
";
            var fixedCode = @"
public record TestRecord
{
    public int A { get; init; }
    public int B { get; init; }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTopLevelStatementsAsync()
        {
            var testCode = @"using System;

Console.WriteLine();
[|  |]Console.WriteLine();
if (Environment.TickCount > 0)
{
[|  |]Console.WriteLine();
}
";
            var fixedCode = @"using System;

Console.WriteLine();
Console.WriteLine();
if (Environment.TickCount > 0)
{
    Console.WriteLine();
}
";

            await new CSharpTest
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestState =
                {
                    OutputKind = OutputKind.ConsoleApplication,
                    Sources = { testCode },
                },
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
