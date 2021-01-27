using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;

namespace Kooboo.Web.Lighthouse.Items
{
    public class AppendImgWidthHeight :ILightHouseItem
    {
        public  string Name => "AppendWidthHeight";

        public  string Description => "Append Width Height value for image";

        public  void Execute( RenderContext Context)
        {
            throw new NotImplementedException();
        }
    }
}
