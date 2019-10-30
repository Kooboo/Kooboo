//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.InlineEditor.Model;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor.Executor
{
    public class LabelExecutor : IInlineExecutor
    {
        public string EditorType
        {
            get
            {
                return "label";
            }
        }

        public void Execute(RenderContext context, List<IInlineModel> updatelist)
        {
            foreach (var item in updatelist)
            {
                if (!(item is LabelModel labelupate))
                {
                    continue;
                }
                if (labelupate.Action == ActionType.Add)
                {
                    Kooboo.Sites.Contents.Models.Label label = new Contents.Models.Label {Name = labelupate.NameOrId};
                    label.SetValue(context.Culture, labelupate.Value);
                    context.WebSite.SiteDb().Labels.AddOrUpdate(label, context.User.Id);
                }
                else if (labelupate.Action == ActionType.Update)
                {
                    var label = context.WebSite.SiteDb().Labels.GetByNameOrId(labelupate.NameOrId);
                    if (label != null)
                    {
                        label.SetValue(context.Culture, labelupate.Value);
                        context.WebSite.SiteDb().Labels.AddOrUpdate(label, context.User.Id);
                    }
                }
                else if (labelupate.Action == ActionType.Delete)
                {
                    var label = context.WebSite.SiteDb().Labels.GetByNameOrId(labelupate.NameOrId);
                    if (label != null)
                    {
                        context.WebSite.SiteDb().Labels.Delete(label.Id, context.User.Id);
                    }
                }
            }
        }

        public void ExecuteObject(RenderContext context, IRepository repo, string nameOrId, List<IInlineModel> updates)
        {
            throw new NotImplementedException();
        }
    }
}