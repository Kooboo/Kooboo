//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Commands;
using Kooboo.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kooboo.App.Extensions;
using Kooboo.Data;
using Kooboo.Data.Models;
using System.Diagnostics;
using System.IO;
using Kooboo.Sites.Extensions;

namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for ServerPage.xaml
    /// </summary>
    public partial class ServerPage : Page
    {
        public ServerPage()
        {
            InitializeComponent();

            context = new ListViewModel
            {
                Title = Data.Language.Hardcoded.GetValue("Server"),
                Buttons = new[]
                {
                    new NavbarButton
                    {
                        Text = Data.Language.Hardcoded.GetValue("+ New Server"),
                        From = this,
                        Command = new DelegateCommand(vm=>
                        {
                            this.Redirect(new CreateServerPage(this));
                        })
                    }
                }
            };
            Reload();
        }

        private readonly ListViewModel context;

        private void Reload()
        {
            context.ItemsSource = GlobalDb.WebSites
                .GetLocalSites()
                .Select(it => new ListViewItemViewModel
                {
                    Id = it.Id.ToString(),
                    Title = it.DisplayName.ToUpper(),
                    Tooltip = it.Name,
                    SubTitle = it.LocalRootPath,
                    SubTooltip = it.LocalRootPath,
                    Icons = new List<IconButton>
                    {
                        new IconButton
                        {
                            Context = it,
                            Icon="preview-btn",
                            Tooltip= Data.Language.Hardcoded.GetValue("View in browser"),
                            Command = new DelegateCommand<WebSite>(site=> {

                                string url = site.BaseUrl();
                                if (!string.IsNullOrEmpty(url) && url !="/" && url != "\\")
                                { Process.Start(url); }
                                else
                                {  MessageBox.Show(Data.Language.Hardcoded.GetValue("Invalid website configuration"));}

                            })
                        },
                        new IconButton
                        {
                            Context = it,
                            Icon = "folder-btn",
                            Tooltip=Data.Language.Hardcoded.GetValue("Open containing folder"),
                            Command = new DelegateCommand<WebSite>(site=>
                            {
                                if (!string.IsNullOrEmpty(site.LocalRootPath) && Directory.Exists(site.LocalRootPath))
                                {
                                    Process.Start(site.LocalRootPath);
                                }
                                else
                                {
                                    MessageBox.Show(Data.Language.Hardcoded.GetValue("Website path not found"));
                                }
                            })
                        },
                        new IconButton
                        {
                            Context = it,
                            Icon = "delete-btn",
                            Tooltip =Data.Language.Hardcoded.GetValue("Delete server"),
                            Command = new DelegateCommand<WebSite>(site=>
                            {
                                if (MessageBox.Show(Data.Language.Hardcoded.GetValue("Are you sure you want to delete this item?"), Data.Language.Hardcoded.GetValue("Confirm"), MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                     Guid webSiteId=site.Id;
                                    var bindings = GlobalDb.Bindings.GetByWebSite(webSiteId);
                                    var domainDictionary = new Dictionary<string,Guid>();
                                    foreach (var binding in bindings)
                                    {
                                        GlobalDb.Bindings.Delete(binding);
                                        var rootDomain = Kooboo.Data.Helper.DomainHelper.GetRootDomain(binding.FullName);
                                        domainDictionary[rootDomain]= binding.DomainId;
                                    }

                                    // Delete Domain which have no bindings
                                    var domains = GlobalDb.Domains
                                    .ListByUser(GlobalDb.Users.DefaultUser)
                                    .Where(d =>domainDictionary.ContainsValue(d.Id) && !GlobalDb.Bindings.GetByDomain(d.DomainName).Any());
                                    foreach (var domain in domains)
                                    {
                                        GlobalDb.Domains.Delete(domain);
                                    }
                                    GlobalDb.WebSites.Delete(webSiteId);
                                    this.Redirect(new ServerPage());
                                }
                            })
                        }
                    }
                });

            DataContext = context;
        }
    }
}
