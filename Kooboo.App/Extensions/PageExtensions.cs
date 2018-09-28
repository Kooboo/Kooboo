using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
