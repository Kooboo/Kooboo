//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;
using System;
using System.Linq;

namespace Kooboo.Sites.Diagnosis.Implementation
{
    public class MissingImage : IDiagnosis
    {

        public DiagnosisSession session { get; set; }

        public string Group(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Normal", context);
        }

        public string Name(RenderContext context)
        {
            return Hardcoded.GetValue("Missing images", context);
        }


        public void Check()
        {
            var sitedb = session.context.WebSite.SiteDb();

            string name = Hardcoded.GetValue("missing image", session.context);

            session.Headline = Hardcoded.GetValue("Checking", session.context) + " " + name;

            var allroutes = sitedb.Routes.Query.Where(o => o.DestinationConstType == ConstObjectType.Image).SelectAll();

            foreach (var item in allroutes)
            {
                if (item.objectId == default(Guid))
                {
                    string message = item.Name;

                    var usedby = sitedb.Routes.GetUsedBy(item.Id);

                    if (usedby.Any())
                    { 
                        message += DiagnosisHelper.DisplayUsedBy(session.context, usedby); 
                        session.AddMessage(Hardcoded.GetValue("Missing image", this.session.context), message, MessageType.Critical);
                    }
                }
            }
        }

    }
}