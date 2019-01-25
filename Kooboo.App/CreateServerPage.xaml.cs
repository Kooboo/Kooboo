//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.App.Commands;
using Kooboo.App.Extensions;
using Kooboo.App.Models;
using Kooboo.Data;
using Kooboo.Data.Helper;
using Kooboo.Data.Models;
using System.Windows;
using System.Windows.Controls;
using Kooboo.App.UserControls;


namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for CreateServerPage.xaml
    /// </summary>
    public partial class CreateServerPage : Page
    {
        private readonly DependencyObject _parent;
        public CreateServerPage(DependencyObject parent)
            : this()
        {
            _parent = parent;
        }

        internal readonly NewServerViewModel serverViewModel;

        public CreateServerPage()
        {
            InitializeComponent();
            backbtn.Content = Data.Language.Hardcoded.GetValue("back");
            saveBtn.Content = Data.Language.Hardcoded.GetValue("save"); 
            serverViewModel = new NewServerViewModel
            {
                Title = Data.Language.Hardcoded.GetValue("+ New Server"), 
                AddCommand = new DelegateCommand<NewServerViewModel>(vm =>
                {
                    if (!vm.IsValid())
                    {
                        MessageBox.Show(vm.Error);
                        return;
                    }
                    var isBindingToDomain = rbDomain.IsChecked.Value;
                    var messageText = Data.Language.Hardcoded.GetValue("Domain or port number are required");

                    string name = null;
                    System.Guid domainid = default(System.Guid);
                    bool defaultbinding = false;
                    string subDomain = null;
                    int port = 0;
                    if (isBindingToDomain)
                    {
                        if (string.IsNullOrEmpty(vm.Domain))
                        {
                            MessageBox.Show(messageText);
                            return;
                        }
                        DomainResult domainresult = null;

                        domainresult = DomainHelper.Parse(vm.Domain);
                        var domain = new Domain
                        {
                            DomainName = domainresult.Domain,
                            OrganizationId = GlobalDb.Users.DefaultUser.CurrentOrgId
                        };
                        GlobalDb.Domains.AddOrUpdate(domain);
                        if (domainresult != null && !string.IsNullOrEmpty(domainresult.Domain))
                        {
                            name = domainresult.Domain;
                            domainid = IDGenerator.GetDomainId(domainresult.Domain);
                            subDomain = domainresult.SubDomain;
                        }
                        else
                        {
                            MessageBox.Show("Domain is not right");
                            return;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(vm.Port))
                        {
                            int.TryParse(vm.Port, out port);
                        }
                        if (port <= 0 || port>65535)
                        {
                            MessageBox.Show(messageText);
                            return;
                        }
                        name = "Port" + vm.Port.ToString();
                        defaultbinding = true;
                    }

                    // add new website. 
                    WebSite site = new WebSite
                    {
                        Name = name,
                        LocalRootPath = vm.Path
                    };
                    GlobalDb.WebSites.AddOrUpdate(site);

                    var bind = new Data.Models.Binding
                    {
                        DomainId = domainid,
                        SubDomain = subDomain,
                        WebSiteId = site.Id,
                        DefaultPortBinding = defaultbinding,
                        Port = port
                    };

                    GlobalDb.Bindings.AddOrUpdate(bind);

                    this.Redirect(new ServerPage());
                     
                })
            };
            DataContext = serverViewModel;
        } 

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Redirect(new ServerPage());
        }

        protected void Select_BindingTo(object sender, RoutedEventArgs e)
        {
            RadioButton radioButtion = sender as RadioButton;
            var content = radioButtion.Content.ToString().ToLower();
            switch (content)
            {
                case "domain":
                    //portTxt.Text = "";
                    portTxt.Visibility = Visibility.Collapsed;
                    domainTxt.Visibility = Visibility.Visible;
                    break;
                case "port":
                    //domainTxt.Text = "";
                    domainTxt.Visibility = Visibility.Collapsed;
                    portTxt.Visibility = Visibility.Visible;
                    break;
            }

        }
    }
}
