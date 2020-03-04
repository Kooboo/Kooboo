using Kooboo.Api;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Kooboo.Sites.Payment;
using Kooboo.Web.Api.Implementation.Ecommerce.ViewModel;

namespace Kooboo.Web.Api.Implementation.Ecommerce
{
    public class ShoppingCartApi : SiteObjectApi<Cart>
    {

        public override List<object> List(ApiCall call)
        {

            var pageindex = call.GetIntValue("pagenumber");

            int size = 50;

            int skip = (pageindex - 1) * size;
            if (skip < 1)
            {
                skip = 0;
            }

            var items = Kooboo.Sites.Ecommerce.ServiceProvider.Cart(call.Context).List(skip, size);

            return items.Select(o => new ViewModel.ShoppingCartWebViewModel(o, call.Context)).ToList<object>();

        }


        public PagedListViewModel<ShoppingCartWebViewModel> All(ApiCall call)
        {
            var pager = ApiHelper.GetPager(call, 50);

            var service = Sites.Ecommerce.ServiceProvider.Cart(call.Context);

            var items = service.List(pager.SkipCount, pager.PageSize);

            PagedListViewModel<ShoppingCartWebViewModel> result = new PagedListViewModel<ShoppingCartWebViewModel>();

            result.TotalCount = service.Count();
            result.PageNr = pager.PageNr;
            result.PageSize = pager.PageSize;
            result.TotalPages = ApiHelper.GetPageCount(result.TotalCount, pager.PageSize);

            result.List = items.Select(o => new ViewModel.ShoppingCartWebViewModel(o, call.Context)).ToList();

            return result; 
        } 

    }
}
