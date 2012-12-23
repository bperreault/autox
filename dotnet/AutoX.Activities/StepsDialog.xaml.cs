// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using AutoX.Basic;

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
        protected ArrayList UserData = new ArrayList();

        public StepsDialog()
        {
            InitializeComponent();
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
                int count = source.Count;
                if (count < 2) return;
                int index = source.IndexOf(selected);
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
                int count = source.Count;
                if (count < 2) return;
                int index = source.IndexOf(selected);
                if (index == 0) return; //first one, cannot move up anymore
                var temp = source[index - 1] as Step;
                source[index - 1] = selected;
                source[index] = temp;
                StepsTable.Items.Refresh();
            }
        }

        internal object Get()
        {
            XElement xSteps = XElement.Parse("<Steps />");
            var list = StepsTable.ItemsSource as ArrayList;
            if (list != null)
                foreach (Step step in list)
                {
                    xSteps.Add(step.ToXElement());
                }
            return xSteps.ToString();
        }

        internal void Set(string p, string userData)
        {
            Dictionary<string, UserData>.KeyCollection ds = Utilities.GetRawUserData(userData, Host).Keys;
            foreach (string d in ds)
            {
                UserData.Add(d);
            }
            ArrayList steps = Utilities.GetStepsList(p, Actions, Host);
            foreach (Step step in steps)
            {
                step.PossibleData = UserData;
            }
            StepsTable.ItemsSource = steps;
        }
    }
}