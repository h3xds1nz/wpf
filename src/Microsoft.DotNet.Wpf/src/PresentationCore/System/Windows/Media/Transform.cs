﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/***************************************************************************\
*
*
* Description:
*
*
\***************************************************************************/

using MS.Internal;
using System.Windows.Media.Composition;

namespace System.Windows.Media
{
    #region Transform
    ///<summary>
    /// Transform provides a base for all types of transformations, including matrix and list type.
    ///</summary>
    [Localizability(LocalizationCategory.None, Readability=Readability.Unreadable)]
    public abstract partial class Transform : GeneralTransform
    {
        internal Transform()
        {
        }

        ///<summary>
        /// Identity transformation.
        ///</summary>
        public static Transform Identity
        {
            get
            {
                return s_identity;
            }
        }

        private static Transform MakeIdentityTransform()
        {
            Transform identity = new MatrixTransform(Matrix.Identity);
            identity.Freeze();
            return identity;
        }
         
        private static Transform s_identity = MakeIdentityTransform();
        
        ///<summary>
        /// Return the current transformation value.
        ///</summary>
        public abstract Matrix Value { get; }

        ///<summary>
        /// Returns true if transformation if the transformation is definitely an identity.  There are cases where it will
        /// return false because of computational error or presence of animations (And we're interpolating through a
        /// transient identity) -- this is intentional.  This property is used internally only.  If you need to check the
        /// current matrix value for identity, use Transform.Value.Identity.
        ///</summary>
        internal abstract bool IsIdentity {get;}
 
        internal virtual bool CanSerializeToString() { return false; }

        #region Perf Helpers

        internal virtual void TransformRect(ref Rect rect)
        {
            Matrix matrix = Value;
            MatrixUtil.TransformRect(ref rect, ref matrix);
        }

        /// <summary>
        /// MultiplyValueByMatrix - result is set equal to "this" * matrixToMultiplyBy.
        /// </summary>
        /// <param name="result"> The result is stored here. </param>
        /// <param name="matrixToMultiplyBy"> The multiplicand. </param>
        internal virtual void MultiplyValueByMatrix(ref Matrix result, ref Matrix matrixToMultiplyBy)
        {
            result = Value;
            MatrixUtil.MultiplyMatrix(ref result, ref matrixToMultiplyBy);
        }

        internal virtual unsafe void ConvertToD3DMATRIX(/* out */ D3DMATRIX* milMatrix)
        {
            Matrix matrix = Value;
            MILUtilities.ConvertToD3DMATRIX(&matrix, milMatrix);
        }

        #endregion

        /// <summary>
        /// Consolidates the common logic of obtain the value of a 
        /// Transform, after checking the transform for null.
        /// </summary>
        /// <param name="transform"> Transform to obtain value of. </param>
        /// <param name="currentTransformValue"> 
        ///     Current value of 'transform'.  Matrix.Identity if
        ///     the 'transform' parameter is null.
        /// </param>
        internal static void GetTransformValue(
            Transform transform,
            out Matrix currentTransformValue
            )
        {
            if (transform != null)
            {
                currentTransformValue = transform.Value;
            }
            else
            {
                currentTransformValue = Matrix.Identity;
            }    
        }

        /// <summary>
        /// Transforms a point
        /// </summary>
        /// <param name="inPoint">Input point</param>
        /// <param name="result">Output point</param>
        /// <returns>True if the point was successfully transformed</returns>
        public override bool TryTransform(Point inPoint, out Point result)
        {
            Matrix m = Value;
            result = m.Transform(inPoint);
            return true;
        }

        /// <summary>
        /// Transforms the bounding box to the smallest axis aligned bounding box
        /// that contains all the points in the original bounding box
        /// </summary>
        /// <param name="rect">Bounding box</param>
        /// <returns>The transformed bounding box</returns>
        public override Rect TransformBounds(Rect rect)
        {
            TransformRect(ref rect);
            return rect;
        }


        /// <summary>
        /// Returns the inverse transform if it has an inverse, null otherwise
        /// </summary>        
        public override GeneralTransform Inverse
        {
            get
            {
                ReadPreamble();

                Matrix matrix = Value;

                if (!matrix.HasInverse)
                {
                    return null;
                }

                matrix.Invert();
                return new MatrixTransform(matrix);
            }
        }

        /// <summary>
        /// Returns a best effort affine transform
        /// </summary>        
        internal override Transform AffineTransform
        {
            get
            {
                return this;
            }
        }
    }
    #endregion
}


