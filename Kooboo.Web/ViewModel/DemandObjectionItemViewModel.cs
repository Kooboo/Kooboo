using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.ViewModel
{
    public class DemandObjectionItemViewModel
    {
        public Guid Id { get; set; }

        public Guid DemandId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; }

        public string Contact { get; set; }

        public DateTime CreateTime { get; set; }

        public string Status { get; set; }
    }
}
