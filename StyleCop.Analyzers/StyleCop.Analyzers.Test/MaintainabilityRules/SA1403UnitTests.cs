// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;

    public class SA1403UnitTests : FileMayOnlyContainTestBase
    {
        public override string Keyword => "namespace";

        public override bool SupportsCodeFix => true;

        protected override DiagnosticAnalyzer Analyzer => new SA1403FileMayOnlyContainASingleNamespace();

        protected override CodeFixProvider CodeFix => new SA1403CodeFixProvider();

        [Fact]
        public async Task TestNestedNamespacesAsync()
        {
            var testCode = @"namespace Foo
{
    namespace Bar
    {
        class Baz
        {
        }
    }
}";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"namespace Foo
{
}"),
                ("Foo.Bar.cs", @"namespace Foo.Bar
{
    class Baz
    {
    }
}"),
            };

            DiagnosticResult expected = this.Diagnostic().WithLocation(3, 15);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNestedNamespaceCarriesEnclosingUsingsAsync()
        {
            var testCode = @"namespace Foo
{
    using System;

    namespace Bar
    {
        using System.Text;

        class Baz
        {
        }
    }
}";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"namespace Foo
{
    using System;
}"),
                ("Foo.Bar.cs", @"namespace Foo.Bar
{
    using System;
    using System.Text;

    class Baz
    {
    }
}"),
            };

            DiagnosticResult expected = this.Diagnostic().WithLocation(5, 15);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestNamespacesWithUsingsAndHeaderAsync()
        {
            var testCode = @"// Copyright header
using System;

namespace Foo
{
    class A
    {
    }
}

namespace Bar
{
    class B
    {
    }
}";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"// Copyright header
using System;

namespace Foo
{
    class A
    {
    }
}
"),
                ("Bar.cs", @"// Copyright header
using System;

namespace Bar
{
    class B
    {
    }
}"),
            };

            DiagnosticResult expected = this.Diagnostic().WithLocation(11, 11);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestQualifiedNamespaceNameIsUsedForFileNameAsync()
        {
            var testCode = @"namespace Foo
{
}
namespace Bar.Baz
{
}";

            var fixedCode = new[]
            {
                ("/0/Test0.cs", @"namespace Foo
{
}
"),
                ("Bar.Baz.cs", @"namespace Bar.Baz
{
}"),
            };

            DiagnosticResult expected = this.Diagnostic().WithLocation(4, 11);
            await this.VerifyCSharpFixAsync(testCode, this.GetSettings(), expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
