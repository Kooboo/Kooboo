//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Api.ApiResponse;

namespace Kooboo.Api
{
    public interface IApi
    {
        string ModelName { get; }
        bool RequireSite { get; }
        bool RequireUser { get; }
    }

    public abstract class Api : IApi
    {

        public abstract string ModelName { get; }

        public abstract bool RequireSite { get; }

        public abstract bool RequireUser { get; }


        public virtual IResponse OnActionExecuting(ApiCall call)
        {
            return null;
        }

        public virtual void OnActionExecuted(ApiCall call)
        {
        }
    }
}
