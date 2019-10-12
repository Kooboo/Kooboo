//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.App.Commands;

namespace Kooboo.App.Models
{
    internal class EditorPageViewModel<T> : BaseModel
        where T : class
    {
        public DelegateCommand<T> AddCommand { get; set; }
    }
}