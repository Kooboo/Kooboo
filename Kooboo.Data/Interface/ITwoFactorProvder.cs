using Kooboo.Data.Context;
using System;
using System.Collections.Generic;

namespace Kooboo.Data.Interface
{
    public interface ITwoFactorProvder
    {
        Dictionary<string, string> GetHeaders(Guid userId);

        Kooboo.Data.Models.User Validate(HttpRequest request);
    }
}