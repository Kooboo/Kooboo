//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KScript.Sites
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
