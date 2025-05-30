﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Description: Defines XmlNodeComparer object, used to sort a view of data produced by an XmlDataSource.

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

using System.Xml;

namespace MS.Internal.Data
{
    /// <summary>
    /// The XmlNodeComparer is used to sort a view of data produced by an XmlDataSource.
    /// </summary>
    internal class XmlNodeComparer : IComparer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sortParameters"> array of sort parameters </param>
        /// <param name="namespaceManager"> namespace manager, to control queries</param>
        /// <param name="culture">culture to use for comparisons</param>
        internal XmlNodeComparer(SortDescriptionCollection sortParameters, XmlNamespaceManager namespaceManager, CultureInfo culture)
        {
            _sortParameters = sortParameters;
            _namespaceManager = namespaceManager;
            _culture = culture ?? CultureInfo.InvariantCulture;
        }

        int IComparer.Compare(object o1, object o2)
        {
            int result = 0;
            XmlNode node1 = o1 as XmlNode;
            XmlNode node2 = o2 as XmlNode;

            if (node1 == null)
                return -1;
            if (node2 == null)
                return +1;

            for (int k = 0; k < _sortParameters.Count; ++k)
            {
                string valueX = SystemXmlExtension.SelectStringValue(node1, _sortParameters[k].PropertyName, _namespaceManager);
                string valueY = SystemXmlExtension.SelectStringValue(node2, _sortParameters[k].PropertyName, _namespaceManager);

                result = String.Compare(valueX, valueY, false, _culture);
                if (_sortParameters[k].Direction == ListSortDirection.Descending)
                    result = -result;

                if (result != 0)
                    break;
            }

            return result;
        }

        private SortDescriptionCollection  _sortParameters;
        private XmlNamespaceManager  _namespaceManager;
        private CultureInfo _culture;
    }
}


