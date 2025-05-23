// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Markup;

namespace System.Windows.Input
{
    /// <summary>
    /// Key Serializer class for serializing a Key
    /// </summary>
    /// <ExternalAPI/>
    public class KeyValueSerializer : ValueSerializer
    {
        /// <summary>
        /// CanConvertFromString()
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public override bool CanConvertFromString(string value, IValueSerializerContext context) 
        {
            return true;
        }

        /// <summary>
        /// CanConvertToString()
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <ExternalAPI/> 
        public override bool CanConvertToString(object value, IValueSerializerContext context) 
        {
            if (!(value is Key))
                return false;
            Key key = (Key)value;
            return ((int)key >= (int)Key.None && (int)key <= (int)Key.OemClear);
        }

        /// <summary>
        /// ConvertFromString()
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value, IValueSerializerContext context) 
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Key));
            if (converter != null)
                return converter.ConvertFromString(value);
            else
                return base.ConvertFromString(value, context);
        }

        /// <summary>
        /// ConvertToString()
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string ConvertToString(object value, IValueSerializerContext context) 
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Key));
            if (converter != null)
                return converter.ConvertToInvariantString(value);
            else
                return base.ConvertToString(value, context);
        }
    }
}

