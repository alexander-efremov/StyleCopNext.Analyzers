// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Test.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using StyleCop.Analyzers.Lightup;

    public static class CommonMemberData
    {
        public static IEnumerable<object[]> DataTypeDeclarationKeywords
        {
            get
            {
                yield return new[] { "class" };
                yield return new[] { "struct" };

                if (TestLanguageVersion.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (TestLanguageVersion.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                    yield return new[] { "record struct" };
                }
            }
        }

        public static IEnumerable<object[]> ReferenceTypeDeclarationKeywords
        {
            get
            {
                yield return new[] { "class" };

                if (TestLanguageVersion.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (TestLanguageVersion.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                }
            }
        }

        public static IEnumerable<object[]> ValueTypeDeclarationKeywords
        {
            get
            {
                yield return new[] { "struct" };

                if (TestLanguageVersion.SupportsCSharp10)
                {
                    yield return new[] { "record struct" };
                }
            }
        }

        public static IEnumerable<object[]> RecordTypeDeclarationKeywords
        {
            get
            {
                if (TestLanguageVersion.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (TestLanguageVersion.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                    yield return new[] { "record struct" };
                }
            }
        }

        public static IEnumerable<object[]> TypeDeclarationKeywords
        {
            get
            {
                return DataTypeDeclarationKeywords
                    .Concat(new[] { new[] { "interface" } });
            }
        }

        public static IEnumerable<object[]> BaseTypeDeclarationKeywords
        {
            get
            {
                return TypeDeclarationKeywords
                    .Concat(new[] { new[] { "enum" } });
            }
        }

        public static IEnumerable<object[]> AllTypeDeclarationKeywords
        {
            get
            {
                return BaseTypeDeclarationKeywords
                    .Concat(new[] { new[] { "delegate" } });
            }
        }

        public static IEnumerable<object[]> GenericTypeDeclarationKeywords
        {
            get
            {
                return TypeDeclarationKeywords
                    .Concat(new[] { new[] { "delegate" } });
            }
        }

        public static IEnumerable<object[]> ReferenceTypeKeywordsWhichSupportPrimaryConstructors
        {
            get
            {
                if (TestLanguageVersion.SupportsCSharp9)
                {
                    yield return new[] { "record" };
                }

                if (TestLanguageVersion.SupportsCSharp10)
                {
                    yield return new[] { "record class" };
                }

                if (TestLanguageVersion.SupportsCSharp12)
                {
                    yield return new[] { "class" };
                }
            }
        }

        public static IEnumerable<object[]> TypeKeywordsWhichSupportPrimaryConstructors
        {
            get
            {
                foreach (var keyword in ReferenceTypeKeywordsWhichSupportPrimaryConstructors)
                {
                    yield return keyword;
                }

                if (TestLanguageVersion.SupportsCSharp10)
                {
                    yield return new[] { "record struct" };
                }

                if (TestLanguageVersion.SupportsCSharp12)
                {
                    yield return new[] { "struct" };
                }
            }
        }
    }
}
