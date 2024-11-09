using System.Text.Json;
using Kooboo.Api;
using Kooboo.Data;
using Kooboo.Data.Permission;
using Kooboo.Sites.Commerce.Services;
using Kooboo.Sites.Commerce.ViewModels;
using Kooboo.Sites.Commerce.Events;
using Kooboo.Sites.Commerce;

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
            var service = new CustomerService(commerce);
            return service.List(model);
        }

        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.EDIT)]
        public void Create(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CustomerCreate>(body, Defaults.JsonSerializerOptions);
            var customer = commerce.Customer.Get(c => c.Email == model.Email);
            if (customer != default) throw new Exception($"Email {model.Email} Customer exist");
            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                customer = commerce.Customer.Get(c => c.Phone == model.Phone);
                if (customer != default) throw new Exception($"Phone {model.Phone} Customer exist");
            }
            commerce.Customer.AddOrUpdate(model.ToCustomer());
            EventDispatcher.Raise(new CustomerCreateEvent(apiCall.Context)
            {
                Customer = customer
            });
            var tagService = new TagService(commerce);
            tagService.Append(TagType.Customer, model.Tags);
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
        public CustomerDetail Get(string id, ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var customer = commerce.Customer.Get(c => c.Id == id);
            if (customer == default) throw new Exception("Customer not found");
            var membershipService = new MembershipService(commerce);
            var status = membershipService.Match(customer);
            var rewardPointService = new RewardPointService(commerce);
            var points = rewardPointService.Calculate(customer.Id);
            return new CustomerDetail(customer, status, points);
        }

        [Permission(Feature.COMMERCE_CUSTOMER, Action = Data.Permission.Action.EDIT)]
        public void Edit(ApiCall apiCall)
        {
            var commerce = GetSiteCommerce(apiCall);
            var body = apiCall.Context.Request.Body;
            var model = JsonSerializer.Deserialize<CustomerEdit>(body, Defaults.JsonSerializerOptions);
            var customer = commerce.Customer.Get(c => c.Id == model.Id);
            if (customer == default) throw new Exception("Customer not found");
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var exist = commerce.Customer.Count(c => c.Email == model.Email && c.Id != customer.Id) > 0;
                if (exist) throw new Exception($"Customer email {model.Email} exist");
            }

            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                var exist = commerce.Customer.Count(c => c.Phone == model.Phone && c.Id != customer.Id) > 0;
                if (exist) throw new Exception($"Customer phone {model.Phone} exist");
            }
            model.UpdateCustomer(customer);
            commerce.Customer.AddOrUpdate(customer);
            var tagService = new TagService(commerce);
            tagService.Append(TagType.Customer, model.Tags);
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