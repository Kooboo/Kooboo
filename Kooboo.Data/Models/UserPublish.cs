//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
   public class UserPublish : IGolbalObject
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid UserId { get; set; } 

        public string ServerUrl { get; set; }

        public bool IsReserved { get; set; }

        public override int GetHashCode()
        {
            string unique = this.Name + this.UserId.ToString() + this.ServerUrl;
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);

        }
    }
     
}
