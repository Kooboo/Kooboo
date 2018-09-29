//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class UsedByRelation
    {
        public Guid ObjectId { get; set; }
        public byte ConstType { get; set; }

        public Type ModelType { get; set; }

        /// <summary>
        ///  The Url that can be used to display the object info. 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The name that should appears in the Name column... 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Additional information... 
        /// </summary>
        public string Remark { get; set; }

    }
}
