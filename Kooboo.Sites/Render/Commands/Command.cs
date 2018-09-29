//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
 
using System.Collections.Generic;
  
namespace Kooboo.Sites.Render.Commands
{
  public  class Command
    {
        public string Name { get; set; }

        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>(); 
    }
}
