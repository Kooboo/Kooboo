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
        private Command Command { get; set; }

        public bool ClearBefore
        {
            get;
            set;
        }

        public CommandRenderTask(string commentline)
        {
            this.Command = CommandParser.ParseCommand(commentline);
        }

        public CommandRenderTask(Comment comment)
        {
            this.Command = CommandParser.ParseCommand(comment);
        }

        public string Render(RenderContext context)
        {
            if (!context.HasItem<ICommandSourceProvider>("commandsource"))
            {
                var sourceprovider = new DBCommandSourceProvider();
                context.SetItem<ICommandSourceProvider>(sourceprovider, "commandsource");
            }

            var command = CommandManager.GetCommand(this.Command.Name);
            return command?.Execute(context, this.Command.Attributes);
        }

        public void AppendResult(RenderContext context, List<RenderResult> result)
        {
            throw new NotImplementedException();
        }
    }
}