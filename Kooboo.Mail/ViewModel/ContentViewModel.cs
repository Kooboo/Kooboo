//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using static Kooboo.Mail.Utility.ICalendarUtility;

namespace Kooboo.Mail.ViewModel
{

    public class ContentViewModel
    {
        public int Id { get; set; }

        public string Subject { get; set; }
        public string FolderName { get; set; }

        public AddressModel From { get; set; }

        public List<AddressModel> To { get; set; }

        public List<AddressModel> Cc { get; set; }

        public List<AddressModel> Bcc { get; set; }

        // public List<Models.Attachment>  Attachments { get; set; }

        public List<ViewModel.AttachmentViewModel> Attachments { get; set; }

        public string Html { get; set; }

        public string DownloadAttachment { get; set; }

        public DateTime Date { get; set; }

        public int InviteConfirm { get; set; }

        public List<ICalendarViewModel> Calendar { get; set; }
    }

    public class AddressModel
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }

}
