//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
  
using Kooboo.App.Validators; 
using System.ComponentModel.DataAnnotations;
 

namespace Kooboo.App.Models
{
    internal class NewServerViewModel : EditorPageViewModel<NewServerViewModel>
    {
        private string _path;
        [Required]
        [MaxLength(1024)]
        [FolderExists]
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        private string _port;  
        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port)); 
            }
        }

        private string _domain; 
        [MaxLength(1024)]
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
