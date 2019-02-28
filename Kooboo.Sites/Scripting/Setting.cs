//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting
{
  public  class JintSetting
    {
        public static void SetOption(Jint.Options option)
        {
            option.MaxStatements(500);
            option.Strict(false);
            option.TimeoutInterval(new TimeSpan(0, 0, 15));
            option.LimitRecursion(30); 
        }

        public static void SetDebugOption(Jint.Options option)
        {
            option.MaxStatements(2000);
            option.Strict(false);
            option.TimeoutInterval(new TimeSpan(0, 0, 150000));
            option.LimitRecursion(30);
            option.AllowDebuggerStatement(true); 
            option.DebugMode(true);  
        } 
    }
}
