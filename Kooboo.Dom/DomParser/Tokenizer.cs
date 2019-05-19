//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Dom
{
    /// <summary>
    /// Exactly follows the instruction at: http://www.w3.org/TR/html5/syntax.html
    /// a copy of current version as of 2014. 07 has been attached with this source project. 
    /// </summary>
    public class Tokenizer
    {
        private string _htmlText;
        internal int _readIndex;
        private int _maxColLastLine;
        private int _length;
          
        private TreeConstruction _treeConstruction;

        private StringBuilder _buffer;

        // to hold the character inserted by parser, for example, javascript document.write. 
        public StringBuilder parserInsertBuffer;

        private bool isEOF;

        public enumParseState ParseState;
        public enumScriptParseState ScriptState;     // Own element, due to change the way of EmitToken to provide ReadNextToken like method for outside. 

        public int oldInsertionPoint;
        public int insertionPoint;

        public Tokenizer(string htmlText, TreeConstruction treeConstruction)
        {
            ParseState = enumParseState.DATA;
            ScriptState = enumScriptParseState.initial;
            _buffer = new StringBuilder();

            parserInsertBuffer = new StringBuilder();

            _treeConstruction = treeConstruction;

            _htmlText = htmlText;
            if (_htmlText != null)
            {
                _length = _htmlText.Length;
            }

            _maxColLastLine = 0;
            _readIndex = -1;   //make it -1, so that advanced one to read =0

            isEOF = false;
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

        private void moveAhead(int advanceCount)
        {
            for (int i = 0; i < advanceCount; i++)
            {
                moveNext();
            }
        }

        /// <summary>
        /// move the reading index location back one, in order for the next consumeNext to read current char. 
        /// </summary>
        private void reConsume()
        {
            _readIndex = _readIndex - 1;
            if (_readIndex < 0)
            {
                _readIndex = 0;
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
                return _htmlText[_readIndex];
            }
        }

        /// <summary>
        /// Move read index advance one, and get current char. 
        /// </summary>
        /// <returns></returns>
        private char consumeNext()
        {
            moveNext();
            return getCurrentChar();
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
                return _htmlText[_readIndex + aheadcount];
            }
        }

        private HtmlToken createToken(enumHtmlTokenType type)
        {
            HtmlToken token = new HtmlToken(type);
            // if (type == enumHtmlTokenType.StartTag || type == enumHtmlTokenType.EndTag)
            // {
            token.startIndex = _readIndex;

            // }

            return token;
        }

        private HtmlToken emitToken(HtmlToken token)
        {
            token.endIndex = _readIndex;
            if (token.type == enumHtmlTokenType.StartTag || token.type == enumHtmlTokenType.EndTag)
            {
                token.startNewAttribute();
            }
            return token;
        }

        /// <summary>
        /// emit a new character token. 
        /// </summary>
        /// <param name="chr"></param>
        /// <returns></returns>
        private HtmlToken emitTokenChar(char chr)
        {
            HtmlToken token = createToken(enumHtmlTokenType.Character);
            token.data = chr.ToString();
            token.startIndex = this._readIndex;
            token.endIndex = this._readIndex;
            return token;
        }

        private HtmlToken emitTokenString(string tokenString)
        {
            HtmlToken token = createToken(enumHtmlTokenType.Character);
            token.data = tokenString;
            token.startIndex = token.startIndex - tokenString.Length;
            return token;
        }

        private HtmlToken emitTokenEOF()
        {
            HtmlToken token = createToken(enumHtmlTokenType.EOF);
            return token;
        }

        /// <summary>
        /// read the next token.
        /// </summary>
        /// <returns></returns>
        public HtmlToken ReadNextToken()
        {
            switch (ParseState)
            {
                case enumParseState.DATA:
                    return Data();
                case enumParseState.RCDATA:
                    return RCDATA();
                case enumParseState.Plaintext:
                    return PlainText();
                case enumParseState.RAWTEXT:
                    return RAWTEXT();
                case enumParseState.Script:
                    return Script();
                default:
                    return createToken(enumHtmlTokenType.EOF);
            }


        }

        /// <summary>
        /// occure when there is an parsing error. 
        /// </summary>
        /// <param name="errormessage"></param>
        /// 
        private void ParseError(string errormessage)
        {
            //TODO:
            return;
        }

        /// <summary>
        /// 8.2.4.1 Data state  
        /// </summary>
        private HtmlToken Data()
        {
            char current = consumeNext();
            ///EOF //Emit an end-of-file token.

            int startindex = this._readIndex;

            if (isEOF)
            {
                //EOF //Emit an end-of-file token.
                return emitTokenEOF();
            }
            //"<" (U+003C) //Switch to the tag open state.
            else if (current == '\u003C')
            {
                HtmlToken token = tagOpen();
                token.startIndex = startindex;
                return emitToken(token);
            }
            ///U+0000 NULL //Parse error. Emit the current input character as a character token.
            else if (current == '\u0000')
            {
                ParseError("unexpected null character");
                return emitTokenChar(current);
            }
            /// U+0026 AMPERSAND (&) Switch to the character reference in data state.

            else if (current == '\u0026')
            {
                HtmlToken token = characterReferenceInDataState();
                token.startIndex = startindex;
                return emitToken(token);
            }
            ///Anything else //Emit the current input character as a character token.
            else
            {
                return emitTokenChar(current);
            }

        }

        /// <summary>
        /// 8.2.4.7 PLAINTEXT state
        /// </summary>
        /// <returns></returns>
        private HtmlToken PlainText()
        {
            //Consume the next input character:
            char current = consumeNext();

            //U+0000 NULL
            if (current == '\u0000')
            {
                //Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
                return emitTokenChar('\uFFFD');
            }

            //EOF
            //Emit an end-of-file token.
            else if (isEOF)
            {
                return emitTokenEOF();
            }
            else
            {
                //Anything else
                //Emit the current input character as a character token.
                return emitTokenChar(current);

            }

        }


        /// <summary>
        /// 8.2.4.2 Character reference in data state
        /// </summary>
        /// <returns></returns>
        private HtmlToken characterReferenceInDataState()
        {
            ParseState = enumParseState.DATA;
            //Switch to the data state.
            //Attempt to consume a character reference, with no additional allowed character.

            HtmlToken token = consumeCharacterReference(false, null);

            //If nothing is returned, emit a U+0026 AMPERSAND character (&) token.
            //Otherwise, emit the character tokens that were returned.
            if (token == null)
            {
                return emitTokenChar('\u0026');
            }
            else
            {
                return token;
            }
        }

        /// <summary>
        ///   8.2.4.41 Character reference in attribute value state
        /// </summary>
        /// <param name="additionalAllowedChar"></param>
        /// <param name="caller"></param>
        /// <returns></returns>
        private void characterReferenceInAttributeValue(HtmlToken parenttoken, char? additionalAllowedChar)
        {
            //Attempt to consume a character reference.
            HtmlToken chartoken = consumeCharacterReference(true, additionalAllowedChar);

            // If nothing is returned, append a U+0026 AMPERSAND character (&) to the current attribute's value.
            if (chartoken == null)
            {
                //parenttoken.currentAttributeValue += '\u0026'.ToString();
                parenttoken.appendAttributeChar('\u0026');

            }
            else
            {
                /// Otherwise, append the returned character tokens to the current attribute's value.
               // parenttoken.currentAttributeValue += chartoken.data;
                parenttoken.appendAttributeString(chartoken.data);
            }

            ///Finally, switch back to the attribute value state that switched into this state.

            //if (caller == "attdouble")
            //{
            //    return attributeValueDoubleQuoted(parenttoken);
            //}
            //else if (caller == "unquoted")
            //{
            //    return attributeValueUnquoted(parenttoken);
            //}
            //else if (caller == "attsingle")
            //{
            //    return attributeValueSingleQuoted(parenttoken);
            //}
            //else
            //{
            //    return attributeValueUnquoted(parenttoken);
            //}

        }

        /// <summary>
        /// 8.2.4.4 Character reference in RCDATA state
        /// </summary>
        /// <returns></returns>
        private HtmlToken characterReferenceInRCDATA()
        {

            //Switch to the RCDATA state.
            ParseState = enumParseState.RCDATA;

            //Attempt to consume a character reference, with no additional allowed character.
            HtmlToken token = consumeCharacterReference(false, null);

            //If nothing is returned, emit a U+0026 AMPERSAND character (&) token.
            //Otherwise, emit the character tokens that were returned.
            if (token == null)
            {
                return emitTokenChar('\u0026');

            }
            else
            {
                return token;
            }
        }

        /// <summary>
        /// 8.2.4.69 Tokenizing character references
        /// </summary>
        /// <param name="partOfAttribute"></param>
        /// <param name="additionalAllowedChar"></param>
        /// <returns></returns>
        private HtmlToken consumeCharacterReference(bool partOfAttribute, char? additionalAllowedChar)
        {
            HtmlToken token = createToken(enumHtmlTokenType.Character);

            moveNext();

            char current = getCurrentChar();

            // "tab" (U+0009)//"LF" (U+000A)//"FF" (U+000C)//U+0020 SPACE//U+003C LESS-THAN SIGN//U+0026 AMPERSAND//EOF
            //The additional allowed character, if there is one
            if (isEOF || current.isOneOf('\u0009', '\u000A', '\u000D', '\u000C', '\u0020', '\u003C', '\u0026') || (additionalAllowedChar != null && current == additionalAllowedChar))
            {
               
                //No characters are consumed, and nothing is returned
                reConsume();
                return null;
            }

            // "#" (U+0023)
            //Consume the U+0023 NUMBER SIGN.
            else if (current == '\u0023')
            {
                int codepoint = 0;

                //U+0078 LATIN SMALL LETTER X
                //U+0058 LATIN CAPITAL LETTER X
                // using ASCII hex digits.
                //Consume as many characters as match the range of characters given above (ASCII hex digits or ASCII digits).
                //If no characters match the range, then don't consume any characters (and unconsume the U+0023 NUMBER SIGN character and, if appropriate, the X character). This is a parse error; nothing is returned.

                if (LookupChar(1) == '\u0078' || LookupChar(1) == '\u0058')
                {
                    if (CommonIdoms.isAsciiHexDigit(LookupChar(2)))
                    {
                        moveNext(); // == 0078 or 0058.  the X mark. 

                        string hexString = string.Empty;

                        //Consume as many characters as match the range of characters given above
                        while (true)
                        {
                            if (CommonIdoms.isAsciiHexDigit(LookupChar(1)))
                            {
                                hexString += LookupChar(1);
                                moveNext();
                            }
                            else
                            {
                                break;
                            }
                        }

                        // Otherwise, if the next character is a U+003B SEMICOLON, consume that too. If it isn't, there is a parse error.
                        if (LookupChar(1) == '\u003B')
                        {
                            moveNext();
                        }
                        else
                        {
                            ParseError("; expected");
                        }

                        codepoint = Convert.ToInt32(hexString, 16);

                    }
                    else
                    {
                        //If no characters match the range, then don't consume any characters and unconsume the U+0023 
                        reConsume();  // unconsume.
                    }

                }

                else
                {
                    //Anything else
                    //Follow the steps below, but using ASCII digits.
                    //Consume as many characters as match the range of characters given above (ASCII hex digits or ASCII digits).
                    //If no characters match the range, then don't consume any characters (and unconsume the U+0023 NUMBER SIGN character and, if appropriate, the X character). This is a parse error; nothing is returned.
                    string asciiString = string.Empty;

                    //Consume as many characters as match the range of characters given above
                    while (true)
                    {
                        if (CommonIdoms.isAsciiDigit(LookupChar(1)))
                        {
                            asciiString += LookupChar(1);
                            moveNext();
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Otherwise, if the next character is a U+003B SEMICOLON, consume that too. If it isn't, there is a parse error.
                    if (LookupChar(1) == '\u003B')
                    {
                        moveNext();
                    }
                    else
                    {
                        ParseError("; expected");
                    }

                    int.TryParse(asciiString, out codepoint); 
   
                }

                //If one or more characters match the range, then take them all and interpret the string of characters as a number (either hexadecimal or decimal as appropriate).
                // we have the codepoint now. 

                //If that number is one of the numbers in the first column of the following table, 
                // then this is a parse error. Find the row with that number in the first column, 
                //and return a character token for the Unicode character given in the second column of that row.

                if (CharacterReferences.unicodeCharacters().ContainsKey(codepoint))
                {
                    token.data = CharacterReferences.unicodeCharacters()[codepoint];
                    return emitToken(token);
                }

                // Otherwise, if the number is in the range 0xD800 to 0xDFFF or is greater than 0x10FFFF, 
                //then this is a parse error. Return a U+FFFD REPLACEMENT CHARACTER character token.

                if ((codepoint >= Convert.ToInt32("0xD800", 16) && codepoint <= Convert.ToInt32("0xDFFF", 16))
                    || codepoint > Convert.ToInt32("0x10FFFF", 16))
                {
                    ParseError("char in the range of range 0xD800 to 0xDFFF or greater than 0x10FFFF not allowed.");
                    token.data = '\uFFFD'.ToString();
                    return emitToken(token);
                }

                ///Otherwise, return a character token for the Unicode character whose code point is that number. 
                ///Additionally, if the number is in the range 0x0001 to 0x0008, 0x000D to 0x001F, 0x007F to 0x009F, 
                ///0xFDD0 to 0xFDEF, or is one of 0x000B, 0xFFFE, 0xFFFF, 0x1FFFE, 0x1FFFF, 0x2FFFE, 
                ///0x2FFFF, 0x3FFFE, 0x3FFFF, 0x4FFFE, 0x4FFFF, 0x5FFFE, 0x5FFFF, 0x6FFFE, 0x6FFFF, 0x7FFFE,
                ///0x7FFFF, 0x8FFFE, 0x8FFFF, 0x9FFFE, 0x9FFFF, 0xAFFFE, 0xAFFFF, 0xBFFFE, 0xBFFFF, 0xCFFFE, 
                ///0xCFFFF, 0xDFFFE, 0xDFFFF, 0xEFFFE, 0xEFFFF, 0xFFFFE, 0xFFFFF, 0x10FFFE, or 0x10FFFF, 
                ///then this is a parse error.
                if (!CharacterReferences.isValidCharacters(codepoint))
                {
                    ParseError("invalid unicode code point.");
                }

                token.data = char.ConvertFromUtf32(codepoint);

                return emitToken(token);

            }
            else
            {
                //Consume the maximum number of characters possible, with the consumed characters matching one of the 
                // identifiers in the first column of the named character references table (in a case-sensitive manner).
                int advancei = 0;
                string consumeString = string.Empty;
                while (true)
                {
                    if (CommonIdoms.isAlphanumeric(LookupChar(advancei)))
                    {
                        consumeString += LookupChar(advancei);
                        advancei += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (advancei > 0 && LookupChar(advancei) == '\u003B')
                {
                    consumeString += '\u003B';
                    advancei += 1;
                }

                //If no match can be made, then no characters are consumed, and nothing is returned. 
                //In this case, if the characters after the U+0026 AMPERSAND character (&) consist of 
                //a sequence of one or more alphanumeric ASCII characters followed by a U+003B SEMICOLON character (;),
                //then this is a parse error.
                if (!CharacterReferences.namedCharacters().ContainsKey(consumeString))
                {
                    // no consumed. 
                    reConsume();
                    if (consumeString.LastIndexOf(";") > 0)
                    {
                        ParseError("unexpected characters, named character not founded");
                    }
                    //nothing is return.
                }
                else
                {
                    // If the character reference is being consumed as part of an attribute, 
                    // and the last character matched is not a ";" (U+003B) character, 
                    // and the next character is either a "=" (U+003D) character or an alphanumeric ASCII character,
                    // then, for historical reasons, all the characters that were matched after the U+0026 AMPERSAND 
                    // character (&) must be unconsumed, and nothing is returned.
                    if (partOfAttribute)
                    {
                        int stringlength = consumeString.Length;
                        if (consumeString[stringlength - 1] != '\u003B' && (consumeString[stringlength - 2] == '\u003D' || CommonIdoms.isAlphanumeric(consumeString[stringlength - 2])))
                        {
                            // then, for historical reasons, all the characters that were matched after the U+0026 AMPERSAND 
                            // character (&) must be unconsumed, and nothing is returned.

                            // do not consume afer &, do not return. 

                            //However, if this next character is in fact a "=" (U+003D) character, 
                            //then this is a parse error, because some legacy user agents will misinterpret the markup in those cases.
                            if (consumeString[stringlength - 2] == '\u003D')
                            {
                                ParseError("some legacy user agents may misinterpret this");
                            }
                        }
                        else
                        {
                            string namedstring = CharacterReferences.getNamedCharacters(consumeString);
                            moveAhead(advancei);
                            token.data = namedstring;
                            return emitToken(token);
                        }
                    }

                    else
                    {
                        //Otherwise, a character reference is parsed. If the last character matched is not a ";" (U+003B) character, there is a parse error.
                        //Return one or two character tokens for the character(s) corresponding to the character reference name (as given by the second column of the named character references table).
                        string namedstring = CharacterReferences.getNamedCharacters(consumeString);
                        moveAhead(advancei);
                        token.data = namedstring;
                        return emitToken(token);
                    }

                }
            }


            return null;

        }

        /// <summary>
        /// 8.2.4.8 Tag open state
        /// </summary>
        /// <returns></returns>
        private HtmlToken tagOpen()
        {
            //Consume the next input character
            char current = consumeNext();

            if (current == '\u0021')
            {
                //"!" (U+0021)
                //Switch to the markup declaration open state.
                return markupDeclarationOpen();
            }
            else if (current == '\u002F')
            {
                //"/" (U+002F)
                //Switch to the end tag open state.
                return endTagOpen();

            }
            else if (CommonIdoms.isUppercaseAscii(current))
            {
                //Uppercase ASCII letter    create a new start tag token, set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point), then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)

                HtmlToken token = createToken(enumHtmlTokenType.StartTag);
                token.tagName = current.ToString().ToLower();
                return tagName(token);
            }
            else if (CommonIdoms.isLowercaseAscii(current))
            {
                //Lowercase ASCII letter //Create a new start tag token, set its tag name to the current input character, then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
                HtmlToken token = createToken(enumHtmlTokenType.StartTag);
                token.tagName = current.ToString();
                return tagName(token);
            }

            else if (current == '\u003F')
            {
                //"?" (U+003F)
                //Parse error. Switch to the bogus comment state.
                ParseError("unexpected ? mark ");
                return bogusComment();
            }

            else
            {
                //Anything else
                //Parse error. Switch to the data state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
                reConsume();
                ParseError("unexpected character");
                ParseState = enumParseState.DATA;

                return emitTokenChar('\u003C');

            }


        }

        /// <summary>
        /// 8.2.4.45 Markup declaration open state
        /// </summary>
        /// <returns></returns>
        private HtmlToken markupDeclarationOpen()
        {
            //If the next two characters are both "-" (U+002D) characters, consume those two characters, create a comment token whose data is the empty string, and switch to the comment start state.
            if (LookupChar(1) == '\u002D' && LookupChar(2) == '\u002D')
            {
                HtmlToken token = createToken(enumHtmlTokenType.Comment);

                moveAhead(2);

                return commentStart(token);
            }
            else
            {

                string nextseven = LookupChar(1).ToString() + LookupChar(2).ToString() + LookupChar(3).ToString() + LookupChar(4).ToString() + LookupChar(5).ToString() + LookupChar(6).ToString() + LookupChar(7).ToString();

                nextseven = nextseven.ToUpper();

                //Otherwise, if the next seven characters are an ASCII case-insensitive match for the word "DOCTYPE", then consume those characters and switch to the DOCTYPE state.
                if (nextseven == "DOCTYPE")
                {
                    moveAhead(7);
                    return DOCTYPE();
                }
                else if (nextseven == "[CDATA[")
                {
                    //TODO: check this adjusted current node. 
                    //Otherwise, if there is an adjusted current node and it is not an element in the HTML namespace and the next seven characters are a case-sensitive match for the string "[CDATA[" (the five uppercase letters "CDATA" with a U+005B LEFT SQUARE BRACKET character before and after), then consume those characters and switch to the CDATA section state.

                    moveAhead(7);
                    return CDATA();

                }
                else
                {

                    //Otherwise, this is a parse error. Switch to the bogus comment state. The next character that is consumed, if any, is the first character that will be in the comment.
                    ParseError("unexpected char in mark declaration open");
                    return bogusComment();

                }

            }


        }

        /// <summary>
        /// 8.2.4.10 Tag name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken tagName(HtmlToken token)
        {
            /// Consume the next input character:
            while (true)
            {
                char current = consumeNext();

                //"tab" (U+0009)//"LF" (U+000A)//"FF" (U+000C)//U+0020 SPACE
                //Switch to the before attribute name state. 
                if (CommonIdoms.isSpaceCharacters(current))
                ///if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020', '\u000D'))
                {
                    return beforeAttributeName(token);
                }
 
                //"/" (U+002F)
                //Switch to the self-closing start tag state.
                else if (current == '\u002F')
                {
                    return selfClosingStartTag(token);
                }
                //">" (U+003E)
                //Switch to the data state. Emit the current tag token.
                else if (current == '\u003E')
                {
                    ParseState = enumParseState.DATA;

                    return emitToken(token);
                }
                //Uppercase ASCII letter
                //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current tag token's tag name.
                else if (CommonIdoms.isUppercaseAscii(current))
                {
                    token.tagName += current.ToString().ToLower();
                }

                //U+0000 NULL
                //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current tag token's tag name.
                else if (current == '\u0000')
                {
                    ParseError("unexpected null character");
                    token.tagName += '\uFFFD'.ToString();
                }
                //EOF
                //Parse error. Switch to the data state. Reconsume the EOF character.
                else if (isEOF)
                {
                    ParseState = enumParseState.DATA;
                    reConsume();
                    return Data();
                }

                //Anything else
                //Append the current input character to the current tag token's tag name.
                else
                { 
                    token.tagName += current.ToString();
                }

            }
        }

        /// <summary>
        /// 8.2.4.44 Bogus comment state
        /// </summary>
        /// <returns></returns>
        private HtmlToken bogusComment()
        {
            //Consume every character up to and including the first ">" (U+003E) character or the end of the file (EOF), whichever comes first. Emit a comment token whose data is the concatenation of all the characters starting from and including the character that caused the state machine to switch into the bogus comment state, up to and including the character immediately before the last consumed character (i.e. up to the character just before the U+003E or EOF character), but with any U+0000 NULL characters replaced by U+FFFD REPLACEMENT CHARACTER characters. (If the comment was started by the end of the file (EOF), the token is empty. Similarly, the token is empty if it was generated by the string "<!>".)

            //Switch to the data state.

            //If the end of the file was reached, reconsume the EOF character.

            HtmlToken token = createToken(enumHtmlTokenType.Character);

            StringBuilder sb = new StringBuilder();
            while (true)
            {
                char current = consumeNext();

                if (current == '\u003E')
                {
                    break;
                }

                if (isEOF)
                {
                    reConsume();
                    break;
                }

                sb.Append(current.ToString());

            }

            token.data = sb.ToString();



            return token;

        }

        /// <summary>
        /// 8.2.4.9 End tag open state
        /// </summary>
        /// <returns></returns>
        private HtmlToken endTagOpen()
        {
            //Consume the next input character:
            char current = consumeNext();

            //Uppercase ASCII letter
            //Create a new end tag token, set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point), then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
            if (CommonIdoms.isUppercaseAscii(current))
            {
                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                token.tagName = current.ToString().ToLower();
                return tagName(token);
            }

            //Lowercase ASCII letter
            //Create a new end tag token, set its tag name to the current input character, then switch to the tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
            else if (CommonIdoms.isLowercaseAscii(current))
            {
                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                token.tagName = current.ToString();
                return tagName(token);
            }

            //">" (U+003E)
            //Parse error. Switch to the data state.
            else if (current == '\u003E')
            {
                ParseError("unexpected end tag");
                return Data();
            }
            //EOF
            //Parse error. Switch to the data state. Emit a U+003C LESS-THAN SIGN character token and a U+002F SOLIDUS character token. Reconsume the EOF character.
            else if (isEOF)
            {
                reConsume();
                ParseState = enumParseState.DATA;

                return emitTokenString('\u003C'.ToString() + '\u002F'.ToString());

            }
            else
            {
                //Anything else
                //Parse error. Switch to the bogus comment state.
                ParseError("unexpected end tag token");
                return bogusComment();
            }

        }

        /// <summary>
        /// 8.2.4.34 Before attribute name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken beforeAttributeName(HtmlToken token)
        {
            //Consume the next input character:

            char current = consumeNext();

            //"tab" (U+0009) //"LF" (U+000A)  //"FF" (U+000C) //U+0020 SPACE
            //Ignore the character.
            //  if (current.isOneOf('\u0009', '\u000A', '\u000D', '\u000C', '\u0020'))
            if (CommonIdoms.isSpaceCharacters(current))
            {
                return beforeAttributeName(token);  // recursive,  ignore = do it again. 
            }
            //"/" (U+002F)
            //Switch to the self-closing start tag state.
            else if (current == '\u002F')
            {
                return selfClosingStartTag(token);
            }

            //">" (U+003E)
            //Switch to the data state. Emit the current tag token.
            else if (current == '\u003E')
            {
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }

            //Uppercase ASCII letter
            //Start a new attribute in the current tag token. Set that attribute's name to the lowercase version of the current input character (add 0x0020 to the character's code point), and its value to the empty string. Switch to the attribute name state.
            else if (CommonIdoms.isUppercaseAscii(current))
            {
                token.startNewAttribute();
                token.currentAttributeName = current.ToString().ToLower();
                //token.currentAttributeValue = string.Empty;
                token.CleanAttributeValue();

                return attributeName(token);
            }


            //U+0000 NULL
            //Parse error. Start a new attribute in the current tag token. Set that attribute's name to a U+FFFD REPLACEMENT CHARACTER character, and its value to the empty string. Switch to the attribute name state.
            else if (current == '\u0000')
            {
                ParseError("unexpected null characters");
                token.startNewAttribute();
                token.currentAttributeName = '\uFFFD'.ToString().ToLower();
                // token.currentAttributeValue = string.Empty;
                token.CleanAttributeValue();
                return attributeName(token);
            }
            //EOF
            //Parse error. Switch to the data state. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseState = enumParseState.DATA;
                reConsume();
                return Data();
            }
            else
            {
                //U+0022 QUOTATION MARK (")    //"'" (U+0027)   //"<" (U+003C)      //"=" (U+003D)
                //Parse error. Treat it as per the "anything else" entry below.
                if (current.isOneOf('\u0022', '\u0027', '\u003C', '\u003D'))
                {
                    ParseError(@"unexpected char "" ' < or =");
                }

                //Anything else
                //Start a new attribute in the current tag token. Set that attribute's name to the current input character, and its value to the empty string. Switch to the attribute name state.

                token.startNewAttribute();
                token.currentAttributeName = current.ToString();
                // token.currentAttributeValue = string.Empty;
                token.CleanAttributeValue();
                return attributeName(token);
            }
        }

        /// <summary>
        /// 8.2.4.35 Attribute name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken attributeName(HtmlToken token)
        {
            //  Consume the next input character:
            while (true)
            {
                char current = consumeNext();

                //"tab" (U+0009)   //"LF" (U+000A)   //"FF" (U+000C)    //U+0020 SPACE
                //Switch to the after attribute name state.
                if (CommonIdoms.isSpaceCharacters(current))
              //  if (current.isOneOf('\u0009', '\u000A', '\u000D', '\u000C', '\u0020'))
                {
                    return afterAttributeName(token);
                }

                //"/" (U+002F)
                //Switch to the self-closing start tag state.
                else if (current == '\u002F')
                {
                    return selfClosingStartTag(token);
                }
                //"=" (U+003D)
                //Switch to the before attribute value state.
                else if (current == '\u003D')
                {
                    return beforeAttributeValue(token);
                }

                //">" (U+003E)
                //Switch to the data state. Emit the current tag token.
                else if (current == '\u003E')
                {
                    ParseState = enumParseState.DATA;
                    return emitToken(token);
                }
                //Uppercase ASCII letter
                //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current attribute's name.
                else if (CommonIdoms.isUppercaseAscii(current))
                {
                    token.currentAttributeName += current.ToString().ToLower();
                }

                //U+0000 NULL
                //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's name.
                else if (current == '\u0000')
                {
                    ParseError("unexpected null character");
                    token.currentAttributeName += '\uFFFD'.ToString();
                }
                //EOF
                //Parse error. Switch to the data state. Reconsume the EOF character.
                else if (isEOF)
                {
                    ParseError("unexpected EOF");
                    reConsume();
                    return Data();
                }
                else
                {
                    //U+0022 QUOTATION MARK (")   //"'" (U+0027)  //"<" (U+003C)
                    if (current.isOneOf('\u0022', '\u0027', '\u003C'))
                    {
                        ParseError(@"unexpected "", ' or < characters");
                    }

                    //Parse error. Treat it as per the "anything else" entry below.

                    //Anything else
                    //Append the current input character to the current attribute's name.
                    token.currentAttributeName += current.ToString();

                }
            }
        }

        /// <summary>
        /// 8.2.4.36 After attribute name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken afterAttributeName(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)  //"LF" (U+000A)  //"FF" (U+000C) //U+0020 SPACE
            //Ignore the character.
            if (CommonIdoms.isSpaceCharacters(current))
           // if (current.isOneOf('\u0009', '\u000A', '\u000D', '\u000C', '\u0020'))
            {
                return afterAttributeName(token);   //recursive = ignore = advanceone && continue.
            }
            //"/" (U+002F)
            //Switch to the self-closing start tag state.
            else if (current == '\u002F')
            {
                return selfClosingStartTag(token);
            }
            //"=" (U+003D)
            //Switch to the before attribute value state.
            else if (current == '\u003D')
            {
                return beforeAttributeValue(token);
            }

            //">" (U+003E)
            //Switch to the data state. Emit the current tag token.
            else if (current == '\u003E')
            {
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }

            //Uppercase ASCII letter
            //Start a new attribute in the current tag token. Set that attribute's name to the lowercase version of the current input character (add 0x0020 to the character's code point), and its value to the empty string. Switch to the attribute name state.
            else if (CommonIdoms.isUppercaseAscii(current))
            {
                token.startNewAttribute();
                token.currentAttributeName = current.ToString();
                //token.currentAttributeValue = string.Empty;
                token.CleanAttributeValue();
                return attributeName(token);
            }
            //U+0000 NULL
            //Parse error. Start a new attribute in the current tag token. Set that attribute's name to a U+FFFD REPLACEMENT CHARACTER character, and its value to the empty string. Switch to the attribute name state.
            else if (current == '\u0000')
            {
                ParseError("unexpected null character");
                token.startNewAttribute();
                token.currentAttributeName = '\uFFFD'.ToString();
                //token.currentAttributeValue = string.Empty;
                token.CleanAttributeValue();
                return attributeName(token);

            }
            else if (isEOF)
            {
                //EOF
                //Parse error. Switch to the data state. Reconsume the EOF character.
                ParseError("unexpected EOF");
                reConsume();
                return Data();
            }
            else
            {

                //U+0022 QUOTATION MARK (")    //"'" (U+0027)          //"<" (U+003C)
                //Parse error. Treat it as per the "anything else" entry below.
                if (current == '\u0022' || current == '\u0027' || current == '\u003C')
                {
                    ParseError(@"unexpected "", ' or < characters");
                }

                //Anything else
                //Start a new attribute in the current tag token. Set that attribute's name to the current input character, and its value to the empty string. Switch to the attribute name state.
                token.startNewAttribute();
                token.currentAttributeName = current.ToString();
                // token.currentAttributeValue = string.Empty;
                token.CleanAttributeValue();
                return attributeName(token);
            }

        }

        /// <summary>
        /// 8.2.4.37 Before attribute value state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken beforeAttributeValue(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)   //"LF" (U+000A)   //"FF" (U+000C)    //U+0020 SPACE
            //Ignore the character.
            if (CommonIdoms.isSpaceCharacters(current))
           // if (current.isOneOf('\u0009', '\u000A', '\u000D', '\u000C', '\u0020'))
            {
                return beforeAttributeValue(token);   //recursive = ignore = advanceone && continue.
            }

            //U+0022 QUOTATION MARK (")
            //Switch to the attribute value (double-quoted) state.
            else if (current == '\u0022')
            {
                return attributeValueDoubleQuoted(token);
            }
            //U+0026 AMPERSAND (&)
            //Switch to the attribute value (unquoted) state. Reconsume the current input character.
            else if (current == '\u0022')
            {
                reConsume();
                return attributeValueUnquoted(token);
            }
            //"'" (U+0027)
            //Switch to the attribute value (single-quoted) state.
            else if (current == '\u0027')
            {
                return attributeValueSingleQuoted(token);
            }
            //U+0000 NULL
            //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value. Switch to the attribute value (unquoted) state.
            else if (current == '\u0000')
            {
                ParseError("unexpected null in attribue value");
                // token.currentAttributeValue += '\uFFFD'.ToString();
                token.appendAttributeChar('\uFFFD');
                return attributeValueUnquoted(token);
            }

            //">" (U+003E)
            //Parse error. Switch to the data state. Emit the current tag token.
            else if (current == '\u003E')
            {
                ParseError("attribute value expected;");
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }
            //EOF
            //Parse error. Switch to the data state. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                reConsume();
                return Data();
            }
            else
            {
                //"<" (U+003C)         //"=" (U+003D)        //"`" (U+0060)
                //Parse error. Treat it as per the "anything else" entry below.

                if (current.isOneOf('\u0022', '\u0027', '\u003C'))
                {
                    ParseError(@"unexpected "", ' or < characters");
                }

                //Anything else
                //Append the current input character to the current attribute's value. Switch to the attribute value (unquoted) state.
                //token.currentAttributeValue += current.ToString();
                token.appendAttributeChar(current);
                return attributeValueUnquoted(token);
            }

        }

        /// <summary>
        ///   8.2.4.43 Self-closing start tag state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken selfClosingStartTag(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //">" (U+003E)
            //Set the self-closing flag of the current tag token. Switch to the data state. Emit the current tag token.
            if (current == '\u003E')
            {
                token.isSelfClosing = true;
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }
            //EOF
            //Parse error. Switch to the data state. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                reConsume();
                return Data();
            }
            else
            {
                //Anything else
                //Parse error. Switch to the before attribute name state. Reconsume the character.
                ParseError("unexpected char in  self close tag");
                reConsume();
                return beforeAttributeName(token);
            }

        }

        /// <summary>
        ///  8.2.4.38 Attribute value (double-quoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken attributeValueDoubleQuoted(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //U+0022 QUOTATION MARK (")
                //Switch to the after attribute value (quoted) state.
                if (current == '\u0022')
                {
                    return afterAttributeValueQuoted(token);
                }

                //U+0026 AMPERSAND (&)
                //Switch to the character reference in attribute value state, with the additional allowed character being U+0022 QUOTATION MARK (").
                else if (current == '\u0026')
                {
                     characterReferenceInAttributeValue(token, '\u0022');
                }
                //U+0000 NULL
                //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value.
                else if (current == '\u0000')
                {
                    // token.currentAttributeValue += '\uFFFD';
                    token.appendAttributeChar('\uFFFD');
                }
                //EOF
                //Parse error. Switch to the data state. Reconsume the EOF character.
                else if (isEOF)
                {
                    ParseError("unexpected EOF ");
                    ParseState = enumParseState.DATA;
                    reConsume();
                    return Data();
                }
                else
                {
                    //Anything else
                    //Append the current input character to the current attribute's value.
                    // token.currentAttributeValue += current.ToString();
                    token.appendAttributeChar(current);
                }

            }


        }

        /// <summary>
        ///  8.2.4.40 Attribute value (unquoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken attributeValueUnquoted(HtmlToken token)
        {
            //Consume the next input character:
            while (true)
            {
                char current = consumeNext();

                //"tab" (U+0009) //"LF" (U+000A)  //"FF" (U+000C)  //U+0020 SPACE
                //Switch to the before attribute name state.
                if (CommonIdoms.isSpaceCharacters(current))
                //if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
               // if (current == '\u0009' || current == '\u000A' || current == '\u000C' || current == '\u0020')
                {
                    return beforeAttributeName(token);
                }
                //U+0026 AMPERSAND (&)
                //Switch to the character reference in attribute value state, with the additional allowed character being ">" (U+003E).
                else if (current == '\u0026')
                {
                      characterReferenceInAttributeValue(token, '\u003E');
                }

                //">" (U+003E)
                //Switch to the data state. Emit the current tag token.
                else if (current == '\u003E')
                {
                    ParseState = enumParseState.DATA;
                    return emitToken(token);
                }
                //U+0000 NULL
                //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value.
                else if (current == '\u0000')
                {
                    ParseError("unexpected null character");
                    //token.currentAttributeValue += '\uFFFD'.ToString();
                    token.appendAttributeChar('\uFFFD');
                }
                //EOF
                //Parse error. Switch to the data state. Reconsume the EOF character.
                else if (isEOF)
                {
                    ParseError("unexpectd EOF");
                    reConsume();
                    ParseState = enumParseState.DATA;
                    return Data();
                }
                else
                {

                    //U+0022 QUOTATION MARK (") //"'" (U+0027) //"<" (U+003C) //"=" (U+003D)  //"`" (U+0060)
                    //Parse error. Treat it as per the "anything else" entry below.
                    if (current == '\u0022' || current == '\u0027' || current == '\u0022' || current == '\u003C' || current == '\u003D' || current == '\u0060')
                    {
                        ParseError("unexpected double/single quote, < = ` chars ");
                    }
                    else
                    {
                        //Anything else
                        //Append the current input character to the current attribute's value.
                        // token.currentAttributeValue += current.ToString();
                        token.appendAttributeChar(current);

                    }

                }


            }


        }

        /// <summary>
        ///  8.2.4.39 Attribute value (single-quoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken attributeValueSingleQuoted(HtmlToken token)
        {
            //Consume the next input character:
            while (true)
            {
                char current = consumeNext();

                //"'" (U+0027)
                //Switch to the after attribute value (quoted) state.
                if (current == '\u0027')
                {
                    return afterAttributeValueQuoted(token);
                }

                //U+0026 AMPERSAND (&)
                //Switch to the character reference in attribute value state, with the additional allowed character being "'" (U+0027).
                else if (current == '\u0026')
                {
                      characterReferenceInAttributeValue(token, '\u0027');
                }
                //U+0000 NULL
                //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current attribute's value.
                else if (current == '\u0000')
                {
                    ParseError("unexpected null character");
                    //token.currentAttributeValue += '\uFFFD'.ToString();
                    token.appendAttributeChar('\uFFFD');
                }
                //EOF
                //Parse error. Switch to the data state. Reconsume the EOF character.
                else if (isEOF)
                {
                    ParseError("unexpected EOF ");
                    ParseState = enumParseState.DATA;
                    reConsume();
                    return Data();
                }

                else
                {
                    //Anything else
                    //Append the current input character to the current attribute's value.
                    // token.currentAttributeValue += current.ToString();
                    token.appendAttributeChar(current);

                }

            }
        }

        /// <summary>
        /// 8.2.4.42 After attribute value (quoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken afterAttributeValueQuoted(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)   //"LF" (U+000A)   //"FF" (U+000C)  //U+0020 SPACE
            //Switch to the before attribute name state.
            if (CommonIdoms.isSpaceCharacters(current))
           // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                return beforeAttributeName(token);
            }
            //"/" (U+002F)
            //Switch to the self-closing start tag state.

            else if (current == '\u002F')
            {
                return selfClosingStartTag(token);
            }
            //">" (U+003E)
            //Switch to the data state. Emit the current tag token.
            else if (current == '\u003E')
            {
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }
            //EOF
            //Parse error. Switch to the data state. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                reConsume();
                return Data();
            }
            else
            {
                //Anything else
                //Parse error. Switch to the before attribute name state. Reconsume the character.
                ParseError("unexpected characters");
                reConsume();
                return beforeAttributeName(token);
            }
        }

        /// <summary>
        /// 8.2.4.46 Comment start state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken commentStart(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            //Switch to the comment start dash state.
            if (current == '\u002D')
            {
                return commentStartDash(token);
            }

            //U+0000 NULL
            //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
            else if (current == '\u0000')
            {
                ParseError("unexpected null character");
                token.data += '\uFFFD'.ToString();
                return this.comment(token);
            }

            //">" (U+003E)
            //Parse error. Switch to the data state. Emit the comment token.
            else if (current == '\u003E')
            {
                ParseError("unexpected > character");
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }

            //EOF
            //Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                reConsume();
                ParseState = enumParseState.DATA;
                return emitToken(token);

            }
            else
            {
                //Anything else
                //Append the current input character to the comment token's data. Switch to the comment state.
                token.data += current.ToString();
                return this.comment(token);
            }

        }

        /// <summary>
        ///  8.2.4.47 Comment start dash state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken commentStartDash(HtmlToken token)
        {

            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            //Switch to the comment end state
            if (current == '\u002D')
            {
                return commentEnd(token);
            }
            //U+0000 NULL
            //Parse error. Append a "-" (U+002D) character and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
            else if (current == '\u0000')
            {
                ParseError("unexpected null character");
                token.data += '\u002D'.ToString() + '\uFFFD'.ToString();
                return this.comment(token);
            }

            //">" (U+003E)
            //Parse error. Switch to the data state. Emit the comment token.
            else if (current == '\u003E')
            {
                ParseError("unexpected > ");
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }

            //EOF
            //Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                reConsume();
                return emitToken(token);
            }
            else
            {

                //Anything else
                //Append a "-" (U+002D) character and the current input character to the comment token's data. Switch to the comment state.
                token.data += '\u002D'.ToString() + current.ToString();
                return this.comment(token);

            }
        }

        /// <summary>
        ///   8.2.4.48 Comment state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken comment(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:

                char current = consumeNext();

                //"-" (U+002D)
                //Switch to the comment end dash state
                if (current == '\u002D')
                {
                    return commentEndDash(token);
                }

                //U+0000 NULL
                //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the comment token's data.
                else if (current == '\u0000')
                {
                    ParseError("unexpected null character");
                    token.data += '\uFFFD'.ToString();
                }
                //EOF
                //Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
                else if (isEOF)
                {
                    ParseError("unexpected EOF");
                    ParseState = enumParseState.DATA;
                    reConsume();
                    return emitToken(token);
                }
                else
                {
                    //Anything else
                    //Append the current input character to the comment token's data.
                    token.data += current.ToString();
                }

            }
        }

        /// <summary>
        ///  8.2.4.49 Comment end dash state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken commentEndDash(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            //Switch to the comment end state
            if (current == '\u002D')
            {
                return commentEnd(token);
            }

            //U+0000 NULL
            //Parse error. Append a "-" (U+002D) character and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
            else if (current == '\u0000')
            {
                token.data += '\u002D'.ToString() + '\uFFFD'.ToString();
                return this.comment(token);
            }

            //EOF
            //Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                reConsume();
                return emitToken(token);
            }
            else
            {
                //Anything else
                //Append a "-" (U+002D) character and the current input character to the comment token's data. Switch to the comment state.
                token.data += '\u002D'.ToString() + current.ToString();
                return this.comment(token);
            }
        }

        /// <summary>
        ///  8.2.4.50 Comment end state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken commentEnd(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //">" (U+003E)
                //Switch to the data state. Emit the comment token.
                if (current == '\u003E')
                {
                    ParseState = enumParseState.DATA;
                    return emitToken(token);
                }
                //U+0000 NULL
                //Parse error. Append two "-" (U+002D) characters and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
                else if (current == '\u0000')
                {
                    ParseError("unexpected null character");
                    token.data += '\u002D'.ToString() + '\u002D'.ToString() + '\uFFFD'.ToString();
                    return this.comment(token);
                }

                //"!" (U+0021)
                //Parse error. Switch to the comment end bang state.
                else if (current == '\u0021')
                {
                    ParseError("unexpected ! char");
                    return commentEndBang(token);
                }

                //"-" (U+002D)
                //Parse error. Append a "-" (U+002D) character to the comment token's data.
                else if (current == '\u002D')
                {
                    ParseError("unexpected - ");
                    token.data += '\u002D'.ToString();
                }
                //EOF
                //Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
                else if (isEOF)
                {
                    ParseError("unexpected EOF");
                    reConsume();
                    return emitToken(token);
                }
                else
                {
                    //Anything else
                    //Parse error. Append two "-" (U+002D) characters and the current input character to the comment token's data. Switch to the comment state.
                    ParseError("unexpected");
                    token.data += "--" + current.ToString();
                    return this.comment(token);
                }
            }
        }

        /// <summary>
        ///  8.2.4.51 Comment end bang state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken commentEndBang(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            //Append two "-" (U+002D) characters and a "!" (U+0021) character to the comment token's data. Switch to the comment end dash state.
            if (current == '\u002D')
            {
                token.data += "--!";
                return commentEndDash(token);
            }
            //">" (U+003E)
            //Switch to the data state. Emit the comment token.
            else if (current == '\u003E')
            {
                ParseState = enumParseState.DATA;
                return emitToken(token);
            }
            //U+0000 NULL
            //Parse error. Append two "-" (U+002D) characters, a "!" (U+0021) character, and a U+FFFD REPLACEMENT CHARACTER character to the comment token's data. Switch to the comment state.
            else if (current == '\u0000')
            {
                ParseError("unexpected null char");
                token.data += "--!" + '\uFFFD'.ToString();
                return this.comment(token);
            }
            //EOF
            //Parse error. Switch to the data state. Emit the comment token. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                reConsume();
                return emitToken(token);

            }
            else
            {
                //Anything else
                //Append two "-" (U+002D) characters, a "!" (U+0021) character, and the current input character to the comment token's data. Switch to the comment state.
                token.data += "--!" + current.ToString();
                return this.comment(token);
            }
        }

        /// <summary>
        /// 8.2.4.3 RCDATA state
        /// </summary>
        /// <returns></returns>
        private HtmlToken RCDATA()
        {
            //Consume the next input character:
            char current = consumeNext();
            int startindex = this._readIndex;

            //U+0026 AMPERSAND (&)
            //Switch to the character reference in RCDATA state.
            if (current == '\u0026')
            {
                HtmlToken token = characterReferenceInRCDATA();
                token.startIndex = startindex;
                return emitToken(token);

            }
            //"<" (U+003C)
            //Switch to the RCDATA less-than sign state.
            else if (current == '\u003C')
            {
                HtmlToken token = RCDATALessThanSign();
                token.startIndex = startindex;
                return emitToken(token);
            }
            //EOF
            //Emit an end-of-file token.
            else if (isEOF)
            {
                //EOF //Emit an end-of-file token.
                return emitTokenEOF();
            }
            //U+0000 NULL
            //Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
            else if (current == '\u0000')
            {
                return emitTokenChar('\uFFFD');

            }
            else
            {
                //Anything else
                //Emit the current input character as a character token.
                return emitTokenChar(current);

            }

        }

        /// <summary>
        /// 8.2.4.11 RCDATA less-than sign state
        /// </summary>
        /// <returns></returns>
        private HtmlToken RCDATALessThanSign()
        {

            //Consume the next input character:

            char current = consumeNext();

            if (current == '\u002F')
            {
                //"/" (U+002F)
                //Set the temporary buffer to the empty string. Switch to the RCDATA end tag open state.
                _buffer.Clear();
                return RCDATAEndTagOpen();
            }
            else
            {

                //Anything else
                //Switch to the RCDATA state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
                ParseState = enumParseState.RCDATA;
                reConsume();
                return emitTokenChar('\u003C');
            }

        }

        /// <summary>
        ///8.2.4.12 RCDATA end tag open state
        /// </summary>
        /// <returns></returns>
        private HtmlToken RCDATAEndTagOpen()
        {

            //Consume the next input character:
            char current = consumeNext();

            //Uppercase ASCII letter
            if (CommonIdoms.isUppercaseAscii(current))
            {

                //Create a new end tag token, and set its tag name to the lowercase version of the current input character. Append the current input character to the temporary buffer. Finally, switch to the RCDATA end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                token.tagName = current.ToString().ToLower();

                _buffer.Append(current);

                return RCDATAEndTagName(token);

            }
            //Lowercase ASCII letter
            else if (CommonIdoms.isLowercaseAscii(current))
            {
                //Create a new end tag token, and set its tag name to the current input character. Append the current input character to the temporary buffer. Finally, switch to the RCDATA end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                token.tagName = current.ToString();

                _buffer.Append(current);

                return RCDATAEndTagName(token);

            }
            //Anything else
            else
            {
                //Switch to the RCDATA state. Emit a U+003C LESS-THAN SIGN character token and a U+002F SOLIDUS character token. Reconsume the current input character.
                ParseState = enumParseState.RCDATA;

                reConsume();

                return emitTokenString('\u003C'.ToString() + '\u002F'.ToString());
            }

        }

        /// <summary>
        /// 8.2.4.13 RCDATA end tag name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken RCDATAEndTagName(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //"tab" (U+0009)        //"LF" (U+000A)        //"FF" (U+000C)        //U+0020 SPACE
                if (CommonIdoms.isSpaceCharacters(current))
             // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the before attribute name state. Otherwise, treat it as per the "anything else" entry below.
                    return beforeAttributeName(token);

                }

                //"/" (U+002F)
                //If the current end tag token is an appropriate end tag token, then switch to the self-closing start tag state. Otherwise, treat it as per the "anything else" entry below.
                else if (current == '\u002F' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    return selfClosingStartTag(token);
                }

                //">" (U+003E)
                //If the current end tag token is an appropriate end tag token, then switch to the data state and emit the current tag token. Otherwise, treat it as per the "anything else" entry below.
                else if (current == '\u003E' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    ParseState = enumParseState.DATA;
                    return emitToken(token);
                }
                //Uppercase ASCII letter
                //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current tag token's tag name. Append the current input character to the temporary buffer.
                else if (CommonIdoms.isUppercaseAscii(current))
                {
                    token.tagName += current.ToString().ToLower();
                    _buffer.Append(current);

                }

                //Lowercase ASCII letter
                //Append the current input character to the current tag token's tag name. Append the current input character to the temporary buffer.
                else if (CommonIdoms.isLowercaseAscii(current))
                {
                    token.tagName += current.ToString();
                    _buffer.Append(current);
                }
                else
                {
                    //Anything else
                    //Switch to the RCDATA state. Emit a U+003C LESS-THAN SIGN character token, a U+002F SOLIDUS character token, and a character token for each of the characters in the temporary buffer (in the order they were added to the buffer). Reconsume the current input character.
                    ParseState = enumParseState.RCDATA;

                    reConsume();

                    string chars = '\u003C'.ToString() + '\u002F'.ToString() + _buffer.ToString();
                    return emitTokenString(chars);

                }
            }
        }

        /// <summary>
        /// 8.2.4.5 RAWTEXT state
        /// </summary>
        /// <returns></returns>
        private HtmlToken RAWTEXT()
        {
            //Consume the next input character:
            char current = consumeNext();
            int startindex = this._readIndex;

            //"<" (U+003C)
            //Switch to the RAWTEXT less-than sign state.
            if (current == '\u003C')
            {
                HtmlToken token = RAWTEXTLessThanSign();
                token.startIndex = startindex;
                return emitToken(token);

            }
            //U+0000 NULL
            //Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
            else if (current == '\u0000')
            {
                ParseError("unexpected null character");

                return emitTokenChar('\uFFFD');

            }
            //EOF
            //Emit an end-of-file token.
            else if (isEOF)
            {
                return emitTokenEOF();
            }
            else
            {
                //Anything else
                //Emit the current input character as a character token.
                return emitTokenChar(current);
            }

        }

        /// <summary>
        /// 8.2.4.14 RAWTEXT less-than sign state
        /// </summary>
        /// <returns></returns>
        private HtmlToken RAWTEXTLessThanSign()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"/" (U+002F)
            //Set the temporary buffer to the empty string. Switch to the RAWTEXT end tag open state.
            if (current == '\u002F')
            {
                _buffer.Clear();
                return RAWTEXTEndTagOpen();
            }
            else
            {
                //Anything else
                //Switch to the RAWTEXT state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
                ParseState = enumParseState.RAWTEXT;
                reConsume();

                return emitTokenChar('\u003C');
            }

        }
        /// <summary>
        /// 8.2.4.15 RAWTEXT end tag open state
        /// </summary>
        /// <returns></returns>
        private HtmlToken RAWTEXTEndTagOpen()
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //Uppercase ASCII letter

                if (CommonIdoms.isUppercaseAscii(current))
                {
                    //Create a new end tag token, and set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point). Append the current input character to the temporary buffer. Finally, switch to the RAWTEXT end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
                    HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                    token.tagName = current.ToString().ToLower();
                    _buffer.Append(current);

                    return RAWTEXTEndTagName(token);

                }
                //Lowercase ASCII letter
                else if (CommonIdoms.isLowercaseAscii(current))
                {
                    //Create a new end tag token, and set its tag name to the current input character. Append the current input character to the temporary buffer. Finally, switch to the RAWTEXT end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
                    HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                    token.tagName = current.ToString();
                    _buffer.Append(current);
                    ParseState = enumParseState.RAWTEXT;
                    return RAWTEXTEndTagName(token);
                }
                else
                {
                    //Anything else
                    //Switch to the RAWTEXT state. Emit a U+003C LESS-THAN SIGN character token and a U+002F SOLIDUS character token. Reconsume the current input character.

                    ParseState = enumParseState.RAWTEXT;

                    reConsume();

                    return emitTokenString('\u003C'.ToString() + '\u002F'.ToString());

                }

            }



        }

        /// <summary>
        /// 8.2.4.16 RAWTEXT end tag name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken RAWTEXTEndTagName(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //"tab" (U+0009)  //"LF" (U+000A)  //"FF" (U+000C)//U+0020 SPACE
                if (CommonIdoms.isSpaceCharacters(current))
                // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the before attribute name state. Otherwise, treat it as per the "anything else" entry below.
                    return beforeAttributeValue(token);
                }
                //"/" (U+002F)
                else if (current == '\u002F' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the self-closing start tag state. Otherwise, treat it as per the "anything else" entry below.

                    return selfClosingStartTag(token);
                }
                //">" (U+003E)
                else if (current == '\u003E' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the data state and emit the current tag token. Otherwise, treat it as per the "anything else" entry below.
                    ParseState = enumParseState.DATA;
                    return emitToken(token);

                }
                //Uppercase ASCII letter
                else if (CommonIdoms.isUppercaseAscii(current))
                {
                    //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current tag token's tag name. Append the current input character to the temporary buffer.
                    token.tagName += current.ToString().ToLower();
                    _buffer.Append(current);
                }
                //Lowercase ASCII letter
                else if (CommonIdoms.isLowercaseAscii(current))
                {
                    //Append the current input character to the current tag token's tag name. Append the current input character to the temporary buffer.
                    token.tagName += current.ToString();
                    _buffer.Append(current);
                }
                else
                {
                    //Anything else
                    //Switch to the RAWTEXT state. Emit a U+003C LESS-THAN SIGN character token, a U+002F SOLIDUS character token, and a character token for each of the characters in the temporary buffer (in the order they were added to the buffer). Reconsume the current input character.
                    ParseState = enumParseState.RAWTEXT;

                    reConsume();

                    return emitTokenString('\u003C'.ToString() + '\u002F'.ToString() + _buffer.ToString());

                }
            }
        }

        /// <summary>
        /// 8.2.4.6 Script data state
        /// </summary>
        /// <returns></returns>
        private HtmlToken Script()
        {
            if (ScriptState == enumScriptParseState.initial)
            {
                // Consume the next input character:
                char current = consumeNext();
                int startindex = this._readIndex;

                //"<" (U+003C)
                //Switch to the script data less-than sign state.
                if (current == '\u003C')
                {
                    HtmlToken token = ScriptDataLessThanSign();
                    token.startIndex = startindex;
                    return emitToken(token);
                }
                //U+0000 NULL
                //Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
                else if (current == '\u0000')
                {
                    ParseError("unexpected null character");
                    return emitTokenChar('\uFFFD');
                }
                //EOF
                //Emit an end-of-file token.
                else if (isEOF)
                {
                    return emitTokenEOF();
                }
                else
                {
                    //Anything else
                    //Emit the current input character as a character token.
                    return emitTokenChar(current);
                }
            }
            else
            {
                int startindex = this._readIndex + 1;
                HtmlToken token = ScriptDataSubState();
                token.startIndex = startindex;
                return emitToken(token);
            }
        }

        private HtmlToken ScriptDataSubState()
        {
            switch (ScriptState)
            {

                case enumScriptParseState.dataEscapeStart:
                    return ScriptDataEscapeStart();

                case enumScriptParseState.dataEscapeStartDash:
                    return ScriptDataEscapeStartDash();

                case enumScriptParseState.dataEscapedDashDash:
                    return ScriptDataEscapedDashDash();

                case enumScriptParseState.dataEscaped:
                    return ScriptDataEscaped();

                case enumScriptParseState.dataDoubleEscapeStart:
                    return ScriptDataDoubleEscapeStart();

                case enumScriptParseState.dataEscapedDash:
                    return ScriptDataEscapedDash();

                case enumScriptParseState.dataDoubleEscapedDashDash:
                    return ScriptDataDoubleEscapedDashDash();

                case enumScriptParseState.dataDoubleEscapedLessThanSign:
                    return ScriptDataDoubleEscapedLessThanSign();

                case enumScriptParseState.dataDoubleEscaped:
                    return ScriptDataDoubleEscaped();

                case enumScriptParseState.dataDoubleEscapedDash:
                    return ScriptDataDoubleEscapedDash();

                case enumScriptParseState.dataDoubleEscapeEnd:
                    return ScriptDataDoubleEscapeEnd();

                default:
                    return Script();
            }

        }

        /// <summary>
        /// 8.2.4.17 Script data less-than sign state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataLessThanSign()
        {

            //Consume the next input character:
            char current = consumeNext();

            //"/" (U+002F)
            //Set the temporary buffer to the empty string. Switch to the script data end tag open state.
            if (current == '\u002F')
            {
                _buffer.Clear();
                return ScriptDataEndTagOpen();
            }
            //"!" (U+0021)
            //Switch to the script data escape start state. Emit a U+003C LESS-THAN SIGN character token and a U+0021 EXCLAMATION MARK character token.
            else if (current == '\u0021')
            {
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscapeStart;

                return emitTokenString('\u003C'.ToString() + '\u0021'.ToString());


            }
            else
            {
                //Anything else
                //Switch to the script data state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.initial;

                reConsume();

                return emitTokenChar('\u003C');

            }
        }

        /// <summary>
        ///  8.2.4.18 Script data end tag open state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEndTagOpen()
        {
            //Consume the next input character:
            char current = consumeNext();

            //Uppercase ASCII letter
            //Create a new end tag token, and set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point). Append the current input character to the temporary buffer. Finally, switch to the script data end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
            if (CommonIdoms.isUppercaseAscii(current))
            {
                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                _buffer.Append(current);
                token.tagName = current.ToString().ToLower();
                return ScriptDataEndTagName(token);

            }

            //Lowercase ASCII letter
            //Create a new end tag token, and set its tag name to the current input character. Append the current input character to the temporary buffer. Finally, switch to the script data end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)
            else if (CommonIdoms.isLowercaseAscii(current))
            {

                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                _buffer.Append(current);
                token.tagName = current.ToString();
                return ScriptDataEndTagName(token);

            }
            else
            {
                //Anything else
                //Switch to the script data state. Emit a U+003C LESS-THAN SIGN character token and a U+002F SOLIDUS character token. Reconsume the current input character.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.initial;

                reConsume();
                return emitTokenString('\u003C'.ToString() + '\u002F'.ToString());

            }

        }

        /// <summary>
        ///  8.2.4.20 Script data escape start state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEscapeStart()
        {

            //Consume the next input character:

            char current = consumeNext();

            //"-" (U+002D)
            if (current == '\u002D')
            {
                //Switch to the script data escape start dash state. Emit a U+002D HYPHEN-MINUS character token.

                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscapeStartDash;

                return emitTokenChar('\u002D');

            }

            else
            {

                //Anything else
                //Switch to the script data state. Reconsume the current input character.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.initial;
                reConsume();
                return Script();

            }

        }

        /// <summary>
        /// 8.2.4.19 Script data end tag name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken ScriptDataEndTagName(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //"tab" (U+0009) //"LF" (U+000A)  //"FF" (U+000C) //U+0020 SPACE
                if (CommonIdoms.isSpaceCharacters(current))
                // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the before attribute name state. Otherwise, treat it as per the "anything else" entry below.
                    return beforeAttributeName(token);
                }

                //"/" (U+002F)
                else if (current == '\u002F' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the self-closing start tag state. Otherwise, treat it as per the "anything else" entry below.
                    return selfClosingStartTag(token);
                }

                //">" (U+003E)
                else if (current == '\u003E' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the data state and emit the current tag token. Otherwise, treat it as per the "anything else" entry below.

                    ScriptState = enumScriptParseState.initial;
                    ParseState = enumParseState.DATA;

                    return emitToken(token);
                }

                //Uppercase ASCII letter
                else if (CommonIdoms.isUppercaseAscii(current))
                {
                    //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current tag token's tag name. Append the current input character to the temporary buffer.
                    token.tagName += current.ToString().ToLower();
                    _buffer.Append(current);
                }
                //Lowercase ASCII letter
                else if (CommonIdoms.isLowercaseAscii(current))
                {
                    //Append the current input character to the current tag token's tag name. Append the current input character to the temporary buffer.
                    token.tagName += current.ToString();
                    _buffer.Append(current);

                }
                else
                {
                    //Anything else
                    //Switch to the script data state. Emit a U+003C LESS-THAN SIGN character token, a U+002F SOLIDUS character token, and a character token for each of the characters in the temporary buffer (in the order they were added to the buffer). Reconsume the current input character.

                    ParseState = enumParseState.Script;
                    ScriptState = enumScriptParseState.initial;

                    reConsume();

                    return emitTokenString('\u003C'.ToString() + '\u002F'.ToString() + _buffer.ToString());

                }


            }
        }

        /// <summary>
        /// 8.2.4.21 Script data escape start dash state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEscapeStartDash()
        {
            // Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            if (current == '\u002D')
            {
                //Switch to the script data escaped dash dash state. Emit a U+002D HYPHEN-MINUS character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscapedDashDash;

                return emitTokenChar('\u002D');

            }
            else
            {
                //Anything else
                //Switch to the script data state. Reconsume the current input character.
                reConsume();
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.initial;

                return Script();
            }

        }

        /// <summary>
        /// 8.2.4.24 Script data escaped dash dash state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEscapedDashDash()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            //Emit a U+002D HYPHEN-MINUS character token.
            if (current == '\u002D')
            {
                return emitTokenChar('\u002D');
            }

            //"<" (U+003C)
            //Switch to the script data escaped less-than sign state.
            else if (current == '\u003C')
            {
                return ScriptDataEscapedLessThanSign();
            }

            //">" (U+003E)
            //Switch to the script data state. Emit a U+003E GREATER-THAN SIGN character token.

            else if (current == '\u003E')
            {

                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.initial;

                return emitTokenChar('\u003E');
            }
            //U+0000 NULL
            //Parse error. Switch to the script data escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
            else if (current == '\u0000')
            {
                ParseError("unexpected null character");

                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscaped;

                return emitTokenChar('\uFFFD');
            }

            //EOF
            //Parse error. Switch to the data state. Reconsume the EOF character.
            else if (isEOF)
            {
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                ScriptState = enumScriptParseState.initial;
                reConsume();
                return Data();
            }
            else
            {
                //Anything else
                //Switch to the script data escaped state. Emit the current input character as a character token.

                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscaped;

                return emitTokenChar(current);

            }

        }

        /// <summary>
        /// 8.2.4.25 Script data escaped less-than sign state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEscapedLessThanSign()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"/" (U+002F)
            //Set the temporary buffer to the empty string. Switch to the script data escaped end tag open state.
            if (current == '\u002F')
            {
                _buffer.Clear();
                return ScriptDataEscapedEndTagOpen();
            }

            //Uppercase ASCII letter
            else if (CommonIdoms.isUppercaseAscii(current))
            {
                //Set the temporary buffer to the empty string. Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the temporary buffer. Switch to the script data double escape start state. Emit a U+003C LESS-THAN SIGN character token and the current input character as a character token.
                _buffer.Clear();
                _buffer.Append(current.ToString().ToLower());
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapeStart;

                return emitTokenString('\u003C'.ToString() + current.ToString());

            }
            //Lowercase ASCII letter
            else if (CommonIdoms.isLowercaseAscii(current))
            {

                //Set the temporary buffer to the empty string. Append the current input character to the temporary buffer. Switch to the script data double escape start state. Emit a U+003C LESS-THAN SIGN character token and the current input character as a character token.
                _buffer.Clear();
                _buffer.Append(current.ToString());
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapeStart;

                return emitTokenString('\u003C'.ToString() + current.ToString());
            }
            else
            {
                //Anything else
                //Switch to the script data escaped state. Emit a U+003C LESS-THAN SIGN character token. Reconsume the current input character.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscaped;

                reConsume();

                return emitTokenChar('\u003C');
            }

        }


        /// <summary>
        /// 8.2.4.26 Script data escaped end tag open state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEscapedEndTagOpen()
        {
            //Consume the next input character:

            char current = consumeNext();

            //Uppercase ASCII letter
            if (CommonIdoms.isUppercaseAscii(current))
            {
                //Create a new end tag token, and set its tag name to the lowercase version of the current input character (add 0x0020 to the character's code point). Append the current input character to the temporary buffer. Finally, switch to the script data escaped end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)

                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                token.tagName = current.ToString().ToLower();
                _buffer.Append(current);

                return ScriptDataEscapedEndTagName(token);

            }

            //Lowercase ASCII letter
            else if (CommonIdoms.isLowercaseAscii(current))
            {
                //Create a new end tag token, and set its tag name to the current input character. Append the current input character to the temporary buffer. Finally, switch to the script data escaped end tag name state. (Don't emit the token yet; further details will be filled in before it is emitted.)

                HtmlToken token = createToken(enumHtmlTokenType.EndTag);
                token.tagName = current.ToString();
                _buffer.Append(current);

                return ScriptDataEscapedEndTagName(token);

            }
            else
            {
                //Anything else
                //Switch to the script data escaped state. Emit a U+003C LESS-THAN SIGN character token and a U+002F SOLIDUS character token. Reconsume the current input character.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscaped;

                reConsume();

                return emitTokenString('\u003C'.ToString() + '\u002F'.ToString());

            }
        }


        /// <summary>
        /// 8.2.4.28 Script data double escape start state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataDoubleEscapeStart()
        {

            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)   //"LF" (U+000A)   //"FF" (U+000C)  //U+0020 SPACE   //"/" (U+002F)   //">" (U+003E)
            if (current.isOneOf('\u0009', '\u000A','\u000D', '\u000C', '\u0020', '\u002F', '\u003E'))
            {
                //If the temporary buffer is the string "script", then switch to the script data double escaped state. Otherwise, switch to the script data escaped state. Emit the current input character as a character token.
                if (_buffer.ToString().ToLower() == "script")
                {
                    return ScriptDataDoubleEscaped();
                }
                else
                {
                    ParseState = enumParseState.Script;
                    ScriptState = enumScriptParseState.dataEscaped;

                    return emitTokenChar(current);

                }
            }

            //Uppercase ASCII letter
            else if (CommonIdoms.isUppercaseAscii(current))
            {
                //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the temporary buffer. Emit the current input character as a character token.

                _buffer.Append(current.ToString().ToLower());

                return emitTokenChar(current);

            }
            //Lowercase ASCII letter
            else if (CommonIdoms.isLowercaseAscii(current))
            {
                //Append the current input character to the temporary buffer. Emit the current input character as a character token.
                _buffer.Append(current.ToString().ToLower());

                return emitTokenChar(current);

            }
            else
            {
                //Anything else
                //Switch to the script data escaped state. Reconsume the current input character.
                reConsume();
                return ScriptDataEscaped();
            }
        }

        /// <summary>
        /// 8.2.4.30 Script data double escaped dash state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataDoubleEscapedDash()
        {

            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            if (current == '\u002D')
            {
                //Switch to the script data double escaped dash dash state. Emit a U+002D HYPHEN-MINUS character token.

                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapedDashDash;

                return emitTokenChar('\u002D');

            }
            //"<" (U+003C)
            else if (current == '\u003C')
            {
                //Switch to the script data double escaped less-than sign state. Emit a U+003C LESS-THAN SIGN character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapedLessThanSign;

                return emitTokenChar('\u003C');
            }

            //U+0000 NULL
            else if (current == '\u0000')
            {
                //Parse error. Switch to the script data double escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
                ParseError("unexpected null character");

                return emitTokenChar('\uFFFD');

            }

            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Reconsume the EOF character.
                ParseState = enumParseState.DATA;
                ScriptState = enumScriptParseState.initial;
                reConsume();
                return Data();
            }
            else
            {
                //Anything else
                //Switch to the script data double escaped state. Emit the current input character as a character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscaped;

                return emitTokenChar(current);

            }
        }

        /// <summary>
        /// 8.2.4.29 Script data double escaped state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataDoubleEscaped()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            if (current == '\u002D')
            {
                //Switch to the script data double escaped dash state. Emit a U+002D HYPHEN-MINUS character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapedDash;

                return emitTokenChar('\u002D');

            }
            //"<" (U+003C)
            else if (current == '\u003C')
            {
                //Switch to the script data double escaped less-than sign state. Emit a U+003C LESS-THAN SIGN character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapedLessThanSign;

                return emitTokenChar('\u003C');

            }

            //U+0000 NULL
            else if (current == '\u0000')
            {
                //Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
                ParseError("unexpected null character");

                return emitTokenChar('\uFFFD');

            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Reconsume the EOF character.
                ParseError("unexpected EOF");
                reConsume();
                ScriptState = enumScriptParseState.initial;
                ParseState = enumParseState.DATA;
                return Data();
            }
            else
            {
                //Anything else
                //Emit the current input character as a character token.

                return emitTokenChar(current);
            }
        }

        /// <summary>
        /// 8.2.4.22 Script data escaped state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEscaped()
        {
            //Consume the next input character:

            char current = consumeNext();

            //"-" (U+002D)
            if (current == '\u002D')
            {
                //Switch to the script data escaped dash state. Emit a U+002D HYPHEN-MINUS character token.

                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscapedDash;

                return emitTokenChar('\u002D');

            }
            //"<" (U+003C)
            else if (current == '\u003C')
            {
                //Switch to the script data escaped less-than sign state.
                return ScriptDataEscapedLessThanSign();
            }

            //U+0000 NULL
            else if (current == '\u0000')
            {
                //Parse error. Emit a U+FFFD REPLACEMENT CHARACTER character token.
                ParseError("unexpected null character");

                return emitTokenChar('\uFFFD');

            }
            //EOF
            else if (isEOF)
            {
                //Switch to the data state. Parse error. Reconsume the EOF character.
                ParseState = enumParseState.DATA;
                ScriptState = enumScriptParseState.initial;
                ParseError("unexpected EOF");
                reConsume();
                return Data();
            }
            else
            {
                //Anything else
                //Emit the current input character as a character token.
                return emitTokenChar(current);
            }
        }

        /// <summary>
        /// 8.2.4.27 Script data escaped end tag name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken ScriptDataEscapedEndTagName(HtmlToken token)
        {
            while (true)
            {

                //Consume the next input character:
                char current = consumeNext();

                //"tab" (U+0009)       //"LF" (U+000A)           //"FF" (U+000C)           //U+0020 SPACE
                if (CommonIdoms.isSpaceCharacters(current))
               // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the before attribute name state. Otherwise, treat it as per the "anything else" entry below.
                    return beforeAttributeName(token);
                }
                //"/" (U+002F)
                else if (current == '\u002F' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the self-closing start tag state. Otherwise, treat it as per the "anything else" entry below.
                    return selfClosingStartTag(token);
                }
                //">" (U+003E)
                else if (current == '\u003E' && _treeConstruction.openElements.isAppropriateEndTag(token.tagName))
                {
                    //If the current end tag token is an appropriate end tag token, then switch to the data state and emit the current tag token. Otherwise, treat it as per the "anything else" entry below.
                    ParseState = enumParseState.DATA;
                    ScriptState = enumScriptParseState.initial;
                    return emitToken(token);
                }
                //Uppercase ASCII letter
                else if (CommonIdoms.isUppercaseAscii(current))
                {
                    //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current tag token's tag name. Append the current input character to the temporary buffer.
                    token.tagName += current.ToString().ToLower();
                    _buffer.Append(current);

                }
                //Lowercase ASCII letter
                else if (CommonIdoms.isLowercaseAscii(current))
                {
                    //Append the current input character to the current tag token's tag name. Append the current input character to the temporary buffer.
                    token.tagName += current.ToString();
                    _buffer.Append(current);

                }
                else
                {
                    //Anything else
                    //Switch to the script data escaped state. Emit a U+003C LESS-THAN SIGN character token, a U+002F SOLIDUS character token, and a character token for each of the characters in the temporary buffer (in the order they were added to the buffer). Reconsume the current input character.

                    ParseState = enumParseState.Script;
                    ScriptState = enumScriptParseState.dataEscaped;
                    reConsume();

                    return emitTokenString('\u003C'.ToString() + '\u002F'.ToString() + _buffer.ToString());


                }
            }

        }

        /// <summary>
        /// 8.2.4.23 Script data escaped dash state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataEscapedDash()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            if (current == '\u002D')
            {
                //Switch to the script data escaped dash dash state. Emit a U+002D HYPHEN-MINUS character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscapedDashDash;
                return emitTokenChar('\u002D');
            }
            //"<" (U+003C)
            else if (current == '\u003C')
            {
                //Switch to the script data escaped less-than sign state.
                return ScriptDataEscapedLessThanSign();
            }
            //U+0000 NULL
            else if (current == '\u0000')
            {
                //Parse error. Switch to the script data escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
                ParseError("unexpected null char");
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscaped;

                return emitTokenChar('\uFFFD');
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                ScriptState = enumScriptParseState.initial;
                reConsume();
                return Data();
            }
            else
            {
                //Anything else
                //Switch to the script data escaped state. Emit the current input character as a character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataEscaped;

                return emitTokenChar(current);
            }
        }

        /// <summary>
        /// 8.2.4.31 Script data double escaped dash dash state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataDoubleEscapedDashDash()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"-" (U+002D)
            if (current == '\u002D')
            {
                //Emit a U+002D HYPHEN-MINUS character token.
                return emitTokenChar('\u002D');
            }
            //"<" (U+003C)
            else if (current == '\u003C')
            {
                //Switch to the script data double escaped less-than sign state. Emit a U+003C LESS-THAN SIGN character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapedLessThanSign;
                return emitTokenChar('\u003C');
            }
            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Switch to the script data state. Emit a U+003E GREATER-THAN SIGN character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.initial;
                return emitTokenChar('\u003E');
            }
            //U+0000 NULL
            else if (current == '\u0000')
            {
                //Parse error. Switch to the script data double escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
                ParseError("unexpected null char");
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscaped;

                return emitTokenChar('\uFFFD');

            }

            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Reconsume the EOF character.
                ParseState = enumParseState.DATA;
                ScriptState = enumScriptParseState.initial;
                reConsume();
                return Data();
            }
            else
            {
                //Anything else
                //Switch to the script data double escaped state. Emit the current input character as a character token.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscaped;
                return emitTokenChar(current);
            }
        }

        /// <summary>
        /// 8.2.4.32 Script data double escaped less-than sign state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataDoubleEscapedLessThanSign()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"/" (U+002F)
            if (current == '\u002F')
            {
                //Set the temporary buffer to the empty string. Switch to the script data double escape end state. Emit a U+002F SOLIDUS character token.
                _buffer.Clear();
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscapeEnd;

                return emitTokenChar('\u002F');

            }
            else
            {
                //Anything else
                //Switch to the script data double escaped state. Reconsume the current input character.
                reConsume();
                return ScriptDataDoubleEscaped();

            }

        }

        /// <summary>
        /// 8.2.4.33 Script data double escape end state
        /// </summary>
        /// <returns></returns>
        private HtmlToken ScriptDataDoubleEscapeEnd()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)    //"LF" (U+000A)   //"FF" (U+000C)    //U+0020 SPACE    //"/" (U+002F)    //">" (U+003E)
            if (current.isOneOf('\u0009', '\u000A','\u000D', '\u000C', '\u0020', '\u002F', '\u003E'))
            {
                //If the temporary buffer is the string "script", then switch to the script data escaped state. Otherwise, switch to the script data double escaped state. Emit the current input character as a character token.
                if (_buffer.ToString().ToLower() == "script")
                {
                    return ScriptDataEscaped();
                }
                else
                {
                    ParseState = enumParseState.Script;
                    ScriptState = enumScriptParseState.dataDoubleEscaped;
                    return emitTokenChar(current);
                }
            }

            //Uppercase ASCII letter
            else if (CommonIdoms.isUppercaseAscii(current))
            {
                //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the temporary buffer. Emit the current input character as a character token.
                _buffer.Append(current.ToString().ToLower());
                return emitTokenChar(current);
            }

            //Lowercase ASCII letter
            else if (CommonIdoms.isLowercaseAscii(current))
            {
                //Append the current input character to the temporary buffer. Emit the current input character as a character token.
                _buffer.Append(current.ToString());
                return emitTokenChar(current);
            }
            else
            {
                //Anything else
                //Switch to the script data double escaped state. Reconsume the current input character.
                ParseState = enumParseState.Script;
                ScriptState = enumScriptParseState.dataDoubleEscaped;
                reConsume();
                return ScriptDataDoubleEscaped();
            }

        }

        /// <summary>
        /// 8.2.4.68 CDATA section state
        /// </summary>
        /// <returns></returns>
        private HtmlToken CDATA()
        {
            //Switch to the data state.

            ParseState = enumParseState.DATA;

            //Consume every character up to the next occurrence of the three character sequence U+005D RIGHT SQUARE BRACKET U+005D RIGHT SQUARE BRACKET U+003E GREATER-THAN SIGN (]]>), or the end of the file (EOF), whichever comes first. Emit a series of character tokens consisting of all the characters consumed except the matching three character sequence at the end (if one was found before the end of the file).

            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char current = consumeNext();
                /// three character sequence U+005D RIGHT SQUARE BRACKET U+005D RIGHT SQUARE BRACKET U+003E GREATER-THAN SIGN (]]>)

                if (current == '\u005D')
                {
                    if (LookupChar(1) == '\u005D' && LookupChar(2) == '\u003E')
                    {
                        moveAhead(2);
                        return emitTokenString(sb.ToString());

                    }
                    else
                    {
                        sb.Append(current);
                    }
                }
                else if (isEOF)
                {
                    reConsume();
                    return emitTokenString(sb.ToString());
                }
                else
                {
                    sb.Append(current);
                }

            }

            //If the end of the file was reached, reconsume the EOF character.
        }


        /// <summary>
        /// 8.2.4.52 DOCTYPE state
        /// </summary>
        /// <returns></returns>
        private HtmlToken DOCTYPE()
        {
            //Consume the next input character:
            char current = consumeNext();


            //"tab" (U+0009)        //"LF" (U+000A)       //"FF" (U+000C)       //U+0020 SPACE
            if (CommonIdoms.isSpaceCharacters(current))
            // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                return beforeDOCTYPEName();
                //Switch to the before DOCTYPE name state.
            }

            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Create a new DOCTYPE token. Set its force-quirks flag to on. Emit the token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                HtmlToken token = createToken(enumHtmlTokenType.DocType);
                token.forceQuirks = true;

                reConsume();

                return token;

            }
            else
            {
                //Anything else
                //Parse error. Switch to the before DOCTYPE name state. Reconsume the character.
                ParseError("unexpected char");
                reConsume();
                return beforeDOCTYPEName();
            }
        }


        /// <summary>
        /// 8.2.4.53 Before DOCTYPE name state
        /// </summary>
        /// <returns></returns>
        private HtmlToken beforeDOCTYPEName()
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)      //"LF" (U+000A)      //"FF" (U+000C)     //U+0020 SPACE
            if (CommonIdoms.isSpaceCharacters(current))
         // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                return beforeDOCTYPEName();
                //Ignore the character.
            }

            //Uppercase ASCII letter
            else if (CommonIdoms.isUppercaseAscii(current))
            {
                //Create a new DOCTYPE token. Set the token's name to the lowercase version of the current input character (add 0x0020 to the character's code point). Switch to the DOCTYPE name state.
                HtmlToken token = createToken(enumHtmlTokenType.DocType);
                token.name = current.ToString().ToLower();
                return DOCTYPEName(token);

            }
            //U+0000 NULL
            else if (current == '\u0000')
            {
                //Parse error. Create a new DOCTYPE token. Set the token's name to a U+FFFD REPLACEMENT CHARACTER character. Switch to the DOCTYPE name state.
                ParseError("unexpected null char");
                HtmlToken token = createToken(enumHtmlTokenType.DocType);
                token.name = '\uFFFD'.ToString();
                return DOCTYPEName(token);
            }

            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Parse error. Create a new DOCTYPE token. Set its force-quirks flag to on. Switch to the data state. Emit the token.
                ParseError("unexpected char");
                HtmlToken token = createToken(enumHtmlTokenType.DocType);
                token.forceQuirks = true;
                ParseState = enumParseState.DATA;
                return token;
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Create a new DOCTYPE token. Set its force-quirks flag to on. Emit the token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                HtmlToken token = createToken(enumHtmlTokenType.DocType);
                token.forceQuirks = true;

                return token;

            }
            else
            {
                //Anything else
                //Create a new DOCTYPE token. Set the token's name to the current input character. Switch to the DOCTYPE name state.
                HtmlToken token = createToken(enumHtmlTokenType.DocType);
                token.name = current.ToString();
                return DOCTYPEName(token);
            }

        }


        /// <summary>
        /// 8.2.4.54 DOCTYPE name state
        /// </summary>
        /// <returns></returns>
        private HtmlToken DOCTYPEName(HtmlToken token)
        {

            //Consume the next input character:
            while (true)
            {
                char current = consumeNext();


                //"tab" (U+0009)      //"LF" (U+000A)      //"FF" (U+000C)      //U+0020 SPACE
                if (CommonIdoms.isSpaceCharacters(current))
              //if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
                {
                    return afterDOCTYPEName(token);
                    //Switch to the after DOCTYPE name state.
                }
                //">" (U+003E)
                else if (current == '\u003E')
                {
                    //Switch to the data state. Emit the current DOCTYPE token.
                    ParseState = enumParseState.DATA;

                    return emitToken(token);

                }
                //Uppercase ASCII letter
                else if (CommonIdoms.isUppercaseAscii(current))
                {
                    //Append the lowercase version of the current input character (add 0x0020 to the character's code point) to the current DOCTYPE token's name.
                    token.name += current.ToString().ToLower();

                }
                //U+0000 NULL
                else if (current == '\u0000')
                {
                    ParseError("unexpected null char");
                    token.name += '\uFFFD'.ToString();
                    //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's name.
                }
                //EOF
                else if (isEOF)
                {
                    //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                    ParseError("unexpected EOF");
                    ParseState = enumParseState.DATA;
                    token.forceQuirks = true;
                    return token;
                }
                else
                {
                    //Anything else
                    //Append the current input character to the current DOCTYPE token's name.
                    token.name += current.ToString();

                }
            }
        }

        /// <summary>
        /// 8.2.4.55 After DOCTYPE name state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken afterDOCTYPEName(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)     //"LF" (U+000A)    //"FF" (U+000C)   //U+0020 SPACE
            if (CommonIdoms.isSpaceCharacters(current))
          // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                //Ignore the character.
                return afterDOCTYPEName(token);
            }

            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Switch to the data state. Emit the current DOCTYPE token.
                ParseState = enumParseState.DATA;
                return token;
            }

            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                token.forceQuirks = true;
                return token;
            }
            //Anything else
            else
            {
                string nextsix = current.ToString() + LookupChar(1).ToString() + LookupChar(2).ToString() + LookupChar(3).ToString() + LookupChar(4).ToString() + LookupChar(5).ToString();
                nextsix = nextsix.ToUpper();

                //If the six characters starting from the current input character are an ASCII case-insensitive match for the word "PUBLIC", then consume those characters and switch to the after DOCTYPE public keyword state.
                if (nextsix == "PUBLIC")
                {
                    moveAhead(5);
                    return afterDOCTYPEPublicKeyword(token);
                }
                else if (nextsix == "SYSTEM")
                {
                    //Otherwise, if the six characters starting from the current input character are an ASCII case-insensitive match for the word "SYSTEM", then consume those characters and switch to the after DOCTYPE system keyword state.
                    moveAhead(5);
                    return afterDOCTYPESystemKeyword(token);
                }
                else
                {
                    //Otherwise, this is a parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
                    ParseError("unexpected char in Doctype");
                    token.forceQuirks = true;
                    return bogusDOCTYPE(token);
                }
            }
        }

        /// <summary>
        /// 8.2.4.56 After DOCTYPE public keyword state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken afterDOCTYPEPublicKeyword(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)     //"LF" (U+000A)     //"FF" (U+000C)   //U+0020 SPACE
            if (CommonIdoms.isSpaceCharacters(current))
          // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                //Switch to the before DOCTYPE public identifier state.
                return beforeDOCTYPEPublicIdentifier(token);
            }

            //U+0022 QUOTATION MARK (")
            else if (current == '\u0022')
            {

                //Parse error. Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (double-quoted) state.
                ParseError("unexpected quotation mark");
                token.publicId = string.Empty;
                return DOCTYPEPublicIdentifierDoubleQuoted(token);
            }

            //"'" (U+0027)
            else if (current == '\u0027')
            {
                //Parse error. Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (single-quoted) state.
                ParseError("unexpected ' ");
                token.publicId = string.Empty;
                return DOCTYPEPublicIdentifierSingleQuoted(token);
            }

            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                ParseError("unexpected ends");
                token.forceQuirks = true;
                ParseState = enumParseState.DATA;
                return token;
            }
            else if (isEOF)
            {
                //EOF
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseState = enumParseState.DATA;
                token.forceQuirks = true;
                reConsume();
                return token;
            }
            else
            {
                //Anything else
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
                ParseError("unexpected chars");
                token.forceQuirks = true;
                return bogusDOCTYPE(token);
            }

        }

        /// <summary>
        /// 8.2.4.62 After DOCTYPE system keyword state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken afterDOCTYPESystemKeyword(HtmlToken token)
        {

            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)    //"LF" (U+000A)       //"FF" (U+000C)       //U+0020 SPACE
            if (CommonIdoms.isSpaceCharacters(current))
         // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                //Switch to the before DOCTYPE system identifier state.
                return beforeDOCTYPESystemIdentifier(token);
            }
            //U+0022 QUOTATION MARK (")
            else if (current == '\u0022')
            {
                //Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.
                ParseError("unexpected quotation mark");
                token.systemId = string.Empty;
                return DOCTYPESystemIdentifierDoubleQuoted(token);
            }

            //"'" (U+0027)
            else if (current == '\u0027')
            {
                //Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
                ParseError("unexpected '");
                token.systemId = string.Empty;
                return DOCTYPESystemIdentifierSingleQuoted(token);

            }
            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                ParseError("unexpected >");
                token.forceQuirks = true;
                ParseState = enumParseState.DATA;
                return token;
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                token.forceQuirks = true;
                reConsume();
                return token;
            }
            else
            {
                //Anything else
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
                ParseError("unexpected char");
                token.forceQuirks = true;
                return bogusDOCTYPE(token);
            }
        }

        /// <summary>
        ///  8.2.4.67 Bogus DOCTYPE state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken bogusDOCTYPE(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //">" (U+003E)
            //Switch to the data state. Emit the DOCTYPE token.
            if (current == '\u003E')
            {
                ParseState = enumParseState.DATA;
                return token;
            }
            else if (isEOF)
            {
                //EOF
                //Switch to the data state. Emit the DOCTYPE token. Reconsume the EOF character.
                ParseState = enumParseState.DATA;
                reConsume();
                return token;
            }
            else
            {
                //Anything else
                //Ignore the character
                return bogusDOCTYPE(token);
            }

        }

        /// <summary>
        /// 8.2.4.57 Before DOCTYPE public identifier state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken beforeDOCTYPEPublicIdentifier(HtmlToken token)
        {

            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)      //"LF" (U+000A)           //"FF" (U+000C)           //U+0020 SPACE
            if (CommonIdoms.isSpaceCharacters(current))
           //if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                //Ignore the character.
                return beforeDOCTYPEPublicIdentifier(token);
            }

            //U+0022 QUOTATION MARK (")
            else if (current == '\u0022')
            {
                //Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (double-quoted) state.
                token.publicId = string.Empty;
                return DOCTYPEPublicIdentifierDoubleQuoted(token);
            }

            //"'" (U+0027)
            else if (current == '\u0027')
            {
                //Set the DOCTYPE token's public identifier to the empty string (not missing), then switch to the DOCTYPE public identifier (single-quoted) state.
                token.publicId = string.Empty;
                return DOCTYPEPublicIdentifierSingleQuoted(token);
            }
            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                ParseError("unexpected char");
                token.forceQuirks = true;
                ParseState = enumParseState.DATA;
                return token;
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                reConsume();
                token.forceQuirks = true;
                return token;
            }
            else
            {
                //Anything else
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
                ParseError("unexpected chars");
                token.forceQuirks = true;
                return bogusDOCTYPE(token);
            }
        }


        /// <summary>
        /// 8.2.4.58 DOCTYPE public identifier (double-quoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken DOCTYPEPublicIdentifierDoubleQuoted(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //U+0022 QUOTATION MARK (")
                if (current == '\u0022')
                {
                    //Switch to the after DOCTYPE public identifier state.
                    return afterDOCTYPEPublicIdentifier(token);
                }
                //U+0000 NULL
                else if (current == '\u0000')
                {
                    //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's public identifier.
                    ParseError("unexpected null char");
                    token.publicId += '\uFFFD'.ToString();
                }
                //">" (U+003E)
                else if (current == '\u003E')
                {
                    //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                    ParseError("unexpected end");
                    token.forceQuirks = true;
                    ParseState = enumParseState.DATA;
                    return token;
                }
                //EOF
                else if (isEOF)
                {
                    //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                    ParseError("unexpected EOF");
                    ParseState = enumParseState.DATA;
                    token.forceQuirks = true;
                    reConsume();
                    return token;
                }
                else
                {
                    //Anything else
                    //Append the current input character to the current DOCTYPE token's public identifier.
                    token.publicId += current.ToString();
                }
            }
        }


        /// <summary>
        /// 8.2.4.59 DOCTYPE public identifier (single-quoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken DOCTYPEPublicIdentifierSingleQuoted(HtmlToken token)
        {
            while (true)
            {
                //Consume the next input character:
                char current = consumeNext();

                //"'" (U+0027)
                if (current == '\u0027')
                {
                    //Switch to the after DOCTYPE public identifier state.
                    return afterDOCTYPEPublicIdentifier(token);
                }
                //U+0000 NULL
                else if (current == '\u0000')
                {
                    //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's public identifier.
                    ParseError("unexpected null char");
                    token.publicId += '\uFFFD'.ToString();

                }
                //">" (U+003E)
                else if (current == '\u003E')
                {
                    //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                    ParseError("unexpected >");
                    token.forceQuirks = true;
                    ParseState = enumParseState.DATA;
                    return token;
                }

                //EOF
                else if (isEOF)
                {
                    //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                    ParseError("unexpected EOF");
                    ParseState = enumParseState.DATA;
                    token.forceQuirks = true;
                    return token;
                }
                else
                {
                    //Anything else
                    //Append the current input character to the current DOCTYPE token's public identifier.
                    token.publicId += current.ToString();
                }
            }

        }


        /// <summary>
        /// 8.2.4.60 After DOCTYPE public identifier state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken afterDOCTYPEPublicIdentifier(HtmlToken token)
        {
            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)      //"LF" (U+000A)       //"FF" (U+000C)         //U+0020 SPACE
            if (CommonIdoms.isSpaceCharacters(current))
          // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                //Switch to the between DOCTYPE public and system identifiers state.
                return betweenDOCTYPEPublicAndSystemIdentifier(token);

            }
            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Switch to the data state. Emit the current DOCTYPE token.
                ParseState = enumParseState.DATA;
                return token;
            }
            //U+0022 QUOTATION MARK (")
            else if (current == '\u0022')
            {
                //Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.
                ParseError("unexpected quotation mark");
                token.publicId = string.Empty;
                return DOCTYPESystemIdentifierDoubleQuoted(token);
            }
            //"'" (U+0027)
            else if (current == '\u0027')
            {
                //Parse error. Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
                ParseError("unexpected single quote");
                token.publicId = string.Empty;
                return DOCTYPESystemIdentifierSingleQuoted(token);
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseError("unexpeced EOF");
                ParseState = enumParseState.DATA;
                token.forceQuirks = true;
                reConsume();
                return token;
            }
            else
            {
                //Anything else
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
                ParseError("unknown chars");
                token.forceQuirks = true;
                return bogusDOCTYPE(token);
            }
        }

        /// <summary>
        /// 8.2.4.61 Between DOCTYPE public and system identifiers state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken betweenDOCTYPEPublicAndSystemIdentifier(HtmlToken token)
        {

            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)    //"LF" (U+000A)        //"FF" (U+000C)     //U+0020 SPACE
            //Ignore the character.
            if (CommonIdoms.isSpaceCharacters(current))
           //if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                return betweenDOCTYPEPublicAndSystemIdentifier(token);
            }

            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Switch to the data state. Emit the current DOCTYPE token.
                ParseState = enumParseState.DATA;
                return token;

            }
            //U+0022 QUOTATION MARK (")
            else if (current == '\u0022')
            {
                //Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.
                token.systemId = string.Empty;
                return DOCTYPESystemIdentifierDoubleQuoted(token);

            }
            //"'" (U+0027)
            else if (current == '\u0027')
            {
                //Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
                token.systemId = string.Empty;
                return DOCTYPESystemIdentifierSingleQuoted(token);
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                token.forceQuirks = true;
                reConsume();
                return token;
            }
            else
            {
                //Anything else
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
                ParseError("unknown char");
                token.forceQuirks = true;
                return bogusDOCTYPE(token);
            }
        }


        /// <summary>
        /// 8.2.4.64 DOCTYPE system identifier (double-quoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken DOCTYPESystemIdentifierDoubleQuoted(HtmlToken token)
        {
            while (true)
            {

                //Consume the next input character:
                char current = consumeNext();

                //U+0022 QUOTATION MARK (")
                if (current == '\u0022')
                {
                    //Switch to the after DOCTYPE system identifier state.
                    return afterDOCTYPESystemIdentifier(token);
                }
                //U+0000 NULL
                else if (current == '\u0000')
                {
                    //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's system identifier.
                }
                //">" (U+003E)
                else if (current == '\u003E')
                {
                    //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                    ParseError("unexpected end mark >");
                    token.forceQuirks = true;
                    ParseState = enumParseState.DATA;
                    return token;
                }
                //EOF
                else if (isEOF)
                {
                    //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                    ParseError("unexpected EOF");
                    ParseState = enumParseState.DATA;
                    token.forceQuirks = true;
                    reConsume();
                    return token;
                }
                else
                {
                    //Anything else
                    //Append the current input character to the current DOCTYPE token's system identifier.
                    token.systemId += current.ToString();
                }
            }
        }


        /// <summary>
        /// 8.2.4.65 DOCTYPE system identifier (single-quoted) state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken DOCTYPESystemIdentifierSingleQuoted(HtmlToken token)
        {
            while (true)
            {

                //Consume the next input character:
                char current = consumeNext();

                //"'" (U+0027)
                if (current == '\u0027')
                {
                    //Switch to the after DOCTYPE system identifier state.
                    return afterDOCTYPESystemIdentifier(token);
                }
                //U+0000 NULL
                if (current == '\u0000')
                {
                    //Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's system identifier.
                    ParseError("unexpected null char");
                    token.systemId += '\uFFFD'.ToString();
                }
                //">" (U+003E)
                if (current == '\u003E')
                {
                    //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                    ParseError("unexpected > end mark");
                    token.forceQuirks = true;
                    ParseState = enumParseState.DATA;
                    return token;
                }
                //EOF
                else if (isEOF)
                {
                    //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                    ParseError("unexpected EOF");
                    ParseState = enumParseState.DATA;
                    token.forceQuirks = true;
                    reConsume();
                    return token;
                }
                else
                {
                    //Anything else
                    //Append the current input character to the current DOCTYPE token's system identifier.
                    token.systemId += current.ToString();

                }
            }

        }


        /// <summary>
        /// 8.2.4.63 Before DOCTYPE system identifier state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken beforeDOCTYPESystemIdentifier(HtmlToken token)
        {

            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)     //"LF" (U+000A)      //"FF" (U+000C)      //U+0020 SPACE
            //Ignore the character.
            if (CommonIdoms.isSpaceCharacters(current))
           //if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                return beforeDOCTYPESystemIdentifier(token);
            }


            //U+0022 QUOTATION MARK (")
            else if (current == '\u0022')
            {
                //Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (double-quoted) state.

                token.systemId = string.Empty;
                return DOCTYPESystemIdentifierDoubleQuoted(token);

            }
            //"'" (U+0027)
            else if (current == '\u0027')
            {
                //Set the DOCTYPE token's system identifier to the empty string (not missing), then switch to the DOCTYPE system identifier (single-quoted) state.
                token.systemId = string.Empty;
                return DOCTYPESystemIdentifierSingleQuoted(token);

            }
            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
                ParseError("unexected > end mark");
                token.forceQuirks = true;
                ParseState = enumParseState.DATA;
                return token;
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                token.forceQuirks = true;
                reConsume();
                return token;
            }
            else
            {
                //Anything else
                //Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the bogus DOCTYPE state.
                ParseError("unknown chars");
                token.forceQuirks = true;
                return bogusDOCTYPE(token);
            }

        }

        /// <summary>
        /// 8.2.4.66 After DOCTYPE system identifier state
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private HtmlToken afterDOCTYPESystemIdentifier(HtmlToken token)
        {

            //Consume the next input character:
            char current = consumeNext();

            //"tab" (U+0009)       //"LF" (U+000A)           //"FF" (U+000C)           //U+0020 SPACE
            //Ignore the character.
            if (CommonIdoms.isSpaceCharacters(current))
           // if (current.isOneOf('\u0009', '\u000A', '\u000C', '\u0020'))
            {
                return afterDOCTYPESystemIdentifier(token);
            }

            //">" (U+003E)
            else if (current == '\u003E')
            {
                //Switch to the data state. Emit the current DOCTYPE token.
                ParseState = enumParseState.DATA;
                return token;
            }
            //EOF
            else if (isEOF)
            {
                //Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
                ParseError("unexpected EOF");
                ParseState = enumParseState.DATA;
                token.forceQuirks = true;
                reConsume();
                return token;
            }
            else
            {
                //Anything else
                //Parse error. Switch to the bogus DOCTYPE state. (This does not set the DOCTYPE token's force-quirks flag to on.)
                ParseError("unknown char");
                return bogusDOCTYPE(token);
            }
        }

    }

}
