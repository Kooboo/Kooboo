//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.ViewModel
{ 
    public class ColorDeclaration
    {
        public bool IsInline { get; set; }
  
        public string KoobooId { get; set; }

        public string Selector { get; set; }

        public Guid DeclarationId { get; set; }

        public Guid RuleId { get; set; }

        public Guid StyleId { get; set; }

        public string Property { get; set; }

        public bool Important { get; set; }

       public string CssProperty { get { return Property;  } }

        public string Value { get; set; }

        public string Color { get; set; }

        public Guid OwnerObjectId { get; set; }

        public byte OwnerConstType { get; set; }
         

    }
}
