//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript {
    using System;
    using System.IO;
    using SassAndCoffee.Core;

    public class JavaScriptCombineContentTransform : ContentTransformBase {
        private readonly string _appRootPath;

        public JavaScriptCombineContentTransform() {
            var tempPath = AppDomain.CurrentDomain.BaseDirectory;
            if (!tempPath.EndsWith(@"\"))
                tempPath += @"\";
            _appRootPath = tempPath;
        }

        public override void Execute(ContentTransformState state) {
            // We're a content provider.  If content is already set, do nothing.
            if (state.Content != null)
                return;

            // Support 404, not just 500
            if (state.RootPath == null)
                return;

            var fileInfo = new FileInfo(state.RootPath + ".combine");
            if (fileInfo.Exists) {
                state.AddCacheInvalidationFiles(new string[] { fileInfo.FullName });

                var lines = File.ReadLines(fileInfo.FullName);
                foreach (var line in lines) {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                        continue;
                    string newPath = null;
                    if (trimmed.StartsWith("~/") && trimmed.Length > 2 && _appRootPath != null) {
                        // ASP.Net absolute path
                        newPath = _appRootPath + trimmed.Substring(2);
                    } else {
                        // Relative path
                        newPath = Path.Combine(fileInfo.DirectoryName, trimmed);
                    }
                    var newContent = state.Pipeline.ProcessRequest(newPath);
                    if (newContent != null) {
                        newContent.Content += ";";
                        state.AppendContent(newContent);
                    }
                }
            }

        }
    }
}
