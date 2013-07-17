#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using AutoX.Basic;

#endregion

#endregion

namespace AutoX.Activities
{
    public class DatumConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rawData = value as string;
            var userDataList = Utilities.GetUserData(rawData, HostManager.GetInstance().GetHost());
            var retList = new List<UserData>();
            foreach (UserData userData in userDataList)
            {
                if (!Utilities.ReservedList.Contains(userData.Name))
                    retList.Add(userData);
            }
            return retList;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}