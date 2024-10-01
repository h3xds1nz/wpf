// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description:
//      FixedTextBuilder contains heuristics to map fixed document elements
//      into stream of flow text
//

using System.Windows.Documents.DocumentStructures;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Collections;

namespace System.Windows.Documents
{
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

        public FixedDSBuilder(FixedPage fixedPage, StoryFragments storyFragments)
        {
            _nameHashTable = new Dictionary<string, NameHashFixedNode>();
            _fixedPage = fixedPage;
            _storyFragments = storyFragments;
        }

        public void BuildNameHashTable(string name, UIElement element, int indexToFixedNodes)
        {
            _nameHashTable.TryAdd(name, new NameHashFixedNode(element, indexToFixedNodes));
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
                    CreateFlowNodes(be);
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

            for (int i = 0; i < _visitedArray.Count; i++)
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

        private void AddFixedNodeInFlow(int index, UIElement element)
        {
            if (_visitedArray[index])
            {
                // this has already been added to the document structure
                // Debug.Assert(false, "An element is referenced in the document structure multiple times");
                return; // ignore this reference
            }
            FixedNode fn = _fixedNodes[index];

            if (element is null)
                element = _fixedPage.GetElement(fn) as UIElement;

            _visitedArray[index] = true;

            FixedSOMElement somElement = FixedSOMElement.CreateFixedSOMElement(_fixedPage, element, fn, -1, -1);
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
        private void CreateFlowNodes(BlockElement blockElement)
        {
            //
            // Break, NamedElement and SemanticBasicElement all derived from BlockElement.
            // Break element is ignored for now.
            //
            if (blockElement is NamedElement namedElement)
            {
                //
                // That is the NamedElement, it might use namedReference or HierachyReference, 
                // we need to construct FixedSOMElement list from this named element.
                //
                ConstructSomElement(namedElement);
            }
            else
            {
                if (blockElement is SemanticBasicElement sbe)
                {
                    //
                    // Add the start node in the flow array.
                    //
                    _flowBuilder.AddStartNode(blockElement.ElementType);

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
                        CreateFlowNodes(bElement);
                    }

                    //
                    // Add the end node in the flow array.
                    //
                    _flowBuilder.AddEndNode();
                }
            }
        }

        private void AddChildofFixedNodeinFlow(int[] childIndex)
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
                for (int i = startIndex + 1; i < _fixedNodes.Count; i++)
                {
                    fn = _fixedNodes[i];
                    if (fn.ComparetoIndex(childIndex) == 0)
                    {
                        AddFixedNodeInFlow(i, null);
                    }
                    else
                        break;
                }
            }
        }

        private void SpecialProcessing(SemanticBasicElement sbe)
        {
            if (sbe is ListItemStructure listItem && listItem.Marker != null)
            {
                if (_nameHashTable.TryGetValue(listItem.Marker, out NameHashFixedNode fen))
                {
                    _visitedArray[fen.Index] = true;
                }
            }
        }

        private void ConstructSomElement(NamedElement namedElement)
        {
            if (_nameHashTable.TryGetValue(namedElement.NameReference, out NameHashFixedNode fen))
            {
                if (fen.UIElement is Glyphs || fen.UIElement is Path || fen.UIElement is Image)
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

                        AddChildofFixedNodeinFlow(childIndex);
                    }
                }
            }
        }

        private readonly Dictionary<string, NameHashFixedNode> _nameHashTable;
        private readonly StoryFragments _storyFragments;
        private readonly FixedPage _fixedPage;

        private List<FixedNode> _fixedNodes;
        private BitArray _visitedArray;
        private FixedTextBuilder.FlowModelBuilder _flowBuilder;
    }
}

