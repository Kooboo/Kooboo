using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Authorization
{
    public class PermissionTree
    {  
        // not required, only for visualization... 
        public string Name { get; set; }

        private Dictionary<string, PermissionTree> _children;

        public Dictionary<string, PermissionTree> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new Dictionary<string, PermissionTree>(StringComparer.OrdinalIgnoreCase);
                }
                return _children;
            }
            set
            {
                _children = value;
            }
        }

        // Root Access has access to all subs... 
        public bool RootAccess { get; set; } 

    }
     

}
