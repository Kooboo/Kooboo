using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Contents.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kooboo.Sites.DataTraceAndModify.Modifiers
{
    public class TextContentModifier : ModifierBase
    {
        public override string Source => "textcontent";

        public string Path => GetValue("path");

        public string NewId => GetValue("new");

        public override void Modify(RenderContext context)
        {
            if (Id == null) return;
            var repo = context.WebSite.SiteDb().TextContent;
            var textContent = repo.GetByNameOrId(Id);
            if (textContent == null) return;
            var culture = GetCulture(context);

            switch (Action)
            {
                case ActionType.update:
                    if (Path == null) return;
                    textContent.SetValue(Path, Value, culture);
                    repo.AddOrUpdate(textContent, context.User.Id);
                    break;
                case ActionType.delete:
                    repo.Delete(textContent.Id);
                    break;
                case ActionType.copy:
                    var cloned = textContent.Clone<TextContent>();
                    cloned.UserKey = NewId;
                    repo.AddOrUpdate(cloned, context.User.Id);
                    break;
                default:
                    break;
            }
        }

    }
}
