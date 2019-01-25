//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Commands;
using Kooboo.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Kooboo.App.Extensions;
using Kooboo.Data.Hosts;

namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for HostPage.xaml
    /// </summary>
    public partial class HostPage : Page
    {

        private readonly ListViewModel context = new ListViewModel
        {
            Title = Data.Language.Hardcoded.GetValue("Host")
        };

        public HostPage()
        {
            InitializeComponent();
            context.Buttons = new[]
            {
                new NavbarButton
                {
                    Text = Data.Language.Hardcoded.GetValue("+ New host"), 
                    Command = new DelegateCommand(vm=> {
                        this.Redirect(new CreateHostPage(this));
                    }),
                    From = this
                }
            };
            DataContext = context;
            Reload();
        }

        private void Reload()
        {
            context.ItemsSource = WindowsHost
                .GetList()
                .OrderBy(it => it.Domain)
                .Select(it => new ListViewItemViewModel
                {
                    Id = it.Domain,
                    Title = it.Domain.ToUpper(),
                    Tooltip = it.Domain,
                    SubTitle = it.IpAddress,
                    SubTooltip = it.IpAddress,
                    Icons = new List<IconButton>
                    {
                        new IconButton
                        {
                            Icon = "delete-btn",
                            Context = it,
                            Command = new DelegateCommand<HostRecord>(record=> {
                                if (MessageBoxResult.Yes == MessageBox.Show(Data.Language.Hardcoded.GetValue("Are you sure you want to delete this item?"),Data.Language.Hardcoded.GetValue("Confirm"),MessageBoxButton.YesNo,MessageBoxImage.Asterisk))
                                {
                                    WindowsHost.Delete(record.Domain);
                                    this.Redirect(new HostPage());
                                }
                            })
                        }
                    }
                });
            DataContext = context;
        }
    }
}
