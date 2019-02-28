//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.SystemTray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kooboo.App.Commands
{
    internal class TaskbarClickCommand : CommandBase<TaskbarClickCommand>
    {
        public override void Execute(object parameter)
        {
            TaskbarIcon icon = parameter as TaskbarIcon;
            if (icon != null && icon.Parent != null)
            {
                icon.Parent.Visibility = Visibility.Visible;
                icon.Parent.Focus();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public override bool CanExecute(object parameter)
        {
            TaskbarIcon icon = parameter as TaskbarIcon;

            return icon != null && icon.Parent != null && (icon.Parent.Visibility == Visibility.Hidden || icon.Parent.Visibility == Visibility.Collapsed);
        }
    }
}
