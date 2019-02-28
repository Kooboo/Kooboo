//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Sync.Cluster
{
  public  class NodeUpdate
    {
        private Guid _id; 
       
        public Guid Id
        {
            get {

                if (_id == default(Guid))
                {
                    string unique = this.ObjectId.ToString() + this.Sender.ToString() + this.SenderTick.ToString() + this.IsDelete.ToString() + this.Language;
                    _id=  unique.ToHashGuid();  
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }
        }

        public Guid ObjectId { get; set; }

        /// <summary>
        /// Action can be Add, Update or Delete, when = Delete, only objectId and ConstType is required;
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// The log version number on local database of this object. 
        /// After an object is saved, it will get a version number. 
        /// </summary>
        public long LocalVersion { get; set; }
          
        public byte ObjectConstType { get; set; }
 
        /// <summary>
        /// The culture of text content....
        /// </summary>
        public string Language { get; set; }
         
        /// <summary>
        /// Used for multi master synchronization. 
        /// </summary>
        public int Sender { get; set; }
        /// <summary>
        /// Used for multi master synchronization
        /// </summary>
        public long SenderTick { get; set; } 
   
    }
}
