using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Infrastructure
{
  public  interface IInfrastructure
    {
        bool Test(Guid OrganizationId, byte InfraType, int amount);

        void Add(Guid OrganizationId, byte InfraType, int Amount, string Item); 
    }
}
