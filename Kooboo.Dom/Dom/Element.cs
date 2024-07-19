//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom.CSS;

namespace Kooboo.Dom
{
    [Serializable]
    public class Element : Node
    {
        public Element()
        {
            nodeType = enumNodeType.ELEMENT;
        }

        public string namespaceURI;
        public string prefix;
        public string localName;

        /// <summary>
        /// If namespace prefix is not null, returns the concatenation of namespace prefix, ":", and local name. 
        /// Otherwise it returns the local name. (The return value is uppercased in an HTML document.)
        /// </summary>
        public string tagName;

        private string _id;

        public string id
        {
            get
            {

                if (string.IsNullOrEmpty(_id))
                {
                    if (this.hasAttribute("id"))
                    {
                        _id = this.getAttribute("id");
                    }
                    else
                    {
                        _id = string.Empty;
                    }

                    return _id;
                }
                else
                {
                    return _id;
                }
            }
            set
            {
                _id = value;
            }

        }

        private string _classname;
        /// <summary>
        /// the "class" content attribute value. 
        /// </summary>
        public string className
        {
            get
            {

                if (string.IsNullOrEmpty(_classname))
                {
                    if (this.hasAttribute("class"))
                    {
                        _classname = this.getAttribute("class");
                    }
                    else
                    {
                        _classname = string.Empty;
                    }

                    return _classname;
                }
                else
                {
                    return _classname;
                }

            }
            set
            {
                _classname = value;
            }
        }

        private DOMTokenList _classlist;

        /// <summary>
        /// The classList attribute must return the associated DOMTokenList object representing the context object's classes.
        /// </summary>
        public DOMTokenList classList
        {
            get
            {

                if (_classlist == null)
                {
                    _classlist = new DOMTokenList();

                    if (this.hasAttribute("class"))
                    {
                        string classvalue = this.getAttribute("class");

                        string[] listofvalue = classvalue.Split(' ');

                        foreach (var item in listofvalue)
                        {
                            _classlist.add(item);
                        }
                    }
                }

                return _classlist;
            }
            set
            {

                _classlist = value;
            }

        }

        private List<Attr> _attribute;

        public List<Attr> attributes
        {
            get
            {
                if (_attribute == null)
                {
                    _attribute = new List<Attr>();
                }
                return _attribute;
            }
            set
            {
                _attribute = value;
            }
        }

        public string getAttribute(string name)
        {
            //The getAttribute(name) method must run these steps:
            //If the context object is in the HTML namespace and its node document is an HTML document, let name be converted to ASCII lowercase.
            //Return the value of the first attribute in the context object's attribute list whose name is name, and null otherwise.
            if (_attribute == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            name = name.ToLower().Trim();
            foreach (var item in attributes)
            {
                if (item.name == name)
                {
                    if (item.value == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return item.value;
                    }
                }
            }

            return string.Empty;
        }

        public string getAttributeNS(string @namespace, string localName)
        {
            throw new NotImplementedException();
        }

        public void setAttribute(string name, string value)
        {
            name = name.ToLower();
            foreach (var item in attributes)
            {
                if (item.name == name)
                {
                    item.value = value;
                    return;
                }
            }

            Attr newAttribute = new Attr();
            newAttribute.name = name;
            newAttribute.value = value;
            attributes.Add(newAttribute);
        }

        public void setAttributeNS(string @namespace, string name, string value)
        {
            throw new NotImplementedException();
        }

        public void removeAttribute(string name)
        {
            name = name.ToLower();

            foreach (var item in attributes)
            {
                if (item.name == name)
                {
                    attributes.Remove(item);
                    return;
                }
            }
        }

        public void removeAttributeNS(string @namespace, string localName)
        {
            throw new NotImplementedException();
        }

        public bool hasAttribute(string name)
        {
            if (_attribute == null)
            {
                return false;
            }

            name = name.ToLower();
            foreach (var item in attributes)
            {
                if (item.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool hasAttributeNS(string @namespace, string localName)
        {
            throw new NotImplementedException();
        }

        public bool matches(string selectors)
        {
            return selectorMatch.Match(this, selectors);
        }

        public bool matches(List<simpleSelector> selectorlist)
        {
            return selectorMatch.Match(this, selectorlist);
        }

        public Element getOneElementByTagName(string tagname)
        {
            string lowername = tagname.ToLower();
            return _getOneElementByTagName(this, lowername);
        }

        private Element _getOneElementByTagName(Element topElement, string tagname)
        {
            if (topElement.tagName.ToLower() == tagname)
            {
                return topElement;
            }

            foreach (var item in topElement.childNodes.item)
            {
                if (item.nodeType == enumNodeType.ELEMENT)
                {
                    var result = _getOneElementByTagName(item as Element, tagname);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public HTMLCollection getElementsByTagName(string tagName)
        {
            if (tagName.Contains(","))
            {
                return getElementsByTagNames(tagName.Split(','));
            }
            else
            {
                HTMLCollection collection = new HTMLCollection();

                _getElementByTagName(this, collection, tagName.Trim().ToLower());
                return collection;
            }
        }

        private void _getElementByTagName(Node topElement, HTMLCollection collection, string tagname)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;
                if (tagname.Equals("*") || element.tagName == tagname)
                {
                    collection.Add(element);
                }
            }

            foreach (var item in topElement.childNodes.item)
            {
                _getElementByTagName(item, collection, tagname);
            }
        }




        public HTMLCollection getElementsByTagNames(params string[] tagNames)
        {
            for (int i = 0; i < tagNames.Length; i++)
            {
                tagNames[i] = tagNames[i].Trim().ToLower();
            }

            HTMLCollection collection = new HTMLCollection();

            _getElementByTagNames(this, collection, tagNames);

            return collection;
        }

        private void _getElementByTagNames(Node topElement, HTMLCollection collection, params string[] tagnames)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;
                if (element.tagName.isOneOf(tagnames))
                {
                    collection.Add(element);
                }
            }

            foreach (var item in topElement.childNodes.item)
            {
                _getElementByTagNames(item, collection, tagnames);
            }
        }

        public Element getElementsById(string id)
        {
            HTMLCollection collection = new HTMLCollection();

            return _getElementById(this, id);
        }

        private Element _getElementById(Node topElement, string id)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                if (element.hasAttribute("id"))
                {
                    string idvalue = element.getAttribute("id");
                    if (idvalue == id)
                    {
                        return element;
                    }
                }
            }

            foreach (var item in topElement.childNodes.item)
            {
                Element returnelement = _getElementById(item, id);
                if (returnelement != null)
                {
                    return returnelement;
                }
            }

            return null;
        }


        public HTMLCollection getElementsByAttribute(string AttributeName)
        {
            HTMLCollection collection = new HTMLCollection();

            _getElementByAttribute(this, collection, AttributeName.ToLower());
            return collection;
        }


        private Element _getElementByIndex(Element TopElement, int TokenIndex, bool IsStartIndex)
        {
            if ((IsStartIndex && TopElement.location.openTokenStartIndex == TokenIndex) || (!IsStartIndex && TopElement.location.endTokenEndIndex == TokenIndex))
            {
                return TopElement;
            }
            foreach (var item in TopElement.childNodes.item)
            {
                if (item != null && item.nodeType == enumNodeType.ELEMENT)
                {
                    Element element = item as Element;
                    if (element != null)
                    {
                        var result = _getElementByIndex(element, TokenIndex, IsStartIndex);
                        if (result != null)
                        { return result; }
                    }
                }
            }

            return null;
        }

        public Element getElementByIndex(int TokenIndex, bool IsStartIndex = true)
        {
            if (TokenIndex <= 0)
            {
                return null;
            }
            return _getElementByIndex(this, TokenIndex, IsStartIndex);
        }

        private void _getElementByAttribute(Node topElement, HTMLCollection collection, string AttributeName)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;
                if (element.hasAttribute(AttributeName))
                {
                    collection.Add(element);
                }
            }

            foreach (var item in topElement.childNodes.item)
            {
                _getElementByAttribute(item, collection, AttributeName);
            }
        }


        public HTMLCollection getElementsByAttributeValues(string AttributeName, string AttributeValue)
        {

            if (string.IsNullOrEmpty(AttributeValue))
            {
                return getElementsByAttribute(AttributeName);
            }

            HTMLCollection collection = new HTMLCollection();

            _getElementByAttributeValue(this, collection, AttributeName.ToLower(), AttributeValue.ToLower());
            return collection;
        }


        private void _getElementByAttributeValue(Node topElement, HTMLCollection collection, string AttributeName, string AttributeValue)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                string value = element.getAttribute(AttributeName);

                if (!string.IsNullOrEmpty(value) && value.ToLower() == AttributeValue)
                {
                    collection.Add(element);
                }

            }

            foreach (var item in topElement.childNodes.item)
            {
                _getElementByAttributeValue(item, collection, AttributeName, AttributeValue);
            }
        }



        public HTMLCollection getElementsByTagNameNS(string @namespace, string localName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get Elements by class name. Class name is case sensitive. 
        /// </summary>
        /// <param name="classNames"></param>
        /// <returns></returns>
        public HTMLCollection getElementsByClassName(string classNames)
        {
            HTMLCollection collection = new HTMLCollection();

            _getElementByClassName(this, collection, classNames);

            return collection;
        }

        private void _getElementByClassName(Node topElement, HTMLCollection collection, string classnames)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;

                if (element.hasAttribute("class"))
                {
                    string classvalue = element.getAttribute("class");

                    if (!string.IsNullOrEmpty(classvalue))
                    {
                        string[] classlist = classnames.Split(' ');

                        bool matched = true;

                        foreach (var item in classlist)
                        {
                            if (!classvalue.Contains(item))
                            {
                                matched = false;
                                break;
                            }
                        }

                        if (matched)
                        {
                            collection.Add(element);
                        }
                    }
                }
            }

            foreach (var item in topElement.childNodes.item)
            {
                _getElementByClassName(item, collection, classnames);
            }
        }

        public override bool isEqualNode(Node node)
        {
            if (this.location.openTokenStartIndex != 0 && this.location.endTokenEndIndex != 0)
            {
                return (this.location.openTokenStartIndex == node.location.openTokenStartIndex && this.location.endTokenEndIndex == node.location.endTokenEndIndex);
            }

            if (node.nodeType != enumNodeType.ELEMENT)
            {
                return false;
            }

            Element element = (Element)node;

            if (this.depth != element.depth || this.siblingIndex != element.siblingIndex || this.tagName != element.tagName)
            {
                return false;
            }

            var xparent = this.parentElement;
            var yparent = element.parentElement;

            while (true)
            {
                if (xparent == null && yparent == null)
                {
                    return true;
                }
                if (xparent == null || yparent == null)
                {
                    return false;
                }

                if (xparent.tagName == "body" && yparent.tagName == "body")
                {
                    return true;
                }

                if (xparent.tagName == "body" || yparent.tagName == "body")
                {
                    return false;
                }

                if (xparent.siblingIndex != yparent.siblingIndex || xparent.tagName != yparent.tagName)
                {
                    return false;
                }

                xparent = xparent.parentElement;
                yparent = yparent.parentElement;
            }

        }

        public HTMLCollection Select(string CSSSelector)
        {
            List<simpleSelector> selectorList = SelectorParser.parseSelectorGroup(CSSSelector);
            return getElementByCSSSelector(selectorList);
        }

        public HTMLCollection Select(List<simpleSelector> selectorList)
        {
            return getElementByCSSSelector(selectorList);
        }

        private HTMLCollection getElementByCSSSelector(List<simpleSelector> selectorList)
        {

            HTMLCollection collection = new HTMLCollection();

            _getElementByCSSSelector(this, collection, selectorList);

            return collection;
        }

        private void _getElementByCSSSelector(Node topElement, HTMLCollection collection, List<simpleSelector> selectorList)
        {
            if (topElement.nodeType == enumNodeType.ELEMENT)
            {
                Element element = (Element)topElement;
                if (selectorMatch.Match(element, selectorList))
                {
                    collection.Add(element);
                }
            }

            foreach (var item in topElement.childNodes.item)
            {
                _getElementByCSSSelector(item, collection, selectorList);
            }
        }


        private CSSStyleDeclaration _rawComputedStyle;
        public CSSStyleDeclaration RawComputedStyle
        {
            get
            {
                if (_rawComputedStyle == null)
                {
                    _rawComputedStyle = new CSSStyleDeclaration();

                    foreach (var item in StyleRules)
                    {
                        _rawComputedStyle.merge(item.style);
                    }

                    if (this.hasAttribute("style"))
                    {
                        string stringvalue = this.getAttribute("style");

                        if (!string.IsNullOrEmpty(stringvalue))
                        {

                            CSSStyleDeclaration inlinestyle = CSSSerializer.deserializeDeclarationBlock(stringvalue);

                            _rawComputedStyle.merge(inlinestyle);
                        }

                    }
                }

                return _rawComputedStyle;
            }
            set
            {
                _rawComputedStyle = value;
            }
        }

        /// <summary>
        /// matached cssrules will be append to this.
        /// </summary> 
        private List<CSSStyleRule> _styleRules;
        public List<CSSStyleRule> StyleRules
        {
            get
            {
                if (_styleRules == null)
                {
                    _styleRules = new List<CSSStyleRule>();
                }
                return _styleRules;

            }
            set
            {
                _styleRules = value;
            }
        }


        public override void Dispose()
        {
            this.StyleRules = null;
            this.RawComputedStyle = null;

            this.attributes.Clear();
            this.attributes = null;
            this.classList.item.Clear();
            this.classList = null;

            this.className = null;
            base.Dispose();
        }



    }
}
