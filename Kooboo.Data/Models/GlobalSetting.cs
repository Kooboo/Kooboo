//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
   public class GlobalSetting : IGolbalObject
    {
        public string Name { get; set; }

        public Guid OrganizationId { get; set; }

        public Dictionary<string, string> KeyValues { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); 

        public bool HasKey(string key)
        {
            return this.KeyValues.ContainsKey(key); 
        }

        public string GetValue(string key)
        {
            if (this.KeyValues.ContainsKey(key))
            {
                return this.KeyValues[key]; 
            }
            return null; 
        }

        public string Values { get; set; }

        public DateTime LastModified { get; set; } = DateTime.Now; 

        public DateTime Expiration { get; set; }

        public int Version { get; set; }
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string unique = this.Name + this.OrganizationId.ToString() + this.Version.ToString();
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique); 
                }
                
                return _id;
            }
            set { _id = value; }
        }
         
    }
}
