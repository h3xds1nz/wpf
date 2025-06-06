﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Description: Some applications implement menus with toolbars.  This proxy
//              will used the IAccessible to expose these toolbars as
//              menus.  This proxy is derived from WindowsToolbar since 
//              the underlying control really is a toolbar and WindowsToolbar
//              knows how to communicate with then underlying toolbar control
//              already.
//

using System;
using System.Windows.Automation;

namespace MS.Internal.AutomationProxies
{
    internal class WindowsToolbarAsMenu : WindowsToolbar
    {
        // ------------------------------------------------------
        //
        // Constructors
        //
        // ------------------------------------------------------

        #region Constructors

        internal WindowsToolbarAsMenu(IntPtr hwnd, ProxyFragment parent, int item, Accessible acc)
            : base( hwnd, parent, item )
        {
            _acc = acc;

            // Set the control type based on the IAccessible role.
            AccessibleRole role = acc.Role;

            if (role == AccessibleRole.MenuBar)
            {
                _cControlType = ControlType.MenuBar;
                _sAutomationId = "MenuBar"; // This string is a non-localizable string
            }
            else if (role == AccessibleRole.MenuPopup)
            {
                _cControlType = ControlType.Menu;
                _sAutomationId = "MenuPopup"; // This string is a non-localizable string
            }
            else
            {
                System.Diagnostics.Debug.Fail("Unexpected role " + role);
            }
        }

        #endregion

        // ------------------------------------------------------
        //
        // Private Fields
        //
        // ------------------------------------------------------

        #region Private Fields

        private Accessible _acc;

        #endregion
    }
}

