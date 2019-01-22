using System;
using System.Collections.Generic;
using Kooboo.Sites.InlineEditor.Model; 

namespace Kooboo.Sites.InlineEditor
{
   public static class EditorContainer
    {
        private static List<IInlineModel> _List; 

        public static List<IInlineModel> ModelList
        {
            get
            {
                if (_List == null)
                {
                    _List = new List<IInlineModel>();
                    _List.Add(new ContentModel());
                    _List.Add(new DomModel());
                    _List.Add(new HtmlblockModel());
                    _List.Add(new LabelModel());
                    _List.Add(new StyleModel());
                    _List.Add(new ImageModel());
                    _List.Add(new ConverterModel()); 
                }
                return _List; 
            }
        }

        private static List<IInlineExecutor> _ExecutorList;

        public static List<IInlineExecutor> ExecutorList
        {
            get
            {
                if (_ExecutorList == null)
                {
                    _ExecutorList = new List<IInlineExecutor>();
                    _ExecutorList.Add(new Executor.DomExecutor());
                    _ExecutorList.Add(new Executor.LabelExecutor());
                    _ExecutorList.Add(new Executor.ContentExecutor());
                    _ExecutorList.Add(new Executor.HtmlBlockExecutor());
                    _ExecutorList.Add(new Executor.StyleExecutor());
                    _ExecutorList.Add(new Executor.ImageExecutor());
                    _ExecutorList.Add(new Executor.ConverterExecutor()); 
                }
                return _ExecutorList;
            }
        }
        
        public static Type GetModelType(string editorType)
        {
            var type = ModelList.Find(o => o.EditorType == editorType); 
            if (type != null)
            {
                return type.GetType(); 
            }
            return null; 
        }
        
        public static IInlineExecutor GetExecutor(string editorType)
        {
          return ExecutorList.Find(o => o.EditorType == editorType); 
        }

    }
}
