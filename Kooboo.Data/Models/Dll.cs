//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Extensions;


namespace Kooboo.Data.Models
{
   public class Dll : IGolbalObject
    {
       private Guid _id;

       public Guid Id
       {
           get
           {
               if (_id == default(Guid))
               {
                   _id = IDGenerator.GetId(this.AssemblyName); 
               }

               return _id; 
           }
           set
           {
               _id = value; 
           }

       }

       public string AssemblyName { get; set; }

       public string AssemblyVersion { get; set; }

       /// <summary>
       /// The byte content of this assembly. 
       /// </summary>
       public byte[] Content { get; set; }

        public override int GetHashCode()
        {
            var guid =  Lib.Security.Hash.ComputeGuid(Content);
            return Lib.Security.Hash.ComputeIntCaseSensitive(guid.ToString()); 

        }
    }
}
