using System;
using System.Collections.Generic;

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
                return _children ??
                       (_children = new Dictionary<string, PermissionTree>(StringComparer.OrdinalIgnoreCase));
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