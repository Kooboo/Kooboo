//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{
    public class AppDataModel
    {
        public byte[] Bytes { get; set; }

        private Guid _bodyhash;

        public Guid ByteHash
        {
            get
            {
                if (_bodyhash == default(Guid) && Bytes != null)
                {
                    _bodyhash = Lib.Security.Hash.ComputeGuid(Bytes);
                }
                return _bodyhash;
            }
            set { _bodyhash = value; }
        }

        public string Link { get; set; }

        public string Name { get; set; }

        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

        public decimal Price { get; set; } = 0;

        public string Currency { get; set; } = "CNY";

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }

        public List<PackageImages> Images { get; set; } = new List<PackageImages>();

    }

    public class PackageImages
    {
        public string FileName { get; set; }
        public string Base64 { get; set; }
        public bool IsDefault { get; set; }
    }
}
