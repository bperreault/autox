#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

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
        private bool _disposed; // to detect redundant calls

        public void Dispose()
        {
            Dispose(true);
            //GC.SupressFinalize(this);
        }

        public XElement GetDataObject(string id)
        {
            return DBFactory.GetData().Read(id);
        }

        public Config GetConfig()
        {
            var config = Configuration.Clone();
            
            return config;
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

        private static async void Delete(TreeView parent)
        {
            await Task.Factory.StartNew(()=>  DeleteTask(parent));
        }

        private static void DeleteTask(TreeView parent)
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

        private static void ExportResultTask(TreeView parent)
        {
            var selected = (TreeViewItem)parent.SelectedItem;
            if (selected == null)
            {
                return;
            }
            var messageBoxResult = MessageBox.Show(
                "Do you really want to export this result? It may take long time!", "Export Results",
                MessageBoxButton.YesNo);

            if (messageBoxResult != MessageBoxResult.Yes) return;
            var xElement = (XElement)selected.DataContext;
            if (xElement.GetAttributeValue(Constants.PARENT_ID).Equals(Configuration.Settings("Root")))
            {
                MessageBox.Show("Cannot Export Root Item!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var parentItem = selected.Parent as TreeViewItem;
            if (parentItem == null)
            {
                MessageBox.Show("Cannot Export an Result without correct xml format data context!", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var htmlString = GetHtmlHeader()+ ExportReulstToHtml(xElement)+GetHtmlFooter();
            var htmlResultsDialog = new SaveFileDialog
            {
                FileName = "Results",
                DefaultExt = "html",
                Filter = "HTML format (*.html)|*.html"
            };
            if (!htmlResultsDialog.ShowDialog().Value) return;
            var fileName = htmlResultsDialog.FileName;
            File.WriteAllText(fileName, htmlString);

        }

        private static string GetHtmlHeader()
        {
            return 
"<html><head><title>Automation Results</title><meta http-equiv=\"Content-Type\" content=\"text/html; charset=gb2312\"><script language=\"javascript\">" +
"function ShowOrUnShow(item){ if (!item.parentNode.getElementsByTagName(\"ul\")[0]) return; var x = item.parentNode.getElementsByTagName(\"ul\")[0];x.style.display = (x.style.display == \"\") ? 'none' : \"\"; }  "+
"function ExpandAll(){var e = document.getElementsByTagName(\"ul\");for ( var i = 0, len = e.length; i < len; i++ ){e[i].style.display = \"\";}}"+
"function CollapseAll(){var e = document.getElementsByTagName(\"ul\");for ( var i = 0, len = e.length; i < len; i++ ){e[i].style.display = \"none\";}}"+
"var popUpWin = 0;"+
"function popUpWindow(content){if(popUpWin){if(!popUpWin.closed) popUpWin.close();}popUpWin = window.open('','','width=800,height=600,resizeable,scrollbars');var data=\"<html><body><img src='data:image/jpg;base64,\"+content.getAttribute('snapshot')+\"' /></body></html>\";popUpWin.document.write(data);}"+
"</script></head><a href=\"#\" onclick=\"ExpandAll()\" >Expand</a><span>    </span><a href=\"#\" onclick=\"CollapseAll()\">Collapse</a><div>";
        }

        private static string GetHtmlFooter()
        {
            return "</div></html>";
        }
        private static string ExportReulstToHtml(XElement xElement)
        {
            var toBeExportdId = xElement.GetAttributeValue(Constants._ID);
            var tag = xElement.Name.ToString();
            if (tag.Equals("Result"))
            {
                //this is a result node
                var name = xElement.GetAttributeValue(Constants.NAME);
                var origianl = xElement.GetAttributeValue("Original");
                var final = xElement.GetAttributeValue("Final");
                if (string.IsNullOrEmpty(final))
                    final = "&nbsp";
                var created = xElement.GetAttributeValue("Created");
                var updated = xElement.GetAttributeValue("Updated");
                var color = final.Equals("Success") ? "GREEN" : "RED";
                var ret =
                    "<li><span title=\"Start:" + created + "&#10End:" + updated + "\"   onclick=\"ShowOrUnShow(this)\"> " + name + " </span> <span title=\"Original:" + origianl + "&#10Final:" + final + "\" style=\"color:" + color + "\" > " + final + " </span><ul>";
                var children = DBFactory.GetData().GetChildren(toBeExportdId);
                var kidTag =children.Descendants().First().Name.ToString();
                if (kidTag.Equals("Result"))
                {
                    foreach (var kid in children.Descendants())
                    {
                        ret += ExportReulstToHtml(kid);
                    }
                }
                else
                {
                    //add table header
                    ret +=
                        "<table cellspacing=\"0\" cellpadding=\"1\" border=\"1\"  width=\"100%\"><thead><tr id=\"headingrow\">" +
                        "<th  nowrap=\"nowrap\" >Action </th>" +
                        "<th  nowrap=\"nowrap\" >UI Object </th>" +
                        "<th  nowrap=\"nowrap\" >Data </th>" +
                        "<th  nowrap=\"nowrap\" >Result </th>" +
                        "<th  nowrap=\"nowrap\" >Duration </th>" +
                        "<th  nowrap=\"nowrap\" >Reason </th>" +"</tr></thead><tbody>";
                    foreach (var kid in children.Descendants())
                    {
                        ret += "<tr>";
                        var action = kid.GetAttributeValue("Action");
                        var uiobject = kid.GetAttributeValue("UIObject");
                        var data = kid.GetAttributeValue("Data");
                        var result = kid.GetAttributeValue("Result");
                        var duration = kid.GetAttributeValue("Duration");
                        var reason = kid.GetAttributeValue("Reason");
                        if (string.IsNullOrEmpty(reason))
                            reason = "&nbsp&nbsp";
                        var snapshot = kid.GetAttributeValue("Link");
                        var starttime = kid.GetAttributeValue("StartTime");
                        var endtime = kid.GetAttributeValue("EndTime");
                        var id = kid.GetAttributeValue("_id");
                        var uiid = kid.GetAttributeValue("UIId");
                        var colour = result.Equals("Success") ? "GREEN" : "RED";
                        ret += "<td title=\""+id+"\">"+action+"</td>";
                        ret += "<td title=\"" + uiid + "\">" + uiobject + "</td>";
                        ret += "<td>"+data+"</td>";
                        ret += "<td align=\"center\" style=\"color:" + colour + "\">" + result.ToUpper() + "</td>";
                        ret += "<td align=\"right\" title=\"Start At:" + starttime+"&#10End At:"+endtime + "\">" + duration + "</td>";
                        ret += "<td><a href=\"#\" onclick=\"javascript:popUpWindow(this)\" snapshot=\""+snapshot+"\">"+reason+"</a></td>";
                        ret += "</tr>";
                    }
                    //add table footer
                    ret += "</tbody></table>";
                }
                return ret + "</ul></li>";
            }
            return "";
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

        private async Task DoubleClickOnTree(TreeView treeView)
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
            await HandleDoubleClick(selected, treeViewName, xRoot);
        }

        private async Task HandleDoubleClick(TreeViewItem selected, string treeViewName, XElement xRoot)
        {
            selected.Items.Clear();
            foreach (var kid in xRoot.Descendants())
            {
                if (treeViewName.Equals("SuiteTree"))
                {
                    var type = kid.GetAttributeValue(Constants._TYPE);
                    if (type != null && type.Equals(Constants.SCRIPT))
                    {
                        var scriptType = kid.GetAttributeValue(Constants.SCRIPT_TYPE);

                        if (!scriptType.Equals("TestSuite")) continue;
                        var scriptContent = kid.GetAttributeValue("Content");
                        if(string.IsNullOrEmpty(scriptContent)) continue;
                        var scriptMaturity = XElement.Parse(scriptContent).GetAttributeValue("Maturity");
                        if(!Configuration.Settings("Maturity","Playground;Stadium").Contains(scriptMaturity)) continue;
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
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_autoClient != null)
                    {
                        _autoClient.Dispose();
                    }
                }

                _disposed = true;
            }
        }
    }
}