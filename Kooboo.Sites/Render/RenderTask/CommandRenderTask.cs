//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Dom;
using Kooboo.Sites.Render.Commands;
using System;
using System.Collections.Generic;
  
namespace Kooboo.Sites.Render.RenderTask
{
    public class CommandRenderTask : IRenderTask
    {
        private Command command { get; set; }
        private EvaluatorOption options { get; set; }

        public bool ClearBefore
        {
            get; 
            set; 
        }

        public CommandRenderTask(string commentline)
        {
            this.command = CommandParser.ParseCommand(commentline);
        }
        public CommandRenderTask(Comment comment, EvaluatorOption options)
        {
            this.command = CommandParser.ParseCommand(comment);
            this.options = options; 
        }

        public string Render(RenderContext context)
        { 
           if (!context.HasItem<ICommandSourceProvider>("commandsource"))
            {
                var sourceprovider = new DBCommandSourceProvider();
                context.SetItem<ICommandSourceProvider>(sourceprovider, "commandsource"); 
            }

            var command = CommandManager.GetCommand(this.command.Name);
            if (command != null)
            {
                return command.Execute(context, this.command.Attributes, this.options);
            }
            return null;
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            throw new NotImplementedException();
        }
    }
}
