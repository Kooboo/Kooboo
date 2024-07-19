using System;
using System.Collections.Generic;

namespace Kooboo.Lib.Utilities
{
    public class JsStringScanner
    {

        public static List<string> ScanStringList(string jstext)
        {
            JsStringScanner scanner = new JsStringScanner(jstext);
            return scanner.GetStringList();
        }

        private char[] chars;
        private int _readIndex;
        private int _length;
        private bool isEOF;
        private char EOFChar = (Char)0x1a;

        public JsStringScanner(string JsText)
        {
            chars = JsText.ToCharArray();
            _length = chars.Length;

            isEOF = false;
            _readIndex = -1; // set it to -1, so that the consumenext() will starts from 0 index. 
        }
        public string ConsumeString()
        {
            while (true)
            {
                //Consume comments.
                ConsumeComment();
                //Consume the next input code point.
                char current = consumeNext();

                //U+0022 QUOTATION MARK (")
                //Consume a string token with the ending code point U+0022 QUOTATION MARK (") and return it.
                if (current == '\u0022')
                {
                    return consumeStringToken('\u0022');
                }

                //U+0027 APOSTROPHE (‘)
                else if (current == '\u0027')
                {
                    //Consume a string token with the ending code point U+0027 APOSTROPHE (’) and return it.
                    return consumeStringToken('\u0027');
                }
                else if (current == '`')
                {
                    //Consume a string token with the ending code point '`', new ES6 string..
                    return consumeStringToken('`');
                }

                else if (isEOF)
                {
                    return null;
                }
            }
        }

        public List<string> GetStringList()
        {
            List<string> result = new List<string>();

            var item = ConsumeString();
            while (item != null)
            {
                result.Add(item);
                item = ConsumeString();
            }
            return result;
        }

        private void moveNext()
        {
            _readIndex = _readIndex + 1;
            if (_readIndex >= _length)
            {
                isEOF = true;
            }
        }


        /// <summary>
        /// consume but do not return the current char, move the reading index one ahead.
        /// </summary>
        /// <param name="advanceCount"></param>
        private void consumeIt(int advanceCount)
        {
            for (int i = 0; i < advanceCount; i++)
            {
                moveNext();
            }
        }

        /// <summary>
        /// Lookup a char ahead, 
        /// </summary>
        /// <param name="aheadcount">0=current char.</param>
        /// <returns></returns>
        private char LookupChar(int aheadcount)
        {
            if ((_readIndex + aheadcount) >= _length)
            {
                return (Char)0x1a;   //EOF
            }
            else
            {
                return chars[_readIndex + aheadcount];
            }
        }

        private char getCurrentChar()
        {
            if (_readIndex >= _length)
            {
                return (Char)0x1a;   //EOF
            }
            else
            {
                return chars[_readIndex];
            }
        }

        /// <summary>
        /// Move read index advance one, and get current char/input code point
        /// </summary>
        /// <returns></returns>
        private char consumeNext()
        {
            moveNext();
            return getCurrentChar();
        }

        /// <summary>
        ///Consume an escaped code point.  It assumes that the U+005C REVERSE SOLIDUS (\) has already been consumed
        /// </summary>
        /// <returns></returns>
        private char ConsumeEscapedCodePoint()
        {
            //This section describes how to consume an escaped code point. It assumes that the U+005C REVERSE SOLIDUS (\) has already been consumed and that the next input code point has already been verified to not be a newline. It will return a code point.

            //Consume the next input code point.
            char next = consumeNext();

            //hex digit
            if (ishexdigit(next))
            {
                //Consume as many hex digits as possible, but no more than 5. Note that this means 1-6 hex digits have been consumed in total. If the next input code point is whitespace, consume it as well. Interpret the hex digits as a hexadecimal number. If this number is zero, or is for a surrogate code point, or is greater than the maximum allowed code point, return U+FFFD REPLACEMENT CHARACTER (�). Otherwise, return the code point with that value.

                string hex = next.ToString();
                int maxi = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (ishexdigit(LookupChar(i + 1)))
                    {
                        hex += LookupChar(i + 1).ToString();
                        maxi = i;
                    }
                    else
                    {
                        break;
                    }
                }
                consumeIt(maxi);

                if (WhiteSpace(LookupChar(1)))
                {
                    //If the next input code point is whitespace, consume it as well
                    consumeIt(1);
                }
                int charcode = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);

                /// If this number is zero, or is for a surrogate code point, or is greater than the maximum allowed code point, return U+FFFD REPLACEMENT CHARACTER (�). Otherwise, return the code point with that value.
                /// 

                if (charcode == 0)
                {
                    return '\uFFFD';
                }

                char hexchar = (char)charcode;

                if (char.IsSurrogate(hexchar) || hexchar > char.MaxValue)
                {
                    return '\uFFFD';
                }
                else
                {
                    return hexchar;
                }

            }
            //EOF code point
            else if (next == this.EOFChar)
            {
                //Return U+FFFD REPLACEMENT CHARACTER (�).
                return '\uFFFD';
            }
            else
            {
                //anything else
                //Return the current input code point.
                return next;
            }

        }

        private static bool ishexdigit(char input)
        {
            if (isDigit(input) || (input >= '\u0041' && input <= '\u0046') || (input >= '\u0061' && input <= '\u0066'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //digit
        private static bool isDigit(char input)
        {
            //A code point between U+0030 DIGIT ZERO (0) and U+0039 DIGIT NINE (9).
            if (input >= '\u0030' && input <= '\u0039')
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static bool WhiteSpace(char input)
        {
            if (input == '\u0009' || input == '\u0020')
            {
                return true;
            }
            else
            {
                return NewLine(input);
            }
        }


        /// Consume a string token. This section describes how to consume a string token from a stream of code points. It returns either a string-token or bad-string-token.
        ///  This algorithm must be called with an ending code point, which denotes the code point that ends the string.
        private string consumeStringToken(char endmark)
        {
            //Initially create a <string-token> with its value set to the empty string.
            string result = string.Empty;

            //Repeatedly consume the next input code point from the stream:
            char current;
            while (true)
            {
                current = consumeNext();
                //ending code point
                //EOF
                //Return the <string-token>.
                if (current == endmark || isEOF || current == this.EOFChar)
                {
                    return result;
                }

                //U+005C REVERSE SOLIDUS (\)
                else if (current == '\u005C')
                {
                    //If the next input code point is EOF, do nothing. 
                    char nextchar = LookupChar(1);

                    if (nextchar == this.EOFChar)
                    {
                        return result;
                    }
                    else if (NewLine(nextchar))
                    {
                        //Otherwise, if the next input code point is a newline,
                        // consume it.   //TODO: check the meaning of consume it (append the newline to string or not)
                        consumeIt(1);
                    }
                    else
                    {
                        //Otherwise, if the stream starts with a valid escape, consume an escaped code point and append the returned code point to the <string-token>’s value.
                        if (isValidEscape(current, nextchar))
                        {
                            result += ConsumeEscapedCodePoint();
                        }
                    }


                }

                //anything else
                //Append the current input code point to the <string-token>’s value.
                else
                {
                    result += current.ToString();
                }

            }


        }

        /// <summary>
        /// 4.3.8. Check if two code points are a valid escape. the two code points in question are the current input code point and the next input code point, in that order.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        private static bool isValidEscape(char one, char two)
        {
            //This section describes how to check if two code points are a valid escape. The algorithm described here can be called explicitly with two code points, or can be called with the input stream itself. In the latter case, the two code points in question are the current input code point and the next input code point, in that order.

            //Note: This algorithm will not consume any additional code point.

            //If the first code point is not U+005C REVERSE SOLIDUS (\), return false.

            if (one != '\u005C')
            {
                return false;
            }
            else if (NewLine(two))
            {
                //Otherwise, if the second code point is a newline, return false.
                return false;
            }
            else
            {
                //Otherwise, return true.
                return true;
            }
        }

        private static bool NewLine(char input)
        {
            if (input == '\u000A')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Consume comments
        /// </summary>
        private void ConsumeComment()
        {
            //This section describes how to consume comments from a stream of code points. It returns nothing.

            //If the next two input code point are U+002F SOLIDUS (/) followed by a U+002A ASTERISK (*), consume them and all following code points up to and including the first U+002A ASTERISK (*) followed by a U+002F SOLIDUS (/), or up to an EOF code point. Return to the start of this step.

            if (LookupChar(1) == '\u002F')
            {
                if (LookupChar(2) == '\u002A')
                {
                    while (true)
                    {
                        consumeIt(1);   // the first one is the * 
                        if (LookupChar(1) == '\u002A' && LookupChar(2) == '\u002F')
                        {
                            consumeIt(2);
                            break;
                        }
                        else if (LookupChar(1) == this.EOFChar)
                        {
                            break;
                        }
                    }
                }
                else if (LookupChar(2) == '\u002F')
                {
                    // consume till end of the lines. 
                    while (true)
                    {
                        var nextchar = consumeNext();
                        if (NewLine(nextchar) || nextchar == this.EOFChar)
                        {
                            break;
                        }
                    }
                }
            }

            //If the preceding paragraph ended by consuming an EOF code point, this is a parse error. 
            //Return nothing.  
        }
    }

}


