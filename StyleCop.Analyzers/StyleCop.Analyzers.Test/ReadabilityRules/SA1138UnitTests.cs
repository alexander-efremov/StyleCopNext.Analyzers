// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1138IndentElementsCorrectly,
        StyleCop.Analyzers.ReadabilityRules.SA1138CodeFixProvider>;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1138IndentElementsCorrectly"/>.
    /// </summary>
    public class SA1138UnitTests
    {
        [Fact]
        public async Task TestCorrectlyIndentedCodeAsync()
        {
            var testCode = @"using System;

namespace N
{
    [Serializable]
    public class C
    {
        private int field;

        [Obsolete]
        public int Property
        {
            get { return this.field; }
            set
            {
                this.field = value;
            }
        }

        public enum E
        {
            A,
            B,
        }

        public int Method(int value)
        {
            if (value > 0) {
                value++;
            }
            else
            {
                value--;
            }

            try
            {
                value++;
            }
            catch (Exception)
            {
                value--;
            }
            finally
            {
                value = 0;
            }

            switch (value)
            {
                case 1:
                case 2:
                    value++;
                    break;

                default:
                    {
                        value--;
                    }

                    break;
            }

            goto done;
        done:
            return value;
        }

        private class Nested
        {
            private void M()
            {
                {
                    int x = 0;
                    x++;
                }
            }
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockStatementsAsync()
        {
            var testCode = @"class TestClass
{
    static void A() { }

    static void M(bool b)
    {
[|A|]();
[|  |]A();
        A();
[|            |]A();
        if (b)
        {
[|        |]A();
        }
    }
}
";
            var fixedCode = @"class TestClass
{
    static void A() { }

    static void M(bool b)
    {
        A();
        A();
        A();
        A();
        if (b)
        {
            A();
        }
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestChildrenAreExpectedRelativeToTheExpectedIndentationOfTheirParentAsync()
        {
            var testCode = @"class TestClass
{
    static void A() { }

[|  |]static void M(bool b)
[|  |]{
[|      |]if (b)
[|      |]{
[|          |]A();
[|      |]}
[|  |]}
}
";
            var fixedCode = @"class TestClass
{
    static void A() { }

    static void M(bool b)
    {
        if (b)
        {
            A();
        }
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestTypesEnumsAndAccessorsAsync()
        {
            var testCode = @"namespace N
{
[|  |]class Outer
    {
        int a;
[|      |]int b;

        enum E
        {
[|    |]A,
            B,
        }

        int P
        {
[|      |]get { return this.a; }
            set { this.a = value; }
        }

        struct S
        {
[|    |]int c;
        }

        interface I
        {
[|          |]void M();
        }
    }
}
";
            var fixedCode = @"namespace N
{
    class Outer
    {
        int a;
        int b;

        enum E
        {
            A,
            B,
        }

        int P
        {
            get { return this.a; }
            set { this.a = value; }
        }

        struct S
        {
            int c;
        }

        interface I
        {
            void M();
        }
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCompilationUnitMembersAsync()
        {
            var testCode = @"using System;
[|  |]using System.Text;

[|  |][assembly: CLSCompliant(true)]

[|  |]class TestClass
{
}

[|    |]class TestClass2
{
}
";
            var fixedCode = @"using System;
using System.Text;

[assembly: CLSCompliant(true)]

class TestClass
{
}

class TestClass2
{
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNamespaceMembersAsync()
        {
            var testCode = @"namespace Outer
{
[|  |]using System;
[|        |]class TestClass
    {
    }

    namespace Inner
    {
[|    |]class TestClass2
        {
        }
    }
}
";
            var fixedCode = @"namespace Outer
{
    using System;
    class TestClass
    {
    }

    namespace Inner
    {
        class TestClass2
        {
        }
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesAsync()
        {
            var testCode = @"using System;

class TestClass
{
[|  |][Obsolete]
[|      |]void M1() { }

    [Obsolete]
    [System.Diagnostics.Conditional(""X"")]
[|  |]void M2() { }
}
";
            var fixedCode = @"using System;

class TestClass
{
    [Obsolete]
    void M1() { }

    [Obsolete]
    [System.Diagnostics.Conditional(""X"")]
    void M2() { }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestKAndRStyleBlocksAsync()
        {
            var testCode = @"class TestClass {
    static void A() { }

    static void M(int v) {
        if (v == 1) {
            A();
        } else if (v == 2) {
[|    |]A();
        } else {
[|              |]A();
        }

        foreach (var x in new[] { 1, 2 }) {
[|      |]A();
        }
    }
}
";
            var fixedCode = @"class TestClass {
    static void A() { }

    static void M(int v) {
        if (v == 1) {
            A();
        } else if (v == 2) {
            A();
        } else {
            A();
        }

        foreach (var x in new[] { 1, 2 }) {
            A();
        }
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmbeddedStatementOwnsBlockAsync()
        {
            var testCode = @"class TestClass
{
    static void A() { }

    static void M(bool b)
    {
        if (b)
            foreach (var x in new[] { 1, 2 })
            {
[|        |]A();
            }
    }
}
";
            var fixedCode = @"class TestClass
{
    static void A() { }

    static void M(bool b)
    {
        if (b)
            foreach (var x in new[] { 1, 2 })
            {
                A();
            }
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(true, true, 12, 16)]
        [InlineData(true, false, 12, 12)]
        [InlineData(false, true, 8, 12)]
        [InlineData(false, false, 8, 8)]
        public async Task TestSwitchSectionsAsync(bool indentSwitchSection, bool indentSwitchCaseSection, int labelColumn, int bodyColumn)
        {
            const string template = @"class TestClass
{
    static void A() { }

    static void M(int v)
    {
        switch (v)
        {
@L@case 1:
@L@case 2:
@B@A();
@B@break;

@L@default:
@B@A();
@B@break;
        }
    }
}
";
            string settings = $@"{{
  ""settings"": {{
    ""indentation"": {{
      ""indentSwitchSection"": {indentSwitchSection.ToString().ToLowerInvariant()},
      ""indentSwitchCaseSection"": {indentSwitchCaseSection.ToString().ToLowerInvariant()}
    }}
  }}
}}";

            string correct = template
                .Replace("@L@", new string(' ', labelColumn))
                .Replace("@B@", new string(' ', bodyColumn));
            string incorrect = template
                .Replace("@L@", "[|   |]")
                .Replace("@B@", "[|   |]");

            await VerifyAsync(correct, correct, settings).ConfigureAwait(false);
            await VerifyAsync(incorrect, correct, settings).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestSwitchBlocksAndLabelsAreNotReportedWhenAmbiguousAsync()
        {
            var testCode = @"class TestClass
{
    static void A() { }

    static void M(int v)
    {
        switch (v)
        {
            case 1: {
                    A();
                break;
            }

            case 2:
                {
                    A();
                }

                break;

            case 3: A();
                break;

            case 4:
        {
            A();
        }

                break;
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("leftMost", 0)]
        [InlineData("oneLess", 4)]
        [InlineData("noIndent", 8)]
        public async Task TestLabelPositioningAsync(string labelPositioning, int labelColumn)
        {
            const string template = @"class TestClass
{
    static int M(int v)
    {
        v++;
        goto done;
@L@done:
        return v;
    }
}
";
            string settings = $@"{{
  ""settings"": {{
    ""indentation"": {{
      ""labelPositioning"": ""{labelPositioning}""
    }}
  }}
}}";

            string correct = template.Replace("@L@", new string(' ', labelColumn));
            string incorrect = template.Replace("@L@", "[|  |]");

            await VerifyAsync(correct, correct, settings).ConfigureAwait(false);
            await VerifyAsync(incorrect, correct, settings).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLabelStatementOnNextLineAsync()
        {
            var testCode = @"class TestClass
{
    static int M(int v)
    {
        goto done;
    done:
[|    |]return v;
    }
}
";
            var fixedCode = @"class TestClass
{
    static int M(int v)
    {
        goto done;
    done:
        return v;
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndentBlockDisabledAsync()
        {
            var settings = @"{
  ""settings"": {
    ""indentation"": {
      ""indentBlock"": false
    }
  }
}";
            var testCode = @"class TestClass
{
    static void A() { }

    static void M(bool b)
    {
    A();
    if (b)
    {
    A();
[|        |]A();
    }
    }
}
";
            var fixedCode = @"class TestClass
{
    static void A() { }

    static void M(bool b)
    {
    A();
    if (b)
    {
    A();
    A();
    }
    }
}
";

            await VerifyAsync(testCode, fixedCode, settings).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUseTabsAsync()
        {
            var testCode = "class TestClass\r\n{\r\n\tstatic void A() { }\r\n\r\n\tstatic void M()\r\n\t{\r\n\t\tA();\r\n[|\t|]A();\r\n[|    |]A();\r\n        A();\r\n[|\t\t\t|]A();\r\n\t}\r\n}\r\n";
            var fixedCode = "class TestClass\r\n{\r\n\tstatic void A() { }\r\n\r\n\tstatic void M()\r\n\t{\r\n\t\tA();\r\n\t\tA();\r\n\t\tA();\r\n        A();\r\n\t\tA();\r\n\t}\r\n}\r\n";

            // A line which reaches the expected column with spaces is not reported by this rule.
            await VerifyAsync(testCode, fixedCode, settings: null, test => test.UseTabs = true).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUseTabsWithTabSizeDifferentFromIndentationSizeAsync()
        {
            var testCode = "class TestClass\r\n{\r\n    static void A() { }\r\n\r\n    static void M()\r\n    {\r\n\tA();\r\n[|      |]A();\r\n[|\t    |]A();\r\n    }\r\n}\r\n";
            var fixedCode = "class TestClass\r\n{\r\n    static void A() { }\r\n\r\n    static void M()\r\n    {\r\n\tA();\r\n\tA();\r\n\tA();\r\n    }\r\n}\r\n";

            await VerifyAsync(
                testCode,
                fixedCode,
                settings: null,
                test =>
                {
                    test.UseTabs = true;
                    test.TabSize = 8;
                    test.IndentationSize = 4;
                }).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndentationSizeAsync()
        {
            var testCode = @"class TestClass
{
  static void A() { }

  static void M()
  {
    A();
[|        |]A();
  }
}
";
            var fixedCode = @"class TestClass
{
  static void A() { }

  static void M()
  {
    A();
    A();
  }
}
";

            await VerifyAsync(testCode, fixedCode, settings: null, test => test.IndentationSize = 2).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLinesWhichAreNotElementsAreNotReportedAsync()
        {
            var testCode = @"using System;

class TestClass
{
    static int Add(int a, int b, int c) => a + b + c;

    static void A() { }

    static void M(int x)
    {
        var y = Add(1,
                2,
          3);
        var s = @""line1
  line2
        line3"";
        var t = $@""a {x +
       1} b"";
        var u = x
    .ToString();
        // comment
          // comment
              /* comment */
        /* comment */ A();
        /* comment
    spanning */ A();
        Action a = () =>
        {
              A();
        };
        Action b = delegate
        {
          A();
        };
        var arr = new[]
        {
          1,
              2,
        };
#if DISABLED
          weird();
#endif
        A();
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestBlockOwnedByLabelIsNotReportedAsync()
        {
            var testCode = @"class TestClass
{
    static void A() { }

    static void M()
    {
        goto done;
    done:
        {
      A();
        }
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEnabledDirectivesAreCheckedAsync()
        {
            var testCode = @"class TestClass
{
    static void A() { }

    static void M()
    {
#if !DISABLED
[|  |]A();
#else
          weird();
#endif
        A();
    }
}
";
            var fixedCode = @"class TestClass
{
    static void A() { }

    static void M()
    {
#if !DISABLED
        A();
#else
          weird();
#endif
        A();
    }
}
";

            await VerifyAsync(testCode, fixedCode).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestFixAllInDocumentAsync()
        {
            var testCode = @"namespace N
{
 class C
 {
  void A() { }

  void M(int v)
  {
   switch (v)
   {
   case 1:
    A();
    break;
   }

   if (v > 0)
   {
  A();
   }
  }
 }
}
";

            await new CSharpTest
            {
                TestCode = testCode,
                ExpectedDiagnostics =
                {
                    Diagnostic().WithLocation(3, 1),
                    Diagnostic().WithLocation(4, 1),
                    Diagnostic().WithLocation(8, 1),
                    Diagnostic().WithLocation(10, 1),
                    Diagnostic().WithLocation(14, 1),
                    Diagnostic().WithLocation(17, 1),
                    Diagnostic().WithLocation(19, 1),
                    Diagnostic().WithLocation(20, 1),
                    Diagnostic().WithLocation(21, 1),
                    Diagnostic().WithLocation(5, 1),
                    Diagnostic().WithLocation(7, 1),
                    Diagnostic().WithLocation(9, 1),
                    Diagnostic().WithLocation(11, 1),
                    Diagnostic().WithLocation(12, 1),
                    Diagnostic().WithLocation(13, 1),
                    Diagnostic().WithLocation(16, 1),
                    Diagnostic().WithLocation(18, 1),
                },
                FixedCode = @"namespace N
{
    class C
    {
        void A() { }

        void M(int v)
        {
            switch (v)
            {
                case 1:
                    A();
                    break;
            }

            if (v > 0)
            {
                A();
            }
        }
    }
}
",
            }.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEditorConfigOptionsAsync()
        {
            var editorConfig = @"root = true

[*.cs]
csharp_indent_block_contents = false
csharp_indent_switch_labels = false
csharp_indent_case_contents = false
csharp_indent_labels = flush_left
";
            var testCode = @"class TestClass
{
    static int M(int v)
    {
    switch (v)
    {
    case 1:
[|        |]v++;
    break;
    }

    goto done;
[|    |]done:
    return v;
    }
}
";
            var fixedCode = @"class TestClass
{
    static int M(int v)
    {
    switch (v)
    {
    case 1:
    v++;
    break;
    }

    goto done;
done:
    return v;
    }
}
";

            await VerifyAsync(testCode, fixedCode, settings: null, test => test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig))).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestStyleCopJsonTakesPrecedenceOverEditorConfigAsync()
        {
            var settings = @"{
  ""settings"": {
    ""indentation"": {
      ""indentBlock"": true
    }
  }
}";
            var editorConfig = @"root = true

[*.cs]
csharp_indent_block_contents = false
";
            var testCode = @"class TestClass
{
    static void A() { }

    static void M()
    {
[|    |]A();
    }
}
";
            var fixedCode = @"class TestClass
{
    static void A() { }

    static void M()
    {
        A();
    }
}
";

            await VerifyAsync(testCode, fixedCode, settings, test => test.TestState.AnalyzerConfigFiles.Add(("/.editorconfig", editorConfig))).ConfigureAwait(false);
        }

        private static Task VerifyAsync(string testCode, string fixedCode, string settings = null, Action<CSharpTest> configure = null)
        {
            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                Settings = settings,
            };

            configure?.Invoke(test);
            return test.RunAsync(CancellationToken.None);
        }
    }
}
