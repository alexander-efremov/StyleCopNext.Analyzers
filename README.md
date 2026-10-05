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

The severity of individual rules may be configured using [rule set files](https://docs.microsoft.com/en-us/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules)
in Visual Studio 2015 or newer. **Settings.StyleCop** is not supported, but a **stylecop.json** file may be used to
customize the behavior of certain rules. See [Configuration.md](documentation/Configuration.md) for more information.
See [ConfiguringRules.md](documentation/ConfiguringRules.md) for how to set rule severities in **.editorconfig** and
**.globalconfig**, run the analyzers only in the IDE, and exclude files from analysis.

For documentation and reasoning on the rules themselves, see the [Documentation](DOCUMENTATION.md).

For users upgrading from StyleCop Classic, see the [migration guide](documentation/MigratingFromStyleCopClassic.md) and
[KnownChanges.md](documentation/KnownChanges.md) for information about known differences which you may notice when
switching to StyleCop Analyzers.

### C# language versions
Not all versions of StyleCop.Analyzers support all features of each C# language version. The table below shows the minimum version of StyleCop.Analyzers required for proper support of a C# language version.

| C# version | StyleCop.Analyzers version | Visual Studio version |
|------------|----------------------------|-----------------------|
| 1.0 - 6.0  | v1.0.2 or higher           | VS2015+               |
| 7.0 - 7.3  | v1.1.0-beta or higher      | VS2017+               |
|    8.0     | v1.2.0-beta or higher      | VS2019                |

## Installation

StyleCopNext.Analyzers can be installed using the NuGet command line or the NuGet Package Manager in Visual Studio 2015.

**Install using the command line:**
```bash
Install-Package StyleCopNext.Analyzers
```

**Install using the package manager:**
![Install via nuget](https://cloud.githubusercontent.com/assets/1408396/8233513/491f301a-159c-11e5-8b7a-1e16a0695da6.png)

## Team Considerations

If you use older versions of Visual Studio in addition to Visual Studio 2015 or Visual Studio 2017, you may still install these analyzers. They will be automatically disabled when you open the project back up in Visual Studio 2013 or earlier.

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
