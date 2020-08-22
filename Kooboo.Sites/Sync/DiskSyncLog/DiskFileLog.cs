using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Sync
{
  public  class DiskFileLog
    {
        private Guid _id; 
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrWhiteSpace(this.FullPath))
                    {
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(this.FullPath);
                    } 
                }
                return _id;
            }
            set
            {
                _id = value; 
            }
        }

        public string FullPath { get; set; }

        public Guid ObjectId { get; set; }

        public  DateTime LastModify { get; set; }

        public Guid ByteHash { get; set; }

        public bool ToDisk { get; set; } = true;

        public override int GetHashCode()
        {
            string unique = this.FullPath + this.ObjectId.ToString() + this.LastModify.ToShortTimeString() + this.ToDisk.ToString() + this.ByteHash.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
             
        }
    }
}
