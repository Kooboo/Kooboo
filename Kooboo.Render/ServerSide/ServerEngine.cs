//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Render.ObjectSource;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtualFile;

namespace Kooboo.Render.ServerSide
{
    public static class ServerEngine
    {
        private static object _locker = new object(); 

        public static Dictionary<string, JsRenderPlan> cache = new Dictionary<string, JsRenderPlan>(StringComparer.OrdinalIgnoreCase);

        private static JsRenderPlan GetJs(string relativeUrl)
        {
            lock(_locker)
            {
                if (cache.ContainsKey(relativeUrl))
                {
                    return cache[relativeUrl]; 
                }
                else
                {
                    return null; 
                }
            }

        }

        private static void SetJs(string relativeUrl, JsRenderPlan plan)
        {
           lock(_locker)
            {
                cache[relativeUrl] = plan; 
            }
        }
         

        public static RenderRespnose RenderJs(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string RelativeUrl)
        {
            var fullname = sourceProvider.GetFullFileName(context, RelativeUrl);

            if (string.IsNullOrEmpty(fullname))
            {
                return new RenderRespnose() { Body = null };
            }

            if (option.EnableMultilingual && RelativeUrl.ToLower().EndsWith(option.MultilingualJsFile))
            { 
                return RenderJsLangFile(fullname, context);
            }
             
            System.IO.FileInfo info = new System.IO.FileInfo(fullname);

            if (info != null && info.LastWriteTime != null)
            {
                JsRenderPlan renderplan = null;

                Guid hash = Lib.Security.Hash.ComputeGuidIgnoreCase(info.LastWriteTime.ToLongTimeString());

                var cacheplan = GetJs(RelativeUrl);

                if (cacheplan != null && cacheplan.Hash == hash)
                {
                    renderplan = cacheplan; 
                }
                  
                //either not key found not hash not the same. 
                if (renderplan == null)
                {
                    string fulltext = VirtualResources.ReadAllText(fullname);

                    renderplan = new JsRenderPlan();
                    renderplan.Tasks = GetJsRenderPlan(fulltext);
                    renderplan.Hash = hash;
                    SetJs(RelativeUrl, renderplan);  
                }

                if (renderplan != null)
                {
                    string result = string.Empty;
                    foreach (var task in renderplan.Tasks)
                    {
                        result += task.Render(sourceProvider, option, context, RelativeUrl);
                    }
                    return new RenderRespnose() { Body = result, ContentType = "application/javascript" };

                }  
            }
            else
            {
                return new RenderRespnose() { Body = null };
            }
             
            return new RenderRespnose() { Body = null };

        }

        public static RenderRespnose RenderJsLangFile(string FullFileName, RenderContext context)
        {

            var values = Kooboo.Data.Cache.MultiLingualRender.GetJs(context);
            if (values == null)
            {

                var bytes = VirtualResources.ReadAllBytes(FullFileName);

                values = Kooboo.Data.Cache.MultiLingualRender.SetGetJs(context, bytes);
            }

            return new RenderRespnose() { BinaryBytes = values, ContentType =  "application/javascript" }; 

        }


        public static List<IServerTask> GetJsRenderPlan(CommandDiskSourceProvider sourceProvider, RenderOption option, RenderContext context, string fullfilename, string relativeUrl)
        {
            List<IServerTask> result = new List<IServerTask>();

            List<preTask> pretask = new List<preTask>();

            string source = VirtualResources.ReadAllText(fullfilename);

            if (string.IsNullOrWhiteSpace(source))
            {
                result.Add(new StringTask() { content = "" });
                return result;
            }

            foreach (var item in TaskContainer.list)
            {

                var startEndList = GetStartEndList(source, item.Key);

                foreach (var one in startEndList)
                {
                    var line = source.Substring(one.Key, one.Value - one.Key + 1);

                    var paras = GetParas(line);

                    var taskInstance = Activator.CreateInstance(item.Value) as IServerTask;
                    taskInstance.paras = paras;

                    pretask.Add(new preTask() { start = one.Key, end = one.Value, Task = taskInstance });

                }
            }

            if (pretask.Count() > 0)
            {
                int currentIndex = 0;
                foreach (var item in pretask.OrderBy(o => o.start))
                {
                    int len = item.start - currentIndex;

                    if (len > 0)
                    {
                        string before = source.Substring(currentIndex, len);
                        result.Add(new StringTask() { content = before });
                    }

                    result.Add(item.Task);

                    currentIndex = item.end + 1;
                }
            }
            else
            {
                result.Add(new StringTask() { content = source });
            }

            return result;
        }

        public static List<IServerTask> GetJsRenderPlan(string source)
        {
            List<IServerTask> result = new List<IServerTask>();

            List<preTask> pretask = new List<preTask>();

            foreach (var item in TaskContainer.list)
            {
                var startEndList = GetStartEndList(source, item.Key);

                foreach (var one in startEndList)
                {
                    var line = source.Substring(one.Key, one.Value - one.Key + 1);

                    var paras = GetParas(line);

                    var taskInstance = Activator.CreateInstance(item.Value) as IServerTask;
                    taskInstance.paras = paras;

                    pretask.Add(new preTask() { start = one.Key, end = one.Value, Task = taskInstance });

                }
            }

            if (pretask.Count() > 0)
            {
                int currentIndex = 0;
                foreach (var item in pretask.OrderBy(o => o.start))
                {
                    int len = item.start - currentIndex;

                    if (len > 0)
                    {
                        string before = source.Substring(currentIndex, len);
                        result.Add(new StringTask() { content = before });
                    }
                    result.Add(item.Task);
                    currentIndex = item.end + 1;
                }

                if (currentIndex < source.Length - 1)
                {
                    result.Add(new StringTask() { content = source.Substring(currentIndex, source.Length - currentIndex) });
                }
            }
            else
            {
                result.Add(new StringTask() { content = source });
            }
            return result;
        }


        public static Dictionary<int, int> GetStartEndList(string source, string key)
        {
            //TODO: this should be replaced by a JavaScript token parser, but the jint does not work with index now, only line + column. 
            Dictionary<int, int> startend = new Dictionary<int, int>();

            int currentpos = 0;

            while (currentpos < source.Length - 1)
            {

                int index = source.IndexOf(key, currentpos);

                if (index > 0)
                {
                    var beforechar = source[index - 1];
                    if (beforechar != '\r' && beforechar == '\n' && beforechar == ' ')
                    {
                        currentpos = index + 1;
                        continue;
                    }
                }

                if (index > -1)
                {
                    // find the end..
                    int end = source.IndexOf(";", index);
                    int EndLine=source.IndexOf("\r\n", index);

                    if (EndLine > 0 && EndLine < end)
                    {
                        end = EndLine;
                    }
                    if (end == -1)
                    {
                        end = source.Length - 1;
                    }

                    currentpos = end + 1;

                    startend.Add(index, end);

                }
                else
                {
                    break;
                }

            }

            return startend;
        }


        public static List<string> GetParas(string commandline)
        {
            if (string.IsNullOrWhiteSpace(commandline))
            {
                return new List<string>();
            }

            List<string> result = new List<string>();

            int bracketStart = commandline.IndexOf("(");
            int bracketEnd = commandline.IndexOf(")");

            if (bracketStart > -1 && bracketEnd > -1 && bracketEnd > bracketStart)
            {
                string betweenText = commandline.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);

                if (!string.IsNullOrWhiteSpace(betweenText))
                {
                    string[] paras = betweenText.Split(',');

                    foreach (var item in paras)
                    {
                        if (item == null)
                        {
                            result.Add("");
                        }
                        else
                        {
                            string para = item.Trim();
                            para = para.Trim('"');
                            para = para.Trim('\'');
                            para = para.Trim();
                            result.Add(para);
                        }
                    }
                }
            }

            return result;
        }
    }

    public class preTask
    {
        public int start { get; set; }
        public int end { get; set; }

        public IServerTask Task { get; set; }
    }

    public class JsRenderPlan
    {
        public Guid Hash { get; set; }

        public List<IServerTask> Tasks { get; set; }

    }
}
