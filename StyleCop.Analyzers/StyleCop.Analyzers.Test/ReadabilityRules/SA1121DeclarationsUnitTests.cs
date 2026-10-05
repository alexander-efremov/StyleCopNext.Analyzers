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
    /// Unit tests for <see cref="SA1121UseBuiltInTypeAlias"/> in declarations. They are in a class of their own
    /// because xunit runs the tests of one class one after another.
    /// </summary>
    public class SA1121DeclarationsUnitTests
    {
        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestVariableDeclarationAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        {0} test;
    }}
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 9);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestEscapedVariableDeclarationAsync(string predefined, string fullName)
        {
            if (fullName.IndexOf('.') >= 0)
            {
                return;
            }

            string testSource = @"namespace NotSystem {{
public class ClassName
{{
    public void Bar()
    {{
        @{0} test;
    }}

    public struct @{0} {{ }}
}}
}}";

            await VerifyCSharpDiagnosticAsync(string.Format(testSource, predefined), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
            await VerifyCSharpDiagnosticAsync(string.Format(testSource, fullName), DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestDefaultDeclarationAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar()
    {{
        var test = default({0});
    }}
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 28);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestReturnTypeAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public {0} Bar()
    {{
        return default({0});
    }}
}}
}}";
            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(4, 12),
                Diagnostic().WithLocation(6, 24),
            };

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestArgumentAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public void Bar({0} test)
    {{
    }}
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(4, 21);

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }
    }
}
