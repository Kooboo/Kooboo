//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom
{
    public static class CharacterExtension
    {

        public static bool isOneOf(this string input, params string[] stringlist)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            foreach (var item in stringlist)
            {
                if (item == input)
                {
                    return true;
                }
            }

            return false;

        }

        public static bool isOneOf(this char input, params char[] chars)
        {
            foreach (var item in chars)
            {
                if (input == item)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// The first char of the string is one of the chars. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool isOneOf(this string input, params char[] chars)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            char firstchar = input[0];

            foreach (var item in chars)
            {
                if (firstchar == item)
                {
                    return true;
                }
            }

            return false;

        }

        public static bool isSpaceCharacter(this string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return true;
            }

            if (data.Length > 1)
            {
                foreach (var item in data.ToCharArray())
                {
                    if (!_isSpaceCharacter(item))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return _isSpaceCharacter(data[0]);
            }
        }

        public static bool _isSpaceCharacter(char chr)
        {
            return CommonIdoms.isSpaceCharacters(chr);
        }

        public static bool isSpaceCharacter(this char chr)
        {
            return _isSpaceCharacter(chr);
        }

        public static bool isCSSCombinator(this char chr)
        {
            foreach (var item in CSS.Deinitions.Combinator())
            {
                if (chr == item)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isCSSWhiteSpace(this char chr)
        {
            foreach (var item in CSS.Deinitions.WhiteSpace())
            {
                if (chr == item)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
