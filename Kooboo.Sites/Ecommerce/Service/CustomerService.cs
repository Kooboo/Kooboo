using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Repository;
using Kooboo.Sites.Ecommerce.Service;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Ecommerce
{
    public class CustomerService : ServiceBase<Customer>
    {   
        public bool Login(string username, string password)
        {
            if (this.Repo != null)
            {
                var customer = this.Repo.Get(username);
                if (customer != null)
                {
                    var passwordhash = Lib.IOC.Service.GetSingleTon<IPasswordHash>();
                    bool loginok = passwordhash.Verify(password, customer.Password);
                    if (loginok)
                    {
                        //write the cookie. 
                        this.Context.Response.AppendCookie(Constants.CustomerCookieName, customer.Id.ToString(), DateTime.Now.AddDays(30));
                    } 
                    return loginok;
                }
            }
            return false;
        }



    }
}
