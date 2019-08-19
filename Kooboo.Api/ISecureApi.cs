using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Api
{
    public interface ISecureApi : IApi
    {
        bool AccessCheck(ApiCommand command, RenderContext call);
    }
}
