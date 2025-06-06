// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


//------------------------------------------------------------------------------
//

//
// Description: Collection of helper methods for generating IAnimatable
//              implementations.
//
//---------------------------------------------------------------------------

namespace MS.Internal.MilCodeGen.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    using MS.Internal.MilCodeGen.Runtime;
    using MS.Internal.MilCodeGen.ResourceModel;

    public class IAnimatableHelper : GeneratorMethods
    {
        
        public static string WriteImplementation()
        {
            return
                [[inline]]
                    #region IAnimatable
                    
                    /// <summary>
                    /// Applies an AnimationClock to a DepencencyProperty which will
                    /// replace the current animations on the property using the snapshot
                    /// and replace HandoffBehavior.
                    /// </summary>
                    /// <param name="dp">
                    /// The DependencyProperty to animate.
                    /// </param>
                    /// <param name="clock">
                    /// The AnimationClock that will animate the property. If this is null
                    /// then all animations will be removed from the property.
                    /// </param>
                    public void ApplyAnimationClock(
                        DependencyProperty dp,
                        AnimationClock clock)
                    {
                        ApplyAnimationClock(dp, clock, HandoffBehavior.SnapshotAndReplace);
                    }

                    /// <summary>
                    /// Applies an AnimationClock to a DependencyProperty. The effect of
                    /// the new AnimationClock on any current animations will be determined by
                    /// the value of the handoffBehavior parameter.
                    /// </summary>
                    /// <param name="dp">
                    /// The DependencyProperty to animate.
                    /// </param>
                    /// <param name="clock">
                    /// The AnimationClock that will animate the property. If parameter is null
                    /// then animations will be removed from the property if handoffBehavior is
                    /// SnapshotAndReplace; otherwise the method call will have no result.
                    /// </param>
                    /// <param name="handoffBehavior">
                    /// Determines how the new AnimationClock will transition from or
                    /// affect any current animations on the property.
                    /// </param>
                    public void ApplyAnimationClock(
                        DependencyProperty dp,
                        AnimationClock clock,
                        HandoffBehavior handoffBehavior)
                    {
                        ArgumentNullException.ThrowIfNull(dp);

                        if (!AnimationStorage.IsPropertyAnimatable(this, dp))
                        {
                            throw new ArgumentException(SR.Format(SR.Animation_DependencyPropertyIsNotAnimatable, dp.Name, this.GetType()), nameof(dp));
                        }

                        if (clock != null
                            && !AnimationStorage.IsAnimationValid(dp, clock.Timeline))
                        {
                            throw new ArgumentException(SR.Format(SR.Animation_AnimationTimelineTypeMismatch, clock.Timeline.GetType(), dp.Name, dp.PropertyType), nameof(clock));
                        }

                        if (!HandoffBehaviorEnum.IsDefined(handoffBehavior))
                        {
                            throw new ArgumentException(SR.Animation_UnrecognizedHandoffBehavior);
                        }

                        if (IsSealed)
                        {
                            throw new InvalidOperationException(SR.Format(SR.IAnimatable_CantAnimateSealedDO, dp, this.GetType()));
                        }
                        
                        AnimationStorage.ApplyAnimationClock(this, dp, clock, handoffBehavior);
                    }

                    /// <summary>
                    /// Starts an animation for a DependencyProperty. The animation will
                    /// begin when the next frame is rendered.
                    /// </summary>
                    /// <param name="dp">
                    /// The DependencyProperty to animate.
                    /// </param>
                    /// <param name="animation">
                    /// <para>The AnimationTimeline to used to animate the property.</para>
                    /// <para>If the AnimationTimeline's BeginTime is null, any current animations
                    /// will be removed and the current value of the property will be held.</para>
                    /// <para>If this value is null, all animations will be removed from the property
                    /// and the property value will revert back to its base value.</para>
                    /// </param>
                    public void BeginAnimation(DependencyProperty dp, AnimationTimeline animation)
                    {
                        BeginAnimation(dp, animation, HandoffBehavior.SnapshotAndReplace);
                    }

                    /// <summary>
                    /// Starts an animation for a DependencyProperty. The animation will
                    /// begin when the next frame is rendered.
                    /// </summary>
                    /// <param name="dp">
                    /// The DependencyProperty to animate.
                    /// </param>
                    /// <param name="animation">
                    /// <para>The AnimationTimeline to used to animate the property.</para>
                    /// <para>If the AnimationTimeline's BeginTime is null, any current animations
                    /// will be removed and the current value of the property will be held.</para>
                    /// <para>If this value is null, all animations will be removed from the property
                    /// and the property value will revert back to its base value.</para>
                    /// </param>
                    /// <param name="handoffBehavior">
                    /// Specifies how the new animation should interact with any current
                    /// animations already affecting the property value.
                    /// </param>
                    public void BeginAnimation(DependencyProperty dp, AnimationTimeline animation, HandoffBehavior handoffBehavior)
                    {
                        ArgumentNullException.ThrowIfNull(dp);

                        if (!AnimationStorage.IsPropertyAnimatable(this, dp))
                        {
                            throw new ArgumentException(SR.Format(SR.Animation_DependencyPropertyIsNotAnimatable, dp.Name, this.GetType()), nameof(dp));
                        }

                        if (animation != null
                            && !AnimationStorage.IsAnimationValid(dp, animation))
                        {
                            throw new ArgumentException(SR.Format(SR.Animation_AnimationTimelineTypeMismatch, animation.GetType(), dp.Name, dp.PropertyType), nameof(animation));
                        }

                        if (!HandoffBehaviorEnum.IsDefined(handoffBehavior))
                        {
                            throw new ArgumentException(SR.Animation_UnrecognizedHandoffBehavior);
                        }

                        if (IsSealed)
                        {
                            throw new InvalidOperationException(SR.Format(SR.IAnimatable_CantAnimateSealedDO, dp, this.GetType()));
                        }
                        
                        AnimationStorage.BeginAnimation(this, dp, animation, handoffBehavior);
                    }

                    /// <summary>
                    /// Returns true if any properties on this DependencyObject have a
                    /// persistent animation or the object has one or more clocks associated
                    /// with any of its properties.
                    /// </summary>
                    public bool HasAnimatedProperties
                    {
                        get
                        {
                            VerifyAccess();

                            return IAnimatable_HasAnimatedProperties;
                        }
                    }

                    /// <summary>
                    ///   If the dependency property is animated this method will
                    ///   give you the value as if it was not animated.
                    /// </summary>
                    /// <param name="dp">The DependencyProperty</param>
                    /// <returns>
                    ///   The value that would be returned if there were no
                    ///   animations attached.  If there aren't any attached, then
                    ///   the result will be the same as that returned from
                    ///   GetValue.
                    /// </returns>
                    public object GetAnimationBaseValue(DependencyProperty dp)
                    {
                        ArgumentNullException.ThrowIfNull(dp);
                    
                        return this.GetValueEntry(
                                LookupEntry(dp.GlobalIndex),
                                dp,
                                null,
                                RequestFlags.AnimationBaseValue).Value;
                    }
                    
                    #endregion IAnimatable

                    #region Animation

                    /// <summary>
                    ///     Allows subclasses to participate in property animated value computation
                    /// </summary>
                    /// <param name="dp"></param>
                    /// <param name="metadata"></param>
                    /// <param name="entry">EffectiveValueEntry computed by base</param>
                    internal sealed override void EvaluateAnimatedValueCore(
                            DependencyProperty  dp,
                            PropertyMetadata    metadata,
                        ref EffectiveValueEntry entry)
                    {
                        if (IAnimatable_HasAnimatedProperties)
                        {
                            AnimationStorage storage = AnimationStorage.GetStorage(this, dp);

                            storage?.EvaluateAnimatedValue(metadata, ref entry);
                        }
                    }

                    #endregion Animation
                [[/inline]];
        }
    }
}



