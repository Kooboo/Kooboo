using Kooboo.Data.Context;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.Commands
{
  public  interface ICommand
    {
        string Name { get; }
         
        string Execute(RenderContext context, Dictionary<string, string> Paras);
         
    }
}
