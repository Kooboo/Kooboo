//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.App.SystemTray;
using System.Windows;
using System.Windows.Input;

namespace Kooboo.App.Commands
{
    internal class TaskbarClickCommand : CommandBase<TaskbarClickCommand>
    {
        public override void Execute(object parameter)
        {
            if (parameter is TaskbarIcon icon && icon.Parent != null)
            {
                icon.Parent.Visibility = Visibility.Visible;
                icon.Parent.Focus();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is TaskbarIcon icon && icon.Parent != null && (icon.Parent.Visibility == Visibility.Hidden || icon.Parent.Visibility == Visibility.Collapsed);
        }
    }
}