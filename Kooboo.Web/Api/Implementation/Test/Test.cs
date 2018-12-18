using Kooboo.Api;
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace Kooboo.Web.Api.Implementation.Test
{
    public class TestApi : IApi
    {
        public string ModelName => "Test";

        public bool RequireSite => false;

        public bool RequireUser => false;


        public object Test(ApiCall call)
        {
            var allassem = Lib.Reflection.AssemblyLoader.AllAssemblies;

           return  allassem.Select(o => o.FullName).ToList();  
        }
    }
}
