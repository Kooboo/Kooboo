using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Authorization.Model
{
   public class PermissionViewModel
    {
        public string Name { get; set; }

        public bool Selected { get; set; }

        public List<PermissionViewModel> SubItems { get; set; } 
    }
}
