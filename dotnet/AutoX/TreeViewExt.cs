// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;
using AutoX.Basic;

#endregion

namespace AutoX
{
    public static class TreeViewExt
    {
        public static TreeViewItem GetTreeViewItemFromXElement(this XElement xElement)
        {
            var treeViewItem = new TreeViewItem {DataContext = xElement};
            treeViewItem.UpdateTreeViewItem(xElement);
            return treeViewItem;
        }

        public static void UpdateTreeViewItem(this TreeViewItem treeViewItem, XElement xElement)
        {
            treeViewItem.DataContext = xElement;
            var head = new StackPanel {Orientation = Orientation.Horizontal};
            Image image = null;
            var text = new TextBlock();
            var type = xElement.GetAttributeValue("_type");
            var name = xElement.GetAttributeValue("Name");

            if (!string.IsNullOrWhiteSpace(type))
            {
                var bitmap = ImageList.GetInstance().Get(type);
                if (bitmap != null)
                {
                    // this is important, remove it, the tree will be 20 times slower
                    Dispatcher.CurrentDispatcher.BeginInvoke((DispatcherPriority.Normal), (Action) (() =>
                        {
                            image = new Image
                                {
                                    Source
                                        =
                                        bitmap,
                                    Stretch
                                        =
                                        Stretch
                                            .
                                            Uniform,
                                    Width
                                        =
                                        text
                                            .
                                            FontSize,
                                    Height
                                        =
                                        text
                                            .
                                            FontSize,
                                    MinHeight
                                        =
                                        16,
                                    MinWidth
                                        =
                                        16,
                                    ToolTip
                                        =
                                        xElement
                                            .
                                            GetAttributeValue
                                            ("Description")
                                };
                            //head.Children.Add(image);
                            head.
                                Children
                                .Insert
                                (0,
                                 image);
                        }
                                                                                                   ));
                }
            }

            text.Text = name;
            text.ToolTip = new ToolTip {Content = xElement.GetSimpleDescriptionFromXElement()};

            if (image == null)
                text.ToolTip = new ToolTip {Content = xElement.GetText()};

            head.Children.Add(text);

            treeViewItem.Header = head;
        }

        public static bool FilterTreeItem(this TreeViewItem tree, string value)
        {
            if (tree == null) return false;
            var visible = false;

            foreach (object kid in tree.Items)
            {
                var kidItem = kid as TreeViewItem;
                if (kidItem == null) continue;
                if (FilterTreeItem(kidItem, value)) visible = true;
            }
            if (!visible)
            {
                var data = tree.DataContext as XElement;
                if (string.IsNullOrEmpty(value))
                    visible = true;
                else
                {
                    if (data != null && data.ToString().Contains(value)) visible = true;
                }
            }

            tree.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

            return visible;
        }

        private static string GetNTab(int n)
        {
            var tab = "";
            for (var i = 0; i < n; i++)
            {
                tab += "\t";
            }
            return tab;
        }

        public static string GetSimpleDescriptionFromXElement(this XElement element)
        {
            return element.GetSimpleDescriptionFromXElement(0);
        }

        public static string GetSimpleDescriptionFromXElement(this XElement element, int level)
        {
            if (element == null)
                return "element is null!";
            var result = "";
            result += GetNTab(level) + element.Name;
            if (!String.IsNullOrEmpty(element.Value))
            {
                result += " : [" + element.Value + "] ";
            }
            result += "\n";
            result = element.Attributes().Aggregate(result,
                                                    (current, xa) =>
                                                    current + (GetNTab(level + 1) + xa.Name + "=" + xa.Value + "\n"));
            if (result.Length > 1024)
                return result.Substring(0, 1000) + " ...";
            foreach (XElement xe in element.Elements())
            {
                result += GetSimpleDescriptionFromXElement(xe, level + 1);
                if (result.Length > 1024)
                    return result.Substring(0, 1000) + " ...";
            }
            if (result.Length > 1024)
                return result.Substring(0, 1000) + " ...";
            return result;
        }

        public static string GetText(this XElement element)
        {
            if (element == null)
                return "Element is NULL";
            var retString = element.Name + "\n";
            if (!string.IsNullOrWhiteSpace(element.Value))
                retString += element.Value + "\n";
            return element.Attributes().Aggregate(retString,
                                                  (current, a) => current + (" " + a.Name + " : " + a.Value + "\n"));
        }
    }
}