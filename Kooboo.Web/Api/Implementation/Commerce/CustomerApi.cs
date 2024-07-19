using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.DataStorage;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Commerce.Events;

namespace Kooboo.Web.Api.Implementation.Commerce
{
    public class CustomerApi : CommerceApi
    {
        public override string ModelName => "Customer";


        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        public PagingResult List(ApiCall apiCall)
        {
            var model = JsonSerializer.Deserialize<CustomerQuery>(apiCall.Context.Request.Body, Defaults.JsonSerializerOptions);
            var commerce = GetSiteCommerce(apiCall);
            var skipCount = (model.PageIndex - 1) * model.PageSize;
            Expression<Func<CustomerModel, bool>> where = null;

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                model.Keyword = model.Keyword.ToLower().Trim();
                where = c => c.Email.ToLower().Contains(model.Keyword) || c.Phone == model.Keyword || c.FirstName.ToLower().Contains(model.Keyword) || c.LastName.ToLower().Contains(model.Keyword);
            }

            var list = commerce.Customer.Query(where == default ? q => q.Skip(skipCount).OrderByDescending(s => s.UpdatedAt).Take(model.PageSize) : q => q.Where(where).OrderByDescending(s => s.UpdatedAt).Skip(skipCount).Take(model.PageSize));
            var count = commerce.Customer.Count(where);
            return new PagingResult(list.Select(s => new CustomerListItem(s)).ToArray(), count, model);
        }

        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CustomerCreate>(body, Defaults.JsonSerializerOptions);
            var customer = commerce.Customer.Get(c => c.Email == model.Email || c.Phone == model.Phone);
            if (customer != default) throw new Exception("Customer exist");
            commerce.Customer.AddOrUpdate(model.ToCustomer());
            EventDispatcher.Raise(new CustomerCreateEvent(apiCall.Context)
            {
                Customer = customer
            });
            var tagService = new TagService(commerce);
            tagService.Append(Sites.Commerce.Entities.Tag.TagType.Customer, model.Tags);
        }

        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.VIEW)]
        public CustomerEdit GetEdit(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var customer = commerce.Customer.Get(c => c.Id == id);
            if (customer == default) throw new Exception("Customer not found");
            return new CustomerEdit(customer);
        }

        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.VIEW)]
        [Permission(Feature.COMMERCE_CART, Action = Data.Permission.Action.VIEW)]
        public CustomerListItem Get(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var customer = commerce.Customer.Get(c => c.Id == id);
            if (customer == default) throw new Exception("Customer not found");
            return new CustomerListItem(customer);
        }

        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CustomerEdit>(body, Defaults.JsonSerializerOptions);
            var exist = commerce.Customer.Get(c => c.Id != model.Id && (c.Email == model.Email || c.Phone == model.Phone));
            if (exist != default) throw new Exception("Customer exist");
            var customer = commerce.Customer.Get(c => c.Id == model.Id);
            if (customer == default) throw new Exception("Customer not found");
            model.UpdateCustomer(customer);
            commerce.Customer.AddOrUpdate(customer);
            var tagService = new TagService(commerce);
            tagService.Append(Sites.Commerce.Entities.Tag.TagType.Customer, model.Tags);
        }

        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.EDIT)]
        public void Deletes(string[] ids, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var service = new CustomerService(commerce);
            service.Deletes(ids);
        }
    }
}