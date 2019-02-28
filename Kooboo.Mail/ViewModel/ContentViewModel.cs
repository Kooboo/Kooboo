//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.ViewModel
{
   
        public class ContentViewModel
        {
            public int Id { get; set; }

            public string Subject { get; set; }

            public AddressModel From { get; set; }

            public List<AddressModel> To { get; set; }

            public List<AddressModel> Cc { get; set; }

            public List<AddressModel> Bcc { get; set; }

            public List<Models.Attachment>  Attachments { get; set; }

            public string Html { get; set; }

            public DateTime Date { get; set; }
        }

        public class AddressModel
        {
            public string Name { get; set; }

            public string Address { get; set; }
        }
    
}
