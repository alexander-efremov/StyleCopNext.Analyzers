// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Immutable;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using StyleCop.Analyzers.MaintainabilityRules;
    using Xunit;

    public class SA1402CodeFixProviderUnitTests
    {
        /// <summary>
        /// Verifies that no code fix is offered for a document that is shared with another project (Shared Project or
        /// linked file), because the fix cannot add the new file next to the shared file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyNoCodeFixForDocumentWithLinkedDocumentsAsync()
        {
            const string source = "namespace TestNamespace\r\n{\r\n    public class TestType1\r\n    {\r\n    }\r\n\r\n    public class TestType2\r\n    {\r\n    }\r\n}\r\n";
            const string filePath = "C:\\Shared\\TestType1.cs";

            using (var workspace = new AdhocWorkspace())
            {
                var projectA = workspace.AddProject("ProjectA", LanguageNames.CSharp);
                var documentA = workspace.AddDocument(DocumentInfo.Create(DocumentId.CreateNewId(projectA.Id), "TestType1.cs", filePath: filePath, loader: TextLoader.From(TextAndVersion.Create(SourceText.From(source), VersionStamp.Create()))));
                var projectB = workspace.AddProject("ProjectB", LanguageNames.CSharp);
                var documentB = workspace.AddDocument(DocumentInfo.Create(DocumentId.CreateNewId(projectB.Id), "TestType1.cs", filePath: filePath, loader: TextLoader.From(TextAndVersion.Create(SourceText.From(source), VersionStamp.Create()))));

                documentA = workspace.CurrentSolution.GetDocument(documentA.Id);
                Assert.Equal(documentB.Id, Assert.Single(documentA.GetLinkedDocumentIds()));

                var compilation = await documentA.Project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
                var diagnostics = await compilation
                    .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new SA1402FileMayOnlyContainASingleType()))
                    .GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                    .ConfigureAwait(false);
                var diagnostic = Assert.Single(diagnostics, d => d.Id == SA1402FileMayOnlyContainASingleType.DiagnosticId && d.Location.SourceTree == documentA.GetSyntaxTreeAsync().Result);

                var actions = ImmutableArray.CreateBuilder<CodeAction>();
                var context = new CodeFixContext(documentA, diagnostic, (action, ignored) => actions.Add(action), CancellationToken.None);
                await new SA1402CodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);

                Assert.Empty(actions);
            }
        }

        /// <summary>
        /// Verifies that the code fix is offered for a document of a multi-targeted project, where the linked documents
        /// belong to other target framework projects of the same project file, and that the new file is added only to the
        /// current project with a path next to the original file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCodeFixOfferedForMultiTargetedProjectAsync()
        {
            const string source = "namespace TestNamespace\r\n{\r\n    public class TestType1\r\n    {\r\n    }\r\n\r\n    public class TestType2\r\n    {\r\n    }\r\n}\r\n";
            const string filePath = "C:\\Project\\TestType1.cs";
            const string projectPath = "C:\\Project\\Project.csproj";

            using (var workspace = new AdhocWorkspace())
            {
                var projectA = workspace.AddProject(ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "Project(net8.0)", "Project", LanguageNames.CSharp, filePath: projectPath));
                var documentA = workspace.AddDocument(DocumentInfo.Create(DocumentId.CreateNewId(projectA.Id), "TestType1.cs", filePath: filePath, loader: TextLoader.From(TextAndVersion.Create(SourceText.From(source), VersionStamp.Create()))));
                var projectB = workspace.AddProject(ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Create(), "Project(net10.0)", "Project", LanguageNames.CSharp, filePath: projectPath));
                var documentB = workspace.AddDocument(DocumentInfo.Create(DocumentId.CreateNewId(projectB.Id), "TestType1.cs", filePath: filePath, loader: TextLoader.From(TextAndVersion.Create(SourceText.From(source), VersionStamp.Create()))));

                documentA = workspace.CurrentSolution.GetDocument(documentA.Id);
                Assert.Equal(documentB.Id, Assert.Single(documentA.GetLinkedDocumentIds()));

                var compilation = await documentA.Project.GetCompilationAsync(CancellationToken.None).ConfigureAwait(false);
                var diagnostics = await compilation
                    .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new SA1402FileMayOnlyContainASingleType()))
                    .GetAnalyzerDiagnosticsAsync(CancellationToken.None)
                    .ConfigureAwait(false);
                var diagnostic = Assert.Single(diagnostics, d => d.Id == SA1402FileMayOnlyContainASingleType.DiagnosticId && d.Location.SourceTree == documentA.GetSyntaxTreeAsync().Result);

                var actions = ImmutableArray.CreateBuilder<CodeAction>();
                var context = new CodeFixContext(documentA, diagnostic, (action, ignored) => actions.Add(action), CancellationToken.None);
                await new SA1402CodeFixProvider().RegisterCodeFixesAsync(context).ConfigureAwait(false);

                var action = Assert.Single(actions);
                var operations = await action.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
                var applyOperation = Assert.IsType<ApplyChangesOperation>(Assert.Single(operations));
                var fixedSolution = applyOperation.ChangedSolution;

                var fixedProjectA = fixedSolution.GetProject(projectA.Id);
                var newDocument = Assert.Single(fixedProjectA.Documents, d => d.Id != documentA.Id);
                Assert.Equal("TestType2.cs", newDocument.Name);
                Assert.Equal(Path.Combine(Path.GetDirectoryName(filePath), "TestType2.cs"), newDocument.FilePath);
                Assert.Single(fixedSolution.GetProject(projectB.Id).Documents);
            }
        }
    }
}
