//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting
{
    public static class JintSetting
    {
        public static int MaxStatements { get; set; } = 1000000;

        public static int LimitRecursion { get; set; } = 5000;

        public static int TimeOutSeconds { get; set; } = 300;

        public static void SetOption(Jint.Options option)
        { 
            option.MaxStatements(MaxStatements); 
            option.Strict(false);
            option.TimeoutInterval(new TimeSpan(0, 0, TimeOutSeconds));
            option.LimitRecursion(LimitRecursion);
        }

        public static void SetDebugOption(Jint.Options option)
        {
            option.MaxStatements(MaxStatements);
            option.Strict(false);
            option.TimeoutInterval(new TimeSpan(0, 0, 150000));
            option.LimitRecursion(LimitRecursion);
            option.AllowDebuggerStatement(true);
            option.DebugMode(true);
        }
    }
}
