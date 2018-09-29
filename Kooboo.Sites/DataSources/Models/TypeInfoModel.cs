//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataSources.New.Models
{
    public class TypeInfoModel
    {
        public string Name { get; set; }

        public string DeclareType { get; set; }

        public Guid Id { get; set; }
        public bool Enumerable { get; set; }

        public bool IsPagedResult { get; set; }

        public Dictionary<string, string> Paras = new Dictionary<string, string>(); 
         
        public string ModelType { get; set; }


        public bool IsPublic { get; set; }
        public List<TypeFieldModel> ItemFields { get; set; }
    }
}
