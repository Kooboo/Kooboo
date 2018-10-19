//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Data.Models
{

    /// <summary>
    /// The group or organization that this user belongs to.
    /// This can be the company name, or school, or other organizations. 
    /// </summary>
    public class Organization : IGolbalObject
    {
        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = IDGenerator.GetId(this.Name);
                } 
                return _id;
            }
        }

        // unique name... will be used as key.. same as the first user's username. 
        private string _name;
        public string Name
        {
            get
            { return _name; }
            set
            { _name = value; _id = default(Guid);  }
        }

        public string DisplayName { get; set; }

        public Guid AdminUser { get; set; }

        public decimal Balance { get; set; } = 0;

        public string Currency { get; set; } = "CNY";

        public int ServerId { get; set; }

        public int ServiceLevel { get; set; }

        public bool IsBanned { get; set; }

        public override int GetHashCode()
        {
            string unique = this.DisplayName + this.Balance.ToString();
            unique += this.ServerId.ToString() + this.ServiceLevel.ToString();
            unique += this.AdminUser.ToString() + this.IsBanned.ToString(); 

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
}
