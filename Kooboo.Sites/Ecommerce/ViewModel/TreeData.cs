using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce.ViewModel
{
    public class TreeData
    {
        public string Key { get; set; }

        public string Value { get; set; }

        private List<TreeData> _children;
        public List<TreeData> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<TreeData>();
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
