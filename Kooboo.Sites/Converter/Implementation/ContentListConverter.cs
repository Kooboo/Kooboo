//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Converter
{
    public class ContentListConverter : IConverter
    {
        public string Type
        {
            get
            {
                return "contentlist";
            }
        }

        public ConvertResponse Convert(RenderContext context, JObject result)
        {
            var sitedb = context.WebSite.SiteDb();

            string resultname = Lib.Helper.JsonHelper.GetString(result, "Name");
            if (string.IsNullOrEmpty(resultname))
            {
                resultname = "contentlist";
            }
            string type = Lib.Helper.JsonHelper.GetString(result, "ConvertToType");
            var name = ConvertManager.GetUniqueName(context, type, resultname);
            var data = Lib.Helper.JsonHelper.GetObject(result, "data");

            var res = DataManager.AddData(sitedb, name, data);

            View view = new View();
            view.Name = name;

            string viewbody = Lib.Helper.JsonHelper.GetString(result, "HtmlBody");

             view.Body = UpdateViewTemplate(viewbody, res); 
             
            sitedb.Views.AddOrUpdate(view);

            DataManager.AddGetContentListDataMethod(sitedb, view.Id, res.contentFolder.Id, "List");

            return new ConvertResponse()
            {
                IsSuccess = true,
                ComponentNameOrId = view.Name,
                Tag = "<view id='" + view.Name.ToString() + "'></view>"
            };

        }


        private bool HasCategory(SiteDb SiteDb, Guid PageId, string KoobooId)
        {
            return false;
        }

        public string UpdateViewTemplate(string template, DataAddResponse dataResponse)
        {
            if (dataResponse == null || dataResponse.DateList == null || !dataResponse.DateList.Any())
            {
                return template;
            }

            List<SourceUpdate> sourceUpdate = new List<SourceUpdate>(); 

            var doc = Kooboo.Dom.DomParser.CreateDom(template);
            var els = GetPossibleEls(doc);

            foreach (var el in els)
            {

                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var att in el.attributes)
                {
                    dict.Add(att.name, att.value);
                }

                Dictionary<string, string> updates = new Dictionary<string, string>(); 

                foreach (var at in dict)
                {
                    if (at.Key.ToLower() == "k-content" || at.Key.ToLower() == "k-replace")
                    {
                        var value = at.Value;
                        foreach (var date in dataResponse.DateList)
                        {
                            if (value == date.Name || value.EndsWith("." + date.Name))
                            {
                                var newvalue =  "DateFormat(" + at.Value + ", '" + date.Format + "')";
                                updates.Add(at.Key, newvalue); 
                            }
                        }
                    }
                }


                if (updates.Count()>0)
                {
                    foreach (var item in updates)
                    {
                        dict[item.Key] = item.Value; 
                    }

                    SourceUpdate supdate = new SourceUpdate();
                    supdate.StartIndex = el.location.openTokenStartIndex;
                    supdate.EndIndex = el.location.openTokenEndIndex;

                    string newopentag = Kooboo.Sites.Service.DomService.GenerateOpenTag(dict, el.tagName);
                    supdate.NewValue = newopentag;

                    sourceUpdate.Add(supdate); 

                }

            }

            if (sourceUpdate.Any())
            {
                return Kooboo.Sites.Service.DomService.UpdateSource(template, sourceUpdate); 
            }
            else
            {
                return template; 
            }
        }

        private List<Kooboo.Dom.Element> GetPossibleEls(Dom.Document doc)
        {
            List<Kooboo.Dom.Element> elements = new List<Dom.Element>();

            var contents = doc.getElementByAttribute("k-content");

            if (contents != null && contents.length > 0)
            {
                elements.AddRange(contents.item);
            }

            var replaces = doc.getElementByAttribute("k-replace");

            if (replaces != null && replaces.length > 0)
            {
                elements.AddRange(replaces.item);
            }

            return elements;
        }

    }
}
