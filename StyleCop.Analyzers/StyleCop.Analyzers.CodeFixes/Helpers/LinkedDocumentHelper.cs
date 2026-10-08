// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class LinkedDocumentHelper
    {
        /// <summary>
        /// Determines whether a document is shared with a different project (a Shared Project item or a file linked
        /// into several projects). Linked documents that belong to the same project file are not shared: Visual Studio
        /// loads a multi-targeted project as one Roslyn project per target framework, and every file of such a project
        /// is linked across those projects.
        /// </summary>
        /// <param name="document">The document to inspect.</param>
        /// <returns><see langword="true"/> if a linked document belongs to a project with a different project file
        /// (or if that cannot be determined); otherwise, <see langword="false"/>.</returns>
        public static bool IsSharedWithOtherProjects(Document document)
        {
            var solution = document.Project.Solution;
            var projectFilePath = document.Project.FilePath;

            foreach (var linkedDocumentId in document.GetLinkedDocumentIds())
            {
                var linkedProjectFilePath = solution.GetProject(linkedDocumentId.ProjectId)?.FilePath;
                if (projectFilePath == null
                    || linkedProjectFilePath == null
                    || !string.Equals(projectFilePath, linkedProjectFilePath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
