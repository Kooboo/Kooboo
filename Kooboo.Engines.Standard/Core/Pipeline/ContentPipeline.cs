//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.Core {
    using System;
    using System.Collections.Generic;

    public class ContentPipeline : IContentPipeline {
        private List<IContentTransform> _transformations = new List<IContentTransform>();

        public IList<IContentTransform> Transformations { get { return _transformations; } }

        public ContentPipeline(params IContentTransform[] transformations)
            : this((IEnumerable<IContentTransform>)transformations) { }

        public ContentPipeline(IEnumerable<IContentTransform> transformations) {
            _transformations.AddRange(transformations);
        }

        public ContentResult ProcessRequest(string physicalPath) {
            ContentResult result = null;

            // TODO: Check cache
            // result = GetFromCache

            if (result == null) { // Cache miss
                var state = new ContentTransformState(this, physicalPath);

                // Pre-Execute
                foreach (var transform in _transformations) {
                    transform.PreExecute(state);
                }

                // Execute
                foreach (var transform in _transformations) {
                    transform.Execute(state);
                }

                if (state.Content == null) {
                    // No source content found
                    return null;
                }

                result = new ContentResult() {
                    CacheInvalidationFileList = state.CacheInvalidationFileList,
                    Content = state.Content,
                    MimeType = state.MimeType,
                };

                // TODO: Save in cache
            }

            return result;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_transformations != null) {
                    foreach (var item in _transformations)
                        item.Dispose();
                    _transformations = null;
                }
            }
        }
    }
}
