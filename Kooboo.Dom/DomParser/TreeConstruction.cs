//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kooboo.Dom
{
    public class TreeConstruction
    {
        private enumInsertionMode insertionMode;
        private enumInsertionMode originalInsertionMode;
        public Document doc;

        public StackOpenElements openElements;
        private StackTemplateMode templateMode;
        private ElementPointer elementPointer;
        public ActiveFormattingElementList activeFormatingElements;

        private List<HtmlToken> pendingTableCharacterTokens = new List<HtmlToken>();

        private Tokenizer tokenizer;

        public bool fosterParent;
        public bool scripting;

        public bool frameset_ok;

        public bool fragmentParsing;

        public bool stop;
        /// <summary>
        /// for fragment parsing.
        /// </summary>
        public Element context;  // for fragment. 

        private HtmlToken _currentToken;

        internal bool EnableErrorLogging;

        private Dictionary<int, string> _Errors;
        internal Dictionary<int, string> Errors
        {
            get
            {
                if (_Errors == null)
                {
                    _Errors = new Dictionary<int, string>();
                }
                return _Errors;
            }
            set { _Errors = value; }
        }

        public TreeConstruction()
        {

            insertionMode = enumInsertionMode.Initial;
            doc = new Document();
            doc.depth = -1;
            doc.siblingIndex = -1;
            openElements = new StackOpenElements(this);
            templateMode = new StackTemplateMode();
            activeFormatingElements = new ActiveFormattingElementList(this);
            elementPointer = new ElementPointer();

            fosterParent = false;
            scripting = false;
            frameset_ok = true;

            fragmentParsing = false;
            stop = false;

            context = new Element();
        }

        public HtmlToken _nexttoken;
        /// <summary>
        /// look up next token. 
        /// </summary>
        /// <returns></returns>
        private HtmlToken nextToken()
        {
            _nexttoken = tokenizer.ReadNextToken();
            return _nexttoken;
        }

        private void ignoreNextToken()
        {
            _nexttoken = null;
        }

        public Document Parse(string htmlText)
        {
            this.fragmentParsing = false;

            tokenizer = new Tokenizer(htmlText, this);
            _currentToken = tokenizer.ReadNextToken();

            while (_currentToken != null)
            {

                ProcessToken(_currentToken);

                if (stop)
                {
                    break;
                }

                if (_nexttoken != null)
                {
                    _currentToken = _nexttoken;
                    ignoreNextToken();
                }
                else
                {
                    _currentToken = tokenizer.ReadNextToken();
                }
            }

            openElements.item.Clear();
            activeFormatingElements.item.Clear();

            doc.documentElement.depth = 0;
            doc.documentElement.siblingIndex = doc.childNodes.length;

            ReOrganizeDepthSibling(doc.documentElement);

            return doc;
        }

        public NodeList ParseFragment(string input)
        {
            return ParseFragment(input, this.context);
        }


        public NodeList ParseFragment(string input, Element contextElement)
        {
            if (contextElement == null)
            {
                contextElement = new Element();
                contextElement.tagName = "fragment";
                this.context = contextElement;
            }
            else
            {
                this.context = contextElement;
            }

            this.fragmentParsing = true;

            //   8.4 Parsing HTML fragments

            //The following steps form the HTML fragment parsing algorithm. The algorithm optionally takes as input an Element node, referred to as the context element, which gives the context for the parser, as well as input, a string to parse, and returns a list of zero or more nodes.

            //Parts marked fragment case in algorithms in the parser section are parts that only occur if the parser was created for the purposes of this algorithm (and with a context element). The algorithms have been annotated with such markings for informational purposes only; such markings have no normative weight. If it is possible for a condition described as a fragment case to occur even when the parser wasn't created for the purposes of handling this algorithm, then that is an error in the specification.

            //Create a new Document node, and mark it as being an HTML document.


            //If there is a context element, and the Document of the context element is in quirks mode, then let the Document be in quirks mode. Otherwise, if there is a context element, and the Document of the context element is in limited-quirks mode, then let the Document be in limited-quirks mode. Otherwise, leave the Document in no-quirks mode.

            //Create a new HTML parser, and associate it with the just created Document node.

            tokenizer = new Tokenizer(input, this);

            //If there is a context element, run these substeps:

            if (contextElement != null)
            {

                //Set the state of the HTML parser's tokenization stage as follows:

                //If it is a title or textarea element
                if (contextElement.tagName.isOneOf("title", "textarea"))
                {
                    //Switch the tokenizer to the RCDATA state.
                    tokenizer.ParseState = enumParseState.RCDATA;
                }
                //If it is a style, xmp, iframe, noembed, or noframes element
                else if (contextElement.tagName.isOneOf("style", "xmp", "iframe", "noembed", "noframes"))
                {
                    //Switch the tokenizer to the RAWTEXT state.
                    tokenizer.ParseState = enumParseState.RAWTEXT;
                }
                //If it is a script element
                else if (contextElement.tagName == "script")
                {
                    //Switch the tokenizer to the script data state.
                    tokenizer.ParseState = enumParseState.Script;
                }
                //If it is a noscript element
                else if (contextElement.tagName == "noscript")
                {
                    //If the scripting flag is enabled, switch the tokenizer to the RAWTEXT state. Otherwise, leave the tokenizer in the data state.
                    if (this.scripting)
                    {
                        tokenizer.ParseState = enumParseState.RAWTEXT;
                    }
                }
                //If it is a plaintext element
                else if (contextElement.tagName == "plaintext")
                {
                    //Switch the tokenizer to the PLAINTEXT state.
                    tokenizer.ParseState = enumParseState.Plaintext;
                }
                else
                {
                    //Otherwise
                    //Leave the tokenizer in the data state.
                    tokenizer.ParseState = enumParseState.DATA;
                }
            }



            //For performance reasons, an implementation that does not report errors and that uses the actual state machine described in this specification directly could use the PLAINTEXT state instead of the RAWTEXT and script data states where those are mentioned in the list above. Except for rules regarding parse errors, they are equivalent, since there is no appropriate end tag token in the fragment case, yet they involve far fewer state transitions.

            //Let root be a new html element with no attributes. 
            //Append the element root to the Document node created above.


            //Set up the parser's stack of open elements so that it contains just the single element root.
            this.openElements.push(contextElement);

            //If the context element is a template element, push "in template" onto the stack of template insertion modes so that it is the new current template insertion mode.
            if (this.context.tagName == "template")
            {
                //TODO:
            }


            //Reset the parser's insertion mode appropriately.
            resetInsertionMode();

            //The parser will reference the context element as part of that algorithm.

            //Set the parser's form element pointer to the nearest node to the context element that is a form element (going straight up the ancestor chain, and including the element itself, if it is a form element), if any. (If there is no such form element, the form element pointer keeps its initial value, null.)

            //Place into the input stream for the HTML parser just created the input. The encoding confidence is irrelevant.

            //Start the parser and let it run until it has consumed all the characters just inserted into the input stream.

            _currentToken = tokenizer.ReadNextToken();

            while (_currentToken != null)
            {

                ProcessToken(_currentToken);

                if (stop)
                {
                    break;
                }

                if (_nexttoken != null)
                {
                    _currentToken = _nexttoken;
                    ignoreNextToken();
                }
                else
                {
                    _currentToken = tokenizer.ReadNextToken();
                }
            }

            openElements.item.Clear();
            activeFormatingElements.item.Clear();


            //If there is a context element, return the child nodes of root, in tree order.

            //Otherwise, return the children of the Document object, in tree order.

            return this.context.childNodes;

        }


        /// <summary>
        /// re calculate the sibling index of elements. 
        /// </summary>
        /// <param name="element"></param>
        private void ReOrganizeDepthSibling(Kooboo.Dom.Node element)
        {
            for (int i = 0; i < element.childNodes.length; i++)
            {
                element.childNodes.item[i].depth = element.depth + 1;
                element.childNodes.item[i].siblingIndex = i;
                ReOrganizeDepthSibling(element.childNodes.item[i]);
            }
        }

        private void ProcessToken(HtmlToken token)
        {
            switch (insertionMode)
            {
                case enumInsertionMode.Initial:
                    Initial(token);
                    return;

                case enumInsertionMode.beforeHtml:
                    beforeHtml(token);
                    return;

                case enumInsertionMode.beforeHead:
                    beforeHead(token);
                    return;

                case enumInsertionMode.inHead:
                    inHead(token);
                    return;

                case enumInsertionMode.inHeadNoScript:
                    InHeadNoScript(token);
                    return;

                case enumInsertionMode.afterHead:
                    afterHead(token);
                    return;

                case enumInsertionMode.inBody:
                    inBody(token);
                    return;

                case enumInsertionMode.text:
                    Text(token);
                    return;

                case enumInsertionMode.inTable:
                    inTable(token);
                    return;

                case enumInsertionMode.inTableText:
                    inTableText(token);
                    return;

                case enumInsertionMode.inCaption:
                    inCaption(token);
                    return;

                case enumInsertionMode.inColumnGroup:
                    inColumnGroup(token);
                    return;

                case enumInsertionMode.inTableBody:
                    inTableBody(token);
                    return;

                case enumInsertionMode.inRow:
                    inRow(token);
                    return;

                case enumInsertionMode.inCell:
                    inCell(token);
                    return;

                case enumInsertionMode.inSelect:
                    inSelect(token);
                    return;

                case enumInsertionMode.inSelectInTable:
                    inSelectInTable(token);
                    return;

                case enumInsertionMode.inTemplate:
                    inTemplate(token);
                    return;

                case enumInsertionMode.afterBody:
                    afterBody(token);
                    return;

                case enumInsertionMode.inFrameset:
                    inFrameset(token);
                    return;

                case enumInsertionMode.afterFrameset:
                    afterFrameset(token);
                    return;

                case enumInsertionMode.afterAfterBody:
                    afterAfterBody(token);
                    return;

                case enumInsertionMode.afterAfterFrameset:
                    afterAfterFrameset(token);
                    return;
                default:
                    break;
            }

        }

        public HtmlToken CurrentProcessingToken
        {
            get
            {
                if (_nexttoken != null)
                {
                    return _nexttoken;
                }
                else
                {
                    return _currentToken;
                }
            }
        }

        private void stopParsing()
        {
            stop = true;
        }

        private void ClosePElement()
        {
            //When the steps above say the user agent is to close a p element, it means that the user agent must run the following steps:
            //Generate implied end tags, except for p elements.
            generateImpliedEndTags("p");
            //If the current node is not a p element, then this is a parse error.

            if (openElements.currentNode().tagName != "p")
            {
                onError("closing P elements, found open tags inside.");
            }

            //Pop elements from the stack of open elements until a p element has been popped from the stack.
            openElements.popOffTill("p", true);
        }

        private void onError(string errorMessage, [CallerMemberName] string memberName = null)
        {
            if (this.EnableErrorLogging && !string.IsNullOrEmpty(errorMessage))
            {
                string err = null;
                if (string.IsNullOrEmpty(memberName))
                {
                    err = this.CurrentProcessingToken.type.ToString() + " " + this.CurrentProcessingToken.tagName + ", " + errorMessage;
                }
                else
                {
                    err = this.CurrentProcessingToken.type.ToString() + " " + this.CurrentProcessingToken.tagName + ", " + memberName + ", " + errorMessage;
                }

                this.Errors[this.tokenizer._readIndex] = err;
            }
        }

        /// <summary>
        /// The adjusted place to insert a new node. 
        /// </summary>
        /// <returns></returns>
        private insertionLocation appropriatePlaceNewNode(Element overrideTarget)
        {

            //  The appropriate place for inserting a node, optionally using a particular override target, is the position in an element returned by running the following steps:

            Element target;
            //If there was an override target specified, then let target be the override target.
            if (overrideTarget != null)
            {
                target = overrideTarget;
            }
            else
            {
                //Otherwise, let target be the current node.
                target = openElements.currentNode();
            }

            //Determine the adjusted insertion location using the first matching steps from the following list:

            //If foster parenting is enabled and target is a table, tbody, tfoot, thead, or tr element
            //Foster parenting happens when content is misnested in tables.
            if (fosterParent && target.tagName.isOneOf("table", "tbody", "tfoot", "thead", "tr"))
            {
                //Run these substeps:

                //Let last template be the last template element in the stack of open elements, if any.
                Element lastTemplate = null;
                int templatei = 0;
                for (int i = openElements.length - 1; i >= 0; i--)
                {
                    if (openElements.item[i].tagName == "template")
                    {
                        lastTemplate = openElements.item[i];
                        templatei = i;
                        break;
                    }
                }

                //Let last table be the last table element in the stack of open elements, if any.
                Element lastTable = null;
                int tablei = 0;
                for (int i = openElements.length - 1; i >= 0; i--)
                {
                    if (openElements.item[i].tagName == "table")
                    {
                        lastTable = openElements.item[i];
                        tablei = i;
                        break;
                    }
                }


                //If there is a last template and either there is no last table, or there is one, but last template is lower (more recently added) than last table in the stack of open elements, then: let adjusted insertion location be inside last template's template contents, after its last child (if any), and abort these substeps.
                if (lastTemplate != null && (lastTable == null || templatei > tablei))
                {
                    //TODO: what is template's template contents. Check it. 
                    return new insertionLocation() { partentElement = lastTemplate, insertAt = -1 };
                }

                //If there is no last table, then let adjusted insertion location be inside the first element in the stack of open elements (the html element), after its last child (if any), and abort these substeps. (fragment case)
                if (lastTable == null)
                {
                    return new insertionLocation() { partentElement = openElements.topElement(), insertAt = -1 };
                }

                //If last table has a parent element, then let adjusted insertion location be inside last table's parent element, immediately before last table, and abort these substeps.
                if (lastTable.parentElement != null)
                {
                    Element parentElement = lastTable.parentElement;

                    for (int i = 0; i < parentElement.childNodes.length - 1; i++)
                    {
                        if (parentElement.childNodes.item[i].isEqualNode(lastTable))
                        {
                            return new insertionLocation() { partentElement = parentElement, insertAt = i };
                        }
                    }

                    return new insertionLocation() { partentElement = parentElement, insertAt = -1 };
                }

                //Let previous element be the element immediately above last table in the stack of open elements.
                Element previousElement = null;

                for (int i = 0; i < openElements.length - 1; i++)
                {
                    if (this.IsSameDomElement(openElements.item[i], lastTable))
                    {
                        break;
                    }
                    previousElement = openElements.item[i];
                }

                //Let adjusted insertion location be inside previous element, after its last child (if any).

                return new insertionLocation() { partentElement = previousElement, insertAt = -1 };

                //These steps are involved in part because it's possible for elements, the table element in this case in particular, to have been moved by a script around in the DOM, or indeed removed from the DOM entirely, after the element was inserted by the parser.
            }
            //Otherwise

            else
            {
                //Let adjusted insertion location be inside target, after its last child (if any).
                return new insertionLocation() { partentElement = target, insertAt = -1 };   // always append, so always after the last child. 
            }

        }

        private Comment insertComment(HtmlToken token)
        {
            Comment comment = createComment(token);

            insertionLocation insertlocation = appropriatePlaceNewNode(null);

            Node node = insertlocation.partentElement;

            if (node != null)
            {
                comment.parentNode = node;
                if (node.nodeType == enumNodeType.ELEMENT)
                {
                    comment.parentElement = (Element)node;
                }
                if (insertlocation.insertAt == -1)
                {
                    node.appendChild(comment);
                }
                else
                {
                    node.childNodes.item.Insert(insertlocation.insertAt, comment);
                }
                return comment;
            }
            else
            {
                comment.parentNode = doc;
                doc.appendChild(comment);

                return comment;
            }


            //Let data be the data given in the comment token being processed.

            //If position was specified, then let the adjusted insertion location be position. Otherwise, let adjusted insertion location be the appropriate place for inserting a node.

            //Create a Comment node whose data attribute is set to data and whose ownerDocument is the same as that of the node in which the adjusted insertion location finds itself.

            //Insert the newly created node at the adjusted insertion location.

        }

        public Element insertElement(HtmlToken token)
        {
            Element element = createElement(token);

            insertionLocation insertLocation = appropriatePlaceNewNode(null);

            Node node = insertLocation.partentElement;

            if (node != null)
            {
                element.parentNode = node;
                if (node.nodeType == enumNodeType.ELEMENT)
                {
                    element.parentElement = (Element)node;
                }

                if (insertLocation.insertAt == -1)
                {
                    node.appendChild(element);
                }
                else
                {
                    node.childNodes.item.Insert(insertLocation.insertAt, element);
                }

                openElements.push(element);

                return element;
            }
            else
            {
                element.parentNode = doc;
                doc.appendChild(element);
                openElements.push(element);
                return element;
            }
        }

        //private void insertCharacter(HtmlToken token)
        //{
        //    insertCharacter(token.data);
        //}

        private void insertCharacter(HtmlToken token)
        {
            // When the steps below require the user agent to insert a character while processing a token, the user agent must run the following steps:

            //Let data be the characters passed to the algorithm, or, if no characters were explicitly specified, the character of the character token being processed.

            //Let the adjusted insertion location be the appropriate place for inserting a node.
            insertionLocation insertlocation = appropriatePlaceNewNode(null);

            Node node = insertlocation.partentElement;

            //If the adjusted insertion location is in a Document node, then abort these steps.
            //The DOM will not let Document nodes have Text node children, so they are dropped on the floor.

            if (node != null && node.nodeType != enumNodeType.DOCUMENT)
            {

                //If there is a Text node immediately before the adjusted insertion location, then append data to that Text node's data.
                //Otherwise, create a new Text node whose data is data and whose ownerDocument is the same as that of the element in which the adjusted insertion location finds itself, and insert the newly created node at the adjusted insertion location.

                Node lastchild = node.lastChild();

                if (lastchild == null || lastchild.nodeType != enumNodeType.TEXT)
                {
                    Text textnode = createTextNode(node);
                    //textnode.data = characterData;
                    textnode.appendData(token.data);
                    textnode.location.openTokenStartIndex = token.startIndex;
                    textnode.location.openTokenEndIndex = token.startIndex;

                    textnode.location.endTokenEndIndex = token.endIndex;
                    textnode.location.endTokenStartIndex = token.endIndex;

                    if (insertlocation.insertAt == -1)
                    {
                        node.appendChild(textnode);
                    }
                    else
                    {
                        node.childNodes.item.Insert(insertlocation.insertAt, textnode);
                    }

                }
                else
                {
                    Text textnode = (Text)lastchild;
                    //textnode.data += characterData;
                    textnode.appendData(token.data);
                    textnode.location.endTokenEndIndex = token.endIndex;
                    textnode.location.endTokenStartIndex = token.endIndex;
                }

            }

        }

        /// <summary>
        /// This is only used by the before html comment now. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private Comment createComment(HtmlToken token)
        {
            Comment comment = doc.createComment(token.data);

            comment.location.openTokenStartIndex = token.startIndex;
            comment.location.endTokenStartIndex = token.startIndex;

            comment.location.openTokenEndIndex = token.endIndex;
            comment.location.endTokenEndIndex = token.endIndex;


            return comment;

        }

        private Text createTextNode(Node parentNode)
        {
            Text textNode = new Text();
            textNode.ownerDocument = parentNode.ownerDocument;
            textNode.parentNode = parentNode;

            if (parentNode.nodeType == enumNodeType.ELEMENT)
            {
                textNode.parentElement = (Element)parentNode;
            }

            return textNode;
        }

        private DocumentType createDocType(HtmlToken token)
        {
            DocumentType doctype = new DocumentType();
            doctype.name = token.name;
            doctype.systemId = token.systemId;
            doctype.publicId = token.publicId;

            doctype.ownerDocument = doc;

            if (token.forceQuirks)
            {
                doc.setQuirksMode();
            }

            Node currentOpenNode = openElements.currentNode();

            doctype.location.openTokenStartIndex = token.startIndex;
            doctype.location.openTokenEndIndex = token.endIndex;

            doctype.location.endTokenStartIndex = token.startIndex;
            doctype.location.endTokenEndIndex = token.endIndex;


            return doctype;

        }

        internal Element createElement(HtmlToken token)
        {
            Element element = doc.createElement(token.tagName);

            foreach (var item in token.attributes)
            {
                element.setAttribute(item.Key, item.Value);
            }

            element.location.openTokenStartIndex = token.startIndex;

            element.location.openTokenEndIndex = token.endIndex;

            return element;
        }

        private void acknowledgedSelfClosing(HtmlToken token)
        {
            return;
            // When a start tag token is emitted with its self-closing flag set, if the flag is not acknowledged when it is processed by the tree construction stage, that is a parse error.

            //When an end tag token is emitted with attributes, that is a parse error.

            //When an end tag token is emitted with its self-closing flag set, that is a parse error.

        }

        private List<string> _impliedEndTags;
        private List<string> ImpliedEndTags
        {
            get
            {
                if (_impliedEndTags == null)
                {
                    _impliedEndTags = new List<string>();
                    _impliedEndTags.Add("dd");
                    _impliedEndTags.Add("dt");
                    _impliedEndTags.Add("li");
                    _impliedEndTags.Add("option");
                    _impliedEndTags.Add("optgroup");
                    _impliedEndTags.Add("p");
                    _impliedEndTags.Add("rb");
                    _impliedEndTags.Add("rp");
                    _impliedEndTags.Add("rt");
                    _impliedEndTags.Add("rtc");
                }
                return _impliedEndTags;

            }
        }
        /// <summary>
        ///  8.2.5.3 Closing elements that have implied end tags
        /// </summary>
        private void generateImpliedEndTags(string excludeElementTag = null)
        {
            //When the steps below require the UA to generate implied end tags, then, while the current node is a dd element, a dt element, an li element, an option element, an optgroup element, a p element, an rb element, an rp element, an rt element, or an rtc element, the UA must pop the current node off the stack of open elements.
            //If a step requires the UA to generate implied end tags but lists an element to exclude from the process, then the UA must perform the above steps as if that element was not in the above list. 
            ///TODO: check whether we need to loop currentnode or not, to popoff recursive.  
            Element currentnode = openElements.currentNode();
            if (currentnode == null)
            { return; }

            string currentNodeTagName = currentnode.tagName.ToLower();


            if (ImpliedEndTags.Contains(currentNodeTagName))
            {
                if (string.IsNullOrEmpty(excludeElementTag) || excludeElementTag != currentNodeTagName)
                {
                    openElements.popOffLast(currentnode);
                    return;
                }
            }
        }


        private void generateImpliedEndTagsThroughly()
        {
            //TODO: this is for template tag parsing, did not implemented now. 
            return;
        }

        public bool IsSameDomElement(Element one, Element two)
        {
            if (one.tagName != two.tagName)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(one.namespaceURI) || !string.IsNullOrEmpty(two.namespaceURI))
            {
                if (one.namespaceURI != two.namespaceURI)
                {
                    return false;
                }
            }

            //For these purposes, the attributes must be compared as they were when the elements were created by the parser; two elements have the same attributes if all their parsed attributes can be paired such that the two attributes in each pair have identical names, namespaces, and values (the order of the attributes does not matter).

            if (one.attributes.Count() != two.attributes.Count())
            {
                return false;
            }

            foreach (var item in one.attributes)
            {
                string valueInTwo = two.getAttribute(item.name);

                if (string.IsNullOrWhiteSpace(item.value) && string.IsNullOrWhiteSpace(valueInTwo))
                {
                    continue;
                }

                if (valueInTwo != item.value)
                {

                    return false;
                }

            }

            return true;
        }



        private void closeCell()
        {
            //  Where the steps above say to close the cell, they mean to run the following algorithm:

            //Generate implied end tags.
            generateImpliedEndTags();

            //If the current node is not now a td element or a th element, then this is a parse error.
            if (!openElements.currentNode().tagName.isOneOf("td", "th"))
            {
                onError("expect open td or th tag instead of " + openElements.currentNode().tagName);
            }

            //Pop elements from the stack of open elements stack until a td element or a th element has been popped from the stack.

            openElements.popOffTillOneOf(true, "td", "th");

            //Clear the list of active formatting elements up to the last marker.
            activeFormatingElements.clearUpToLastMarker();

            //Switch the insertion mode to "in row".
            insertionMode = enumInsertionMode.inRow;

            //The stack of open elements cannot have both a td and a th element in table scope at the same time, nor can it have neither when the close the cell algorithm is invoked.
        }

        private void resetInsertionMode()
        {
            ///Let last be false.
            bool last = false;

            //Let node be the last node in the stack of open elements.
            int index = openElements.length - 1;
            if (index < 0)
            {
                return;
            }
            Element node = openElements.item[index];

        //Loop: If node is the first node in the stack of open elements, then set last to true, and, if the parser was originally created as part of the HTML fragment parsing algorithm (fragment case) set node to the context element.

        myLoop:
            if (IsSameDomElement(node, openElements.firstElement()))
            {
                last = true;
            }

            if (fragmentParsing)
            {
                node = context;
            }

            //If node is a select element, run these substeps:
            if (node.tagName == "select")
            {
                //If last is true, jump to the step below labeled done.
                if (last)
                {
                    goto done;
                }

                //Let ancestor be node.

                Element ancestor = node;

            //Loop: If ancestor is the first node in the stack of open elements, jump to the step below labeled done.
            myinnerloop:
                if (IsSameDomElement(ancestor, openElements.firstElement()))
                {
                    goto done;
                }

                //Let ancestor be the node before ancestor in the stack of open elements.
                index = index - 1;
                ancestor = openElements.item[index];

                //If ancestor is a template node, jump to the step below labeled done.
                if (ancestor.tagName == "template")
                {
                    goto done;
                }

                //If ancestor is a table node, switch the insertion mode to "in select in table" and abort these steps.
                if (ancestor.tagName == "table")
                {
                    insertionMode = enumInsertionMode.inSelectInTable;
                }
                //Jump back to the step labeled loop.
                goto myinnerloop;


            //Done: Switch the insertion mode to "in select" and abort these steps.
            done:

                insertionMode = enumInsertionMode.inSelect;
                return;
            }


            //If node is a td or th element and last is false, then switch the insertion mode to "in cell" and abort these steps.
            if (node.tagName.isOneOf("td", "th") && !last)
            {
                insertionMode = enumInsertionMode.inCell;
                return;
            }

            //If node is a tr element, then switch the insertion mode to "in row" and abort these steps.
            if (node.tagName == "tr")
            {
                insertionMode = enumInsertionMode.inRow;
                return;
            }

            //If node is a tbody, thead, or tfoot element, then switch the insertion mode to "in table body" and abort these steps.
            if (node.tagName.isOneOf("tbody", "thead", "tfoot"))
            {
                insertionMode = enumInsertionMode.inTableBody;
                return;
            }

            //If node is a caption element, then switch the insertion mode to "in caption" and abort these steps.
            if (node.tagName == "caption")
            {
                insertionMode = enumInsertionMode.inCaption;
                return;
            }

            //If node is a colgroup element, then switch the insertion mode to "in column group" and abort these steps.
            if (node.tagName == "colgroup")
            {
                insertionMode = enumInsertionMode.inColumnGroup;
                return;
            }

            //If node is a table element, then switch the insertion mode to "in table" and abort these steps.
            if (node.tagName == "table")
            {
                insertionMode = enumInsertionMode.inTable;
                return;
            }

            //If node is a template element, then switch the insertion mode to the current template insertion mode and abort these steps.
            if (node.tagName == "template")
            {
                insertionMode = templateMode.currentMode();
                return;
            }

            //If node is a head element and last is true, then switch the insertion mode to "in body" ("in body"! not "in head"!) and abort these steps. (fragment case)
            if (node.tagName == "head" && last)
            {
                insertionMode = enumInsertionMode.inBody;
                return;
            }


            //If node is a head element and last is false, then switch the insertion mode to "in head" and abort these steps.
            if (node.tagName == "head" && !last)
            {
                insertionMode = enumInsertionMode.inHead;
                return;
            }

            //If node is a body element, then switch the insertion mode to "in body" and abort these steps.
            if (node.tagName == "body")
            {
                insertionMode = enumInsertionMode.inBody;
                return;
            }

            //If node is a frameset element, then switch the insertion mode to "in frameset" and abort these steps. (fragment case)

            if (node.tagName == "frameset")
            {
                insertionMode = enumInsertionMode.inFrameset;
                return;
            }

            //If node is an html element, run these substeps:
            if (node.tagName == "html")
            {

                //If the head element pointer is null, switch the insertion mode to "before head" and abort these steps. (fragment case)
                if (elementPointer.head == null)
                {
                    insertionMode = enumInsertionMode.beforeHead;
                    return;
                }
                else
                {
                    //Otherwise, the head element pointer is not null, switch the insertion mode to "after head" and abort these steps.
                    insertionMode = enumInsertionMode.afterHead;
                    return;
                }

            }

            //If last is true, then switch the insertion mode to "in body" and abort these steps. (fragment case)
            if (last)
            {
                insertionMode = enumInsertionMode.inBody;
                return;
            }

            //Let node now be the node before node in the stack of open elements.
            index = index - 1;
            node = openElements.item[index];

            //Return to the step labeled loop.
            goto myLoop;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <returns>return = false = gotoanything else. else, return = true</returns>
        private bool adoptionAgency(string subject)
        {
            //  The adoption agency algorithm, which takes as its only argument a tag name subject for which the algorithm is being run, consists of the following steps:

            //If the current node is an HTML element whose tag name is subject, then run these substeps:
            if (openElements.currentNode().tagName == subject)
            {
                //Let element be the current node.
                Element element = openElements.currentNode();

                //Pop element off the stack of open elements.
                openElements.popOffLast(element);

                //If element is also in the list of active formatting elements, remove the element from the list.
                activeFormatingElements.Remove(element.tagName, false);

                //Abort the adoption agency algorithm.
                return true;
            }

            int outLoopCounter = 0;
        //Let outer loop counter be zero.

        //Outer loop: If outer loop counter is greater than or equal to eight, then abort these steps.

        outerLoop:
            if (outLoopCounter >= 8)
            {
                return true;
            }

            //Increment outer loop counter by one.
            outLoopCounter += 1;

            //Let formatting element be the last element in the list of active formatting elements that:

            //is between the end of the list and the last scope marker in the list, if any, or the start of the list otherwise, and
            //has the tag name subject.
            int index = activeFormatingElements.length - 1;

            FormattingElement formattingelement = null;

            for (int i = index; i >= 0; i--)
            {
                if (activeFormatingElements.item[i].isMarker)
                {
                    break;
                }
                else
                {
                    if (activeFormatingElements.item[i].element.tagName == subject)
                    {
                        formattingelement = activeFormatingElements.item[i];
                        break;
                    }

                }

            }

            //If there is no such element, then abort these steps and instead act as described in the "any other end tag" entry above.

            if (formattingelement == null)
            {
                //TODO: as described in the "any other end tag" entry above.
                return false;
            }


            //If formatting element is not in the stack of open elements, then this is a parse error; remove the element from the list, and abort these steps.

            if (!openElements.hasElement(formattingelement.element))
            {
                onError("element not in the list of open elements");
                activeFormatingElements.Remove(formattingelement.element.tagName, false);
                return true;
            }

            //If formatting element is in the stack of open elements, but the element is not in scope, then this is a parse error; abort these steps.

            if (openElements.hasElement(formattingelement.element) && !openElements.hasElementInScope(formattingelement.element.tagName, ScopeType.inScope))
            {
                onError("element not in scope");
                return true;
            }


            //If formatting element is not the current node, this is a parse error. (But do not abort these steps.)
            if (!IsSameDomElement(formattingelement.element, openElements.currentNode()))
            {
                onError("formatting element is not the current open element");
            }


            //Let furthest block be the topmost node in the stack of open elements that is lower in the stack than formatting element, and is an element in the special category. There might not be one.
            int openElementIndex = openElements.length - 1;
            Element furthestBlock = null;

            for (int i = openElementIndex; i >= 0; i--)
            {
                if (IsSameDomElement(openElements.item[i], formattingelement.element))
                {
                    break;
                }

                if (openElements.Special().Contains(openElements.item[i].tagName))
                {
                    furthestBlock = openElements.item[i];
                }
            }


            //If there is no furthest block, then the UA must first pop all the nodes from the bottom of the stack of open elements, from the current node up to and including formatting element, then remove formatting element from the list of active formatting elements, and finally abort these steps.
            if (furthestBlock == null)
            {
                openElements.popOffTill(formattingelement.element.tagName, true);
                activeFormatingElements.Remove(formattingelement.element.tagName, false);
                return true;
            }

            //Let common ancestor be the element immediately above formatting element in the stack of open elements.
            Element commonAncestor = null;
            bool found = false;
            for (int i = openElementIndex; i >= 0; i--)
            {

                if (found)
                {
                    commonAncestor = openElements.item[i];
                    break;
                }

                if (openElements.item[i].tagName == formattingelement.element.tagName)
                {
                    found = true;
                }

            }


            //Let a bookmark note the position of formatting element in the list of active formatting elements relative to the elements on either side of it in the list.


            int bookmarkBeforeCounter = 0;   // the counter of elements on top of formatting elements 
            for (int i = 0; i < activeFormatingElements.length - 1; i++)
            {
                if (formattingelement.isEqualTo(activeFormatingElements.item[i]))
                {
                    break;
                }
                else
                {
                    bookmarkBeforeCounter = i;
                }
            }


            //Let node and last node be furthest block. Follow these steps:

            Element node = furthestBlock;
            Element lastnode = furthestBlock;

            //Let inner loop counter be zero.
            int innerLoopCounter = 0;

        //Inner loop: Increment inner loop counter by one.
        innerLoop:
            innerLoopCounter += 1;

            //Let node be the element immediately above node in the stack of open elements, or if node is no longer in the stack of open elements (e.g. because it got removed by this algorithm), the element that was immediately above node in the stack of open elements before node was removed.
            Element aboveNodeBeforeRemoved = null;

            openElementIndex = openElements.length - 1;
            if (openElements.hasTag(node.tagName))
            {
                bool matched = false;

                for (int i = openElementIndex; i >= 0; i--)
                {
                    if (matched)
                    {
                        node = openElements.item[i];
                        break;
                    }

                    if (openElements.item[i].tagName == node.tagName)
                    {
                        matched = true;
                    }
                }

            }
            else
            {
                //the element that was immediately above node in the stack of open elements before node was removed.
                if (aboveNodeBeforeRemoved != null)
                {
                    node = aboveNodeBeforeRemoved;
                    aboveNodeBeforeRemoved = null;
                }
                else
                {
                    node = openElements.item[openElementIndex];
                }

            }


            //If node is formatting element, then go to the next step in the overall algorithm.
            if (IsSameDomElement(node, formattingelement.element))
            {
                goto nextStep;
            }

            //If inner loop counter is greater than three and node is in the list of active formatting elements, then remove node from the list of active formatting elements.
            if (innerLoopCounter > 3 && activeFormatingElements.hasElement(node))
            {
                activeFormatingElements.Remove(node.tagName, false);

            }

            //If node is not in the list of active formatting elements, then remove node from the stack of open elements and then go back to the step labeled inner loop.

            if (!activeFormatingElements.hasElement(node))
            {
                ///   openElements.popOff(node);  use manually removal instead of popoff. 

                int removei = 0;
                int abovei = 0;

                for (int i = openElements.length - 1; i >= 0; i--)
                {
                    if (openElements.item[i].tagName == node.tagName)
                    {
                        removei = i;
                        abovei = i - 1;
                        break;
                    }
                }

                openElements.item.RemoveAt(removei);
                if (abovei >= 0)
                {
                    aboveNodeBeforeRemoved = openElements.item[abovei];
                }
                goto innerLoop;

            }

            //Create an element for the token for which the element node was created, in the HTML namespace, with common ancestor as the intended parent; replace the entry for node in the list of active formatting elements with an entry for the new element, replace the entry for node in the stack of open elements with an entry for the new element, and let node be the new element.

            Element newelement = doc.createElement(node.tagName);
            foreach (var item in node.attributes)
            {
                // ignore namespace now. 
                newelement.setAttribute(item.name, item.value);
            }
            newelement.parentNode = commonAncestor;
            newelement.parentElement = commonAncestor;

            commonAncestor.appendChild(newelement);

            for (int i = 0; i < openElements.length - 1; i++)
            {
                if (IsSameDomElement(openElements.item[i], node))
                {
                    openElements.item[i] = newelement;
                    break;
                }
            }

            for (int i = 0; i < activeFormatingElements.length - 1; i++)
            {
                if (!activeFormatingElements.item[i].isMarker && (ActiveFormattingElementList.IsSameDomElement(activeFormatingElements.item[i].element, node)))
                {
                    FormattingElement newformatelement = new FormattingElement();
                    newformatelement.element = newelement;
                    newformatelement.isMarker = false;

                    activeFormatingElements.item[i] = newformatelement;
                    break;
                }
            }

            node = newelement;

            //If last node is furthest block, then move the aforementioned bookmark to be immediately after the new node in the list of active formatting elements.

            if (IsSameDomElement(lastnode, furthestBlock))
            {
                bool itemfound = false;
                for (int i = 0; i < activeFormatingElements.length - 1; i++)
                {
                    if (itemfound)
                    {
                        bookmarkBeforeCounter = i;
                        break;
                    }

                    if (!activeFormatingElements.item[i].isMarker && (ActiveFormattingElementList.IsSameDomElement(activeFormatingElements.item[i].element, node)))
                    {
                        itemfound = true;
                    }
                }

                if (!itemfound)
                {
                    bookmarkBeforeCounter = activeFormatingElements.length - 1;  //TODO: check whether we need -1 or not. 
                }
            }

            //Insert last node into node, first removing it from its previous parent node if any.

            lastnode.parentElement = node;
            lastnode.parentNode = node;

            node.appendChild(lastnode);

            //Let last node be node.
            lastnode = node;

            //Return to the step labeled inner loop.
            goto innerLoop;

        //Insert whatever last node ended up being in the previous step at the appropriate place for inserting a node, but using common ancestor as the override target.

        nextStep:

            insertionLocation insertlocation = appropriatePlaceNewNode(commonAncestor);
            lastnode.parentElement = insertlocation.partentElement;
            lastnode.parentNode = insertlocation.partentElement;
            if (insertlocation.insertAt == -1)
            {
                insertlocation.partentElement.appendChild(lastnode);
            }
            else
            {
                insertlocation.partentElement.childNodes.item.Insert(insertlocation.insertAt, lastnode);
            }

            //Create an element for the token for which formatting element was created, in the HTML namespace, with furthest block as the intended parent.
            Element newElementInFormatting = doc.createElement(formattingelement.element.tagName);
            foreach (var item in formattingelement.element.attributes)
            {
                newElementInFormatting.setAttribute(item.name, item.value);
            }
            newElementInFormatting.parentNode = furthestBlock;
            newElementInFormatting.parentElement = furthestBlock;

            //Take all of the child nodes of furthest block and append them to the element created in the last step.
            foreach (var item in furthestBlock.childNodes.item)
            {
                newElementInFormatting.appendChild(item);
            }

            furthestBlock.childNodes.item.Clear();

            //Append that new element to furthest block.
            furthestBlock.appendChild(newElementInFormatting);

            //Remove formatting element from the list of active formatting elements, and insert the new element into the list of active formatting elements at the position of the aforementioned bookmark.
            activeFormatingElements.Remove(formattingelement.element.tagName, false);

            FormattingElement newFormatElement = new FormattingElement();
            newFormatElement.element = newElementInFormatting;
            newFormatElement.isMarker = false;

            activeFormatingElements.item.Insert(bookmarkBeforeCounter, newFormatElement);

            //Remove formatting element from the stack of open elements, and insert the new element into the stack of open elements immediately below the position of furthest block in that stack.
            openElements.popOff(formattingelement.element.tagName);

            int insertposition = 0;
            for (int i = 0; i < openElements.length - 1; i++)
            {
                if (openElements.item[i].tagName == furthestBlock.tagName)
                {
                    insertposition = i + 1;
                    break;
                }
            }

            openElements.item.Insert(insertposition, newElementInFormatting);
            //Jump back to the step labeled outer loop.
            goto outerLoop;

            //This algorithm's name, the "adoption agency algorithm", comes from the way it causes elements to change parents, and is in contrast with other possible algorithms for dealing with misnested content, which included the "incest algorithm", the "secret affair algorithm", and the "Heisenberg algorithm".


        }


        /// <summary>
        /// 8.2.5.2 Parsing elements that contain only text
        /// </summary>
        /// <param name="token"></param>
        private void rawTextParsing(HtmlToken token)
        {
            //Insert an HTML element for the token.
            insertElement(token);

            //If the algorithm that was invoked is the generic raw text element parsing algorithm, switch the tokenizer to the RAWTEXT state; 
            tokenizer.ParseState = enumParseState.RAWTEXT;
            tokenizer.ScriptState = enumScriptParseState.initial;

            //Let the original insertion mode be the current insertion mode.
            originalInsertionMode = insertionMode;

            //Then, switch the insertion mode to "text".
            insertionMode = enumInsertionMode.text;
        }

        /// <summary>
        /// 8.2.5.2 Parsing elements that contain only text
        /// </summary>
        /// <param name="token"></param>
        private void RCDATAParsing(HtmlToken token)
        {
            //Insert an HTML element for the token.
            insertElement(token);

            //if the algorithm invoked was the generic RCDATA element parsing algorithm, switch the tokenizer to the RCDATA state.
            tokenizer.ParseState = enumParseState.RCDATA;
            tokenizer.ScriptState = enumScriptParseState.initial;

            //Let the original insertion mode be the current insertion mode.
            originalInsertionMode = insertionMode;
            //Then, switch the insertion mode to "text".
            insertionMode = enumInsertionMode.text;

        }


        /// <summary>
        /// this token is a home token or a foreign token. 
        /// see 8.2.5 Tree construction
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool isHome(HtmlToken token)
        {

            /// return true for now. Foreign token is not common. does not justify the time for now.
            /// TO BE Implemented.
            return true;

            // As each token is emitted from the tokenizer, the user agent must follow the appropriate steps from the following list, known as the tree construction dispatcher:

            //If there is no adjusted current node
            //If the adjusted current node is an element in the HTML namespace
            //If the adjusted current node is a MathML text integration point and the token is a start tag whose tag name is neither "mglyph" nor "malignmark"
            //If the adjusted current node is a MathML text integration point and the token is a character token
            //If the adjusted current node is an annotation-xml element in the MathML namespace and the token is a start tag whose tag name is "svg"
            //If the adjusted current node is an HTML integration point and the token is a start tag
            //If the adjusted current node is an HTML integration point and the token is a character token
            //If the token is an end-of-file token
            //Process the token according to the rules given in the section corresponding to the current insertion mode in HTML content.
            //Otherwise
            //Process the token according to the rules given in the section for parsing tokens in foreign content.
            //The next token is the token that is about to be processed by the tree construction dispatcher (even if the token is subsequently just ignored).

            //A node is a MathML text integration point if it is one of the following elements:

            //An mi element in the MathML namespace
            //An mo element in the MathML namespace
            //An mn element in the MathML namespace
            //An ms element in the MathML namespace
            //An mtext element in the MathML namespace
            //A node is an HTML integration point if it is one of the following elements:

            //An annotation-xml element in the MathML namespace whose start tag token had an attribute with the name "encoding" whose value was an ASCII case-insensitive match for the string "text/html"
            //An annotation-xml element in the MathML namespace whose start tag token had an attribute with the name "encoding" whose value was an ASCII case-insensitive match for the string "application/xhtml+xml"
            //A foreignObject element in the SVG namespace
            //A desc element in the SVG namespace
            //A title element in the SVG namespace

        }

        /// <summary>
        ///  8.2.5.4.1 The "initial" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void Initial(HtmlToken token)
        {
            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //Ignore the token.
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {

                return;
            }

            //A comment token
            //Insert a comment as the last child of the Document object.
            else if (token.type == enumHtmlTokenType.Comment)
            {
                Comment comment = createComment(token);
                doc.appendChild(comment);

                return;
            }
            //A DOCTYPE token
            //Append a DocumentType node to the Document node, with the name attribute set to the name given in the DOCTYPE token,
            //the publicId attribute set to the public identifier given in the DOCTYPE token
            //the systemId attribute set to the system identifier given in the DOCTYPE token
            //Associate the DocumentType node with the Document object so that it is returned as the value of the doctype attribute of the Document object

            else if (token.type == enumHtmlTokenType.DocType)
            {
                DocumentType doctype = createDocType(token);
                doc.appendChild(doctype);
                doc.doctype = doctype;

                if (documentMode.checkQuirkMode(doc, doctype))
                {
                    doc.setQuirksMode();
                }

                ///Then, switch the insertion mode to "before html".
                insertionMode = enumInsertionMode.beforeHtml;

                return;
            }

            else
            {
                //  Anything else

                //If the document is not an iframe srcdoc document, then this is a parse error; set the Document to quirks mode.
                if (!doc.iframeSrcDoc)
                {
                    onError("expect a doctype definition or iframe srcdoc document.");
                    doc.setQuirksMode();
                }

                //In any case, switch the insertion mode to "before html", then reprocess the token.
                insertionMode = enumInsertionMode.beforeHtml;
                beforeHtml(token);

            }




        }

        /// <summary>
        /// 8.2.5.4.2 The "before html" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void beforeHtml(HtmlToken token)
        {
            //A DOCTYPE token
            //Parse error. Ignore the token.
            if (token.type == enumHtmlTokenType.DocType)
            {
                onError("Doctype must be the first declaration in HTML document");
                return;
            }
            //A comment token
            //Insert a comment as the last child of the Document object.
            else if (token.type == enumHtmlTokenType.Comment)
            {
                Comment comment = createComment(token);
                doc.appendChild(comment);
                return;
            }
            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //Ignore the token.
            else if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {
                return;
            }

            //A start tag whose tag name is "html"
            //Create an element for the token in the HTML namespace, with the Document as the intended parent. Append it to the Document object. Put this element in the stack of open elements.
            //application cached algo not supported. 
            //Switch the insertion mode to "before head".

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                Element htmlElement = this.createElement(token);
                htmlElement.parentNode = doc;
                doc.appendChild(htmlElement);

                doc.documentElement = htmlElement;

                openElements.push(htmlElement);

                insertionMode = enumInsertionMode.beforeHead;

                return;
            }

            //An end tag whose tag name is one of: "head", "body", "html", "br"
            //Act as described in the "anything else" entry below.
            //Any other end tag
            //Parse error. Ignore the token.
            else if (token.type == enumHtmlTokenType.EndTag)
            {
                if (token.tagName.isOneOf("head", "body", "html", "br"))
                {
                    goto anythingelse;
                }
                else
                {
                    onError("end tag head, body, html or br expected.");
                }
                return;
            }
            else
            {
                goto anythingelse;
            }

        anythingelse:
            {
                //Anything else
                //Create an html element whose ownerDocument is the Document object. Append it to the Document object. 
                // Put this element in the stack of open elements.

                //fake a token.
                HtmlToken faketoken = new HtmlToken(enumHtmlTokenType.StartTag);
                faketoken.tagName = "html";

                faketoken.startIndex = token.startIndex;
                faketoken.endIndex = -1;

                Element htmlElement = this.createElement(faketoken);
                htmlElement.parentNode = doc;
                doc.appendChild(htmlElement);

                doc.documentElement = htmlElement;

                openElements.push(htmlElement);

                insertionMode = enumInsertionMode.beforeHead;

                //Switch the insertion mode to "before head", then reprocess the token.
                beforeHead(token);

            }

        }

        /// <summary>
        ///  8.2.5.4.3 The "before head" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void beforeHead(HtmlToken token)
        {
            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //Ignore the token.
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {
                return;
            }
            //A comment token 
            else if (token.type == enumHtmlTokenType.Comment)
            {
                //Insert a comment.
                insertComment(token);
                return;
            }
            //A DOCTYPE token
            //Parse error. Ignore the token.
            else if (token.type == enumHtmlTokenType.DocType)
            {
                onError("doctype definition must be in the first line of a page.");
                return;
            }
            //A start tag whose tag name is "html"
            //Process the token using the rules for the "in body" insertion mode.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                inBody(token);
                return;
            }
            //A start tag whose tag name is "head"
            //Insert an HTML element for the token.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "head")
            {
                //Set the head element pointer to the newly created head element.
                //Switch the insertion mode to "in head".

                elementPointer.head = insertElement(token);
                insertionMode = enumInsertionMode.inHead;
            }

            //An end tag whose tag name is one of: "head", "body", "html", "br"
            //Act as described in the "anything else" entry below.
            //Any other end tag
            //Parse error. Ignore the token.
            else if (token.type == enumHtmlTokenType.EndTag && !token.tagName.isOneOf("head", "body", "html", "br"))
            {
                onError("expect an open tag first.");
                return;
            }
            else
            {
                //Anything else
                //Insert an HTML element for a "head" start tag token with no attributes.
                HtmlToken faketoken = new HtmlToken(enumHtmlTokenType.StartTag);
                faketoken.tagName = "head";

                // faketoken.startIndex = token.startIndex;
                faketoken.startIndex = token.startIndex;
                faketoken.endIndex = -1;

                //Set the head element pointer to the newly created head element.
                elementPointer.head = insertElement(faketoken);

                //Switch the insertion mode to "in head".
                insertionMode = enumInsertionMode.inHead;

                //Reprocess the current token.
                inHead(token);
            }
        }

        /// <summary>
        /// 8.2.5.4.4 The "in head" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inHead(HtmlToken token)
        {
            // A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //Insert the character.
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {
                insertCharacter(token);
                return;
            }
            //A comment token
            //Insert a comment.
            else if (token.type == enumHtmlTokenType.Comment)
            {
                insertComment(token);
                return;
            }
            //A DOCTYPE token
            //Parse error. Ignore the token.
            else if (token.type == enumHtmlTokenType.DocType)
            {
                onError("doctype must be defined in the first line");
                return;
            }

            //A start tag whose tag name is "html"
            //Process the token using the rules for the "in body" insertion mode.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                inBody(token);
                return;
            }
            //A start tag whose tag name is one of: "base", "basefont", "bgsound", "link"
            //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
            //Acknowledge the token's self-closing flag, if it is set.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("base", "basefont", "bgsound", "link"))
            {
                Element element = insertElement(token);
                openElements.popOff(element);

                acknowledgedSelfClosing(token);

            }

            //A start tag whose tag name is "meta"
            //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
            //Acknowledge the token's self-closing flag, if it is set.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "meta")
            {
                Element element = insertElement(token);
                openElements.popOff(element);

                acknowledgedSelfClosing(token);

                //If the element has a charset attribute, and getting an encoding from its value results in a supported 
                //ASCII-compatible character encoding or a UTF-16 encoding, and the confidence is currently tentative,
                //then change the encoding to the resulting encoding.
                if (element.hasAttribute("charset"))
                {
                    // TODO: to be implemented.  this is done by .NET string now. 
                }
                else
                {
                    //Otherwise, if the element has an http-equiv attribute whose value is an ASCII case-insensitive match for the string "Content-Type", and the element has a content attribute, and applying the algorithm for extracting a character encoding from a meta element to that attribute's value returns a supported ASCII-compatible character encoding or a UTF-16 encoding, and the confidence is currently tentative, then change the encoding to the extracted encoding.

                    //TODO: to be implemented. 
                }

                return;
            }
            //A start tag whose tag name is "title"
            //Follow the generic RCDATA element parsing algorithm.

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "title")
            {
                RCDATAParsing(token);
                return;
            }

            //A start tag whose tag name is "noscript", if the scripting flag is enabled
            //A start tag whose tag name is one of: "noframes", "style"
            //Follow the generic raw text element parsing algorithm.
            else if ((token.type == enumHtmlTokenType.StartTag) && ((token.tagName == "noscript" && scripting) || token.tagName.isOneOf("noframes", "style")))
            {
                rawTextParsing(token);
                return;
            }
            //A start tag whose tag name is "noscript", if the scripting flag is disabled
            //Insert an HTML element for the token.
            //Switch the insertion mode to "in head noscript".
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "noscript" && !scripting)
            {
                insertElement(token);
                insertionMode = enumInsertionMode.inHeadNoScript;
                return;
            }

            //A start tag whose tag name is "script"
            //Run these steps:
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "script")
            {
                //Let the adjusted insertion location be the appropriate place for inserting a node.

                insertionLocation insertlocation = appropriatePlaceNewNode(null);

                Node parentnode = insertlocation.partentElement;

                //Create an element for the token in the HTML namespace, with the intended parent being the element in which the adjusted insertion location finds itself.

                Element element = createElement(token);
                element.parentNode = parentnode;
                if (parentnode.nodeType == enumNodeType.ELEMENT)
                { element.parentElement = (Element)parentnode; }

                //TODO: below has  not been implemented yet. 

                //Mark the element as being "parser-inserted" and unset the element's "force-async" flag.
                //This ensures that, if the script is external, any document.write() calls in the script will execute in-line, instead of blowing the document away, as would happen in most other cases. It also prevents the script from executing until the end tag is seen.

                //If the parser was originally created for the HTML fragment parsing algorithm, then mark the script element as "already started". (fragment case)

                //Insert the newly created element at the adjusted insertion location.
                if (insertlocation.insertAt == -1)
                {
                    parentnode.appendChild(element);
                }
                else
                {
                    parentnode.childNodes.item.Insert(insertlocation.insertAt, element);
                }

                //Push the element onto the stack of open elements so that it is the new current node.
                openElements.push(element);

                //Switch the tokenizer to the script data state.

                tokenizer.ParseState = enumParseState.Script;
                tokenizer.ScriptState = enumScriptParseState.initial;

                //Let the original insertion mode be the current insertion mode.
                originalInsertionMode = insertionMode;

                //Switch the insertion mode to "text".
                insertionMode = enumInsertionMode.text;
                return;
            }

            //An end tag whose tag name is "head"
            //Pop the current node (which will be the head element) off the stack of open elements.
            //Switch the insertion mode to "after head".
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "head")
            {
                openElements.popOff(token.tagName);
                insertionMode = enumInsertionMode.afterHead;
                return;
            }

            //An end tag whose tag name is one of: "body", "html", "br"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("body", "html", "br"))
            {
                //Act as described in the "anything else" entry below.
                //Anything else
                //Pop the current node (which will be the head element) off the stack of open elements.
                //Switch the insertion mode to "after head".
                //Reprocess the token.
                openElements.popOff("head");
                afterHead(token);
                return;
            }
            //A start tag whose tag name is "template"

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "template")
            {
                //Insert an HTML element for the token.
                Element element = insertElement(token);

                //Insert a marker at the end of the list of active formatting elements.
                activeFormatingElements.insertMarker();

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                //Switch the insertion mode to "in template".

                insertionMode = enumInsertionMode.inTemplate;

                //Push "in template" onto the stack of template insertion modes so that it is the new current template insertion mode.
                templateMode.push(enumInsertionMode.inTemplate);
                return;
            }
            //An end tag whose tag name is "template"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "template")
            {
                //If there is no template element on the stack of open elements, then this is a parse error; ignore the token.
                if (!openElements.hasTag("template"))
                {
                    onError("template open tag not founded");
                    return;
                }
                else
                {
                    //Otherwise, run these steps:
                    //Generate implied end tags.

                    //If the current node is not a template element, then this is a parse error.
                    if (openElements.currentNode().tagName != "template")
                    {
                        onError("template element expected in the open elements list");
                    }

                    //Pop elements from the stack of open elements until a template element has been popped from the stack.
                    openElements.popOffTill("template", true);

                    //Clear the list of active formatting elements up to the last marker.
                    activeFormatingElements.clearUpToLastMarker();


                    //Pop the current template insertion mode off the stack of template insertion modes.
                    templateMode.popOffCurrent();

                    //Reset the insertion mode appropriately.
                    resetInsertionMode();

                }
                return;
            }

            //A start tag whose tag name is "head"
            //Any other end tag
            //Parse error. Ignore the token.
            else if ((token.type == enumHtmlTokenType.StartTag && token.tagName == "head") || token.type == enumHtmlTokenType.EndTag)
            {
                onError("unexpected end tag in header");
                return;
            }
            else
            {
                //Anything else
                //Pop the current node (which will be the head element) off the stack of open elements.
                //Switch the insertion mode to "after head".
                //Reprocess the token.
                openElements.popOff("head");
                afterHead(token);
            }

        }


        /// <summary>
        /// 8.2.5.4.5 The "in head noscript" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void InHeadNoScript(HtmlToken token)
        {

            //A DOCTYPE token
            //Parse error. Ignore the token.
            if (token.type == enumHtmlTokenType.DocType)
            {
                onError("doctype must be defined in the first line of a page");
                return;
            }

            //A start tag whose tag name is "html"
            //Process the token using the rules for the "in body" insertion mode.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                inBody(token);
                return;
            }

            //An end tag whose tag name is "noscript"
            //Pop the current node (which will be a noscript element) from the stack of open elements; the new current node will be a head element.
            //Switch the insertion mode to "in head".

            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "noscript")
            {
                openElements.popOff("noscript");

                insertionMode = enumInsertionMode.inHead;

                return;
            }


            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //A comment token
            //A start tag whose tag name is one of: "basefont", "bgsound", "link", "meta", "noframes", "style"
            //Process the token using the rules for the "in head" insertion mode.

            else if ((token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020')) |
                token.type == enumHtmlTokenType.Comment |
                (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("basefont", "bgsound", "link", "meta", "noframes", "style")))
            {
                inHead(token);
                return;

            }



            //An end tag whose tag name is "br"
            //Act as described in the "anything else" entry below.
            //A start tag whose tag name is one of: "head", "noscript"
            //Any other end tag
            //Parse error. Ignore the token.

            else if ((token.type == enumHtmlTokenType.StartTag && (token.tagName == "head" || token.tagName == "noscript")) || (token.type == enumHtmlTokenType.EndTag && token.tagName != "br"))
            {
                onError("unexpected tag");
                return;
            }
            else
            {

                //Anything else
                //Parse error.

                //Pop the current node (which will be a noscript element) from the stack of open elements; the new current node will be a head element.

                openElements.popOff("noscript");

                //Switch the insertion mode to "in head".

                insertionMode = enumInsertionMode.inHead;

                //Reprocess the token.
                inHead(token);

            }

        }


        /// <summary>
        /// 8.2.5.4.6 The "after head" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void afterHead(HtmlToken token)
        {

            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //Insert the character.
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {
                insertCharacter(token);
            }
            //A comment token
            //Insert a comment.
            else if (token.type == enumHtmlTokenType.Comment)
            {
                insertComment(token);
            }

            //A DOCTYPE token
            //Parse error. Ignore the token.
            else if (token.type == enumHtmlTokenType.DocType)
            {
                onError("doctype must be defined at the first of the page");
                return;
            }

            //A start tag whose tag name is "html"
            //Process the token using the rules for the "in body" insertion mode.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                inBody(token);
            }
            //A start tag whose tag name is "body"
            //Insert an HTML element for the token.
            //Set the frameset-ok flag to "not ok".
            //Switch the insertion mode to "in body".
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "body")
            {
                insertElement(token);
                frameset_ok = false;
                insertionMode = enumInsertionMode.inBody;
                return;
            }

            //A start tag whose tag name is "frameset"
            //Insert an HTML element for the token.
            //Switch the insertion mode to "in frameset".
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "frameset")
            {
                insertElement(token);
                insertionMode = enumInsertionMode.inFrameset;
                return;
            }

            //A start tag whose tag name is one of: "base", "basefont", "bgsound", "link", "meta", "noframes", "script", "style", "template", "title"
            //Parse error.
            //Push the node pointed to by the head element pointer onto the stack of open elements.
            //Process the token using the rules for the "in head" insertion mode.
            //Remove the node pointed to by the head element pointer from the stack of open elements. (It might not be the current node at this point.)
            //The head element pointer cannot be null at this point.

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("base", "basefont", "bgsound", "link", "meta", "noframes", "script", "style", "template", "title"))
            {
                if (elementPointer.head != null)
                {

                    openElements.push(elementPointer.head);

                    inHead(token);

                    openElements.popOff(elementPointer.head);

                }

            }

            //An end tag whose tag name is "template"
            //Process the token using the rules for the "in head" insertion mode.
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "template")
            {
                inHead(token);
                return;
            }

            //An end tag whose tag name is one of: "body", "html", "br"
            //Act as described in the "anything else" entry below.
            //A start tag whose tag name is "head"
            //Any other end tag
            //Parse error. Ignore the token.

            else if ((token.type == enumHtmlTokenType.StartTag && token.tagName == "head") || (token.type == enumHtmlTokenType.EndTag && (token.tagName != "body" || token.tagName != "html" || token.tagName != "br")))
            {
                onError("unexpected tag");
                return;

            }
            else
            {

                //Anything else
                //Insert an HTML element for a "body" start tag token with no attributes.
                HtmlToken faketoken = new HtmlToken(enumHtmlTokenType.StartTag);
                faketoken.tagName = "body";

                // faketoken.startIndex = token.startIndex;
                faketoken.startIndex = token.startIndex;
                faketoken.endIndex = -1;

                Element bodyelement = insertElement(faketoken);

                //Switch the insertion mode to "in body".
                //Reprocess the current token.
                insertionMode = enumInsertionMode.inBody;
                inBody(token);
                return;
            }
        }


        /// <summary>
        /// 8.2.5.4.7 The "in body" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inBody(HtmlToken token)
        {
            //A character token that is U+0000 NULL
            //Parse error. Ignore the token.
            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //Reconstruct the active formatting elements, if any.
            //Insert the token's character.
            //Any other character token
            //Reconstruct the active formatting elements, if any.
            //Insert the token's character.
            //Set the frameset-ok flag to "not ok".

            if (token.type == enumHtmlTokenType.Character)
            {
                if (string.IsNullOrEmpty(token.data) || (!string.IsNullOrEmpty(token.data) && token.data[0] == '\u0000'))
                {
                    onError("invalid null character.");

                }
                //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE

                else if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
                {
                    //Reconstruct the active formatting elements, if any.
                    //Insert the token's character.
                    activeFormatingElements.Reconstruct();
                    insertCharacter(token);

                }
                else
                {
                    activeFormatingElements.Reconstruct();
                    insertCharacter(token);
                    frameset_ok = false;

                }
                return;
            }

            //A comment token
            //Insert a comment.
            else if (token.type == enumHtmlTokenType.Comment)
            {
                insertComment(token);
                return;
            }

            //A DOCTYPE token
            //Parse error. Ignore the token.
            else if (token.type == enumHtmlTokenType.DocType)
            {
                onError("unexpected doctype declaration");
                return;
            }

            //A start tag whose tag name is "html"
            //Parse error.
            //If there is a template element on the stack of open elements, then ignore the token.
            //Otherwise, for each attribute on the token, check to see if the attribute is already present on the top element of the stack of open elements. If it is not, add the attribute and its corresponding value to that element.

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                onError("unexpected html tag");

                if (openElements.hasTag("template"))
                {
                    return;
                }
                else
                {
                    Element topElement = openElements.topElement();

                    foreach (var item in token.attributes)
                    {
                        if (!topElement.hasAttribute(item.Key))
                        {
                            topElement.setAttribute(item.Key, item.Value);
                        }
                    }

                }
                return;
            }


            //A start tag whose tag name is one of: "base", "basefont", "bgsound", "link", "meta", "noframes", "script", "style", "template", "title"
            //An end tag whose tag name is "template"
            //Process the token using the rules for the "in head" insertion mode.
            else if ((token.type == enumHtmlTokenType.EndTag && token.tagName == "template") || (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("base", "basefont", "bgsound", "link", "meta", "noframes", "script", "style", "template", "title")))
            {
                inHead(token);
                return;
            }

            //A start tag whose tag name is "body"
            //Parse error.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "body")
            {
                onError("unexpected body tag");
                //If the second element on the stack of open elements is not a body element, if the stack of open elements has only one node on it, or if there is a template element on the stack of open elements, then ignore the token. (fragment case)

                if (openElements.secondElement().tagName != "body" || openElements.length <= 1 || openElements.hasTag("template"))
                {
                    return;
                }

                else
                {

                    //Otherwise, set the frameset-ok flag to "not ok"; then, for each attribute on the token, check to see if the attribute is already present on the body element (the second element) on the stack of open elements, and if it is not, add the attribute and its corresponding value to that element.
                    frameset_ok = false;

                    Element body = openElements.secondElement();

                    foreach (var item in token.attributes)
                    {
                        if (!body.hasAttribute(item.Key))
                        {
                            body.setAttribute(item.Key, item.Value);
                        }

                    }

                }

                return;
            }


            //A start tag whose tag name is "frameset"
            //Parse error.
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "frameset")
            {
                onError("unexpected frameset tag");

                //If the stack of open elements has only one node on it, or if the second element on the stack of open elements is not a body element, then ignore the token. (fragment case)
                if (openElements.length == 1 || openElements.secondElement().tagName != "body")
                {
                    return;
                }

                //If the frameset-ok flag is set to "not ok", ignore the token.
                if (frameset_ok == false)
                {

                }

                //Otherwise, run the following steps:
                else
                {

                    //Remove the second element on the stack of open elements from its parent node, if it has one.
                    Element body = openElements.secondElement();
                    if (body.parentNode != null)
                    {
                        body.parentNode = null;
                    }
                    if (body.parentElement != null)
                    {
                        body.parentElement = null;
                    }

                    //Pop all the nodes from the bottom of the stack of open elements, from the current node up to, but not including, the root html element.
                    openElements.popOffTill("html", false);

                    //Insert an HTML element for the token.
                    insertElement(token);
                    //Switch the insertion mode to "in frameset".
                    insertionMode = enumInsertionMode.inFrameset;

                }
                return;
            }


            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //If there is a node in the stack of open elements that is not either a dd element, a dt element, an li element, a p element, a tbody element, a td element, a tfoot element, a th element, a thead element, a tr element, the body element, or the html element, then this is a parse error.
                if (openElements.length > 0)
                {
                    foreach (var oneitem in openElements.item)
                    {
                        if (oneitem.tagName != "dd" && oneitem.tagName != "dt" && oneitem.tagName != "li" && oneitem.tagName != "p" && oneitem.tagName != "tbody" && oneitem.tagName != "td" && oneitem.tagName != "tfoot" && oneitem.tagName != "th" && oneitem.tagName != "thead" && oneitem.tagName != "tr" && oneitem.tagName != "body" && oneitem.tagName != "html")
                        {
                            onError("File ends, Tags open " + oneitem.tagName);
                        }
                    }
                }

                //If the stack of template insertion modes is not empty, then process the token using the rules for the "in template" insertion mode.
                if (templateMode.length > 0)
                {
                    inTemplate(token);

                }
                else
                {

                    //Otherwise, stop parsing.
                    stopParsing();

                }
                return;
            }

            //An end tag whose tag name is "body"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "body")
            {
                //NON-W3C, mark body tag close index.
                foreach (var item in openElements.item)
                {
                    if (item.tagName == "body")
                    {
                        item.location.endTokenStartIndex = token.startIndex;
                        item.location.endTokenEndIndex = token.endIndex;
                        break;
                    }
                }


                //If the stack of open elements does not have a body element in scope, this is a parse error; ignore the token.
                if (!openElements.hasElementInScope("body", ScopeType.inScope))
                {
                    onError("did found open body tag");
                    return;
                }
                else
                {
                    //Otherwise, if there is a node in the stack of open elements that is not either a dd element, a dt element, an li element, an optgroup element, an option element, a p element, an rb element, an rp element, an rt element, an rtc element, a tbody element, a td element, a tfoot element, a th element, a thead element, a tr element, the body element, or the html element, then this is a parse error.


                    foreach (var oneitem in openElements.item)
                    {
                        if (oneitem.tagName != "dd" && oneitem.tagName != "dt" && oneitem.tagName != "li" && oneitem.tagName != "p" && oneitem.tagName != "tbody" && oneitem.tagName != "td" && oneitem.tagName != "tfoot" && oneitem.tagName != "th" && oneitem.tagName != "thead" && oneitem.tagName != "tr" && oneitem.tagName != "body" && oneitem.tagName != "html")
                        {
                            onError("tag should not open " + oneitem.tagName);
                        }
                    }

                }


                //Switch the insertion mode to "after body".
                insertionMode = enumInsertionMode.afterBody;


                return;

            }

            //An end tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "html")
            {

                //If the stack of open elements does not have a body element in scope, this is a parse error; ignore the token.
                if (!openElements.hasElementInScope("body", ScopeType.inScope))
                {
                    onError("did found open body tag");
                    return;
                }
                else
                {
                    //Otherwise, if there is a node in the stack of open elements that is not either a dd element, a dt element, an li element, an optgroup element, an option element, a p element, an rb element, an rp element, an rt element, an rtc element, a tbody element, a td element, a tfoot element, a th element, a thead element, a tr element, the body element, or the html element, then this is a parse error.

                    foreach (var oneitem in openElements.item)
                    {
                        if (oneitem.tagName != "dd" && oneitem.tagName != "dt" && oneitem.tagName != "li" && oneitem.tagName != "p" && oneitem.tagName != "tbody" && oneitem.tagName != "td" && oneitem.tagName != "tfoot" && oneitem.tagName != "th" && oneitem.tagName != "thead" && oneitem.tagName != "tr" && oneitem.tagName != "body" && oneitem.tagName != "html")
                        {
                            onError("unexpected open tag " + oneitem.tagName);
                        }
                    }

                }
                //Switch the insertion mode to "after body".
                insertionMode = enumInsertionMode.afterBody;

                //Reprocess the token.
                ProcessToken(token);
                return;
            }

            //A start tag whose tag name is one of: "address", "article", "aside", "blockquote", "center", "details", "dialog", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "main", "nav", "ol", "p", "section", "summary", "ul"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("address", "article", "aside", "blockquote", "center", "details", "dialog", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "main", "nav", "ol", "p", "section", "summary", "ul"))
            {

                //If the stack of open elements has a p element in button scope, then close a p element.

                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                //Insert an HTML element for the token.

                insertElement(token);

                return;
            }


            //A start tag whose tag name is one of: "h1", "h2", "h3", "h4", "h5", "h6"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("h1", "h2", "h3", "h4", "h5", "h6"))
            {

                //If the stack of open elements has a p element in button scope, then close a p element.
                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                //If the current node is an HTML element whose tag name is one of "h1", "h2", "h3", "h4", "h5", or "h6", then this is a parse error; pop the current node off the stack of open elements.

                Element currentnode = openElements.currentNode();
                if (currentnode.tagName.isOneOf("h1", "h2", "h3", "h4", "h5", "h6"))
                {
                    onError("unclosed H tag");

                    openElements.popOffLast(currentnode);

                }

                //Insert an HTML element for the token.
                insertElement(token);

                return;
            }

            //A start tag whose tag name is one of: "pre", "listing"

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("pre", "listing"))
            {
                //If the stack of open elements has a p element in button scope, then close a p element.
                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                //Insert an HTML element for the token.
                insertElement(token);


                //If the next token is a "LF" (U+000A) character token, then ignore that token and move on to the next one. (Newlines at the start of pre blocks are ignored as an authoring convenience.)
                HtmlToken nexttoken = nextToken();
                if (nexttoken.type == enumHtmlTokenType.Character && nexttoken.data.isOneOf('\u000A'))
                {
                    ignoreNextToken();
                }

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                return;

            }

            //A start tag whose tag name is "form"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "form")
            {

                //If the form element pointer is not null, and there is no template element on the stack of open elements, then this is a parse error; ignore the token.
                if (elementPointer.form != null && !openElements.hasElement("template"))
                {
                    onError("unexpected form start tag");
                    return;
                }

                //Otherwise:
                else
                {
                    //If the stack of open elements has a p element in button scope, then close a p element.
                    if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                    {
                        ClosePElement();
                    }

                    //Insert an HTML element for the token, and, if there is no template element on the stack of open elements, set the form element pointer to point to the element created.
                    Element formelement = insertElement(token);

                    if (!openElements.hasElement("template"))
                    {
                        elementPointer.form = formelement;
                    }


                }

                return;
            }


            //A start tag whose tag name is "li"
            //Run these steps:
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "li")
            {

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                //Initialize node to be the current node (the bottommost node of the stack).
                //Loop: If node is an li element, then run these substeps:
                //Generate implied end tags, except for li elements.
                //If the current node is not an li element, then this is a parse error.
                //Pop elements from the stack of open elements until an li element has been popped from the stack.
                //Jump to the step labeled done below.
                //If node is in the special category, but is not an address, div, or p element, then jump to the step labeled done below.
                //Otherwise, set node to the previous entry in the stack of open elements and return to the step labeled loop.

                int index = openElements.length - 1;

                for (int i = index; i >= 0; i--)
                {
                    Element currentNode = openElements.item[index];
                    if (currentNode.tagName == "li")
                    {
                        generateImpliedEndTags("li");

                        if (openElements.currentNode().tagName != "li")
                        {
                            //If the current node is not an li element, then this is a parse error.
                            //Guoqi: find a open li tag, but not the last open elements. 
                            onError(" closing li tag, unexpected open tag " + openElements.currentNode().tagName);
                        }
                        //Pop elements from the stack of open elements until an li element has been popped from the stack.
                        openElements.popOffTill("li", true);

                        ///Jump to the step labeled done below.

                        goto done;

                    }
                    else if (openElements.Special().Contains(currentNode.tagName) && currentNode.tagName != "address" && currentNode.tagName != "div" && currentNode.tagName != "p")
                    {
                        //If node is in the special category, but is not an address, div, or p element, then jump to the step labeled done below. 
                        goto done;

                    }

                }

            done:
                //Done: If the stack of open elements has a p element in button scope, then close a p element.
                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                //Finally, insert an HTML element for the token.
                insertElement(token);

                return;
            }

            //A start tag whose tag name is one of: "dd", "dt"

            else if (token.type == enumHtmlTokenType.StartTag && (token.tagName == "dd" || token.tagName == "dt"))
            {

                //Run these steps:
                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;


                //Initialize node to be the current node (the bottommost node of the stack).
                int index = openElements.length - 1;

                for (int i = index; i >= 0; i--)
                {

                    //Loop: If node is a dd element, then run these substeps:

                    Element currentNode = openElements.item[index];

                    if (currentNode.tagName == "dd")
                    {
                        //Generate implied end tags, except for dd elements.
                        //If the current node is not a dd element, then this is a parse error.
                        //Pop elements from the stack of open elements until a dd element has been popped from the stack.
                        //Jump to the step labeled done below.
                        generateImpliedEndTags("dd");
                        if (openElements.currentNode().tagName != "dd")
                        {
                            //If the current node is not a dd element, then this is a parse error.
                            onError("expected a dd tag, but found difference.");
                        }
                        //Pop elements from the stack of open elements until a dd element has been popped from the stack.
                        openElements.popOffTill("dd", true);
                        break;

                    }

                    //If node is a dt element, then run these substeps:
                    if (currentNode.tagName == "dt")
                    {
                        //Generate implied end tags, except for dt elements.
                        generateImpliedEndTags("dt");

                        //If the current node is not a dt element, then this is a parse error.
                        if (openElements.currentNode().tagName != "dt")
                        {
                            onError("closing tag for dt, but other tags are found to be open.");
                        }
                        //Pop elements from the stack of open elements until a dt element has been popped from the stack.
                        //Jump to the step labeled done below.
                        openElements.popOffTill("dt", true);
                        break;
                    }


                    //If node is in the special category, but is not an address, div, or p element, then jump to the step labeled done below.

                    if (openElements.Special().Contains(currentNode.tagName) && currentNode.tagName != "address" && currentNode.tagName != "div" && currentNode.tagName != "p")
                    {
                        break;
                    }

                    //Otherwise, set node to the previous entry in the stack of open elements and return to the step labeled loop.

                }


                //Done: If the stack of open elements has a p element in button scope, then close a p element.
                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }
                //Finally, insert an HTML element for the token.
                insertElement(token);

                return;
            }


            //A start tag whose tag name is "plaintext"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "plaintext")
            {
                //If the stack of open elements has a p element in button scope, then close a p element.
                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                //Insert an HTML element for the token.
                insertElement(token);

                //Switch the tokenizer to the PLAINTEXT state.
                tokenizer.ParseState = enumParseState.Plaintext;
                tokenizer.ScriptState = enumScriptParseState.initial;

                return;

                //Once a start tag with the tag name "plaintext" has been seen, that will be the last token ever seen other than character tokens (and the end-of-file token), because there is no way to switch out of the PLAINTEXT state.
            }


            //A start tag whose tag name is "button"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "button")
            {
                //If the stack of open elements has a button element in scope, then run these substeps:
                if (openElements.hasElementInScope("button", ScopeType.inScope))
                {
                    //Parse error.
                    onError("has button element in scope");

                    //Generate implied end tags.
                    generateImpliedEndTags();

                    //Pop elements from the stack of open elements until a button element has been popped from the stack.
                    openElements.popOffTill("button", true);

                }

                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token.
                insertElement(token);

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                return;
            }


            //An end tag whose tag name is one of: "address", "article", "aside", "blockquote", "button", "center", "details", "dialog", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "listing", "main", "nav", "ol", "pre", "section", "summary", "ul"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("address", "article", "aside", "blockquote", "button", "center", "details", "dialog", "dir", "div", "dl", "fieldset", "figcaption", "figure", "footer", "header", "hgroup", "listing", "main", "nav", "ol", "pre", "section", "summary", "ul"))
            {

                //If the stack of open elements does not have an element in scope that is an HTML element and with the same tag name as that of the token, then this is a parse error; ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inScope))
                {
                    onError(token.tagName + " in open element scope");
                    return;
                }
                else
                {

                    //Otherwise, run these steps:
                    //Generate implied end tags.

                    generateImpliedEndTags();

                    //If the current node is not an HTML element with the same tag name as that of the token, then this is a parse error.
                    if (openElements.currentNode().tagName != token.tagName)
                    {
                        onError("unclosed open tag");
                    }

                    //Pop elements from the stack of open elements until an HTML element with the same tag name as the token has been popped from the stack.

                    openElements.popOffTill(token.tagName, true);

                }
                return;
            }
            //An end tag whose tag name is "form"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "form")
            {
                //If there is no template element on the stack of open elements, then run these substeps:
                if (!openElements.hasElement("template"))
                {
                    //Let node be the element that the form element pointer is set to, or null if it is not set to an element.
                    Element formnode = null;
                    if (elementPointer.form != null)
                    {
                        formnode = elementPointer.form;
                        formnode.location.endTokenStartIndex = token.startIndex;
                        formnode.location.endTokenEndIndex = token.endIndex;
                    }

                    //Set the form element pointer to null. Otherwise, let node be null.
                    elementPointer.form = null;

                    //If node is null or if the stack of open elements does not have node in scope, then this is a parse error; abort these steps and ignore the token.
                    if (formnode == null || !openElements.hasElementInScope(formnode.tagName, ScopeType.inScope))
                    {
                        onError("did not find an open tag ");
                        return;
                    }

                    //Generate implied end tags.
                    generateImpliedEndTags();
                    //If the current node is not node, then this is a parse error.
                    if (openElements.currentNode().tagName != "form")
                    {
                        onError("form node expected in stack of open element");
                    }
                    //Remove node from the stack of open elements.
                    openElements.popOff(formnode.tagName);

                }
                //If there is a template element on the stack of open elements, then run these substeps instead:
                else
                {

                    //If the stack of open elements does not have a form element in scope, then this is a parse error; abort these steps and ignore the token.
                    if (!openElements.hasElementInScope("form", ScopeType.inScope))
                    {
                        onError("does not have form element in scope");
                        return;
                    }

                    //Generate implied end tags.
                    generateImpliedEndTags();

                    //If the current node is not a form element, then this is a parse error.
                    if (openElements.currentNode().tagName != "form")
                    {
                        onError("form node expected in stack of open element");
                    }
                    //Pop elements from the stack of open elements until a form element has been popped from the stack.
                    openElements.popOffTill("form", true);
                }
                return;
            }

            //An end tag whose tag name is "p"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "p")
            {
                //If the stack of open elements does not have a p element in button scope, then this is a parse error; insert an HTML element for a "p" start tag token with no attributes.
                if (!openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    onError("an p end tag found, but open tag p has been implied closed or not exists. This often happen with miss nested tags");
                    HtmlToken newtoken = new HtmlToken(enumHtmlTokenType.StartTag);
                    newtoken.startIndex = token.startIndex;
                    newtoken.endIndex = -1;

                    newtoken.tagName = "p";
                    insertElement(newtoken);

                }

                //Close a p element.
                ClosePElement();

                return;

            }

            //An end tag whose tag name is "li"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "li")
            {
                //If the stack of open elements does not have an li element in list item scope, then this is a parse error; ignore the token.
                if (!openElements.hasElementInScope("li", ScopeType.inListItemScope))
                {
                    onError("an li end tag, but there is no start li tag logged.");
                    return;
                }
                else
                {

                    //Otherwise, run these steps:
                    //Generate implied end tags, except for li elements.
                    generateImpliedEndTags("li");

                    //If the current node is not an li element, then this is a parse error.
                    if (openElements.currentNode().tagName != "li")
                    {
                        onError("unexpected tags included between li tags: " + openElements.currentNode().tagName);
                    }

                    //Pop elements from the stack of open elements until an li element has been popped from the stack.
                    openElements.popOffTill("li", true);
                }
                return;
            }
            //An end tag whose tag name is one of: "dd", "dt"
            else if (token.type == enumHtmlTokenType.EndTag && (token.tagName.isOneOf("dd", "dt")))
            {
                //If the stack of open elements does not have an element in scope that is an HTML element and with the same tag name as that of the token, then this is a parse error; ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inScope))
                {
                    onError(token.tagName + " in scope not found");
                }
                else
                {
                    //Otherwise, run these steps:

                    //Generate implied end tags, except for HTML elements with the same tag name as the token.
                    generateImpliedEndTags(token.tagName);

                    //If the current node is not an HTML element with the same tag name as that of the token, then this is a parse error.
                    if (openElements.currentNode().tagName != token.tagName)
                    {
                        onError(token.tagName + " expected in current open element");
                    }

                    //Pop elements from the stack of open elements until an HTML element with the same tag name as the token has been popped from the stack.
                    openElements.popOffTill(token.tagName, true);
                }
                return;
            }

            //An end tag whose tag name is one of: "h1", "h2", "h3", "h4", "h5", "h6"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("h1", "h2", "h3", "h4", "h5", "h6"))
            {

                //If the stack of open elements does not have an element in scope that is an HTML element and whose tag name is one of "h1", "h2", "h3", "h4", "h5", or "h6", then this is a parse error; ignore the token.
                if (!openElements.hasOneOfElementsInScope(ScopeType.inScope, "h1", "h2", "h3", "h4", "h5", "h6"))
                {
                    onError("h1-h6 tags in scope not found");
                    return;
                }
                else
                {
                    //Otherwise, run these steps:
                    //Generate implied end tags.
                    generateImpliedEndTags();

                    //If the current node is not an HTML element with the same tag name as that of the token, then this is a parse error.
                    if (openElements.currentNode().tagName != token.tagName)
                    {
                        onError(token.tagName + " expected in current open element");
                    }

                    //Pop elements from the stack of open elements until an HTML element whose tag name is one of "h1", "h2", "h3", "h4", "h5", or "h6" has been popped from the stack.

                    openElements.popOffTillOneOf(true, "h1", "h2", "h3", "h4", "h5", "h6");

                }
                return;
            }
            //An end tag whose tag name is "sarcasm"
            //Take a deep breath, then act as described in the "any other end tag" entry below.
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "sarcasm")
            {
                // deep breath
                goto AnyOtherEndTag;
            }

            //A start tag whose tag name is "a"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "a")
            {
                //If the list of active formatting elements contains an a element between the end of the list and the last marker on the list (or the start of the list if there is no marker on the list), 
                bool isContains = false;
                int index = activeFormatingElements.length - 1;
                for (int i = index; i >= 0; i--)
                {
                    if (activeFormatingElements.item[i].isMarker)
                    {
                        break;
                    }

                    if (activeFormatingElements.item[i].element != null && activeFormatingElements.item[i].element.tagName == "a")
                    {
                        isContains = true;
                        break;
                    }
                }

                if (isContains)
                {

                    //then this is a parse error; 
                    ///run the adoption agency algorithm for the tag name "a", then remove that element from the list of active formatting elements and the stack of open elements if the adoption agency algorithm didn't already remove it (it might not have if the element is not in table scope).

                    bool anyend = adoptionAgency("a");

                    if (!anyend)
                    {
                        ///****COPY FROM AnyOtherEndTag
                        //Any other start tag
                        //Reconstruct the active formatting elements, if any.
                        activeFormatingElements.Reconstruct();

                        //Insert an HTML element for the token.
                        insertElement(token);
                    }

                    activeFormatingElements.Remove("a", false);
                    openElements.popOff("a");


                }
                //In the non-conforming stream <a href="a">a<table><a href="b">b</table>x, the first a element would be closed upon seeing the second one, and the "x" character would be inside a link to "b", not to "a". This is despite the fact that the outer a element is not in table scope (meaning that a regular </a> end tag at the start of the table wouldn't close the outer a element). The result is that the two a elements are indirectly nested inside each other — non-conforming markup will often result in non-conforming DOMs when parsed.

                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token. Push onto the list of active formatting elements that element.

                Element element = insertElement(token);
                activeFormatingElements.Push(element, token);
                return;

            }


            //A start tag whose tag name is one of: "b", "big", "code", "em", "font", "i", "s", "small", "strike", "strong", "tt", "u"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("b", "big", "code", "em", "font", "i", "s", "small", "strike", "strong", "tt", "u"))
            {
                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token. Push onto the list of active formatting elements that element.
                Element element = insertElement(token);
                activeFormatingElements.Push(element, token);

                return;

            }
            //A start tag whose tag name is "nobr"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "nobr")
            {
                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //If the stack of open elements has a nobr element in scope, then this is a parse error; run the adoption agency algorithm for the tag name "nobr", then once again reconstruct the active formatting elements, if any.

                if (openElements.hasElementInScope("nobr", ScopeType.inScope))
                {
                    onError("nobr in scope");
                    bool adoptionok = adoptionAgency("nobr");
                    if (!adoptionok)
                    {
                        ///****COPY FROM AnyOtherEndTag
                        //Any other start tag
                        //Reconstruct the active formatting elements, if any.
                        activeFormatingElements.Reconstruct();

                        //Insert an HTML element for the token.
                        insertElement(token);
                    }
                    activeFormatingElements.Reconstruct();
                }

                //Insert an HTML element for the token. Push onto the list of active formatting elements that element.
                Element element = insertElement(token);
                activeFormatingElements.Push(element, token);

                return;

            }

            //An end tag whose tag name is one of: "a", "b", "big", "code", "em", "font", "i", "nobr", "s", "small", "strike", "strong", "tt", "u"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("a", "b", "big", "code", "em", "font", "i", "nobr", "s", "small", "strike", "strong", "tt", "u"))
            {

                //Run the adoption agency algorithm for the token's tag name.
                bool ok = adoptionAgency(token.tagName);
                if (!ok)
                {
                    ///****COPY FROM AnyOtherEndTag
                    //Any other start tag
                    //Reconstruct the active formatting elements, if any.
                    activeFormatingElements.Reconstruct();

                    //Insert an HTML element for the token.
                    insertElement(token);
                }
                return;

            }

            //A start tag whose tag name is one of: "applet", "marquee", "object"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("applet", "marquee", "object"))
            {

                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token.
                insertElement(token);

                //Insert a marker at the end of the list of active formatting elements.
                activeFormatingElements.insertMarker();

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                return;

            }

            //An end tag token whose tag name is one of: "applet", "marquee", "object"

            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("applet", "marquee", "object"))
            {
                //If the stack of open elements does not have an element in scope that is an HTML element and with the same tag name as that of the token, then this is a parse error; ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inScope))
                {
                    onError(token.tagName + " is not in scope");
                    return;
                }
                else
                {

                    //Otherwise, run these steps:

                    //Generate implied end tags.
                    generateImpliedEndTags();

                    //If the current node is not an HTML element with the same tag name as that of the token, then this is a parse error.
                    if (openElements.currentNode().tagName != token.tagName)
                    {
                        onError(token.tagName + " expected in the current open element.");
                    }

                    //Pop elements from the stack of open elements until an HTML element with the same tag name as the token has been popped from the stack.
                    openElements.popOffTill(token.tagName, true);

                    //Clear the list of active formatting elements up to the last marker.
                    activeFormatingElements.clearUpToLastMarker();
                    return;
                }
            }

            //A start tag whose tag name is "table"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "table")
            {
                //If the Document is not set to quirks mode, and the stack of open elements has a p element in button scope, then close a p element.
                if (!doc.isQuirksMode && openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                insertElement(token);
                //Insert an HTML element for the token.

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                //Switch the insertion mode to "in table".
                insertionMode = enumInsertionMode.inTable;
                return;
            }

            //An end tag whose tag name is "br"
            //Parse error. Act as described in the next entry, as if this was a "br" start tag token, rather than an end tag token.
            //A start tag whose tag name is one of: "area", "br", "embed", "img", "keygen", "wbr"

            else if ((token.type == enumHtmlTokenType.EndTag && token.tagName == "br") || token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("area", "br", "embed", "img", "keygen", "wbr"))
            {

                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
                Element element = insertElement(token);
                openElements.popOff(element);

                //Acknowledge the token's self-closing flag, if it is set.
                acknowledgedSelfClosing(token);

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                return;

            }


            //A start tag whose tag name is "input"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "input")
            {
                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
                Element element = insertElement(token);
                openElements.popOff(element);

                //Acknowledge the token's self-closing flag, if it is set.
                acknowledgedSelfClosing(token);

                //If the token does not have an attribute with the name "type", or if it does, but that attribute's value is not an ASCII case-insensitive match for the string "hidden", then: set the frameset-ok flag to "not ok".

                if (!token.attributes.ContainsKey("type"))
                {
                    frameset_ok = false;
                }
                else
                {
                    string value = token.attributes["type"];
                    if (value != null && value.ToLower() != "hidden")
                    {
                        frameset_ok = false;
                    }
                }
                return;
            }

            //A start tag whose tag name is one of: "param", "source", "track"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("param", "source", "track"))
            {
                //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
                Element element = insertElement(token);
                openElements.popOff(element);

                //Acknowledge the token's self-closing flag, if it is set.
                acknowledgedSelfClosing(token);
                return;
            }


            //A start tag whose tag name is "hr"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "hr")
            {
                //If the stack of open elements has a p element in button scope, then close a p element.
                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
                Element element = insertElement(token);
                openElements.popOff(element);

                //Acknowledge the token's self-closing flag, if it is set.
                acknowledgedSelfClosing(token);

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;
                return;
            }

            //A start tag whose tag name is "image"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "image")
            {
                //Parse error. Change the token's tag name to "img" and reprocess it. (Don't ask.)
                onError("image should be img");
                token.tagName = "img";
                inBody(token);
                return;
            }

            //A start tag whose tag name is "isindex"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "isindex")
            {
                //Parse error.
                onError("");
                return;
                //If there is no template element on the stack of open elements and the form element pointer is not null, then ignore the token.

                //Otherwise:

                //Acknowledge the token's self-closing flag, if it is set.

                //Set the frameset-ok flag to "not ok".

                //If the stack of open elements has a p element in button scope, then close a p element.

                //Insert an HTML element for a "form" start tag token with no attributes, and, if there is no template element on the stack of open elements, set the form element pointer to point to the element created.

                //If the token has an attribute called "action", set the action attribute on the resulting form element to the value of the "action" attribute of the token.

                //Insert an HTML element for an "hr" start tag token with no attributes. Immediately pop the current node off the stack of open elements.

                //Reconstruct the active formatting elements, if any.

                //Insert an HTML element for a "label" start tag token with no attributes.

                //Insert characters (see below for what they should say).

                //Insert an HTML element for an "input" start tag token with all the attributes from the "isindex" token except "name", "action", and "prompt", and with an attribute named "name" with the value "isindex". (This creates an input element with the name attribute set to the magic balue "isindex".) Immediately pop the current node off the stack of open elements.

                //Insert more characters (see below for what they should say).

                //Pop the current node (which will be the label element created earlier) off the stack of open elements.

                //Insert an HTML element for an "hr" start tag token with no attributes. Immediately pop the current node off the stack of open elements.

                //Pop the current node (which will be the form element created earlier) off the stack of open elements, and, if there is no template element on the stack of open elements, set the form element pointer back to null.

                //Prompt: If the token has an attribute with the name "prompt", then the first stream of characters must be the same string as given in that attribute, and the second stream of characters must be empty. Otherwise, the two streams of character tokens together should, together with the input element, express the equivalent of "This is a searchable index. Enter search keywords: (input field)" in the user's preferred language.

            }


            //A start tag whose tag name is "textarea"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "textarea")
            {

                //Run these steps:

                //Insert an HTML element for the token.
                insertElement(token);

                //If the next token is a "LF" (U+000A) character token, then ignore that token and move on to the next one. (Newlines at the start of textarea elements are ignored as an authoring convenience.)
                HtmlToken nexttoken = nextToken();
                if (nexttoken.type == enumHtmlTokenType.Character && nexttoken.data.isOneOf('\u000A'))
                {
                    ignoreNextToken();
                }

                //Switch the tokenizer to the RCDATA state.
                tokenizer.ParseState = enumParseState.RCDATA;
                tokenizer.ScriptState = enumScriptParseState.initial;

                //Let the original insertion mode be the current insertion mode.
                originalInsertionMode = insertionMode;

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                //Switch the insertion mode to "text".
                insertionMode = enumInsertionMode.text;

                return;
            }

            //A start tag whose tag name is "xmp"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "xmp")
            {
                //If the stack of open elements has a p element in button scope, then close a p element.
                if (openElements.hasElementInScope("p", ScopeType.inButtonScope))
                {
                    ClosePElement();
                }

                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                //Follow the generic raw text element parsing algorithm.
                rawTextParsing(token);
                return;

            }

            //A start tag whose tag name is "iframe"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "iframe")
            {
                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                //Follow the generic raw text element parsing algorithm.
                rawTextParsing(token);
                return;
            }
            //A start tag whose tag name is "noembed"
            //A start tag whose tag name is "noscript", if the scripting flag is enabled
            else if ((token.type == enumHtmlTokenType.StartTag && token.tagName == "noembed") || (this.scripting && token.type == enumHtmlTokenType.StartTag && token.tagName == "noscript"))
            {
                //Follow the generic raw text element parsing algorithm.
                rawTextParsing(token);
                return;
            }
            //A start tag whose tag name is "select"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "select")
            {
                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token.
                insertElement(token);

                //Set the frameset-ok flag to "not ok".
                frameset_ok = false;

                //If the insertion mode is one of "in table", "in caption", "in table body", "in row", or "in cell", then switch the insertion mode to "in select in table". Otherwise, switch the insertion mode to "in select".

                if (insertionMode == enumInsertionMode.inTable || insertionMode == enumInsertionMode.inCaption || insertionMode == enumInsertionMode.inTableBody || insertionMode == enumInsertionMode.inRow || insertionMode == enumInsertionMode.inCell)
                {
                    insertionMode = enumInsertionMode.inSelectInTable;
                }
                else
                {
                    insertionMode = enumInsertionMode.inSelect;
                }

                return;
            }

            //A start tag whose tag name is one of: "optgroup", "option"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("optgroup", "option"))
            {
                //If the current node is an option element, then pop the current node off the stack of open elements.
                if (openElements.currentNode().tagName == "option")
                {
                    openElements.popOffLast(openElements.currentNode());
                }

                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token.
                insertElement(token);
                return;
            }

            //A start tag whose tag name is one of: "rb", "rp", "rtc"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("rb", "rp", "rtc"))
            {
                //If the stack of open elements has a ruby element in scope, then generate implied end tags. If the current node is not then a ruby element, this is a parse error.
                if (openElements.hasElementInScope("ruby", ScopeType.inScope))
                {
                    generateImpliedEndTags();

                    if (openElements.currentNode().tagName != "ruby")
                    {
                        onError("expecting a ruby current node");
                    }
                }

                //Insert an HTML element for the token.
                insertElement(token);
                return;
            }

            //A start tag whose tag name is "rt"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "rt")
            {

                //If the stack of open elements has a ruby element in scope, then generate implied end tags, except for rtc elements. If the current node is not then a ruby element or an rtc element, this is a parse error.
                if (openElements.hasElementInScope("ruby", ScopeType.inScope))
                {
                    generateImpliedEndTags("rtc");

                    if (openElements.currentNode().tagName != "ruby" && openElements.currentNode().tagName != "rtc")
                    {
                        onError("expecting a ruby current node");
                    }
                }

                //Insert an HTML element for the token.
                insertElement(token);
                return;
            }

            //A start tag whose tag name is "math"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "math")
            {

                onError("");

                goto AnyOtherStartTag;

                //Reconstruct the active formatting elements, if any.

                //Adjust MathML attributes for the token. (This fixes the case of MathML attributes that are not all lowercase.)

                //Adjust foreign attributes for the token. (This fixes the use of namespaced attributes, in particular XLink.)

                //Insert a foreign element for the token, in the MathML namespace.

                //If the token has its self-closing flag set, pop the current node off the stack of open elements and acknowledge the token's self-closing flag.

            }

            //A start tag whose tag name is "svg"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "svg")
            {
                onError("");

                goto AnyOtherStartTag;

                //Reconstruct the active formatting elements, if any.

                //Adjust SVG attributes for the token. (This fixes the case of SVG attributes that are not all lowercase.)

                //Adjust foreign attributes for the token. (This fixes the use of namespaced attributes, in particular XLink in SVG.)

                //Insert a foreign element for the token, in the SVG namespace.

                //If the token has its self-closing flag set, pop the current node off the stack of open elements and acknowledge the token's self-closing flag.

            }

            //A start tag whose tag name is one of: "caption", "col", "colgroup", "frame", "head", "tbody", "td", "tfoot", "th", "thead", "tr"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("caption", "col", "colgroup", "frame", "head", "tbody", "td", "tfoot", "th", "thead", "tr"))
            {
                //Parse error. Ignore the token.
                onError("unexpected start tag whose tag name is one of: caption, col, colgroup, frame, head, tbody, td, tfoot, th, thead, tr");
                FixWithoutTable(token);
                return;
            }
            else if (token.type == enumHtmlTokenType.StartTag)
            {

                goto AnyOtherStartTag;

            }
            else if (token.type == enumHtmlTokenType.EndTag)
            {
                goto AnyOtherEndTag;

            }
            else
            {
                return;
            }


        AnyOtherStartTag:
            {
                //Any other start tag
                //Reconstruct the active formatting elements, if any.
                activeFormatingElements.Reconstruct();

                //Insert an HTML element for the token.
                insertElement(token);
                return;
                //This element will be an ordinary element.

            }

        AnyOtherEndTag:
            {
                //Any other end tag
                //Run these steps:

                //Initialize node to be the current node (the bottommost node of the stack).
                int itemindex = openElements.length - 1;
                if (itemindex < 0)
                {
                    return;
                }

                Element node = openElements.item[itemindex];

            //Loop: 
            myLoop:

                //If node is an HTML element with the same tag name as the token, then:

                if (node.tagName == token.tagName)
                {

                    //Generate implied end tags, except for HTML elements with the same tag name as the token.
                    generateImpliedEndTags(token.tagName);

                    //If node is not the current node, then this is a parse error.
                    if (!IsSameDomElement(openElements.currentNode(), node))
                    {
                        onError("expecting element " + node.tagName);
                    }

                    //Pop all the nodes from the current node up to node, including node, then stop these steps.
                    openElements.popOffTill(node.tagName, true);
                    return;
                }
                else
                {
                    //Otherwise, if node is in the special category, then this is a parse error; ignore the token, and abort these steps.
                    if (openElements.Special().Contains(node.tagName))
                    {
                        onError("tag in special category");
                        return;
                    }

                }


                //Set node to the previous entry in the stack of open elements.
                itemindex = itemindex - 1;
                if (itemindex < 0)
                {
                    return;
                }

                node = openElements.item[itemindex];

                goto myLoop;
                //Return to the step labeled loop.


            }


        }

        /// <summary>
        /// 8.2.5.4.8 The "text" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void Text(HtmlToken token)
        {

            //When the user agent is to apply the rules for the "text" insertion mode, the user agent must handle the token as follows:

            //A character token
            if (token.type == enumHtmlTokenType.Character)
            {
                //Insert the token's character.
                //This can never be a U+0000 NULL character; the tokenizer converts those to U+FFFD REPLACEMENT CHARACTER characters.
                insertCharacter(token);
                return;
            }

            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //Parse error.
                onError("unexpected EOF");


                //If the current node is a script element, mark the script element as "already started".
                Element currentnode = openElements.currentNode();

                if (currentnode != null)
                {
                    if (currentnode.GetType().IsEquivalentTo(typeof(HTMLScriptElement)))
                    {
                        //TODO: Script not supported now.  
                    }
                    //Pop the current node off the stack of open elements.
                    openElements.popOffLast(currentnode);

                    //Switch the insertion mode to the original insertion mode and reprocess the token.
                    insertionMode = originalInsertionMode;
                    ProcessToken(token);
                }

                return;

            }


            //An end tag whose tag name is "script"

            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "script")
            {
                //TODO: Script execution will be hanlded later. 


                //Perform a microtask checkpoint.

                //Provide a stable state.


                //Let script be the current node (which will be a script element).
                Element script = openElements.currentNode();

                //Pop the current node off the stack of open elements.
                openElements.popOff(script);

                //Switch the insertion mode to the original insertion mode.
                insertionMode = originalInsertionMode;

                // GUOQI: looks like document bug. add below. 
                tokenizer.ParseState = enumParseState.DATA;

                return;

                //Let the old insertion point have the same value as the current insertion point. Let the insertion point be just before the next input character.

                //Increment the parser's script nesting level by one.

                //Prepare the script. This might cause some script to execute, which might cause new characters to be inserted into the tokenizer, and might cause the tokenizer to output more tokens, resulting in a reentrant invocation of the parser.

                //Decrement the parser's script nesting level by one. If the parser's script nesting level is zero, then set the parser pause flag to false.

                //Let the insertion point have the value of the old insertion point. (In other words, restore the insertion point to its previous value. This value might be the "undefined" value.)

                //At this stage, if there is a pending parsing-blocking script, then:

                //If the script nesting level is not zero:
                //Set the parser pause flag to true, and abort the processing of any nested invocations of the tokenizer, yielding control back to the caller. (Tokenization will resume when the caller returns to the "outer" tree construction stage.)

                //The tree construction stage of this particular parser is being called reentrantly, say from a call to document.write().

                //Otherwise:
                //Run these steps:

                //Let the script be the pending parsing-blocking script. There is no longer a pending parsing-blocking script.

                //Block the tokenizer for this instance of the HTML parser, such that the event loop will not run tasks that invoke the tokenizer.

                //If the parser's Document has a style sheet that is blocking scripts or the script's "ready to be parser-executed" flag is not set: spin the event loop until the parser's Document has no style sheet that is blocking scripts and the script's "ready to be parser-executed" flag is set.

                //If this parser has been aborted in the meantime, abort these steps.

                //This could happen if, e.g., while the spin the event loop algorithm is running, the browsing context gets closed, or the document.open() method gets invoked on the Document.

                //Unblock the tokenizer for this instance of the HTML parser, such that tasks that invoke the tokenizer can again be run.

                //Let the insertion point be just before the next input character.

                //Increment the parser's script nesting level by one (it should be zero before this step, so this sets it to one).

                //Execute the script.

                //Decrement the parser's script nesting level by one. If the parser's script nesting level is zero (which it always should be at this point), then set the parser pause flag to false.

                //Let the insertion point be undefined again.

                //If there is once again a pending parsing-blocking script, then repeat these steps from step 1.
            }

            else
            {

                //Any other end tag
                //Pop the current node off the stack of open elements.
                Element currentnode = openElements.currentNode();
                openElements.popOff(currentnode);

                //Switch the insertion mode to the original insertion mode.
                insertionMode = originalInsertionMode;

                // GUOQI: looks like document bug. add below. 
                tokenizer.ParseState = enumParseState.DATA;

            }
        }


        /// <summary>
        /// 8.2.5.4.9 The "in table" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inTable(HtmlToken token)
        {
            //When the user agent is to apply the rules for the "in table" insertion mode, the user agent must handle the token as follows:
            Element currentnode = openElements.currentNode();

            //A character token, if the current node is table, tbody, tfoot, thead, or tr element
            if (token.type == enumHtmlTokenType.Character && currentnode.tagName.isOneOf("table", "tbody", "tfoot", "thead", "tr"))
            {
                //Let the pending table character tokens be an empty list of tokens.
                pendingTableCharacterTokens.Clear();

                //Let the original insertion mode be the current insertion mode.

                originalInsertionMode = insertionMode;

                //Switch the insertion mode to "in table text" and reprocess the token.
                insertionMode = enumInsertionMode.inTableText;
                inTableText(token);
                return;
            }

            //A comment token
            else if (token.type == enumHtmlTokenType.Comment)
            {
                //Insert a comment.
                insertComment(token);
                return;
            }
            else if (token.type == enumHtmlTokenType.DocType)
            {
                //A DOCTYPE token
                //Parse error. Ignore the token.
                onError("invalid Doctype token");
                return;
            }

            //A start tag whose tag name is "caption"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "caption")
            {
                //Clear the stack back to a table context. (See below.)
                openElements.ClearStackBackToTableContext();

                //Insert a marker at the end of the list of active formatting elements.
                activeFormatingElements.insertMarker();

                //Insert an HTML element for the token, then switch the insertion mode to "in caption".
                insertElement(token);
                insertionMode = enumInsertionMode.inCaption;
                return;
            }

            //A start tag whose tag name is "colgroup"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "colgroup")
            {
                //Clear the stack back to a table context. (See below.)
                openElements.ClearStackBackToTableContext();

                //Insert an HTML element for the token, then switch the insertion mode to "in column group".
                insertElement(token);
                insertionMode = enumInsertionMode.inColumnGroup;

                return;

            }
            //A start tag whose tag name is "col"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "col")
            {
                //Clear the stack back to a table context. (See below.)
                openElements.ClearStackBackToTableContext();

                //Insert an HTML element for a "colgroup" start tag token with no attributes, then switch the insertion mode to "in column group".
                HtmlToken faketoken = new HtmlToken(enumHtmlTokenType.StartTag);
                faketoken.tagName = "colgroup";
                faketoken.startIndex = token.startIndex;
                faketoken.endIndex = -1;

                insertElement(faketoken);

                insertionMode = enumInsertionMode.inColumnGroup;

                //Reprocess the current token.
                inColumnGroup(token);
                return;

            }

            //A start tag whose tag name is one of: "tbody", "tfoot", "thead"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("tbody", "tfoot", "thead"))
            {
                //Clear the stack back to a table context. (See below.)
                openElements.ClearStackBackToTableContext();

                //Insert an HTML element for the token, then switch the insertion mode to "in table body".
                insertElement(token);

                insertionMode = enumInsertionMode.inTableBody;
                return;

            }

            //A start tag whose tag name is one of: "td", "th", "tr"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("td", "th", "tr"))
            {

                //Clear the stack back to a table context. (See below.)
                openElements.ClearStackBackToTableContext();

                //Insert an HTML element for a "tbody" start tag token with no attributes, then switch the insertion mode to "in table body".

                HtmlToken faketoken = new HtmlToken(enumHtmlTokenType.StartTag);
                faketoken.tagName = "tbody";
                faketoken.startIndex = token.startIndex;
                faketoken.endIndex = -1;
                insertElement(faketoken);

                insertionMode = enumInsertionMode.inTableBody;

                //Reprocess the current token.
                inTableBody(token);
                return;

            }

            //A start tag whose tag name is "table"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "table")
            {

                //Parse error.
                onError("unexpected table start tag");


                //If the stack of open elements does not have a table element in table scope, 
                if (!openElements.hasElementInScope("table", ScopeType.inTableScope))
                {

                    //ignore the token.
                    return;
                }
                //Otherwise:
                else
                {
                    //Pop elements from this stack until a table element has been popped from the stack.
                    openElements.popOffTill("table", true);
                    //Reset the insertion mode appropriately.
                    resetInsertionMode();
                    //Reprocess the token.
                    ProcessToken(token);
                    return;
                }
            }

            //An end tag whose tag name is "table"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "table")
            {
                //If the stack of open elements does not have a table element in table scope, this is a parse error; ignore the token.
                if (!openElements.hasElementInScope("table", ScopeType.inTableScope))
                {
                    onError("table in table scope expected");
                    return;
                }
                else
                {
                    //Otherwise:

                    //Pop elements from this stack until a table element has been popped from the stack.
                    openElements.popOffTill("table", true);

                    //Reset the insertion mode appropriately.
                    resetInsertionMode();

                }
                return;

            }

            //An end tag whose tag name is one of: "body", "caption", "col", "colgroup", "html", "tbody", "td", "tfoot", "th", "thead", "tr"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("body", "caption", "col", "colgroup", "html", "tbody", "td", "tfoot", "th", "thead", "tr"))
            {
                //Parse error. Ignore the token.
                onError(token.tagName + " unexpected");
                return;
            }
            //A start tag whose tag name is one of: "style", "script", "template"
            //An end tag whose tag name is "template"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("style", "script", "template") || token.type == enumHtmlTokenType.EndTag && token.tagName == "template")
            {
                //Process the token using the rules for the "in head" insertion mode.
                inHead(token);
                return;
            }
            //A start tag whose tag name is "input"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "input")
            {

                //If the token does not have an attribute with the name "type", or if it does, but that attribute's value is not an ASCII case-insensitive match for the string "hidden", then: act as described in the "anything else" entry below.
                if (!token.attributes.ContainsKey("type") || token.attributes["type"].ToLower() != "hidden")
                {
                    goto inTableAnythingElse;

                }
                else
                {

                    //Otherwise:

                    //Parse error.
                    onError("invalid input token");

                    //Insert an HTML element for the token.
                    insertElement(token);

                    //Pop that input element off the stack of open elements.
                    openElements.popOff("input");

                    //Acknowledge the token's self-closing flag, if it is set.
                    acknowledgedSelfClosing(token);
                }
                return;
            }

            //A start tag whose tag name is "form"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "form")
            {

                //Parse error.
                onError("start tag form unexpected");

                //If there is a template element on the stack of open elements, or if the form element pointer is not null, ignore the token.
                if (openElements.hasTag("template") || elementPointer.form != null)
                {
                    return;
                }
                else
                {

                    //Otherwise:

                    //Insert an HTML element for the token, and set the form element pointer to point to the element created.
                    Element formelement = insertElement(token);
                    elementPointer.form = formelement;

                    //Pop that form element off the stack of open elements.

                    openElements.popOffLast(formelement);
                    return;

                }
            }

            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
            }
            else
            {
                goto inTableAnythingElse;

            }

        inTableAnythingElse:
            {

                //Anything else

                onError("non table element");

                //Parse error. Enable foster parenting, process the token using the rules for the "in body" insertion mode, and then disable foster parenting.
                fosterParent = true;
                inBody(token);
                fosterParent = false;

            }

        }


        /// <summary>
        /// 8.2.5.4.10 The "in table text" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inTableText(HtmlToken token)
        {
            //A character token that is U+0000 NULL
            if (token.type == enumHtmlTokenType.Character)
            {
                if (!string.IsNullOrEmpty(token.data) && token.data[0] == '\u0000')
                {
                    //Parse error. Ignore the token.
                    onError("unexpected null character");
                    return;
                }

                else
                {

                    //Any other character token
                    //Append the character token to the pending table character tokens list.
                    pendingTableCharacterTokens.Add(token);
                    return;
                }
            }

            //Anything else
            else
            {
                //If any of the tokens in the pending table character tokens list are character tokens that are not space characters, then reprocess the character tokens in the pending table character tokens list using the rules given in the "anything else" entry in the "in table" insertion mode.

                /// copy from in table.
                /// *******************************************************************************
                /// Anything else: Parse error. Enable foster parenting, process the token using the rules for the "in body" insertion mode, and then disable foster parenting.
                /// *******************************************************************************

                //Otherwise, insert the characters given by the pending table character tokens list.

                ///TODO: please check whether we need to remove the items from pendingTable or not. 

                bool ifAny = false;
                foreach (var item in pendingTableCharacterTokens)
                {
                    if (!item.data.isSpaceCharacter())
                    {
                        ifAny = true;
                        break;
                    }
                }

                if (ifAny)
                {
                    fosterParent = true;
                    foreach (var item in pendingTableCharacterTokens)
                    {
                        inBody(token);
                    }
                    fosterParent = false;
                }
                else
                {
                    foreach (var item in pendingTableCharacterTokens)
                    {
                        insertCharacter(item);
                    }

                }


                //Switch the insertion mode to the original insertion mode and reprocess the token.
                insertionMode = originalInsertionMode;
                ProcessToken(token);
            }
        }

        /// <summary>
        /// 8.2.5.4.11 The "in caption" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inCaption(HtmlToken token)
        {

            //An end tag whose tag name is "caption"
            if (token.type == enumHtmlTokenType.EndTag && token.tagName == "caption")
            {
                //If the stack of open elements does not have a caption element in table scope, this is a parse error; ignore the token. (fragment case)
                if (!openElements.hasElementInScope("caption", ScopeType.inTableScope))
                {
                    return;
                }
                else
                {
                    //Otherwise:

                    //Generate implied end tags.
                    generateImpliedEndTags();

                    //Now, if the current node is not a caption element, then this is a parse error.
                    if (openElements.currentNode().tagName != "caption")
                    {
                        onError("expect caption in the current node");
                    }

                    //Pop elements from this stack until a caption element has been popped from the stack.
                    openElements.popOffTill("caption", true);

                    //Clear the list of active formatting elements up to the last marker.
                    activeFormatingElements.clearUpToLastMarker();

                    //Switch the insertion mode to "in table".
                    insertionMode = enumInsertionMode.inTable;
                }

                return;
            }

            //A start tag whose tag name is one of: "caption", "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr"
            //An end tag whose tag name is "table"
            else if ((token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("caption", "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr")) || (token.type == enumHtmlTokenType.EndTag && token.tagName == "table"))
            {

                //Parse error.
                onError(token.tagName + " not expected");

                //If the stack of open elements does not have a caption element in table scope, ignore the token. (fragment case)
                if (!openElements.hasElementInScope("caption", ScopeType.inTableScope))
                {
                    return;
                }
                else
                {
                    //Otherwise:

                    //Pop elements from this stack until a caption element has been popped from the stack.
                    openElements.popOffTill("caption", true);

                    //Clear the list of active formatting elements up to the last marker.
                    activeFormatingElements.clearUpToLastMarker();

                    //Switch the insertion mode to "in table".
                    insertionMode = enumInsertionMode.inTable;

                    //Reprocess the token.
                    ProcessToken(token);
                    return;
                }
            }


            //An end tag whose tag name is one of: "body", "col", "colgroup", "html", "tbody", "td", "tfoot", "th", "thead", "tr"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("body", "col", "colgroup", "html", "tbody", "td", "tfoot", "th", "thead", "tr"))
            {
                //Parse error. Ignore the token.
                onError("An end tag whose tag name is one of: body, col,colgroup, html, tbody, td, tfoot, th, thead, tr");
                return;
            }
            else
            {
                //Anything else
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }

        }

        /// <summary>
        /// 8.2.5.4.12 The "in column group" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inColumnGroup(HtmlToken token)
        {
            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {
                //Insert the character.
                insertCharacter(token);
                return;
            }

            //A comment token
            else if (token.type == enumHtmlTokenType.Comment)
            {
                //Insert a comment.
                insertComment(token);
                return;
            }

            //A DOCTYPE token
            else if (token.type == enumHtmlTokenType.DocType)
            {
                //Parse error. Ignore the token.
                onError("unexpected doctype");
                return;
            }
            //A start tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }

            //A start tag whose tag name is "col"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "col")
            {
                //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
                Element element = insertElement(token);
                openElements.popOffLast(element);

                //Acknowledge the token's self-closing flag, if it is set.
                acknowledgedSelfClosing(token);
                return;
            }

            //An end tag whose tag name is "colgroup"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "colgroup")
            {
                //If the current node is not a colgroup element, then this is a parse error; ignore the token.
                Element element = openElements.currentNode();
                if (element.tagName != "colgroup")
                {
                    onError("colgroup tag expected");
                    return;
                }
                else
                {
                    //Otherwise, pop the current node from the stack of open elements. Switch the insertion mode to "in table".
                    openElements.popOffLast(element);
                    insertionMode = enumInsertionMode.inTable;
                    return;
                }
            }

            //An end tag whose tag name is "col"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "col")
            {
                //Parse error. Ignore the token.
                onError("unexpected end tag col");
                return;
            }

            //A start tag whose tag name is "template"
            //An end tag whose tag name is "template"
            else if ((token.type == enumHtmlTokenType.StartTag && token.tagName == "template") || (token.type == enumHtmlTokenType.EndTag && token.tagName == "template"))
            {
                //Process the token using the rules for the "in head" insertion mode.
                inHead(token);
                return;
            }

            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }
            else
            {
                //Anything else
                //If the current node is not a colgroup element, then this is a parse error; ignore the token.
                Element currentnode = openElements.currentNode();
                if (currentnode.tagName != "colgroup")
                {
                    onError("expect colgroup in current open tag.");
                    return;
                }
                else
                {

                    //Otherwise, pop the current node from the stack of open elements.
                    openElements.popOffLast(currentnode);

                    //Switch the insertion mode to "in table".
                    insertionMode = enumInsertionMode.inTable;

                    //Reprocess the token.
                    inTable(token);
                    return;
                }
            }

        }

        /// <summary>
        /// 8.2.5.4.13 The "in table body" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inTableBody(HtmlToken token)
        {
            //When the user agent is to apply the rules for the "in table body" insertion mode, the user agent must handle the token as follows:

            //A start tag whose tag name is "tr"
            if (token.type == enumHtmlTokenType.StartTag && token.tagName == "tr")
            {
                //Clear the stack back to a table body context. (See below.)
                openElements.ClearStackBackToTableBodyContext();
                //Insert an HTML element for the token, then switch the insertion mode to "in row".
                insertElement(token);
                insertionMode = enumInsertionMode.inRow;
                return;
            }
            //A start tag whose tag name is one of: "th", "td"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("th", "td"))
            {
                //Parse error.
                onError(token.tagName + " in table body unexpected");

                //Clear the stack back to a table body context. (See below.)
                openElements.ClearStackBackToTableBodyContext();

                //Insert an HTML element for a "tr" start tag token with no attributes, then switch the insertion mode to "in row".
                HtmlToken faketoken = new HtmlToken(enumHtmlTokenType.StartTag);
                faketoken.startIndex = token.startIndex;
                faketoken.endIndex = -1;
                faketoken.tagName = "tr";

                insertElement(faketoken);
                insertionMode = enumInsertionMode.inRow;
                //Reprocess the current token.
                inRow(token);
                return;

            }
            //An end tag whose tag name is one of: "tbody", "tfoot", "thead"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("tbody", "tfoot", "thead"))
            {
                //If the stack of open elements does not have an element in table scope that is an HTML element and with the same tag name as the token, this is a parse error; ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inTableScope))
                {
                    onError("expect " + token.tagName + "in table scope");
                    return;

                }
                else
                {
                    //Otherwise:

                    //Clear the stack back to a table body context. (See below.)
                    openElements.ClearStackBackToTableBodyContext();

                    //Pop the current node from the stack of open elements. Switch the insertion mode to "in table".
                    openElements.popOffLast(openElements.currentNode());
                    insertionMode = enumInsertionMode.inTable;
                    return;
                }
            }
            //A start tag whose tag name is one of: "caption", "col", "colgroup", "tbody", "tfoot", "thead"
            //An end tag whose tag name is "table"
            if ((token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("caption", "col", "colgroup", "tbody", "tfoot", "thead")) || (token.type == enumHtmlTokenType.EndTag && token.tagName == "table"))
            {

                //If the stack of open elements does not have a tbody, thead, or tfoot element in table scope, this is a parse error; ignore the token.
                if (!openElements.hasOneOfElementsInScope(ScopeType.inTableScope, "tbody", "thead", "tfoot"))
                {
                    onError("expect tbody, thead or tfoot in table scope");
                    return;

                }
                else
                {
                    //Otherwise:

                    //Clear the stack back to a table body context. (See below.)
                    openElements.ClearStackBackToTableBodyContext();

                    //Pop the current node from the stack of open elements. Switch the insertion mode to "in table".
                    openElements.popOffLast(openElements.currentNode());

                    insertionMode = enumInsertionMode.inTable;
                    //Reprocess the token.
                    inTable(token);
                    return;
                }
            }

            //An end tag whose tag name is one of: "body", "caption", "col", "colgroup", "html", "td", "th", "tr"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("body", "caption", "col", "colgroup", "html", "td", "th", "tr"))
            {
                //Parse error. Ignore the token.
                onError(token.tagName + " end tag not expected");
                return;
            }
            else
            {
                //Anything else
                //Process the token using the rules for the "in table" insertion mode.

                inTable(token);
                return;

            }

        }

        /// <summary>
        /// 8.2.5.4.14 The "in row" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inRow(HtmlToken token)
        {
            //When the user agent is to apply the rules for the "in row" insertion mode, the user agent must handle the token as follows:

            //A start tag whose tag name is one of: "th", "td"
            if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("th", "td"))
            {
                //Clear the stack back to a table row context. (See below.)
                openElements.ClearStackBackToTableRowContext();

                //Insert an HTML element for the token, then switch the insertion mode to "in cell".
                insertElement(token);

                insertionMode = enumInsertionMode.inCell;

                //Insert a marker at the end of the list of active formatting elements.
                activeFormatingElements.insertMarker();
                return;
            }
            //An end tag whose tag name is "tr"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "tr")
            {
                //If the stack of open elements does not have a tr element in table scope, this is a parse error; ignore the token.
                if (!openElements.hasElementInScope("tr", ScopeType.inTableScope))
                {
                    onError("expect an tr tag in table scope");

                }
                else
                {

                    //Otherwise:

                    //Clear the stack back to a table row context. (See below.)
                    openElements.ClearStackBackToTableRowContext();

                    //Pop the current node (which will be a tr element) from the stack of open elements. Switch the insertion mode to "in table body".
                    openElements.popOffLast(openElements.currentNode());
                    insertionMode = enumInsertionMode.inTableBody;

                }
                return;
            }
            //A start tag whose tag name is one of: "caption", "col", "colgroup", "tbody", "tfoot", "thead", "tr"
            //An end tag whose tag name is "table"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("caption", "col", "colgroup", "tbody", "tfoot", "thead", "tr"))
            {
                //If the stack of open elements does not have a tr element in table scope, this is a parse error; ignore the token.
                if (!openElements.hasElementInScope("tr", ScopeType.inTableScope))
                {
                    onError("expect a tr in table scope");
                }

                else
                {

                    //Otherwise:

                    //Clear the stack back to a table row context. (See below.)
                    openElements.ClearStackBackToTableRowContext();

                    //Pop the current node (which will be a tr element) from the stack of open elements. Switch the insertion mode to "in table body".
                    openElements.popOffLast(openElements.currentNode());
                    insertionMode = enumInsertionMode.inTableBody;
                    //Reprocess the token.
                    ProcessToken(token);

                }
                return;
            }
            //An end tag whose tag name is one of: "tbody", "tfoot", "thead"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("tbody", "tfoot", "thead"))
            {

                //If the stack of open elements does not have an element in table scope that is an HTML element and with the same tag name as the token, this is a parse error; ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inTableScope))
                {
                    onError(token.tagName + " expected in table scope");
                    return;
                }

                //If the stack of open elements does not have a tr element in table scope, ignore the token.
                if (!openElements.hasElementInScope("tr", ScopeType.inTableScope))
                {

                    return;
                }
                else
                {
                    //Otherwise:

                    //Clear the stack back to a table row context. (See below.)
                    openElements.ClearStackBackToTableRowContext();

                    //Pop the current node (which will be a tr element) from the stack of open elements. Switch the insertion mode to "in table body".
                    openElements.popOffLast(openElements.currentNode());
                    insertionMode = enumInsertionMode.inTableBody;

                    //Reprocess the token.
                    ProcessToken(token);

                }
                return;
            }
            //An end tag whose tag name is one of: "body", "caption", "col", "colgroup", "html", "td", "th"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("body", "caption", "col", "colgroup", "html", "td", "th"))
            {

                //Parse error. Ignore the token.
                onError("end tag " + token.tagName + " not expected");
                return;
            }
            else
            {
                //Anything else
                //Process the token using the rules for the "in table" insertion mode.
                inTable(token);

                return;

            }
        }

        /// <summary>
        /// 8.2.5.4.15 The "in cell" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inCell(HtmlToken token)
        {
            //When the user agent is to apply the rules for the "in cell" insertion mode, the user agent must handle the token as follows:

            //An end tag whose tag name is one of: "td", "th"

            if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("td", "th"))
            {

                //If the stack of open elements does not have an element in table scope that is an HTML element and with the same tag name as that of the token, then this is a parse error; ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inTableScope))
                {
                    onError(token.tagName + " expected in table scope");
                    return;

                }
                //Otherwise:
                else
                {

                    //Generate implied end tags.
                    generateImpliedEndTags();

                    //Now, if the current node is not an HTML element with the same tag name as the token, then this is a parse error.
                    if (openElements.currentNode().tagName != token.tagName)
                    {
                        onError(token.tagName + " expected in current node");
                    }

                    //Pop elements from the stack of open elements stack until an HTML element with the same tag name as the token has been popped from the stack.
                    openElements.popOffTill(token.tagName, true);

                    //Clear the list of active formatting elements up to the last marker.
                    activeFormatingElements.clearUpToLastMarker();

                    //Switch the insertion mode to "in row".
                    insertionMode = enumInsertionMode.inRow;
                }
                return;
            }

            //A start tag whose tag name is one of: "caption", "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("caption", "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr"))
            {

                //If the stack of open elements does not have a td or th element in table scope, then this is a parse error; ignore the token. (fragment case)
                if (!openElements.hasOneOfElementsInScope(ScopeType.inTableScope, "td", "th"))
                {
                    onError("expect td or th in table scope");
                    return;
                }
                else
                {
                    //Otherwise, close the cell (see below) and reprocess the token.
                    closeCell();
                    ProcessToken(token);
                }
                return;
            }


            //An end tag whose tag name is one of: "body", "caption", "col", "colgroup", "html"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("body", "caption", "col", "colgroup", "html"))
            {
                //Parse error. Ignore the token.
                onError("unexpected end tag " + token.tagName);
                return;
            }

            //An end tag whose tag name is one of: "table", "tbody", "tfoot", "thead", "tr"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("table", "tbody", "tfoot", "thead", "tr"))
            {
                //If the stack of open elements does not have an element in table scope that is an HTML element and with the same tag name as that of the token, then this is a parse error; ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inTableScope))
                {
                    onError(token.tagName + " expected in table scope");
                    return;
                }
                else
                {
                    //Otherwise, close the cell (see below) and reprocess the token.
                    closeCell();
                    ProcessToken(token);
                }
                return;
            }
            else
            {
                //Anything else
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }

        }

        /// <summary>
        /// 8.2.5.4.16 The "in select" insertion mode
        /// </summary>
        private void inSelect(HtmlToken token)
        {

            //A character token that is U+0000 NULL
            if (token.type == enumHtmlTokenType.Character)
            {
                if (string.IsNullOrEmpty(token.data) || token.data[0] == '\u0000')
                {
                    //Parse error. Ignore the token.
                    onError("null character");
                    return;

                }
                else
                {

                    //Any other character token
                    //Insert the token's character.
                    insertCharacter(token);

                }
                return;

            }
            //A comment token
            //Insert a comment.
            else if (token.type == enumHtmlTokenType.Comment)
            {
                insertComment(token);
                return;
            }

            //A DOCTYPE token
            else if (token.type == enumHtmlTokenType.DocType)
            {
                //Parse error. Ignore the token.
                onError("unexpected doctype");
                return;

            }
            //A start tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }
            //A start tag whose tag name is "option"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "option")
            {
                //If the current node is an option element, pop that node from the stack of open elements.

                Element currentNode = openElements.currentNode();
                if (currentNode.tagName == "option")
                {
                    openElements.popOffLast(currentNode);
                }

                //Insert an HTML element for the token.
                insertElement(token);
                return;
            }
            //A start tag whose tag name is "optgroup"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "optgroup")
            {
                //If the current node is an option element, pop that node from the stack of open elements.
                Element currentnode = openElements.currentNode();
                if (currentnode.tagName == "option")
                {
                    openElements.popOffLast(currentnode);
                }

                //If the current node is an optgroup element, pop that node from the stack of open elements.
                currentnode = openElements.currentNode();
                if (currentnode.tagName == "optgroup")
                {
                    openElements.popOffLast(currentnode);
                }
                //Insert an HTML element for the token.
                insertElement(token);
                return;

            }

            //An end tag whose tag name is "optgroup"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "optgroup")
            {

                //First, if the current node is an option element, and the node immediately before it in the stack of open elements is an optgroup element, then pop the current node from the stack of open elements.

                int index = openElements.length - 1;
                if (index > 1)
                {
                    Element currentnode = openElements.item[index];
                    Element thenodeBefore = openElements.item[index - 1];

                    if (currentnode.tagName == "option" && thenodeBefore.tagName == "optgroup")
                    {
                        openElements.popOffLast(currentnode);
                    }

                }


                //If the current node is an optgroup element, then pop that node from the stack of open elements. Otherwise, this is a parse error; ignore the token.
                if (openElements.currentNode().tagName == "optgroup")
                {
                    openElements.popOffLast(openElements.currentNode());

                }
                else
                {
                    onError("expect optgroup in the open element");
                }
                return;
            }

            //An end tag whose tag name is "option"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "option")
            {

                //If the current node is an option element, then pop that node from the stack of open elements. Otherwise, this is a parse error; ignore the token.
                if (openElements.currentNode().tagName == "option")
                {
                    openElements.popOffLast(openElements.currentNode());
                }
                else
                {
                    onError("start tag option not found");

                }
                return;
            }

            //An end tag whose tag name is "select"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "select")
            {
                //If the stack of open elements does not have a select element in select scope, this is a parse error; ignore the token. (fragment case)

                if (!openElements.hasElementInScope("select", ScopeType.inSelectScope))
                {
                    onError("expect an select element in select scope");
                    return;
                }

                //Otherwise:
                else
                {
                    //Pop elements from the stack of open elements until a select element has been popped from the stack.
                    openElements.popOffTill("select", true);
                    //Reset the insertion mode appropriately.
                    resetInsertionMode();
                }

                return;
            }

            //A start tag whose tag name is "select"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "select")
            {

                //Parse error.
                onError("start tag select will be treated as end tag");

                //Pop elements from the stack of open elements until a select element has been popped from the stack.
                openElements.popOffTill("select", true);

                //Reset the insertion mode appropriately.
                resetInsertionMode();

                //It just gets treated like an end tag.
                return;
            }
            //A start tag whose tag name is one of: "input", "keygen", "textarea"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("input", "keygen", "textarea"))
            {
                //Parse error.
                onError(token.tagName + " unexpected");
                //If the stack of open elements does not have a select element in select scope, ignore the token. (fragment case)
                if (!openElements.hasElementInScope("select", ScopeType.inSelectScope))
                {
                    onError("expect an select element in select scope");
                    return;
                }

                //Pop elements from the stack of open elements until a select element has been popped from the stack.
                openElements.popOffTill("select", true);

                //Reset the insertion mode appropriately.
                resetInsertionMode();

                //Reprocess the token.
                ProcessToken(token);
                return;

            }
            //A start tag whose tag name is one of: "script", "template"
            //An end tag whose tag name is "template"
            else if ((token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("script", "template")) |
                (token.type == enumHtmlTokenType.EndTag && token.tagName == "template"))
            {

                //Process the token using the rules for the "in head" insertion mode.
                inHead(token);
                return;
            }

            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }
            else
            {
                //Anything else
                //Parse error. Ignore the token.
                onError("unexpected token");
            }
        }

        /// <summary>
        /// 8.2.5.4.17 The "in select in table" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inSelectInTable(HtmlToken token)
        {
            //When the user agent is to apply the rules for the "in select in table" insertion mode, the user agent must handle the token as follows:

            //A start tag whose tag name is one of: "caption", "table", "tbody", "tfoot", "thead", "tr", "td", "th"

            if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("caption", "table", "tbody", "tfoot", "thead", "tr", "td", "th"))
            {

                //Parse error.
                onError("unexpected tag");

                //Pop elements from the stack of open elements until a select element has been popped from the stack.
                openElements.popOffTill("select", true);

                //Reset the insertion mode appropriately.
                resetInsertionMode();

                //Reprocess the token.
                ProcessToken(token);
                return;
            }
            //An end tag whose tag name is one of: "caption", "table", "tbody", "tfoot", "thead", "tr", "td", "th"

            else if (token.type == enumHtmlTokenType.EndTag && token.tagName.isOneOf("caption", "table", "tbody", "tfoot", "thead", "tr", "td", "th"))
            {
                //Parse error.
                onError("unexpected tag");

                //If the stack of open elements does not have an element in table scope that is an HTML element and with the same tag name as that of the token, then ignore the token.
                if (!openElements.hasElementInScope(token.tagName, ScopeType.inTableScope))
                {

                    return;
                }
                else
                {
                    //Otherwise:

                    //Pop elements from the stack of open elements until a select element has been popped from the stack.
                    openElements.popOffTill("select", true);

                    //Reset the insertion mode appropriately.
                    resetInsertionMode();

                    //Reprocess the token.
                    ProcessToken(token);

                }
                return;
            }
            else
            {
                //Anything else
                //Process the token using the rules for the "in select" insertion mode.
                inSelect(token);
                return;
            }
        }


        /// <summary>
        /// 8.2.5.4.18 The "in template" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inTemplate(HtmlToken token)
        {

            //When the user agent is to apply the rules for the "in template" insertion mode, the user agent must handle the token as follows:

            //A character token
            //A comment token
            //A DOCTYPE token
            if (token.type == enumHtmlTokenType.Character || token.type == enumHtmlTokenType.Comment || token.type == enumHtmlTokenType.DocType)
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }
            //A start tag whose tag name is one of: "base", "basefont", "bgsound", "link", "meta", "noframes", "script", "style", "template", "title"
            //An end tag whose tag name is "template"
            else if ((token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("base", "basefont", "bgsound", "link", "meta", "noframes", "script", "style", "template", "title")) || (token.type == enumHtmlTokenType.EndTag && token.tagName == "template"))
            {
                //Process the token using the rules for the "in head" insertion mode.
                inHead(token);
                return;

            }

            //A start tag whose tag name is one of: "caption", "colgroup", "tbody", "tfoot", "thead"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("caption", "colgroup", "tbody", "tfoot", "thead"))
            {

                //Pop the current template insertion mode off the stack of template insertion modes.
                templateMode.popOffCurrent();

                //Push "in table" onto the stack of template insertion modes so that it is the new current template insertion mode.

                templateMode.push(enumInsertionMode.inTable);


                //Switch the insertion mode to "in table", and reprocess the token.
                insertionMode = enumInsertionMode.inTable;
                inTable(token);
                return;
            }

            //A start tag whose tag name is "col"

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "col")
            {
                //Pop the current template insertion mode off the stack of template insertion modes.
                templateMode.popOffCurrent();

                //Push "in column group" onto the stack of template insertion modes so that it is the new current template insertion mode.
                templateMode.push(enumInsertionMode.inColumnGroup);

                //Switch the insertion mode to "in column group", and reprocess the token.
                insertionMode = enumInsertionMode.inColumnGroup;
                ProcessToken(token);
                return;
            }
            //A start tag whose tag name is "tr"

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "tr")
            {

                //Pop the current template insertion mode off the stack of template insertion modes.
                templateMode.popOffCurrent();

                //Push "in table body" onto the stack of template insertion modes so that it is the new current template insertion mode.
                templateMode.push(enumInsertionMode.inTableBody);

                //Switch the insertion mode to "in table body", and reprocess the token.
                insertionMode = enumInsertionMode.inTableBody;
                ProcessToken(token);
                return;

            }
            //A start tag whose tag name is one of: "td", "th"

            else if (token.type == enumHtmlTokenType.StartTag && token.tagName.isOneOf("td", "th"))
            {
                //Pop the current template insertion mode off the stack of template insertion modes.
                templateMode.popOffCurrent();

                //Push "in row" onto the stack of template insertion modes so that it is the new current template insertion mode.
                templateMode.push(enumInsertionMode.inRow);

                //Switch the insertion mode to "in row", and reprocess the token.
                insertionMode = enumInsertionMode.inRow;
                ProcessToken(token);
                return;
            }
            else if (token.type == enumHtmlTokenType.StartTag)
            {
                //Any other start tag
                //Pop the current template insertion mode off the stack of template insertion modes.
                templateMode.popOffCurrent();

                //Push "in body" onto the stack of template insertion modes so that it is the new current template insertion mode.
                templateMode.push(enumInsertionMode.inBody);

                //Switch the insertion mode to "in body", and reprocess the token.
                insertionMode = enumInsertionMode.inBody;
                ProcessToken(token);
                return;
            }
            //Any other end tag
            else if (token.type == enumHtmlTokenType.EndTag)
            {
                //Parse error. Ignore the token.
                onError("unexpected end tag");
                return;

            }
            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //If there is no template element on the stack of open elements, then stop parsing. (fragment case)
                if (!openElements.hasTag("template"))
                {
                    stopParsing();
                }
                //Otherwise, this is a parse error.
                else
                {
                    //Pop elements from the stack of open elements until a template element has been popped from the stack.
                    openElements.popOffTill("template", true);

                    //Clear the list of active formatting elements up to the last marker.
                    activeFormatingElements.clearUpToLastMarker();

                    //Pop the current template insertion mode off the stack of template insertion modes.
                    templateMode.popOffCurrent();

                    //Reset the insertion mode appropriately.
                    resetInsertionMode();

                    //Reprocess the token.
                    ProcessToken(token);
                }
            }
        }


        /// <summary>
        /// 8.2.5.4.19 The "after body" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void afterBody(HtmlToken token)
        {

            //When the user agent is to apply the rules for the "after body" insertion mode, the user agent must handle the token as follows:

            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {

                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;

            }
            //A comment token
            else if (token.type == enumHtmlTokenType.Comment)
            {
                //Insert a comment as the last child of the first element in the stack of open elements (the html element).
                Comment commentelement = createComment(token);
                openElements.topElement().appendChild(commentelement);
                return;
            }
            //A DOCTYPE token
            else if (token.type == enumHtmlTokenType.DocType)
            {
                onError("unexpected doctype token");
                //Parse error. Ignore the token.
                return;
            }

            //A start tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }
            //An end tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "html")
            {
                //NON-W3C. 
                foreach (var item in openElements.item)
                {
                    if (item.tagName == "html")
                    {
                        item.location.endTokenStartIndex = token.startIndex;
                        item.location.endTokenEndIndex = token.endIndex;
                    }
                }


                //If the parser was originally created as part of the HTML fragment parsing algorithm, this is a parse error; ignore the token. (fragment case)
                if (fragmentParsing)
                {
                    onError("unexpected end html tag");
                    return;
                }
                else
                {
                    //Otherwise, switch the insertion mode to "after after body".
                    insertionMode = enumInsertionMode.afterAfterBody;

                }
                return;

            }
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //An end-of-file token
                //Stop parsing.
                stopParsing();
                return;
            }
            else
            {
                //Anything else
                //Parse error. Switch the insertion mode to "in body" and reprocess the token.
                onError("unexpected token");
                insertionMode = enumInsertionMode.inBody;
                ProcessToken(token);
                return;
            }
        }

        /// <summary>
        /// 8.2.5.4.20 The "in frameset" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void inFrameset(HtmlToken token)
        {
            //When the user agent is to apply the rules for the "in frameset" insertion mode, the user agent must handle the token as follows:

            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {
                //Insert the character.
                insertCharacter(token);
                return;
            }
            //A comment token

            else if (token.type == enumHtmlTokenType.Comment)
            {
                //Insert a comment.
                insertComment(token);
                return;
            }
            //A DOCTYPE token
            else if (token.type == enumHtmlTokenType.DocType)
            {
                //Parse error. Ignore the token.
                onError("unexpected doctype token");
                return;
            }
            //A start tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }
            //A start tag whose tag name is "frameset"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "frameset")
            {
                //Insert an HTML element for the token.
                insertElement(token);
                return;
            }
            //An end tag whose tag name is "frameset"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "frameset")
            {
                //If the current node is the root html element, then this is a parse error; ignore the token. (fragment case)
                if (openElements.currentNode().tagName == "html")
                {
                    onError("currentnode expect frameset");
                    return;

                }

                //Otherwise, pop the current node from the stack of open elements.
                else
                {
                    //If the parser was not originally created as part of the HTML fragment parsing algorithm (fragment case), and the current node is no longer a frameset element, then switch the insertion mode to "after frameset".
                    if (!fragmentParsing && openElements.currentNode().tagName != "frameset")
                    {
                        insertionMode = enumInsertionMode.afterFrameset;
                    }
                }

                return;
            }
            //A start tag whose tag name is "frame"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "frame")
            {
                //Insert an HTML element for the token. Immediately pop the current node off the stack of open elements.
                insertElement(token);

                //Acknowledge the token's self-closing flag, if it is set.
                acknowledgedSelfClosing(token);
                return;
            }
            //A start tag whose tag name is "noframes"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "noframes")
            {

                //Process the token using the rules for the "in head" insertion mode.
                inHead(token);
                return;
            }
            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //If the current node is not the root html element, then this is a parse error.
                if (openElements.currentNode().tagName != "html")
                {
                    onError("current node html expected");
                }
                //NOTE: The current node can only be the root html element in the fragment case.

                //Stop parsing.
                stopParsing();
                return;
            }
            else
            {
                //Anything else
                //Parse error. Ignore the token.
                onError("unexpected token");
                return;
            }
        }


        /// <summary>
        /// 8.2.5.4.21 The "after frameset" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void afterFrameset(HtmlToken token)
        {

            //When the user agent is to apply the rules for the "after frameset" insertion mode, the user agent must handle the token as follows:

            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            if (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020'))
            {
                //Insert the character.
                insertCharacter(token);
                return;
            }
            //A comment token
            else if (token.type == enumHtmlTokenType.Comment)
            {

                //Insert a comment.
                insertComment(token);
                return;
            }
            //A DOCTYPE token
            else if (token.type == enumHtmlTokenType.DocType)
            {

                //Parse error. Ignore the token.
                onError("unexpected doctype token");
                return;
            }
            //A start tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "html")
            {
                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }
            //An end tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.EndTag && token.tagName == "html")
            {
                //Switch the insertion mode to "after after frameset".
                insertionMode = enumInsertionMode.afterAfterFrameset;
                return;
            }
            //A start tag whose tag name is "noframes"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "noframes")
            {
                //Process the token using the rules for the "in head" insertion mode.
                inHead(token);
                return;
            }
            //An end-of-file token
            else if (token.type == enumHtmlTokenType.EOF)
            {
                //Stop parsing.
                stopParsing();
                return;
            }
            else
            {
                //Anything else
                //Parse error. Ignore the token.
                onError("unexpected token");
                return;
            }
        }


        /// <summary>
        /// 8.2.5.4.22 The "after after body" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void afterAfterBody(HtmlToken token)
        {

            //When the user agent is to apply the rules for the "after after body" insertion mode, the user agent must handle the token as follows:

            //A comment token
            if (token.type == enumHtmlTokenType.Comment)
            {
                //Insert a comment as the last child of the Document object.
                Comment comment = createComment(token);
                doc.appendChild(comment);
                return;
            }
            //A DOCTYPE token
            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //A start tag whose tag name is "html"
            else if (token.type == enumHtmlTokenType.DocType || (token.type == enumHtmlTokenType.StartTag && token.tagName == "html") || (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020')))
            {

                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }

            //An end-of-file token
            //Stop parsing.
            else if (token.type == enumHtmlTokenType.EOF)
            {
                stopParsing();
                return;
            }
            //Anything else
            else
            {
                //Parse error. Switch the insertion mode to "in body" and reprocess the token.
                insertionMode = enumInsertionMode.inBody;
                ProcessToken(token);
                return;
            }
        }


        /// <summary>
        /// 8.2.5.4.23 The "after after frameset" insertion mode
        /// </summary>
        /// <param name="token"></param>
        private void afterAfterFrameset(HtmlToken token)
        {

            //A comment token
            if (token.type == enumHtmlTokenType.Comment)
            {
                //Insert a comment as the last child of the Document object.
                Comment comment = createComment(token);
                doc.appendChild(comment);
                return;
            }


            //A DOCTYPE token
            //A character token that is one of U+0009 CHARACTER TABULATION, "LF" (U+000A), "FF" (U+000C), "CR" (U+000D), or U+0020 SPACE
            //A start tag whose tag name is "html"
            //Process the token using the rules for the "in body" insertion mode.
            else if (token.type == enumHtmlTokenType.DocType || (token.type == enumHtmlTokenType.StartTag && token.tagName == "html") || (token.type == enumHtmlTokenType.Character && token.data.isOneOf('\u0009', '\u000A', '\u000C', '\u000D', '\u0020')))
            {

                //Process the token using the rules for the "in body" insertion mode.
                inBody(token);
                return;
            }



            //An end-of-file token
            //Stop parsing.
            else if (token.type == enumHtmlTokenType.EOF)
            {
                stopParsing();
                return;
            }


            //A start tag whose tag name is "noframes"
            else if (token.type == enumHtmlTokenType.StartTag && token.tagName == "noframes")
            {
                //Process the token using the rules for the "in head" insertion mode.
                inHead(token);
                return;
            }
            else
            {
                //Anything else
                //Parse error. Ignore the token.
                onError("unexpected token");
                return;
            }

        }


        #region "Fix Error"

        private void FixWithoutTable(HtmlToken token)
        {
            if (token.tagName == "tr")
            {
                inTableBody(token);
            }
            else if (token.tagName == "td")
            {
                inRow(token);
            }
        }
        #endregion
    }
}