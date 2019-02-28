//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kooboo.Sites.Contents.Models
{
    [Kooboo.Attributes.NameAsID]
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class Label : MultipleLanguageObject
    {
        public Label()
        {
            this.ConstType = ConstObjectType.Label; 
        }

        public override int GetHashCode()
        {
            string unique = string.Empty;
            foreach (var item in this.Values)
            {
                unique += item;
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
