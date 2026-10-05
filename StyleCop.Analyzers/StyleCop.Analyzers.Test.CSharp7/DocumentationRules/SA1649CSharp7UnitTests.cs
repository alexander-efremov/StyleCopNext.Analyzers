// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.CSharp7.DocumentationRules
{
    using System.Linq;
    using System.Threading.Tasks;
    using StyleCop.Analyzers.Test.DocumentationRules;
    using Xunit;

    public partial class SA1649CSharp7UnitTests : SA1649UnitTests
    {
        /// <summary>
        /// Verifies that the code fix renames the document in place (same <see cref="Microsoft.CodeAnalysis.DocumentId"/>)
        /// instead of removing it and adding a new one, so that source control can track the rename. This requires a
        /// Roslyn version that provides <c>Solution.WithDocumentName</c>, which these test projects reference.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task VerifyCodeFixRenamesDocumentInPlaceAsync()
        {
            var (originalId, _, fixedSolution) = await ApplyCodeFixToMisnamedDocumentAsync().ConfigureAwait(false);

            var renamed = fixedSolution.Projects.Single().Documents.Single();
            Assert.Equal(originalId, renamed.Id);
        }
    }
}
