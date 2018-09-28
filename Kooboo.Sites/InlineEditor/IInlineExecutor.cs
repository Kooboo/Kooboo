using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.InlineEditor
{
   public interface IInlineExecutor
    {
         string EditorType
        { get;  }

        void Execute(RenderContext context, List<IInlineModel> updatelist);
        void ExecuteObject(RenderContext context, IRepository repo, string NameOrId, List<IInlineModel> updates); 
    }
}
