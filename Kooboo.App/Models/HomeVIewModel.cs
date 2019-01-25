//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.App.Models
{
    internal class HomeViewModel : BaseModel
    {
        private bool _done;
        public bool Done
        {
            get { return _done; }
            set
            {
                if (_done != value)
                {
                    _done = value;
                    OnPropertyChanged(nameof(Done));
                }
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description!=value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private string _buttonText;
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                if (_buttonText != value)
                {
                    _buttonText = value;
                    OnPropertyChanged(nameof(ButtonText));
                }
            }
        }

        private bool _ready;
        public bool Ready
        {
            get { return _ready; }
            set
            {
                if (_ready != value)
                {
                    _ready = value;
                    OnPropertyChanged(nameof(Ready));
                }
            }
        }
    }
}
