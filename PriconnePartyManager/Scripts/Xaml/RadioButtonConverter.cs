using System;
using System.Globalization;
using System.Windows.Data;

namespace PriconnePartyManager.Scripts.Xaml
{
    [ValueConversion(typeof(System.Enum), typeof(bool))]
    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            return value.ToString() == parameter.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Binding.DoNothing;
            if ((bool)value)
            {
                return System.Enum.Parse(targetType, parameter.ToString());
            }
            return Binding.DoNothing;
        }
    }
}