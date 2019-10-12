//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Windows;
using System.Windows.Controls;

namespace Kooboo.App.Extensions
{
    public static class PageExtensions
    {
        public static void Redirect(this DependencyObject page, Page target)
        {
            var wind = Window.GetWindow(page);
            if (wind != null)
            {
                wind.Content = target;
            }
        }
    }
}