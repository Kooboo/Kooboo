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
    internal class NavbarButton
    {
        public string Text { get; set; }

        public ICommand Command { get;set; }

        public DependencyObject From { get; set; }
    }
}
