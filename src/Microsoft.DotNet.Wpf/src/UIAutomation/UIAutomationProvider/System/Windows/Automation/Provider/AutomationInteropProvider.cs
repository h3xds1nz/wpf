﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Description: Provides functionality that Win32/Avalon servers need (non-Avalon specific)

using MS.Internal.Automation;

namespace System.Windows.Automation.Provider
{
    /// <summary>
    /// Class containing methods used by Win32 Automation implementations
    /// </summary>
#if (INTERNAL_COMPILE)
    internal static class AutomationInteropProvider
#else
    public static class AutomationInteropProvider
#endif
    {
        //------------------------------------------------------
        //
        //  Public Constants & readonly Fields
        //
        //------------------------------------------------------
 
        #region Public Constants & readonly Fields

        /// <summary>WM_GETOBJECT lParam value indicating that server should return a reference to the root RawElementProvider</summary>
        public const int RootObjectId = -25;

        /// <summary>Maximum number of events to send before batching</summary>
        public const int InvalidateLimit = 20;

        /// <summary>When returned as the first element of IRawElementProviderFragment.GetRuntimeId(), indicates
        /// that the ID is partial and should be appended to the ID provided by the base provider. Typically
        /// only used by Win32 proxies</summary>
        public const int AppendRuntimeId = 3;

        /// <summary>Maximum number of events to send before batching for Items in Containers</summary>
        public const int ItemsInvalidateLimit = 5;

        #endregion Public Constants & readonly Fields

        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------
 
        #region Public Methods

        /// <summary>
        /// Servers that are slotting into the HWND tree can use this to get a base implementation.
        /// </summary>
        /// <param name="hwnd">HWND that server is slotting in over</param>
        /// <returns>base raw element for specified window</returns>
        public static IRawElementProviderSimple HostProviderFromHandle ( IntPtr hwnd )
        {
            ValidateArgument(hwnd != IntPtr.Zero, nameof(SR.HwndMustBeNonNULL));
            return UiaCoreProviderApi.UiaHostProviderFromHwnd(hwnd);
        }
    
        /// <summary>
        /// Server uses this to return an element in response to WM_GETOBJECT.
        /// </summary>
        /// <param name="hwnd">hwnd from the WM_GETOBJECT message</param>
        /// <param name="wParam">wParam from the WM_GETOBJECT message</param>
        /// <param name="lParam">lParam from the WM_GETOBJECT message</param>
        /// <param name="el">element to return</param>
        /// <returns>Server should return the return value as the lresult return value to the WM_GETOBJECT windows message</returns>
        public static IntPtr ReturnRawElementProvider (IntPtr hwnd, IntPtr wParam, IntPtr lParam, IRawElementProviderSimple el )
        {
            ValidateArgument( hwnd != IntPtr.Zero, nameof(SR.HwndMustBeNonNULL));
            ArgumentNullException.ThrowIfNull(el);

            return UiaCoreProviderApi.UiaReturnRawElementProvider(hwnd, wParam, lParam, el);
        }

        /// <summary>
        /// Called by a server to determine if there are any listeners for events.
        /// </summary>
        public static bool ClientsAreListening 
        { 
            get 
            {
                return UiaCoreProviderApi.UiaClientsAreListening();
            } 
        }

        /// <summary>
        /// Called by a server to notify the UIAccess server of a AutomationPropertyChangedEvent event.
        /// </summary>
        /// <param name="element">The actual server-side element associated with this event.</param>
        /// <param name="e">Contains information about the property that changed.</param>
        public static void RaiseAutomationPropertyChangedEvent(IRawElementProviderSimple element, AutomationPropertyChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(element);
            ArgumentNullException.ThrowIfNull(e);

            UiaCoreProviderApi.UiaRaiseAutomationPropertyChangedEvent(element, e.Property.Id, e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called to notify listeners of a pattern or custom event.  This could could be called by a server implementation or by a proxy's event
        /// translator.
        /// </summary>
        /// <param name="eventId">An AutomationEvent representing this event.</param>
        /// <param name="provider">The actual server-side element associated with this event.</param>
        /// <param name="e">Contains information about the event (may be null).</param>
        public static void RaiseAutomationEvent(AutomationEvent eventId, IRawElementProviderSimple provider, AutomationEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(eventId);
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(e);

            if (e.EventId == AutomationElementIdentifiers.AsyncContentLoadedEvent)
            {
                AsyncContentLoadedEventArgs asyncArgs = e as AsyncContentLoadedEventArgs;
                if(asyncArgs == null)
                    ThrowInvalidArgument("e");

                UiaCoreProviderApi.UiaRaiseAsyncContentLoadedEvent(provider, asyncArgs.AsyncContentLoadedState, asyncArgs.PercentComplete);
                return;
            }

            if (e.EventId == AutomationElementIdentifiers.NotificationEvent)
            {
                NotificationEventArgs notificationArgs = e as NotificationEventArgs;
                if (notificationArgs == null)
                    ThrowInvalidArgument("e");

                UiaCoreProviderApi.UiaRaiseNotificationEvent(provider,
                    notificationArgs.NotificationKind,
                    notificationArgs.NotificationProcessing,
                    notificationArgs.DisplayString,
                    notificationArgs.ActivityId);
                return;
            }

            if (e.EventId == AutomationElementIdentifiers.ActiveTextPositionChangedEvent)
            {
                ActiveTextPositionChangedEventArgs activeTextPositionChangedArgs = e as ActiveTextPositionChangedEventArgs;
                if (activeTextPositionChangedArgs == null)
                    ThrowInvalidArgument("e");

                UiaCoreProviderApi.UiaRaiseActiveTextPositionChangedEvent(provider, activeTextPositionChangedArgs.TextRange);
                return;
            }

            if (e.EventId == WindowPatternIdentifiers.WindowClosedEvent && !(e is WindowClosedEventArgs))
                ThrowInvalidArgument("e");

            // fire to all clients
            UiaCoreProviderApi.UiaRaiseAutomationEvent(provider, eventId.Id);
        }

        /// <summary>
        /// Called by a server to notify the UIAccess server of a tree change event.
        /// </summary>
        /// <param name="provider">The actual server-side element associated with this event.</param>
        /// <param name="e">Contains information about the event.</param>
        public static void RaiseStructureChangedEvent(IRawElementProviderSimple provider, StructureChangedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(e);

            UiaCoreProviderApi.UiaRaiseStructureChangedEvent(provider, e.StructureChangeType, e.GetRuntimeId());
        }
        #endregion Public Methods


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        // Throw an argument Exception with a generic error
        private static void ThrowInvalidArgument(string argName)
        {
            throw new ArgumentException(SR.Format(SR.GenericInvalidArgument, argName));
        }

        // Check that specified condition is true; if not, throw exception
        private static void ValidateArgument(bool cond, string reason)
        {
            if (!cond)
            {
                throw new ArgumentException(SR.GetResourceString(reason, null));
            }
        }

        #endregion Private Methods
    }
}
