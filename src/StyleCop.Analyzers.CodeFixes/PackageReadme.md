# StyleCopNext.Analyzers

StyleCop rules as Roslyn analyzers and code fixes. A maintained continuation of
[StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers): the same SA rules and `stylecop.json`
settings, with stable releases.

## Install

```shell
dotnet add package StyleCopNext.Analyzers
```

## Requirements

This is the 1.x line, for older toolchains and Unity. For .NET SDK 8 or Visual Studio 17.8 and later use version 2.x.

## Migrate from StyleCop.Analyzers

Replace the `StyleCop.Analyzers` package reference with `StyleCopNext.Analyzers`. Rule IDs, `.editorconfig` severities
and `stylecop.json` stay as they are. Rules changed since StyleCop.Analyzers 1.2.0-beta.556 can report new issues; for
example SA1121 asks for `nint` instead of `IntPtr`.

## Links

- [Source and releases](https://github.com/alexander-efremov/StyleCopNext.Analyzers)
- [Rule documentation](https://github.com/alexander-efremov/StyleCopNext.Analyzers/tree/release/1.x/documentation)
- License: MIT
