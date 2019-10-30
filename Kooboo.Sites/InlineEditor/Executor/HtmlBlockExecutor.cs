//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor.Executor
{
    public class HtmlBlockExecutor : IInlineExecutor
    {
        public string EditorType
        {
            get
            {
                return "htmlblock";
            }
        }

        public void Execute(RenderContext context, List<IInlineModel> updatelist)
        {
            foreach (var item in updatelist)
            {
                var blockupdate = item as Model.HtmlblockModel;
                if (blockupdate == null)
                {
                    continue;
                }
                switch (blockupdate.Action)
                {
                    case ActionType.Add:
                    {
                        Kooboo.Sites.Contents.Models.HtmlBlock block = new Contents.Models.HtmlBlock
                        {
                            Name = blockupdate.NameOrId
                        };
                        block.SetValue(context.Culture, blockupdate.Value);
                        context.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(block, context.User.Id);
                        break;
                    }
                    case ActionType.Update:
                    {
                        var block = context.WebSite.SiteDb().HtmlBlocks.GetByNameOrId(blockupdate.NameOrId);
                        if (block != null)
                        {
                            block.SetValue(context.Culture, blockupdate.Value);
                            context.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(block, context.User.Id);
                        }

                        break;
                    }
                    case ActionType.Delete:
                    {
                        var block = context.WebSite.SiteDb().HtmlBlocks.GetByNameOrId(blockupdate.NameOrId);
                        if (block != null)
                        {
                            context.WebSite.SiteDb().HtmlBlocks.Delete(block.Id, context.User.Id);
                        }

                        break;
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