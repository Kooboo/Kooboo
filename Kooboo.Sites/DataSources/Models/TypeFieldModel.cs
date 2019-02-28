//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataSources.New.Models
{
    public class TypeFieldModel
    {
        public string Name { get; set; }

        public bool IsComplexType { get; set; }

        public bool Enumerable { get; set; }

        public string Type { get; set; }

        public List<TypeFieldModel> Fields { get; set; }

        public TypeFieldModel()
        {
            Fields = new List<TypeFieldModel>();
        }
    }
}
