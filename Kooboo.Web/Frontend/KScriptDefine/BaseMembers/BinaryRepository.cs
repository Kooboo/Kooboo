using Kooboo.Data.Interface;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Kooboo.Web.Frontend.KScriptDefine.BaseMembers
{
    public interface BinaryRepository
    {

        [Description("Return an array of the SiteObjects")]
        ISiteObject[] All();

        [Description("Delete an item")]
        void Delete(object nameOrId);

        [Description("Get an item based on Name or Id")]
        ISiteObject Get(object nameOrId);

        [Description("update the text object")]
        void Update(ISiteObject siteObject);

        [Description("	Update the binary content")]
        void UpdateBinary(object nameOrId, string newbody);
    }
}
