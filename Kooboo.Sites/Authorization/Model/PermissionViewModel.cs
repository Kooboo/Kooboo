using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kooboo.Sites.Authorization.Model
{
    public class PermissionViewModel
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public bool Selected { get; set; }

        public string DisplayName { get; set; }


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

        public PermissionViewModel Clone()
        {
            PermissionViewModel result = new PermissionViewModel();
            result.Name = this.Name;
            result.Id = this.Id;
            result.Selected = this.Selected;

            if (_subitems != null && _subitems.Any())
            {
                foreach (var item in _subitems)
                {
                    result.SubItems.Add(item.Clone()); 
                }
            }

            return result;  
        }
    }
}
