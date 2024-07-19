//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using Kooboo.Dom.CSS;

namespace Kooboo.Dom
{

    /// <summary>
    /// see:  http://www.w3.org/TR/dom/#document
    /// </summary>
    [Serializable]
    public class Document : Node
    {

        public Document()
        {
            nodeType = enumNodeType.DOCUMENT;
        }

        public DOMImplementation implementation;

        public string URL;

        public string documentURI;

        private string _baseurl;
        /// <summary>
        /// get the base href tag or document url when base is not defined. 
        /// </summary>
        /// <returns></returns>
        public string getBaseUrl()
        {
            if (string.IsNullOrEmpty(_baseurl))
            {

                string baseUri = this.baseURI;

                if (!string.IsNullOrEmpty(baseUri))
                {
                    _baseurl = baseUri;
                }


                if (!string.IsNullOrEmpty(_baseurl) && _baseurl.StartsWith("http"))
                {
                    return _baseurl;
                }

                _baseurl = this.URL;

                if (!string.IsNullOrEmpty(_baseurl) && _baseurl.StartsWith("http"))
                {
                    return _baseurl;
                }

                _baseurl = this.documentURI;

                if (!string.IsNullOrEmpty(_baseurl) && _baseurl.StartsWith("http"))
                {
                    return _baseurl;
                }
            }

            return _baseurl;

        }


        public new string baseURI
        {
            get
            {
                var basehref = this.documentElement.getOneElementByTagName("base");
                if (basehref == null)
                {
                    return null;
                }
                return basehref.getAttribute("href");
            }
        }

        /// <summary>
        /// The compatMode attribute must return "BackCompat" if the context object is in quirks mode, and "CSS1Compat" otherwise.
        /// </summary>
        public string compatMode = "CSS1Compat";
        public void setQuirksMode()
        {
            this.compatMode = "BackCompat";
        }

        public bool isQuirksMode
        {
            get
            {
                return (this.compatMode == "BackCompat");
            }
        }

        public string contentType;

        public DocumentType doctype;

        /// <summary>
        /// Returns the Element that is the root element of the document (for example, the <html> element for HTML documents).
        /// </summary>
        public Element documentElement;

        public Element body
        {
            get
            {
                foreach (var item in documentElement.childNodes.item)
                {
                    if (item.nodeName == "body")
                    {
                        return (Element)item;
                    }
                }
                return null;
            }
        }

        public Element head
        {
            get
            {
                foreach (var item in documentElement.childNodes.item)
                {
                    if (item.nodeName == "head")
                    {
                        return (Element)item;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// comma seperated tagNames
        /// </summary>
        /// <param name="tagName">comma seperated tagNames</param>
        /// <returns></returns>
        public HTMLCollection getElementsByTagName(string tagName)
        {
            return this.documentElement.getElementsByTagName(tagName);
        }

        public HTMLCollection getElementsByTagNameNS(string @namespace, string localName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get elements based on one or more classnames.
        /// </summary>
        /// <param name="classnames">comma seperated classnames.</param>
        /// <returns></returns>
        public HTMLCollection getElementsByClassName(string classnames)
        {
            return this.documentElement.getElementsByClassName(classnames);
        }

        public HTMLCollection getElementsByCSSSelector(string CSSSelector)
        {
            return Select(CSSSelector);
        }

        public HTMLCollection Select(string CSSSelector)
        {
            return this.documentElement.Select(CSSSelector);
        }

        public Element getElementById(string id)
        {
            return this.documentElement.getElementsById(id);
        }

        public Element getElementByIndex(int TokenIndex, bool IsStartIndex = true)
        {
            return this.documentElement.getElementByIndex(TokenIndex, IsStartIndex);
        }

        public HTMLCollection getElementByAttribute(string AttributeName)
        {
            return this.documentElement.getElementsByAttribute(AttributeName);
        }

        public HTMLCollection getElementByAttributeValue(string AttributeName, string AttributeValue)
        {
            return this.documentElement.getElementsByAttributeValues(AttributeName, AttributeValue);
        }

        public Element createElement(string localName)
        {
            return createElementNS(string.Empty, localName);
        }

        public Element createElementNS(string @namespace, string qualifiedName)
        {
            Element element = new Element();
            element.ownerDocument = this;
            element.tagName = qualifiedName;
            element.namespaceURI = @namespace;

            return element;
        }

        public DocumentFragment createDocumentFragment()
        {
            throw new NotImplementedException();
        }

        public Text createTextNode(string data)
        {
            Text text = new Text(data);

            text.ownerDocument = this;

            return text;
        }

        public NodeIterator createNodeIterator(Node root, enumWhatToShow whatToShow, NodeFilter filter)
        {
            //The createNodeIterator(root, whatToShow, filter) method must run these steps:
            //Create a NodeIterator object.
            //Set root and initialize the referenceNode attribute to the root argument.
            //Initialize the pointerBeforeReferenceNode attribute to true.
            //Set whatToShow to the whatToShow argument.
            //Set filter to filter.
            //Return the newly created NodeIterator object. 

            NodeIterator iterator = new NodeIterator();
            iterator.filter = filter;
            iterator.root = root;
            iterator.referenceNode = root;
            iterator.WhatToShow = whatToShow;
            return iterator;
        }


        public Comment createComment(string data)
        {
            Comment comment = new Comment();
            comment.ownerDocument = this;
            //comment.data = data;
            comment.appendData(data);
            return comment;
        }

        public ProcessingInstruction createProcessingInstruction(string target, string data)
        {
            throw new NotImplementedException();
        }

        public Node importNode(Node node, bool deepcopy)
        {
            throw new NotImplementedException();
        }

        public Node adoptNode(Node node)
        {
            throw new NotImplementedException();
        }

        public Event createEvent(string @interface)
        {
            throw new NotImplementedException();
        }

        public Range createRange()
        {
            throw new NotImplementedException();
        }


        public TreeWalker createTreeWalker(Node root, enumWhatToShow whatToShow, NodeFilter filter)
        {
            throw new NotImplementedException();
        }


        public bool iframeSrcDoc = false;


        public StyleSheetList StyleSheets = new StyleSheetList();


        private HTMLCollection _links;

        /// <summary>
        /// The links property returns a collection of all <area> elements and <a> elements in a document with a value for the href attribute.
        /// </summary>
        public HTMLCollection Links
        {
            get
            {
                if (_links == null)
                {
                    _links = new HTMLCollection();

                    foreach (var item in this.getElementsByTagName("a").item)
                    {
                        if (item.hasAttribute("href"))
                        {
                            string hrefvalue = item.getAttribute("href");
                            if (!string.IsNullOrEmpty(hrefvalue))
                            {
                                _links.Add(item);
                            }
                        }
                    }

                    foreach (var areaitem in this.getElementsByTagName("area").item)
                    {

                        if (areaitem.hasAttribute("href"))
                        {
                            string hrefvalue = areaitem.getAttribute("href");
                            if (!string.IsNullOrEmpty(hrefvalue))
                            {
                                _links.Add(areaitem);
                            }
                        }

                    }

                }
                return _links;
            }

        }

        /// <summary>
        /// document.images returns a collection of the images in the current HTML document.
        /// </summary>
        public HTMLCollection images
        {
            get
            {
                return this.getElementsByTagName("img");
            }
        }

        /// <summary>
        /// forms returns a collection (an HTMLCollection) of the form elements within the current document.
        /// </summary>
        public HTMLCollection forms
        {
            get
            {
                return this.getElementsByTagName("form");
            }

        }


        /// <summary>
        /// parse and download the stylesheets and make them available at StyleSheets. 
        /// </summary>
        public void ParseStyleSheet()
        {
            HTMLCollection styletags = this.getElementsByTagName("link, style");

            HTMLCollection availablesheets = new HTMLCollection();

            foreach (var item in styletags.item)
            {
                if (item.tagName == "style")
                {

                    availablesheets.Add(item);
                }

                else if (item.hasAttribute("type"))
                {
                    if (item.getAttribute("type").ToLower().Contains("css"))
                    {
                        availablesheets.Add(item);
                    }
                }
                else if (item.hasAttribute("rel"))
                {
                    if (item.getAttribute("rel").ToLower().Contains("stylesheet"))
                    {
                        availablesheets.Add(item);
                    }
                }
            }

            foreach (var item in availablesheets.item)
            {
                if (item.tagName == "link")
                {
                    string href = item.getAttribute("href");

                    if (string.IsNullOrEmpty(href))
                    {
                        continue;
                    }

                    string absoluteUrl = PathHelper.combine(this.getBaseUrl(), href);

                    string cssText = Loader.LoadCss(absoluteUrl);

                    if (string.IsNullOrEmpty(cssText))
                    {
                        continue;
                    }

                    CSSStyleSheet newStyleSheet = CSSParser.ParseCSSStyleSheet(cssText, absoluteUrl, true);
                    newStyleSheet.ownerNode = item;

                    if (newStyleSheet != null)
                    {
                        newStyleSheet.ownerNode = item;

                        string media = item.getAttribute("media");
                        if (!string.IsNullOrEmpty(media))
                        {
                            string[] medialist = media.Split(',');
                            foreach (var mediaitem in medialist)
                            {
                                newStyleSheet.Medialist.appendMedium(mediaitem);
                            }
                        }
                        this.StyleSheets.appendStyleSheet(newStyleSheet);
                    }

                }

                else if (item.tagName == "style")
                {
                    string cssText = item.InnerHtml;

                    CSSStyleSheet newStyleSheet = CSSParser.ParseCSSStyleSheet(cssText, this.getBaseUrl(), true);

                    newStyleSheet.ownerNode = item;

                    string media = item.getAttribute("media");
                    if (!string.IsNullOrEmpty(media))
                    {
                        string[] medialist = media.Split(',');
                        foreach (var mediaitem in medialist)
                        {
                            newStyleSheet.Medialist.appendMedium(mediaitem);
                        }
                    }
                    this.StyleSheets.appendStyleSheet(newStyleSheet);

                }

            }

            hasParseCSS = true;
        }

        public bool hasParseCSS = false;

        /// <summary>
        /// this apply style for media query = all. 
        /// TO BE Improved.
        /// </summary>
        public void ApplyStyleSheet(string mediadeviceinfo)
        {
            if (!hasParseCSS && StyleSheets.item.Count() == 0)
            {
                ParseStyleSheet();
            }

            foreach (var item in this.StyleSheets.item)
            {
                CSSStyleSheet stylesheet = item as CSSStyleSheet;
                if (stylesheet != null)
                {
                    this.ApplyCssRules(stylesheet.cssRules, mediadeviceinfo);
                }
            }

        }

        public void ApplyStyleSheet()
        {
            this.ApplyStyleSheet(string.Empty);
        }

        /// <summary>
        /// apply css rules to current document. 
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="rulelist"></param>
        /// <param name="mediadeviceInfo"></param>
        public void ApplyCssRules(CSS.CSSRuleList rulelist, string mediadeviceInfo)
        {
            if (rulelist == null)
            {
                return;
            }

            foreach (var item in rulelist.item)
            {
                if (item.type == CSS.enumCSSRuleType.STYLE_RULE)
                {

                    CSS.CSSStyleRule stylerule = item as CSS.CSSStyleRule;

                    foreach (var elemntitem in this.Select(stylerule.selectorText).item)
                    {
                        elemntitem.StyleRules.Add(stylerule);
                    }
                }
                else if (item.type == CSS.enumCSSRuleType.MEDIA_RULE)
                {
                    CSS.CSSMediaRule mediarule = item as CSS.CSSMediaRule;

                    if (string.IsNullOrEmpty(mediadeviceInfo))
                    {
                        ApplyCssRules(mediarule.cssRules, string.Empty);
                    }
                    else
                    {
                        if (MediaQuery.isMatch(mediarule.media, mediadeviceInfo))
                        {
                            ApplyCssRules(mediarule.cssRules, string.Empty);
                        }
                    }
                }
                else if (item.type == enumCSSRuleType.IMPORT_RULE)
                {
                    CSS.CSSImportRule importrule = item as CSS.CSSImportRule;

                    if (importrule.stylesheet != null && importrule.stylesheet.cssRules != null)
                    {
                        if (MediaQuery.isMatch(importrule.media, mediadeviceInfo))
                        {
                            ApplyCssRules(importrule.stylesheet.cssRules, mediadeviceInfo);
                        }
                    }
                }

            }
        }

        public void ApplyCssText(string cssText)
        {
            if (!string.IsNullOrEmpty(cssText))
            {
                var stylesheet = CSSParser.ParseCSSStyleSheet(cssText, string.Empty, true);

                this.ApplyCssRules(stylesheet.cssRules, string.Empty);
            }
        }

        /// <summary>
        /// The html source code that attached with this dom.
        /// </summary>
        public string HtmlSource;

        public override void Dispose()
        {
            this.HtmlSource = null;

            this.documentElement.Dispose();

            this.StyleSheets = null;

            base.Dispose();
        }
    }
}
