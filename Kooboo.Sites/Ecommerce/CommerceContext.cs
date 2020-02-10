using Kooboo.Data.Context;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce
{
    public class CommerceContext
    {
        public CommerceContext(RenderContext context)
        {
            // init the context...
            this.RenderContext = context;
            InitContext(); 
        }

        private void InitContext()
        {
            this.customer = initCustomer();
        }
         
        private Customer initCustomer()
        {
            if (RenderContext.Request.Cookies.ContainsKey(Constants.CustomerCookieName))
            {
                var service = ServiceProvider.GetService<ICustomerService>(this.RenderContext);
                if (this.RenderContext.Request.Cookies.TryGetValue(Constants.CustomerCookieName, out string cookie))
                {
                    if (System.Guid.TryParse(cookie, out Guid customerid))
                    {
                        var customer = service.Get(customerid);
                        if (customer != null)
                        {
                            return customer;
                        }
                    }
                }
            }
            var newtemp = new Customer() { IsTemp = true } ;
            this.RenderContext.Response.AppendCookie(Constants.CustomerTempCookieName, newtemp.Id.ToString(), DateTime.Now.AddDays(10));
            return newtemp;
        }

        public RenderContext RenderContext { get; set; }
        
        public bool IsLogin
        {
            get
            {
                return !this.customer.IsTemp; 
            }
        }

        private Customer _Customer;
        public Customer customer
        {
            get
            {
                if (_Customer == null)
                {
                    _Customer = initCustomer();
                }
                return _Customer;
            }
            set
            {
                _Customer = value;
            }
        }
          
    }
}
