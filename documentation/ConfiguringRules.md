# Configuring rules

This page describes how to enable, disable, and change the severity of StyleCop Analyzers rules, and how to limit
where they run. Settings that change how a rule behaves (company name, indentation, using placement, ...) are described
in [Configuration.md](Configuration.md).

## Rule severity in .editorconfig

The severity of a rule is set in an [**.editorconfig**](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files)
file in the repository, using the rule ID:

```ini
[*.cs]
# Turn a rule off
dotnet_diagnostic.SA1309.severity = none

# Change a rule to a warning or an error
dotnet_diagnostic.SA1600.severity = warning
dotnet_diagnostic.SA1633.severity = error
```

The supported values are `none`, `silent`, `suggestion`, `warning`, `error`, and `default`.

The severity of all rules in a category can be set with a category setting. The category name is part of the key:

```ini
[*.cs]
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.NamingRules.severity = suggestion
```

A setting for an individual rule takes precedence over the setting for its category, and the category setting takes
precedence over `dotnet_analyzer_diagnostic.severity`, which applies to all analyzers.

### Category names

The category name is not the name shown in the documentation (for example `Documentation Rules`). Use the following
names exactly:

| Rules | Category name |
| --- | --- |
| [Special Rules (SA0000-)](SpecialRules.md) | `StyleCop.CSharp.SpecialRules` |
| [Spacing Rules (SA1000-)](SpacingRules.md) | `StyleCop.CSharp.SpacingRules` |
| [Readability Rules (SA1100-)](ReadabilityRules.md) | `StyleCop.CSharp.ReadabilityRules` |
| [Ordering Rules (SA1200-)](OrderingRules.md) | `StyleCop.CSharp.OrderingRules` |
| [Naming Rules (SA1300-)](NamingRules.md) | `StyleCop.CSharp.NamingRules` |
| [Maintainability Rules (SA1400-)](MaintainabilityRules.md) | `StyleCop.CSharp.MaintainabilityRules` |
| [Layout Rules (SA1500-)](LayoutRules.md) | `StyleCop.CSharp.LayoutRules` |
| [Documentation Rules (SA1600-)](DocumentationRules.md) | `StyleCop.CSharp.DocumentationRules` |

The same names are used as the first argument of `SuppressMessage`, for example
`[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "...")]`.

### Compilation-level diagnostics and .globalconfig

Section headers in an **.editorconfig** file select source files by path. A diagnostic which is not tied to a source
file, such as [SA0001](SA0001.md), is not affected by a `[*.cs]` section. Set the severity of such diagnostics in a
[**.globalconfig**](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/configuration-files#global-analyzerconfig)
file. A **.globalconfig** file also works for files which are not below the **.editorconfig** file, for example when one
file is shared by several repositories:

```ini
is_global = true

dotnet_diagnostic.SA0001.severity = none
```

A file named **.globalconfig** in the project directory or any directory above it is picked up automatically by the
.NET SDK. A file with another name is added with a `GlobalAnalyzerConfigFiles` item:

```xml
<ItemGroup>
  <GlobalAnalyzerConfigFiles Include="$(MSBuildThisFileDirectory)stylecop.globalconfig" />
</ItemGroup>
```

Rule severity can also be set with a [rule set file](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules)
or the `NoWarn` / `WarningsAsErrors` MSBuild properties. **.editorconfig** and **.globalconfig** settings are preferred.

## Running the analyzers only in the IDE

Analyzers run in the IDE while editing (live analysis) and during the build. To keep the squiggles in the editor
but skip the analyzers when building, use the following MSBuild properties in a project file or in
**Directory.Build.props**:

| Property | Effect when `false` |
| --- | --- |
| `RunAnalyzers` | Analyzers do not run, neither during the build nor in live analysis |
| `RunAnalyzersDuringBuild` | Analyzers do not run during the build; live analysis in the IDE is not affected |
| `RunAnalyzersDuringLiveAnalysis` | Analyzers do not run in live analysis; the build is not affected |

For example, to run the analyzers only in the IDE when building inside Visual Studio, and keep them for command-line
and CI builds:

```xml
<PropertyGroup Condition="'$(BuildingInsideVisualStudio)' == 'true'">
  <RunAnalyzersDuringBuild>false</RunAnalyzersDuringBuild>
</PropertyGroup>
```

Projects which do not honor these properties can remove the analyzers from the build with a target:

```xml
<Target Name="DisableAnalyzersForVisualStudioBuild"
        BeforeTargets="CoreCompile"
        Condition="'$(BuildingInsideVisualStudio)' == 'True' And '$(BuildingProject)' == 'True'">
  <ItemGroup>
    <Analyzer Remove="@(Analyzer)" />
  </ItemGroup>
</Target>
```

> :warning: Skipping the analyzers in the build can change the build behavior in ways that increase the chance of
> submitting code which does not compile in the automated build. Avoid it for projects which use `/warnaserror`
> (`TreatWarningsAsErrors`), have an analyzer installed with a default severity of error, or set the severity of a rule to
> error. Always run the analyzers in the CI build.

## Excluding files from analysis

### Generated code

The StyleCop Analyzers do not analyze generated code. A file is treated as generated when its name ends in `.g.cs`,
`.g.i.cs`, `.designer.cs`, or `.generated.cs`, when it starts with an `<auto-generated>` comment,
or when it is marked in **.editorconfig**:

```ini
[Generated/**.cs]
generated_code = true
```

### Selected files and folders

To exclude other files, such as third-party code, use an **.editorconfig** section for their path and set the severity
to `none`. The section can turn off individual rules, or all rules of a category:

```ini
[ThirdParty/**.cs]
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.DocumentationRules.severity = none
dotnet_analyzer_diagnostic.category-StyleCop.CSharp.LayoutRules.severity = none
dotnet_diagnostic.SA1633.severity = none
```

Files can also be marked with `generated_code = true` in such a section, as shown above.

### Why SuppressMessage is not enough

`SuppressMessage` applies to a symbol (type, member, assembly) and the code inside it. Diagnostics which are reported
for a whole file, such as [SA1633](SA1633.md) (file header) and [SA1200](SA1200.md) (using directives), have no containing symbol, so `SuppressMessage` cannot suppress them. Use an
**.editorconfig** section as shown above, or a `#pragma warning disable` directive at the top of the file.

## Next steps

Settings that change the behavior of individual rules are described in [Configuration.md](Configuration.md).

Rule pages such as [SA1200](SA1200.md) end with a **Related .NET rules** section, which lists the .NET code style rules
and **.editorconfig** options that match or overlap the rule, so that Visual Studio formatting can agree with StyleCop.
