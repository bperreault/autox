#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

using System.Linq;

#region

using System;
using System.Collections;
using System.Windows;
using System.Xml.Linq;
using AutoX.Basic;
using AutoX.DB;
using System.Globalization;

#endregion

#endregion

namespace AutoX.Activities
{
    /// <summary>
    ///   Interaction logic for StepsDialog.xaml
    /// </summary>
    public partial class StepsDialog
    {
        protected ArrayList Actions = Configuration.GetSupportedAction();
        public IHost Host = HostManager.GetInstance().GetHost();
        private string _stepId;
        protected ArrayList UserData = new ArrayList();

        public StepsDialog()
        {
            InitializeComponent();
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }

        private void ButtonReload(object sender, RoutedEventArgs e)
        {
            var p =
                XElement.Parse(DBFactory.GetData().Read(_stepId).GetAttributeValue(Constants.CONTENT))
                    .GetAttributeValue("Steps");
            var steps = Utilities.GetStepsList(p, Actions, Host);
            var current = StepsTable.ItemsSource as ArrayList;

            //add new things and remove something
            var newList = new ArrayList();
            if (current != null)
                foreach (var currentStep in current.Cast<Step>().Where(currentStep => InSteps(currentStep, steps)))
                {
                    newList.Add(currentStep);
                }
            foreach (var original in steps.Cast<Step>().Where(original => !InSteps(original, newList)))
            {
                newList.Add(original);
            }
            StepsTable.ItemsSource = newList;
        }

        private bool InSteps(Step oneStep, ArrayList steps)
        {
            return steps.Cast<Step>().Any(step => step._id.Equals(oneStep._id));
        }

        private void ButtonOKClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            var selected = StepsTable.SelectedItem as Step;
            if (selected == null)
            {
                MessageBox.Show("Please select one row first, then delete work.");
                return;
            }
            var source = StepsTable.ItemsSource as ArrayList;
            if (source != null)
            {
                source.Remove(selected);
                StepsTable.Items.Refresh();
            }
        }

        private void ButtonDownClick(object sender, RoutedEventArgs e)
        {
            var selected = StepsTable.SelectedItem as Step;
            if (selected == null) return;
            var source = StepsTable.ItemsSource as ArrayList;
            if (source != null)
            {
                var count = source.Count;
                if (count < 2) return;
                var index = source.IndexOf(selected);
                if (index == count - 1) return; //last one, cannot move down anymore
                var temp = source[index + 1] as Step;
                source[index + 1] = selected;
                source[index] = temp;
                StepsTable.Items.Refresh();
            }
        }

        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            //ICollectionView view = CollectionViewSource.GetDefaultView(StepsTable.ItemsSource) as ListCollectionView;
            //if (view == null) return;
            //view.MoveCurrentToPrevious();
            //if (view.IsCurrentBeforeFirst)
            //    view.MoveCurrentToFirst();
            //e.Handled = true;
            //StepsTable.Items.Refresh();
            var selected = StepsTable.SelectedItem as Step;
            if (selected == null) return;
            var source = StepsTable.ItemsSource as ArrayList;
            if (source != null)
            {
                var count = source.Count;
                if (count < 2) return;
                var index = source.IndexOf(selected);
                if (index == 0) return; //first one, cannot move up anymore
                var temp = source[index - 1] as Step;
                source[index - 1] = selected;
                source[index] = temp;
                StepsTable.Items.Refresh();
            }
        }

        internal object Get()
        {
            var xSteps = XElement.Parse("<Steps />");
            var list = StepsTable.ItemsSource as ArrayList;
            if (list != null)
                foreach (Step step in list)
                {
                    xSteps.Add(step.ToXElement());
                }
            return xSteps.ToString();
        }

        internal void Set(string stepId)
        {
            ReloadButton.IsEnabled = true;
            _stepId = stepId;
        }

        internal void Set(string p, string userData)
        {
            var ds = Utilities.GetRawUserData(userData, Host).Keys;
            foreach (string d in ds)
            {
                if (Utilities.ReservedList.Contains(d))
                    continue;
                UserData.Add(d);
            }
            var steps = Utilities.GetStepsList(p, Actions, Host);
            foreach (Step step in steps)
            {
                step.PossibleData = UserData;
            }
            StepsTable.ItemsSource = steps;
        }

        private void ExtractAll(object sender, RoutedEventArgs e)
        {
            var xData = initDataXElement();
            foreach (Step row in StepsTable.Items)
            {
                ExtractOneRow(xData, row);
            }
            Clipboard.SetText(xData.ToString(SaveOptions.None));
            MessageBox.Show("Extracted data has been put to the clipboard, you can paste it to proper editor and update them");
        }

        private static void ExtractOneRow(XElement xData, Step row)
        {
            if ("Click;Close;Start;Existed,NotExisted".Contains(row.Action))
                return;
            var key = row.Data;
            if (string.IsNullOrEmpty(key))
                key = row.Action + row.UIObject;
            xData.SetAttributeValue(key, row.DefaultData);
        }

        private static XElement initDataXElement()
        {
            var data = new XElement("Datum");
            data.SetAttributeValue("Name", "New Data");
            data.SetAttributeValue("_type", "Datum");
            data.SetAttributeValue("Description", "Extracted Data");
            data.SetAttributeValue("_id", Guid.NewGuid().ToString());
            data.SetAttributeValue("Created", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            data.SetAttributeValue("Updated", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            data.SetAttributeValue("_parentId", Configuration.Settings("DataRoot"));
            return data;
        }

        private void ExtractEnabled(object sender, RoutedEventArgs e)
        {
            var xData = initDataXElement();
            foreach (Step row in StepsTable.Items)
            {
                if(row.Enable)
                    ExtractOneRow(xData, row);

            }
            Clipboard.SetText(xData.ToString(SaveOptions.None));
            MessageBox.Show("Extracted data has been put to the clipboard, you can paste it to proper editor and update them");
        }

        private void ExtractDefault(object sender, RoutedEventArgs e)
        {
            var xData = initDataXElement();
            foreach (Step row in StepsTable.Items)
            {
                if(!string.IsNullOrEmpty(row.DefaultData))
                    ExtractOneRow(xData, row);
            }
            Clipboard.SetText(xData.ToString(SaveOptions.None));
            MessageBox.Show("Extracted data has been put to the clipboard, you can paste it to proper editor and update them");
        }
    }
}