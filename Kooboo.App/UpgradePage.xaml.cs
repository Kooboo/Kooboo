//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.App.Models;
using Kooboo.Data;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kooboo.App
{
    /// <summary>
    /// Interaction logic for UpgradePage.xaml
    /// </summary>
    public partial class UpgradePage : Page
    {
        private readonly UpgradeViewModel _vm;
        private static bool _isUpgrade = false;

        public UpgradePage()
        {
            InitializeComponent();
            _vm = new UpgradeViewModel
            {
                Title = Data.Language.Hardcoded.GetValue("Upgrade"),
                From = this,
            };
            DataContext = _vm;
            Init();
        }

        private void Init()
        {
            update.Content = Data.Language.Hardcoded.GetValue("Upgrade");

            title.Text = Data.Language.Hardcoded.GetValue("Setting");

            #region 语言设置

            langName.Text = Data.Language.Hardcoded.GetValue("Language");

            var systemLangCode = Data.Language.LanguageSetting.SystemLangCode;
            var cmslangs = Data.Language.LanguageSetting.CmsLangs;

            cbLang.ItemsSource = Data.Language.LanguageSetting.CmsLangs;
            cbLang.SelectedValuePath = "Key";
            cbLang.DisplayMemberPath = "Value";

            if (cmslangs.ContainsKey(systemLangCode))
            {
                cbLang.SelectedValue = systemLangCode;
                cbLang.SelectionChanged += LangSelectedChange;
            }

            #endregion 语言设置

            #region 自动升级

            lbupgrade.Text = Data.Language.Hardcoded.GetValue("Upgrade");
            chAutoUpgrade.Content = Data.Language.Hardcoded.GetValue("Auto Update");
            chAutoUpgrade.IsChecked = KoobooUpgrade.IsAutoUpgrade;
            chAutoUpgrade.Checked += AutoUpgradeEvent;
            chAutoUpgrade.Unchecked += AutoUpgradeEvent;

            #endregion 自动升级

            #region 开机启动设置

            lbstart.Text = Data.Language.Hardcoded.GetValue("Start-up");
            chAutoStart.Content = Data.Language.Hardcoded.GetValue("Auto Start");
            chAutoStart.IsChecked = KoobooAutoStart.IsAutoStart();
            chAutoStart.Checked += AutoStartEvent;
            chAutoStart.Unchecked += AutoStartEvent;

            #endregion 开机启动设置

            upgradetxt.Text = Data.Language.Hardcoded.GetValue("Version");
            var version = AppSettings.Version;
            update.Visibility = Visibility.Hidden;
            currentVersion.Text = version.ToString();

            CheckVersion();

            right.Text = "© " + System.DateTime.Now.Year.ToString() + " Kooboo. " + Data.Language.Hardcoded.GetValue("All rights reserved");
            _vm.LinkText = "http://www.kooboo.com";
        }

        private void CheckVersion()
        {
            if (_isUpgrade)
            {
                SetUpgradeBtnStatus();
                return;
            }
            UpdateVersionAsync();
        }

        public async void UpdateVersionAsync()
        {
            var version = AppSettings.Version;
            var newversion = await KoobooUpgrade.GetLatestVersion();

            void Action()
            {
                if (newversion > version)
                {
                    update.Visibility = Visibility.Visible;
                    update.Click += Upgrade_Click;
                    update.Content = Data.Language.Hardcoded.GetValue("Upgrade") + $"({newversion})";
                }
                else
                {
                    update.Visibility = Visibility.Hidden;
                    currentVersion.Text = $"{version}";
                }
            }

            await Dispatcher?.BeginInvoke((Action) Action);
        }

        #region 自动升级

        private void AutoUpgradeEvent(object sender, RoutedEventArgs e)
        {
            var auto = chAutoUpgrade.IsChecked.Value;
            KoobooUpgrade.SetAutoUpgrade(auto);
        }

        #endregion 自动升级

        #region 开机自动启动

        private void AutoStartEvent(object sender, RoutedEventArgs e)
        {
            var auto = chAutoStart.IsChecked.Value;
            KoobooAutoStart.AutoStart(auto);
        }

        #endregion 开机自动启动

        #region 语言设置

        private void LangSelectedChange(object sender, SelectionChangedEventArgs e)
        {
            var cmsLang = cbLang.SelectedValue.ToString();
            Data.Language.LanguageSetting.SystemLangCode = cmsLang;
            AppSettings.SetConfigValue("CmsLang", cmsLang);
            this.Init();
        }

        #endregion 语言设置

        private void SetUpgradeBtnStatus()
        {
            update.Visibility = Visibility.Visible;
            update.Background = new SolidColorBrush(Colors.Gray);
            update.Content = Data.Language.Hardcoded.GetValue("Downloading") + "...";
            update.IsEnabled = false;
        }

        #region 自动升级程序

        private async void Upgrade_Click(object sender, RoutedEventArgs e)
        {
            //防止重复点击
            if (!_isUpgrade)
            {
                _isUpgrade = true;
                try
                {
                    void Action()
                    {
                        SetUpgradeBtnStatus();
                    }

                    await Dispatcher?.BeginInvoke((Action) Action);

                    await KoobooUpgrade.Upgrade(DownloadProgressChanged);
                }
                finally
                {
                    _isUpgrade = false;
                }
            }
        }

        private void DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            int percentage = e.ProgressPercentage;

            update.Content = Data.Language.Hardcoded.GetValue("Downloading") + $"({percentage}%)";
        }

        private void HypeLink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.kooboo.com");
        }

        #endregion 自动升级程序
    }
}