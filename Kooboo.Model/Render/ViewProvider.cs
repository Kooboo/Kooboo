using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kooboo.Sites.Render.Commands;

namespace Kooboo.Model.Render
{
    public class ViewProvider : IViewProvider
    {
        private ModelRenderContext _context;

        public ViewProvider(ModelRenderContext context)
        {
            _context = context;
        }

        public string GetView(string path)
        {
            var relativeUrl = GetRelativeUrl(_context.RelativeUrl, path);
            return _context.SourceProvider.GetString(_context.HttpContext, relativeUrl);
        }

        public static string GetRelativeUrl(string relativeUrl, string path)
        {
            path = path.Replace("\\", "/").TrimStart('/');

            var lastIndex = relativeUrl.LastIndexOf("/");
            if (lastIndex < 0)
                return path;

            return relativeUrl.Substring(0, lastIndex) + '/' + path;
        }
    }
}
