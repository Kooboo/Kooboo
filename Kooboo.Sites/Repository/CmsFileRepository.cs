//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;
using Kooboo.Lib.Helper;
using Kooboo.IndexedDB;

namespace Kooboo.Sites.Repository
{
    public class CmsFileRepository : SiteRepositoryBase<CmsFile>
    { 
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var paras = new ObjectStoreParameters();
                paras.AddColumn<CmsFile>(o=>o.Size);
                paras.AddColumn<CmsFile>(o=>o.Extension,10);
                paras.AddColumn<CmsFile>(o => o.ContentType, 50);
                paras.AddColumn<CmsFile>(o => o.Name, 200);

                paras.SetPrimaryKeyField<Code>(o => o.Id); 

                return paras;
            }
        }



        public CmsFile Upload(byte[] contentBytes, string fullName, System.Guid UserId)
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
                SiteDb.Routes.AddOrUpdate(relativeUrl, ConstObjectType.CmsFile, file.Id, UserId);
            }

            this.AddOrUpdate(file, UserId); 

            return file;
        }
          
    }
}
