//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.DataTraceAndModify.CustomTraces;
using Kooboo.Sites.Render.RenderTask;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.Evaluators
{

    public class CommandEvaluator : IEvaluator
    {
        public EvaluatorResponse Evaluate(Node node, EvaluatorOption options)
        {
            if (node.nodeType != enumNodeType.COMMENT)
            {
                return null;
            }

            /// NOTO: There are two place for evaluating comment command. 
            if (Commands.CommandManager.IsCommand(node as Comment))
            {
                var response = new EvaluatorResponse();
                List<IRenderTask> result = new List<IRenderTask>();

                Guid OwnerObjectId = options.OwnerObjectId;

                var command = Commands.CommandParser.ParseCommand(node as Comment);

                if (command.Name.ToLower() == "layout")
                {
                    if ((options.IgnoreEvaluators & EnumEvaluator.LayoutCommand) == EnumEvaluator.LayoutCommand)
                    {
                        return null;
                    }
                    var task = new CommandRenderTask(node as Comment, options);
                    task.ClearBefore = true;
                    result.Add(task);
                    response.AppendTask = result;
                }
                else
                {
                    var task = new CommandRenderTask(node as Comment, options);
                    result.Add(task);
                    response.ContentTask = result;
                }

                response.StopNextEvaluator = true;
                response.OmitTag = true;

                if (options.RequireBindingInfo)
                {
                    if (response.BindingTask == null) response.BindingTask = new List<IRenderTask>();
                    var traceability = new ComponentTrace(command.Name, "command");
                    var bindingTask = new BindingRenderTask(traceability);
                    response.BindingTask.Add(bindingTask);
                    if (response.EndBindingTask == null) response.EndBindingTask = new List<IRenderTask>();
                    response.EndBindingTask.Add(bindingTask.BindingEndRenderTask);
                }
                return response;

            }

            return null;
        }
    }

}
