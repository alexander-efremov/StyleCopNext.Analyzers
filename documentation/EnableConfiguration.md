# Enabling **stylecop.json**

The analyzers use **stylecop.json** (or **.stylecop.json**) only when it is part of the project as an additional file.
The **Add StyleCop settings file to the project** code fix creates the file, but it may not register it with the project,
and the package does not add it automatically. After creating the file, make sure it is included in one of the
following ways.

* In the project file (or a shared **Directory.Build.props**), which works for SDK-style projects in every IDE and on the
  command line:

  ```xml
  <ItemGroup>
    <AdditionalFiles Include="stylecop.json" />
  </ItemGroup>
  ```

* In Visual Studio, select the file in **Solution Explorer** and, in the **Properties** window, set **Build Action** to
  **C# analyzer additional file**.

Many settings can also be provided in **.editorconfig** without a **stylecop.json** file, see
[Settings in .editorconfig](Configuration.md#settings-in-editorconfig).

## Next steps

Additional information about the content of **stylecop.json** is available in [Configuration.md](Configuration.md).
Information about rule severities, running the analyzers only in the IDE, and excluding files is available in
[ConfiguringRules.md](ConfiguringRules.md).
