﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MS.Win32
{
    internal partial class SafeNativeMethods
    {
        [Flags]
        internal enum PlaySoundFlags
        {
            SND_SYNC = 0x00000000, /* play synchronously (default) */
            SND_ASYNC = 0x00000001, /* play asynchronously */
            SND_NODEFAULT = 0x00000002, /* silence (!default) if sound not found */
            SND_MEMORY = 0x00000004, /* pszSound points to a memory file */
            SND_LOOP = 0x00000008, /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x00000010, /* don't stop any currently playing sound */
            SND_PURGE = 0x00000040, /* purge non-static events for task */
            SND_APPLICATION = 0x00000080, /* look for application specific association */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000, /* name is a registry alias */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040000, /* name is resource name or atom */
        }

        public static bool IsUxThemeActive() { return SafeNativeMethodsPrivate.IsThemeActive() != 0; }

        public static bool SetCaretPos(int x, int y)
        {
            // To be consistent with our other PInvoke wrappers
            // we should "throw" a Win32Exception on error here.
            // But we don't want to introduce new "throws" w/o 
            // time to follow up on any new problems that causes.

            return SafeNativeMethodsPrivate.SetCaretPos(x, y);
        }

        public static bool DestroyCaret()
        {
            // To be consistent with our other PInvoke wrappers
            // we should "throw" a Win32Exception on error here.
            // But we don't want to introduce new "throws" w/o 
            // time to follow up on any new problems that causes.

            return SafeNativeMethodsPrivate.DestroyCaret();
        }

        // NOTE:  CLR has this in UnsafeNativeMethodsCLR.cs.  Not sure why it is unsafe - need to follow up.
        public static int GetCaretBlinkTime()
        {
            // To be consistent with our other PInvoke wrappers
            // we should "throw" a Win32Exception on error here.
            // But we don't want to introduce new "throws" w/o 
            // time to follow up on any new problems that causes.

            return SafeNativeMethodsPrivate.GetCaretBlinkTime();
        }

        // Constants for GetStringTypeEx.
        public const uint CT_CTYPE1 = 1;
        public const uint CT_CTYPE2 = 2;
        public const uint CT_CTYPE3 = 4;

        public const ushort C1_SPACE = 0x0008;
        public const ushort C1_PUNCT = 0x0010;
        public const ushort C1_BLANK = 0x0040;

        public const ushort C3_NONSPACING = 0x0001;
        public const ushort C3_DIACRITIC = 0x0002;
        public const ushort C3_VOWELMARK = 0x0004;
        public const ushort C3_KATAKANA = 0x0010;
        public const ushort C3_HIRAGANA = 0x0020;
        public const ushort C3_HALFWIDTH = 0x0040;
        public const ushort C3_FULLWIDTH = 0x0080;
        public const ushort C3_IDEOGRAPH = 0x0100;
        public const ushort C3_KASHIDA = 0x0200;

        public static unsafe bool GetStringTypeEx(uint locale, uint infoType, ReadOnlySpan<char> sourceString, Span<ushort> charTypes)
        {
            // Since we do not use [LibraryImport], Span<T> marshallers are not available by default
            fixed (char* ptrSourceString = sourceString)
            fixed (ushort* ptrCharTypes = charTypes)
            {
                if (!SafeNativeMethodsPrivate.GetStringTypeEx(locale, infoType, ptrSourceString, sourceString.Length, ptrCharTypes))
                    throw new Win32Exception(); // Initializes with Marshal.GetLastPInvokeError()
            }

            return true;
        }

        public static int GetSysColor(int nIndex)
        {
            return SafeNativeMethodsPrivate.GetSysColor(nIndex);
        }

#if FRAMEWORK_NATIVEMETHODS || BASE_NATIVEMETHODS 
        public static bool IsDebuggerPresent() { return SafeNativeMethodsPrivate.IsDebuggerPresent(); }
#endif

#if BASE_NATIVEMETHODS
        public static void QueryPerformanceCounter(out long lpPerformanceCount)
        {
            if (!SafeNativeMethodsPrivate.QueryPerformanceCounter(out lpPerformanceCount))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static void QueryPerformanceFrequency(out long lpFrequency)
        {
            if (!SafeNativeMethodsPrivate.QueryPerformanceFrequency(out lpFrequency))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        internal static int GetMessageTime()
        {
            return SafeNativeMethodsPrivate.GetMessageTime();
        }
#endif // BASE_NATIVEMETHODS

        internal static int GetWindowStyle(HandleRef hWnd, bool exStyle)
        {
            int nIndex = exStyle ? NativeMethods.GWL_EXSTYLE : NativeMethods.GWL_STYLE;
            return UnsafeNativeMethods.GetWindowLong(hWnd, nIndex);
        }

        private static partial class SafeNativeMethodsPrivate
        {
            [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Unicode)]
            public static extern int IsThemeActive();

            [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SetCaretPos(int x, int y);

            [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool DestroyCaret();

            // NOTE:  CLR has this in UnsafeNativeMethodsCLR.cs.  Not sure why it is unsafe - need to follow up.
            [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int GetCaretBlinkTime();

            [DllImport(ExternDll.Kernel32, SetLastError = true, CharSet = CharSet.Auto)]
            public static extern unsafe bool GetStringTypeEx(uint locale, uint infoType, char* sourceString, int count, ushort* charTypes);

            [DllImport(ExternDll.User32, CharSet = CharSet.Auto)]
            public static extern int GetSysColor(int nIndex);

#if FRAMEWORK_NATIVEMETHODS || BASE_NATIVEMETHODS
            [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            internal static extern bool IsDebuggerPresent();
#endif

#if BASE_NATIVEMETHODS

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool QueryPerformanceFrequency(out long lpFrequency);

            [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
            internal static extern int GetMessageTime();
#endif
        }
    }
}

