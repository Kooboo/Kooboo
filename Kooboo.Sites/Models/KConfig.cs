//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class KConfig : CoreObject
    {      
        public KConfig()
        {
            this.ConstType = ConstObjectType.Kconfig; 
        }

        private Dictionary<string, string> _binding; 
               
        public Dictionary<string, string> Binding {
            get {
                if (_binding == null)
                {
                    _binding = new Dictionary<string, string>(); 
                }
                return _binding; 
            }
            set {
                _binding = value; 
            }
        }

        // The original tag.
        public string TagName { get; set; }

        public string TagHtml { get; set; }

        //public Dictionary<string, string> Binding { get; set; } 
        public override int GetHashCode()
        {
            string unique = this.TagName + this.TagHtml; 
            if (this.Binding !=null)
            {
                foreach (var item in this.Binding)
                {
                    unique += item.Key + item.Value; 
                }
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }

    }
}
