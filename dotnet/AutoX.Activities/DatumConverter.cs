// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Globalization;
using System.Windows.Data;
using AutoX.Basic;

#endregion

namespace AutoX.Activities
{
    public class DatumConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rawData = value as string;
            var ret = Utilities.GetUserData(rawData, HostManager.GetInstance().GetHost());

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}