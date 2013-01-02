// Hapa Project, CC
// Created @2012 08 24 09:25
// Last Updated  by Huang, Jien @2012 08 24 09:25

#region

using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using AutoX.Basic;

#endregion

namespace AutoX.Activities
{
    public class StepsConverter : IValueConverter
    {
        private readonly ArrayList _options = Configuration.GetSupportedAction();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var textValue = value as string;
            var ret = Utilities.GetStepsList(textValue, _options, HostManager.GetInstance().GetHost());

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}