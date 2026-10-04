// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.CSharp10.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.CSharp9.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1138IndentElementsCorrectly,
        StyleCop.Analyzers.ReadabilityRules.SA1138CodeFixProvider>;

    public partial class SA1138CSharp10UnitTests : SA1138CSharp9UnitTests
    {
        [Fact]
        public async Task TestFileScopedNamespaceAsync()
        {
            var testCode = @"
using System;

namespace N;

class TestClass
{
    void M()
    {
[|  |]Console.WriteLine();
    }
}

[|  |]class TestClass2
{
}
";
            var fixedCode = @"
using System;

namespace N;

class TestClass
{
    void M()
    {
        Console.WriteLine();
    }
}

class TestClass2
{
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
