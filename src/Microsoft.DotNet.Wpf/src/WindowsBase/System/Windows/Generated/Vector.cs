// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// This file was generated, please do not edit it directly.
//
// Please see MilCodeGen.html for more information.
//

using MS.Internal;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Converters;

namespace System.Windows
{

    [Serializable]
    [TypeConverter(typeof(VectorConverter))]
    [ValueSerializer(typeof(VectorValueSerializer))] // Used by MarkupWriter
    public partial struct Vector : IFormattable
    {
        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods




        /// <summary>
        /// Compares two Vector instances for exact equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        public static bool operator == (Vector vector1, Vector vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y;
        }

        /// <summary>
        /// Compares two Vector instances for exact inequality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, Double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        public static bool operator != (Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }
        /// <summary>
        /// Compares two Vector instances for object equality.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Vector instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        public static bool Equals (Vector vector1, Vector vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y);
        }

        /// <summary>
        /// Equals - compares this Vector with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Vector and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override readonly bool Equals(object o)
        {
            return o is Vector other && Vector.Equals(this, other);
        }

        /// <summary>
        /// Equals - compares this Vector with the passed in object.  In this equality
        /// Double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Vector to compare to "this"</param>
        public readonly bool Equals(Vector value)
        {
            return Vector.Equals(this, value);
        }
        /// <summary>
        /// Returns the HashCode for this Vector
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Vector
        /// </returns>
        public override readonly int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode();
        }

        /// <summary>
        /// Parse - returns an instance converted from the provided string using
        /// the culture "en-US"
        /// <param name="source"> string with Vector data </param>
        /// </summary>
        public static Vector Parse(string source)
        {
            IFormatProvider formatProvider = System.Windows.Markup.TypeConverterHelper.InvariantEnglishUS;

            TokenizerHelper th = new TokenizerHelper(source, formatProvider);

            Vector value;

            String firstToken = th.NextTokenRequired();

            value = new Vector(
                Convert.ToDouble(firstToken, formatProvider),
                Convert.ToDouble(th.NextTokenRequired(), formatProvider));

            // There should be no more tokens in this string.
            th.LastTokenRequired();

            return value;
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------




        #region Public Properties

        /// <summary>
        ///     X - double.  Default value is 0.
        /// </summary>
        public double X
        {
            readonly get
            {
                return _x;
            }

            set
            {
                _x = value;
            }

        }

        /// <summary>
        ///     Y - double.  Default value is 0.
        /// </summary>
        public double Y
        {
            readonly get
            {
                return _y;
            }

            set
            {
                _y = value;
            }

        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods





        #endregion ProtectedMethods

        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods









        #endregion Internal Methods

        //------------------------------------------------------
        //
        //  Internal Properties
        //
        //------------------------------------------------------

        #region Internal Properties


        /// <summary>
        /// Creates a string representation of this object based on the current culture.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public override string ToString()
        {

            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null /* format string */, null /* format provider */);
        }

        /// <summary>
        /// Creates a string representation of this object based on the IFormatProvider
        /// passed in.  If the provider is null, the CurrentCulture is used.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {

            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(null /* format string */, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {

            // Delegate to the internal method which implements all ToString calls.
            return ConvertToString(format, provider);
        }

        /// <summary>
        /// Creates a string representation of this object based on the format string
        /// and IFormatProvider passed in.
        /// If the provider is null, the CurrentCulture is used.
        /// See the documentation for IFormattable for more information.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            // Helper to get the numeric list separator for a given culture.
            char separator = MS.Internal.TokenizerHelper.GetNumericListSeparator(provider);
            return String.Format(provider,
                                 "{1:" + format + "}{0}{2:" + format + "}",
                                 separator,
                                 _x,
                                 _y);
        }



        #endregion Internal Properties

        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties



        #endregion Dependency Properties

        //------------------------------------------------------
        //
        //  Internal Fields
        //
        //------------------------------------------------------

        #region Internal Fields


        internal double _x;
        internal double _y;




        #endregion Internal Fields



        #region Constructors

        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------




        #endregion Constructors
    }
}
