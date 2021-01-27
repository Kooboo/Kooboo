using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Lighthouse
{
    public interface ILightHouseItem
    {
        string Name { get; }

        string Description { get; }

        List<Setting> Setting { get; }

        void Execute(Dictionary<string, string> Setting, RenderContext Context);
    }
}
