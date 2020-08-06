//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
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


        public virtual bool OnActionExecuting(ApiCall ApiCall)
        {
            return true;
        }

        public virtual void OnActionExecuted(ApiCall ApiCall)
        {
        }
    }
}
