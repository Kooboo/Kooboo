//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;

namespace Kooboo.Data.Events
{
    public interface IMailHandler<T> where T : IEvent
    {
        void Handle(T theEvent, RenderContext context);
    }
}
