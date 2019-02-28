//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.ViewModel
{                         

    public class ProductFieldViewModel
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public Dictionary<string, string> Values = new Dictionary<string, string>();

        public string ControlType { get; set; }

        public string Validations { get; set; }

        public string ToolTip { get; set; }

        public int Order { get; set; }

        public bool IsMultilingual { get; set; }

        public bool MultipleValue { get; set; }

        public string selectionOptions { get; set; }

        public bool IsSpecification { get; set; }

    }






}
