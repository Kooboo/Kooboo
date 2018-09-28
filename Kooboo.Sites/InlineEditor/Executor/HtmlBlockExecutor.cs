using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.Extensions;

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
                if (blockupdate ==null)
                {
                    continue; 
                }
                if (blockupdate.Action == ActionType.Add)
                {
                    Kooboo.Sites.Contents.Models.HtmlBlock block = new Contents.Models.HtmlBlock();
                    block.Name = blockupdate.NameOrId;
                    block.SetValue(context.Culture, blockupdate.Value);
                    context.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(block, context.User.Id);
                }
                else if (blockupdate.Action == ActionType.Update)
                {
                    var block = context.WebSite.SiteDb().HtmlBlocks.GetByNameOrId(blockupdate.NameOrId);
                    if (block != null)
                    {
                        block.SetValue(context.Culture, blockupdate.Value);
                        context.WebSite.SiteDb().HtmlBlocks.AddOrUpdate(block, context.User.Id);
                    }
                }
                else if (blockupdate.Action == ActionType.Delete)
                {
                    var block = context.WebSite.SiteDb().HtmlBlocks.GetByNameOrId(blockupdate.NameOrId);
                    if (block != null)
                    {
                        context.WebSite.SiteDb().HtmlBlocks.Delete(block.Id, context.User.Id);
                    }
                }

            }

        }

        public void ExecuteObject(RenderContext context, IRepository repo, string NameOrId, List<IInlineModel> updates)
        {
            throw new NotImplementedException();
        }
    }
}
