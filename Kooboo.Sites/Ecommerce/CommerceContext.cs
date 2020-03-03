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
            var customerService = ServiceProvider.GetService<ICustomerService>(this.RenderContext);
            this.customer = customerService.GetFromContext(this.RenderContext); 

        } 

        public RenderContext RenderContext { get; set; }
        
        public bool IsLogin
        {
            get
            {
                return !this.customer.NoLogin; 
            }
        } 

        public Customer customer
        {
            get;set;
        }
          
    }
}
