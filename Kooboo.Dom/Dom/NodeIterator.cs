//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{

    /// <summary>
    /// NOTE: this implemented is not fully according to the lines from W3C, instead implement the description accordig to own understanding to meet our needs. 
    /// </summary>
    [Serializable]
    public class NodeIterator
    {

        public NodeIterator()
        {
            this.pointerBeforeReferenceNode = false;
        }

        private Node _root;
        /// <summary>
        /// The root of an object is itself, if its parent is null, or else it is the root of its parent.
        /// </summary>
        public Node root
        {
            get
            {
                if (_root != null)
                {
                    while (_root.parentNode != null)
                    {
                        _root = _root.parentNode;
                    }
                }
                return _root;
            }
            set
            {
                _root = value;
            }
        }

        /// <summary>
        /// Whether root will always go up the toppest parent or not. 
        /// </summary>
        public bool AllowChildAsRoot { get; set; }

        /// <summary>
        /// current node, the node that has been consumed. 
        /// </summary>
        public Node referenceNode { get; set; }

        public bool pointerBeforeReferenceNode { get; set; }

        public enumWhatToShow WhatToShow { get; set; }

        public NodeFilter filter { get; set; }

        public enumNodeFilterAcceptNode FilterNode(Node node)
        {
            // To filter node run these steps:
            //If active flag is set throw an "InvalidStateError".
            //if (this.Active)
            //{
            //    throw new Exception("InvalidStateError");
            //}

            //Let n be node's nodeType attribute value minus 1.
            //If the nth bit (where 0 is the least significant bit) of whatToShow is not set, return FILTER_SKIP.
            if (!this.WhatToShowCanShow(node))
            {
                return enumNodeFilterAcceptNode.FILTER_SKIP;
            }

            //If filter is null, return FILTER_ACCEPT.
            if (this.filter == null)
            {
                return enumNodeFilterAcceptNode.FILTER_ACCEPT;
            }

            //Set the active flag.
            //Let result be the return value of invoking filter.
            //Unset the active flag.
            //If an exception was thrown, re-throw the exception.
            //Return result.
            //this.Active = true;
            var result = this.filter.AcceptNode(node);
            //this.Active = false;

            return result;
        }

        private bool WhatToShowCanShow(Node node)
        {
            switch (node.nodeType)
            {
                case enumNodeType.ELEMENT:
                    return this.WhatToShow.HasFlag(enumWhatToShow.ELEMENT);

                case enumNodeType.ATTRIBUTE:
                    return this.WhatToShow.HasFlag(enumWhatToShow.ATTRIBUTE);

                case enumNodeType.TEXT:
                    return this.WhatToShow.HasFlag(enumWhatToShow.TEXT);

                case enumNodeType.CDATA_SECTION:
                    return this.WhatToShow.HasFlag(enumWhatToShow.CDATA_SECTION);

                case enumNodeType.ENTITY_REFERENCE:
                    return this.WhatToShow.HasFlag(enumWhatToShow.ENTITY_REFERENCE);

                case enumNodeType.ENTITY:
                    return this.WhatToShow.HasFlag(enumWhatToShow.ENTITY);

                case enumNodeType.PROCESSING_INSTRUCTION:
                    return this.WhatToShow.HasFlag(enumWhatToShow.PROCESSING_INSTRUCTION);

                case enumNodeType.COMMENT:
                    return this.WhatToShow.HasFlag(enumWhatToShow.COMMENT);

                case enumNodeType.DOCUMENT:
                    return this.WhatToShow.HasFlag(enumWhatToShow.DOCUMENT);

                case enumNodeType.DOCUMENT_TYPE:
                    return this.WhatToShow.HasFlag(enumWhatToShow.DOCUMENT_TYPE);

                case enumNodeType.DOCUMENT_FRAGMENT:
                    return this.WhatToShow.HasFlag(enumWhatToShow.DOCUMENT_FRAGMENT);

                case enumNodeType.NOTATION:
                    return this.WhatToShow.HasFlag(enumWhatToShow.NOTATION);

                default:
                    return true;
            }
        }


        public Node nextNode()
        {
            return Following();
        }

        public Node previousNode()
        {
            return Proceeding();
        }

        public void detach()
        {
            throw new NotImplementedException();
        }

        //public Node TraverseNode(TraverseType type)
        //{

        //    //  To traverse in direction direction run these steps:

        //    //Let node be the value of the referenceNode attribute.

        //    var node = this.referenceNode; 

        //    //Let before node be the value of the pointerBeforeReferenceNode attribute.
        //    var beforeNode = this.pointerBeforeReferenceNode;  

        //    //Run these substeps:
        //    substeps:

        //    //If direction is next
        //    if (type == TraverseType.Next)
        //    {
        //        //If before node is false, let node be the first node following node in the iterator collection. If there is no such node return null.
        //        if (!beforeNode)
        //        {


        //        }
        //        else
        //        {
        //            //If before node is true, set it to false.
        //            beforeNode = false;
        //        }
        //    }

        //    //If direction is previous
        //    //If before node is true, let node be the first node preceding node in the iterator collection. If there is no such node return null.

        //    //If before node is false, set it to true.

        //    //Filter node and let result be the return value.

        //    //If result is FILTER_ACCEPT, go to the next step in the overall set of steps.

        //    //Otherwise, run these substeps again.

        //    //Set the referenceNode attribute to node, set the pointerBeforeReferenceNode attribute to before node, and return node.

        //}

        /// <summary>
        /// return the node following current node. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node Following()
        {
            // deepest first. when there is a child nodes, always go down to child first. 
            // when there is no node any more, go back to parent, parent always has been consumed. 

            var node = this.referenceNode;

            if (node.hasChildNodes())
            {
                node = node.firstChild();

                var result = this.FilterNode(node);

                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.referenceNode = node;
                    return node;
                }
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    this.referenceNode = node;
                    return Following();
                }
                else
                {
                    // rejected. the same as all the sub has been consumed. 
                    /// must go silbing or parent now. 
                    // do nothing and go out to the "when there is no child any more. 

                }

            }
            return NextSibling(node);
        }

        /// <summary>
        /// skip the sub of this node and continue consume the next sibling or parent sibling. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node NextSibling(Node node)
        {

        // when there is no child any more. 
        checksibling:

            if (node.nextSibling() != null)
            {
                node = node.nextSibling();

                var result = this.FilterNode(node);

                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.referenceNode = node;
                    return node;
                }
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    this.referenceNode = node;
                    return Following();
                }
                else
                {
                    // when rejected, check sibling again. 
                    goto checksibling;
                }
            }

        checkparentSibling:


            // there is no sibling, check parent sibling. 
            /// go to any parent that has sibling. 
            node = getParentNextSibling(node);
            if (node != null)
            {
                var result = this.FilterNode(node);
                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.referenceNode = node;
                    return node;
                }
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    this.referenceNode = node;
                    return Following();
                }
                else
                {
                    // when rejected, check sibling again. 
                    goto checkparentSibling;
                }

            }

            return null;
        }

        public Node Proceeding()
        {
            // deepest first. when there is a child nodes, always go down to child first. 
            // when there is no node any more, go back to parent, parent always has been consumed. 

            var node = this.referenceNode;

            if (node.hasChildNodes())
            {
                node = node.lastChild();

                var result = this.FilterNode(node);

                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.referenceNode = node;
                    return node;
                }
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    this.referenceNode = node;
                    return Proceeding();
                }
                else
                {
                    // rejected. the same as all the sub has been consumed. 
                    /// must go silbing or parent now. 
                    // do nothing and go out to the "when there is no child any more. 

                }

            }

        // when there is no child any more. 
        checksibling:

            if (node.previousSibling() != null)
            {
                node = node.previousSibling();

                var result = this.FilterNode(node);

                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.referenceNode = node;
                    return node;
                }
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    this.referenceNode = node;
                    return Following();
                }
                else
                {
                    // when rejected, check sibling again. 
                    goto checksibling;
                }
            }

        checkparentSibling:

            // there is no sibling, check parent sibling. 
            /// go to any parent that has sibling. 
            node = getParentPreviousSibling(node);
            if (node != null)
            {
                var result = this.FilterNode(node);

                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.referenceNode = node;
                    return node;
                }
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    this.referenceNode = node;
                    return Proceeding();
                }
                else
                {
                    // when rejected, check sibling again. 
                    goto checkparentSibling;
                }
            }
            return null;
        }

        private Node getParentNextSibling(Node checknode)
        {
            Node parent = checknode.parentNode;

            if (parent == null)
            {
                return null;
            }

            Node nextsibling = parent.nextSibling();

            if (nextsibling != null)
            {
                return nextsibling;
            }
            else
            {
                return getParentNextSibling(parent);
            }
        }

        private Node getParentPreviousSibling(Node checknode)
        {
            Node parent = checknode.parentNode;

            if (parent == null)
            {
                return null;
            }

            Node previousSibling = parent.previousSibling();

            if (previousSibling != null)
            {
                return previousSibling;
            }
            else
            {
                return getParentPreviousSibling(parent);
            }
        }

    }




    public enum TraverseType
    {
        Next = 0,
        Previous = 1
    }


}
