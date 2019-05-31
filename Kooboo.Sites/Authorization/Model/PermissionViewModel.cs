using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Authorization.Model
{
    public class PermissionViewModel
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public bool Selected { get; set; }
        

        private List<PermissionViewModel> _subitems;
        public List<PermissionViewModel> SubItems
        {
            get
            {
                if (_subitems == null)
                {
                    _subitems = new List<PermissionViewModel>();
                }
                return _subitems;
            }
            set
            {
                _subitems = value;
            }
        }

        //development/view , layout /sublayout.
    }
}
