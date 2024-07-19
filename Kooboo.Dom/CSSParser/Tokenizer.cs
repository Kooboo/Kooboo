//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom.CSS.Tokens;

namespace Kooboo.Dom.CSS
{

    /// <summary>
    /// see: http://dev.w3.org/csswg/css-syntax/
    /// </summary>
    public class Tokenizer
    {

        private char[] chars;
        private int _readIndex;
        private int _length;
        private bool isEOF;

        private char EOFChar = (Char)0x1a;

        public Tokenizer(string cssText)
        {
            chars = codePointSubstitutions(cssText);
            _length = chars.Length;

            isEOF = false;
            _readIndex = -1; // set it to -1, so that the consumenext() will starts from 0 index. 
        }


        private cssToken tokenbuffer;
        private cssToken currenttoken;

        public cssToken ConsumeNextToken()
        {
            if (tokenbuffer != null)
            {
                currenttoken = tokenbuffer;
                tokenbuffer = null;
                return currenttoken;
            }
            else
            {
                currenttoken = ConsumeToken();
                return currenttoken;
            }

        }

        public void ReconsumeToken()
        {
            tokenbuffer = currenttoken;
        }


        /// <summary>
        /// move the reading index one position ahead.
        /// </summary>
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

        /// <summary>
        /// move the reading index location back one, in order for the next consumeNext to read current char. 
        /// </summary>
        private void reConsume()
        {
            _readIndex = _readIndex - 1;
            if (_readIndex < -1)
            {
                _readIndex = -1;
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
        public char consumeNext()
        {
            //TODO: handle  character inserted by JavaScript. 
            moveNext();
            return getCurrentChar();
        }

        /// <summary>
        /// 3.3. Preprocessing the input stream
        /// </summary>
        private char[] codePointSubstitutions(string input)
        {
            //The input stream consists of the code points pushed into it as the input byte stream is decoded.
            //Before sending the input stream to the tokenizer, implementations must make the following code point substitutions:
            //Replace any U+000D CARRIAGE RETURN (CR) code point, U+000C FORM FEED (FF) code point, or pairs of U+000D CARRIAGE RETURN (CR) followed by U+000A LINE FEED (LF) by a single U+000A LINE FEED (LF) code point.
            //Replace any U+0000 NULL code point with U+FFFD REPLACEMENT CHARACTER (�).

            // not sure of the .NET implement, but it is safer and should be faster to keep all the position that needs to be replaced and replace at the end. 

            char[] chararray = input.ToCharArray();

            int charlen = chararray.Length;

            List<int> removeList = new List<int>();   // the replacement of CR + LF pair will be done by removing the CR position. 
            bool lastwasCR = false;  // determine the last char was a CR, so that when next is LF, LF can be removed. 

            for (int i = 0; i < charlen; i++)
            {

                if (chararray[i] == '\u000D')
                {
                    //U+000D CARRIAGE RETURN (CR)
                    lastwasCR = true;
                    chararray[i] = '\u000A';
                }
                else if (chararray[i] == '\u000C')
                {
                    //U+000C FORM FEED (FF) code point,
                    chararray[i] = '\u000A';
                }
                else if (chararray[i] == '\u0000')
                {
                    // Replace any U+0000 NULL code point with U+FFFD REPLACEMENT CHARACTER (�).
                    chararray[i] = '\uFFFD';
                }


                if (lastwasCR)
                {
                    if (chararray[i] == '\u000A')
                    {
                        // mark this to be removed. 
                        removeList.Add(i);
                    }
                    lastwasCR = false;
                }


            }

            return chararray;

        }

        /// <summary>
        /// the reason to use this creatToken is insert other information like start index. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private cssToken createToken(enumTokenType type)
        {
            cssToken token = null;
            switch (type)
            {
                case enumTokenType.ident:
                    token = new ident_token();
                    break;
                case enumTokenType.function:
                    token = new function_token();
                    break;
                case enumTokenType.at_keyword:
                    token = new at_keyword_token();
                    break;
                case enumTokenType.hash:
                    token = new hash_token();
                    break;
                case enumTokenType.String:
                    token = new string_token();
                    break;
                case enumTokenType.bad_string:
                    token = new bad_string_token();
                    break;
                case enumTokenType.url:
                    token = new url_token();
                    break;
                case enumTokenType.bad_url:
                    token = new bad_url_token();
                    break;
                case enumTokenType.delim:
                    token = new delim_token();
                    break;
                case enumTokenType.number:
                    token = new number_token();
                    break;
                case enumTokenType.percentage:
                    token = new percentage_token();
                    break;
                case enumTokenType.dimension:
                    token = new dimension_token();
                    break;
                case enumTokenType.unicode_range:
                    token = new unicode_range_token();
                    break;
                case enumTokenType.include_match:
                    token = new include_match_token();
                    break;
                case enumTokenType.dash_match:
                    token = new dash_match_token();
                    break;
                case enumTokenType.prefix_match:
                    token = new prefix_match_token();
                    break;
                case enumTokenType.suffix_match:
                    token = new suffix_match_token();
                    break;
                case enumTokenType.substring_match:
                    token = new substring_match_token();
                    break;
                case enumTokenType.column:
                    token = new column_token();
                    break;
                case enumTokenType.whitespace:
                    token = new whitespace_token();
                    break;
                case enumTokenType.CDO:
                    token = new CDO_token();
                    break;
                case enumTokenType.CDC:
                    token = new CDC_token();
                    break;
                case enumTokenType.colon:
                    token = new colon_token();
                    break;
                case enumTokenType.semicolon:
                    token = new semicolon_token();
                    break;
                case enumTokenType.comma:
                    token = new comma_token();
                    break;
                case enumTokenType.square_bracket_left:
                    token = new square_bracket_left_token();
                    break;
                case enumTokenType.square_bracket_right:
                    token = new square_bracket_right_token();
                    break;
                case enumTokenType.round_bracket_left:
                    token = new round_bracket_left_token();
                    break;
                case enumTokenType.round_bracket_right:
                    token = new round_bracket_right_token();
                    break;
                case enumTokenType.curly_bracket_left:
                    token = new curly_bracket_left_token();
                    break;
                case enumTokenType.curly_bracket_right:
                    token = new curly_bracket_right_token();
                    break;
                case enumTokenType.EOF:
                    token = new EOF_token();
                    break;
                default:
                    break;
            }

            if (token != null)
            {
                token.startIndex = this._readIndex;
            }

            return token;
        }

        private cssToken emitToken(cssToken token)
        {
            token.endIndex = this._readIndex;
            return token;
        }

        /// <summary>
        /// 4.3.1. Consume a token. This section describes how to consume a token from a stream of code points. It will return a single token of any type
        /// </summary>
        /// <returns></returns>
        private cssToken ConsumeToken()
        {

            //Consume comments.
            ConsumeComment();

            //Consume the next input code point.
            char current = consumeNext();

            //whitespace
            //Consume as much whitespace as possible. Return a <whitespace-token>.
            if (Definition.isWhiteSpace(current))
            {
                cssToken token = createToken(enumTokenType.whitespace);

                while (LookupChar(1).isWhiteSpace())
                {
                    consumeIt(1);
                }

                return emitToken(token);
            }

            //U+0022 QUOTATION MARK (")
            //Consume a string token with the ending code point U+0022 QUOTATION MARK (") and return it.
            else if (current == '\u0022')
            {
                return consumeStringToken('\u0022');
            }

            //U+0023 NUMBER SIGN (#)
            else if (current == '\u0023')
            {
                //If the next input code point is a name code point or the next two input code points are a valid escape, then:

                if (LookupChar(1).isNameCodePoint() || TokenizerHelper.isValidEscape(LookupChar(1), LookupChar(2)))
                {
                    //Create a <hash-token>.
                    hash_token hashtoken = createToken(enumTokenType.hash) as hash_token;

                    //If the next 3 input code points would start an identifier, set the <hash-token>’s type flag to "id".
                    if (TokenizerHelper.isStartIdentifier(LookupChar(1), LookupChar(2), LookupChar(3)))
                    {
                        hashtoken.typeFlag = enumHashFlag.id;
                    }

                    //Consume a name, and set the <hash-token>’s value to the returned string.
                    hashtoken.value = ConsumeAName();
                    //Return the <hash-token>.
                    return emitToken(hashtoken);

                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token delimtoken = createToken(enumTokenType.delim) as delim_token;
                    delimtoken.value = current;
                    return emitToken(delimtoken);
                }
            }

            //U+0024 DOLLAR SIGN ($)
            else if (current == '\u0024')
            {
                //If the next input code point is U+003D EQUALS SIGN (=), consume it and return a <suffix-match-token>.
                if (LookupChar(1) == '\u003D')
                {
                    suffix_match_token suffixtoken = createToken(enumTokenType.suffix_match) as suffix_match_token;
                    consumeIt(1);
                    return emitToken(suffixtoken);
                }
                else
                {
                    //Otherwise, emit a <delim-token> with its value set to the current input code point.
                    delim_token delimtoken = createToken(enumTokenType.delim) as delim_token;
                    delimtoken.value = current;
                    return emitToken(delimtoken);
                }
            }


            //U+0027 APOSTROPHE (‘)
            else if (current == '\u0027')
            {
                //Consume a string token with the ending code point U+0027 APOSTROPHE (’) and return it.
                return consumeStringToken('\u0027');
            }

            //U+0028 LEFT PARENTHESIS (()
            else if (current == '\u0028')
            {
                //Return a <(-token>.
                round_bracket_left_token brackettoken = createToken(enumTokenType.round_bracket_left) as round_bracket_left_token;
                return emitToken(brackettoken);
            }

            //U+0029 RIGHT PARENTHESIS ())
            else if (current == '\u0029')
            {
                //Return a <)-token>.
                round_bracket_right_token brackettoken = createToken(enumTokenType.round_bracket_right) as round_bracket_right_token;
                return emitToken(brackettoken);

            }
            //U+002A ASTERISK (*)
            else if (current == '\u002A')
            {
                //If the next input code point is U+003D EQUALS SIGN (=), consume it and return a <substring-match-token>.
                if (LookupChar(1) == '\u003D')
                {
                    substring_match_token substringtoken = createToken(enumTokenType.substring_match) as substring_match_token;
                    consumeIt(1);
                    return emitToken(substringtoken);
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token delimtoken = createToken(enumTokenType.delim) as delim_token;
                    delimtoken.value = current;
                    return emitToken(delimtoken);

                }
            }
            //U+002B PLUS SIGN (+)
            else if (current == '\u002B')
            {
                //If the input stream starts with a number, reconsume the current input code point, consume a numeric token and return it.
                if (TokenizerHelper.isStartWithNumber(current, LookupChar(1), LookupChar(2)))
                {
                    reConsume();
                    return consumeANumericToken();
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    token.value = current;
                    return emitToken(token);
                }

            }
            //U+002C COMMA (,)
            else if (current == '\u002C')
            {
                //Return a <comma-token>.
                comma_token token = createToken(enumTokenType.comma) as comma_token;
                return emitToken(token);
            }

            //U+002D HYPHEN-MINUS (-)
            else if (current == '\u002D')
            {
                //If the input stream starts with a number, reconsume the current input code point, consume a numeric token, and return it.
                if (TokenizerHelper.isStartWithNumber(current, LookupChar(1), LookupChar(2)))
                {
                    reConsume();
                    return consumeANumericToken();
                }
                //Otherwise, if the input stream starts with an identifier, reconsume the current input code point, consume an ident-like token, and return it.
                else if (TokenizerHelper.isStartIdentifier(current, LookupChar(1), LookupChar(2)))
                {
                    reConsume();

                    return emitToken(ConsumeAnIdentLike());
                }
                //Otherwise, if the next 2 input code points are U+002D HYPHEN-MINUS U+003E GREATER-THAN SIGN (->), consume them and return a <CDC-token>.
                else if (LookupChar(1) == '\u002D' && LookupChar(2) == '\u003E')
                {

                    CDC_token cdctoken = createToken(enumTokenType.CDC) as CDC_token;
                    consumeIt(2);
                    return cdctoken;
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token delimtoken = createToken(enumTokenType.delim) as delim_token;
                    delimtoken.value = current;
                    return emitToken(delimtoken);
                }

            }

            //U+002E FULL STOP (.)
            else if (current == '\u002E')
            {
                //If the input stream starts with a number, reconsume the current input code point, consume a numeric token, and return it.
                if (TokenizerHelper.isStartWithNumber(current, LookupChar(1), LookupChar(2)))
                {
                    reConsume();
                    cssToken token = consumeANumericToken();

                    return emitToken(token);
                }
                else
                {

                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    token.value = current;
                    return emitToken(token);
                }
            }
            //U+002F SOLIDUS (/)
            else if (current == '\u002F')
            {
                //If the next input code point is U+002A ASTERISK (*), consume it and all following code points up to and including the first U+002A ASTERISK (*) followed by a U+002F SOLIDUS (/), or up to an EOF code point. Then consume a token and return it.
                if (LookupChar(1) == '\u002A')
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

                    return ConsumeToken();

                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token delimtoken = createToken(enumTokenType.delim) as delim_token;
                    delimtoken.value = current;
                    return emitToken(delimtoken);
                }
            }
            //U+003A COLON (:)
            else if (current == '\u003A')
            {
                //Return a <colon-token>.
                colon_token token = createToken(enumTokenType.colon) as colon_token;
                return emitToken(token);
            }
            //U+003B SEMICOLON (;)
            else if (current == '\u003B')
            {
                //Return a <semicolon-token>.
                semicolon_token token = createToken(enumTokenType.semicolon) as semicolon_token;
                return emitToken(token);
            }

            //U+003C LESS-THAN SIGN (<)
            else if (current == '\u003C')
            {
                //If the next 3 input code points are U+0021 EXCLAMATION MARK U+002D HYPHEN-MINUS U+002D HYPHEN-MINUS (!--), consume them and return a <CDO-token>.

                if (LookupChar(1) == '\u0021' && LookupChar(2) == '\u002D' && LookupChar(3) == '\u002D')
                {
                    CDO_token token = createToken(enumTokenType.CDO) as CDO_token;
                    consumeIt(3);

                    return emitToken(token);
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.

                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    token.value = current;
                    return emitToken(token);

                }
            }
            //U+0040 COMMERCIAL AT (@)
            else if (current == '\u0040')
            {
                //If the next 3 input code points would start an identifier, consume a name, create an <at-keyword-token> with its value set to the returned value, and return it.
                if (TokenizerHelper.isStartIdentifier(LookupChar(1), LookupChar(2), LookupChar(3)))
                {
                    at_keyword_token token = createToken(enumTokenType.at_keyword) as at_keyword_token;
                    string name = ConsumeAName();
                    token.value = name;
                    return emitToken(token);
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    token.value = current;
                    return emitToken(token);
                }
            }
            //U+005B LEFT SQUARE BRACKET ([)
            //Return a <[-token>.
            else if (current == '\u005B')
            {
                square_bracket_left_token token = createToken(enumTokenType.square_bracket_left) as square_bracket_left_token;

                return emitToken(token);

            }

            //U+005C REVERSE SOLIDUS (\)
            else if (current == '\u005C')
            {
                //If the input stream starts with a valid escape, reconsume the current input code point, consume an ident-like token, and return it.
                if (TokenizerHelper.isValidEscape(current, LookupChar(1)))
                {
                    reConsume();
                    cssToken token = ConsumeAnIdentLike();
                    return emitToken(token);
                }
                else
                {
                    //Otherwise, this is a parse error. Return a <delim-token> with its value set to the current input code point.
                    onError("invalid escape");
                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    token.value = current;
                    return emitToken(token);
                }
            }
            //U+005D RIGHT SQUARE BRACKET (])
            else if (current == '\u005D')
            {
                //Return a <]-token>.
                square_bracket_right_token token = createToken(enumTokenType.square_bracket_right) as square_bracket_right_token;

                return emitToken(token);

            }
            //U+005E CIRCUMFLEX ACCENT (^)
            else if (current == '\u005E')
            {
                //If the next input code point is U+003D EQUALS SIGN (=), consume it and return a <prefix-match-token>.
                if (LookupChar(1) == '\u003D')
                {
                    prefix_match_token token = createToken(enumTokenType.prefix_match) as prefix_match_token;
                    consumeIt(1);
                    return emitToken(token);
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    token.value = current;
                    return emitToken(token);
                }
            }
            //U+007B LEFT CURLY BRACKET ({)
            else if (current == '\u007B')
            {
                //Return a <{-token>.
                curly_bracket_left_token token = createToken(enumTokenType.curly_bracket_left) as curly_bracket_left_token;

                return emitToken(token);

            }
            //U+007D RIGHT CURLY BRACKET (})
            else if (current == '\u007D')
            {
                //Return a <}-token>.
                curly_bracket_right_token token = createToken(enumTokenType.curly_bracket_right) as curly_bracket_right_token;
                return emitToken(token);
            }
            //digit
            else if (current.isDigit())
            {
                //Reconsume the current input code point, consume a numeric token, and return it.
                reConsume();
                cssToken token = consumeANumericToken();
                return emitToken(token);
            }
            //U+0055 LATIN CAPITAL LETTER U (U)
            //U+0075 LATIN SMALL LETTER U (u)
            //NOTE: this part was removed from w3c on 2014-11-20
            //else if (current == '\u0055' || current == '\u0075')
            //{

            //    //If the next 2 input code points are U+002B PLUS SIGN (+) followed by a hex digit or U+003F QUESTION MARK (?), consume the next input code point. Note: don’t consume both of them. Consume a unicode-range token and return it.
            //    if (LookupChar(1) == '\u002B' && (LookupChar(2) == '\u003F' || LookupChar(2).isHexDigit()))
            //    {
            //        int startindex = this._readIndex;

            //      consumeIt(1);

            //      cssToken token   =  ConsumeAUnicodeRange();
            //      token.startIndex = startindex;
            //      return emitToken(token);

            //    }
            //    else
            //    {
            //        //Otherwise, reconsume the current input code point, consume an ident-like token, and return it.
            //        reConsume();
            //        cssToken token = ConsumeAnIdentLike();
            //        return emitToken(token);
            //    }

            //}

            //name-start code point
            else if (current.isNameStartCodePoint())
            {
                //Reconsume the current input code point, consume an ident-like token, and return it.
                reConsume();
                cssToken token = ConsumeAnIdentLike();
                return emitToken(token);

            }
            //U+007C VERTICAL LINE (|)
            else if (current == '\u007C')
            {
                //If the next input code point is U+003D EQUALS SIGN (=), consume it and return a <dash-match-token>.
                if (LookupChar(1) == '\u003D')
                {
                    dash_match_token token = createToken(enumTokenType.dash_match) as dash_match_token;
                    consumeIt(1);
                    return emitToken(token);

                }
                else if (LookupChar(1) == '\u0073')
                {
                    //Otherwise, if the next input code point is U+0073 VERTICAL LINE (|), consume it and return a <column-token>.
                    column_token token = createToken(enumTokenType.column) as column_token;
                    consumeIt(1);
                    return emitToken(token);
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    token.value = current;
                    return emitToken(token);
                }

            }
            //U+007E TILDE (~)
            else if (current == '\u007E')
            {
                //If the next input code point is U+003D EQUALS SIGN (=), consume it and return an <include-match-token>.
                if (LookupChar(1) == '\u003D')
                {
                    include_match_token token = createToken(enumTokenType.include_match) as include_match_token;
                    consumeIt(1);
                    return emitToken(token);
                }
                else
                {
                    //Otherwise, return a <delim-token> with its value set to the current input code point.
                    delim_token token = createToken(enumTokenType.delim) as delim_token;
                    return emitToken(token);
                }
            }
            //EOF
            else if (isEOF)
            {
                //Return an <EOF-token>.
                EOF_token token = createToken(enumTokenType.EOF) as EOF_token;
                return emitToken(token);
            }
            else
            {
                //anything else
                //Return a <delim-token> with its value set to the current input code point.
                delim_token token = createToken(enumTokenType.delim) as delim_token;
                token.value = current;

                return emitToken(token);

            }

        }


        /// <summary>
        /// 4.3.7. Consume an escaped code point.  It assumes that the U+005C REVERSE SOLIDUS (\) has already been consumed
        /// </summary>
        /// <returns></returns>
        private char ConsumeEscapedCodePoint()
        {
            //This section describes how to consume an escaped code point. It assumes that the U+005C REVERSE SOLIDUS (\) has already been consumed and that the next input code point has already been verified to not be a newline. It will return a code point.

            //Consume the next input code point.
            char next = consumeNext();

            //hex digit
            if (next.isHexDigit())
            {
                //Consume as many hex digits as possible, but no more than 5. Note that this means 1-6 hex digits have been consumed in total. If the next input code point is whitespace, consume it as well. Interpret the hex digits as a hexadecimal number. If this number is zero, or is for a surrogate code point, or is greater than the maximum allowed code point, return U+FFFD REPLACEMENT CHARACTER (�). Otherwise, return the code point with that value.

                string hex = next.ToString();
                int maxi = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (LookupChar(i + 1).isHexDigit())
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

                if (LookupChar(1).isWhiteSpace())
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
            else if (next.isEOF())
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


        private void onError(string errorMessage)
        {

        }


        /// <summary>
        /// 4.3.11. Consume a name. This section describes how to consume a name from a stream of code points. It returns a string containing the largest name that can be formed from adjacent code points in the stream, starting from the first.
        /// </summary>
        /// <returns></returns>
        private string ConsumeAName()
        {
            //Note: This algorithm does not do the verification of the first few code points that are necessary to ensure the returned code points would constitute an <ident-token>. If that is the intended use, ensure that the stream starts with an identifier before calling this algorithm.

            //Let result initially be an empty string.

            string result = string.Empty;

            char current;
            while (true)
            {
                //Repeatedly consume the next input code point from the stream:
                current = consumeNext();

                //name code point
                if (current.isNameCodePoint())
                {
                    //Append the code point to result.
                    result += current.ToString();
                }
                //the stream starts with a valid escape
                else if (TokenizerHelper.isValidEscape(current, LookupChar(1)))
                {
                    //Consume an escaped code point. Append the returned code point to result.
                    result += ConsumeEscapedCodePoint();
                }
                else
                {
                    //anything else

                    //Reconsume the current input code point. Return result.

                    this.reConsume();

                    return result;
                }
            }

        }

        /// <summary>
        ///  4.3.4. Consume a string token. This section describes how to consume a string token from a stream of code points. It returns either a string-token or bad-string-token.
        ///  This algorithm must be called with an ending code point, which denotes the code point that ends the string.
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        private cssToken consumeStringToken(char endmark)
        {
            string_token token = createToken(enumTokenType.String) as string_token;
            token.value = string.Empty;

            //Initially create a <string-token> with its value set to the empty string.

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
                    return emitToken(token);
                }

                //newline
                else if (current.isNewLine())
                {
                    //This is a parse error. Reconsume the current input code point, create a <bad-string-token>, and return it.
                    onError("unexpected new line");

                    reConsume();

                    bad_string_token badtoken = createToken(enumTokenType.bad_string) as bad_string_token;
                    return emitToken(badtoken);
                }


                //U+005C REVERSE SOLIDUS (\)
                else if (current == '\u005C')
                {
                    //If the next input code point is EOF, do nothing.

                    char nextchar = LookupChar(1);

                    if (nextchar.isEOF())
                    {


                    }
                    else if (nextchar.isNewLine())
                    {
                        //Otherwise, if the next input code point is a newline,
                        // consume it.   //TODO: check the meaning of consume it (append the newline to string or not)
                        consumeIt(1);

                    }
                    else
                    {
                        //Otherwise, if the stream starts with a valid escape, consume an escaped code point and append the returned code point to the <string-token>’s value.
                        if (TokenizerHelper.isValidEscape(current, nextchar))
                        {
                            token.value += ConsumeEscapedCodePoint();
                        }
                    }


                }

                //anything else
                //Append the current input code point to the <string-token>’s value.
                else
                {
                    token.value += current.ToString();
                }

            }


        }

        private cssToken consumeStringToken()
        {
            char currentchar = this.getCurrentChar();
            return consumeStringToken(currentchar);
        }

        /// <summary>
        /// Consume a numeric token
        /// </summary>
        /// <returns></returns>
        private cssToken consumeANumericToken()
        {
            //This section describes how to consume a numeric token from a stream of code points. It returns either a <number-token>, <percentage-token>, or <dimension-token>.

            ///WARNING, a  reconsume always occure before.
            int currentindex = this._readIndex + 1;

            //Consume a number.
            var number = ConsumeANumber();

            //If the next 3 input code points would start an identifier, then:
            if (TokenizerHelper.isStartIdentifier(LookupChar(1), LookupChar(2), LookupChar(3)))
            {
                //Create a <dimension-token> with the same representation, value, and type flag as the returned number, and a unit set initially to the empty string.
                dimension_token token = createToken(enumTokenType.dimension) as dimension_token;
                token.startIndex = currentindex;
                token.representation = number.Item1;
                token.TypeFlag = number.Item3;
                token.Number = number.Item2;

                //Consume a name. Set the <dimension-token>’s unit to the returned value.
                token.unit = ConsumeAName();
                //Return the <dimension-token>.
                return emitToken(token);
            }
            else if (LookupChar(1) == '\u0025')
            {
                //Otherwise, if the next input code point is U+0025 PERCENTAGE SIGN (%), consume it. Create a <percentage-token> with the same representation and value as the returned number, and return it.
                consumeIt(1);
                percentage_token token = createToken(enumTokenType.percentage) as percentage_token;
                token.startIndex = currentindex;
                token.representation = number.Item1;
                token.Number = number.Item2;

                return emitToken(token);
            }
            else
            {

                //Otherwise, create a <number-token> with the same representation, value, and type flag as the returned number, and return it.
                number_token token = createToken(enumTokenType.number) as number_token;
                token.startIndex = currentindex;
                token.representation = number.Item1;
                token.TypeFlag = number.Item3;
                token.Number = number.Item2;

                return emitToken(token);
            }
        }


        /// <summary>
        /// 4.3.12. Consume a number.   Ensure that the stream starts with a number before calling this algorithm. 
        /// </summary>
        /// <returns></returns>
        private Tuple<string, double, enumNumericType> ConsumeANumber()
        {
            //Note: This algorithm does not do the verification of the first few code points that are necessary to ensure a number can be obtained from the stream. Ensure that the stream starts with a number before calling this algorithm.
            //This section describes how to consume a number from a stream of code points. It returns a 3-tuple of a string representation, a numeric value, and a type flag which is either "integer" or "number".


            Tuple<string, double, enumNumericType> tuple;

            string repr = string.Empty;

            enumNumericType typeflag = enumNumericType.integer;


            //Execute the following steps in order:
            // Initially set repr to the empty string and type to "integer".
            //If the next input code point is U+002B PLUS SIGN (+) or U+002D HYPHEN-MINUS (-), consume it and append it to repr.
            if (LookupChar(1) == '\u002B' || LookupChar(1) == '\u002D')
            {
                char next = consumeNext();
                repr += next.ToString();
            }


            //While the next input code point is a digit, consume it and append it to repr.
            while (LookupChar(1).isDigit())
            {
                char next = consumeNext();
                repr += next.ToString();
            }

            //If the next 2 input code points are U+002E FULL STOP (.) followed by a digit, then:
            if (LookupChar(1) == '\u002E' && LookupChar(2).isDigit())
            {
                //Consume them.
                //Append them to repr.
                char nextone = consumeNext();
                char nexttwo = consumeNext();

                repr = repr + nextone.ToString() + nexttwo.ToString();

                //Set type to "number".
                typeflag = enumNumericType.number;

                //While the next input code point is a digit, consume it and append it to repr.
                while (LookupChar(1).isDigit())
                {
                    char next = consumeNext();
                    repr += next.ToString();
                }

            }
            //If the next 2 or 3 input code points are U+0045 LATIN CAPITAL LETTER E (E) or U+0065 LATIN SMALL LETTER E (e), optionally followed by U+002D HYPHEN-MINUS (-) or U+002B PLUS SIGN (+), followed by a digit, then:

            if (LookupChar(1) == '\u0045' || LookupChar(1) == '\u0065')
            {

                //Consume them.
                //Append them to repr.
                //Set type to "number".
                //While the next input code point is a digit, consume it and append it to repr.

                if ((LookupChar(2) == '\u002D' || LookupChar(2) == '\u002B') && LookupChar(3).isDigit())
                {
                    /// here 3 input.
                    char one = consumeNext();
                    char two = consumeNext();
                    char three = consumeNext();
                    repr = repr + one.ToString() + two.ToString() + three.ToString();

                    typeflag = enumNumericType.number;

                    while (LookupChar(1).isDigit())
                    {
                        char next = consumeNext();
                        repr += next.ToString();
                    }
                }
                else if (LookupChar(2).isDigit())
                {
                    /// here 2 input.
                    char one = consumeNext();
                    char two = consumeNext();
                    repr = repr + one.ToString() + two.ToString();

                    typeflag = enumNumericType.number;

                    while (LookupChar(1).isDigit())
                    {
                        char next = consumeNext();
                        repr += next.ToString();
                    }

                }
            }


            double value = TokenizerHelper.ConvertStringToNumber(repr);

            tuple = new Tuple<string, double, enumNumericType>(repr, value, typeflag);

            //Convert repr to a number, and set the value to the returned value.
            //Return a 3-tuple of repr, value, and type.
            return tuple;

        }


        /// <summary>
        /// 4.3.3. Consume an ident-like token
        /// </summary>
        /// <returns></returns>
        private cssToken ConsumeAnIdentLike()
        {

            //This section describes how to consume an ident-like token from a stream of code points. It returns an <ident-token>, <function-token>, <url-token>, or <bad-url-token>.

            int startindex = this._readIndex + 1;  // a reconsume always called before. 

            //Consume a name.
            string name = ConsumeAName();

            //If the returned string’s value is an ASCII case-insensitive match for "url", and the next input code point is U+0028 LEFT PARENTHESIS ((), consume it.            Consume a url token, and return it.

            // new as per 2014-11-20.             
            //If the returned string’s value is an ASCII case-insensitive match for "url", and the next input code point is U+0028 LEFT PARENTHESIS ((), consume it. While the next two input code points are whitespace, consume the next input code point. If the next one or two input code points are U+0022 QUOTATION MARK ("), U+0027 APOSTROPHE ('), or whitespace followed by U+0022 QUOTATION MARK (") orU+0027 APOSTROPHE ('), then create a <function-token> with its value set to the returned string and return it. Otherwise, consume a url token, and return it.

            if (name.ToLower() == "url" && LookupChar(1) == '\u0028')
            {
                consumeIt(1);

                while (LookupChar(1).isWhiteSpace() && LookupChar(2).isWhiteSpace())
                {
                    consumeIt(1);
                }

                if ((LookupChar(1) == '\u0022' || LookupChar(1) == '\u0027') || (LookupChar(1).isWhiteSpace() && (LookupChar(2) == '\u0022' || LookupChar(2) == '\u0027')))
                {
                    function_token functoken = createToken(enumTokenType.function) as function_token;
                    functoken.Value = name;
                    functoken.startIndex = startindex;
                    return functoken;
                }
                else
                {
                    cssToken token = ConsumeAUrl();
                    token.startIndex = startindex;
                    return token;
                }
            }

            //Otherwise, if the next input code point is U+0028 LEFT PARENTHESIS ((), consume it. Create a <function-token> with its value set to the returned string and return it.
            else if (LookupChar(1) == '\u0028')
            {
                consumeIt(1);
                function_token functoken = createToken(enumTokenType.function) as function_token;
                functoken.startIndex = startindex;
                functoken.Value = name;

                return functoken;
            }
            else
            {
                //Otherwise, create an <ident-token> with its value set to the returned string and return it.
                ident_token identtoken = createToken(enumTokenType.ident) as ident_token;
                identtoken.startIndex = startindex;
                identtoken.value = name;

                return identtoken;
            }
        }

        /// <summary>
        /// 4.3.5. Consume a url token
        /// </summary>
        /// <returns></returns>
        private cssToken ConsumeAUrl()
        {
            //This section describes how to consume a url token from a stream of code points. It returns either a <url-token> or a <bad-url-token>.

            //Note: This algorithm assumes that the initial "url(" has already been consumed.

            //Execute the following steps in order:

            //Initially create a <url-token> with its value set to the empty string.
            url_token token = createToken(enumTokenType.url) as url_token;

            //Consume as much whitespace as possible.
            while (LookupChar(1).isWhiteSpace())
            {
                consumeIt(1);
            }

            //If the next input code point is EOF, return the <url-token>.
            if (LookupChar(1) == this.EOFChar)
            {
                return token;
            }

            //If the next input code point is a U+0022 QUOTATION MARK (") or U+0027 APOSTROPHE (‘), then:
            //UPDATED: this has  been changed as per 2014-11-20, but keep it here should not harm.
            if (LookupChar(1) == '\u0022' || LookupChar(1) == '\u0027')
            {
                //Consume a string token with the current input code point as the ending code point.
                cssToken returntoken = consumeStringToken(this.getCurrentChar());

                //If a <bad-string-token> was returned, consume the remnants of a bad url, create a <bad-url-token>, and return it.
                if (returntoken.Type == enumTokenType.bad_string)
                {
                    ConsumeTheRemnantOfBadUrl();
                    bad_url_token urltoken = createToken(enumTokenType.bad_url) as bad_url_token;
                    return urltoken;
                }

                string_token stringToken = returntoken as string_token;

                //Set the <url-token>’s value to the returned <string-token>’s value.
                token.value = stringToken.value;


                //Consume as much whitespace as possible.
                while (LookupChar(1).isWhiteSpace())
                {
                    consumeIt(1);
                }



                //If the next input code point is U+0029 RIGHT PARENTHESIS ()) or EOF, consume it and return the <url-token>; otherwise, consume the remnants of a bad url, create a <bad-url-token>, and return it.
                if (LookupChar(1) == '\u0029' || LookupChar(1) == this.EOFChar)
                {
                    consumeIt(1);
                    return token;
                }
                else
                {
                    ConsumeTheRemnantOfBadUrl();

                    bad_url_token badtoken = createToken(enumTokenType.bad_url) as bad_url_token;

                    return badtoken;

                }

            }

            char current;
            while (true)
            {
                //Repeatedly consume the next input code point from the stream:

                current = consumeNext();

                //U+0029 RIGHT PARENTHESIS ())
                //EOF
                if (current == '\u0029' || current == this.EOFChar)
                {
                    //Return the <url-token>.
                    return token;
                }

                //whitespace
                else if (current.isWhiteSpace())
                {
                    //Consume as much whitespace as possible. If the next input code point is U+0029 RIGHT PARENTHESIS ()) or EOF, consume it and return the <url-token>; otherwise, consume the remnants of a bad url, create a <bad-url-token>, and return it.

                    while (LookupChar(1).isWhiteSpace())
                    {
                        consumeIt(1);
                    }

                    if (LookupChar(1) == '\u0029' || LookupChar(1) == this.EOFChar)
                    {
                        consumeIt(1);
                        return token;
                    }
                    else
                    {
                        ConsumeTheRemnantOfBadUrl();
                        bad_url_token urltoken = createToken(enumTokenType.bad_url) as bad_url_token;
                        return urltoken;
                    }


                }

                //U+0022 QUOTATION MARK (")
                //U+0027 APOSTROPHE (’)
                //U+0028 LEFT PARENTHESIS (()
                //non-printable code point
                else if (current == '\u0022' || current == '\u0027' || current == '\u0028' || current.isNonPrintableCodePoint())
                {
                    //This is a parse error. Consume the remnants of a bad url, create a <bad-url-token>, and return it.
                    onError("bad url");
                    ConsumeTheRemnantOfBadUrl();

                }

                //U+005C REVERSE SOLIDUS
                else if (current == '\u005C')
                {
                    //If the stream starts with a valid escape, consume an escaped code point and append the returned code point to the <url-token>’s value.
                    //Otherwise, this is a parse error. Consume the remnants of a bad url, create a <bad-url-token>, and return it.
                    if (TokenizerHelper.isValidEscape(current, LookupChar(1)))
                    {
                        token.value += ConsumeEscapedCodePoint().ToString();
                    }
                    else
                    {
                        onError("unexpected escape char");
                        ConsumeTheRemnantOfBadUrl();
                        bad_url_token badurltoken = createToken(enumTokenType.bad_url) as bad_url_token;
                        return badurltoken;
                    }

                }
                else
                {
                    //anything else
                    //Append the current input code point to the <url-token>’s value.
                    token.value += current.ToString();

                }
            }
        }


        /// <summary>
        /// 4.3.14. Consume the remnants of a bad url. its sole use is to consume enough of the input stream to reach a recovery point where normal tokenizing can resume.
        /// </summary>
        private void ConsumeTheRemnantOfBadUrl()
        {
            //This section describes how to consume the remnants of a bad url from a stream of code points, "cleaning up" after the tokenizer realizes that it’s in the middle of a <bad-url-token>rather than a <url-token>. It returns nothing; its sole use is to consume enough of the input stream to reach a recovery point where normal tokenizing can resume.


            //Repeatedly consume the next input code point from the stream:
            char current;
            while (true)
            {
                current = consumeNext();

                //U+0029 RIGHT PARENTHESIS ())
                //EOF
                if (current == '\u0029' || isEOF || current == this.EOFChar)
                {
                    return;
                }

                //the input stream starts with a valid escape
                if (TokenizerHelper.isValidEscape(current, LookupChar(1)))
                {
                    //Consume an escaped code point. This allows an escaped right parenthesis ("\)") to be encountered without ending the <bad-url-token>. This is otherwise identical to the "anything else" clause.
                    ConsumeEscapedCodePoint();
                }

                //anything else
                //Do nothing.

            }



        }

        /// <summary>
        /// 4.3.6. Consume a unicode-range token. This algorithm assumes that the initial "u+" has been consumed, and the next code point verified to be a hex digit or a "?".
        /// </summary>
        /// <returns></returns>
        private cssToken ConsumeAUnicodeRange()
        {
            //This section describes how to consume a unicode-range token. It returns a <unicode-range-token>.
            //Note: This algorithm assumes that the initial "u+" has been consumed, and the next code point verified to be a hex digit or a "?".
            //Execute the following steps in order:

            //Consume as many hex digits as possible, but no more than 6. If less than 6 hex digits were consumed, consume as many U+003F QUESTION MARK (?) code points as possible, but no more than enough to make the total of hex digits and U+003F QUESTION MARK (?) code points equal to 6.

            string strhex = string.Empty;
            int hexcount = 0;
            int rangestart = 0;
            int rangeend = 0;
            while (LookupChar(1).isHexDigit())
            {
                strhex += consumeNext().ToString();
                hexcount += 1;

                if (hexcount >= 6)
                {
                    break;
                }
            }

            int QmarkCount = 0;

            if (hexcount < 6)
            {
                while (LookupChar(1) == '\u003F')
                {
                    strhex += consumeNext().ToString();
                    QmarkCount += 1;
                    if ((QmarkCount + hexcount) >= 6)
                    {
                        break;
                    }
                }
            }

            //If any U+003F QUESTION MARK (?) code points were consumed, then:
            if (QmarkCount > 0)
            {

                //Interpret the consumed code points as a hexadecimal number, with the U+003F QUESTION MARK (?) code points replaced by U+0030 DIGIT ZERO (0) code points. This is the start of the range.

                string start = strhex.Replace('\u003F', '\u0030');

                //Interpret the consumed code points as a hexadecimal number again, with the U+003F QUESTION MARK (?) code point replaced by U+0046 LATIN CAPITAL LETTER F (F) code points. This is the end of the range.

                string end = strhex.Replace('\u003F', '\u0046');

                //Return a new <unicode-range-token> with the above start and end.
                unicode_range_token token = createToken(enumTokenType.unicode_range) as unicode_range_token;

                rangestart = Convert.ToInt32(start, 16);
                rangeend = Convert.ToInt32(end, 16);

                token.start = rangestart;
                token.end = rangeend;

                return token;

            }
            else
            {
                //Otherwise, interpret the digits as a hexadecimal number. This is the start of the range.
                rangestart = Convert.ToInt32(strhex, 16);
            }

            //If the next 2 input code point are U+002D HYPHEN-MINUS (-) followed by a hex digit, then:

            if (LookupChar(1) == '\u002D' && LookupChar(2).isHexDigit())
            {
                //Consume the next input code point.
                //Consume as many hex digits as possible, but no more than 6. Interpret the digits as a hexadecimal number. This is the end of the range.

                consumeNext();
                string endhex = string.Empty;
                int endcount = 0;
                while (LookupChar(1).isHexDigit())
                {
                    strhex += consumeNext().ToString();
                    endcount += 1;

                    if (endcount >= 6)
                    {
                        break;
                    }
                }

                rangeend = Convert.ToInt32(endhex, 16);

            }
            else
            {
                //Otherwise, the end of the range is the start of the range.
                rangeend = rangestart;
            }


            //Return the <unicode-range-token> with the above start and end.

            unicode_range_token unicodetoken = createToken(enumTokenType.unicode_range) as unicode_range_token;

            unicodetoken.start = rangestart;
            unicodetoken.end = rangeend;

            return unicodetoken;

        }


        /// <summary>
        /// 4.3.2. Consume comments
        /// </summary>
        private void ConsumeComment()
        {
            //This section describes how to consume comments from a stream of code points. It returns nothing.

            //If the next two input code point are U+002F SOLIDUS (/) followed by a U+002A ASTERISK (*), consume them and all following code points up to and including the first U+002A ASTERISK (*) followed by a U+002F SOLIDUS (/), or up to an EOF code point. Return to the start of this step.

            if (LookupChar(1) == '\u002F' && LookupChar(2) == '\u002A')
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

            //If the preceding paragraph ended by consuming an EOF code point, this is a parse error.

            //Return nothing.


        }
    }
}
