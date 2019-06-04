//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{
    public class SiteUser : SiteObject
    {
        public SiteUser()
        {
            this.ConstType = ConstObjectType.SiteUser;
        }

        public override Guid Id
        {
            get
            {
                return UserId;
            }
            set
            {
                this.UserId = value;
            }
        }

        public Guid UserId { get; set; }

        [Obsolete]
        [JsonConverter(typeof(StringEnumConverter))]
        public Kooboo.Sites.Authorization.EnumUserRole Role { get; set; }



        private string _siterole;
        public string SiteRole
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_siterole))
                {
                    if (this.Role == Authorization.EnumUserRole.SiteMaster || this.Role == Authorization.EnumUserRole.Administrator)
                    {
                        _siterole = Kooboo.Sites.Authorization.DefaultData.Master.Name;
                    }
                    else if (this.Role == Authorization.EnumUserRole.Developer)
                    {
                        _siterole = Kooboo.Sites.Authorization.DefaultData.Developer.Name;
                    }
                    else if (this.Role == Authorization.EnumUserRole.ContentManager)
                    {
                        _siterole = Kooboo.Sites.Authorization.DefaultData.ContentManager.Name;
                    }
                    else
                    {
                        _siterole = Kooboo.Sites.Authorization.DefaultData.Master.Name;
                    }
                }
                return _siterole;
            }
            set
            {
                _siterole = value; 
            }
         }

        public override int GetHashCode()
        {
            return Lib.Security.Hash.ComputeInt(this.SiteRole);
        }
    }
}
