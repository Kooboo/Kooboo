//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor.Model
{
    public class StyleModel : IInlineModel
    {
        public ActionType Action
        {
            get;set;
        } 

        public string EditorType
        {
            get
            {
                return "style"; 
            }
        }

        public string NameOrId
        { get;set;
        }

        public string ObjectType
        {
            get;set;
        }

        public string Value
        {
            get;set;
        }
         
        public Guid RuleId { get; set; }
         
        public Guid StyleId { get; set; }
         
        public string Property { get; set; }
          
        public string Selector { get; set; }
           
        public bool Important { get; set; }
 
        public string StyleSheetUrl { get; set; } 

        //For modify on the embedded style sheet. rules under <style>div{}</style>
        public string StyleTagKoobooId { get; set; } 

        public string KoobooId { get; set; }
    }  
}
