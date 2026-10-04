// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp7.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1138IndentElementsCorrectly,
        StyleCop.Analyzers.ReadabilityRules.SA1138CodeFixProvider>;

    public partial class SA1138CSharp7UnitTests : SA1138UnitTests
    {
        [Fact]
        public async Task TestLocalFunctionAsync()
        {
            var testCode = @"
class TestClass
{
    static int Method(int value)
    {
[|  |]int Local(int x)
        {
[|      |]return x + 1;
        }

        return Local(value);
    }
}
";
            var fixedCode = @"
class TestClass
{
    static int Method(int value)
    {
        int Local(int x)
        {
            return x + 1;
        }

        return Local(value);
    }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }
    }
}
