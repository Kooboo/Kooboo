//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;
using Kooboo.Sites.Models;

namespace Kooboo.Sites.Repository
{
    public class CmsFileRepository : SiteRepositoryBase<CmsFile>
    {
        public CmsFile Upload(byte[] contentBytes, string fullName, System.Guid userId)
        {
            string relativeUrl = UrlHelper.RelativePath(fullName);
            bool found = true;

            CmsFile file = this.GetByUrl(relativeUrl);

            if (file == null)
            {
                file = new CmsFile();
                found = false;
            }
            file.Name = UrlHelper.FileName(fullName);
            file.Extension = UrlHelper.FileExtension(fullName);

            file.ContentBytes = contentBytes;

            if (!found)
            {
                SiteDb.Routes.AddOrUpdate(relativeUrl, ConstObjectType.CmsFile, file.Id, userId);
            }

            this.AddOrUpdate(file, userId);

            return file;
        }
    }
}