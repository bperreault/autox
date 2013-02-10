using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using AutoX.Basic;

namespace AutoX
{
    /// <summary>
    /// Interaction logic for BrowsersDialog.xaml
    /// </summary>
    public partial class BrowsersDialog
    {
        public BrowsersDialog()
        {
            InitializeComponent();
            AUT_Version.Text = Configuration.Settings("AUTVersion", "AutoX v0.45");
            AUT_Build.Text = Configuration.Settings("AUTBuild", "0.1.1");
            //load data from browsers.xml
            string browsers = File.ReadAllText("Browsers.xml");
            _choices = XElement.Parse(browsers);
            //set default data
            BrowserType.Items.Clear();
            foreach (var browser in _choices.Elements())
            {
                var current = new ListBoxItem() {Content = browser.Name};
                BrowserType.Items.Add(current);
                if (Content.Equals("Firefox"))
                    BrowserType.SelectedItem = current;
            }
            
        }

        private readonly XElement _choices;
        public XElement BrowserSetting { get; private set; }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            BrowserSetting.SetAttributeValue("AUTVersion", AUT_Version.Text);
            BrowserSetting.SetAttributeValue("AUTBuild", AUT_Build.Text);
            if (Configuration.Settings("AUTVersion", null) == null)
            {
                Configuration.Set("AUTVersion", AUT_Version.Text);
                Configuration.SaveSettings();
            }
            if (Configuration.Settings("AUTBuild", null) == null)
            {
                Configuration.Set("AUTBuild", AUT_Build.Text);
                Configuration.SaveSettings();
            }
            
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
        }

        private void Platform_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Platform.SelectedItem == null) return;
            string platform = ((ListBoxItem)Platform.SelectedItem).Content.ToString();
            string browser = ((ListBoxItem)BrowserType.SelectedItem).Content.ToString();
            Version.Items.Clear();
            if (browser.Equals("Chrome"))
            {
                BrowserSetting = new XElement(browser);
                BrowserSetting.SetAttributeValue("Platform", platform);
                return;
            }
            var query = from o in _choices.Element(browser).Elements()
                        where o.GetAttributeValue(Constants._NAME).Equals(platform)
                        select o;
                        

            foreach (var descendant in query.First().Descendants())
            {
                Version.Items.Add(new ListBoxItem() {Content = descendant.Attribute("value").Value});
            }
        }

        private void BrowserType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string browser = ((ListBoxItem) BrowserType.SelectedItem).Content.ToString();
            Platform.Items.Clear();
            Version.Items.Clear();
            foreach (var descendant in _choices.Element(browser).Descendants("Platform"))
            {
                string name = descendant.Attribute(Constants._NAME).Value;
                string value = descendant.Attribute("value").Value;
                Platform.Items.Add(new ListBoxItem() {Content = name, Tag = value});
            }
            
        }

        private void Version_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Version.SelectedItem == null)
                return;
            string platform = ((ListBoxItem)Platform.SelectedItem).Tag.ToString();
            string browser = ((ListBoxItem)BrowserType.SelectedItem).Content.ToString();
            string version = ((ListBoxItem)Version.SelectedItem).Content.ToString();
            BrowserSetting = new XElement(browser);
            BrowserSetting.SetAttributeValue("Platform", platform);
            BrowserSetting.SetAttributeValue("Version",version);
        }

       
    }
}
