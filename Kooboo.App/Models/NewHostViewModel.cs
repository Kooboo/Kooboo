//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.ComponentModel.DataAnnotations;

namespace Kooboo.App.Models
{
    internal class NewHostViewModel : EditorPageViewModel<NewHostViewModel>
    {
        private string _ip;
        private string _domain;

        [Required]
        public string IP
        {
            get => _ip;
            set
            {
                if (_ip == value) return;
                _ip = value;
                OnPropertyChanged(nameof(IP));
            }
        }

        public string IpPlaceholder { get; } = Data.Language.Hardcoded.GetValue("IP");

        public string DomainPlaceholder { get; } = Data.Language.Hardcoded.GetValue("Domain");

        [Required]
        public string Domain
        {
            get => _domain;
            set
            {
                if (_domain == value) return;
                _domain = value;
                OnPropertyChanged(nameof(Domain));
            }
        }
    }
}