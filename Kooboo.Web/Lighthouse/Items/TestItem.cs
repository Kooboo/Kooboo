using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Lighthouse.Items
{
    public class TestItem : LightHouseItemWithSetting<CustomSetting>
    {
        public override string Name => "Test";

        public override string Description => "Description Description Description";

        public override void Execute(CustomSetting setting, RenderContext Context)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomSetting : ILightHouseItemSetting
    {
        [Description("Text Description")]
        public string Text { get; set; }

        [Description("Number Description")]
        public int Number { get; set; }

        public bool Switch { get; set; }
    }
}
