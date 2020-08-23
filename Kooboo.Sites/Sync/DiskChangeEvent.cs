//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Kooboo.Data.Models;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Sync.DiskSyncLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync
{  
    public class DiskChangeEvent : IEquatable<DiskChangeEvent>
    {
        public Guid Id { get; set; } = System.Guid.NewGuid();
        // for rename. 
        public string OldFullPath { get; set; }

        public string FullPath { get; set; }

        public DiskChangeType ChangeType { get; set; }

        public bool Peeked { get; set; }

        public Guid ObjectId { get; set; }

        public override int GetHashCode()
        {
            return Lib.Security.Hash.ComputeInt(this.Id.ToString());
        }

        public bool Equals(DiskChangeEvent other)
        {
            return this.Id == other.Id;
        }
    }

    public enum DiskChangeType
    {
        Created = 1,
        Updated = 2,
        Deleted = 3,
        Rename = 4
    }

}

