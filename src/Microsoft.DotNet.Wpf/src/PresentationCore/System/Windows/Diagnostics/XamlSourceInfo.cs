// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
//
// Description:
//      Defines XAML source info.
//

namespace System.Windows.Diagnostics
{
    [DebuggerDisplay("{line={LineNumber}, offset={LinePosition}, uri={SourceUri}}")]
    public class XamlSourceInfo
    {
        /// <summary>
        /// Source URI.
        /// </summary>
        public Uri SourceUri { get; private set; }

        /// <summary>
        /// Line within source file.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Position within line.
        /// </summary>
        public int LinePosition { get; private set; }

        public XamlSourceInfo(Uri sourceUri, int lineNumber, int linePosition)
        {
            SourceUri = sourceUri;
            LineNumber = lineNumber;
            LinePosition = linePosition;
        }
    }
}
