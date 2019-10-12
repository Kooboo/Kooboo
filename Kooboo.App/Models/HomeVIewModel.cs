//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.App.Models
{
    internal class HomeViewModel : BaseModel
    {
        private bool _done;

        public bool Done
        {
            get => _done;
            set
            {
                if (_done == value) return;
                _done = value;
                OnPropertyChanged(nameof(Done));
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                if (_description == value) return;
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _buttonText;

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                if (_buttonText == value) return;
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        private bool _ready;

        public bool Ready
        {
            get => _ready;
            set
            {
                if (_ready == value) return;
                _ready = value;
                OnPropertyChanged(nameof(Ready));
            }
        }
    }
}