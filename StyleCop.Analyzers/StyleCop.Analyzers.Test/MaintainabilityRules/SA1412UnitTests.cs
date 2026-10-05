// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopDiagnosticVerifier<StyleCop.Analyzers.MaintainabilityRules.SA1412StoreFilesAsUtf8>;

    public class SA1412UnitTests
    {
#if NET
        static SA1412UnitTests()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
#endif

        public static IEnumerable<object[]> NonUtf8Encodings
        {
            get
            {
                yield return new object[] { Encoding.ASCII.CodePage };
                yield return new object[] { Encoding.BigEndianUnicode.CodePage };
#if NETFRAMEWORK
                yield return new object[] { Encoding.Default.CodePage };
#else
                yield return new object[] { 1252 };
#endif
                yield return new object[] { Encoding.Unicode.CodePage };
                yield return new object[] { Encoding.UTF32.CodePage };
#pragma warning disable SYSLIB0001 // Type or member is obsolete
                yield return new object[] { Encoding.UTF7.CodePage };
#pragma warning restore SYSLIB0001 // Type or member is obsolete
            }
        }

        [Theory]
        [MemberData(nameof(NonUtf8Encodings))]
        public async Task TestFileWithWrongEncodingAsync(int codepage)
        {
            var testCode = SourceText.From("class TypeName { }", GetEncoding(codepage));

            var expected = Diagnostic().WithLocation(1, 1);

            var test = new CSharpTest
            {
                TestSources = { testCode },
                ExpectedDiagnostics = { expected },
            };

            test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestFileWithUtf8EncodingWithoutBOMAsync(string lineEnding)
        {
            var source = "class TypeName\n{\n}\n".ReplaceLineEndings(lineEnding);
            var testCode = SourceText.From(source, new UTF8Encoding(false));

            var expected = Diagnostic().WithLocation(1, 1);

            var test = new CSharpTest
            {
                TestSources = { testCode },
                ExpectedDiagnostics = { expected },
            };

            test.TestBehaviors |= TestBehaviors.SkipSuppressionCheck;
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        private static Encoding GetEncoding(int codepage)
        {
#pragma warning disable SYSLIB0001 // Type or member is obsolete
            if (codepage == Encoding.UTF7.CodePage)
            {
                return Encoding.UTF7;
            }
#pragma warning restore SYSLIB0001 // Type or member is obsolete

            return Encoding.GetEncoding(codepage);
        }
    }
}
