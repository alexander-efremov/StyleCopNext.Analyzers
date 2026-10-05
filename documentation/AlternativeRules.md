### Alternative Rules (SX0000-)
Rules which are non-standard extensions to the default StyleCop behavior, and represent an alternative style which is adopted by some projects. Alternative rules are known to directly conflict with standard StyleCop rules.

Alternative rules belong to the category of the standard rules they replace. For `.editorconfig` and `SuppressMessage`,
use `StyleCop.CSharp.ReadabilityRules` for SX1101 and `StyleCop.CSharp.NamingRules` for SX1309 and SX1309S. See
[Configuring rules](ConfiguringRules.md).

Identifier | Name | Description
-----------|------|------------
[SX1101](SX1101.md) | DoNotPrefixLocalMembersWithThis | A call to an instance member of the local class or a base class is prefixed with 'this.', within a C# code file. 
[SX1309](SX1309.md) | FieldNamesMustBeginWithUnderscore | A field name does not begin with an underscore.
[SX1309S](SX1309S.md) | StaticFieldNamesMustBeginWithUnderscore | A static field name does not begin with an underscore.
