// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Test.MaintainabilityRules
{
    using System.Collections.Immutable;
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
    }
}
