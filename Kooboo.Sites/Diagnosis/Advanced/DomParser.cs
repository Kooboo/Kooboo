//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Diagnosis.Advanced
{


    public class DomParser : IDiagnosis
    {
        public DiagnosisSession session { get; set; }

        public string Group(RenderContext context)
        {
            return Hardcoded.GetValue("Advanced", context);
        }

        public string Name(RenderContext context)
        {
            return Hardcoded.GetValue("Check html using Document structure", context);
        }

        public void Check()
        {
            var sitedb = this.session.context.WebSite.SiteDb();

            this.session.Headline = Hardcoded.GetValue("Checking", session.context) + " " + Hardcoded.GetValue("Dom structure", session.context) + "...";

            var allrepos = sitedb.ActiveRepositories();

            string name = Hardcoded.GetValue("Dom structure error", session.context);

            foreach (var repo in allrepos)
            {
                if (Lib.Reflection.TypeHelper.HasInterface(repo.ModelType, typeof(IDomObject)))

                {
                    var allitems = repo.All();

                    foreach (var item in allitems)
                    {
                        var domobj = item as DomObject;
                        if (domobj != null)
                        {
                            List<string> errors = new List<string>();

                            var dom = Kooboo.Dom.DomParser.CreateDom(domobj.Body, errors);

                            foreach (var err in errors)
                            {
                                string message = err; 
                                message += DiagnosisHelper.DisplayUsedBy(session.context, domobj as SiteObject);
                                session.AddMessage(name, message, MessageType.Warning);
                            }
                        }
                    }
                }
            }
        } 
         
    }




}



