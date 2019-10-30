//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Sites.ViewModel
{
    /// <summary>
    /// Used for submit a new menu to back end...
    /// </summary>
    public class MenuViewModel
    {
        public string LinkElement { get; set; }
        public string ContainerElement { get; set; }
        public string ItemContainer { get; set; }
        public string href { get; set; }
        public string text { get; set; }

        public bool RenderSubMenuIndepedently { get; set; }

        private List<MenuViewModel> _children;

        public List<MenuViewModel> children
        {
            get { return _children ?? (_children = new List<MenuViewModel>()); }
            set
            {
                _children = value;
            }
        }
    }
}