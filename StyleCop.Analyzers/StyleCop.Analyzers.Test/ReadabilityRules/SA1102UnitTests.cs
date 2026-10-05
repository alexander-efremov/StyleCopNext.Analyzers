// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using StyleCop.Analyzers.ReadabilityRules;
    using StyleCop.Analyzers.Test.Helpers;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA110xQueryClauses,
        StyleCop.Analyzers.ReadabilityRules.SA1102CodeFixProvider>;

    public class SA1102UnitTests
    {
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestSelectOnSeparateLineWithAdditionalEmptyLineAsync(string lineEnding)
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query = 
            from m in source
            where m > 0

            {|#0:select|} m;
    }
}".ReplaceLineEndings(lineEnding);

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query = 
            from m in source
            where m > 0
            select m;
    }
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestComplexQueryWithAdditionalEmptyLineAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var source2 = new int[0];

        var query = 
            from m in source

            let z  = source.Take(10)

            join f in source2  
            on m equals f

            where m > 0 && 
            m < 1

            group m by m into g

            select new {g.Key, Sum = g.Sum()};
    }
}";

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var source2 = new int[0];

        var query = 
            from m in source
            let z  = source.Take(10)
            join f in source2  
            on m equals f
            where m > 0 && 
            m < 1
            group m by m into g
            select new {g.Key, Sum = g.Sum()};
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(13, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(15, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(18, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(21, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(23, 13),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestComplexQueryInOneLineAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];
        var source2 = new int[0];

        var query = from m in source let z  = source.Take(10) join f in source2 on m equals f where m > 0 && m < 1 group m by m into g select new {g.Key, Sum = g.Sum()};
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestQueryInsideQueryAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var query = from m in
            (from s in Enumerable.Empty<int>()

            where s > 0

            select s)

            where m > 0

            orderby m descending 
            select m;
    }
}";

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var query = from m in
            (from s in Enumerable.Empty<int>()
            where s > 0
            select s)
            where m > 0
            orderby m descending 
            select m;
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(10, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(12, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(14, 13),
                    Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(16, 13),
                };

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("// A single-line comment.")]
        [InlineData("/* A multi-line comment. */")]
        [InlineData("/* A multi-line comment\n            with an empty line inside\n\n            still the comment. */")]
        [InlineData("/// A documentation-style comment.")]
        [InlineData("// First comment.\n            // Second comment.")]
        public async Task TestCommentLinesBetweenClausesAreNotReportedAsync(string comment)
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query =
            from m in source
            let z = m + 1
            COMMENT
            select m + z;
    }
}".Replace("COMMENT", comment).ReplaceLineEndings("\n");

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCommentLineInsideNestedCallIsNotReportedAsync()
        {
            var testCode = @"
using System;
using System.Collections.Generic;
using System.Linq;
public class Foo4
{
    public int Bar(int[] summaries, Func<int, int> weightSelector)
    {
        return Sum(
            from summary in summaries
            let weight = weightSelector(summary)
            // Multiplies each value independently while preserving the structure.
            select summary * weight);
    }

    private static int Sum(IEnumerable<int> values) => 0;
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task TestEmptyLineBeforeCommentIsReportedAndCommentKeptAsync(string lineEnding)
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query =
            from m in source
            where m > 0

            // The comment stays.
            {|#0:select|} m;
    }
}".ReplaceLineEndings(lineEnding);

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query =
            from m in source
            where m > 0
            // The comment stays.
            select m;
    }
}".ReplaceLineEndings(lineEnding);

            DiagnosticResult expected = Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyLineAfterCommentIsReportedAndCommentKeptAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query =
            from m in source
            where m > 0
            /* The comment stays. */

            {|#0:select|} m;
    }
}";

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query =
            from m in source
            where m > 0
            /* The comment stays. */
            select m;
    }
}";

            DiagnosticResult expected = Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestEmptyLinesAroundMultiLineCommentAreReportedAndCommentKeptAsync()
        {
            var testCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query =
            from m in source
            where m > 0

            /* A comment
               over two lines. */

            {|#0:select|} m;
    }
}";

            var fixedTestCode = @"
using System.Linq;
public class Foo4
{
    public void Bar()
    {
        var source = new int[0];

        var query =
            from m in source
            where m > 0
            /* A comment
               over two lines. */
            select m;
    }
}";

            DiagnosticResult expected = Diagnostic(SA110xQueryClauses.SA1102Descriptor).WithLocation(0);

            await VerifyCSharpFixAsync(testCode, expected, fixedTestCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
