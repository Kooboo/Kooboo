//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using Kooboo.Data.Attributes;

namespace Kooboo.Data.Models
{
    public class User : IGolbalObject
    {
        public User()
        {
            this.ConstType = ConstObjectType.User;
        }

        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(UserName);
                }
                return _id;
            }
        }

        public Guid CurrentOrgId { get; set; }

        // redundant. 
        public string CurrentHostDomain { get; set; }
        // redundant
        public string CurrentOrgName { get; set; }

        // Is Admin of Current Organization. 
        public bool IsAdmin { get; set; }

        private string _username;
        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                _id = default(Guid);
            }
        }
        
        public string EmailAddress {get;set; }
        
        public string Password { get; set; }

        public Guid PasswordHash { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
     
        public string FullName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(FirstName) && String.IsNullOrWhiteSpace(LastName))
                {
                    return UserName;
                }
                return String.Concat(FirstName, " ", LastName);
            }
        }
   
        public byte ConstType { get; set; }
        
        private string _language;
        public string Language
        {
            get
            {
                if (string.IsNullOrEmpty(_language))
                {
                    return "en"; 
                }
                return _language;
            }
            set {
                string input = value; 
                if (!string.IsNullOrWhiteSpace(input))
                {
                    input = input.Trim(); 
                    if (input.Length> 2)
                    {
                        input = input.Substring(0, 2); 
                    }
                    
                    _language = input.ToLower(); 
                }

            }
        }

        public string RegisterIp { get; set; }

        
        public string  TempRedirectUrl { get; set; }

        public override int GetHashCode()
        {
            string unique = this.CurrentHostDomain + this.CurrentOrgId.ToString() + this.CurrentOrgName;
            unique += this.EmailAddress + this.FirstName + this.LastName + this.Language;
            unique += this.Password + this.PasswordHash.ToString(); 
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }
    }
}
