//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class ApiGenerationViewModel
    {
        public string Type { get; set; }

        public string TypeDisplayName { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<string> Actions { get; set; }
    }


    public class ApiGenerationUpdate
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public List<string> Actions { get; set; }

    }
}
