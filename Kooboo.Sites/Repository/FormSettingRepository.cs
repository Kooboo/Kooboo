//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;

namespace Kooboo.Sites.Repository
{
    public class FormSettingRepository : SiteRepositoryBase<FormSetting>
    {
        public override ObjectStoreParameters StoreParameters
        {
            get
            {
                var storeParams = new ObjectStoreParameters();
                storeParams.AddIndex<FormSetting>(x => x.FormId);
                storeParams.SetPrimaryKeyField<FormSetting>(o => o.Id);
                return storeParams;
            }
        }

        public FormSetting GetByFormId(Guid formId)
        {
            var value = this.Query.Where(o => o.FormId == formId).FirstOrDefault();

            if (value == null)
            {
                // for backward compatible...
                var form = this.SiteDb.Forms.Get(formId);
                if (form != null && !string.IsNullOrWhiteSpace(form.FormSubmitter))
                {
                    FormSetting setting = new FormSetting
                    {
                        FormSubmitter = form.FormSubmitter,
                        FormId = form.Id,
                        AllowAjax = form.AllowAjax,
                        Enable = true,
                        CreationDate = form.CreationDate,
                        FailedCallBack = form.FailedCallBack,
                        Method = form.Method,
                        RedirectUrl = form.RedirectUrl,
                        SuccessCallBack = form.SuccessCallBack,
                        Setting = form.Setting
                    };
                    return setting;
                }
            }
            return value;
        }
    }
}