//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Sites.Scripting.Global.SiteItem
{
    public class UserModel
    {
        public UserModel(Kooboo.Data.Models.User user)
        {
            if (user != null)
            {
                this.UserName = user.UserName;
                this.FirstName = user.FirstName;
                this.Language = user.Language;
                this.LastName = user.LastName;
            }
        }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Language { get; set; }
    }
}