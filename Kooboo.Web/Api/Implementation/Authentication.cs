using Kooboo.Api;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Web.Api.Implementation
{
    public class Authentication : IApi
    {
        public string ModelName => "Authentication";

        public bool RequireSite => true;

        public bool RequireUser => true;

    }
}
