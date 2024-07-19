//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail
{
    public class TargetAddress : IMailObject
    {
        private int _id;
        public int Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(int))
                {
                    _id = ToId(Address);
                }
                return _id;
            }
        }

        public string Name { get; set; }

        public string Address { get; set; }

        public static int ToId(string address)
        {
            return Lib.Security.Hash.ComputeInt(address);
        }
    }
}
