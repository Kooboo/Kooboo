//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Attributes
{
    //accepted model type of this method...
   public class RequireModel: Attribute
    {
        public Type ModelType { get; set; }

        public RequireModel(Type ModelType)
        {
            this.ModelType = ModelType;
        }
    } 
}
