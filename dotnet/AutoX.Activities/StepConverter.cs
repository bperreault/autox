// Hapa Project, CC
// Created @2012 07 16 18:49
// Last Updated  by Huang, Jien @2012 07 16 18:49

#region

using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace AutoX.Activities
{
    public class StepConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var options = new ArrayList {"Check", "Enter", "Click"};
            var step = new Step
                {
                    UIId = 123,
                    UIObject = "UIObject",
                    Data = "testdata",
                    Action = options[0].ToString(),
                    PossibleAction = options,
                    Enable = true
                };
            if (value == null)
            {
                return step;
            }

            return step;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}