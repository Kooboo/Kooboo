using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Web.JQL.Commands
{
    public class DatabaseGet : ICommand
    {
        public string NameOrId { get; set; }
        public string Table { get; set; }
    }
}
