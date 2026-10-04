// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.SpacingRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.SpacingRules.SA1029DoNotSplitNullConditionalOperators,
        StyleCop.Analyzers.SpacingRules.SA1029CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1029DoNotSplitNullConditionalOperators"/> and
    /// <see cref="SA1029CodeFixProvider"/>.
    /// </summary>
    public class SA1029UnitTests
    {
        [Fact]
        public async Task UnsplitOperatorsAndOtherQuestionTokensAreNotReportedAsync()
        {
            var testCode = @"
class TestClass
{
    int? field;
    string[] values;

    int TestMethod(string foo, string[] array, int? nullable, bool condition)
    {
        foo?.Trim();
        array?[0].ToString();
        foo?.Trim()?.ToString();
        array?[0]?.Trim();
        this.values?[0]?.Trim();
        return condition ? (nullable ?? 1) : (field ?? 2);
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task SpacesBeforeMemberBindingAreReportedAndFixedAsync()
        {
            var testCode = @"
class TestClass
{
    void TestMethod(string foo)
    {
        foo{|#0:?|}         .Trim();
    }
}
";
            var fixedCode = @"
class TestClass
{
    void TestMethod(string foo)
    {
        foo?.Trim();
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task SpacesBeforeElementBindingAreReportedAndFixedAsync()
        {
            var testCode = @"
class TestClass
{
    void TestMethod(string[] foo)
    {
        foo{|#0:?|}             [0].ToString();
    }
}
";
            var fixedCode = @"
class TestClass
{
    void TestMethod(string[] foo)
    {
        foo?[0].ToString();
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task LineBreakBeforeMemberBindingIsReportedAndFixedAsync(string lineEnding)
        {
            var testCode = @"
class TestClass
{
    void TestMethod(string foo)
    {
        foo{|#0:?|}
            .Trim();
    }
}
".Replace("\r\n", "\n").Replace("\n", lineEnding);
            var fixedCode = @"
class TestClass
{
    void TestMethod(string foo)
    {
        foo?.Trim();
    }
}
".Replace("\r\n", "\n").Replace("\n", lineEnding);

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task LineBreakBeforeElementBindingIsReportedAndFixedAsync()
        {
            var testCode = @"
class TestClass
{
    void TestMethod(string[] foo)
    {
        foo{|#0:?|}
            [0].ToString();
    }
}
";
            var fixedCode = @"
class TestClass
{
    void TestMethod(string[] foo)
    {
        foo?[0].ToString();
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CommentAfterOperatorIsReportedButNotFixedAsync()
        {
            var testCode = @"
class TestClass
{
    void TestMethod(string foo)
    {
        foo{|#0:?|} // comment
            .Trim();
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task CommentBeforeBindingIsReportedButNotFixedAsync()
        {
            var testCode = @"
class TestClass
{
    void TestMethod(string foo)
    {
        foo{|#0:?|}
            // comment
            .Trim();
    }
}
";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task ChainedAndNestedOperatorsAreFixedTogetherAsync()
        {
            var testCode = @"
class TestClass
{
    void TestMethod(string[] foo, string bar)
    {
        foo{|#0:?|}
            [0]{|#1:?|}  .Trim(){|#2:?|}
            .ToString();
        bar{|#3:?|} .Replace(foo{|#4:?|} [0], bar{|#5:?|}
            .Trim());
    }
}
";
            var fixedCode = @"
class TestClass
{
    void TestMethod(string[] foo, string bar)
    {
        foo?[0]?.Trim()?.ToString();
        bar?.Replace(foo?[0], bar?.Trim());
    }
}
";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
                Diagnostic().WithLocation(4),
                Diagnostic().WithLocation(5),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
