//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly HomeViewModel vm;

        public HomePage()
        {
            InitializeComponent();
            vm = new HomeViewModel
            {
                Title = Data.Language.Hardcoded.GetValue("Home"),
                From = this,
            };
            DataContext = vm;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            vm.Done = true;
            vm.ButtonText = Data.Language.Hardcoded.GetValue("Start");
            vm.Description = Data.Language.Hardcoded.GetValue("Kooboo Ready at port") + ": " + Kooboo.Data.AppSettings.HttpPort.ToString();
            Img.VerticalAlignment = VerticalAlignment.Top;
            btn.VerticalAlignment = VerticalAlignment.Top;
            txt.VerticalAlignment = VerticalAlignment.Top;

            Img.Margin = new Thickness(0, 10, 0, 0);
            btn.Margin = new Thickness(0, txt.DesiredSize.Height + 240, 0, 0);
            btn.Opacity = 1;
            txt.Margin = new Thickness(0, 230, 0, 0);
            txt.Opacity = 1;
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SystemStatus.StartUrl))
            {
                Process.Start(SystemStatus.StartUrl);
            }
        }
    }
}
