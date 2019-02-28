//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kooboo.App.Models
{
    internal class NewHostViewModel : EditorPageViewModel<NewHostViewModel>
    {
        private string _ip;
        private string _domain;

        [Required]
        public string IP
        {
            get { return _ip; }
            set
            {
                if (_ip != value)
                {
                    _ip = value;
                    OnPropertyChanged(nameof(IP));
                }
            }
        }

        public string IpPlaceholder { get; } = Data.Language.Hardcoded.GetValue("IP");

        public string DomainPlaceholder { get; } = Data.Language.Hardcoded.GetValue("Domain"); 

        [Required]
        public string Domain
        {
            get { return _domain; }
            set
            {
                if (_domain != value)
                {
                    _domain = value;
                    OnPropertyChanged(nameof(Domain));
                }
            }
        }
    }
}
