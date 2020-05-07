//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System; 

namespace Kooboo.Data.Models
{
    public class User : IGolbalObject
    { 
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
         
        // redundant
        public string CurrentOrgName { get; set; }

        private bool _isadmin; 
        public bool IsAdmin {
            get {
                if (_isadmin)
                {
                    return true; 
                }
                return this.Id == this.CurrentOrgId; 
            }
            set {
                _isadmin = value; 
            }
        }

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

        private string _emailaddress; 
        public string EmailAddress {
            get
            {
                return _emailaddress; 
            }
            set
            {
                _emailaddress = value;
                _emailId = default(Guid); 
            }

        }

        private Guid _emailId; 
        public Guid EmailId
        {
            get {
                if (_emailId == default(Guid))
                {
                    if (!string.IsNullOrWhiteSpace(EmailAddress))
                    {
                        _emailId = Lib.Security.Hash.ComputeGuidIgnoreCase(EmailAddress); 
                    }
                }
                return _emailId; 
            }
            set
            {
                _emailId = value; 
            }
        }

        public bool IsEmailVerified { get; set; }
        
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
         
       // public string TempServerIp { get; set; }

        private DateTime _registerdate; 

        public DateTime RegistrationDate {
            get
            {
                if (_registerdate == default(DateTime))
                {
                    _registerdate = DateTime.Now; 
                }
                return _registerdate; 
            }
            set
            {
                _registerdate = value; 
            } 
        }

        public string Tel { get; set; }

        public override int GetHashCode()
        {
            string unique =  this.CurrentOrgId.ToString() + this.CurrentOrgName;
            unique += this.EmailAddress + this.FirstName + this.LastName + this.Language;
            unique += this.Password + this.PasswordHash.ToString();
            unique += this.IsEmailVerified.ToString();
            unique += this.EmailId.ToString() + this.Tel; 
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  
        }
    }
}
