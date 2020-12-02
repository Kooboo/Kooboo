using Kooboo.Data.Attributes;
using System.Collections.Generic;

namespace Kooboo.Sites.OAuth2
{
    public interface IOAuth2
    {
        [KIgnore]
        string Callback(IDictionary<string, object> query);
        string GetAuthUrl(IDictionary<string, object> @params);
    }
}