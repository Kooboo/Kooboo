using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Dom;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.DataTraceAndModify
{


    public abstract class ModifierBase
    {
        public enum ActionType
        {
            update,
            delete,
            copy
        }

        public abstract string Source { get; }

        public ActionType Action
        {
            get
            {
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

        public string KoobooId => GetValue("koobooid");

        public string Value => GetValue("value");

        public string Attribute => GetValue("attribute");

        public string Id => GetValue("id");

        internal string GetNewDomBody(string oldDom, Element element)
        {
            SourceUpdate sourceUpdate = null;

            switch (Action)
            {
                case ActionType.copy:
                    sourceUpdate = new SourceUpdate
                    {
                        StartIndex = element.location.openTokenStartIndex,
                        EndIndex = element.location.endTokenEndIndex,
                        NewValue = element.OuterHtml + element.OuterHtml
                    };
                    break;
                case ActionType.delete:
                    sourceUpdate = new SourceUpdate
                    {
                        StartIndex = element.location.openTokenStartIndex,
                        EndIndex = element.location.endTokenEndIndex,
                        NewValue = string.Empty
                    };
                    break;
                default:
                    if (Value == null) break;
                    sourceUpdate = HandleDomUpdate(element, element);
                    break;
            }

            return Service.DomService.UpdateSource(oldDom, new List<SourceUpdate> { sourceUpdate });
        }

        private SourceUpdate HandleDomUpdate(Node node, Element element)
        {
            SourceUpdate sourceUpdate;
            if (string.IsNullOrWhiteSpace(Attribute))
            {
                sourceUpdate = new SourceUpdate
                {
                    StartIndex = node.location.openTokenEndIndex + 1,
                    EndIndex = node.location.endTokenStartIndex - 1,
                    NewValue = Value
                };
            }
            else
            {
                element.setAttribute(Attribute, Value);
                sourceUpdate = new SourceUpdate
                {
                    StartIndex = node.location.openTokenStartIndex,
                    EndIndex = node.location.openTokenEndIndex,
                    NewValue = Service.DomService.ReSerializeOpenTag(element)
                };

            }

            return sourceUpdate;
        }

        public string GetValue(string key)
        {
            var action = Infos.FirstOrDefault(f => f.Key == key);
            return action.Value;
        }

        public abstract void Modify(RenderContext context);

    }
}
