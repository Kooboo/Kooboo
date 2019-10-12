//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.App.Commands;
using Kooboo.App.Extensions;
using Kooboo.App.Models;
using Kooboo.Data.Hosts;
using System.Windows;
using System.Windows.Controls;

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

        internal readonly NewHostViewModel HostViewModel;

        public CreateHostPage()
        {
            InitializeComponent();
            back.Content = Data.Language.Hardcoded.GetValue("back");
            save.Content = Data.Language.Hardcoded.GetValue("save");
            HostViewModel = new NewHostViewModel
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
            DataContext = HostViewModel;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Redirect(new HostPage());
        }
    }
}