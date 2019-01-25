//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Commands;
using Kooboo.App.Models;
using Kooboo.App.Extensions;
using Kooboo.Data.Hosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for CreateHostPage.xaml
    /// </summary>
    public partial class CreateHostPage : Page
    {
        private readonly DependencyObject _parent;
        public CreateHostPage(DependencyObject parent)
            : this()
        {
            _parent = parent;
        }

        internal readonly NewHostViewModel hostViewModel;

        public CreateHostPage()
        {
            InitializeComponent();
            back.Content = Data.Language.Hardcoded.GetValue("back");
            save.Content = Data.Language.Hardcoded.GetValue("save");
            hostViewModel = new NewHostViewModel
            {
                Title = Data.Language.Hardcoded.GetValue("+ New host"),
                From = _parent,
                AddCommand = new DelegateCommand<NewHostViewModel>(vm =>
                {
                    if (!vm.IsValid())
                    {
                        MessageBox.Show(vm.Error);
                        return;
                    }
                    if (vm.Domain == vm.DomainPlaceholder)
                    {
                        MessageBox.Show(Data.Language.Hardcoded.GetValue("Domain is required"));
                        return;
                    }
                    else if (vm.IP == vm.IpPlaceholder)
                    {
                        MessageBox.Show(Data.Language.Hardcoded.GetValue("IP is required"));
                        return;
                    }
                    WindowsHost.AddOrUpdate(vm.Domain, vm.IP);
                    this.Redirect(new HostPage());
                })
            };
            DataContext = hostViewModel;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Redirect(new HostPage());
        }
    }
}
