//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Diskable : Attribute
    {
        public DiskType DiskType { get; set; }
        public Diskable(DiskType DiskType)
        {
            this.DiskType = DiskType; 
        }

        public Diskable()
        {
            this.DiskType = DiskType.Text; 
        }
    }

    public enum DiskType
    {
        Text=1,
        Binary =2,
        Json = 3
    }
}
