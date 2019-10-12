//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
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