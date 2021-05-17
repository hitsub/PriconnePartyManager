#nullable enable
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class StringIntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string?) value ?? "0";
            if (int.TryParse(text, out var intValue))
            {
                return intValue == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}