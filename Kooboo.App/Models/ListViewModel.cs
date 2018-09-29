//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.App.Models
{
    internal class ListViewModel : BaseModel
    {
        public IEnumerable<NavbarButton> Buttons { get; set; } = Enumerable.Empty<NavbarButton>();

        public IEnumerable<ListViewItemViewModel> ItemsSource { get; set; } = Enumerable.Empty<ListViewItemViewModel>();
    }
}
