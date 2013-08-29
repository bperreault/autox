#region

// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

using System.Linq;

#region

using System;
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
            return userDataList.Where(userData => !Utilities.ReservedList.Contains(userData.Name)).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}