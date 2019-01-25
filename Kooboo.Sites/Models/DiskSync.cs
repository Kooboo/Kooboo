//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
   public class DiskSync  : SiteObject
    {
        public DiskSync()
        {
            this.ConstType = ConstObjectType.DiskSync;
        }
        private Guid _id;
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string unique =  this.StoreName + this.ObjectId.ToString() + this.DbToDisk.ToString();
                    _id = unique.ToHashGuid();
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }
         
        public string StoreName { get; set; }

        // default =  the main index... 
        public Guid ObjectId { get; set; }
         
        //true = from db to disk..
        public bool DbToDisk { get; set; }

        public long LastestVersion { get; set; }

        public Guid DiskByteHash { get; set; }

        public override int GetHashCode()
        {
            return Lib.Security.Hash.ComputeInt(LastestVersion.ToString());
        }
    }

 

}
