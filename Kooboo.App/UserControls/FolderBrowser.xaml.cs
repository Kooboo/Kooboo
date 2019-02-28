//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using winform = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Kooboo.App.UserControls
{
    /// <summary>
    /// Interaction logic for FolderBrowser.xaml
    /// </summary>
    public partial class FolderBrowser : UserControl
    {
        public string Text
        {
            get { return GetValue(TextProperty)?.ToString(); }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
           nameof(Text),
           typeof(string),
           typeof(FolderBrowser),
           new UIPropertyMetadata(string.Empty)
           );

        public FolderBrowser()
        {
            InitializeComponent();
        }

        private void BrowserButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new winform.FolderBrowserDialog();
            if (dialog.ShowDialog() == winform.DialogResult.OK)
            {
                Text = dialog.SelectedPath;
            }
        }
    }
}
