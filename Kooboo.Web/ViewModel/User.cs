//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Web.ViewModel
{


    public class SimpleUser
    {
        public SimpleUser(Kooboo.Data.Models.User user)
        {
            if (user != null)
            {
                this.Id = user.Id;
                this.CurrentOrgId = user.CurrentOrgId;
                this.CurrentOrgName = user.CurrentOrgName;
                this.UserName = user.UserName;
                this.EmailAddress = user.EmailAddress;
                this.Language = user.Language;
            }
        }

        public Guid Id { get; set; }

        public Guid CurrentOrgId { get; set; }

        // redundant
        public string CurrentOrgName { get; set; }


        public string UserName
        {
            get; set;
        }

        public string EmailAddress { get; set; }

        public string Language
        {
            get; set;
        }

        public string redirectUrl { get; set; }

    }
}
