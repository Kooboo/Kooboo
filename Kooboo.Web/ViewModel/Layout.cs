//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    
    public class LayoutItemViewModel
    {

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public DateTime LastModified { get; set; }

        public Dictionary<string, int> Relations { get; set; }

    }



}
