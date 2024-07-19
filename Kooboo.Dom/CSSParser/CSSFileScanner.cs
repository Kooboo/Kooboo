//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{
    /// <summary>
    /// Css file scanner to return the list of rules inside the documents. 
    /// This is a very expensive praser now, not fully operate according to syntax from W3C, without things like badstring, security exception. 
    /// Depreciated： use the tokenizer instead.
    /// </summary>
    public class CSSFileScanner
    {
        private string _cssText;
        private int _currentRead;
        private int _length;

        public CSSFileScanner(string cssText)
        {
            ///charset/encoding is not supported right now.
            _cssText = cssText;
            _length = _cssText.Length;
            _currentRead = 0;
        }

        /// <summary>
        /// read the next rule declaration
        /// </summary>
        /// <returns></returns>
        public CSSFileScannerResult ReadNext()
        {
            int lastread = _currentRead;

            for (int i = lastread; i < _length; i++)
            {
                // check comment, skip till the */
                if (_cssText[i] == 47 && _cssText[i + 1] == 42)
                {
                    int endComment = _cssText.IndexOf("*/", i);
                    i = endComment + 1;    // to be checked. 
                    continue;
                }
                else
                {
                    //if null, space or another char that should be ignored, ignore them
                    if (isEscapeChar(_cssText[i]))
                    {
                        continue;
                    }

                    if (isAtRule(_cssText[i]))
                    {

                        int endposition = 0;
                        string ruletext = getAtRuleText(i, ref endposition);


                        if (!string.IsNullOrEmpty(ruletext) && endposition > 0)
                        {
                            i = endposition;
                            _currentRead = endposition + 1;  // to be checked + 1 or not. 

                            enumCSSRuleType type = getAtRuleType(ruletext);

                            if (type == enumCSSRuleType.reserved)
                            {
                                continue;
                            }
                            else
                            {
                                CSSFileScannerResult result = new CSSFileScannerResult();
                                result.type = type;
                                result.cssText = ruletext;

                                return result;

                            }
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else
                    {
                        if (isValidIndent(_cssText[i]))
                        {
                            int endMark = 0;
                            string ruletext = getNextBlock(i, ref endMark);

                            if (!string.IsNullOrEmpty(ruletext) && endMark > 0)
                            {
                                CSSFileScannerResult result = new CSSFileScannerResult();
                                result.type = enumCSSRuleType.STYLE_RULE;
                                result.cssText = ruletext;

                                _currentRead = endMark + 1;
                                return result;
                            }


                        }
                        else
                        {
                            /// nothing match, can not escape, can not read. should throw exception

                            continue;

                        }
                    }


                }

            }

            return null;
        }

        /// <summary>
        /// get the whole block text that contains this rule.
        /// </summary>
        /// <param name="starti"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        private string getAtRuleText(int starti, ref int endPosition)
        {
            if (_cssText[starti + 1] == 73 || _cssText[starti + 1] == 105)
            {
                // 73 = i, 105=I. this is an import rule. find the next ";"
                int endMark = _cssText.IndexOf(";", starti);

                if (endMark > 0)
                {
                    endPosition = endMark;
                    return _cssText.Substring(starti, endMark - starti + 1);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                string nextblock = getNextBlock(starti, ref endPosition);
                return nextblock;
            }

        }

        private string getNextBlock(int startIndex, ref int endMark)
        {

            // now find out the close } position. 
            int leftBracketIndex = _cssText.IndexOf("{", startIndex);

            if (leftBracketIndex < 0)
            {
                return string.Empty;
            }

            int nextSearchPosition = leftBracketIndex + 1;

            int pairbalance = 1;   /// The target to make paribalance =0 

            int rightBracketIndex = 0;

            int maxi = 0;

            while (pairbalance > 0)
            {
                leftBracketIndex = _cssText.IndexOf("{", nextSearchPosition);

                rightBracketIndex = _cssText.IndexOf("}", nextSearchPosition);

                if (rightBracketIndex < 0 || pairbalance <= 0)
                {
                    break;
                }

                if (leftBracketIndex < 0 || rightBracketIndex < leftBracketIndex)
                {
                    pairbalance = pairbalance - 1;
                    nextSearchPosition = rightBracketIndex + 1;
                }
                else
                {
                    pairbalance = pairbalance + 1;
                    nextSearchPosition = leftBracketIndex + 1;
                }


                maxi++;  // only to prevent dead loop, should never happen.
                if (maxi > 1000)
                {
                    return string.Empty;
                }
            }

            nextSearchPosition = nextSearchPosition - 1; /// back to the position where it was found. 
            endMark = nextSearchPosition;

            /// till now we have the start and end position. 

            return _cssText.Substring(startIndex, endMark - startIndex + 1);
        }

        private bool isAtRule(int charcode)
        {
            /// 64 = @
            return (charcode == 64);
        }

        private bool isEscapeChar(int charcode)
        {
            // all chars before 33 (!) will be ignored. 
            return (charcode < 33);
        }

        private bool isValidIndent(int charcode)
        {
            /// 0-9 = ASCII 48-57  // not allowed as the start indentifier. 
            /// +, -, . :  43, 45, 46,
            /// _: 95
            /// #, *: 23, 42.
            /// a-z: 97-122
            /// @: 64  // Check by isAtRule.
            /// A-Z: 65 - 90

            // > 122, can  non-ascii chars, or 123-128.  { || } ~ DEL
            if (charcode > 122)
            {
                return false;
            }
            // a-z: 97-122
            if (charcode >= 97)
            {
                return true;
            }

            // A-Z: 65 - 90
            if (charcode >= 65 && charcode <= 90)
            {
                return true;
            }

            if (charcode == 46 || charcode == 23 || charcode == 43 || charcode == 42 || charcode == 45)
            {
                return true;
            }

            return false;

        }

        private enumCSSRuleType getAtRuleType(string cssText)
        {
            int firstspace = cssText.IndexOf(" ");

            if (firstspace < 0)
            {
                return enumCSSRuleType.reserved;
            }

            string type = cssText.Substring(0, firstspace);

            string typeLowerCase = type.ToLower();

            if (typeLowerCase.Contains("import"))
            {
                return enumCSSRuleType.IMPORT_RULE;
            }
            else
            {

                if (typeLowerCase.Contains("media"))
                {
                    return enumCSSRuleType.MEDIA_RULE;
                }
                else
                {

                    if (typeLowerCase.Contains("page"))
                    { return enumCSSRuleType.PAGE_RULE; }
                    else
                    {
                        if (typeLowerCase.Contains("font"))
                        {
                            return enumCSSRuleType.FONT_FACE_RULE;
                        }
                        else
                        {
                            return enumCSSRuleType.reserved;
                        }
                    }
                }
            }

        }

    }


    public class CSSFileScannerResult
    {
        public enumCSSRuleType type;
        public string cssText;

    }
}
