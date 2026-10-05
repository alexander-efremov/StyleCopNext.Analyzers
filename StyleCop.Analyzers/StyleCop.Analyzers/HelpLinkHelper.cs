// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers;

internal static class HelpLinkHelper
{
    internal static string GetHelpLink(string diagnosticId) => $"https://github.com/alexander-efremov/StyleCopNext.Analyzers/blob/release/1.x/documentation/{diagnosticId}.md";
}
