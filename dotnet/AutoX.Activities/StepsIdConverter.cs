using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using AutoX.Basic;
using AutoX.DB;
using System.Xml.Linq;

namespace AutoX.Activities
{
    public class StepsIdConverter : IValueConverter
    {
        private readonly ArrayList _options = Configuration.GetSupportedAction();

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var textValue = value as string;
            if (string.IsNullOrEmpty(textValue))
                return null;
            string content = Data.Read(textValue).GetAttributeValue("Content");
            if (string.IsNullOrEmpty(content))
                return null;
            var steps = XElement.Parse(content).GetAttributeValue("Steps");
            var ret = Utilities.GetStepsList(steps, _options, HostManager.GetInstance().GetHost());

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        
    }
}
