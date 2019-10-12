//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Header_MouseMove(object sender, MouseEventArgs e)
        {
            var header = sender as Border;
            if (header == null)
            {
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed && e.Source == header)
            {
                Window.GetWindow(header)?.DragMove();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && Equals(e.Source, btn))
            {
                var window = Window.GetWindow(btn);
                if (window is MainWindow mainWindow)
                {
                    //kooboo will not close when click close btn.
                    //it will be put in system tray;
                    mainWindow.NeedCancel = true;
                    mainWindow.Close();
                }
            }
        }

        private void SidebarButton_Checked(object sender, RoutedEventArgs e)
        {
            var pageMapping = new Dictionary<string, Page>
            {
                ["site"] = new HomePage(),
                ["server"] = new ServerPage(),
                ["host"] = new HostPage(),
                ["upgrade"] = new UpgradePage()
            };
            if (sender is RadioButton btn && Equals(e.Source, btn))
            {
                var wind = Window.GetWindow(btn);
                if (wind != null && pageMapping.TryGetValue(btn.Uid.ToLower(), out var page))
                {
                    wind.Content = page;
                }
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            KoobooUpgrade.DeleteUpgradeRemainedFiles();

            if (KoobooAutoStart.IsFirstBoot() || //first run default auto start
               KoobooAutoStart.IsAutoStart() || //override task when application path changed
               KoobooAutoStart.OldCodeHadSetAutoSart()) //compatible old code
            {
                KoobooAutoStart.AutoStart(true);
            }
            GlobalSettings.RootPath = Data.AppSettings.DatabasePath;
            KoobooStartUp.StartAll();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            KoobooStartUp.StopAll();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}