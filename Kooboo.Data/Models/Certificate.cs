//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Data.Models
{
    public class Certificate
    {
        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(Domain);
                }
                return _id;
            }
        }

        public string Domain { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime ExpireDate { get; set; }
        /// <summary>
        /// certificate renew date
        /// </summary>
        public DateTime RenewDate { get; set; }

        public string ThumbPrint { get; set; }

        /// <summary>
        /// certificate content
        /// </summary>
        public Byte[] Content { get; set; }

        public bool IsDelete { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime UpdateDate { get; set; }

        
    }
}
