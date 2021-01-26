using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;

namespace Kooboo.Web.Lighthouse
{
    public class AppendImgWidthHeight : ILightHouseItem
    {
        public string Name => "AppendWidthHeight";

        public string Description => "Append Width Height value for image";

        public List<Setting> Setting => null;

        public void Execute(Dictionary<string, string> Setting, RenderContext Context)
        {
            throw new NotImplementedException();
        }
    }
}
