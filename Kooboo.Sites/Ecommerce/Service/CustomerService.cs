using Kooboo.Data.Interface;
using Kooboo.Sites.Ecommerce.Models;
using Kooboo.Sites.Ecommerce.Service;
using System;

namespace Kooboo.Sites.Ecommerce
{
    public class CustomerService : ServiceBase<Customer>, ICustomerService
    {
        public Customer Login(string nameOrEmail, string password)
        { 
            var customer = this.Repo.Get(nameOrEmail);
            if (customer != null)
            { 
                bool loginok = Service.PasswordService.Verify(password, customer.Password);
                if (loginok)
                {
                    //write the cookie. 
                    this.Context.Response.AppendCookie(Constants.CustomerCookieName, customer.Id.ToString(), DateTime.Now.AddDays(30));
                    return customer;
                }
                else
                {
                    return null;
                }
            }

            if (nameOrEmail.Contains("@"))
            {
                var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(nameOrEmail);
                customer = this.Repo.Query.Where(o => o.EmailHash == hash).FirstOrDefault();
                if (customer != null)
                {
                    var passwordhash = Lib.IOC.Service.GetSingleTon<IPasswordHash>();
                    bool loginok = passwordhash.Verify(password, customer.Password);
                    if (loginok)
                    {
                        //write the cookie. 
                        this.Context.Response.AppendCookie(Constants.CustomerCookieName, customer.Id.ToString(), DateTime.Now.AddDays(30));
                        return customer;
                    }

                }
            }
             

            return null;
        }

        public Customer CreatAccount(string UserName, string email, string password, string firstName, string LastName, string Telephone)
        {

            if (!IsUSerNameAvailable(UserName) || !IsEmailAddressAvailable(email))
            {
                var messge = Data.Language.Hardcoded.GetValue("Username or email address already exists", this.Context);
                throw new Exception(messge);
            } 

            Customer newcus = new Customer()
            {
                Name = UserName,
                EmailAddress = email,
                FirstName = firstName,
                LastName = LastName,
                Telephone = Telephone,
                Password = Service.PasswordService.Hash(password)
            };

            this.Repo.AddOrUpdate(newcus);

            return newcus;
        }

        public bool IsUSerNameAvailable(string username)
        {
            var find = this.Repo.Get(username);
            return find == null;
        }

        public bool IsEmailAddressAvailable(string emailaddress)
        {
            var hash = Lib.Security.Hash.ComputeGuidIgnoreCase(emailaddress);

            var find = this.Repo.Query.Where(o => o.EmailHash == hash).FirstOrDefault();

            return find == null;
        }

    }
}
