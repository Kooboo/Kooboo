using Kooboo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Ensurance
{ 
    public class EnsureObject : IGolbalObject
    {
        /// <summary>
        /// Full name of task model type.. 
        /// </summary>
        public string  ModelType { get; set; }

        public string Json  { get; set; }
          
        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid();
                }
                return _id;
            }

            set { _id = value; }
        }

        public override int GetHashCode()
        {
            string unique = this.Json + this.ModelType; 
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    } 

}
