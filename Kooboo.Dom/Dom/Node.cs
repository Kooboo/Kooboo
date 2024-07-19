//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{

    // http://www.w3.org/TR/dom/

    /// <summary>
    /// Node is an abstract interface and does not exist as node. It is used by all nodes (Document, DocumentFragment, DocumentType, Element, Text, ProcessingInstruction, and Comment).
    /// </summary>
    [Serializable]
    public abstract class Node : IDisposable
    {

        public enumNodeType nodeType;


        public string nodeName
        {
            get
            {
                switch (this.nodeType)
                {
                    case enumNodeType.ELEMENT:
                        {
                            Element element = (Element)this;
                            return element.tagName;
                        }
                    case enumNodeType.ATTRIBUTE:
                        return "#attribute";

                    case enumNodeType.TEXT:
                        return "#text";

                    case enumNodeType.CDATA_SECTION:
                        return "#cdata";

                    case enumNodeType.ENTITY_REFERENCE:
                        return "#entity_reference";

                    case enumNodeType.ENTITY:
                        return "#entity";

                    case enumNodeType.PROCESSING_INSTRUCTION:
                        {
                            ProcessingInstruction processinstruction = (ProcessingInstruction)this;
                            return processinstruction.target;
                        }
                    case enumNodeType.COMMENT:
                        return "#comment";

                    case enumNodeType.DOCUMENT:
                        return "#document";

                    case enumNodeType.DOCUMENT_TYPE:
                        return "document_type";

                    case enumNodeType.DOCUMENT_FRAGMENT:
                        return "#document_fragment";

                    case enumNodeType.NOTATION:
                        return "#notation";

                    default:
                        return "#unknown";
                }

            }

        }

        /// <summary>
        /// The baseURI attribute must return the associated base URL.
        /// </summary>
        public string baseURI;

        public string nodeValue
        {
            //CDATASection	content of the CDATA Section
            //Comment	content of the comment
            //Document	null
            //DocumentFragment	null
            //DocumentType	null
            //Element	null
            //NamedNodeMap	null
            //EntityReference	null
            //Notation	null
            //ProcessingInstruction	entire content excluding the target
            //Text	content of the text node

            get
            {
                switch (this.nodeType)
                {
                    case enumNodeType.ELEMENT:
                        return null;

                    case enumNodeType.ATTRIBUTE:
                        return null;
                    case enumNodeType.TEXT:
                        {
                            Text text = (Text)this;
                            return text.data;
                        }
                    case enumNodeType.CDATA_SECTION:
                        {
                            Text text = (Text)this;
                            return text.data;
                        }
                    case enumNodeType.ENTITY_REFERENCE:
                        return null;

                    case enumNodeType.ENTITY:
                        return null;
                    case enumNodeType.PROCESSING_INSTRUCTION:
                        {
                            throw new NotImplementedException();
                        }
                    case enumNodeType.COMMENT:
                        {
                            Comment comment = (Comment)this;
                            return comment.data;
                        }
                    case enumNodeType.DOCUMENT:
                        return null;

                    case enumNodeType.DOCUMENT_TYPE:
                        return null;

                    case enumNodeType.DOCUMENT_FRAGMENT:
                        return null;

                    case enumNodeType.NOTATION:
                        return null;

                    default:
                        return null;
                }


            }


        }

        private string _textcontent;
        public string textContent
        {
            get
            {

                if (string.IsNullOrEmpty(_textcontent))
                {
                    _textcontent = string.Empty;
                    _getTextContent(this, ref _textcontent);
                }
                return _textcontent;
            }

            //Note that while textContent gets the content of all elements, including <script> and <style> elements, the mostly equivalent IE-specific property, innerText, does not.
            //innerText is also aware of style and will not return the text of hidden elements, whereas textContent will.
            //As innerText is aware of CSS styling, it will trigger a reflow, whereas textContent will not.
        }

        private void _getTextContent(Node node, ref string content)
        {
            if (node.nodeType == enumNodeType.DOCUMENT)
            {
                content = string.Empty;
                return;
            }

            if (node.nodeType == enumNodeType.TEXT || node.nodeType == enumNodeType.CDATA_SECTION)
            {
                content = content + ((Text)node).data;
            }


            foreach (var item in node.childNodes.item)
            {
                _getTextContent(item, ref content);
            }
        }

        public Document ownerDocument;

        public Node parentNode { get; set; }

        /// <summary>
        /// A node's parent of type Element is known as a parent element. If the node has a parent of a different type, its parent element is null.
        /// </summary>
        public Element parentElement
        {
            get
            {
                if (_parentElement == null)
                {
                    if (parentNode != null && parentNode.nodeType == enumNodeType.ELEMENT)
                    {
                        _parentElement = (Element)parentNode;
                    }
                }
                return _parentElement;
            }
            set { _parentElement = value; }
        }
        private Element _parentElement;

        public NodeList childNodes = new NodeList();

        public bool hasChildNodes()
        {
            return (this.childNodes.length > 0);
        }

        public Node firstChild()
        {
            if (hasChildNodes())
            {
                return childNodes.item[0];
            }
            else
            {
                return null;
            }
        }

        public Node lastChild()
        {
            if (hasChildNodes())
            {
                int lastindex = childNodes.length - 1;
                return childNodes.item[lastindex];
            }
            else
            {
                return null;
            }
        }

        public virtual Node previousSibling()
        {
            if (this.siblingIndex == 0 || parentNode == null)
            {
                return null;
            }
            else
            {
                return parentNode.childNodes.item[this.siblingIndex - 1];
            }
        }

        public Node nextSibling()
        {
            if (this.parentNode == null)
            {
                return null;
            }

            int parentChildrenCount = this.parentNode.childNodes.length;

            // this is the max. 
            if (this.siblingIndex == (parentChildrenCount - 1))
            {
                return null;
            }

            if (this.siblingIndex > (parentChildrenCount - 1))
            {
                // it should never happen. 
                return null;
            }

            return this.parentNode.childNodes.item[this.siblingIndex + 1];

        }

        public virtual bool isEqualNode(Node node)
        {
            int thisstart = this.location.openTokenStartIndex;
            int thisend = this.location.endTokenEndIndex;

            int nodestart = node.location.openTokenStartIndex;
            int nodeend = node.location.endTokenEndIndex;

            if (thisstart > 0 && thisend > 0 && nodestart > 0 && nodeend > 0)
            {
                return (thisstart == nodestart && thisend == nodeend);
            }


            if (this.nodeName != node.nodeName || this.depth != node.depth || this.siblingIndex != node.siblingIndex)
            {
                return false;
            }

            Node currentParent = this.parentNode;
            Node otherParent = node.parentNode;

            while (currentParent != null && otherParent != null)
            {
                if (currentParent.nodeName != otherParent.nodeName || currentParent.depth != otherParent.depth || currentParent.siblingIndex != otherParent.siblingIndex)
                {
                    return false;
                }
                currentParent = currentParent.parentNode;
                otherParent = otherParent.parentNode;
            }

            if (currentParent != null || otherParent != null)
            {
                return false;
            }

            return true;

        }

        public bool contains(Node other)
        {
            if (other.depth <= this.depth)
            {
                return false;
            }

            foreach (var item in other.getNodePath(false))
            {
                if (item.isEqualNode(this))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// compare the node position, this is also possible to compare node with other document.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public enumNodeComparePosition compareDocumentPoistion(Node other)
        {
            if (this.isEqualNode(other))
            {
                return enumNodeComparePosition.DOCUMENT_POSITION_SAME_NODE;
            }

            var currentPath = this.getNodePath(false);
            var otherPath = other.getNodePath(false);
            int currentCount = currentPath.Count();
            int otherCount = otherPath.Count();

            if (currentCount == otherCount)
            {
                // possible: disconnected preceding, following.  

                for (int i = 0; i < currentCount; i++)
                {
                    if (!currentPath[i].isEqualNode(otherPath[i]))
                    {
                        return enumNodeComparePosition.DOCUMENT_POSITION_DISCONNECTED;
                    }
                }

                if (this.siblingIndex > other.siblingIndex)
                {
                    return enumNodeComparePosition.DOCUMENT_POSITION_PRECEDING;
                }
                else
                {
                    return enumNodeComparePosition.DOCUMENT_POSITION_FOLLOWING;
                }

            }

            else
            {
                // possible: contains or contain_by or disconnect. 

                if (currentCount > otherCount)
                {
                    // check containby
                    foreach (var item in currentPath)
                    {
                        if (item.isEqualNode(other))
                        {
                            return enumNodeComparePosition.DOCUMENT_POSITION_CONTAINED_BY;
                        }
                    }
                }
                else
                {
                    // check if it contains. 
                    foreach (var item in otherPath)
                    {
                        if (item.isEqualNode(this))
                        {
                            return enumNodeComparePosition.DOCUMENT_POSITION_CONTAINS;
                        }
                    }

                }

            }

            return enumNodeComparePosition.DOCUMENT_POSITION_DISCONNECTED;
        }

        public void normalize()
        {
            throw new NotImplementedException();
        }

        public Node cloneNode(bool deepcopy)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// insert a node before the position of other child  nodes.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <returns>return the result of pre-inserting node into the context object before child.</returns>
        public Node insertBefore(Node node, Node child)
        {
            if (child.parentNode == null || !this.isEqualNode(child.parentNode))
            {
                // the child is not a child of current node, exception. 
                throw new Exception("child is not a sub of current node.");
            }

            node.parentNode = this;
            node.siblingIndex = child.siblingIndex;
            node.depth = this.depth + 1;

            Node nextNode = child.nextSibling();

            child.siblingIndex = child.siblingIndex + 1;


            while (nextNode != null)
            {
                Node newnode = nextNode.nextSibling();
                nextNode.siblingIndex = nextNode.siblingIndex + 1;

                nextNode = newnode;
            }

            this.childNodes.item.Insert(child.siblingIndex, node);

            return node;

        }


        /// <summary>
        /// The appendChild(node) method must return the result of appending node to the context object.
        /// </summary>
        /// <param name="node"></param>
        /// <returns>The appendChild(node) method must return the result of appending node to the context object.</returns>
        public Node appendChild(Node node)
        {
            node.parentNode = this;
            node.siblingIndex = this.childNodes.length;  // the lasgest sibling = length -1;
            node.depth = this.depth + 1;

            this.childNodes.item.Add(node);
            return node;
        }

        /// <summary>
        /// The replaceChild(node, child) method must return the result of replacing child with node within the context object.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <returns>The replaceChild(node, child) method must return the result of replacing child with node within the context object.</returns>
        public Node replaceChild(Node node, Node child)
        {
            if (child.parentNode == null || !this.isEqualNode(child.parentNode))
            {
                // the child is not a child of current node, exception. 
                throw new Exception("child is not a sub of current node.");
            }

            int itemindex = child.siblingIndex;

            node.siblingIndex = itemindex;
            node.depth = this.depth + 1;
            node.parentNode = this;

            node.childNodes.item[itemindex] = node;

            return node;

        }

        /// <summary>
        /// The removeChild(child) method must return the result of pre-removing child from the context object.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public Node removeChild(Node child)
        {
            Node nextNode = child.nextSibling();

            while (nextNode != null)
            {
                Node newnode = nextNode.nextSibling();

                nextNode.siblingIndex = nextNode.siblingIndex - 1;

                nextNode = newnode;
            }

            this.childNodes.item.Remove(child);

            return child;
        }

        /// these are extremely useful, but did not find it in the w3c. 

        /// <summary>
        /// The depth of this node in the document. html = 0
        /// </summary>
        public int depth;

        /// <summary>
        /// the 0 base siblingindex of current under its parent. html= {depth=10, siblingindex =0}
        /// this is the same as the item[index] in the parentNode. 
        /// </summary>
        public int siblingIndex;


        public int ItemIndex { get; set; }

        /// <summary>
        /// the location of this node in the underlying file stream and calculated x.y position within the position. 
        /// </summary>
        public Location location;

        public Size size;


        public string InnerHtml
        {
            get
            {
                int start = this.location.openTokenEndIndex + 1;

                int len = this.location.endTokenStartIndex - this.location.openTokenEndIndex - 1;
                if (this.location.endTokenStartIndex == this.location.endTokenEndIndex)
                {
                    len = len + 1;
                }
                if (start < 1 || len < 1)
                {
                    return null;
                }

                else
                {
                    return this.ownerDocument.HtmlSource.Substring(start, len);
                }
            }
        }


        public string OuterHtml
        {
            get
            {
                int start = this.location.openTokenStartIndex;
                //if (this.location.endTokenStartIndex < start)
                //{
                //    start = this.location.endTokenStartIndex; 
                //}

                int end = this.location.endTokenEndIndex;
                if (this.location.openTokenEndIndex > end)
                {
                    end = this.location.openTokenEndIndex;
                }

                int len = end - start + 1;

                if (start < 0 || len < 1)
                {
                    return null;
                }
                else
                {
                    return this.ownerDocument.HtmlSource.Substring(start, len);
                }

            }
        }



        /// <summary>
        /// The list of nodes from first node till current node. The node navigation path. 
        /// </summary>
        /// <returns></returns>
        public List<Node> getNodePath(bool includeSelf)
        {
            List<Node> nodelist = new List<Node>();
            if (includeSelf)
            {
                nodelist.Add(this);
            }

            Node parent = this.parentNode;

            while (parent != null)
            {
                nodelist.Add(parent);
                parent = parent.parentNode;
            }

            nodelist.Reverse();

            return nodelist;
        }

        public virtual void Dispose()
        {
            this.ownerDocument = null;
            this.parentElement = null;
            this.parentNode = null;

            foreach (var item in childNodes.item)
            {
                item.Dispose();
            }

        }
    }
}
