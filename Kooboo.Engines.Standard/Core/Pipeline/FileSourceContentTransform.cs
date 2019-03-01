//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System.IO;

    public class FileSourceContentTransform : ContentTransformBase {
        private string _mimeType;
        private string[] _extensions;

        public FileSourceContentTransform(string mimeType, params string[] extensions) {
            _mimeType = mimeType;
            _extensions = extensions;
        }

        public override void Execute(ContentTransformState state) {
            // We're a content provider.  If content is already set, do nothing.
            if (state.Content != null)
                return;

            // Support 404, not just 500
            if (state.RootPath == null)
                return;

            // Search for file to use as content, stop at first
            string content = null;
            string fileName = null;
            foreach (var extension in _extensions) {
                fileName = state.RootPath + extension;
                if (File.Exists(fileName)) {
                    content = File.ReadAllText(fileName);
                    break;
                }
            }

            if (content != null) {
                state.AppendContent(new ContentResult() {
                    Content = content,
                    MimeType = _mimeType,
                    CacheInvalidationFileList = new string[] { fileName },
                });
            }
        }
    }
}
