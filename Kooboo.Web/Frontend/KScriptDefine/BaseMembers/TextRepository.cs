using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface TextRepository
    {
        [Description("add an item")]
        void Add(object obj);

        [Description("Return an array of the SiteObjects")]
        SiteObject[] All();

        [Description("Delete an item")]
        void Delete(object nameOrId);

        [Description("Get an item based on Name or Id")]
        SiteObject Get(object nameOrId);

        [Description("update the text object")]
        void Update(SiteObject siteObject);

        [Description("Update the text value of the body property")]
        void UpdateBody(object nameOrId, string newbody);
    }
}
