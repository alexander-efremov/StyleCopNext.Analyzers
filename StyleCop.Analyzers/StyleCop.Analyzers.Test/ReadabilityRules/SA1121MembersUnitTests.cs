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
    /// Unit tests for <see cref="SA1121UseBuiltInTypeAlias"/> in members and comments. They are in a class of their own
    /// because xunit runs the tests of one class one after another.
    /// </summary>
    public class SA1121MembersUnitTests
    {
        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestIndexerAsync(string predefined, string fullName)
        {
            string testSource = @"namespace System {{
public class Foo
{{
    public {0} this
            [{0} test]
    {{
        get {{ return default({0}); }}
    }}
}}
}}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(4, 12),
                    Diagnostic().WithLocation(5, 14),
                    Diagnostic().WithLocation(7, 30),
                };

            await VerifyCSharpFixAsync(string.Format(testSource, fullName), expected, string.Format(testSource, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestGenericAndLambdaAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
public class Foo
{{
    public void Bar()
    {{
        Func<{0}, 
                {0}> f = 
                    ({0} param) => param;
    }}
}}";
            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(6, 14),
                    Diagnostic().WithLocation(7, 17),
                    Diagnostic().WithLocation(8, 22),
                };

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestDocumentationCommentDirectReferenceAsync(string predefined, string fullName)
        {
            string testCode = @"#pragma warning disable CS0419 // Ambiguous reference in cref attribute
namespace System {{
/// <seealso cref=""{0}""/>
public class Foo
{{
}}
}}";

            DiagnosticResult expected = Diagnostic().WithLocation(3, 20);

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined), CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(SA1121UnitTests.AllTypes), MemberType = typeof(SA1121UnitTests))]
        public async Task TestDocumentationCommentIndirectReferenceAsync(string predefined, string fullName)
        {
            string testCode = @"using System;
/// <seealso cref=""Convert.ToBoolean({0})""/>
public class Foo
{{
}}";
            DiagnosticResult expected = Diagnostic().WithLocation(2, 38);

            await VerifyCSharpFixAsync(string.Format(testCode, fullName), expected, string.Format(testCode, predefined), CancellationToken.None).ConfigureAwait(false);
        }
    }
}
