//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Data.Language;
using Kooboo.Lib.Helper;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;

namespace Kooboo.Sites.Diagnosis.Implementation
{
    public class BrokenLink : IDiagnosis
    {
        public DiagnosisSession session { get; set; }

        public string Group(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Normal", context);
        }

        public string Name(RenderContext context)
        {
            return Hardcoded.GetValue("check all internal links to make sure that the resource did exists", context);
        }


        public void Check()
        {
            this.session.Headline = Data.Language.Hardcoded.GetValue("Scanning links", this.session.context);

            var context = this.session.context;
            var sitedb = this.session.context.WebSite.SiteDb();

            var allroutes = sitedb.Routes.Query.Where(o => o.DestinationConstType == ConstObjectType.Page).SelectAll();

            foreach (var item in allroutes)
            {
                if (item.objectId == default(Guid))
                {
                    var usedby = sitedb.Routes.GetUsedBy(item.Id);

                    var ImageDataUri = Kooboo.Lib.Utilities.DataUriService.isDataUri(item.Name);

                    if (!ImageDataUri)
                    {
                        string message = item.Name + "; " + DiagnosisHelper.DisplayUsedBy(session.context, usedby);

                        this.session.AddMessage(Hardcoded.GetValue("Missing link", context), message, MessageType.Critical);
                    }
                }
            }
        }

    }
}





