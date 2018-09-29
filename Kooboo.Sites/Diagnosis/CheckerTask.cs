//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Diagnosis
{
 public   class CheckerTask
    {

        public DiagnosisSession Session { get; set; }


        public void Exe()
        {
            foreach (var item in Session.AllCheckers)
            {
                Session.Current = item; 
                if (!item.IsCode)
                {
                    try
                    {
                        var instance = Activator.CreateInstance(item.Type) as IDiagnosis;

                        instance.session = Session;
                        instance.Check();
                    }
                    catch (Exception ex)
                    { 
                        
                    
                    } 
                }
                else
                { 

                    var sitedb = Session.context.WebSite.SiteDb();
                    
                    var code = sitedb.Code.Get(item.Name); 
                                        
                    if (code !=null)
                    {
                        var kk = Kooboo.Sites.Scripting.Manager.GetOrSetK(this.Session.context);
                        kk.diagnosis = new KDiagnosis(this.Session); 

                        Kooboo.Sites.Scripting.Manager.ExecuteCode(this.Session.context, code.Body, code.Id);

                        kk.diagnosis = null; 
                    }
                    
                }
            }

            Session.IsFinished = true; 
        }
    }
}
