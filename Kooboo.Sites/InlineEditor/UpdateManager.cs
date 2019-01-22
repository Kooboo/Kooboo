using Kooboo.Data.Context;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.InlineEditor
{
    public static class UpdateManager
    {
        public static void Execute(RenderContext context, List<IInlineModel> updatelist)
        {
            foreach (var item in updatelist.GroupBy(o => o.EditorType))
            {
                Execute(context, item.Key, item.ToList());  
            }
        }

        public static void Execute(RenderContext context, string editortype, List<IInlineModel> updates)
        {
            var executor = EditorContainer.GetExecutor(editortype);
            executor.Execute(context, updates); 
        }
    }
}
