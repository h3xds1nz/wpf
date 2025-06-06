﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


//
// Description: The AutomationPeer for PasswordBox.
//

using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Documents;
using MS.Internal.Automation;

namespace System.Windows.Automation.Peers
{
    /// 
    public class PasswordBoxAutomationPeer : TextAutomationPeer, IValueProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">PasswordBox for which this is an AutomationPeer</param>
        public PasswordBoxAutomationPeer(PasswordBox owner) : base(owner)
        {
        }
    
        /// <summary>
        /// Class name for the type for which this is a peer.
        /// </summary>
        /// <returns></returns>
        protected override string GetClassNameCore()
        {
            return "PasswordBox";
        }

        /// <summary>
        /// Type for which this is a peer.
        /// </summary>
        /// <returns></returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Edit;
        }

        /// <summary>
        /// Return the patterns supported by PasswordBoxAutomationPeer
        /// </summary>
        /// <param name="patternInterface"></param>
        /// <returns></returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            object returnValue = null;

            if (patternInterface == PatternInterface.Value)
            {
                returnValue = this;
            }
            else if (patternInterface == PatternInterface.Text)
            {
                if (_textPattern == null)
                {
                    _textPattern = new TextAdaptor(this, ((PasswordBox)Owner).TextContainer);
                }

                returnValue = _textPattern;
            }
            else if (patternInterface == PatternInterface.Scroll)
            {
                PasswordBox owner = (PasswordBox)Owner;
                if (owner.ScrollViewer != null)
                {
                    returnValue = owner.ScrollViewer.CreateAutomationPeer();
                    ((AutomationPeer)returnValue).EventsSource = this;
                }
            }
            else
            {
                returnValue = base.GetPattern(patternInterface);
            }

            return returnValue;
        }

        /// <summary>
        /// Indicates whether or not this is a password control
        /// </summary>
        /// <returns>true</returns>
        protected override bool IsPasswordCore()
        {
            return true;
        }
        
        bool IValueProvider.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        string IValueProvider.Value
        {
            get 
            {
                //  We shouldn't throw InvalidOperationException, return an empty string instead
                if (AccessibilitySwitches.UseNetFx47CompatibleAccessibilityFeatures)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        void IValueProvider.SetValue(string value)
        {
            if(!IsEnabled())
                throw new ElementNotEnabledException();

            PasswordBox owner = (PasswordBox)Owner;

            ArgumentNullException.ThrowIfNull(value);

            owner.Password = value;
        }

        // BUG 1555137: Never inline, as we don't want to unnecessarily link the automation DLL
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseValuePropertyChangedEvent(string oldValue, string newValue)
        {
            if (oldValue != newValue)
            {
                RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, oldValue, newValue);
            }
        }

        // BUG 1555137: Never inline, as we don't want to unnecessarily link the automation DLL
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseIsReadOnlyPropertyChangedEvent(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                RaisePropertyChangedEvent(ValuePatternIdentifiers.IsReadOnlyProperty, oldValue, newValue);
            }
        }

        /// <summary>
        /// Gets collection of AutomationPeers for given text range.
        /// </summary>
        internal override List<AutomationPeer> GetAutomationPeersFromRange(ITextPointer start, ITextPointer end)
        {
            return new List<AutomationPeer>();
        }

        private TextAdaptor _textPattern;
    }
}

