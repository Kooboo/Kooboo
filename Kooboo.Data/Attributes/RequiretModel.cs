//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Attributes
{
    //accepted model type of this method...
    public class RequireModel : Attribute
    {
        public Type ModelType { get; set; }

        public RequireModel(Type modelType)
        {
            this.ModelType = modelType;
        }
    }
}