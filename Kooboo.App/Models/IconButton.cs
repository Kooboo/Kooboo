//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kooboo.App.Models
{
    internal class IconButton
    {
        public string Icon { get; set; }

        public string Tooltip { get; set; }

        public ICommand Command { get; set; }

        public object Context { get; set; }
    }
}