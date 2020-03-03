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
        
        } 

        public RenderContext RenderContext { get; set; }
        
        public bool IsLogin
        {
            get
            {
                return !this.customer.NoLogin; 
            }
        }

        private Customer _customer; 
        public Customer customer
        {
            get
            {
                if (_customer == null)
                {
                    var customerService = ServiceProvider.GetService<ICustomerService>(this.RenderContext);
                    _customer = customerService.GetFromContext(this.RenderContext);
                }
                return _customer; 
            }
            set
            {
                _customer = value; 
            }
        }
          
    }
}
