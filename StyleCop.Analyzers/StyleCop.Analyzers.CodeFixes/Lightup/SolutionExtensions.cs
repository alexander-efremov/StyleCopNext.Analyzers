// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Lightup
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CodeAnalysis;

    /// <summary>
    /// Provides light-up access to <c>Solution.WithDocumentName</c>, which is not available in Roslyn 1.x (it was
    /// introduced in Roslyn 2.x).
    /// </summary>
    internal static class SolutionExtensions
    {
        private static readonly Func<Solution, DocumentId, string, Solution> WithDocumentNameAccessor = CreateWithDocumentNameAccessor();

        /// <summary>
        /// Renames a document in place if the host Roslyn version supports it.
        /// </summary>
        /// <param name="solution">The solution containing the document.</param>
        /// <param name="documentId">The identifier of the document to rename.</param>
        /// <param name="name">The new name of the document.</param>
        /// <param name="newSolution">The solution with the renamed document, or <see langword="null"/> if the host
        /// does not support renaming a document in place.</param>
        /// <returns><see langword="true"/> if the document was renamed in place; otherwise, <see langword="false"/>.</returns>
        public static bool TryWithDocumentName(this Solution solution, DocumentId documentId, string name, out Solution newSolution)
        {
            if (WithDocumentNameAccessor == null)
            {
                newSolution = null;
                return false;
            }

            newSolution = WithDocumentNameAccessor(solution, documentId, name);
            return true;
        }

        private static Func<Solution, DocumentId, string, Solution> CreateWithDocumentNameAccessor()
        {
            MethodInfo method = null;
            foreach (var candidate in typeof(Solution).GetTypeInfo().GetDeclaredMethods("WithDocumentName"))
            {
                var parameters = candidate.GetParameters();
                if (!candidate.IsStatic
                    && parameters.Length == 2
                    && Equals(typeof(DocumentId), parameters[0].ParameterType)
                    && Equals(typeof(string), parameters[1].ParameterType)
                    && Equals(typeof(Solution), candidate.ReturnType))
                {
                    method = candidate;
                    break;
                }
            }

            if (method == null)
            {
                return null;
            }

            var solutionParameter = Expression.Parameter(typeof(Solution), "solution");
            var documentIdParameter = Expression.Parameter(typeof(DocumentId), "documentId");
            var nameParameter = Expression.Parameter(typeof(string), "name");
            var call = Expression.Call(solutionParameter, method, documentIdParameter, nameParameter);
            return Expression.Lambda<Func<Solution, DocumentId, string, Solution>>(call, solutionParameter, documentIdParameter, nameParameter).Compile();
        }
    }
}
