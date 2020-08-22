//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using Kooboo.Sites.Render.RenderTask;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Kooboo.Sites.Render
{
    public static class RenderEvaluator
    {
        /// <summary>
        /// Evaluate the to be rendered HTML into a list of RenderTask. 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<IRenderTask> Evaluate(string html, EvaluatorOption options)
        {
            if (options == null)
            {
                options = new EvaluatorOption();
            }

            List<IRenderTask> tasklist = new List<IRenderTask>();
            List<IRenderTask> appendtask = new List<IRenderTask>();

            if (string.IsNullOrEmpty(html))
            {
                return tasklist;
            }
            var doc = DomParser.CreateDom(html);
            int currentindex = 0;
            int totallen = html.Length;

            // handle comment in the top.  
            foreach (var item in doc.childNodes.item)
            {
                if (item is Comment && Kooboo.Sites.Render.Commands.CommandManager.IsCommand(item as Comment))
                {
                    var command = Kooboo.Sites.Render.Commands.CommandParser.ParseCommand(item as Comment);

                    if (command.Name.ToLower() == "layout")
                    {
                        if ((options.IgnoreEvaluators & EnumEvaluator.LayoutCommand) == EnumEvaluator.LayoutCommand)
                        {
                            continue;
                        }
                    }

                    var comment = item as Comment;
                    if (comment != null)
                    {
                        if (comment.location.endTokenEndIndex <= doc.documentElement.location.openTokenStartIndex)
                        {
                            tasklist.Add(new ContentRenderTask(doc.HtmlSource.Substring(currentindex,
                         comment.location.openTokenStartIndex - currentindex)));

                            var commandtask = new CommandRenderTask(comment, options);
                            if (command.Name.ToLower() == "layout")
                            {
                                commandtask.ClearBefore = true;
                                appendtask.Add(commandtask);
                            }
                            else
                            {
                                tasklist.Add(commandtask);
                            }

                            currentindex = comment.location.endTokenEndIndex + 1;
                        }
                        else
                        {
                            var commandtask = new CommandRenderTask(comment, options);
                            if (command.Name.ToLower() == "layout")
                            {
                                commandtask.ClearBefore = true;
                                appendtask.Add(commandtask);
                            }
                            else
                            {
                                tasklist.Add(commandtask);
                            }
                        }
                    }
                }
            }

            var iterator = doc.createNodeIterator(doc.documentElement, enumWhatToShow.ELEMENT | enumWhatToShow.COMMENT, null);

            var nextnode = iterator.nextNode();

            List<EvaluatorResponse> responseList = new List<EvaluatorResponse>();

            List<IEvaluator> Evaluator;
            if (options.Evaluators != null)
            {
                Evaluator = options.Evaluators;
            }
            else
            {
                Evaluator = EvaluatorContainer.DefaultList;
            }

            while (nextnode != null)
            {
                if (ShouldTryRender(nextnode))
                {

                    foreach (var item in Evaluator)
                    {
                        var response = item.Evaluate(nextnode, options);
                        if (response != null)
                        {
                            responseList.Add(response);
                            if (response.StopNextEvaluator)
                            {
                                break;
                            }
                        }
                    }
                    int len = nextnode.location.openTokenStartIndex - currentindex;
                    //document parse error,may cause nextnode openTokenStartIndex less than currentindex.
                    //then get repeated content
                    if (responseList.Count() > 0 && len >= 0)
                    {
                        var element = nextnode as Element;
                        bool IsSelfClosed = element == null ? false : Service.DomService.IsSelfCloseTag(element.tagName);
                        bool OmitTag = responseList.Any(o => o.OmitTag);


                        if (len > 0)
                        {
                            tasklist.Add(new ContentRenderTask(doc.HtmlSource.Substring(currentindex, len)));
                        }
                        var bindings = GetBinding(responseList);
                        if (bindings != null)
                        {
                            tasklist.AddRange(bindings);
                        }
                        var attributes = GetAttribute(responseList);
                        var contenttask = GetContent(responseList);

                        if (IsSelfClosed)
                        {
                            if (attributes != null && !OmitTag)
                            {
                                tasklist.Add(new ContentRenderTask(GetHalfOpenTag(element)));
                                tasklist.AddRange(attributes);
                                tasklist.Add(new ContentRenderTask("/>"));
                            }
                            else
                            {
                                if (contenttask != null)
                                {
                                    tasklist.AddRange(contenttask);
                                }
                            }
                        }
                        else
                        {
                            if (attributes != null && !OmitTag)
                            {
                                tasklist.Add(new ContentRenderTask(GetHalfOpenTag(element)));
                                tasklist.AddRange(attributes);
                                tasklist.Add(new ContentRenderTask(">"));
                            }
                            else
                            {
                                if (!OmitTag)
                                {
                                    tasklist.Add(new ContentRenderTask(GetHalfOpenTag(element) + ">"));
                                }
                            }

                            if (contenttask != null)
                            {
                                tasklist.AddRange(contenttask);
                            }

                            if (!OmitTag && contenttask != null)
                            {
                                tasklist.Add(new ContentRenderTask("</" + element.tagName + ">"));
                            }
                        }

                        var endbinding = GetEndBinding(responseList);
                        if (endbinding != null)
                        {
                            tasklist.AddRange(endbinding);
                        }

                        var append = GetAppend(responseList);
                        if (append != null)
                        {
                            appendtask.AddRange(append);
                        }

                        if (IsFakeHeader(element))
                        {
                            currentindex = nextnode.location.openTokenStartIndex;
                            nextnode = iterator.NextSibling(nextnode);
                        }
                        else if (contenttask != null)
                        {
                            currentindex = nextnode.location.endTokenEndIndex + 1;
                            //document parse error,endTokenEndIndex may be zero
                            if (nextnode.location.endTokenEndIndex + 1 < nextnode.location.openTokenEndIndex + 1)
                            {
                                currentindex = nextnode.location.openTokenEndIndex + 1;
                            }
                            nextnode = iterator.NextSibling(nextnode);
                        }
                        else
                        {
                            currentindex = nextnode.location.openTokenEndIndex + 1;
                            nextnode = iterator.nextNode();
                        }

                        responseList.Clear();
                    }
                    else
                    {
                        nextnode = iterator.nextNode();
                    }
                }
                else
                {
                    nextnode = iterator.nextNode();
                }

            }

            if (currentindex < totallen - 1)
            {
                tasklist.Add(new ContentRenderTask(doc.HtmlSource.Substring(currentindex, totallen - currentindex)));
            }

            if (appendtask.Count() > 0)
            {
                tasklist.AddRange(appendtask);
            }
            RenderHelper.OptimizeTask(tasklist);

            return tasklist;
        }


        private static bool IsFakeHeader(Element element)
        {
            return element != null && element.tagName == "head" && Kooboo.Sites.Service.DomService.IsFakeElement(element);
        }

        private static List<IRenderTask> GetBinding(List<EvaluatorResponse> results)
        {
            if (results.Any(o => o.BindingTask != null))
            {
                List<IRenderTask> returnList = new List<IRenderTask>();
                foreach (var item in results)
                {
                    if (item.BindingTask != null)
                    {
                        returnList.AddRange(item.BindingTask);
                    }
                }
                return returnList;
            }
            return null;
        }

        private static List<IRenderTask> GetEndBinding(List<EvaluatorResponse> results)
        {
            if (results.Any(o => o.EndBindingTask != null))
            {
                List<IRenderTask> returnList = new List<IRenderTask>();
                foreach (var item in results)
                {
                    if (item.EndBindingTask != null)
                    {
                        returnList.AddRange(item.EndBindingTask);
                    }
                }
                returnList.Reverse();
                return returnList;
            }
            return null;
        }

        private static List<IRenderTask> GetAttribute(List<EvaluatorResponse> results)
        {
            if (results.Any(o => o.AttributeTask != null))
            {
                List<IRenderTask> returnList = new List<IRenderTask>();
                foreach (var item in results)
                {
                    if (item.AttributeTask != null)
                    {
                        returnList.AddRange(item.AttributeTask);
                    }
                }
                return returnList;
            }
            return null;
        }

        private static List<IRenderTask> GetContent(List<EvaluatorResponse> results)
        {
            if (results.Any(o => o.ContentTask != null))
            {
                List<IRenderTask> returnList = new List<IRenderTask>();
                foreach (var item in results)
                {
                    if (item.ContentTask != null)
                    {
                        returnList.AddRange(item.ContentTask);
                    }
                }
                return returnList;
            }
            return null;
        }
        private static List<IRenderTask> GetAppend(List<EvaluatorResponse> results)
        {
            if (results.Any(o => o.AppendTask != null))
            {
                List<IRenderTask> returnList = new List<IRenderTask>();
                foreach (var item in results)
                {
                    if (item.AppendTask != null)
                    {
                        returnList.AddRange(item.AppendTask);
                    }
                }
                return returnList;
            }
            return null;
        }

        private static string GetHalfOpenTag(Element element)
        {
            string ehtml = string.Empty;
            ehtml = "<" + element.tagName;
            foreach (var item in element.attributes)
            {
                ehtml += " " + item.name;
                if (!string.IsNullOrEmpty(item.value))
                {
                    ehtml += "=\"" + item.value + "\"";
                }
            }
            return ehtml;
        }

        private static bool ShouldTryRender(Node node)
        {
            if (node.nodeType == enumNodeType.ELEMENT || node.nodeType == enumNodeType.COMMENT)
            {
                if (node.nodeType == enumNodeType.ELEMENT)
                {
                    var el = node as Element;
                    foreach (var item in el.attributes)
                    {
                        if (item.name.ToLower() == "k-ignore")
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;


        }
    }
}
