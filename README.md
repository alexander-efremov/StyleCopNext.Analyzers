# StyleCopNext

A maintained continuation of [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers), published as
[StyleCopNext.Analyzers](https://www.nuget.org/packages/StyleCopNext.Analyzers): the same SA rules and `stylecop.json`
settings, with stable releases. To switch, replace the `StyleCop.Analyzers` package reference with
`StyleCopNext.Analyzers`.

This is the 1.x line for older toolchains (.NET SDK before 8, Visual Studio before 17.8, Unity). It receives bug fixes
only; new rules and C# support go to 2.x on master. The original README follows.

# StyleCop Analyzers for the .NET Compiler Platform

[![NuGet](https://img.shields.io/nuget/v/StyleCopNext.Analyzers.svg)](https://www.nuget.org/packages/StyleCopNext.Analyzers)

[![CI](https://github.com/alexander-efremov/StyleCopNext.Analyzers/actions/workflows/ci.yml/badge.svg?branch=release/1.x)](https://github.com/alexander-efremov/StyleCopNext.Analyzers/actions/workflows/ci.yml?query=branch%3Arelease/1.x)

This repository contains an implementation of the StyleCop rules using the .NET Compiler Platform. Where possible, code fixes are also provided to simplify the process of correcting violations.

## Using StyleCopNext.Analyzers

The preferable way to use the analyzers is to add the nuget package [StyleCopNext.Analyzers](https://www.nuget.org/packages/StyleCopNext.Analyzers/)
to the project where you want to enforce StyleCop rules.

The severity of individual rules is configured in **.editorconfig** or **.globalconfig** files, or in
[rule set files](https://docs.microsoft.com/en-us/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules).
**Settings.StyleCop** is not supported, but a **stylecop.json** file may be used to customize the behavior of certain
rules. See [Configuration.md](documentation/Configuration.md) for more information. See
[ConfiguringRules.md](documentation/ConfiguringRules.md) for how to set rule severities in **.editorconfig** and
**.globalconfig**, run the analyzers only in the IDE, and exclude files from analysis.

For documentation and reasoning on the rules themselves, see the [Documentation](DOCUMENTATION.md).

For users upgrading from StyleCop Classic, see the [migration guide](documentation/MigratingFromStyleCopClassic.md) and
[KnownChanges.md](documentation/KnownChanges.md) for information about known differences which you may notice when
switching to StyleCopNext.Analyzers.

### Versions and toolchains

| StyleCopNext.Analyzers | Required toolchain |
|------------------------|--------------------|
| 2.x (`master`) | .NET SDK 8 or Visual Studio 17.8 or later |
| 1.x (`release/1.x`, bug fixes only) | Older toolchains, including Visual Studio 2015 or later, and Unity |

Syntax of C# 7 through C# 13 is covered by the test suite on both lines. The compiler of your toolchain determines which
C# language versions can be used in the project.

## Installation

Add the package to a project with the .NET CLI:

```shell
dotnet add package StyleCopNext.Analyzers
```

Or reference it in the project file:

```xml
<PackageReference Include="StyleCopNext.Analyzers" Version="1.0.2" PrivateAssets="all" IncludeAssets="runtime; build; native; contentfiles; analyzers; buildtransitive" />
```

## Team Considerations

Every team member and the CI build must use a toolchain that supports the chosen line of the package: 2.x needs
.NET SDK 8 or Visual Studio 17.8 or later, while 1.x works with older toolchains. Include **stylecop.json**,
**.editorconfig** and **.globalconfig** in source control so that all machines apply the same settings.

## Contributing

See [Contributing](CONTRIBUTING.md)

## Current status

The rules are listed by category, with a description and a documentation page for each rule.

* [Special Rules (SA0000-)](documentation/SpecialRules.md)
* [Spacing Rules (SA1000-)](documentation/SpacingRules.md)
* [Readability Rules (SA1100-)](documentation/ReadabilityRules.md)
* [Ordering Rules (SA1200-)](documentation/OrderingRules.md)
* [Naming Rules (SA1300-)](documentation/NamingRules.md)
* [Maintainability Rules (SA1400-)](documentation/MaintainabilityRules.md)
* [Layout Rules (SA1500-)](documentation/LayoutRules.md)
* [Documentation Rules (SA1600-)](documentation/DocumentationRules.md)
* [Alternative Rules (SX0000-)](documentation/AlternativeRules.md)
