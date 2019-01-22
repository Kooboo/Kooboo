using System;
using System.Collections.Generic;


namespace Kooboo.Sites.Scripting.Helper
{
    public class KScriptSetting: ExampleSetting
    {

        public List<Property> Props { get; set; } = new List<Property>();

        public List<Method> Methods { get; set; } = new List<Method>();

    }
}
