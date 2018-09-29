//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Language
{

   
  public   class LanguageTask
    {
        public string Key { get; set; }

        public string Content { get; set;  }

        public LanguageTask(string value, bool Iskey)
        {
            if (Iskey)
            {
                this.Key = value;
            }
            else
            {
                Content = value; 
            }
        }

        public string Render(string LangCode)
        {
            if (Content !=null)
            {
                return Content; 
            }
            else
            {
                return LanguageProvider.GetValue(this.Key, LangCode); 
            }
        }

    }
}
