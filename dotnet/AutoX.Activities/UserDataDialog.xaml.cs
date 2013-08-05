#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.Activities
{
    /// <summary>
    ///   Interaction logic for UserDataDialog.xaml
    /// </summary>
    public partial class UserDataDialog
    {
        public List<UserData> Data = new List<UserData>();

        public IHost Host = HostManager.GetInstance().GetHost();

        public UserDataDialog()
        {
            InitializeComponent();
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }

        public void Set(string userDataIds)
        {
            Data = Utilities.GetUserData(userDataIds, Host);
            var filtered = Data.Where(userData => !Utilities.ReservedList.Contains(userData.Name)).ToList();
            UserDataTable.ItemsSource = filtered;
        }

        public string Get()
        {
            Data = UserDataTable.ItemsSource as List<UserData>;
            return Data == null ? "" : Data.Aggregate("", (current, userData) => current + (userData.DataSetId + ";"));
        }

        private void ButtonOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ButtonDeleteClick(object sender, RoutedEventArgs e)
        {
            var selected = UserDataTable.SelectedItem as UserData;
            if (selected == null)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }
            var selectedId = selected.DataSetId;
            var userDatas = UserDataTable.ItemsSource as List<UserData>;
            if (userDatas != null)
            {
                for (var i = userDatas.Count - 1; i > -1; i--)
                {
                    var ud = userDatas[i];
                    if (ud == null)
                        continue;
                    if (ud.DataSetId.Equals(selectedId))
                        userDatas.RemoveAt(i);
                }
                UserDataTable.ItemsSource = userDatas;
            }
            var ids = Get();
            Set(ids);
            UserDataTable.Items.Refresh();
        }
    }
}