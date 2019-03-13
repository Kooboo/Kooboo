//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.IndexedDB;
using Kooboo.Sites.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public FormSetting GetByFormId(Guid FormId)
        { 
            var value = this.Query.Where(o => o.FormId == FormId).FirstOrDefault(); 

            if (value == null)
            {
                // for backward compatible... 
                var form = this.SiteDb.Forms.Get(FormId); 
                if (form !=null && !string.IsNullOrWhiteSpace(form.FormSubmitter))
                {
                    FormSetting setting = new FormSetting();
                    setting.FormSubmitter = form.FormSubmitter;
                    setting.FormId = form.Id;
                    setting.AllowAjax = form.AllowAjax;
                    setting.Enable = true; 
                    setting.CreationDate = form.CreationDate;
                    setting.FailedCallBack = form.FailedCallBack;
                    setting.Method = form.Method;
                    setting.RedirectUrl = form.RedirectUrl;
                    setting.SuccessCallBack = form.SuccessCallBack;
                    setting.Setting = form.Setting; 
                    return setting;  
                } 
            } 
            return value; 
        } 
          
    }




}
