//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataTraceAndModify
{ 
    public class CssSelectorComparer
    {
        private static HashSet<char> _Combinators;
        public static HashSet<char> Combinators
        {
            get
            {
                if (_Combinators == null)
                {
                    _Combinators = new HashSet<char>();
                    _Combinators.Add('+');
                    _Combinators.Add('|');
                    _Combinators.Add('$');
                    _Combinators.Add('^');
                    _Combinators.Add('[');
                    _Combinators.Add(']');
                    _Combinators.Add('*');
                    _Combinators.Add('=');
                    _Combinators.Add(':');
                    _Combinators.Add('~');
                    _Combinators.Add('.');
                    _Combinators.Add('#');
                    _Combinators.Add('>');
                    _Combinators.Add(',');
                    _Combinators.Add('!');
                }
                return _Combinators;
            }

        }
        public static bool IsEqual(string RuleA, string RuleB)
        {
            if (string.IsNullOrEmpty(RuleA) || string.IsNullOrEmpty(RuleB))
            {
                return false;
            }
            if (RuleA == RuleB)
            {
                return true;
            }

            var regx = new System.Text.RegularExpressions.Regex("[\n|\t|\r| ]");
            var noEmptyContentA = regx.Replace(RuleA, "");
            var noEmptyContentB = regx.Replace(RuleB, "");
            if(noEmptyContentA != noEmptyContentB)
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
            int maxi = RuleA.Length;
            if (maxi < RuleB.Length)
            {
                maxi = RuleB.Length;
            }
            int maxa = RuleA.Length;
            int maxb = RuleB.Length;
            int ai = 0;
            int bi = 0;
            char previous = ' ';
            while (ai < maxa && bi < maxb)
            {
                char a = RuleA[ai];
                char b = RuleB[bi];
                if (a == b)
                {
                    previous = a;
                    ai++;
                    bi++;
                    continue;
                }

                if (IsSpace(a) || IsSpace(b))
                {
                   if (IsSpace(previous) ||IsSpecialMark(previous))
                    {
                        if (IsSpace(a))
                        {
                            ai++;
                            continue; 
                        }
                        else
                        { bi++;
                            continue; 
                        }
                    }
                   else
                    {
                        if (IsSpace(a))
                        { 
                            var next = GetNextNonSpace(ref RuleA,ref ai, maxa); 
                            if (next == b)
                            { 
                                continue; 
                            }
                            else
                            { return false;  }
                        }
                        else
                        {
                            var nextb = GetNextNonSpace(ref RuleB, ref bi, maxb); 
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
            while(current < max)
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
