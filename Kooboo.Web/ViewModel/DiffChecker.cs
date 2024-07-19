using Kooboo.Data.Interface;
using Kooboo.Lib.Exceptions;
using Kooboo.Sites.Models;

namespace Kooboo.Web.ViewModel
{
    public interface IDiffChecker
    {
        public string Body { get; set; }
        public long Version { get; set; }
        public bool? EnableDiffChecker { get; set; }

        public void CheckDiff<T>(T coreObject) where T : CoreObject, ITextObject
        {
            if (false.Equals(EnableDiffChecker)) return;
            if (Version == coreObject.Version) return;
            if (Body == coreObject.Body) return;
            throw new DiffException(coreObject.Version, coreObject.Body);
        }
    }

}

