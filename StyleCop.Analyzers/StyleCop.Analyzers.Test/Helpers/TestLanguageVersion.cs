// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Microsoft.CodeAnalysis.CSharp;
    using StyleCop.Analyzers.Lightup;

    /// <summary>
    /// Provides the C# language version targeted by the test project which is currently running. Each test project
    /// declares its version with an <see cref="AssemblyMetadataAttribute"/> named <c>TestLanguageVersion</c>; when
    /// a test project references the test projects of earlier language versions, the highest declared version wins.
    /// </summary>
    internal static class TestLanguageVersion
    {
        private const string MetadataKey = "TestLanguageVersion";

        public static LanguageVersion Current { get; } = GetCurrent();

        /// <summary>
        /// Gets the language version that tests use when they do not specify one.
        /// </summary>
        /// <value>The language version of the test project which is currently running.</value>
        public static LanguageVersion? Default => Current;

        public static bool SupportsCSharp7 => Current >= LanguageVersion.CSharp7;

        public static bool SupportsCSharp72 => Current >= LanguageVersion.CSharp7_2;

        public static bool SupportsCSharp8 => Current >= LanguageVersion.CSharp8;

        public static bool SupportsCSharp9 => Current >= LanguageVersion.CSharp9;

        public static bool SupportsCSharp10 => Current >= LanguageVersion.CSharp10;

        public static bool SupportsCSharp11 => Current >= LanguageVersion.CSharp11;

        public static bool SupportsCSharp12 => Current >= LanguageVersion.CSharp12;

        public static bool SupportsCSharp13 => Current >= LanguageVersionEx.CSharp13;

        public static bool SupportsCSharp14 => Current >= LanguageVersionEx.CSharp14;

        private static LanguageVersion GetCurrent()
        {
            var result = LanguageVersion.Default;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

                foreach (var metadata in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
                {
                    if (metadata.Key == MetadataKey
                        && int.TryParse(metadata.Value, NumberStyles.None, CultureInfo.InvariantCulture, out var value)
                        && (LanguageVersion)value > result)
                    {
                        result = (LanguageVersion)value;
                    }
                }
            }

            return result;
        }
    }
}
