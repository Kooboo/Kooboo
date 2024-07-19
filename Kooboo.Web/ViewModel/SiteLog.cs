//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
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
        public bool HasVideo { get; set; }
    }

    public class ItemVersionViewModel
    {
        public long Id { get; set; }

        private DateTime _lastmodified;

        public DateTime LastModified
        {
            get
            {
                if (_lastmodified == default(DateTime))
                {
                    _lastmodified = DateTime.UtcNow;
                }
                return _lastmodified;
            }
            set
            {
                _lastmodified = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }
        public string UserName { get; set; }

        public bool HasVideo { get; set; }
    }
}
