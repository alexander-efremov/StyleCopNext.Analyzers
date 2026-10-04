// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.ReadabilityRules
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis.Testing;
    using Xunit;
    using static StyleCop.Analyzers.Test.Verifiers.StyleCopCodeFixVerifier<
        StyleCop.Analyzers.ReadabilityRules.SA1114ParameterListMustFollowDeclaration,
        StyleCop.Analyzers.ReadabilityRules.SA1114CodeFixProvider>;

    public class SA1114UnitTests
    {
        [Fact]
        public async Task TestMethodDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

string s)
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(

1);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(
1);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var e = 1.Equals(1);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMethodCallNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        var i = 1.ToString(
                
            );
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(

string s)
    {

    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(
string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(string s)
    {

    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public Foo () 
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(

1,2);
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(12, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorallParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(
1,2);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public Foo(int i, int j)
    {
    }

    public void Bar()
    {
        var f = new Foo(1,2);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestConstructorCallNoParametersAsync()
        {
            var testCode = @"
public class Foo
{
    public void Bar()
    {
       var f = new Foo(

);
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[

int i]
    {
        get
        {
            return 1;
        }
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[
int i]
    {
        get
        {
            return 1;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    int this[int i]
    {
        get
        {
            return 1;
        }
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizes2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[

4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestMultidimensionalArrayDeclarationSizes2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
var a = new int[

1][

,]
            {
                new int[

1, 1]
                {
                    {1}
                }
            };
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(8, 1),
                    Diagnostic().WithLocation(14, 1),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizesOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[
4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayDeclarationSizesOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[,] array2Da = new int[4, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParameters2LinesAfterOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[

1];
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(9, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParametersOnNextLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[
1];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestIndexerCallParametersOnSameLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
        var i = list[1];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParameters2LinesAfterOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[

0][

1, 0];
    }
}";

            DiagnosticResult[] expected =
                {
                    Diagnostic().WithLocation(14, 1),
                    Diagnostic().WithLocation(16, 1),
                };

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParametersOnNextLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[
0][
1, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestArrayCallParametersOnSameLineAsOpeningBracketAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        int[][,] jaggedArray4 = new int[3][,] 
        {
            new int[,] { {1,3}, {5,7} },
            new int[,] { {0,2}, {4,6}, {8,10} },
            new int[,] { {11,22}, {99,88}, {0,9} } 
        };
        var i = jaggedArray4[0][1, 0];
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(

""DEBUG"")]
    public void Bar()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(
""DEBUG"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAtributeParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(""DEBUG"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributeNoParametersAsync()
        {
            var testCode = @"
[System.Serializable]
class Foo
{

}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesListParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [

Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(7, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAttributesListParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [
Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAtributesListParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
using System.Diagnostics;
class Foo
{
    [Conditional(""DEBUG""),Conditional(""DEBUG2"")]
    public void Bar()
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(

string s);
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(
string s);
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar(string s);
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestDelegateDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public delegate void Bar();
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(

int z, int j)
        {

        };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(
int z, int j)
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = delegate(int z, int j)
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = delegate()
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestAnonymousMethodDeclarationNoOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = delegate
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (

z,j) =>
        {

        };
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(8, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (
z,j) =>
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action<int,int> c = (z,j) =>
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestLambdaExpressionDeclarationNoParametersAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar()
    {
        System.Action c = () => 
        {

        };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(

int i)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(
int i)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCastOperatorDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static explicit operator Foo(int i)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(

Foo a, Foo b)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestUnaryOperatorOverloadDeclarationParametersList2LinesAfterOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(

Foo a)
    {
        return new Foo();
    }
}";

            DiagnosticResult expected = Diagnostic().WithLocation(6, 1);

            await VerifyCSharpDiagnosticAsync(testCode, expected, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersListOnNextLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(
Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestOperatorOverloadDeclarationParametersListOnSameLineAsOpeningParenthesisAsync()
        {
            var testCode = @"
public class Foo
{
    public static Foo operator +(Foo a, Foo b)
    {
        return new Foo();
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestObjectCreationNoArgumentListAsync()
        {
            var testCode = @"
public class Foo
{
    public static void Bar()
    {
        var list = new System.Collections.Generic.List<int> { 42 };
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestPragmaDirectivesAsync()
        {
            var testCode = @"using System;

public class SomeOtherClass
{
    private void SomeMethod()
    {
        this.SomeOtherMethod(
#pragma warning disable 618
                this.SomeObsoleteMethod());
#pragma warning restore 618
    }

    [Obsolete(""Don't use me!"")]
    private int SomeObsoleteMethod()
    {
        return 0;
    }

    private void SomeOtherMethod(int someParameter)
    {
    }
}";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifies that directive trivia will not result in diagnostics.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        [WorkItem(1623, "https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/1623")]
        public async Task TestWithDirectiveTriviaAsync()
        {
            var testCode = @"
public interface ITestInterface1 { }

public interface ITestInterface2 { }

public class TestClass
{
    public void TestMethod(
#if TESTSYMBOL
        ITestInterface1 instance)
#else
        ITestInterface2 instance)
#endif
    {
    }
}
";

            await VerifyCSharpDiagnosticAsync(testCode, DiagnosticResult.EmptyDiagnosticResults, CancellationToken.None).ConfigureAwait(false);
        }

        [Theory]
        [InlineData("public Foo(GAP{|#0:string s|}) { }")]
        [InlineData("public void Bar(GAP{|#0:string s|}) { }")]
        [InlineData("public string this[GAP{|#0:int i|}] => null;")]
        [InlineData("public static Foo operator +(GAP{|#0:Foo a|}, Foo b) => a;")]
        [InlineData("public static explicit operator int(GAP{|#0:Foo a|}) => 0;")]
        [InlineData("delegate void Del(GAP{|#0:string s|});")]
        [InlineData("void Method() { var x = 1.Equals(GAP{|#0:1|}); }")]
        [InlineData("void Method() { var x = new string(GAP{|#0:'a'|}, 1); }")]
        [InlineData("void Method() { var a = new int[2]; var x = a[GAP{|#0:0|}]; }")]
        [InlineData("void Method() { var a = new int[GAP{|#0:2|}]; }")]
        [InlineData("[System.Obsolete(GAP{|#0:\"x\"|})] void Method() { }")]
        [InlineData("[GAP{|#0:System.Obsolete|}] void Method() { }")]
        [InlineData("void Method() { System.Action<int> a = delegate(GAP{|#0:int i|}) { }; }")]
        [InlineData("void Method() { System.Action<int> a = (GAP{|#0:int i|}) => { }; }")]
        public async Task TestCodeFixRemovesBlankLinesAsync(string member)
        {
            const string gap = "\r\n\r\n        ";
            const string fixedGap = "\r\n        ";

            await VerifyFixAsync(member, gap, fixedGap).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixRemovesWhitespaceOnlyLinesAsync()
        {
            const string gap = "\r\n    \r\n \r\n   \r\n        ";
            const string fixedGap = "\r\n        ";

            await VerifyFixAsync("public void Bar(GAP{|#0:string s|}, int i) { }", gap, fixedGap).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixPreservesCommentOnOpeningBracketLineAsync()
        {
            const string gap = " // comment\r\n\r\n        ";
            const string fixedGap = " // comment\r\n        ";

            await VerifyFixAsync("public void Bar(GAP{|#0:string s|}) { }", gap, fixedGap).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixPreservesCommentOnFirstParameterLineAsync()
        {
            const string gap = "\r\n\r\n        /* comment */ ";
            const string fixedGap = "\r\n        /* comment */ ";

            await VerifyFixAsync("public void Bar(GAP{|#0:string s|}) { }", gap, fixedGap).ConfigureAwait(false);
        }

        /// <summary>
        /// A comment on its own line is never moved or removed: only the blank lines around it are removed, so the
        /// first parameter can still be reported when the comment keeps it more than one line below the bracket.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestCodeFixKeepsOwnLineCommentAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(

        // comment

        string s)
    {
    }
}";

            var fixedCode = @"
class Foo
{
    public void Bar(
        // comment
        string s)
    {
    }
}";

            var test = new CSharpTest
            {
                TestCode = testCode,
                FixedCode = fixedCode,
                NumberOfIncrementalIterations = 1,
                NumberOfFixAllIterations = 1,
            };

            test.ExpectedDiagnostics.Add(Diagnostic().WithLocation(8, 9));
            test.RemainingDiagnostics.Add(Diagnostic().WithLocation(6, 9));
            await test.RunAsync(CancellationToken.None).ConfigureAwait(false);
        }

        /// <summary>
        /// When there are no blank lines to remove, the comments alone keep the first parameter away from the bracket,
        /// so no code fix is offered and the document stays unchanged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TestNoCodeFixWithoutBlankLinesAsync()
        {
            var testCode = @"
class Foo
{
    public void Bar(
        // first comment
        // second comment
        {|#0:string s|})
    {
    }
}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), testCode, CancellationToken.None).ConfigureAwait(false);
        }

        [Fact]
        public async Task TestCodeFixAllAsync()
        {
            var testCode = @"
class Foo
{
    public Foo(

        {|#0:int a|})
    {
    }

    public void Bar(


        {|#1:string s|}, int i)
    {
        var x = 1.Equals(

            {|#2:1|});
        var y = new string(

            {|#3:'a'|}, 1);
    }
}";

            var fixedCode = @"
class Foo
{
    public Foo(
        int a)
    {
    }

    public void Bar(
        string s, int i)
    {
        var x = 1.Equals(
            1);
        var y = new string(
            'a', 1);
    }
}";

            DiagnosticResult[] expected =
            {
                Diagnostic().WithLocation(0),
                Diagnostic().WithLocation(1),
                Diagnostic().WithLocation(2),
                Diagnostic().WithLocation(3),
            };

            await VerifyCSharpFixAsync(testCode, expected, fixedCode, CancellationToken.None).ConfigureAwait(false);
        }

        private static async Task VerifyFixAsync(string member, string gap, string fixedGap)
        {
            var testCode = "\r\nclass Foo\r\n{\r\n    " + member.Replace("GAP", gap) + "\r\n}";
            var fixedCode = "\r\nclass Foo\r\n{\r\n    " + member.Replace("GAP", fixedGap).Replace("{|#0:", string.Empty).Replace("|}", string.Empty) + "\r\n}";

            await VerifyCSharpFixAsync(testCode, Diagnostic().WithLocation(0), fixedCode, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
