//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{
    /// <summary>
    ///  4.2. Definitions   This section defines several terms used during the tokenization phase.This section defines several terms used during the tokenization phase.
    /// </summary>
    public static class Definition
    {


        //code point
        //A Unicode code point. [UNICODE] Any value in the Unicode codespace; that is, the range of integers from 0 to (hexadecimal) 10FFFF.

        /// <summary>
        /// A Unicode code point. [UNICODE] Any value in the Unicode codespace; that is, the range of integers from 0 to (hexadecimal) 10FFFF.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool iscodepoint(char input)
        {
            //TODO: need verification of the upper. 
            if (input > '\u0000' && input < '\uFFFF')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isCodePointer(this char input)
        {
            return iscodepoint(input);
        }

        //next input code point
        //The first code point in the input stream that has not yet been consumed.
        //current input code point
        //The last code point to have been consumed.

        //reconsume the current input code point
        //Push the current input code point back onto the front of the input stream, so that the next time you are instructed to consume the next input code point, it will instead reconsume the current input code point.
        //EOF code point
        //A conceptual code point representing the end of the input stream. Whenever the input stream is empty, the next input code point is always an EOF code point.


        //digit
        public static bool isdigit(char input)
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

        public static bool isDigit(this char input)
        {
            return isdigit(input);
        }

        //hex digit
        //A digit, or a code point between U+0041 LATIN CAPITAL LETTER A (A) and U+0046 LATIN CAPITAL LETTER F (F), or a code point between U+0061 LATIN SMALL LETTER A (a) and U+0066 LATIN SMALL LETTER F (f).

        public static bool ishexdigit(char input)
        {
            if (isdigit(input) || (input >= '\u0041' && input <= '\u0046') || (input >= '\u0061' && input <= '\u0066'))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isHexDigit(this char input)
        {
            return ishexdigit(input);
        }

        //uppercase letter
        //A code point between U+0041 LATIN CAPITAL LETTER A (A) and U+005A LATIN CAPITAL LETTER Z (Z).
        //lowercase letter
        //A code point between U+0061 LATIN SMALL LETTER A (a) and U+007A LATIN SMALL LETTER Z (z).
        //letter
        //An uppercase letter or a lowercase letter.

        //non-ASCII code point
        //A code point with a value equal to or greater than U+0080 <control>.
        public static bool nonAscii(char input)
        {
            return (input > '\u0080');
        }

        public static bool isNonAscii(this char input)
        {
            return nonAscii(input);
        }

        //name-start code point
        //A letter, a non-ASCII code point, or U+005F LOW LINE (_).

        public static bool namestartcodepoint(char input)
        {
            return (char.IsLetter(input) || nonAscii(input) || input == '\u005F');

        }

        public static bool isNameStartCodePoint(this char input)
        {
            return namestartcodepoint(input);
        }

        //name code point
        //A name-start code point, A digit, or U+002D HYPHEN-MINUS (-).

        public static bool namecodepoint(char input)
        {
            return (namestartcodepoint(input) || char.IsDigit(input) || input == '\u002D');
        }

        public static bool isNameCodePoint(this char input)
        {
            return namecodepoint(input);
        }

        //non-printable code point
        //A code point between U+0000 NULL and U+0008 BACKSPACE, or U+000B LINE TABULATION, or a code point between U+000E SHIFT OUT and U+001F INFORMATION SEPARATOR ONE, or U+007F DELETE.

        public static bool nonprintablecodepoint(char input)
        {
            return ((input >= '\u0000' && input <= '\u0008') || input == '\u000B' || (input >= '\u000E' && input <= '\u001F') || input == '\u007F');

        }

        public static bool isNonPrintableCodePoint(this char input)
        {
            return nonprintablecodepoint(input);
        }


        //newline
        //U+000A LINE FEED. Note that U+000D CARRIAGE RETURN and U+000C FORM FEED are not included in this definition, as they are converted to U+000A LINE FEED during preprocessing.

        public static bool NewLine(char input)
        {
            if (input == '\u000A')
            {
                return true;
            }

            return false;
        }

        public static bool isNewLine(this char input)
        {
            if (input == '\u000A')
            {
                return true;
            }

            return false;
        }

        //whitespace
        //A newline, U+0009 CHARACTER TABULATION, or U+0020 SPACE.

        public static bool WhiteSpace(char input)
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

        public static bool isWhiteSpace(this char input)
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

        public static bool isEOF(this char input)
        {
            if (input == (Char)0x1a)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //surrogate code point
        //A code point between U+D800 and U+DFFF inclusive.
        //maximum allowed code point
        //The greatest code point defined by Unicode: U+10FFFF.
        //identifier
        //A portion of the CSS source that has the same syntax as an <ident-token>. Also appears in <at-keyword-token>, <function-token>, <hash-token> with the "id" type flag, and the unit of <dimension-token>.

    }
}
