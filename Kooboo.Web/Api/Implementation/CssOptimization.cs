using Kooboo.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Api.Implementation
{
    public class CssOptimization : IApi
    {
        public string ModelName => "CssOptimization";

        public bool RequireSite => true;

        public bool RequireUser => true;

        public object List()
        {
            return new[] {
                new{
                    Id=Guid.NewGuid(),
                    Content=".main{ font-size:14px }"
                },
                new{
                    Id=Guid.NewGuid(),
                    Content=".header{ color:red }"
                },
            };
        }

        public void Delete(Guid[] ids)
        {
        }
    }
}
