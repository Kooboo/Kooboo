using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class QueueObject : ISiteObject
    {  
        /// <summary>
        /// Full name of task model type.. 
        /// </summary>
        public string  TaskModelType { get; set; }
         
        public string JsonModel { get; set; }

        public byte ConstType
        {
            get;set;
        }

        public DateTime CreationDate
        {
            get;set;
        }

        public DateTime LastModified
        {
            get;set;
        }
 
        public Guid Id
        {
            get;set;
        }

        public string Name
        {
            get;set;
        }
    } 

}
