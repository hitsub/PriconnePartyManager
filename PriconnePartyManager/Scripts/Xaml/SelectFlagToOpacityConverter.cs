using System;
using System.Globalization;
using System.Windows.Data;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class SelectFlagToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = (bool?) value ?? true;
            if (flag)
            {
                return 0.7;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}