// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

// Several test methods in this file use the same member data, but in some cases the test does not use all of the
// supported parameters. See https://github.com/xunit/xunit/issues/1556.
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1121UseBuiltInTypeAlias,
        StyleCop.Analyzers.ReadabilityRules.SA1121CodeFixProvider>;

    /// <summary>
    /// Unit tests for <see cref="SA1121UseBuiltInTypeAlias"/> in expressions. They are in a class of their own
    /// because xunit runs the tests of one class one after another.
    /// </summary>
    public class SA1121ExpressionsUnitTests
    {
        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestTypeOfAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var test = typeof({0});
    }}
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 27);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestImplicitCastAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var t = ({0})
                    default({0});
    }}
}}
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 18),
                    Diagnostic().WithLocation(7, 29),
                };

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestArrayAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var array = new {0}[0];
    }}
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 25);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestNameOfAsync(string predefined, string fullName)
        {
            // Not needed for this test
            _ = predefined;

            string testCode = @"
namespace System
{{
    public class Foo
    {{
        public void Bar()
        {{
            string test = nameof({0});
        }}
    }}
}}
";

            await VerifyCSharpDiagnosticAsync(string.Format(testCode, fullName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestNameOfInnerMethodAsync(string predefined, string fullName)
        {
            string testCode = @"
namespace System
{{
    public class Foo
    {{
        public void Bar()
        {{
            string test = nameof({0}.ToString);
        }}
    }}
}}
";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 34);
            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined), CancellationToken.None).ConfigureAwait(false);
        }
    }
}
