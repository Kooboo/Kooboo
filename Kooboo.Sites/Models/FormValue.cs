//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class FormValue : SiteObject
    {
        private Guid _id; 
        public override Guid Id
        {
            get
            {
               if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid(); 
                }
                return _id; 
            }

            set
            {
                _id = value; 
            }
        }

        public FormValue()
        {
            this.ConstType = ConstObjectType.FormValue;
            this.Values = new Dictionary<string, string>(); 
        }
        public Guid FormId { get; set; }
        
        public Dictionary<string, string> Values 
        {
            get; set;
        }
  
    }
}
