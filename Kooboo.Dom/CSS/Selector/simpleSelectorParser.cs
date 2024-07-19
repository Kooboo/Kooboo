//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Text;

namespace Kooboo.Dom.CSS
{
    public class simpleSelectorParser
    {
        private string _selectorText;
        private int length;
        private int readIndex = -1;

        private StringBuilder _buffer;
        private bool stop;

        private simpleSelector simple;

        public simpleSelectorParser(string selectorText)
        {
            _selectorText = selectorText;
            length = _selectorText.Length;
            stop = false;


            _buffer = new StringBuilder();

        }

        private char ReadNext()
        {
            readIndex += 1;

            if (readIndex >= length)
            {
                stop = true;
                return (Char)0x1a;   //EOF
            }
            else
            {
                return _selectorText[readIndex];
            }

        }

        private void reConsume()
        {
            readIndex = readIndex - 1;

        }

        private void Reset()
        {
            readIndex = -1;
        }

        private char LookupNext()
        {
            int nextindex = readIndex + 1;

            if (nextindex >= length)
            {
                return (Char)0x1a;   //EOF
            }
            else
            {
                return _selectorText[nextindex];
            }
        }

        internal bool IsCombinatorSelector(string selectorText)
        {
            bool escape = false;
            foreach (var item in _selectorText.ToCharArray())
            {
                if (item == '[' || item == '(')
                {
                    escape = true;
                }

                if (item == ']' || item == ')')
                { escape = false; }

                if (escape)
                {
                    continue;
                }

                if (item.isCSSCombinator())
                {
                    return true;
                }
            }
            return false;
        }

        public simpleSelector Parse()
        {
            // first check whether this is a combinartor or not. 
            if (IsCombinatorSelector(_selectorText))
            {
                parseCombinartor();
                return simple;
            }

            //TODO: to be checked. 
            if (_selectorText.Contains("::"))
            {
                parsePseudoElement();

                return simple;
            }
            if (_selectorText.Contains(":"))
            {
                parsePseudoClass();
                return simple;
            }

            while (true)
            {
                char currentChar = ReadNext();
                if (stop)
                {
                    break;
                }

                /// An ID selector contains a "number sign" (U+0023, #) immediately followed by the ID value
                if (currentChar == '\u0023')
                {
                    parseIdSelector();
                    return simple;

                }

                /// The universal selector, written as a CSS qualified name [CSS3NAMESPACE] with an asterisk (* U+002A) as the local name
                else if (currentChar == '\u002A')
                {
                    if (_selectorText.Trim() != '\u002A'.ToString())
                    {
                        _buffer.Append(currentChar);
                        continue;
                    }
                    else
                    {
                        return new universalSelector();
                    }
                }
                ///Working with HTML, authors may use the "period" notation (also known as "full stop", U+002E, .) as an alternative to the ~= notation when representing the class attribute.
                else if (currentChar == '\u002E')
                {
                    parseClassSelector();
                    return simple;

                }
                else if (currentChar == '[')
                {
                    parseAttributeSelecotr();
                    return simple;

                }
                else if (currentChar == ':')
                {
                    if (LookupNext() == ':')
                    {
                        parsePseudoElement();
                    }
                    else
                    {
                        parsePseudoClass();
                    }
                    return simple;
                }

                else
                {
                    _buffer.Append(currentChar);
                }

            }


            if (simple == null || simple.Type == enumSimpleSelectorType.unknown)
            {
                // nothing mathc, this is a type selector. 
                typeSelector typeselector = new typeSelector();
                typeselector.elementE = AppendCleanBuffer(_buffer);
                typeselector.wholeText = typeselector.elementE;

                simple = typeselector;
            }
            return simple;

        }

        private void parseAttributeSelecotr()
        {
            attributeSelector attributeselector = new attributeSelector();
            attributeselector.wholeText = _selectorText;

            attributeselector.matchType = enumAttributeType.defaultHas; // default is exactly equal without any *~|..


            if (_buffer.Length > 0)
            {
                attributeselector.elementE = AppendCleanBuffer(_buffer);
            }

            char current = ReadNext();

            while (!stop)
            {
                if (current == '=')
                {
                    if (_buffer.Length > 0)
                    {
                        attributeselector.attributeName = AppendCleanBuffer(_buffer);
                    }
                    if (attributeselector.matchType == enumAttributeType.defaultHas)
                    {
                        attributeselector.matchType = enumAttributeType.exactlyEqual;
                    }
                    current = ReadNext();
                }
                else if (current == '~')
                {
                    if (_buffer.Length > 0)
                    {
                        attributeselector.attributeName = AppendCleanBuffer(_buffer);
                    }

                    attributeselector.matchType = enumAttributeType.whitespaceSeperated;

                    current = ReadNext();
                }
                else if (current == '^')
                {
                    if (_buffer.Length > 0)
                    {
                        attributeselector.attributeName = AppendCleanBuffer(_buffer);
                    }
                    attributeselector.matchType = enumAttributeType.exactlyBegin;

                    current = ReadNext();

                }

                else if (current == '$')
                {
                    if (_buffer.Length > 0)
                    {
                        attributeselector.attributeName = AppendCleanBuffer(_buffer);
                    }

                    attributeselector.matchType = enumAttributeType.exactlyEnd;

                    current = ReadNext();
                }
                else if (current == '*')
                {
                    if (_buffer.Length > 0)
                    {
                        attributeselector.attributeName = AppendCleanBuffer(_buffer);
                    }

                    attributeselector.matchType = enumAttributeType.contains;

                    current = ReadNext();

                }
                else if (current == '|')
                {
                    if (_buffer.Length > 0)
                    {
                        attributeselector.attributeName = AppendCleanBuffer(_buffer);
                    }
                    attributeselector.matchType = enumAttributeType.hyphenSeperated;
                    current = ReadNext();
                }
                //"'" (U+0027)  //U+0022 QUOTATION MARK (")
                else if (current == '\u0027' || current == '\u0022')
                {
                    //ignore the ' " mark. 
                    current = ReadNext();
                }
                else if (current == ']')
                {
                    break;
                }
                else
                {
                    _buffer.Append(current);
                    current = ReadNext();
                }
            }


            if (_buffer.Length > 0)
            {
                string value = AppendCleanBuffer(_buffer);
                if (!string.IsNullOrEmpty(value))
                {

                    if (string.IsNullOrEmpty(attributeselector.attributeName))
                    {
                        attributeselector.attributeName = value;
                    }
                    else
                    {
                        attributeselector.attributeValue = value;
                    }
                }

            }

            simple = attributeselector;

        }

        private void parseClassSelector()
        {
            classSelector classselector = new classSelector();
            classselector.wholeText = _selectorText;


            if (_buffer.Length > 0)
            {
                classselector.elementE = AppendCleanBuffer(_buffer);
            }


            char current = ReadNext();

            while (!stop)
            {
                if (current == '.')
                {
                    classselector.classList.Add(AppendCleanBuffer(_buffer));
                    current = ReadNext();
                }
                else
                {
                    _buffer.Append(current);
                    current = ReadNext();
                }
            }

            if (_buffer.Length > 0)
            {
                classselector.classList.Add(AppendCleanBuffer(_buffer));

            }

            simple = classselector;

        }

        private void parseIdSelector()
        {

            idSelector idselector = new idSelector();
            idselector.wholeText = _selectorText;

            if (_buffer.Length > 0)
            {
                idselector.elementE = AppendCleanBuffer(_buffer);
            }

            char current = ReadNext();

            while (!stop)
            {
                _buffer.Append(current);
                current = ReadNext();
            }

            idselector.id = _buffer.ToString();

            simple = idselector;

        }

        private void parsePseudoElement()
        {
            pseudoElementSelector pseudoelement = new pseudoElementSelector();
            pseudoelement.wholeText = _selectorText;

            if (_buffer.Length > 0)
            {
                pseudoelement.elementE = AppendCleanBuffer(_buffer);
            }

            char current = ReadNext();

            while (!stop)
            {
                if (current == ':')
                {
                    if (_buffer.Length > 0)
                    {
                        pseudoelement.elementE += AppendCleanBuffer(_buffer);
                    }
                }
                else
                {
                    _buffer.Append(current);
                }
                current = ReadNext();
            }

            pseudoelement.matchElement = AppendCleanBuffer(_buffer);

            simple = pseudoelement;
        }

        private void parsePseudoClass()
        {
            pseudoClassSelector pseudoClass = new pseudoClassSelector();
            pseudoClass.wholeText = _selectorText;

            if (_buffer.Length > 0)
            {
                pseudoClass.elementE = AppendCleanBuffer(_buffer);
            }

            char current = ReadNext();

            bool InBracketState = false;

            while (!stop)
            {
                if (InBracketState)
                {
                    _buffer.Append(current);

                    if (current == ')')
                    {
                        InBracketState = false;
                        break;
                    }
                }
                else
                {
                    if (current == ':')
                    {
                        if (_buffer.Length > 0)
                        {
                            pseudoClass.elementE += AppendCleanBuffer(_buffer);
                        }
                    }

                    else
                    {
                        if (current == '(')
                        {
                            InBracketState = true;
                        }

                        _buffer.Append(current);
                    }
                }

                current = ReadNext();
            }

            pseudoClass.matchText = AppendCleanBuffer(_buffer);

            simple = pseudoClass;

        }

        private void parseCombinartor()
        {
            combinatorSelector combineSelector = new combinatorSelector();

            combinatorClause clause = new combinatorClause();

            clause.combineType = combinator.unknown;

            char current = ReadNext();

            while (!stop)
            {
                if (current == '>')
                {
                    if (_buffer.Length > 0)
                    {
                        clause.selectorText = AppendCleanBuffer(_buffer);

                        clause.selector = new simpleSelectorParser(clause.selectorText).Parse();
                        combineSelector.item.Add(clause);

                        clause = new combinatorClause();
                        clause.combineType = combinator.Child;

                        current = ReadNext();
                        continue;
                    }

                    if (clause.combineType == combinator.unknown || clause.combineType == combinator.Descendant)
                    {
                        clause.combineType = combinator.Child;

                    }

                    current = ReadNext();
                    continue;

                }
                else if (current == '+')
                {
                    if (_buffer.Length > 0)
                    {
                        clause.selectorText = AppendCleanBuffer(_buffer);

                        clause.selector = new simpleSelectorParser(clause.selectorText).Parse();
                        combineSelector.item.Add(clause);

                        clause = new combinatorClause();
                        clause.combineType = combinator.AdjacentSibling;

                        current = ReadNext();
                        continue;
                    }

                    if (clause.combineType == combinator.unknown || clause.combineType == combinator.Descendant)
                    {
                        clause.combineType = combinator.AdjacentSibling;

                    }
                    current = ReadNext();
                    continue;

                }
                else if (current == '~')
                {
                    if (_buffer.Length > 0)
                    {
                        clause.selectorText = AppendCleanBuffer(_buffer);

                        clause.selector = new simpleSelectorParser(clause.selectorText).Parse();
                        combineSelector.item.Add(clause);

                        clause = new combinatorClause();
                        clause.combineType = combinator.Sibling;
                        current = ReadNext();
                        continue;
                    }

                    if (clause.combineType == combinator.unknown || clause.combineType == combinator.Descendant)
                    {
                        clause.combineType = combinator.Sibling;

                    }
                    current = ReadNext();
                    continue;

                }
                else if (current.isCSSWhiteSpace())
                {
                    if (_buffer.Length > 0)
                    {
                        clause.selectorText = AppendCleanBuffer(_buffer);

                        clause.selector = new simpleSelectorParser(clause.selectorText).Parse();
                        combineSelector.item.Add(clause);

                        clause = new combinatorClause();
                        clause.combineType = combinator.Descendant;
                    }

                    if (clause.combineType == combinator.unknown)
                    {
                        clause.combineType = combinator.Descendant;
                    }

                    current = ReadNext();
                    continue;
                }
                else
                {
                    _buffer.Append(current);
                    current = ReadNext();
                }
            }

            if (_buffer.Length > 0)
            {
                clause.selectorText = AppendCleanBuffer(_buffer);

                clause.selector = new simpleSelectorParser(clause.selectorText).Parse();
                combineSelector.item.Add(clause);
            }

            simple = combineSelector;

        }

        private string AppendCleanBuffer(StringBuilder buffer)
        {
            string value = null;
            if (buffer.Length > 0)
            {
                value = buffer.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    value = value.Trim();
                }
                else
                {
                    value = null;
                }
            }
            buffer.Clear();
            return value;
        }
    }
}
