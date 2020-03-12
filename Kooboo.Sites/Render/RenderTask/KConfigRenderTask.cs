//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.DataTraceAndModify;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.RenderTask
{

    public class KConfigRenderTask : IRenderTask
    {
        private string Key { get; set; }

        private bool IsSelfClose { get; set; }

        private string OrgHtml { get; set; }

        public List<KConfigValues> RenderValues { get; set; }

        public List<KConfigTrace> KConfigTraces { get; set; } = new List<KConfigTrace>();

        public bool ClearBefore
        {
            get
            {
                return false;
            }
        }

        public KConfigRenderTask(Kooboo.Dom.Element configEl, string key)
        {
            this.Key = key;
            this.RenderValues = new List<KConfigValues>();

            this.OrgHtml = configEl.OuterHtml;

            this.IsSelfClose = Service.DomService.IsSelfCloseTag(configEl.tagName);

            string header = "<" + configEl.tagName;

            RenderValues.Add(new KConfigValues() { Value = header });

            foreach (var item in configEl.attributes)
            {
                if (item.name == "k-config")
                {
                    continue;
                }

                if (IsIgnoreAttribute(item.name))
                {
                    string attHtml = " " + item.name;
                    if (!string.IsNullOrEmpty(item.value))
                    {
                        attHtml += "=\"" + item.value + "\"";
                    }
                    RenderValues.Add(new KConfigValues() { Value = attHtml });
                }
                else
                {
                    string start = " " + item.name;
                    start += "=\"";
                    RenderValues.Add(new KConfigValues() { Value = start });
                    RenderValues.Add(new KConfigValues() { Key = item.name, Value = item.value, IsBinding = true });
                    RenderValues.Add(new KConfigValues() { Value = "\"" });
                    KConfigTraces.Add(new KConfigTrace(key, item.name));
                }
            }

            if (IsSelfClose)
            {
                RenderValues.Add(new KConfigValues() { Value = " />" });
            }
            else
            {
                RenderValues.Add(new KConfigValues() { Value = ">" });
                RenderValues.Add(new KConfigValues() { Key = "innerHtml", Value = configEl.InnerHtml, IsBinding = true });
                string endtag = "</" + configEl.tagName + ">";
                RenderValues.Add(new KConfigValues() { Value = endtag });
                KConfigTraces.Add(new KConfigTrace(key, null));
            }
        }

        public string Render(RenderContext context)
        {
            var config = context.WebSite.SiteDb().KConfig.GetByNameOrId(this.Key);

            if (config != null)
            {
                return Render(config);
            }
            return this.OrgHtml;
        }

        public string Render(KConfig config)
        {
            string result = string.Empty;

            foreach (var item in this.RenderValues)
            {
                result += item.GetValue(config);
            }
            return result;
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            result.Add(new RenderResult() { Value = Render(context) });
        }

        public bool IsIgnoreAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            string lower = name.ToLower();

            if (lower.StartsWith("on") || lower.Contains("-") || lower.Contains("_") || lower == "style")
            {
                return true;
            }
            return false;
        }

    }


    public class KConfigValues
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public bool IsBinding { get; set; }

        public string GetValue(KConfig config)
        {
            if (IsBinding)
            {
                if (config.Binding.ContainsKey(Key))
                {
                    return config.Binding[Key];
                }
            }
            return Value;
        }
    }

}
