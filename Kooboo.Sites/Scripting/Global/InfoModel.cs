using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using KScript;
using KScript.Sites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Sites.Scripting.Global
{
    public class InfoModel
    { 
        public InfoModel(RenderContext OrgContext)
        {
            this.context = OrgContext; 
        }

        public string Culture
        { 
            get
            {
                return this.context.Culture;
            }
        }

        [Description("WebSite name")]
        public string Name {
            get
            {
                return this.context.WebSite.Name; 
            } 
        }

        private KDictionary _setting;
        public KDictionary Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = new KDictionary(this.context.WebSite.CustomSettings);
                }
                return _setting;
            }
            set { _setting = value; }
        }

        private UserModel _user;
        public UserModel User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserModel(this.context.User);
                }
                return _user;
            }
            set { _user = value; } 
        }

        public string BaseUrl {

            get
            {
                return this.context.WebSite.BaseUrl(); 
            }

        }

        private RenderContext context { get; set; }

        public long Version
        { 
            get
            {
                var db = this.context.WebSite.SiteDb().DatabaseDb;

                return db.Log.Store.LastKey; 
            } 
        }

        private List<string> _domains; 
        public List<string> Domains
        {
            get
            {
                if (_domains == null)
                {
                    _domains = new List<string>();

                    if (this.context != null && this.context.WebSite != null)
                    {
                        var list = GlobalDb.Bindings.GetByWebSite(this.context.WebSite.Id);

                        foreach (var item in list)
                        {
                            _domains.Add(item.FullName);
                        }
                    }
                }  
                return _domains;
            }
        }


        public void EnableSsl(string fulldomain)
        {
            if (this.context.WebSite != null)
            {
                Kooboo.Data.SSL.SslService.SetSsl(fulldomain, this.context.WebSite.OrganizationId);
            }
        }

        public void SetDomain(string fulldomain)
        {
            GlobalDb.Bindings.AddOrUpdate(fulldomain, this.context.WebSite.Id, this.context.WebSite.OrganizationId);
        }

    }
}
