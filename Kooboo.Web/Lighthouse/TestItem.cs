using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Lighthouse
{
    public class TestItem : ILightHouseItem
    {
        public string Name => "Test";

        public string Description => "Description Description Description";

        public List<Setting> Setting => new List<Setting> {
            new Setting{Name="Text",ControlType=ControlType.Text,DisplayName="Custom text setting" },
            new Setting{Name="Switch",ControlType=ControlType.Switch,DisplayName="Custom switch setting" },
            new Setting{Name="Number",ControlType=ControlType.Number,DisplayName="Custom number setting" },
        };

        public void Execute(Dictionary<string, string> Setting, RenderContext Context)
        {

        }
    }
}
