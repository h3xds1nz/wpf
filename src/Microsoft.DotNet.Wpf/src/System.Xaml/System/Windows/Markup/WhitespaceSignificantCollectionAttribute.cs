// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Windows.Markup
{
    /// <summary>
    /// An attribute that specifies that a collection considers whitespacing to be
    /// significant.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    [TypeForwardedFrom("WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
    public sealed class WhitespaceSignificantCollectionAttribute : Attribute
    {
        /// <summary>
        /// Creates a new content property attriubte that indicates that the associated
        /// class does consider whitespace to be signifant.
        /// </summary>
        public WhitespaceSignificantCollectionAttribute()
        {
        }
    }
}
