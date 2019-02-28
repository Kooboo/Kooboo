//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.ViewModel
{

    /// <summary>
    /// Used for submit a new menu to back end... 
    /// </summary>
   public class MenuViewModel
    { 
        public string LinkElement { get; set;}
        public string ContainerElement { get; set; }
        public string ItemContainer { get; set; } 
        public string href { get; set; }
        public string text { get; set; }

        public bool RenderSubMenuIndepedently { get; set; }

        private List<MenuViewModel> _children; 
        public List<MenuViewModel> children {
            get
            {
                if (_children == null)
                {
                    _children = new List<MenuViewModel>(); 
                }
                return _children; 
            }
            set
            {
                _children = value; 
            }
        }  
    }
}
