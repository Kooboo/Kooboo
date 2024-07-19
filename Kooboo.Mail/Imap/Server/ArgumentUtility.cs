//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Mail.Imap
{
    public class ArgumentUtility
    {

        // TODO: Please add unit test here.....(GUOQI)
        public static PartialArgs Parse(string args)
        {
            if (!args.EndsWith("}"))
                return new PartialArgs { Args = args };

            for (int i = args.Length - 2; i >= 0; i--)
            {
                if (args[i] == '{')
                {
                    // No digit found between { }
                    if (i == args.Length - 2)
                        return new PartialArgs { Args = args };

                    // Get size between { }, and parse string before { as partial argument
                    return new PartialArgs
                    {
                        Args = args.Substring(0, i),
                        Size = Convert.ToInt32(args.Substring(i + 1, args.Length - 2 - i))
                    };
                }
                else if (!Char.IsDigit(args[i]))
                {
                    return new PartialArgs { Args = args };
                }
            }

            return new PartialArgs { Args = args };
        }
    }

    public class PartialArgs
    {
        public string Args { get; set; }

        public int? Size { get; set; }
    }
}
