#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.Basic.Model;
using AutoX.DB;

#endregion

#endregion

namespace AutoX
{
    public partial class MainWindow : IDisposable
    {
        private bool disposed; // to detect redundant calls

        public void Dispose()
        {
            Dispose(true);
            //GC.SupressFinalize(this);
        }

        public XElement GetDataObject(string id)
        {
            return DBFactory.GetData().Read(id);
        }

        private bool BeforeActionCheck(TreeView treeView, string action, string objName)
        {
            var selected = (TreeViewItem) treeView.SelectedItem;
            if (selected != null)
            {
                var type = ((XElement) selected.DataContext).GetAttributeValue(Constants._TYPE);
                if (type == null)
                    return false;
                var query = from o in _xValidation.Descendants()
                    where o.GetAttributeValue(Constants.ACTION).Equals(action)
                          && o.GetAttributeValue("Type").Equals(type)
                          && o.GetAttributeValue("Object").Equals(objName)
                    select o;
                return (query.Any());
            }

            return false;
        }

        private static TreeViewItem GetItemFromXElement(XElement element, string parentId)
        {
            var guid = element.GetAttributeValue(Constants._ID);
            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
                element.SetAttributeValue(Constants._ID, guid);
            }
            var rootPart = element.GetRootPartElement();
            rootPart.SetAttributeValue(Constants.PARENT_ID, parentId);

            if (!DBFactory.GetData().Save(rootPart))
            {
                MessageBox.Show("update Tree item Failed.");
            }
            else
            {
                var itself = rootPart.GetTreeViewItemFromXElement();

                foreach (XElement kid in element.Descendants())
                {
                    itself.Items.Add(GetItemFromXElement(kid, guid));
                }
                return itself;
            }

            return null;
        }

        private static void Delete(TreeView parent)
        {
            var selected = (TreeViewItem) parent.SelectedItem;
            if (selected == null)
            {
                return;
            }
            var messageBoxResult = MessageBox.Show(
                "Do you really want to delete this item? Cannot be recover!", "Delete Item",
                MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes) return;
            var xElement = (XElement) selected.DataContext;
            if (xElement.GetAttributeValue(Constants.PARENT_ID).Equals(Configuration.Settings("Root")))
            {
                MessageBox.Show("Cannot Delete Root Item!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var parentItem = selected.Parent as TreeViewItem;
            if (parentItem == null)
            {
                MessageBox.Show("Cannot Delete an Item without correct xml format data context!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var toBeDeletedId = xElement.GetAttributeValue(Constants._ID);

            // xElement.SetAttributeValue(Constants.PARENT_ID, "Deleted");
            DBFactory.GetData().Delete(toBeDeletedId);
            parentItem.Items.Remove(selected);
        }

        private static TreeViewItem AddNewItemToTree(TreeView tree, XElement xElement)
        {
            var dialog = new XElementDialog(xElement, false);
            dialog.ShowDialog();
            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value) return null;
            var xE = dialog.GetElement();
            var selected = (TreeViewItem) tree.SelectedItem;
            var xParent = (XElement) selected.DataContext;
            var parentId = xParent.GetAttributeValue(Constants._ID);

            xE.SetAttributeValue(Constants.PARENT_ID, parentId);

            if (!DBFactory.GetData().Save(xE))
            {
                MessageBox.Show("Add Tree item Failed.");
                return null;
            }
            var ret = xE.GetTreeViewItemFromXElement();
            selected.Items.Add(ret);
            return ret;
        }

        //private void InitTree(ItemsControl tree, string rootId)
        //{
        //    var sRoot = Communication.GetInstance().GetById(rootId);

        //    var xRoot = XElement.Parse(sRoot);
        //    var result = xRoot.GetAttributeValue(Constants.RESULT);
        //    if (!string.IsNullOrEmpty(result))
        //    {
        //        MessageBox.Show("Get Tree Root Failed. id=" + rootId + "\nReason:" + xRoot.GetAttributeValue("Reason"));
        //        return;
        //    }
        //    tree.Items.Clear();
        //    tree.Items.Add(xRoot.GetTreeViewItemFromXElement());
        //}

        private void DoubleClickOnTree(TreeView treeView)
        {
            //double click on an item, then get its children
            var selected = treeView.SelectedItem as TreeViewItem;
            if (selected == null)
            {
                return;
            }
            var treeViewName = treeView.Name;
            var parent = selected.DataContext as XElement;
            if (parent == null) return;
            var parentId = parent.GetAttributeValue(Constants._ID);

            if (parent.Name.ToString().Equals(Constants.SCRIPT) && treeViewName.Equals("ProjectTreeView"))
            {
                AddTestDesigner(selected);
                return;
            }
            var xRoot = DBFactory.GetData().GetChildren(parentId);

            if (parent.Name.ToString().Equals(Constants.RESULT))
            {
                //load its children to TestCaseResultTable
                //LoadResultTableOfTreeViewItem(selected, xRoot);

                return;
            }
            HandleDoubleClick(selected, treeViewName, xRoot);
        }

        private static void HandleDoubleClick(TreeViewItem selected, string treeViewName, XElement xRoot)
        {
            selected.Items.Clear();
            foreach (XElement kid in xRoot.Descendants())
            {
                if (treeViewName.Equals("SuiteTree"))
                {
                    var type = kid.GetAttributeValue(Constants._TYPE);
                    if (type != null && type.Equals(Constants.SCRIPT))
                    {
                        var scriptType = kid.GetAttributeValue(Constants.SCRIPT_TYPE);
                        if (!scriptType.Equals("TestSuite")) continue;
                    }
                }
                var newItem = kid.GetTreeViewItemFromXElement();
                selected.Items.Add(newItem);
            }
        }

        private void LoadResultTableOfTreeViewItem(TreeViewItem selected, XElement xRoot)
        {
            _testCaseResultSource.Clear();
            _testStepSource.Clear();
            selected.Items.Clear();
            foreach (XElement kid in xRoot.Descendants())
            {
                var kind = kid.Name.ToString();
                if (kind.Equals(Constants.RESULT) || kind.Equals("AutoX.Basic.Model.Result"))
                {
                    var testcaseresult = kid.GetDataObjectFromXElement() as Result;
                    selected.Items.Add(kid.GetTreeViewItemFromXElement());
                    _testCaseResultSource.Add(testcaseresult);
                }
                if (kind.Equals("StepResult") || kind.Equals("AutoX.Basic.Model.StepResult"))
                {
                    var testStepResult = kid.GetDataObjectFromXElement() as StepResult;
                    _testStepSource.Add(testStepResult);
                }
            }

            TestCaseResultTable.ItemsSource = _testCaseResultSource.Get();
            TestStepsResultTable.ItemsSource = _testStepSource.Get();
        }

        private void TestResultTreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selected = TestResultTree.SelectedItem as TreeViewItem;
            if (selected == null)
            {
                return;
            }
            var parent = selected.DataContext as XElement;
            if (parent == null) return;
            var parentId = parent.GetAttributeValue(Constants._ID);
            var xRoot = DBFactory.GetData().GetChildren(parentId);
            Log.Debug(xRoot.ToString());
            if (parent.Name.ToString().Equals(Constants.RESULT))
            {
                //load its children to TestCaseResultTable
                LoadResultTableOfTreeViewItem(selected, xRoot);
            }
        }

        private static void Edit(TreeViewItem selected)
        {
            if (selected == null)
            {
                return;
            }
            var element = ((XElement) selected.DataContext);
            var dialog = new XElementDialog(element, false);
            dialog.ShowDialog();
            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value) return;
            var xElement = dialog.GetElement();

            if (!DBFactory.GetData().Save(xElement))
            {
                MessageBox.Show("update Tree item Failed.");
                return;
            }
            selected.UpdateTreeViewItem(xElement);
        }

        private static void Edit(XElement element)
        {
            var dialog = new XElementDialog(element, false);
            dialog.ShowDialog();
            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value) return;
            var xElement = dialog.GetElement();

            if (!DBFactory.GetData().Save(xElement))
            {
                MessageBox.Show("update Tree item Failed.");
            }
        }

        private TreeViewItem GetInitScriptXElement(string type)
        {
            var xElement =
                XElement.Parse(
                    @"<Script Name='NewTest " + type + "' ScriptType='Test" + type +
                    "' Description='Please add description here' _type='Script' Content='' _id='" +
                    Guid.NewGuid() + "' />");
            var ret = AddNewItemToTree(ProjectTreeView, xElement);
            return ret;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_autoClient != null)
                    {
                        _autoClient.Dispose();
                    }
                }

                disposed = true;
            }
        }
    }
}