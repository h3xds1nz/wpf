// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
//

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Markup
{
    /// <summary>
    ///     TypeConverter for a resource value expression
    /// </summary>
    public class ComponentResourceKeyConverter : ExpressionConverter
    {
        /// <summary>
        ///     TypeConverter method override.
        /// </summary>
        /// <param name="context">
        ///     ITypeDescriptorContext
        /// </param>
        /// <param name="sourceType">
        ///     Type to convert from
        /// </param>
        /// <returns>
        ///     true if conversion is possible
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            ArgumentNullException.ThrowIfNull(sourceType);

            return base.CanConvertFrom(context, sourceType);
        }
    
        /// <summary>
        ///     TypeConverter method override.
        /// </summary>
        /// <param name="context">
        ///     ITypeDescriptorContext
        /// </param>
        /// <param name="destinationType">
        ///     Type to convert to
        /// </param>
        /// <returns>
        ///     true if conversion is possible
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
        {
            // Validate Input Arguments
            ArgumentNullException.ThrowIfNull(destinationType);

            return base.CanConvertTo(context, destinationType);
        }
        
        /// <summary>
        ///     TypeConverter method implementation.
        /// </summary>
        /// <param name="context">
        ///     ITypeDescriptorContext
        /// </param>
        /// <param name="culture">
        ///     current culture (see CLR specs)
        /// </param>
        /// <param name="value">
        ///     value to convert from
        /// </param>
        /// <returns>
        ///     value that is result of conversion
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return base.ConvertFrom(context, culture, value);
        }
        
        /// <summary>
        ///     TypeConverter method implementation.
        /// </summary>
        /// <param name="context">
        ///     ITypeDescriptorContext
        /// </param>
        /// <param name="culture">
        ///     current culture (see CLR specs)
        /// </param>
        /// <param name="value">
        ///     value to convert from
        /// </param>
        /// <param name="destinationType">
        ///     Type to convert to
        /// </param>
        /// <returns>
        ///     converted value
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // Validate Input Arguments
            ComponentResourceKey key = value as ComponentResourceKey;
            if (key == null)
            {
                throw new ArgumentException(SR.Format(SR.MustBeOfType, "value", "ComponentResourceKey")); 
            }
            ArgumentNullException.ThrowIfNull(destinationType);
            return base.CanConvertTo(context, destinationType);
        }
    }
}

