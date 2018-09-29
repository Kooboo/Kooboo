using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;
using Kooboo.Sites.InlineEditor.Model;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Interface;

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
                var labelupate = item as LabelModel;
                if (labelupate == null)
                {
                    continue; 
                }
                if (labelupate.Action == ActionType.Add)
                {
                    Kooboo.Sites.Contents.Models.Label label = new Contents.Models.Label();
                    label.Name = labelupate.NameOrId;
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

        public void ExecuteObject(RenderContext context, IRepository repo, string NameOrId, List<IInlineModel> updates)
        {
            throw new NotImplementedException();
        }

    }
}
