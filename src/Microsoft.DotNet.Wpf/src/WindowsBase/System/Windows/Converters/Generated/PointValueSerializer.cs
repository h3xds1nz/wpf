// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// This file was generated, please do not edit it directly.
//
// Please see MilCodeGen.html for more information.
//

using System.Windows;
using System.Windows.Markup;

using ConverterHelper = System.Windows.Markup.TypeConverterHelper;

namespace System.Windows.Converters
{
    /// <summary>
    /// PointValueSerializer - ValueSerializer class for converting instances of strings to and from Point instances
    /// This is used by the MarkupWriter class.
    /// </summary>
    public class PointValueSerializer : ValueSerializer 
    {
        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        /// <summary>
        /// Returns true if the given value can be converted into a string
        /// </summary>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            // Validate the input type
            return value is Point;
        }

        /// <summary>
        /// Converts a string into a Point.
        /// </summary>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            return value is not null ? Point.Parse(value) : base.ConvertFromString(value, context);
        }

        /// <summary>
        /// Converts the value into a string.
        /// </summary>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            if (value is not Point point)
            {
                // Let base throw an exception.
                return base.ConvertToString(value, context);
            }

            return point.ConvertToString(null, ConverterHelper.InvariantEnglishUS);
        }
    }
}
