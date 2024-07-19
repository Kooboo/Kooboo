//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom.CSS
{
    public static class TokenizerHelper
    {

        /// <summary>
        /// 4.3.8. Check if two code points are a valid escape. the two code points in question are the current input code point and the next input code point, in that order.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static bool isValidEscape(char one, char two)
        {
            //This section describes how to check if two code points are a valid escape. The algorithm described here can be called explicitly with two code points, or can be called with the input stream itself. In the latter case, the two code points in question are the current input code point and the next input code point, in that order.

            //Note: This algorithm will not consume any additional code point.

            //If the first code point is not U+005C REVERSE SOLIDUS (\), return false.

            if (one != '\u005C')
            {
                return false;
            }
            else if (two.isNewLine())
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

        /// <summary>
        /// 4.3.9. Check if three code points would start an identifier. the three code points in question are the current input code point and the next two input code points, in that order.
        /// </summary>
        /// <returns></returns>
        public static bool isStartIdentifier(char one, char two, char three)
        {
            //This section describes how to check if three code points would start an identifier. The algorithm described here can be called explicitly with three code points, or can be called with the input stream itself. In the latter case, the three code points in question are the current input code point and the next two input code points, in that order.

            //Note: This algorithm will not consume any additional code points.

            //Look at the first code point:

            //U+002D HYPHEN-MINUS
            if (one == '\u002D')
            {
                //If the second code point is a name-start code point or the second and third code points are a valid escape, return true. Otherwise, return false.
                return (two.isNameStartCodePoint() || isValidEscape(two, three));

            }

            //name-start code point
            else if (one.isNameStartCodePoint())
            {
                //Return true.
                return true;
            }

            //U+005C REVERSE SOLIDUS (\)
            else if (one == '\u005C')
            {
                //If the first and second code points are a valid escape, return true. Otherwise, return false.
                return isValidEscape(one, two);
            }

            return false;

        }

        /// <summary>
        /// 4.3.10. Check if three code points would start a number. the three code points in question are the current input code point and the next two input code points, in that order.
        /// </summary>
        /// <returns></returns>
        public static bool isStartWithNumber(char one, char two, char three)
        {
            //This section describes how to check if three code points would start a number. The algorithm described here can be called explicitly with three code points, or can be called with the input stream itself. In the latter case, the three code points in question are the current input code point and the next two input code points, in that order.

            //Note: This algorithm will not consume any additional code points.

            // Look at the first code point:

            //U+002B PLUS SIGN (+)
            //U+002D HYPHEN-MINUS (-)
            if (one == '\u002B' || one == '\u002D')
            {
                //If the second code point is a digit, return true.
                if (two.isDigit())
                {
                    return true;
                }
                else
                {

                    //Otherwise, if the second code point is a U+002E FULL STOP (.) and the third code point is a digit, return true.
                    if (two == '\u002E' && three.isDigit())
                    {
                        return true;
                    }
                    else
                    {
                        //Otherwise, return false.
                        return false;
                    }
                }
            }

            //U+002E FULL STOP (.)
            else if (one == '\u002E')
            {
                //If the second code point is a digit, return true. Otherwise, return false.
                return two.isDigit();
            }

            //digit
            else if (one.isDigit())
            {     //Return true.
                return true;
            }
            else
            {
                //anything else
                //Return false.
                return false;
            }

        }


        /// <summary>
        ///  4.3.13. Convert a string to a number. This section describes how to convert a string to a number. It returns a number.Ensure that the string contains only a valid CSS number before calling this algorithm.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double ConvertStringToNumber(string input)
        {
            int s = 1;   // default is 1
            Int64 i = 0;    // default 0
            Int64 f = 0;    // default 0
            int d = 0;
            int t = 1;
            int e = 0;    // default 0


            int index = 0;
            int len = input.Length;

            if (index >= len)
            {
                goto processdigit;
            }

            //Note: This algorithm does not do any verification to ensure that the string contains only a number. Ensure that the string contains only a valid CSS number before calling this algorithm.

            //Divide the string into seven components, in order from left to right:

            //A sign: a single U+002B PLUS SIGN (+) or U+002D HYPHEN-MINUS (-), or the empty string. Let s be the number -1 if the sign is U+002D HYPHEN-MINUS (-); otherwise, let s be the number 1.

            if (input[index] == '\u002B' || input[index] == '\u002D')
            {
                if (input[index] == '\u002D')
                {
                    s = -1;
                }

                index += 1;
            }

            if (index >= len)
            {
                goto processdigit;
            }


            //An integer part: zero or more digits. If there is at least one digit, let i be the number formed by interpreting the digits as a base-10 integer; otherwise, let i be the number 0.

            string intpart = string.Empty;

            while (input[index].isDigit())
            {
                intpart += input[index].ToString();
                index += 1;

                if (index >= len)
                {

                    if (intpart.Length > 0)
                    {
                        if (intpart.Length > 10) intpart = intpart[..10];
                        i = Convert.ToInt64(intpart, 10);
                    }

                    goto processdigit;
                }
            }

            if (intpart.Length > 0)
            {
                i = Convert.ToInt64(intpart, 10);
            }


            //A decimal point: a single U+002E FULL STOP (.), or the empty string.

            string decimalPoint = string.Empty;
            if (input[index] == '\u002E')
            {
                decimalPoint = '\u002E'.ToString();
                index += 1;
                if (index >= len)
                {
                    goto processdigit;
                }
            }

            //A fractional part: zero or more digits. If there is at least one digit, let f be the number formed by interpreting the digits as a base-10 integer and d be the number of digits; otherwise, let f and d be the number 0.

            string fintpart = string.Empty;

            while (input[index].isDigit())
            {
                fintpart += input[index].ToString();

                index += 1;
                if (index >= len)
                {

                    d = fintpart.Length;

                    if (d > 0)
                    {
                        if (d > 18)
                        {
                            fintpart = fintpart[..18];
                        }
                        f = Convert.ToInt64(fintpart, 10);
                    }
                    goto processdigit;
                }
            }
            d = fintpart.Length;

            if (d > 0)
            {
                if (d > 18)
                {
                    fintpart = fintpart[..18];
                }
                f = Convert.ToInt64(fintpart, 10);
            }


            //An exponent indicator: a single U+0045 LATIN CAPITAL LETTER E (E) or U+0065 LATIN SMALL LETTER E (e), or the empty string.
            string ExponentIndicator = string.Empty;

            if (input[index] == '\u0045' || input[index] == '\u0065')
            {
                ExponentIndicator = '\u0045'.ToString();
                index += 1;
                if (index >= len)
                {
                    goto processdigit;
                }
            }

            //An exponent sign: a single U+002B PLUS SIGN (+) or U+002D HYPHEN-MINUS (-), or the empty string. Let t be the number -1 if the sign is U+002D HYPHEN-MINUS (-); otherwise, let t be the number 1.

            if (input[index] == '\u002B' || input[index] == '\u002D')
            {
                if (input[index] == '\u002D')
                {
                    t = -1;
                }

                index += 1;
            }

            if (index >= len)
            {
                goto processdigit;
            }


            //An exponent: zero or more digits. If there is at least one digit, let e be the number formed by interpreting the digits as a base-10 integer; otherwise, let e be the number 0.


            string eintpart = string.Empty;

            while (input[index].isDigit())
            {
                eintpart += input[index].ToString();

                index += 1;
                if (index >= len)
                {
                    if (eintpart.Length > 0)
                    {
                        e = Convert.ToInt32(eintpart, 10);
                    }

                    goto processdigit;
                }
            }

            if (eintpart.Length > 0)
            {
                e = Convert.ToInt32(eintpart, 10);
            }



        processdigit:
            //Return the number s·(i + f·10-d)·10te.

            double middle = i + f * Math.Pow(10, -1 * d);
            double last = Math.Pow(10, t * e);
            return s * middle * last;

        }

    }
}
