//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
  
namespace Kooboo.Data.Template
{  
    public class TemplateDataModel
    {
        public byte[] Bytes { get; set; }

        private Guid _bodyhash; 

        public Guid ByteHash {
            get {
                if (_bodyhash == default(Guid) && Bytes != null)
                {
                    _bodyhash = Lib.Security.Hash.ComputeGuid(Bytes); 
                }
                return _bodyhash; 
            }
            set { _bodyhash = value;  }
        }

        public string Name { get; set; }
    
        public string Link { get; set; }

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

        public List<TemplateUserImages> Images { get; set; } = new List<TemplateUserImages>();

    }

    public class TemplateUpdateModel
    {
        public Guid Id { get; set; }
        public string Category { get; set; }

        public Guid UserId { get; set; }

        public string Link { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }

        // json images. 
        public string Images { get; set; }

        public string NewDefault { get; set; }

        public List<TemplateUserImages> NewImages { get; set; } = new List<TemplateUserImages>();

        public Guid OrganizationId { get; set; }

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

        public decimal Price { get; set; } = 0;

        public string Currency { get; set; } = "CNY";

        public string Symbol
        {
            get
            {
                return Kooboo.Lib.Helper.CurrencyHelper.GetCurrencySymbol(Currency);
            }
        }
    }

    public class TemplateUserImages
    {
        public string FileName { get; set; }
        public string Base64 { get; set; }
        public bool IsDefault { get; set; }
    }

}
