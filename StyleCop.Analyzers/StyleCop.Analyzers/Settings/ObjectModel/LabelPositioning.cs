// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    /// <summary>
    /// Specifies the possible positions of labels targeted by <c>goto</c> statements.
    /// </summary>
    internal enum LabelPositioning
    {
        /// <summary>
        /// Labels are placed in the first column.
        /// </summary>
        LeftMost,

        /// <summary>
        /// Labels are indented one level less than the statements of the enclosing block.
        /// </summary>
        OneLess,

        /// <summary>
        /// Labels are indented the same as the statements of the enclosing block.
        /// </summary>
        NoIndent,
    }
}
