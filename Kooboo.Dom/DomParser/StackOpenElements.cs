//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{
    /// <summary>
    /// stack of open elements
    /// </summary>
    public class StackOpenElements
    {

        TreeConstruction TreeConstruction;

        public StackOpenElements(TreeConstruction treeconstruction)
        {
            this.TreeConstruction = treeconstruction;
        }

        public List<Element> item = new List<Element>();

        public int length
        {
            get
            {
                return item.Count();
            }
        }

        /// <summary>
        /// The current node is the bottommost node in this stack of open elements.
        /// </summary>
        /// <returns></returns>
        public Element currentNode()
        {
            int index = length - 1;
            if (index >= 0)
            {
                return item[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// add/append a new open elements. 
        /// </summary>
        /// <param name="node"></param>
        public void push(Element node)
        {

            item.Add(node);
        }

        /// <summary>
        /// Only pop off the element if it is the last in the item list. 
        /// </summary>
        /// <param name="node"></param>
        public void popOffLast(Element node)
        {
            int index = length - 1;

            string tagName = node.tagName;

            if (this.TreeConstruction.IsSameDomElement(item[index], node))
            {

                if (TreeConstruction.CurrentProcessingToken.tagName == tagName)
                {
                    item[index].location.endTokenStartIndex = this.TreeConstruction.CurrentProcessingToken.startIndex;
                    item[index].location.endTokenEndIndex = this.TreeConstruction.CurrentProcessingToken.endIndex;
                }
                else
                {
                    int maxend = TreeConstruction.CurrentProcessingToken.startIndex - 1;

                    if (maxend > 0)
                    {
                        if (item[index].location.endTokenStartIndex == 0)
                        {
                            item[index].location.endTokenStartIndex = maxend;
                        }

                        if (item[index].location.endTokenEndIndex == 0)
                        {
                            item[index].location.endTokenEndIndex = maxend;
                        }

                    }
                }


                item.RemoveAt(index);
            }
        }

        /// <summary>
        /// get one off the last append node. 
        /// </summary>
        /// <param name="node"></param>
        public void popOff(Element node)
        {
            popOff(node.tagName);
        }

        public void popOff(string tagName)
        {

            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                if (item[i].tagName == tagName)
                {

                    if (TreeConstruction.CurrentProcessingToken.tagName == tagName)
                    {
                        item[i].location.endTokenStartIndex = this.TreeConstruction.CurrentProcessingToken.startIndex;
                        item[i].location.endTokenEndIndex = this.TreeConstruction.CurrentProcessingToken.endIndex;
                    }
                    else
                    {
                        int maxend = TreeConstruction.CurrentProcessingToken.startIndex - 1;

                        if (maxend > 0)
                        {
                            if (item[i].location.endTokenStartIndex == 0)
                            {
                                item[i].location.endTokenStartIndex = maxend;
                            }

                            if (item[i].location.endTokenEndIndex == 0)
                            {
                                item[i].location.endTokenEndIndex = maxend;
                            }

                        }
                    }

                    item.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>pop off every element,till && includes/exclude the input tagName.</summary>
        /// <param name="tagName"></param>
        /// <param name="selfIncluded"></param>
        public void popOffTill(string tagName, bool selfIncluded)
        {

            List<int> itemToRemoved = new List<int>();
            bool hastag = false;

            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                if (item[i].tagName == tagName)
                {
                    hastag = true;
                    if (selfIncluded)
                    {
                        itemToRemoved.Add(i);
                    }

                    break;
                }
                else
                {
                    itemToRemoved.Add(i);
                }
            }

            if (hastag)
            {
                foreach (var removeIndex in itemToRemoved)
                {
                    string currentTagName = item[removeIndex].tagName;
                    if (TreeConstruction.CurrentProcessingToken.tagName == currentTagName)
                    {
                        item[removeIndex].location.endTokenStartIndex = this.TreeConstruction.CurrentProcessingToken.startIndex;
                        item[removeIndex].location.endTokenEndIndex = this.TreeConstruction.CurrentProcessingToken.endIndex;
                    }
                    else
                    {
                        int maxend = TreeConstruction.CurrentProcessingToken.startIndex - 1;

                        if (maxend > 0)
                        {
                            if (item[removeIndex].location.endTokenStartIndex == 0)
                            {
                                item[removeIndex].location.endTokenStartIndex = maxend;
                            }

                            if (item[removeIndex].location.endTokenEndIndex == 0)
                            {
                                item[removeIndex].location.endTokenEndIndex = maxend;
                            }

                        }
                    }


                    item.RemoveAt(removeIndex);
                }
            }
        }

        public void popOffTillOneOf(bool selfIncluded, params string[] tagNames)
        {
            string foundTagName = null;

            List<int> itemToRemoved = new List<int>();

            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                if (item[i].tagName.isOneOf(tagNames))
                {
                    if (selfIncluded)
                    {
                        itemToRemoved.Add(i);
                    }
                    foundTagName = item[i].tagName;
                    break;
                }
                else
                {
                    itemToRemoved.Add(i);
                }
            }

            if (!string.IsNullOrEmpty(foundTagName))
            {

                foreach (var removeIndex in itemToRemoved)
                {
                    string currentTagName = item[removeIndex].tagName;
                    if (TreeConstruction.CurrentProcessingToken.tagName == currentTagName)
                    {
                        item[removeIndex].location.endTokenStartIndex = this.TreeConstruction.CurrentProcessingToken.startIndex;
                        item[removeIndex].location.endTokenEndIndex = this.TreeConstruction.CurrentProcessingToken.endIndex;
                    }
                    else
                    {
                        int maxend = TreeConstruction.CurrentProcessingToken.startIndex - 1;

                        if (maxend > 0)
                        {
                            if (item[removeIndex].location.endTokenStartIndex == 0)
                            {
                                item[removeIndex].location.endTokenStartIndex = maxend;
                            }

                            if (item[removeIndex].location.endTokenEndIndex == 0)
                            {
                                item[removeIndex].location.endTokenEndIndex = maxend;
                            }

                        }
                    }



                    item.RemoveAt(removeIndex);
                }

            }

        }

        /// <summary>
        /// The adjusted current node is the context element if the stack of open elements has only one element in it and the parser was created by the HTML fragment parsing algorithm; otherwise, the adjusted current node is the current node.
        /// </summary>
        public Element adjustedCurrentNode
        {
            get
            {
                return currentNode();
            }

        }

        public bool hasTag(string tagName)
        {
            for (int i = 0; i < length; i++)
            {
                if (item[i].tagName == tagName)
                {
                    return true;
                }
            }

            return false;
        }

        public bool hasElement(string tagName)
        {
            return hasTag(tagName);
        }

        public bool hasElement(Element element)
        {
            foreach (var oneitem in item)
            {
                if (this.TreeConstruction.IsSameDomElement(oneitem, element))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// An appropriate end tag token is an end tag token whose tag name matches the tag name of the last start tag to have been emitted from this tokenizer, if any. If no start tag has been emitted from this tokenizer, then no end tag token is appropriate.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public bool isAppropriateEndTag(string tagName)
        {
            Element current = currentNode();
            if (current == null)
            {
                return false;
            }
            else
            {
                if (current.tagName == tagName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// should be HTML, the first element. 
        /// </summary>
        /// <returns></returns>
        public Element topElement()
        {
            if (length == 0)
            {
                return new Element();
            }
            else
            {
                return item[0];
            }
        }

        /// <summary>
        /// Should be body
        /// </summary>
        /// <returns></returns>
        public Element secondElement()
        {
            if (length < 2)
            {
                return new Element();
            }
            else
            {
                return item[1];
            }
        }

        public Element firstElement()
        {
            return topElement();
        }

        public Element lastElement()
        {
            return currentNode();
        }

        public bool hasOneOfElementsInScope(ScopeType type, params string[] tagNames)
        {
            foreach (var item in tagNames)
            {
                if (hasElementInScope(item, type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool hasElementInScope(string tagName, ScopeType type)
        {
            // The stack of open elements is said to have an element target node in a specific scope consisting of a list of element types list when the following algorithm terminates in a match state:

            //Initialize node to be the current node (the bottommost node of the stack).

            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                Element element = item[i];

                if (element.tagName == tagName)
                {
                    //If node is the target node, terminate in a match state.
                    return true;
                }
                else
                {

                    if (type == ScopeType.inSelectScope)
                    {
                        // The stack of open elements is said to have a particular element in select scope when it has that element in the specific scope consisting of all element types except the following: 
                        //optgroup in the HTML namespace
                        //option in the HTML namespace
                        if (tagName != "optgroup" && tagName != "option")
                        {
                            return false;
                        }
                    }

                    else
                    {
                        //Otherwise, if node is one of the element types in list, terminate in a failure state.
                        foreach (var tagNameItem in scopeElements(type))
                        {
                            if (tagName == tagNameItem)
                            {
                                return false;
                            }
                        }
                    }


                }

                //Otherwise, set node to the previous entry in the stack of open elements and return to step 2. (This will never fail, since the loop will always terminate in the previous step if the top of the stack — an html element — is reached.)
            }

            return false;

        }

        private List<string> scopeElements(ScopeType type)
        {

            switch (type)
            {
                case ScopeType.inScope:
                    return inScope();

                case ScopeType.inListItemScope:
                    return inListItemScope();

                case ScopeType.inButtonScope:
                    return inButtonScope();

                case ScopeType.inTableScope:
                    return inTableScope();

                default:
                    return inScope();
            }
        }

        private List<string> _inscope;
        private List<string> inScope()
        {
            if (_inscope == null)
            {
                //The stack of open elements is said to have a particular element in scope when it has that element in the specific scope consisting of the following element types:

                //applet in the HTML namespace
                //caption in the HTML namespace
                //html in the HTML namespace
                //table in the HTML namespace
                //td in the HTML namespace
                //th in the HTML namespace
                //marquee in the HTML namespace
                //object in the HTML namespace
                //template in the HTML namespace
                //mi in the MathML namespace
                //mo in the MathML namespace
                //mn in the MathML namespace
                //ms in the MathML namespace
                //mtext in the MathML namespace
                //annotation-xml in the MathML namespace
                //foreignObject in the SVG namespace
                //desc in the SVG namespace
                //title in the SVG namespace
                _inscope = new List<string>();
                _inscope.Add("applet");
                _inscope.Add("caption");
                _inscope.Add("html");
                _inscope.Add("table");
                _inscope.Add("td");
                _inscope.Add("th");
                _inscope.Add("marquee");
                _inscope.Add("object");
                _inscope.Add("template");
                _inscope.Add("mi");
                _inscope.Add("mo");
                _inscope.Add("mn");
                _inscope.Add("ms");
                _inscope.Add("mtext");
                _inscope.Add("annotation-xml");
                _inscope.Add("foreignObject");
                _inscope.Add("desc");
                _inscope.Add("title");

            }
            return _inscope;

        }

        private List<string> _inListItemScope;
        private List<string> inListItemScope()
        {

            //The stack of open elements is said to have a particular element in list item scope when it has that element in the specific scope consisting of the following element types:
            //All the element types listed above for the has an element in scope algorithm.
            //ol in the HTML namespace
            //ul in the HTML namespace
            if (_inListItemScope == null)
            {
                _inListItemScope = new List<string>();
                foreach (var item in inScope())
                {
                    _inListItemScope.Add(item);
                }

                _inListItemScope.Add("ol");
                _inListItemScope.Add("ul");

            }
            return _inListItemScope;

        }

        private List<string> _inButtonScope;
        private List<string> inButtonScope()
        {
            //          The stack of open elements is said to have a particular element in button scope when it has that element in the specific scope consisting of the following element types:

            //All the element types listed above for the has an element in scope algorithm.
            //button in the HTML namespace
            if (_inButtonScope == null)
            {
                _inButtonScope = new List<string>();
                foreach (var item in inScope())
                {
                    _inButtonScope.Add(item);
                }

                _inButtonScope.Add("button");

            }
            return _inButtonScope;

        }


        private List<string> _inTableScope;
        private List<string> inTableScope()
        {
            // The stack of open elements is said to have a particular element in table scope when it has that element in the specific scope consisting of the following element types:

            //html in the HTML namespace
            //table in the HTML namespace
            //template in the HTML namespace
            if (_inTableScope == null)
            {
                _inTableScope = new List<string>();

                _inTableScope.Add("html");
                _inTableScope.Add("table");
                _inTableScope.Add("template");

            }
            return _inTableScope;
        }


        private List<string> _special;
        public List<string> Special()
        {
            if (_special == null)
            {

                //The following elements have varying levels of special parsing rules: HTML's address, applet, area, article, aside, base, basefont, bgsound, blockquote, body, br, button, caption, center, col, colgroup, dd, details, dir, div, dl, dt, embed, fieldset, figcaption, figure, footer, form, frame, frameset, h1, h2, h3, h4, h5, h6, head, header, hgroup, hr, html, iframe, img, input, isindex, li, link, listing, main, marquee, meta, nav, noembed, noframes, noscript, object, ol, p, param, plaintext, pre, script, section, select, source, style, summary, table, tbody, td, template, textarea, tfoot, th, thead, title, tr, track, ul, wbr, and xmp; MathML's mi, mo, mn, ms, mtext, and annotation-xml; and SVG's foreignObject, desc, and title.

                _special = new List<string>();

                string tagstring = "address, applet, area, article, aside, base, basefont, bgsound, blockquote, body, br, button, caption, center, col, colgroup, dd, details, dir, div, dl, dt, embed, fieldset, figcaption, figure, footer, form, frame, frameset, h1, h2, h3, h4, h5, h6, head, header, hgroup, hr, html, iframe, img, input, isindex, li, link, listing, main, marquee, meta, nav, noembed, noframes, noscript, object, ol, p, param, plaintext, pre, script, section, select, source, style, summary, table, tbody, td, template, textarea, tfoot, th, thead, title, tr, track, ul, wbr, xmp,mi, mo, mn, ms, mtext, annotation-xml, foreignObject, desc,title";

                string[] strArray = tagstring.Split(',');

                foreach (var item in strArray)
                {
                    _special.Add(item);
                }

            }

            return _special;
        }


        public void ClearStackBackToTableContext()
        {

            //When the steps above require the UA to clear the stack back to a table context, it means that the UA must, while the current node is not a table, template, or html element, pop elements from the stack of open elements.

            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                Element element = item[i];

                // if (element.tagName.isOneOf("table", "template", "html"))
                if (element.tagName == "table" || element.tagName == "template" || element.tagName == "html")
                {
                    return;
                }
                else
                {
                    popOff(element);
                }
            }

        }

        public void ClearStackBackToTableRowContext()
        {

            //When the steps above require the UA to clear the stack back to a table row context, it means that the UA must, while the current node is not a tr, template, or html element, pop elements from the stack of open elements.
            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                Element element = item[i];

                // if (element.tagName.isOneOf("tr", "template", "html"))
                if (element.tagName == "tr" || element.tagName == "template" || element.tagName == "html")
                {
                    return;
                }
                else
                {
                    popOff(element);
                }
            }
        }


        public void ClearStackBackToTableBodyContext()
        {

            //When the steps above require the UA to clear the stack back to a table body context, it means that the UA must, while the current node is not a tbody, tfoot, thead, template, or html element, pop elements from the stack of open elements.

            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                Element element = item[i];

                if (element.tagName.isOneOf("tbody", "tfoot", "thead", "template", "html"))
                {

                    return;
                }
                else
                {
                    popOff(element);
                }
            }
        }

    }

    public enum ScopeType
    {
        inScope = 1,
        inListItemScope = 2,
        inButtonScope = 3,
        inTableScope = 4,
        inSelectScope = 5,
    }

}
