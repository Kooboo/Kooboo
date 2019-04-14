using System;
using System.Text;
using System.Collections.Generic;

namespace Kooboo.Model.Render.Vue
{
    public interface IVueRenderer
    {
        void Render(InnerJsBuilder builder, IEnumerable<object> items, VueJsBuilderOptions options);
    }
}
