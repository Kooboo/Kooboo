using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class TestData : IFunction
    {
        public string Name => "TestData";

        public List<IFunction> Parameters { get; set; }

        public object Render(RenderContext context)
        {
            List<object> Data = new List<object>();
            Dictionary<string, string> item = new Dictionary<string, string>();
            item.Add("firstname", "myfirstname"); 
            Data.Add(item);
            return Data; 
        }
    }
}
