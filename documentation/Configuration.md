# Configuring StyleCop Analyzers

StyleCop Analyzers can be configured using multiple separate mechanisms:

1. Code analysis rule set files

   * Enable and disable individual rules
   * Configure the severity of violations reported by individual rules

2. **stylecop.json**

   * Specify project-specific text, such as the name of the company and the structure to use for copyright headers
   * Fine-tune the behavior of certain rules

3. **.editorconfig**

   * Can be used in place of rule set files and **stylecop.json**

Rule severities can also be set in **.editorconfig** and **.globalconfig** files, see [ConfiguringRules.md](ConfiguringRules.md).
Many of the **stylecop.json** settings can also be provided in **.editorconfig**, see
[Settings in .editorconfig](#settings-in-editorconfig).
Each rule page lists the settings which affect that rule in its **Configuration** section.

Code analysis rule sets are the standard way to configure most diagnostic analyzers within Visual Studio. Information about creating and customizing these files can be found in the [Using Rule Sets to Group Code Analysis Rules](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules) documentation on docs.microsoft.com.

An example rule set file containing the default StyleCop Analyzers configuration is available at <https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/rulesets/StyleCopAnalyzersDefault.ruleset>.

## Getting Started with **stylecop.json**

The easiest way to add a **stylecop.json** configuration file to a new project is using a code fix provided by the project. To invoke the code fix, open any file where SA1633 is reported¹ and press Ctrl+. to bring up the Quick Fix menu. From the menu, select **Add StyleCop settings file to the project**.

The dot file naming convention is also supported, which makes it possible to name the configuration file **.stylecop.json**.

### JSON Schema for IntelliSense

A JSON schema is available for **stylecop.json**. By including a reference in **stylecop.json** to this schema, Visual Studio will offer IntelliSense functionality (code completion, quick info, etc.) while editing this file. The schema may be configured by adding the following top-level property in **stylecop.json**:

```json
{
  "$schema": "https://raw.githubusercontent.com/DotNetAnalyzers/StyleCopAnalyzers/master/StyleCop.Analyzers/StyleCop.Analyzers/Settings/stylecop.schema.json"
}
```

> :bulb: The code fix described previously automatically configures **stylecop.json** to reference the schema.
> If the schema appears to be out-of-date in Visual Studio, right click anywhere in the **stylecop.json** document and then select **Reload Schemas**.

### Source Control

For best results, **stylecop.json** should be included in source control. This will automatically propagate the expected settings to all team members working on the project.

> :warning: If you are working in Git, make sure your **.gitignore** file *does not* contain the following line. This line should be removed if present.
>
> ```
> [Ss]tyle[Cc]op.*
> ```

## Indentation

This section describes the indentation rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties can be configured in the `indentation` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "indentation": {
    }
  }
}
```

### Basic Indentation

The following properties are used in **stylecop.json** to configure basic indentation in StyleCop Analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `indentationSize` | **4** | 1.1.0 | The number of columns to use for each indentation of code. Depending on the `useTabs` and `tabSize` settings, this will be filled with tabs and/or spaces. |
| `tabSize` | **4** | 1.1.0 | The width of a hard tab character in source code. This value is used when converting between tabs and spaces. |
| `useTabs` | **false** | 1.1.0 | **true** to indent using hard tabs; otherwise, **false** to indent using spaces |

When using an **.editorconfig** file to configure StyleCop Analyzers, the basic indentation settings (`indent_size`, `tab_width` and `indent_style`) as described at editorconfig.org can be used.
> :bulb: When working in Visual Studio, the IDE will not automatically adjust editor settings according to the values in
> **stylecop.json**. To provide this functionality, we recommend using the **.editorconfig** file instead. Users of the [EditorConfig](https://visualstudiogallery.msdn.microsoft.com/c8bccfe2-650c-4b42-bc5c-845e21f96328)
> extension for Visual Studio will not need to update their C# indentation settings in order to match your project style.

### Indentation Behavior

The following properties are used by [SA1138](SA1138.md) to determine the expected indentation of code. The default
values match the default C# formatting options of Visual Studio.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `indentBlock` | **true** | 2.0.0 | **true** to indent the contents of blocks (including method bodies) relative to the line containing the block owner; otherwise, **false** to place the contents at the same indentation as the owner. |
| `indentSwitchSection` | **true** | 2.0.0 | **true** to indent `case` and `default` labels relative to the `switch` statement; otherwise, **false** to align them with the `switch` statement. |
| `indentSwitchCaseSection` | **true** | 2.0.0 | **true** to indent the statements of a switch section relative to its `case` or `default` labels; otherwise, **false** to align them with the labels. |
| `labelPositioning` | **oneLess** | 2.0.0 | The position of labels targeted by `goto` statements. `leftMost` places labels in the first column, `oneLess` indents labels one level less than the statements of the enclosing block, and `noIndent` indents labels the same as the statements of the enclosing block. |

The same options can be provided in an [**.editorconfig**](http://editorconfig.org/) file using the standard .NET
formatting keys. Values in **stylecop.json** take precedence over values in **.editorconfig**, and a severity suffix
(for example `true:suggestion`) is ignored.

| Property | .editorconfig key | .editorconfig values |
| --- | --- | --- |
| `indentBlock` | `csharp_indent_block_contents` | `true`, `false` |
| `indentSwitchSection` | `csharp_indent_switch_labels` | `true`, `false` |
| `indentSwitchCaseSection` | `csharp_indent_case_contents` | `true`, `false` |
| `labelPositioning` | `csharp_indent_labels` | `one_less_than_current` (`oneLess`), `flush_left` (`leftMost`), `no_change` (`noIndent`) |

```json
{
  "settings": {
    "indentation": {
      "indentBlock": true,
      "indentSwitchSection": true,
      "indentSwitchCaseSection": true,
      "labelPositioning": "oneLess"
    }
  }
}
```

## Spacing Rules

This section describes the features of spacing rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties are configured in the `spacingRules` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "spacingRules": {
    }
  }
}
```

> Currently there are no configurable settings for spacing rules.

## Readability Rules

This section describes the features of readability rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties are configured in the `readabilityRules` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "readabilityRules": {
    }
  }
}
```

### Aliases for Built-In Types

The following property is used in **stylecop.json** to configure aliases for built-in types.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `allowBuiltInTypeAliases` | **false** | 1.1.0-beta007 | Specifies whether aliases are allowed for built-in types. |

By default, SA1121 reports a diagnostic for the use of named aliases for built-in types:

```csharp
using HRESULT = System.Int32;

HRESULT hr = SomeNativeOperation(); // SA1121
```

The `allowBuiltInTypeAliases` configuration property can be set to `true` to allow cases like this while continuing to report diagnostics for direct references to the metadata type name, `Int32`.

When using an **.editorconfig** file to configure StyleCop Analyzers, the following property can be used:
```ini
stylecop.readability.allowBuiltInTypeAliases = true
```

## Ordering Rules

This section describes the features of ordering rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties are configured in the `orderingRules` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "orderingRules": {
    }
  }
}
```

### Element Order

The following properties are used in **stylecop.json** to configure element ordering in StyleCop Analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `elementOrder` | `[ "kind", "accessibility", "constant", "static", "readonly" ]` | 1.0.0 | Specifies the traits used for ordering elements within a document, along with their precedence |

The `elementOrder` property is an array of element traits. The ordering rules (SA1201, SA1202, SA1203, SA1204, SA1214,
and SA1215) evaluate these traits in the order they are defined to identify ordering problems, and the code fix uses
this property when reordering code elements. Any traits which are omitted from the array are ignored. The following
traits are supported:

* `kind`: Elements are ordered according to their kind (see [SA1201](SA1201.md) for this predefined order)
* `accessibility`: Elements are ordered according to their declared accessibility (see [SA1202](SA1202.md) for this
  predefined order)
* `constant`: Constant elements are ordered before non-constant elements
* `static`: Static elements are ordered before non-static elements
* `readonly`: Readonly elements are ordered before non-readonly elements

This configuration property allows for a wide variety of ordering configurations, as shown in the following examples.

#### Example: All Constants First

The following example shows a customized element order where *all* constant fields are placed before non-constant
fields, regardless of accessibility.

```json
{
  "settings": {
    "orderingRules": {
      "elementOrder": [
        "kind",
        "constant",
        "accessibility",
        "static",
        "readonly"
      ]
    }
  }
}
```

#### Example: Ignore Accessibility

The following example shows a customized element order where element accessibility is simply ignored, but other ordering
rules remain enforced.

```json
{
  "settings": {
    "orderingRules": {
      "elementOrder": [
        "kind",
        "constant",
        "static",
        "readonly"
      ]
    }
  }
}
```

> :bulb: This property can currently not be set in an **.editorconfig** file.

### Using Directives

The following properties are used in **stylecop.json** to configure using directives in StyleCop Analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `systemUsingDirectivesFirst` | true | 1.0.0 | Specifies whether `System` using directives are placed before other using directives |
| `usingDirectivesPlacement` | `"insideNamespace"` | 1.0.0 | Specifies the desired placement of using directives |
| `blankLinesBetweenUsingGroups` | `"allow"` | 1.1.0 | Specifies is blank lines are required to separate groups of using statements |

`systemUsingDirectivesFirst` affects the following rules and their code fixes:

* [SA1208](SA1208.md) only reports `System` using directives placed after other using directives when this property is `true`.
* [SA1210](SA1210.md) sorts `System` namespaces ahead of other namespaces when this property is `true`, and sorts all
  namespaces together alphabetically when it is `false`.
* [SA1217](SA1217.md) sorts `using static` directives for `System` types ahead of other `using static` directives when
  this property is `true`, and sorts them all together alphabetically when it is `false`.

When using an **.editorconfig** file to configure StyleCop Analyzers, the respective properties for [formatting .NET/C#](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/formatting-rules) can be used:
```ini
dotnet_sort_system_directives_first = true
csharp_using_directive_placement = inside_namespace
dotnet_separate_import_directive_groups = true
```

#### Using Directives Placement

The `usingDirectivesPlacement` property affects the behavior of the following rules which report incorrectly placed
using directives.

* [SA1200 Using directives should be placed correctly](SA1200.md)

> :warning: Use of certain features, including but not limited to preprocessor directives, may cause the using
> directives code fix to not relocate using directives automatically. If SA1200 is still reported after applying the Fix
> All operation for using directives, the remaining cases will need to be resolved manually.

This property has three allowed values, which are described as follows.

##### `"insideNamespace"`

In this mode, using directives should be placed *inside* of namespace declarations. This is the default mode, and
adheres to the original SA1200 behavior from StyleCop Classic.

* SA1200 reports using directives which are located outside of a namespace declaration (a few exceptions exist for cases
  where this is required)
* Using directives code fix moves using directives inside of namespace declarations where possible

##### `"outsideNamespace"`

In this mode, using directives should be placed *outside* of namespace declarations.

* SA1200 reports using directives which are located inside of a namespace declaration
* Using directives code fix moves using directives outside of namespace declarations where possible

##### `"preserve"`

In this mode, using directives may be placed inside or outside of namespaces.

* SA1200 does not report any violations
* Using directives code fix may reorder using directives, but does not relocate them

#### Blank Lines Between Groups
The `blankLinesBetweenUsingGroups` property affects the behavior of the following rules which report the presence / absence
of blanks lines between groups of using directives.

* [SA1516 Elements should be separated by blank line](SA1516.md)

Using directives can grouped based on the purpose of the using directive.
StyleCop Analyzers recognizes the following using directive group types:

- System using directives (only when `systemUsingDirectivesFirst` is true)
- Normal using directives
- Static using directives
- Alias using directives

This property has three allowed values, which are described as follows.

##### `"allow"`

In this mode, a blank line between groups for using directives is *optional*.

* No diagnostic will be produced.
* Using directives code fix will not insert blank lines.

##### `"require"`

In this mode, a blank line between groups for using directives is *mandatory*.

* SA1516 reports missing blank lines between using directive groups.
* Using directives code fix will insert blank lines.
* SA1516 code fix will add a missing blank line.

##### `"omit"`

In this mode, a blank line between groups for using directives is *not allowed*.

* SA1516 reports blank lines between using directive groups.
* Using directives code fix will not insert blank lines.
* SA1516 code fix will remove blank lines between using directive groups.

## Naming Rules

This section describes the features of naming rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties are configured in the `namingRules` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "namingRules": {
    }
  }
}
```

### Hungarian Notation

The following properties are used in **stylecop.json** to configure allowable Hungarian notation prefixes in StyleCop Analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `allowCommonHungarianPrefixes` | **true** | 1.0.0 | Specifies whether common non-Hungarian notation prefixes should be allowed. When true, the two-letter words 'as', 'at', 'by', 'do', 'go', 'if', 'in', 'is', 'it', 'no', 'of', 'on', 'or', and 'to' are allowed to appear as prefixes for variable names. |
| `allowedHungarianPrefixes` | `[ ]` | 1.0.0 | Specifies additional prefixes which are allowed to be used in variable names. See the example below for more information. |

The following example shows a settings file which allows the common prefixes as well as the custom prefixes 'md' and 'cd'.

```json
{
  "settings": {
    "namingRules": {
      "allowedHungarianPrefixes": [
        "cd",
        "md"
      ]
    }
  }
}
```

When using an **.editorconfig** file to configure StyleCop Analyzers, the following properties can be used:
```ini
stylecop.naming.allowCommonHungarianPrefixes = true
stylecop.naming.allowedHungarianPrefixes = cd, md
```

### Namespace Components

The following property is used in **stylecop.json** to configure allowable namespace components (e.g. ones that start with a lowercase letter).

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `allowedNamespaceComponents` | `[ ]` | 1.2.0 | Specifies namespace components that are allowed to be used. See the example below for more information. |

The following example shows a settings file which allows namespace components such as `eBay` or `Apple.iPod`.

```json
{
  "settings": {
    "namingRules": {
      "allowedNamespaceComponents": [
        "eBay",
        "iPod"
      ]
    }
  }
}
```

When using an **.editorconfig** file to configure StyleCop Analyzers, the following property can be used:
```ini
stylecop.naming.allowedNamespaceComponents = eBay, iPod
```

### Tuple element names

The following properties are used in **stylecop.json** to configure the behavior of the tuple element name analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `includeInferredTupleElementNames` | false | 1.2.0 | Specifies whether inferred tuple element names will be analyzed as well. Explicit element names, including those in tuple expressions, are always analyzed. |
| `tupleElementNameCasing` | "PascalCase" | 1.2.0 | Specifies the casing convention used for tuple element names. |

The following example shows a settings file which requires tuple element names to use camel case for all tuple elements (including inferred element names).

```json
{
  "settings": {
    "namingRules": {
      "includeInferredTupleElementNames": true,
      "tupleElementNameCasing" : "camelCase"
    }
  }
}
```

When using an **.editorconfig** file to configure StyleCop Analyzers, the following properties can be used:
```ini
stylecop.naming.includeInferredTupleElementNames = true
stylecop.naming.tupleElementNameCasing = camelCase
```

#### Tuple Element Name Casing
The `tupleElementNameCasing` property affects the behavior of the [SA1316 Tuple element names should use correct casing](SA1316.md) analyzer.

This property has two allowed values, which are described as follows.

##### `"camelCase"`
In this mode, tuple element names must start with a lowercase letter.

##### `"PascalCase"`
In this mode, tuple element names must start with an uppercase letter.


## Maintainability Rules

This section describes the features of maintainability rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties are configured in the `maintainabilityRules` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "maintainabilityRules": {
    }
  }
}
```

The following properties are used in **stylecop.json** to configure maintainability rules in StyleCop Analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `topLevelTypes` | `[ "class" ]` | 1.1.0 | Specifies which kind of types that should be placed in separate files |

The `topLevelTypes` property is an array which specifies which kind of types that should be placed in separate files
according to rule SA1402. The following types are supported:
* `class`
* `interface`
* `struct`
* `enum`
* `delegate`

> :bulb: This property can currently not be set in an **.editorconfig** file.

## Layout Rules

This section describes the features of layout rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties are configured in the `layoutRules` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "layoutRules": {
    }
  }
}
```

The following properties are used in **stylecop.json** to configure layout rules in StyleCop Analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `newlineAtEndOfFile` | `"allow"` | 1.0.0 | Specifies the handling for newline characters which appear at the end of a file |
| `allowConsecutiveUsings` | `true` | 1.1.0 | Specifies if SA1503 and SA1519 will allow consecutive using statements without braces |
| `allowDoWhileOnClosingBrace` | `false` | >1.2.0 | Specifies if SA1500 will allow the `while` expression of a `do`/`while` loop to be on the same line as the closing brace, as is generated by the default code snippet of Visual Studio |

When using an **.editorconfig** file to configure StyleCop Analyzers, the newline setting (`insert_final_newline`) as described at editorconfig.org can be used, and the following additional properties:
```ini
stylecop.layout.allowConsecutiveUsings = true
stylecop.layout.allowDoWhileOnClosingBrace = false
```

### Lines at End of File

The behavior of [SA1518](SA1518.md) can be customized regarding the manner in which newline characters at the end of a
file are handled. The `newlineAtEndOfFile` property supports the following values:

* `"allow"`: Files are allowed to end with a single newline character, but it is not required
* `"require"`: Files are required to end with a single newline character
* `"omit"`: Files may not end with a newline character

### Consecutive using statements without braces

The behavior of [SA1503](SA1503.md) and [SA1519](SA1519.md) can be customized regarding the manner in which consecutive using statements without braces are treated.
The `allowConsecutiveUsings` property specifies the behavior:

* `true`: consecutive using statements without braces will not produce diagnostics
* `false`: consecutive using statements without braces will produce a SA1503 or SA1519 diagnostic

This only allows omitting the braces for a using followed by another using statement. A using statement followed by any other type of statement will still
require braces to used.

### Do-While Loop Placement

The behavior of [SA1500](SA1500.md) can be customized regarding the manner in which the `while` expression of a `do`/`while` loop is allowed to be placed. The `allowDoWhileOnClosingBrace` property specified the behavior:

* `true`: the `while` expression of a `do`/`while` loop may be placed on the same line as the closing brace or on a separate line
* `false`: the `while` expression of a `do`/`while` loop must be on a separate line from the closing brace

## Documentation Rules

This section describes the features of documentation rules which can be configured in **stylecop.json** or **.editorconfig**. Each of the described properties are configured in the `documentationRules` object of the **stylecop.json** file, which is shown in the following sample.

```json
{
  "settings": {
    "documentationRules": {
    }
  }
}
```

### Copyright Headers

The following properties are used in **stylecop.json** to configure copyright headers in StyleCop Analyzers.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `companyName` | `"PlaceholderCompany"` | 1.0.0 | Specifies the company name which should appear in copyright notices |
| `copyrightText` | `"Copyright (c) {companyName}. All rights reserved."` | 1.0.0 | Specifies the default copyright text which should appear in copyright headers |
| `xmlHeader` | **true** | 1.0.0 | Specifies whether file headers should use standard StyleCop XML format, where the copyright notice is wrapped in a `<copyright>` element |
| `variables` | n/a | 1.0.0 | Specifies replacement variables which can be referenced in the `copyrightText` value |
| `headerDecoration` | n/a | 1.1.0 | This value can be set to add a decoration for the header comment so headers look similar to the ones generated by the StyleCop Classic ReSharper fix |

When using an **.editorconfig** file to configure StyleCop Analyzers, the following properties can be used:
```ini
stylecop.documentation.companyName = PlaceholderCompany
stylecop.documentation.copyrightText = Copyright (c) {companyName}. All rights reserved.
stylecop.documentation.xmlHeader = true
stylecop.documentation.headerDecoration = -----------------
```

> :memo: Instead of `stylecop.documentation.copyrightText` the `file_header_template` property as described in [IDE0073 (Require file header)](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0073) can be used. However the StyleCop specific property will take precedence.

> :bulb: The `variables` property can currently not be set in an **.editorconfig** file.

#### Configuring Copyright Text

In order to successfully use StyleCop-checked file headers, most projects will need to configure the `companyName`
property.

> The `companyName` property is so frequently customized that it is included in the default **stylecop.json** file
> produced by the code fix.

The `copyrightText` property is a string which may contain placeholders. Each placeholder has the form `{variable}`,
where `variable` is either a built-in variable (see below), or the name of a property in the `variables` property. The
following sample file shows a custom **stylecop.json** file which references both `companyName` and two custom variables
within the `copyrightText`.

```json
{
  "settings": {
    "documentationRules": {
      "companyName": "FooCorp",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.\nLicensed under the {licenseName} license. See {licenseFile} file in the project root for full license information.",
      "variables": {
        "licenseName": "MIT",
        "licenseFile": "LICENSE"
      }
    }
  }
}
```

With the above configuration, a file **TypeName.cs** would be expected to have the following header.

```csharp
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
```

##### Built-In Variables

| Variable | Meaning |
| --- | --- |
| `companyName` | The value of the `companyName` configuration property in **stylecop.json** |
| `fileName` | The file name of the current source file |

> :memo: If a `fileName` variable is explicitly included within the `variables` property of **stylecop.json**, that
> value will be used instead of the name of the current source file.

#### Configuring XML Headers

When the `xmlHeader` property is **true** (the default), StyleCop Analyzers expects file headers to conform to the following standard StyleCop format.

```csharp
// <copyright file="{fileName}" company="{companyName}">
// {copyrightText}
// </copyright>
```

When the `xmlHeader` property is explicitly set to **false**, StyleCop Analyzers expects file headers to conform to the following customizable format.

```csharp
// {copyrightText}
```

#### Configuring Copyright Text Header Decoration

The `headerDecoration` property is a string which can contain text that's used for decorating the generated header so
headers look similar to the ones generated by the StyleCop Classic ReSharper fix.

The default value for the `headerDecoration` property is empty, so no decoration will be added.

> :memo: The header decoration is not checked, it's only used for fixing the header.

```json
{
  "settings": {
    "documentationRules": {
      "companyName": "FooCorp",
      "copyrightText": "Copyright (c) {companyName}. All rights reserved.",
      "headerDecoration": "-----------------------------------------------------------------------"
    }
  }
}
```

With the above configuration, the fix for a file **TypeName.cs** would look like the following.

```csharp
// -----------------------------------------------------------------------
// <copyright file="TypeName.cs" company="FooCorp">
// Copyright (c) FooCorp. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
```

### Documentation Requirements

StyleCop Analyzers includes rules which require developers to document the majority of a code base by default. This requirement can easily overwhelm a team which did not use StyleCop for the entire development process. To help guide developers towards a properly documented code base, several properties are available in **stylecop.json** to progressively increase the documentation requirements.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `documentInterfaces` | **true** | 1.0.0 | Specifies whether interface members need to be documented. When true, all interface members require documentation, regardless of accessibility. |
| `documentExposedElements` | **true** | 1.0.0 | Specifies whether exposed elements need to be documented. When true, all publicly-exposed types and members require documentation. |
| `documentInternalElements` | **true** | 1.0.0 | Specifies whether internal elements need to be documented. When true, all internally-exposed types and members require documentation. |
| `documentPrivateElements` | **false** | 1.0.0 | Specifies whether private elements need to be documented. When true, all types and members except for declared private fields require documentation. |
| `documentPrivateFields` | **false** | 1.0.0 | Specifies whether private fields need to be documented. When true, all fields require documentation, regardless of accessibility. |

These properties affect the behavior of the following rules which report missing documentation:

* [SA1600 Elements should be documented](SA1600.md)
* [SA1601 Partial elements should be documented](SA1601.md)
* [SA1602 Enumeration items should be documented](SA1602.md)

The rules which report missing parts of a documentation comment only check elements which require documentation under these properties: [SA1604](SA1604.md), [SA1605](SA1605.md), [SA1609](SA1609.md), [SA1611](SA1611.md), [SA1615](SA1615.md), [SA1618](SA1618.md) and [SA1619](SA1619.md). For other elements, [SA1612](SA1612.md) allows `param` elements to be left out, but checks the ones which are present. The other documentation rules, which report incorrect documentation (for example [SA1623](SA1623.md) or [SA1629](SA1629.md)), continue to apply to all documentation comments in the code.

The following example shows a configuration file which requires developers to document all publicly-accessible members and all interfaces (regardless of accessibility), but does not require other internal or private members to be documented.

> :memo: Documenting interfaces is a low-effort task compared to documenting an entire code base, but provides high value in the fact that it covers the sections of code most likely to impact cross-team usage scenarios.


```json
{
  "settings": {
    "documentationRules": {
      "documentInterfaces": true,
      "documentInternalElements": false
    }
  }
}
```

When using an **.editorconfig** file to configure StyleCop Analyzers, the following properties can be used:
```ini
stylecop.documentation.documentInterfaces = true
stylecop.documentation.documentExposedElements = true
stylecop.documentation.documentInternalElements = true
stylecop.documentation.documentPrivateElements = false
stylecop.documentation.documentPrivateFields = false
```

### Documentation Culture

Some documentation rules require summary texts to start with specific strings. To allow teams to document their code in their native language, **stylecop.json** contains the `documentationCulture` property.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `documentationCulture` | `"en-US"` |  1.1.0 | Specifies the culture or language to be used for certain documentation texts. |

This property affects the behavior of the following rules which report incorrect documentation.

* [SA1623 Property summary documentation should match accessors](SA1623.md)
* [SA1624 Property summary documentation should omit set accessor with restricted access](SA1624.md)
* [SA1642 Constructor summary documentation should begin with standard text](SA1642.md)
* [SA1643 Destructor summary documentation should begin with standard text](SA1643.md)

> :memo: The default value for `documentationCulture` is fixed instead of reflecting the user's system language. This is to ensure that different developers working on the same project always use the same value.

The following values are currently supported. Unsupported values will automatically fall back to the default value.

* `"cs-CZ"`
* `"de-DE"`
* `"en-GB"`
* `"en-US"`
* `"es-MX"`
* `"fr-FR"`
* `"nl-NL"`
* `"pl-PL"`
* `"pt-BR"`
* `"ru-RU"`

```json
{
  "settings": {
    "documentationRules": {
      "documentationCulture": "de-DE"
    }
  }
}
```

When using an **.editorconfig** file to configure StyleCop Analyzers, the following property can be used:
```ini
stylecop.documentation.documentationCulture = de-DE
```

### File naming conventions

The `fileNamingConvention` property in **stylecop.json** will determine how the [SA1649 File name should match type name](SA1649.md) analyzer will check file names.

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `fileNamingConvention` | `"stylecop"` |  1.0.0 | Specifies the convention for file names of generics. |

Given the following code:

```csharp
public class Class1<T1, T2, T3>
{
}
```

The analyzer will expect file names according the table below. When the `fileNamingConvention` property is not set, the `stylecop` convention is used as default.

File naming convention | Expected file name
-----------------------| ------------------
stylecop               | Class1{T1,T2,T3}.cs
metadata               | Class1`3.cs

When using an **.editorconfig** file to configure StyleCop Analyzers, the following property can be used:
```ini
stylecop.documentation.fileNamingConvention = stylecop
```

### Text ending with a period

The [SA1629 Documentation Text Must End With A Period](SA1629.md) analyzer checks if sections within XML documentation end with a period. The following properties can be used in **stylecop.json** to control the behavior of the analyzer:

| Property | Default Value | Minimum Version | Summary |
| --- | --- | --- | --- |
| `excludeFromPunctuationCheck` | `[ "seealso" ]` |  1.1.0 | Specifies the top-level tags within XML documentation that will be excluded from analysis. |

When using an **.editorconfig** file to configure StyleCop Analyzers, the following property can be used:
```ini
stylecop.documentation.excludeFromPunctuationCheck = seealso
```

## Settings in .editorconfig

Some of the settings described above can be provided in an [**.editorconfig**](http://editorconfig.org/) file instead of
(or in addition to) **stylecop.json**. The analyzers read the keys in the following table.

* A value in **stylecop.json** takes precedence over the **.editorconfig** value of the same setting. The
  **.editorconfig** value is used only when **stylecop.json** does not set the property (or no **stylecop.json** exists).
* A value of `unset`, or a value which cannot be parsed, is ignored and the default value is used.
* **.editorconfig** keys are evaluated per source file, so different folders can use different values. **stylecop.json**
  applies to the whole project.
* A severity suffix (for example `outside_namespace:error`) is accepted and ignored.
* Settings which are not listed here, such as `elementOrder`, can only be set in **stylecop.json**.

In the second column, `indentation.tabSize` means the `tabSize` property of the `indentation` object inside `settings`.
Values in parentheses are the equivalent **stylecop.json** values.

| .editorconfig key | stylecop.json setting | Values |
| --- | --- | --- |
| `indent_size` | `indentation.indentationSize` | Integer |
| `tab_width` | `indentation.tabSize` | Integer |
| `indent_style` | `indentation.useTabs` | `tab` (true), `space` (false) |
| `csharp_indent_block_contents` | `indentation.indentBlock` | `true`, `false` |
| `csharp_indent_switch_labels` | `indentation.indentSwitchSection` | `true`, `false` |
| `csharp_indent_case_contents` | `indentation.indentSwitchCaseSection` | `true`, `false` |
| `csharp_indent_labels` | `indentation.labelPositioning` | `one_less_than_current` (`oneLess`), `flush_left` (`leftMost`), `no_change` (`noIndent`) |
| `insert_final_newline` | `layoutRules.newlineAtEndOfFile` | `true` (`require`), `false` (`omit`) |
| `dotnet_sort_system_directives_first` | `orderingRules.systemUsingDirectivesFirst` | `true`, `false` |
| `csharp_using_directive_placement` | `orderingRules.usingDirectivesPlacement` | `inside_namespace` (`insideNamespace`), `outside_namespace` (`outsideNamespace`) |
| `dotnet_separate_import_directive_groups` | `orderingRules.blankLinesBetweenUsingGroups` | `true` (`require`), `false` (`allow`) |
| `file_header_template` | `documentationRules.copyrightText` | Text; used only when `stylecop.documentation.copyrightText` is not set |
| `stylecop.readability.allowBuiltInTypeAliases` | `readabilityRules.allowBuiltInTypeAliases` | Boolean |
| `stylecop.layout.allowConsecutiveUsings` | `layoutRules.allowConsecutiveUsings` | Boolean |
| `stylecop.layout.allowDoWhileOnClosingBrace` | `layoutRules.allowDoWhileOnClosingBrace` | Boolean |
| `stylecop.naming.allowCommonHungarianPrefixes` | `namingRules.allowCommonHungarianPrefixes` | Boolean |
| `stylecop.naming.allowedHungarianPrefixes` | `namingRules.allowedHungarianPrefixes` | Comma-separated list |
| `stylecop.naming.allowedNamespaceComponents` | `namingRules.allowedNamespaceComponents` | Comma-separated list |
| `stylecop.naming.includeInferredTupleElementNames` | `namingRules.includeInferredTupleElementNames` | Boolean |
| `stylecop.naming.tupleElementNameCasing` | `namingRules.tupleElementNameCasing` | `camelCase`, `pascalCase` |
| `stylecop.documentation.documentExposedElements` | `documentationRules.documentExposedElements` | Boolean |
| `stylecop.documentation.documentInternalElements` | `documentationRules.documentInternalElements` | Boolean |
| `stylecop.documentation.documentPrivateElements` | `documentationRules.documentPrivateElements` | Boolean |
| `stylecop.documentation.documentInterfaces` | `documentationRules.documentInterfaces` | `all`, `exposed`, `none`, or boolean |
| `stylecop.documentation.documentPrivateFields` | `documentationRules.documentPrivateFields` | Boolean |
| `stylecop.documentation.companyName` | `documentationRules.companyName` | Text |
| `stylecop.documentation.copyrightText` | `documentationRules.copyrightText` | Text; use `\n` for line breaks |
| `stylecop.documentation.headerDecoration` | `documentationRules.headerDecoration` | Text |
| `stylecop.documentation.xmlHeader` | `documentationRules.xmlHeader` | Boolean |
| `stylecop.documentation.fileNamingConvention` | `documentationRules.fileNamingConvention` | `stylecop`, `metadata` |
| `stylecop.documentation.documentationCulture` | `documentationRules.documentationCulture` | Culture name |
| `stylecop.documentation.excludeFromPunctuationCheck` | `documentationRules.excludeFromPunctuationCheck` | Comma-separated list |

```ini
[*.cs]
indent_style = space
indent_size = 4
insert_final_newline = true
csharp_using_directive_placement = outside_namespace
stylecop.documentation.companyName = Contoso
```

Rule severities are not part of **stylecop.json**; see [ConfiguringRules.md](ConfiguringRules.md).

## Sharing configuration among solutions

It is possible to define your preferred configuration once and reuse it across multiple independent projects. This involves rolling out your own NuGet package,
which will contain the `stylecop.json` configuration and potentially a custom ruleset file. A custom `.props` file glues that configuration to any project
that will use the NuGet package.

Example `.nuspec` file:

```xml
<?xml version="1.0"?>
<package>
  <metadata>
    <id>acme.stylecop</id>
    <version>1.0.0</version>
    <dependencies>
      <dependency id="StyleCop.Analyzers" version="1.0.2" />
    </dependencies>
  </metadata>
  <files>
    <file src="stylecop.json" target="" />
    <file src="acme.stylecop.ruleset" target="" />
    <file src="acme.stylecop.props" target="build" />
  </files>
</package>
```

Example `.props` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="14.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
      <CodeAnalysisRuleSet>$(MSBuildThisFileDirectory)..\acme.stylecop.ruleset</CodeAnalysisRuleSet>
  </PropertyGroup>
  <ItemGroup>
    <AdditionalFiles Include="$(MSBuildThisFileDirectory)..\stylecop.json" Link="stylecop.json" />
  </ItemGroup>
</Project>
```
