using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Model.Render
{
    public interface IJsBuilder
    {
        IJsBuilder AddItem(object item);

        string Build();
    }
}
