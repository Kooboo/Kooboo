using System;

namespace Kooboo.Mail.Models
{
    public class AddressBook : IMailObject
    {
        private int _id;
        public int Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(int))
                {
                    if (string.IsNullOrWhiteSpace(Address))
                    {
                        return 0;
                    }
                    _id = Lib.Security.Hash.ComputeInt(Address);
                }
                return _id;
            }
        }

        public Guid UserId { get; set; }

        public string FullAddress { get; set; }

        private string _address;
        public string Address
        {
            get; set;
        }

        public string Name { get; set; }

        public override int GetHashCode()
        {
            string unique = this.FullAddress + this.Name + this.Address;
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }
    }
}
