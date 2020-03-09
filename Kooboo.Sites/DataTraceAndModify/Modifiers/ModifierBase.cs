using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Dom;
using Kooboo.Dom.CSS;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{


    public abstract class ModifierBase
    {
        public enum ActionType
        {
            update,
            delete,
            copy,
            styleSheet
        }


        public abstract string Source { get; }

        public ActionType Action
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Selector)) return ActionType.styleSheet;

                switch (GetValue("action"))
                {
                    case "delete":
                        return ActionType.delete;
                    case "copy":
                        return ActionType.copy;
                    default:
                        return ActionType.update;
                }
            }
        }

        public KeyValuePair<string, string>[] Infos { get; set; }

        public string Value => GetValue("value");

        public string Id => GetValue("id");

        public string Selector => GetValue("selector");

        internal string GetCulture(RenderContext context)
        {
            string culture = context.Culture;

            if (string.IsNullOrEmpty(culture))
            {
                culture = context.WebSite.DefaultCulture;
            }

            return culture;
        }

        public string GetValue(string key)
        {
            var action = Infos.FirstOrDefault(f => f.Key == key);
            return action.Value;
        }

        public abstract void Modify(RenderContext context);
    }
}
