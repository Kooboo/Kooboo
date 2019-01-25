//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kooboo.App.Models
{
    internal class ListViewItemViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Tooltip { get; set; }

        public string SubTitle { get; set; }

        public string SubTooltip { get; set; }

        public List<IconButton> Icons { get; set; } = new List<IconButton>();
    }
}
