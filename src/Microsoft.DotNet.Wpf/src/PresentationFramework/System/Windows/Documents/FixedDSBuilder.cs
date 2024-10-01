// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description:
//      FixedTextBuilder contains heuristics to map fixed document elements
//      into stream of flow text
//

namespace System.Windows.Documents
{
    using MS.Internal.Documents;
    using System.Windows.Controls;     
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Markup;
    using System.Windows.Shapes;
    using System.Windows.Documents.DocumentStructures;
    using Ds=System.Windows.Documents.DocumentStructures;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    //=====================================================================
    /// <summary>
    /// FixedTextBuilder contains heuristics to map fixed document elements
    /// into stream of flow text.
    /// </summary>
    internal sealed class FixedDSBuilder
    {
        private readonly struct NameHashFixedNode
        {
            public readonly int Index { get; }
            public readonly UIElement UIElement { get; }

            internal NameHashFixedNode(UIElement uiElement, int index)
            {
                UIElement = uiElement;
                Index = index;
            }
        }

        public FixedDSBuilder(FixedPage fp, StoryFragments sf)
        {
            _nameHashTable = new Dictionary<string, NameHashFixedNode>();
            _fixedPage = fp;
            _storyFragments = sf;
        }

        public void BuildNameHashTable(string Name, UIElement e, int indexToFixedNodes)
        {
            if (!_nameHashTable.ContainsKey(Name))
            {
                _nameHashTable.Add(Name, new NameHashFixedNode(e,indexToFixedNodes));
            }
        }

        public StoryFragments StoryFragments
        {
            get
            {
                return _storyFragments;
            }
        }

        public void ConstructFlowNodes(FixedTextBuilder.FlowModelBuilder flowBuilder, List<FixedNode> fixedNodes)
        {
            //
            // Initialize some variables inside this class.
            //
            _fixedNodes = fixedNodes;
            _visitedArray = new BitArray(fixedNodes.Count);
            _flowBuilder = flowBuilder;

            List<StoryFragment> StoryFragmentList = StoryFragments.StoryFragmentList;
            foreach (StoryFragment storyFragme in StoryFragmentList)
            {
                List<BlockElement> blockElementList = storyFragme.BlockElementList;
                foreach (BlockElement be in blockElementList)
                {
                    _CreateFlowNodes(be);
                }
            }
            //
            // After all the document structure referenced elements, we will go over
            // the FixedNodes again to take out all the un-referenced elements and put 
            // them in the end of the story. 
            //
            // Add the start node in the flow array.
            //
            _flowBuilder.AddStartNode(FixedElement.ElementType.Paragraph);

            for (int i = 0; i< _visitedArray.Count; i++ )
            {
                if (_visitedArray[i] == false)
                {
                    AddFixedNodeInFlow(i, null);
                }
            }

            _flowBuilder.AddEndNode();

            //Add any undiscovered hyperlinks at the end of the page
            _flowBuilder.AddLeftoverHyperlinks();
        }

        private void AddFixedNodeInFlow(int index, UIElement e)
        {
            if (_visitedArray[index])
            {
                // this has already been added to the document structure
                // Debug.Assert(false, "An element is referenced in the document structure multiple times");
                return; // ignore this reference
            }
            FixedNode fn = _fixedNodes[index];

            if (e is null)
                e = _fixedPage.GetElement(fn) as UIElement;

            _visitedArray[index] = true;

            FixedSOMElement somElement = FixedSOMElement.CreateFixedSOMElement(_fixedPage, e, fn, -1, -1);
            if (somElement != null)
            {
                _flowBuilder.AddElement(somElement);
            }
        }

        /// <summary>
        /// This function will create the flow node corresponding to this BlockElement.
        /// This function will call itself recursively.
        /// </summary>
        /// <param name="be"></param>
        private void _CreateFlowNodes(BlockElement be)
        {
            //
            // Break, NamedElement and SemanticBasicElement all derived from BlockElement.
            // Break element is ignored for now.
            //
            NamedElement ne = be as NamedElement;
            if (ne != null)
            {
                //
                // That is the NamedElement, it might use namedReference or HierachyReference, 
                // we need to construct FixedSOMElement list from this named element.
                //
                ConstructSomElement(ne);
            }
            else
            {
                SemanticBasicElement sbe = be as SemanticBasicElement;
                if (sbe != null)
                {
                    //
                    // Add the start node in the flow array.
                    //
                    _flowBuilder.AddStartNode(be.ElementType);

                    //Set the culture info on this node
                    XmlLanguage language = (XmlLanguage)_fixedPage.GetValue(FrameworkElement.LanguageProperty);
                    _flowBuilder.FixedElement.SetValue(FixedElement.LanguageProperty, language);

                    SpecialProcessing(sbe);
                    //
                    // Enumerate all the childeren under this element.
                    //
                    List<BlockElement> blockElementList = sbe.BlockElementList;
                    foreach (BlockElement bElement in blockElementList)
                    {
                        _CreateFlowNodes(bElement);
                    }

                    //
                    // Add the end node in the flow array.
                    //
                    _flowBuilder.AddEndNode();
                }
            }
        }

        private void AddChildofFixedNodeinFlow(int[] childIndex, NamedElement ne)
        {
            // Create a fake FixedNode to help binary search.
            FixedNode fn = FixedNode.Create(_fixedNodes[0].Page, childIndex);
            // Launch the binary search to find the matching FixedNode 
            int index = _fixedNodes.BinarySearch(fn);

            if (index >= 0)
            {
                int startIndex;
                // Search backward for the first Node in this scope
                for (startIndex = index - 1; startIndex >= 0; startIndex--)
                {
                    fn = _fixedNodes[startIndex];
                    if (fn.ComparetoIndex(childIndex) != 0)
                    {
                        break;
                    }
                }

                // Search forward to add all the nodes in order.
                for (int i = startIndex+1; i < _fixedNodes.Count; i++)
                {
                    fn = _fixedNodes[i];
                    if (fn.ComparetoIndex(childIndex) == 0)
                    {
                        AddFixedNodeInFlow(i, null);
                    }
                    else break;
                }
            }
        }

        private void SpecialProcessing(SemanticBasicElement sbe)
        {
            ListItemStructure listItem = sbe as ListItemStructure;
            if (listItem != null && listItem.Marker != null)
            {
                if (_nameHashTable.TryGetValue(listItem.Marker, out NameHashFixedNode fen) == true)
                {
                    _visitedArray[fen.Index] = true;
                }
            }
        }

        private void ConstructSomElement(NamedElement ne)
        {
            if (_nameHashTable.TryGetValue(ne.NameReference, out NameHashFixedNode fen) == true)
            {
                if (fen.UIElement is Glyphs || fen.UIElement is Path ||
                    fen.UIElement is Image)
                {
                    // Elements that can't have childrent
                    AddFixedNodeInFlow(fen.Index, fen.UIElement);
                }
                else
                {
                    if (fen.UIElement is Canvas)
                    {
                        // We need to find all the fixed nodes inside the scope of 
                        // this grouping element, add all of them in the arraylist.
                        int[] childIndex = _fixedPage._CreateChildIndex(fen.UIElement);

                        AddChildofFixedNodeinFlow(childIndex, ne);
                    }
                }
            }
        }

        private StoryFragments _storyFragments;
        private FixedPage _fixedPage;  
        private List<FixedNode> _fixedNodes;
        private BitArray _visitedArray;
        private Dictionary<string, NameHashFixedNode> _nameHashTable;
        private FixedTextBuilder.FlowModelBuilder _flowBuilder;
   }
}

