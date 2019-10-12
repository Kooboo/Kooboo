//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.App.Models
{
    internal class UpgradeViewModel : BaseModel
    {
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

        public string Copyright { get; set; }

        public string LinkText { get; set; }
    }
}