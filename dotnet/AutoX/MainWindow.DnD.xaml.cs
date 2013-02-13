// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.Comm;
using AutoX.DB;

#endregion

namespace AutoX
{
    public partial class MainWindow
    {
        private Point _startPoint;

        private void PreviewMouseMoveOnTree(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            var mousePos = e.GetPosition(sender as IInputElement);
            var diff = _startPoint - mousePos;

            DragTreeviewItem(e, diff);
        }

        private void TreeViewDragOver(object sender, DragEventArgs e)
        {
            if (sender == null) return;
            var s = sender as IInputElement;
            if (s == null) return;
            var currentPosition = e.GetPosition(s);
            var diff = _startPoint - currentPosition;

            if (!(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                !(Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) return;
            var item = GetNearestContainer(e.OriginalSource as UIElement);
            var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
            e.Effects = CheckValidDrop(item, data) ? DragDropEffects.Copy : DragDropEffects.None;
            //if(item!=null)
              //  item.IsSelected = true;
        }

        private void TreeViewDrop(object sender, DragEventArgs e)
        {
            //TODO drag and drop error, target and source are reversed
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                var item = GetNearestContainer(e.OriginalSource as UIElement);
                var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
                if (data == null) return;
                if (!CheckValidDrop(item, data)) return;
                var xTarget = item.DataContext as XElement;
                var parentId = xTarget.GetAttributeValue(Constants._ID);
                data.SetAttributeValue(Constants.PARENT_ID, parentId);
                
                if (!Data.Save(data))
                {
                    MessageBox.Show("update Tree item Failed.");
                }
                else
                {
                    //sender to find the source item, then delete it
                    var toDelete = FindItemOnTree((sender as TreeView), Constants._ID,
                                                  data.GetAttributeValue(Constants._ID));
                    if (toDelete != null)
                    {
                        var parent = toDelete.Parent as TreeViewItem;
                        if (parent != null) parent.Items.Remove(toDelete);
                    }

                    item.Items.Add(data.GetTreeViewItemFromXElement());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Happen:\n" + ex.Message);
            }
        }


        private static TreeViewItem FindItemOnTree(ItemsControl tree, string name, string value)
        {
            foreach (object item in tree.Items)
            {
                var ti = item as TreeViewItem;
                if (ti == null) continue;
                return FindItemOnTreeViewItem(ti, name, value);
            }
            return null;
        }

        private static TreeViewItem FindItemOnTreeViewItem(TreeViewItem ti, string name, string value)
        {
            var data = ti.DataContext as XElement;
            if (data != null && data.GetAttributeValue(name).Equals(value))
                return ti;
            foreach (object kid in ti.Items)
            {
                var k = kid as TreeViewItem;
                if (k == null) continue;
                var answer = FindItemOnTreeViewItem(k, name, value);
                if (answer != null) return answer;
            }
            return null;
        }

        private static bool CheckValidDrop(FrameworkElement item, XElement data)
        {
            if (item == null)
                return false;
            if (data == null)
                return false;
            //rule 1: only folder accept drop
            var xTarget = item.DataContext as XElement;
            if (xTarget == null) return false;
            var tag = xTarget.Name.ToString();
            if (!tag.Equals("Folder")) return false;
            //rule 2: don't waste your time move to your parent
            var parentId = xTarget.GetAttributeValue(Constants._ID);
            if (parentId.Equals(data.GetAttributeValue(Constants.PARENT_ID)))
                return false;
            return true;
        }

        private static TreeViewItem GetNearestContainer(UIElement element)
        {
            var container = element as TreeViewItem;
            while (container == null && element != null)
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        private static void DragTreeviewItem(MouseEventArgs e, Vector diff)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                // Get the dragged ListViewItem

                var listViewItem =
                    FindAnchestor<TreeViewItem>((DependencyObject) e.OriginalSource);
                if (listViewItem == null)
                    return;

                var xe = listViewItem.DataContext as XElement;
                if (xe == null)
                    return;

                // Initialize the drag & drop operation
                var dragData = new DataObject(Constants.DATA_FORMAT, xe);
                try
                {
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
                catch (Exception exception)
                {
                    Log.Debug("need to improve here:" + exception.Message);
                }
            }
        }

        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T) current;
                }
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);
            return null;
        }

        private void TreePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(sender as IInputElement);
        }

        private void ClientTableDragEnter(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
            if (data == null)
                return;

            e.Effects = (DragDropEffects.Link & e.AllowedEffects);
            e.Handled = true;
        }

        private void InstanceTableDragEnter(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
            if (data == null)
                return;

            e.Effects = (DragDropEffects.Link & e.AllowedEffects);
            e.Handled = true;
        }

        private void ClientTableDragOver(object sender, DragEventArgs e)
        {
            var r = sender as DataGridRow;
            if (r == null) return;
            ClientTable.SelectedItem = r.Item;
            ClientTable.ScrollIntoView(r.Item);
        }


        private void ClientTableLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.DragOver += ClientTableDragOver;
        }

        private void ClientTableDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
            if (data == null) return;
            var client = ClientTable.SelectedItem as Computer;
            if (client == null) return;

            var newInstance = new Instance
                {
                    ClientId = client._id,
                    ClientName = client.ComputerName,
                    Language = "Default",
                    _id = Guid.NewGuid().ToString(),
                    ScriptGUID = data.GetAttributeValue(Constants._ID),
                    SuiteName = data.GetAttributeValue(Constants.NAME),
                    Status = "Ready",
                    TestName = "NewTest"
                };
            var sRoot = Communication.GetInstance().SetInstanceInfo(newInstance.GetXElementFromObject());
            var xRoot = XElement.Parse(sRoot);
            var result = xRoot.GetAttributeValue(Constants.RESULT);
            if (string.IsNullOrEmpty(result)) return;
            if (result.Equals("Failed"))
            {
                MessageBox.Show("Update Instance failed!\nReason:" +
                                xRoot.GetAttributeValue("Reason"));
                return;
            }
            _instanceSource.Add(newInstance);
            InstanceTable.ItemsSource = _instanceSource.Get();

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void InstanceTableDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(Constants.DATA_FORMAT) as XElement;
            if (data == null) return;
            

            var newInstance = new Instance
            {
                
                Language = "Default",
                _id = Guid.NewGuid().ToString(),
                ScriptGUID = data.GetAttributeValue(Constants._ID),
                SuiteName = data.GetAttributeValue(Constants.NAME),
                Status = "Ready",
                TestName = "NewTest"
            };
            var sRoot = Communication.GetInstance().SetInstanceInfo(newInstance.GetXElementFromObject());
            var xRoot = XElement.Parse(sRoot);
            var result = xRoot.GetAttributeValue(Constants.RESULT);
            if (string.IsNullOrEmpty(result)) return;
            if (result.Equals("Failed"))
            {
                MessageBox.Show("Update Instance failed!\nReason:" +
                                xRoot.GetAttributeValue("Reason"));
                return;
            }
            _instanceSource.Add(newInstance);
            InstanceTable.ItemsSource = _instanceSource.Get();

            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        
    }
}