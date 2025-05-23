﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Implements ETW tracing for Avalon Managed Code

#if !SILVERLIGHTXAML

#if SYSTEM_XAML
namespace MS.Internal.Xaml
#else
namespace MS.Utility
#endif
{
    #region Trace

    internal static partial class EventTrace
    {
        internal static readonly TraceProvider EventProvider;

        // EasyTraceEvent
        // Checks the keyword and level before emiting the event
        internal static void EasyTraceEvent(Keyword keywords, Event eventID)
        {
            if (IsEnabled(keywords, Level.Info))
            {
                EventProvider.TraceEvent(eventID, keywords, Level.Info);
            }
        }

        // EasyTraceEvent
        // Checks the keyword and level before emiting the event
        internal static void EasyTraceEvent(Keyword keywords, Level level, Event eventID)
        {
            if (IsEnabled(keywords, level))
            {
                EventProvider.TraceEvent(eventID, keywords, level);
            }
        }

        // EasyTraceEvent
        // Checks the keyword and level before emiting the event
        internal static void EasyTraceEvent<T1>(Keyword keywords, Event eventID, T1 param1)
        {
            if (IsEnabled(keywords, Level.Info))
            {
                EventProvider.TraceEvent(eventID, keywords, Level.Info, param1);
            }
        }

        // EasyTraceEvent
        // Checks the keyword and level before emiting the event
        internal static void EasyTraceEvent<T1>(Keyword keywords, Level level, Event eventID, T1 param1)
        {
            if (IsEnabled(keywords, level))
            {
                EventProvider.TraceEvent(eventID, keywords, level, param1);
            }
        }

        // EasyTraceEvent
        // Checks the keyword and level before emiting the event
        internal static void EasyTraceEvent<T1, T2>(Keyword keywords, Event eventID, T1 param1, T2 param2)
        {
            if (IsEnabled(keywords, Level.Info))
            {
                EventProvider.TraceEvent(eventID, keywords, Level.Info, param1, param2);
            }
        }

        internal static void EasyTraceEvent<T1, T2>(Keyword keywords, Level level, Event eventID, T1 param1, T2 param2)
        {
            if (IsEnabled(keywords, Level.Info))
            {
                EventProvider.TraceEvent(eventID, keywords, Level.Info, param1, param2);
            }
        }

        // EasyTraceEvent
        // Checks the keyword and level before emiting the event
        internal static void EasyTraceEvent<T1, T2, T3>(Keyword keywords, Event eventID, T1 param1, T2 param2, T3 param3)
        {
            if (IsEnabled(keywords, Level.Info))
            {
                EventProvider.TraceEvent(eventID, keywords, Level.Info, param1, param2, param3);
            }
        }

        #region Trace related enumerations

        public enum LayoutSource : byte
        {
            LayoutManager,
            HwndSource_SetLayoutSize,
            HwndSource_WMSIZE
        }

        #endregion

        /// <summary>
        /// Callers use this to check if they should be logging.
        /// </summary>
        internal static bool IsEnabled(Keyword flag, Level level)
        {
            return EventProvider.IsEnabled(flag, level);
        }

        /// <summary>
        /// Internal operations associated with initializing the event provider and
        /// monitoring the Dispatcher and input components.
        /// </summary>
        static EventTrace()
        {
            Guid providerGuid = new Guid("E13B77A8-14B6-11DE-8069-001B212B5009");

            if (Environment.OSVersion.Version.Major < 6 ||
                IsClassicETWRegistryEnabled())
            {
                EventProvider = new ClassicTraceProvider();
            }
            else
            {
                EventProvider = new ManifestTraceProvider();
            }
            EventProvider.Register(providerGuid);
        }

        private static bool IsClassicETWRegistryEnabled()
        {
            string regKey = @"HKEY_CURRENT_USER\Software\Microsoft\Avalon.Graphics\";                
            return int.Equals(1, Microsoft.Win32.Registry.GetValue(regKey, "ClassicETW", 0));
        }
    }

    #endregion Trace
}
#endif 
