// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using AutoX.Basic;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

#endregion

namespace AutoX
{
    /// <summary>
    ///   Interaction logic for XElementDialog.xaml
    /// </summary>
    public partial class XElementDialog
    {
        private readonly string _backup;
        private readonly bool _isReadOnly;
        private XElement _content;

        public XElementDialog(XElement element, bool isReadOnly)
        {
            if (element == null)
                return;
            InitializeComponent();
            _backup = element.ToString(SaveOptions.None);
            _content = element;
            _isReadOnly = isReadOnly;
            SrcEdit.Text = _content.ToString(SaveOptions.None);

            //Refresh(element, isReadOnly);
        }

        public XElement GetElement()
        {
            return _content;
        }

        private void Refresh()
        {
            Refresh(_content, _isReadOnly);
        }

        private void Refresh(XElement element, bool isReadOnly)
        {
            XName.Text = element.Name.ToString();
            XName.TextWrapping = TextWrapping.Wrap;
            XName.IsReadOnly = isReadOnly;
            XName.Tag = element.Name.ToString();
            XName.TextChanged += XNameTextChanged;

            var count = ContentGrid.Children.Count;
            ContentGrid.Children.RemoveRange(0, count);

            //ContentGrid.Rows = element.Attributes().Count();
            if (_isReadOnly)
            {
                BtnOk.Visibility = Visibility.Hidden;
                BtnNew.Visibility = Visibility.Hidden;
            }
            else
            {
                BtnOk.Visibility = Visibility.Visible;
                BtnNew.Visibility = Visibility.Visible;
            }

            foreach (XAttribute a in element.Attributes())
            {
                AddPairControls(a);
            }
        }

        private void XNameTextChanged(object sender, TextChangedEventArgs e)
        {
            var sent = sender as TextBox;
            if (sent == null) return;

            var nameX = sent.Tag.ToString();
            var valueX = sent.Text;
            if (string.IsNullOrWhiteSpace(valueX))
            {
                var choice = MessageBox.Show(
                    "Set element name means you want to delete it.\nAre you sure?", "Delete an Element",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (choice == MessageBoxResult.Yes)
                    _content.Remove();
                else
                {
                    XName.Text = nameX;
                }
            }
            else
            {
                XName.Name = valueX;
                _content.Name = valueX;
            }
        }

        private void AddPairControls(XAttribute a)
        {
            if (_isReadOnly
                || a.Name.ToString().Equals(Constants._TYPE)
                || a.Name.ToString().Equals(Constants.PARENT_ID)
                || a.Name.ToString().Equals(Constants.SCRIPT_TYPE)
                || a.Name.ToString().Equals(Constants._ID)
                )
            {
                var label = new TextBlock
                    {
                        Text = a.Name.ToString(),
                        TextWrapping = TextWrapping.Wrap,
                        FontWeight = FontWeights.Bold
                    };
                var value = new TextBlock { Text = a.Value, TextWrapping = TextWrapping.Wrap };
                ContentGrid.Children.Add(label);
                ContentGrid.Children.Add(value);
            }
            else
            {
                // orignalidea: we thought we can change the name of attribute
                //var label = new TextBox
                //                {
                //                    Text = a.Name.ToString(),
                //                    Tag = a.Name.ToString(),
                //                    TextWrapping = TextWrapping.Wrap
                //                };
                //label.TextChanged += LabelTextChanged;
                var label = new TextBlock { Text = a.Name.ToString(), FontWeight = FontWeights.Bold };
                ContentGrid.Children.Add(label);

                var value = new TextBox
                    {
                        Text = a.Value,
                        Tag = a.Name.ToString(),
                        MaxWidth = 768,
                        MaxHeight = 256,
                        TextWrapping = TextWrapping.WrapWithOverflow,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                        AcceptsReturn = true
                    };
                value.TextChanged += ValueTextChanged;
                ContentGrid.Children.Add(value);
            }

            //ResizeMode = ResizeMode.NoResize;
        }

        private void ValueTextChanged(object sender, TextChangedEventArgs e)
        {
            var nameX = ((TextBox)sender).Tag.ToString();
            var valueX = ((TextBox)sender).Text;
            _content.SetAttributeValue(nameX, valueX);
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            //do nothing, then restore from backup
            _content = XElement.Parse(_backup);

            DialogResult = false;
            Close();
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }

        private void ButtonNewClick(object sender, RoutedEventArgs e)
        {
            var newAttrName = "NewAttribute";

            var i = 1;
            while (_content.Attribute(newAttrName) != null)
            {
                newAttrName = "NewAttribute" + i.ToString(CultureInfo.InvariantCulture).Trim();
                i++;
            }
            var iDlg = new InfoDialog { InfoContent = newAttrName };
            iDlg.ShowDialog();
            if (iDlg.DialogResult == true)
            {
                newAttrName = iDlg.InfoContent;
                var newAttribute = new XAttribute(newAttrName,
                                                  "NewValue" + i.ToString(CultureInfo.InvariantCulture).Trim());
                _content.Add(newAttribute);

                //ContentGrid.Rows += 1;
                AddPairControls(newAttribute);
            }
        }

        private void SrcOkClick(object sender, RoutedEventArgs e)
        {
            //valid first
            try
            {
                var newNode = XElement.Parse(SrcEdit.Text);

                _content.ReplaceAll(newNode.Elements());
                _content.ReplaceAttributes(newNode.Attributes());

                //Refresh(_content, _isReadOnly);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not Valid XML:\n" + ex.Message);
                return;
            }

            DialogResult = true;

            Close();
        }

        private void FocusOnSrc()
        {
            SrcEdit.Text = _content.ToString(SaveOptions.None);
        }

        private void TabChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (Source.IsSelected)
                {
                    FocusOnSrc();
                }
                if (Node.IsSelected)
                {
                    Refresh();
                }
            }
        }

        private void SrcChanged(object sender, TextChangedEventArgs e)
        {
            //valid first
            try
            {
                var newNode = XElement.Parse(SrcEdit.Text);

                _content.ReplaceAll(newNode.Elements());
                _content.ReplaceAttributes(newNode.Attributes());

                //_content = XElement.Parse(SrcEdit.Text);

                //Refresh(_content, _isReadOnly);
            }
            catch (Exception)
            {
                //if not valid, don't change anything
                _content.ReplaceWith(XElement.Parse(_backup));
            }

            //Refresh();
        }
    }
}