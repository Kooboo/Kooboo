using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class MenuItem
    {
        public string Name { get; set; }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    return this.Name; 
                }
               
                return _displayName;
            }
            set
            {
                _displayName = value; 
            }
        }

        public string Icon { get; set; }

        public string Url { get; set; }

        public uint ActionRights { get; set; } = 0; 

        private List<MenuItem> _items;
        public List<MenuItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<MenuItem>();
                }
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        private string SplitName(string name, bool toLower = true)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                var ch = name[i];
                if (ch >= 'A' && ch <= 'Z' && i > 0)
                {
                    var prev = name[i - 1];
                    if (prev != ' ')
                    {
                        if (prev >= 'A' && prev <= 'Z')
                        {
                            if (i < name.Length - 1)
                            {
                                var next = name[i + 1];
                                if (next >= 'a' && next <= 'z')
                                {
                                    builder.Append(' ');
                                }
                            }
                        }
                        else
                        {
                            builder.Append(' ');
                        }
                    }
                    builder.Append(toLower ? ch.ToString().ToLower() : ch.ToString());
                }
                else
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }
    }

    public class GlobalMenuItem : MenuItem
    {
        public int? Count { get; set; }

        public string BadgeIcon { get; set; }
    }

    public class MainMenuItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; }

        public DateTime LastModified { get; set; }
        public Dictionary<string, int> Relations { get; set; }

    }


 
    public class SiteMenuItemViewModel
    { 
        public Guid RootId { get; set; }
         
        public  Guid Id { get; set; }
          
        public string Name { get; set; }
        public string Url { get; set; }


        public string SubItemContainer { get; set; }

     
        public string SubItemTemplate { get; set; }
  
        public Guid ParentId { get; set; }
         
        public Guid DataSourceId { get; set; } = default(Guid);

        public string Template { get; set; }

        public List<SiteMenuItemViewModel> Children { get; set; } = new List<SiteMenuItemViewModel>(); 
  
    }
   

}
