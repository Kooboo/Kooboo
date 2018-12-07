using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class Discussion : IGolbalObject
    {
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
            set
            {
                _id = value;
            }
        }
    
        public string Title { get; set; }

        public string Content { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public DateTime LastModified { get; set; }

        public int ViewCount { get; set; }

        public int CommentCount { get; set; }


        public override int GetHashCode()
        {
            string unique = this.Title + this.Content + this.UserId.ToString() + this.UserName;

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }


    }
}
