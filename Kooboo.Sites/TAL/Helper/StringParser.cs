using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.TAL
{
    public class StringParser
    {
        /// <summary>
        /// Parse a string into name/value(expression) pair, with possible additionals.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static NameValues Parse(string input)
        {
            NameValues names = new NameValues();

            int sepindex = getSeperatorIndex(input);

            if (sepindex > 0)
            {
                names.name = input.Substring(0, sepindex).Trim();
                string sourcekey = input.Substring(sepindex + 1).Trim();

                int additionalsep = getSeperatorIndex(sourcekey);
                if (additionalsep > 0)
                {
                    names.value = sourcekey.Substring(0, additionalsep).Trim();
                    names.additional = sourcekey.Substring(additionalsep + 1);
                }
                else
                {
                    names.value = sourcekey;
                }
            }
            else
            {
                names.name = input;
            }

            return names;
        }

        public static KeyValue ParseKeyValue(string input)
        {
              KeyValue names = new KeyValue();

            int sepindex = getSeperatorIndex(input);

            if (sepindex > 0)
            {
                names.name = input.Substring(0, sepindex).Trim();
                names.value = input.Substring(sepindex + 1).Trim();
            }
            else
            {
                names.name = input;
            }

            return names;
        }

        private static int getSeperatorIndex(string input)
        {
            int sepindex = input.IndexOf(" ");
            if (sepindex < 0)
            {
                sepindex = input.IndexOf(":");
                if (sepindex < 0)
                {
                    sepindex = input.IndexOf("\\");
                    if (sepindex < 0)
                    {
                        sepindex = input.IndexOf("/");
                        if (sepindex < 0)
                        {
                            sepindex = input.IndexOf(".");
                        }
                    }
                }
            }
            return sepindex;
        }

        /// <summary>
        ///  check whether this is a string value or not. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool isString(string input)
        {
            input = input.Trim();

            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                return true;
            }

            if (input.StartsWith("'") && input.EndsWith("'"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// separate a string into a , seperated list of strings. 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<string> getCommaSeparatedString(string input)
        {
            List<string> list = new List<string>();

            if (string.IsNullOrEmpty(input))
            {
                return list;
            }

            input = input.Trim();
            if (string.IsNullOrEmpty(input))
            {
                return list;
            }

            string temp = string.Empty;

            int length = input.Length;
            bool isescape = false;

            for (int i = 0; i < length; i++)
            {
                if (isescape)
                {
                    temp += input[i];
                    isescape = false;
                    continue;
                }

                if (input[i] == '\\')
                {
                    isescape = true;
                    continue;
                }

                if (input[i] == ',')
                {
                    list.Add(temp);
                    temp = string.Empty;
                }
                else
                {
                    temp += input[i].ToString();
                }
            }

            list.Add(temp);

            return list;

        }


        /// <summary>
        /// whether the input is a function expression or not.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool isExpression(string input)
        {
            if (input.Contains(")") && input.Contains("("))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public class NameValues
    {

        public string name;
        public string value;
        public string additional;

    }

    public class KeyValue
    {
        public string name;
        public string value;

    }

}
