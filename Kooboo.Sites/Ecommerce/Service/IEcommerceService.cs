using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Ecommerce.Service
{
    public interface IEcommerceService : Kooboo.Lib.IOC.IPriority
    {
        RenderContext Context { get; set; }

        CommerceContext CommerceContext { get; set; }
    }

    public interface IEcommerceService<T> : IEcommerceService
    {
        bool AddOrUpdate(T value);

        bool AddOrUpdate(T value, Guid UserId);

        void Delete(Guid id, Guid UserId);

        void Delete(Guid id);

        T Get(Guid id);

        Task<T> GetAsync(Guid id);

        T Get(string nameorid);

    }
}
