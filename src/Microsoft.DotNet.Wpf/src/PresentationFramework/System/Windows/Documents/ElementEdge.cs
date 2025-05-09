// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
// Identifies the edge of an object where a TextPointer is located
//

namespace System.Windows.Documents
{
    /// <summary>
    ///  This identifies the edge of an object where a TextPointer is located
    /// </summary>
    [Flags]
    internal enum ElementEdge : byte
    {
        /// <summary>
        ///   Located before the beginning of a DependencyObject
        /// </summary>
        BeforeStart = 1,
        /// <summary>
        ///   Located after the beginning of a DependencyObject
        /// </summary>
        AfterStart = 2,
        /// <summary>
        ///   Located before the end of a DependencyObject
        /// </summary>
        BeforeEnd = 4,
        /// <summary>
        ///   Located after the end of a DependencyObject
        /// </summary>
        AfterEnd = 8
    }
}
