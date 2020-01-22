using Kooboo.Data.Context;
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Interface
{
    public interface ITwoFactorProvder
    {
        Dictionary<string, string> GetHeaders(User user);

        Kooboo.Data.Models.User Validate(HttpRequest request);
    }
}
