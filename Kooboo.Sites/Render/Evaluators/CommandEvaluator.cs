using Kooboo.Dom;
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
                    if ((options.IgnoreEvaluators & EnumEvaluator.LayoutCommand)== EnumEvaluator.LayoutCommand)
                    {
                        return null;
                    } 
                    var task = new CommandRenderTask(node as Comment);
                    task.ClearBefore = true;
                    result.Add(task);
                    response.AppendTask = result; 
                }
                else
                {
                    var task = new CommandRenderTask(node as Comment); 
                    result.Add(task);
                    response.ContentTask = result;
                }
           
                response.StopNextEvaluator = true;
                response.OmitTag = true;
                
                if (options.RequireBindingInfo)
                {
                    string boundary = Kooboo.Lib.Helper.StringHelper.GetUniqueBoundary();

                    var startbinding = new BindingObjectRenderTask()
                    { ObjectType = "Command", Boundary = boundary, NameOrId = command.Name };
                    List<IRenderTask> bindingstarts = new List<IRenderTask>();
                    bindingstarts.Add(startbinding);
                    response.BindingTask = bindingstarts;

                    var endbinding = new BindingObjectRenderTask()
                    { ObjectType = "Command", IsEndBinding = true, Boundary = boundary, NameOrId =command.Name };

                    List<IRenderTask> bindingends = new List<IRenderTask>();
                    bindingends.Add(endbinding);
                    response.EndBindingTask = bindingends;
                }
                return response;

            } 

            return null;
        }
    }

}
