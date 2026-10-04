# Migrating from StyleCop Classic

This guide describes how to move a project from StyleCop Classic (the **StyleCop.MSBuild** package, or the
**StyleCop.targets** MSBuild integration) to StyleCopNext.Analyzers. The analyzers run in the compiler, so no separate
StyleCop build step or Visual Studio extension is needed. **Settings.StyleCop** files are not read by the analyzers.

## 1. Remove StyleCop Classic

Remove the following from the solution, as present:

* The **StyleCop.MSBuild** (or **StyleCop**) package reference.
* An import of **StyleCop.targets** in a project file or **Directory.Build.props**, for example
  `<Import Project="...\StyleCop.targets" />`.
* The MSBuild properties used by StyleCop Classic, such as `StyleCopEnabled`, `StyleCopTreatErrorsAsWarnings`,
  `StyleCopOverrideSettingsFile`, and `StyleCopForceFullAnalysis`.
* The StyleCop Classic Visual Studio extension, if installed.

## 2. Add StyleCopNext.Analyzers

Add the package to a project:

```text
dotnet add package StyleCopNext.Analyzers
```

To enable the analyzers for all projects at once, add the reference to **Directory.Build.props** in the solution root:

```xml
<Project>
  <ItemGroup>
    <PackageReference Include="StyleCopNext.Analyzers" Version="1.0.2" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

Use the current version from [NuGet](https://www.nuget.org/packages/StyleCopNext.Analyzers). Warnings for rule
violations are now reported by the compiler, and in the IDE while editing.

## 3. Move the configuration

### Rule severities

Rules which were turned off in **Settings.StyleCop** are turned off by their rule ID in a **.editorconfig** file in the
solution root. The rules are listed in [DOCUMENTATION.md](../DOCUMENTATION.md); differences from StyleCop Classic are
described in [KnownChanges.md](KnownChanges.md).

```ini
root = true

[*.cs]
dotnet_diagnostic.SA1633.severity = none
dotnet_diagnostic.SA1309.severity = none
```

The default severity of the rules is `warning`. To treat violations as errors, as StyleCop Classic did without
`StyleCopTreatErrorsAsWarnings`, set the severity to `error` or use `TreatWarningsAsErrors` in the project.

[ConfiguringRules.md](ConfiguringRules.md) describes the other options: severities per category, **.globalconfig**,
running the analyzers only in the IDE, and excluding files from analysis.

### Rule settings

Settings which change how a rule works, such as the company name and copyright text, the allowed Hungarian prefixes, or
the documentation requirements, go to a **stylecop.json** file. Link it into the projects as an additional file. In
**Directory.Build.props**:

```xml
<ItemGroup>
  <AdditionalFiles Include="$(MSBuildThisFileDirectory)stylecop.json" Link="stylecop.json" />
</ItemGroup>
```

See [Configuration.md](Configuration.md) for the available settings and [EnableConfiguration.md](EnableConfiguration.md)
for other ways to add the file.

## 4. Fix the violations

Many rules have a code fix. In Visual Studio, use **Fix All** (in Document, Project, or Solution) from the Quick Actions
menu of a violation. From the command line, use `dotnet format`:

```text
dotnet format analyzers --diagnostics SA1101 SA1210 --severity warning
```

Pass the IDs of the rules to fix in `--diagnostics`. Review the result before committing, since a code fix can change
many files. Rules without a code fix must be fixed by hand, or turned off in **.editorconfig**.

## Known differences

Some rules behave differently from StyleCop Classic. These changes are listed in [KnownChanges.md](KnownChanges.md).
