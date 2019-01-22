using Kooboo.Data.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
  public  class SiteUser : SiteObject
    {
        public SiteUser()
        {
            this.ConstType = ConstObjectType.SiteUser; 
        }

        public override Guid Id {
            get
            {
                return UserId; 
            }
            set
            {
                this.UserId = value; 
            }
        }

        public Guid UserId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public  Kooboo.Sites.Authorization.EnumUserRole Role { get; set; }

        public override int GetHashCode()
        {
            return (int)Role; 
        }

    }
}
