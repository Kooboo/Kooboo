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
    internal abstract class BaseModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _title;
        public virtual string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public virtual DelegateCommand<BaseModel> BackCommand { get; set; }

        public virtual DependencyObject From { get; set; }

        public string Error
        {
            get
            {
                return errorMessage;
            }
        }
        private string errorMessage { get; set; } = string.Empty;

        public string this[string columnName]
        {
            get
            {
                var vc = new ValidationContext(this, null, null)
                {
                    MemberName = columnName
                };
                var res = new List<ValidationResult>();
                var result = Validator.TryValidateProperty(GetType().GetProperty(columnName).GetValue(this, null), vc, res);
                if (res.Count > 0)
                {
                    return string.Join(Environment.NewLine, res.Select(r => r.ErrorMessage).ToArray());
                }
                return string.Empty;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid()
        {
            var vc = new ValidationContext(this, null, null);
            var res = new List<ValidationResult>();
            var result = Validator.TryValidateObject(this, vc, res);
            errorMessage = string.Join(Environment.NewLine, res.Select(r => r.ErrorMessage).ToArray());
            return result;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
