//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Sites.Models;

namespace Kooboo.Web.ViewModel
{
    public class FormItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ValueCount { get; set; }

        public string Body { get; set; }

        public Dictionary<string, int> Relations { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    }

    public class FormValueViewModel
    {
        public List<string> Columns { get; set; } = new List<string>();
        public List<FormValue> Values { get; set; } = new List<FormValue>();
    }

    public class FormEditViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public bool IsEmbedded { get; set; }

        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        public string Submitter { get; set; }

        public string Fields { get; set; }
        public string Style { get; set; }

        public List<FormSubmitterViewModel> AvailableSubmitters { get; set; } = new List<FormSubmitterViewModel>();

    }

    public class FormSubmitterViewModel
    {
        public string Name { get; set; }
        public List<Data.Models.SimpleSetting> Settings { get; set; }
    }


    public class FormUpdateViewModel
    {
        public Guid Id { get; set; }

        public string Body { get; set; }

        public Dictionary<string, string> Settings { get; set; }

        public string Submitter { get; set; }

        public string Method { get; set; }

        public string RedirectUrl { get; set; }

        public bool AllowAjax { get; set; }

        public string SuccessCallBack { get; set; }

        public string FailedCallBack { get; set; }
    }

    public class FormListItemViewModel : IEmbeddableItemListViewModel
    {
        public int ValueCount { get; set; }

        public bool IsEmbedded { get; set; }

        public string FormType { get; set; }
    }



    public class FormSettingEditViewModel
    {

        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public bool Enable { get; set; }

        /// <summary>
        /// the Action method, HTTP Post or Http Get. 
        /// </summary>
        public string Method { get; set; }

        public string RedirectUrl { get; set; }

        public bool AllowAjax { get; set; }

        public string SuccessCallBack { get; set; }

        public string FailedCallBack { get; set; }

        private Dictionary<string, string> _setting;

        [Kooboo.IndexedDB.CustomAttributes.KoobooKeyIgnoreCase]
        public Dictionary<string, string> Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _setting;
            }
            set { _setting = value; }
        }

        public string FormSubmitter { get; set; }

        public List<FormSubmitterViewModel> AvailableSubmitters { get; set; } = new List<FormSubmitterViewModel>();

    }


    public class KoobooFormEditModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Body { get; set; }

        public bool IsEmbedded { get; set; }


        public string Fields { get; set; }

        public string Style { get; set; }

        public bool Enable { get; set; }

        public string Method { get; set; }

        public string RedirectUrl { get; set; }

        public bool AllowAjax { get; set; }

        public string SuccessCallBack { get; set; }

        public string FailedCallBack { get; set; }

        public string FormSubmitter { get; set; }

        public Dictionary<string, string> Setting { get; set; }

        public List<FormSubmitterViewModel> AvailableSubmitters { get; set; } = new List<FormSubmitterViewModel>();
    }



}
