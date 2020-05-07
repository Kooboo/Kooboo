using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Models
{
    public class UserDepartmentFunction : IGolbalObject
    {  
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (this.UserId != default(Guid) && this.OrganizationId != default(Guid))
                    {
                        string unique = this.UserId.ToString() + this.OrganizationId.ToString();
                        unique += this.DepartmentId.ToString(); 
                        _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique);
                    } 
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public Guid UserId { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid OrganizationId { get; set; }

        public string Function { get; set; }

        // The manager of this department. The head of this department. 
        public bool IsHead { get; set; }

        public override int GetHashCode()
        {
            string unique = this.UserId.ToString() + this.OrganizationId.ToString() + this.DepartmentId.ToString();

            unique += this.Function + this.IsHead.ToString(); 

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    }
}
