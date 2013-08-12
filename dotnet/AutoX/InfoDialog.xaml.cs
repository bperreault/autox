#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Windows;

#endregion

#endregion

namespace AutoX
{
    /// <summary>
    ///   Interaction logic for InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog
    {
        public InfoDialog()
        {
            InitializeComponent();
            
        }

        private string _infoContent;
        public string InfoContent
        {
            get { return _infoContent; }
            set { _infoContent = value.Trim(); infomationContent.Text = _infoContent; }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            _infoContent = infomationContent.Text.Trim();
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}