using Kooboo.App.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.App.Models
{
    internal class EditorPageViewModel<T> : BaseModel
        where T:class
    {
        public DelegateCommand<T> AddCommand { get; set; }
    }
}
