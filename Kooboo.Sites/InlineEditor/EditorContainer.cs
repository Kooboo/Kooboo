//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Sites.InlineEditor.Model;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.InlineEditor
{
    public static class EditorContainer
    {
        private static List<IInlineModel> _list;

        public static List<IInlineModel> ModelList
        {
            get
            {
                return _list ?? (_list = new List<IInlineModel>
                {
                    new ContentModel(),
                    new DomModel(),
                    new HtmlblockModel(),
                    new LabelModel(),
                    new StyleModel(),
                    new ImageModel(),
                    new ConverterModel()
                });
            }
        }

        private static List<IInlineExecutor> _executorList;

        public static List<IInlineExecutor> ExecutorList
        {
            get
            {
                return _executorList ?? (_executorList = new List<IInlineExecutor>
                {
                    new Executor.DomExecutor(),
                    new Executor.LabelExecutor(),
                    new Executor.ContentExecutor(),
                    new Executor.HtmlBlockExecutor(),
                    new Executor.StyleExecutor(),
                    new Executor.ImageExecutor(),
                    new Executor.ConverterExecutor()
                });
            }
        }

        public static Type GetModelType(string editorType)
        {
            var type = ModelList.Find(o => o.EditorType == editorType);
            return type?.GetType();
        }

        public static IInlineExecutor GetExecutor(string editorType)
        {
            return ExecutorList.Find(o => o.EditorType == editorType);
        }
    }
}