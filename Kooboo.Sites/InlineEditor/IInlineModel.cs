//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor
{
    public interface IInlineModel
    {  
            string EditorType { get; }

            string ObjectType { get; set; }

            ActionType Action { get; set; }

            string NameOrId { get; set; }

            string Value { get; set; }  
    }
}
