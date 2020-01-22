//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Models;
using Kooboo.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System;

namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool NeedCancel = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Content = new HomePage();
            taskBarIcon.Parent = this;
            taskBarIcon.ToolTipText = Data.Language.Hardcoded.GetValue("Kooboo at port") + ":" + AppSettings.HttpPort;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (NeedCancel)
            {
                e.Cancel = true;
                Visibility = Visibility.Hidden;
                NeedCancel = false;
            }
            else
            {
                Application.Current.Shutdown();
                CommandManager.InvalidateRequerySuggested();
            }
           
        }

        private void menuShow_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Visible;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            CommandManager.InvalidateRequerySuggested();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            taskBarIcon.Dispose();
        }
    }
}
