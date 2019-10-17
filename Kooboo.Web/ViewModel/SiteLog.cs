//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class SiteLogViewModel
    {
        public Int64 Id { get; set; }

        public DateTime LastModified { get; set; }

        public string UserName { get; set; }

        public string ItemName { get; set; }

        public string StoreName { get; set; }

        public Guid KeyHash { get; set; }

        public int StoreNameHash { get; set; } 

        public int TableNameHash { get; set; }

        public string TableName { get; set; }
        /// <summary>
        /// insert, update, delete. 
        /// </summary>
        public string ActionType { get; set; }
    }

    public class ItemVersionViewModel
    {
        public long Id { get; set; }

        private DateTime   _lastmodified; 

        public DateTime LastModified {
            get {
               if (_lastmodified == default(DateTime))
                {
                    _lastmodified = DateTime.UtcNow; 
                }
                return _lastmodified; 
            }
            set { 
                _lastmodified = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }
        public string UserName { get; set; }
    }
    
    public class VersionCompareViewModel
    {
        public string Title1 { get; set; }

        public string Title2 { get; set; }

        public Guid ObjectId { get; set; }
        
        public byte ConstType { get; set; }
        
        public Int64 Id1 { get; set; }

        public Int64 Id2 { get; set; }
        
        public string Source1 { get; set; }
        
        public string Source2 { get; set; }

        public VersionDataType DataType { get; set; }
    }


    public enum VersionDataType
    {
        String = 0,
        Image = 1,
        Stream = 2
    }

}
