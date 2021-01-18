using Kooboo.Api;
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Web.Api.Implementation
{
    public class Authentication : SiteObjectApi<Sites.Models.Authentication>
    {
        public object GetTypes()
        {
            return new
            {
                Matcher = GetEnumDescription(typeof(MatcherType)),
                Action = GetEnumDescription(typeof(AuthenticationAction)),
                FailedAction = GetEnumDescription(typeof(FailedAction)),
            };
        }

        private object GetEnumDescription(Type type)
        {
            return Enum.GetNames(type).Select(s => new
            {
                Name = s,
                Display = s
            });
        }
    }
}
