// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Markup
{
    [Obsolete("IReceiveMarkupExtension has been deprecated. This interface is no longer in use.")]
    public interface IReceiveMarkupExtension
    {
        void ReceiveMarkupExtension(String property, MarkupExtension markupExtension, IServiceProvider serviceProvider);
    }   
}
