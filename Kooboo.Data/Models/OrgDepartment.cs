using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Data.Models
{
  public  class OrgDepartment : IGolbalObject
    {
        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (this.OrganizationId != default(Guid) && this.Name !=null)
                    {
                        string unique = this.OrganizationId.ToString() + this.Name;
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

        public string Name { get; set; }
           
        public Guid OrganizationId { get; set; }

        public override int GetHashCode()
        {
            string unique = this.OrganizationId.ToString() + this.Name; 

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
