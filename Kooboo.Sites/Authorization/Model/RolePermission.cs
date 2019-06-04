using Kooboo.Sites.Models; 
using System.Collections.Generic;
using System.Linq; 

namespace Kooboo.Sites.Authorization.Model
{
   public class RolePermission : CoreObject
    { 
        //TODO: should enable cache... 
        public RolePermission()
        {
            this.ConstType = ConstObjectType.RolePermission;
        } 

        private List<string> _permission;  
        // In the format of development/view.
        public List<string> Permission
        {
            get
            {
                if (_permission == null)
                {
                    _permission = new List<string>(); 
                }
                return _permission; 
            }

            set
            {
                _permission = value; 
            } 
        } 

        private PermissionTree _tree;
        public PermissionTree Tree
        {
            get
            {
                if (_tree == null)
                {
                    if (_permission != null && _permission.Any())
                    {
                        _tree = TreeHelper.GenerateTree(_permission); 
                    } 
                }
                return _tree; 
            }
        }

    }
}
