//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Web;
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Diagnosis.Implementation
{
    public class DuplicateId : IDiagnosis
    { 
        public DiagnosisSession session { get; set; }

        public void Check()
        { 
            string checking = Hardcoded.GetValue("Checking", this.session.context);
            string name = Hardcoded.GetValue("duplicate id", this.session.context);

            session.Headline = checking + " " + name; 
             
            var sitedb = session.context.WebSite.SiteDb();

            foreach (var repo in sitedb.ActiveRepositories())
            {
                if (Lib.Reflection.TypeHelper.HasInterface(repo.ModelType, typeof(IDomObject)))
                {
                    var allitems = repo.All();

                    foreach (var item in allitems)
                    {
                        var domitem = item as IDomObject; 

                        var dom = domitem.Dom;

                        var ids = new HashSet<string>();
                        var allidelements = dom.getElementByAttribute("id");

                        foreach (var el in allidelements.item)
                        {
                            var id = el.id;
                            if (!string.IsNullOrEmpty(id))
                            {
                                if (ids.Contains(id))
                                {
                                    string opentag = HttpUtility.HtmlEncode(Service.DomService.GetOpenTag(el));
                                    var message = id + " " + opentag;

                                    message += DiagnosisHelper.DisplayUsedBy(session.context, item as SiteObject); 
                              
                                    session.AddMessage(name, message, MessageType.Critical);
                                }
                                else
                                {
                                    ids.Add(id);
                                }
                            }
                        } 
                    }
                } 
            }  
        }
         
        public string Group(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("Normal", context);
        }

        public string Name(RenderContext context)
        {
            return Data.Language.Hardcoded.GetValue("check duplicate id in html", context);
        }
    }
}
 