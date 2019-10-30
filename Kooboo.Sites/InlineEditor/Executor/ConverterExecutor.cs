//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Interface;
using Kooboo.Sites.InlineEditor.Converter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Sites.InlineEditor.Executor
{
    public class ConverterExecutor : IInlineExecutor
    {
        public string EditorType
        {
            get
            {
                return "converter";
            }
        }

        public void Execute(RenderContext context, List<IInlineModel> updatelist)
        {
            var modellist = updatelist.ToList().Cast<InlineEditor.Model.ConverterModel>().ToList();

            List<JObject> result = new List<JObject>();

            foreach (var item in modellist)
            {
                if (!string.IsNullOrEmpty(item.convertResult))
                {
                    var itemresult = Lib.Helper.JsonHelper.DeserializeObject(item.convertResult);
                    if (itemresult != null)
                    {
                        result.Add((JObject)itemresult);
                    }
                }
            }
            ConvertManager.ConvertComponent(context, result);
            // if it has both category and contentlist... needs to link the content list with category....
        }

        public void ExecuteObject(RenderContext context, IRepository repo, string nameOrId, List<IInlineModel> updates)
        {
            throw new NotImplementedException();
        }
    }
}