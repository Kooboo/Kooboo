//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor
{
    public class CssSelectorComparer
    {
        private static HashSet<char> _combinators;

        public static HashSet<char> Combinators
        {
            get
            {
                return _combinators ?? (_combinators = new HashSet<char>
                {
                    '+',
                    '|',
                    '$',
                    '^',
                    '[',
                    ']',
                    '*',
                    '=',
                    ':',
                    '~',
                    '.',
                    '#',
                    '>',
                    ',',
                    '!'
                });
            }
        }

        public static bool IsEqual(string ruleA, string ruleB)
        {
            if (string.IsNullOrEmpty(ruleA) || string.IsNullOrEmpty(ruleB))
            {
                return false;
            }
            if (ruleA == ruleB)
            {
                return true;
            }

            var regx = new System.Text.RegularExpressions.Regex("[\n|\t|\r| ]");
            var noEmptyContentA = regx.Replace(ruleA, "");
            var noEmptyContentB = regx.Replace(ruleB, "");
            if (noEmptyContentA != noEmptyContentB)
            {
                return false;
            }
            //var NoSpaceA = noNewLineA.Replace(" ", "");
            //var NoSpaceB = noNewLineB.Replace(" ", "");
            //if (NoSpaceA != NoSpaceB)
            //{
            //    return false;
            //}

            // verify they are the same by the rules.
            int maxi = ruleA.Length;
            if (maxi < ruleB.Length)
            {
                maxi = ruleB.Length;
            }
            int maxa = ruleA.Length;
            int maxb = ruleB.Length;
            int ai = 0;
            int bi = 0;
            char previous = ' ';
            while (ai < maxa && bi < maxb)
            {
                char a = ruleA[ai];
                char b = ruleB[bi];
                if (a == b)
                {
                    previous = a;
                    ai++;
                    bi++;
                    continue;
                }

                if (IsSpace(a) || IsSpace(b))
                {
                    if (IsSpace(previous) || IsSpecialMark(previous))
                    {
                        if (IsSpace(a))
                        {
                            ai++;
                            continue;
                        }
                        else
                        {
                            bi++;
                            continue;
                        }
                    }
                    else
                    {
                        if (IsSpace(a))
                        {
                            var next = GetNextNonSpace(ref ruleA, ref ai, maxa);
                            if (next == b)
                            {
                                continue;
                            }
                            else
                            { return false; }
                        }
                        else
                        {
                            var nextb = GetNextNonSpace(ref ruleB, ref bi, maxb);
                            if (nextb == a)
                            {
                                continue;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            // if still contains no space, return false;
            return true;
        }

        private static char GetNextNonSpace(ref string input, ref int current, int max)
        {
            while (current < max)
            {
                var currentchar = input[current];

                if (!IsSpace(currentchar))
                {
                    return currentchar;
                }
                current++;
            }
            return default(char);
        }

        private static bool IsSpace(char input)
        {
            return Kooboo.Lib.Helper.CharHelper.isSpaceCharacters(input);
        }

        private static bool IsSpecialMark(char input)
        {
            foreach (var item in Combinators)
            {
                if (input == item)
                { return true; }
            }
            return false;
        }
    }
}