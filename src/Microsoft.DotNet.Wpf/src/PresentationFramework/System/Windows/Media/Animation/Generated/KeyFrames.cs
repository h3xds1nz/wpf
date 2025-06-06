// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

//
//
// This file was generated, please do not edit it directly.
//
// Please see MilCodeGen.html for more information.
//

using System.Windows.Media.Media3D;

namespace System.Windows.Media.Animation
{


    /// <summary>
    /// This class is used as part of a ThicknessKeyFrameCollection in
    /// conjunction with a KeyFrameThicknessAnimation to animate a
    /// Thickness property value along a set of key frames.
    /// </summary>
    public abstract class ThicknessKeyFrame : Freezable, IKeyFrame
    {
        #region Constructors

        /// <summary>
        /// Creates a new ThicknessKeyFrame.
        /// </summary>
        protected ThicknessKeyFrame()
            : base()
        {
        }

        /// <summary>
        /// Creates a new ThicknessKeyFrame.
        /// </summary>
        protected ThicknessKeyFrame(Thickness value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new DiscreteThicknessKeyFrame.
        /// </summary>
        protected ThicknessKeyFrame(Thickness value, KeyTime keyTime)
            : this()
        {
            Value = value;
            KeyTime = keyTime;
        }

        #endregion

        #region IKeyFrame

        /// <summary>
        /// KeyTime Property
        /// </summary>
        public static readonly DependencyProperty KeyTimeProperty =
            DependencyProperty.Register(
                    "KeyTime",
                    typeof(KeyTime),
                    typeof(ThicknessKeyFrame),
                    new PropertyMetadata(KeyTime.Uniform));

        /// <summary>
        /// The time at which this KeyFrame's value should be equal to the Value
        /// property.
        /// </summary>
        public KeyTime KeyTime
        {
            get
            {
            return (KeyTime)GetValue(KeyTimeProperty);
            }
            set
            {
            SetValueInternal(KeyTimeProperty, value);
            }
        }

        /// <summary>
        /// Value Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                    "Value",
                    typeof(Thickness),
                    typeof(ThicknessKeyFrame),
                    new PropertyMetadata());

        /// <summary>
        /// The value of this key frame at the KeyTime specified.
        /// </summary>
        object IKeyFrame.Value
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (Thickness)value;
            }
        }

        /// <summary>
        /// The value of this key frame at the KeyTime specified.
        /// </summary>
        public Thickness Value
        {
            get
            {
                return (Thickness)GetValue(ValueProperty);
            }
            set
            {
                SetValueInternal(ValueProperty, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the interpolated value of the key frame at the progress value
        /// provided.  The progress value should be calculated in terms of this 
        /// specific key frame.
        /// </summary>
        public Thickness InterpolateValue(
            Thickness baseValue, 
            double keyFrameProgress)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(keyFrameProgress);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(keyFrameProgress, 1.0);

            return InterpolateValueCore(baseValue, keyFrameProgress);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method should be implemented by derived classes to calculate
        /// the value of this key frame at the progress value provided.
        /// </summary>
        protected abstract Thickness InterpolateValueCore(
            Thickness baseValue,
            double keyFrameProgress);

        #endregion
    }

}