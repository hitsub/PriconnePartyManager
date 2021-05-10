using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class EnumDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            var attr = field.GetCustomAttribute<DescriptionAttribute>(false);
            if (attr != null) {
                return attr.Description;
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}