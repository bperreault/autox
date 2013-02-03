// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Windows;

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

        public string InfoContent
        {
            get { return infomationContent.Text.Trim(); }
            set { infomationContent.Text = value; }
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}