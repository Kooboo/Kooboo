//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    /// <summary>
    /// this Tree Walker has not been fully implemented and tested yet. 
    /// </summary>
    [Serializable]
    public class TreeWalker
    {
        public TreeWalker(Node root)
        {
            this.root = root;
            this.currentNode = root;
            this.WhatToShow = enumWhatToShow.All;
        }

        internal bool Active { get; set; }

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

        public enumWhatToShow WhatToShow;

        public NodeFilter filter;

        public enumNodeFilterAcceptNode FilterNode(Node node)
        {
            // To filter node run these steps:
            //If active flag is set throw an "InvalidStateError".
            if (this.Active)
            {
                throw new Exception("InvalidStateError");
            }

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
            this.Active = true;
            var result = this.filter.AcceptNode(node);
            this.Active = false;

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

        public Node currentNode;

        public Node parentNode()
        {
            //The parentNode() method must run these steps:
            //Let node be the value of the currentNode attribute.
            //While node is not null and is not root, run these substeps:
            //Let node be node's parent.
            //If node is not null and filtering node returns FILTER_ACCEPT, then set the currentNode attribute to node, return node.
            //Return null.

            var node = this.currentNode;
            if (node != null && !node.Equals(this.root))
            {
                node = node.parentNode;
                if (node != null && this.filter.AcceptNode(node) == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
            }

            return null;
        }


        public Node firstChild()
        {
            return firstChild(this.currentNode);
        }

        public Node firstChild(Node node)
        {
            return TraverseChildren(TraverseType.First, node);
        }

        public Node lastChild(Node node)
        {
            return TraverseChildren(TraverseType.Last, node);
        }

        public Node lastChild()
        {
            return lastChild(this.currentNode);
        }

        public Node TraverseChildren(TraverseType type, Node currentnode)
        {
            //To traverse children of type type, run these steps:

            //Let node be the value of the currentNode attribute.
            var node = currentnode;

            //Set node to node's first child if type is first, and node's last child if type is last.
            if (type == TraverseType.First && node.childNodes.item.Count > 0)
            {
                node = node.childNodes.item[0];
            }
            else if (type == TraverseType.Last && node.childNodes.item.Count > 0)
            {
                int count = node.childNodes.item.Count;
                node = node.childNodes.item[count - 1];
            }
            else
            {
                node = null;
            }

        //Main: While node is not null, run these substeps:

        main:

            if (node != null)
            {
                //Filter node and let result be the return value.
                var result = FilterNode(node);

                //If result is FILTER_ACCEPT, then set the currentNode attribute to node and return node.
                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
                //If result is FILTER_SKIP, run these subsubsteps:
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    //Let child be node's first child if type is first, and node's last child if type is last.
                    Node child;
                    if (type == TraverseType.First && node.childNodes.item.Count > 0)
                    {
                        child = node.childNodes.item[0];
                    }
                    else if (type == TraverseType.Last && node.childNodes.item.Count > 0)
                    {
                        int count = node.childNodes.item.Count;
                        child = node.childNodes.item[count - 1];
                    }
                    else
                    {
                        child = null;
                    }

                    //If child is not null, set node to child and goto Main.
                    if (child != null)
                    {
                        node = child;
                        goto main;
                    }
                }


                //While node is not null, run these subsubsteps:

                if (node != null)
                {

                    //Let sibling be node's next sibling if type is first, and node's previous sibling if type is last.
                    var sibling = this.nextSibling(node);

                    //If sibling is not null, set node to sibling and goto Main.
                    if (sibling != null)
                    {
                        node = sibling;
                        goto main;
                    }

                    //Let parent be node's parent.
                    var parent = node.parentNode;

                    //If parent is null, parent is root, or parent is currentNode attribute's value, return null.
                    if (parent == null || parent.Equals(this.root) || parent.Equals(this.currentNode))
                    {
                        return null;
                    }
                    else
                    {
                        //Otherwise, set node to parent.
                        node = parent;
                    }
                }


            }

            //Return null.
            return null;

        }


        public Node previousSibling(Node node)
        {
            return TraverseSibling(TraverseType.Previous, node);
        }

        public Node previousSibling()
        {
            return previousSibling(this.currentNode);
        }

        public Node nextSibling(Node node)
        {
            return TraverseSibling(TraverseType.Next, node);
        }

        public Node nextSibling()
        {
            return nextSibling(this.currentNode);
        }


        public Node TraverseSibling(TraverseType type, Node currentnode)
        {
            // To traverse siblings of type type run these steps:
            //Let node be the value of the currentNode attribute.

            var node = currentnode;

            //If node is root, return null.

            if (node.Equals(this.root))
            {
                return null;
            }
            Node sibling;
        //Run these substeps:

        substeps:

            //Let sibling be node's next sibling if type is next, and node's previous sibling if type is previous.

            if (type == TraverseType.Next)
            {
                sibling = node.nextSibling();
            }
            else if (type == TraverseType.Previous)
            {
                sibling = node.previousSibling();
            }
            else
            {
                // will not run.
                sibling = null;
            }

            //While sibling is not null, run these subsubsteps:


            if (sibling != null)
            {
                //Set node to sibling.
                node = sibling;
                //Filter node and let result be the return value.

                var result = this.FilterNode(node);

                //If result is FILTER_ACCEPT, then set the currentNode attribute to node and return node.
                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
                else if (result == enumNodeFilterAcceptNode.FILTER_SKIP)
                {
                    //Set sibling to node's first child if type is next, and node's last child if type is previous.
                    if (type == TraverseType.Next)
                    {
                        sibling = node.firstChild();
                    }
                    else if (type == TraverseType.Previous)
                    {
                        sibling = node.lastChild();
                    }
                }

                if (result == enumNodeFilterAcceptNode.FILTER_REJECT || sibling == null)
                {
                    //If result is FILTER_REJECT or sibling is null, then set sibling to node's next sibling if type is next, and node's previous sibling if type is previous.

                    if (type == TraverseType.Next)
                    {
                        sibling = node.nextSibling();
                    }
                    else if (type == TraverseType.Previous)
                    {
                        sibling = node.previousSibling();
                    }


                }
            }

            node = node.parentNode;
            //Set node to its parent.

            //If node is null or is root, return null.
            if (node == null || node.Equals(this.root))
            {
                return null;
            }

            //Filter node and if the return value is FILTER_ACCEPT, then return null.
            if (this.FilterNode(node) == enumNodeFilterAcceptNode.FILTER_ACCEPT)
            {
                return null;
            }
            //Run these substeps again.
            goto substeps;

        }

        public Node PreviousNode()
        {
            return previousNode(this.currentNode);
        }

        public Node previousNode(Node currentnode)
        {
            //Let node be the value of the currentNode attribute.
            var node = currentnode;
            Node sibling;

            //While node is not root, run these substeps:
            if (node.Equals(this.root))
            {
                //Let sibling be the previous sibling of node.
                sibling = node.previousSibling();

                //While sibling is not null, run these subsubsteps:

                if (sibling != null)
                {

                    //Set node to sibling.
                    node = sibling;

                    //Filter node and let result be the return value.
                    var result = this.FilterNode(node);

                    if (result != enumNodeFilterAcceptNode.FILTER_REJECT && node.childNodes.item.Count > 0)
                    {
                        //While result is not FILTER_REJECT and node has a child, set node to its last child and then filter node and set result to the return value.
                        node = node.lastChild();

                        result = this.FilterNode(node);

                    }
                    //If result is FILTER_ACCEPT, then set the currentNode attribute to node and return node.

                    if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                    {
                        this.currentNode = node;
                        return node;
                    }

                    //Set sibling to the previous sibling of node.
                    sibling = node.previousSibling();
                }

                //If node is root or node's parent is null, return null.
                if (node.Equals(this.root) || node.parentNode == null)
                {
                    return null;
                }

                //Set node to its parent.

                node = node.parentNode;

                //Filter node and if the return value is FILTER_ACCEPT, then set the currentNode attribute to node and return node.
                if (this.FilterNode(node) == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
            }

            //Return null.
            return null;

        }

        public Node nextNode()
        {
            return nextNode(this.currentNode);
        }

        public Node nextNode(Node currentnode)
        {
            //  Let node be the value of the currentNode attribute.
            var node = currentnode;
            //Let result be FILTER_ACCEPT.

            var result = enumNodeFilterAcceptNode.FILTER_ACCEPT;

            //Run these substeps:
            //substeps:

            //While result is not FILTER_REJECT and node has a child, run these subsubsteps:
            if (result != enumNodeFilterAcceptNode.FILTER_REJECT && node.childNodes.item.Count > 0)
            {
                //Set node to its first child.

                node = node.firstChild();

                //Filter node and set result to the return value.
                result = this.FilterNode(node);

                //If result is FILTER_ACCEPT, then set the currentNode attribute to node and return node.

                if (result == enumNodeFilterAcceptNode.FILTER_ACCEPT)
                {
                    this.currentNode = node;
                    return node;
                }
            }


            //If a node is following node and is not following root, set node to the first such node.

            //Otherwise, return null.

            //Filter node and set result to the return value.

            //If result is FILTER_ACCEPT, then set the currentNode attribute to node and return node.

            //Run these substeps again.

            return null;

        }


        public enum TraverseType
        {
            First = 0,
            Last = 1,
            Previous = 2,
            Next = 3
        }



    }
}
